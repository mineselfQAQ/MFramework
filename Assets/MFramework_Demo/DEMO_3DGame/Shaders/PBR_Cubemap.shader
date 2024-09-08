Shader "MineselfShader/PBR/Cubemap"
{
    Properties
    {
        _AmbientInt("AmbientIntensity", Range(0, 1)) = 1

        [Header(Cubemap Settings)][Space(5)]
        _Cubemap("Cubemap", Cube) = ""{}
        _CubemapMip("CubemapMipmap", Range(0, 8)) = 0
        _CubemapInt("CubemapIntensity", Range(0, 1)) = 1
        _CubemapColor("CubemapColor", COLOR) = (1,1,1,1)

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

    CGINCLUDE
    #include "UnityCG.cginc"
    //额外包含文件编译指令
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
    #include "../../../MFramework/CGINC/MFrameworkCG.cginc"
			
    //变量申明
    float _AmbientInt;

    samplerCUBE _Cubemap;
    float _CubemapMip;
    float _CubemapInt;
    fixed3 _CubemapColor;

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

        float3 finalRGB = LightingPBR_BASE
            (_BaseColor, _LightColor0, lDir, nDir, vDir, _Metallic, _Roughness,
                _BaseColorMap, _MetallicMap, _RoughnessMap, _AOMap, _Cubemap, atten, i.uv,
                _AmbientInt, _CubemapMip, _CubemapInt, _CubemapColor);

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

        float3 finalRGB = LightingPBR_ADD
            (_BaseColor, _LightColor0, lDir, nDir, vDir, _Metallic, _Roughness,
                _BaseColorMap, _MetallicMap, _RoughnessMap, atten, i.uv);

        return float4(finalRGB * _Multipler, 1);
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
        // Pass
        // {
        //     //Pass Tags
        //     Tags
        //     {
        //         "LightMode"="ForwardAdd"
        //     }
        //     //渲染状态
        //     Blend One One
            
        //     CGPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment fragAdd
        //     #pragma multi_compile_fwdadd_fullshadows
        //     ENDCG
        // }
    }
}