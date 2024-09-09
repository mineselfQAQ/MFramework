Shader "MineselfShader/PBR/PBRBasedMR"
{
    Properties
    {
        [Header(PBR Settings___Lighting)][Space(5)]
        _BaseColorMap("BaseColorMap", 2D) = "white"{}
        _MetallicMap("MetallicMap", 2D) = "white"{}
        _RoughnessMap("RoughnessMap", 2D) = "white"{}
        [Normal]_NormalMap("NormalMap", 2D) = "bump"{}
        [Space(10)]
        _LightInt("LightIntensity", Range(0, 3)) = 1
        [Space(5)]
        _BaseColor("BaseColor", COLOR) = (1,1,1,1)
        _Metallic("Metallic", Range(0, 1)) = 0
        _Roughness("Roughness", Range(0, 1)) = 0.5
        _BumpScale("BumpScale", Float) = 1

        [Header(PBR Settings___Environment)][Space(5)]
        _AOMap("AOMap", 2D) = "white"{}
        _Cubemap("Cubemap", Cube) = ""{}
        [Space(10)]
        _AmbientInt("AmbientIntensity", Range(0, 3)) = 1
        _CubemapInt("CubemapIntensity", Range(0, 3)) = 0
        [Space(5)]
        _CubemapMip("CubemapMipmap", Range(0, 8)) = 0
        _CubemapColor("CubemapColor", COLOR) = (1,1,1,1)
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    //额外包含文件编译指令
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
    #include "MFrameworkCG.cginc"
    #include "MFramework_PBR.cginc"
    #include "MFramework_Properties.cginc"
			
    //变量申明(PBR部分在MFramework_Properties.cginc中)

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
        SHADOW_COORDS(6)
    };

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
                
        TRANSFER_SHADOW(o);

        return o;
    }

    //片元着色器---ForwardBase
    fixed4 frag (v2f i) : SV_Target
    {
        float3 localNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
        localNormal.xy *= _BumpScale;
        float3 nDir = normalize(mul(localNormal, i.tbn));

        float3 vDir = normalize(UnityWorldSpaceViewDir(i.wPos));
        float3 lDir = normalize(UnityWorldSpaceLightDir(i.wPos));
                
        UNITY_LIGHT_ATTENUATION(atten, i, i.wPos.xyz);

        float3 baseRGB = LightingPBR_BASE
            (_BaseColor, _LightColor0, lDir, nDir, vDir,
                _BaseColorMap, _MetallicMap, _RoughnessMap, _AOMap, _Cubemap, atten, i.uv,
                _Metallic, _Roughness, _LightInt,
                _AmbientInt, _CubemapMip, _CubemapInt, _CubemapColor);

        float3 finalRGB = baseRGB;

        return float4(finalRGB, 1);
    }
    //片元着色器---ForwardAdd
    fixed4 fragAdd (v2f i) : SV_Target
    {
        float3 localNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
        localNormal.xy *= _BumpScale;
        float3 nDir = normalize(mul(localNormal, i.tbn));

        float3 vDir = normalize(UnityWorldSpaceViewDir(i.wPos));
        float3 lDir = normalize(UnityWorldSpaceLightDir(i.wPos));
                
        UNITY_LIGHT_ATTENUATION(atten, i, i.wPos.xyz);

        float3 baseRGB = LightingPBR_ADD
            (_BaseColor, _LightColor0, lDir, nDir, vDir,
                _BaseColorMap, _MetallicMap, _RoughnessMap, atten, i.uv,
                _Metallic, _Roughness, _LightInt);

        float3 finalRGB = baseRGB;

        return float4(finalRGB, 1);
    }
    ENDCG

    SubShader
    {
        //SubShader Tags
		Tags{}
        //Pass1---ForwardBase
        Pass
        {
            //Pass Tags
            Tags
            {
                "LightMode"="ForwardBase"
            }
            //渲染状态
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            ENDCG
        }
        //Pass2---ForwardAdd
        Pass
        {
            //Pass Tags
            Tags
            {
                "LightMode"="ForwardAdd"
            }
            //渲染状态
            Blend One One
           
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragAdd
            #pragma multi_compile_fwdadd_fullshadows
            ENDCG
        }
    }
    Fallback "Diffuse"
}