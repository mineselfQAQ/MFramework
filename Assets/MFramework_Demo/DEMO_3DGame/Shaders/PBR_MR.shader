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
            //��Ⱦ״̬
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            //��������ļ�����ָ��
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
			
            //��������
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
                
			//��������
            struct appdata
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
			
            //�������
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 wNormal : TEXCOORD0;
                float3 wPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3x3 tbn : TEXCOORD3;
            };

            //������BRDF��Cook-Torrance���淴���D/F/G��
            //D��---ʹ��Trowbridge-Reitz GGX
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

            //F��---ʹ��Fresnel-Schlick���Ʒ���������Ҫ�����F0
            float3 F_FresnelSchlick(float cosTheta, float3 F0)
            {
                return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
            }  

            //G��---ʹ��Schlick-GGX
            //Ҫע���м����ڱ�Geometry Obstruction����ͼ�����ӰGeometry Shadowing�������
            //���м����ڱ���Ҫʹ��nDir��vDir��������Ӱ��Ҫʹ��nDir��lDir
            //ʹ�õ���ʷ��˹��Smith's method�������
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
            //�����⼸��󣬾Ϳ��Լ���������PBR��
            float3 LightingPBR(float3 baseColor, float3 lightColor, float3 L, float3 N, float3 V, float metallic, float roughness, sampler2D baseMap, sampler2D metallicMap, sampler2D roughnessMap, sampler2D aoMap, float2 uv)
            {
                //׼������
                float3 H = normalize(L + V);
                float HdotV = saturate(dot(H, V));
                float NdotV = saturate(dot(N, V));
                float NdotL = saturate(dot(N, L));
                float3 var_baseMap = tex2D(baseMap, uv);
                float var_metallicMap = tex2D(metallicMap, uv);
                float var_roughnessMap = tex2D(roughnessMap, uv);
                float var_aoMap = tex2D(aoMap, uv);
                float3 base = baseColor * var_baseMap;//������
                float meta = metallic * var_metallicMap;//������
                float rough = roughness * var_roughnessMap;//�ֲڶ�

                //Fresnel���е�F0ֵ�ļ���
                //���ڷǽ�������ʹ��Ĭ�ϵ�0.04
                float3 F0 = float3(0.04, 0.04, 0.04);
                F0 = lerp(F0, base, meta);

                //D/F/G��ļ���
                float D = D_TRGGX(N, H, rough);
                float3 F = F_FresnelSchlick(HdotV, F0);
                float G = G_SchlickGGX(N, V, L, rough);

                //����BRDF�еĸ߹ⲿ��
                float3 nominator = D * F * G;//����
                float denominator = max(4 * NdotV * NdotL, 0.001);//��ĸ���ҷ�ֹ��0����
                float3 specularTerm = nominator / denominator;

                //���������̷��ص���һ�����������߱�����İٷֱ�
                //����˵�߹ⷴ��Ķ��پ���F�������������1-Ks
                //Ҫע��:F�Ѿ�������specularTerm������Ҫ�ٴγ���Ks��Ҳ����F
                float3 Ks = F;
                float3 Kd = 1 - Ks;
                Kd *= 1.0 - meta;//���ڽ������壬���������������䣬����metallicԽ��KdԽС

                //����BRDF�е������䲿��
                float3 diffuseTerm = Kd * base / UNITY_PI;
                //����radiance
                float3 radiance = lightColor * 1;//ֻ����̫�������������˥��ֵһֱΪ1
                //�������ս��---����radiance
                float3 Lo = (diffuseTerm + specularTerm) * radiance * NdotL;

                //����û�����IBL������˵ֱ��ʹ����򵥵Ļ�������ʽ
                float topMask = saturate(N.y);
                float bottomMask = saturate(-N.y);
                float sideMask = 1 - topMask - bottomMask;
                float3 ambientColor = base * ((unity_AmbientSky * topMask) +
                    (unity_AmbientEquator * sideMask) + (unity_AmbientGround * bottomMask));

                return (Lo + ambientColor) * var_aoMap;
            }

            //������ɫ��
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

            //ƬԪ��ɫ��
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