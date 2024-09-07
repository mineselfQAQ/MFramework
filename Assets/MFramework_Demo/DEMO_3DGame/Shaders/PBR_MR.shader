Shader "MineselfShader/PBR/PBRBasedMR"
{
    Properties
    {
        [Header(PBR Settings)][Space(5)]
        _BaseColorMap("BaseColorMap", 2D) = "white"{}
        _MetallicMap("MetallicMap", 2D) = "white"{}
        _RoughnessMap("RoughnessMap", 2D) = "white"{}
        [Normal]_NormalMap("NormalMap", 2D) = "bump"{}
        _AOMap("AOMap", 2D) = "white"{}
        _Multipler("Multipler", Float) = 1.0
        _BaseColor("BaseColor", COLOR) = (1,1,1,1)
        _Metallic("Metallic", Range(0, 1)) = 0
        _Roughness("Roughness", Range(0, 1)) = 0.5
        _BumpScale("BumpScale", Float) = 1
    }
    SubShader
    {
        //SubShader Tags
		Tags{}
        Pass
        {
            //Pass Tags
            Tags{}
            //渲染状态
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            //额外包含文件编译指令
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
			
            //变量申明
            float _Multipler;
            sampler2D _BaseColorMap;
            sampler2D _MetallicMap;
            sampler2D _RoughnessMap;
            sampler2D _NormalMap;
            sampler2D _AOMap;
            fixed4 _BaseColor;
            float _Metallic;
            float _Roughness;
            float _BumpScale;
                
			//顶点输入
            struct appdata
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
			
            //顶点输出
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 wNormal : TEXCOORD0;
                float3 wPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3x3 tbn : TEXCOORD3;
            };

            //以下是BRDF中Cook-Torrance镜面反射的D/F/G项
            //D项---使用Trowbridge-Reitz GGX
            float D_TRGGX(float3 N, float3 H, float roughness)
            {
                float a      = roughness*roughness;
                float a2     = a*a;
                float NdotH  = max(dot(N, H), 0.0);
                float NdotH2 = NdotH*NdotH;

                float nom   = a2;
                float denom = (NdotH2 * (a2 - 1.0) + 1.0);
                denom = UNITY_PI * denom * denom;

                return nom / denom;
            }

            //F项---使用Fresnel-Schlick近似法，首先需要计算出F0
            float3 F_FresnelSchlick(float cosTheta, float3 F0)
            {
                return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
            }  

            //G项---使用Schlick-GGX
            //要注意有几何遮蔽Geometry Obstruction情况和几何阴影Geometry Shadowing两种情况
            //其中几何遮蔽需要使用nDir·vDir，几何阴影需要使用nDir·lDir
            //使用的是史密斯法Smith's method结合两者
            float GeometrySchlickGGX(float NdotV, float roughness)
            {
                float r = (roughness + 1.0);
                float k = (r*r) / 8.0;

                float nom   = NdotV;
                float denom = NdotV * (1.0 - k) + k;

                return nom / denom;
            }
            float G_SchlickGGX(float3 N, float3 V, float3 L, float roughness)
            {
                float NdotV = max(dot(N, V), 0.0);
                float NdotL = max(dot(N, L), 0.0);
                float ggx2  = GeometrySchlickGGX(NdotV, roughness);
                float ggx1  = GeometrySchlickGGX(NdotL, roughness);

                return ggx1 * ggx2;
            }
            //有了这几项后，就可以计算真正的PBR了
            float3 LightingPBR(float3 baseColor, float3 lightColor, float3 L, float3 N, float3 V, float metallic, float roughness, sampler2D baseMap, sampler2D metallicMap, sampler2D roughnessMap, sampler2D aoMap, float2 uv)
            {
                //准备参数
                float3 H = normalize(L + V);
                float HdotV = saturate(dot(H, V));
                float NdotV = saturate(dot(N, V));
                float NdotL = saturate(dot(N, L));
                float3 var_baseMap = tex2D(baseMap, uv);
                float var_metallicMap = tex2D(metallicMap, uv);
                float var_roughnessMap = tex2D(roughnessMap, uv);
                float var_aoMap = tex2D(aoMap, uv);
                float3 base = baseColor * var_baseMap;//主纹理
                float meta = metallic * var_metallicMap;//金属度
                float rough = roughness * var_roughnessMap;//粗糙度

                //Fresnel项中的F0值的计算
                //对于非金属物体使用默认的0.04
                float3 F0 = float3(0.04, 0.04, 0.04);
                F0 = lerp(F0, base, meta);

                //D/F/G项的计算
                float D = D_TRGGX(N, H, rough);
                float3 F = F_FresnelSchlick(HdotV, F0);
                float G = G_SchlickGGX(N, V, L, rough);

                //计算BRDF中的高光部分
                float3 nominator = D * F * G;//分子
                float denominator = max(4 * NdotV * NdotL, 0.001);//分母，且防止除0发生
                float3 specularTerm = nominator / denominator;

                //菲涅尔方程返回的是一个物体表面光线被反射的百分比
                //所以说高光反射的多少就是F，而漫反射就是1-Ks
                //要注意:F已经存在于specularTerm，不需要再次乘以Ks，也就是F
                float3 Ks = F;
                float3 Kd = 1 - Ks;
                Kd *= 1.0 - meta;//对于金属物体，根本不存在漫反射，所以metallic越大，Kd越小

                //计算BRDF中的漫反射部分
                float3 diffuseTerm = Kd * base / UNITY_PI;
                //计算radiance
                float3 radiance = lightColor * 1;//只考虑太阳光情况，所以衰减值一直为1
                //计算最终结果---出射radiance
                float3 Lo = (diffuseTerm + specularTerm) * radiance * NdotL;

                //由于没有添加IBL，所以说直接使用最简单的环境光形式
                float topMask = saturate(N.y);
                float bottomMask = saturate(-N.y);
                float sideMask = 1 - topMask - bottomMask;
                float3 ambientColor = base * ((unity_AmbientSky * topMask) +
                    (unity_AmbientEquator * sideMask) + (unity_AmbientGround * bottomMask));

                return (Lo + ambientColor) * var_aoMap;
            }

            //顶点着色器
            v2f vert (appdata v)
            {

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.wNormal = UnityObjectToWorldNormal(v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;

                float3 wNormal = UnityObjectToWorldNormal(v.normal);
                float3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 wBitangent = cross(wNormal, wTangent) * v.tangent.w * unity_WorldTransformParams.w;
                o.tbn = float3x3(wTangent, wBitangent, wNormal);

                return o;
            }

            //片元着色器
            fixed4 frag (v2f i) : SV_Target
            {
                float3 localNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
                localNormal.xy *= _BumpScale;
                float3 nDir = normalize(mul(localNormal, i.tbn));

                float3 vDir = normalize(UnityWorldSpaceViewDir(i.wPos));
                float3 lDir = normalize(UnityWorldSpaceLightDir(i.wPos));

                float3 baseRGB = LightingPBR
                    (_BaseColor, _LightColor0, lDir, nDir, vDir, _Metallic, _Roughness,
                     _BaseColorMap, _MetallicMap, _RoughnessMap, _AOMap, i.uv);

                float3 finalRGB = baseRGB;

                return float4(finalRGB * _Multipler, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}