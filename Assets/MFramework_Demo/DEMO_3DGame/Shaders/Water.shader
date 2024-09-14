Shader "MineselfShader/Common/Water"
{
    Properties
    {
        [NoScaleOffset]_BaseMap("BaseMap", 2D) = "white"{}
        [NoScaleOffset]_NoiseMap("NoiseMap", 2D) = "white"{}
        _Color("Color", COLOR) = (1,1,1,1)
        _MainParams("XY:MainScale ZW:MainSpeed", vector) = (1,1,0,0)
        _Noise1Params("X:Noise1Scale Y:Noise1Intensity ZW:Noise1Speed", vector) = (1,1,0,0)
        _Noise2Params("X:Noise2Scale Y:Noise2Intensity ZW:Noise2Speed", vector) = (1,1,0,0)
        _Distortion("Distortion", Range(0, 1)) = 1

        [Header(RenderState Settings)][Space(5)]
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlendMode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlendMode", Float) = 10
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    //��������ļ�����ָ��
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
			
    //��������
    sampler2D _BaseMap;
    sampler2D _NoiseMap;
    fixed4 _Color;
    float4 _MainParams;
    float4 _Noise1Params;
    float4 _Noise2Params;
    float _Distortion;

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
        SHADOW_COORDS(3)
    };

    //������ɫ��
    v2f vert (appdata v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);

        o.wNormal = UnityObjectToWorldNormal(v.normal);
        o.wPos = mul(unity_ObjectToWorld, v.vertex);
        o.uv = v.uv;

        TRANSFER_SHADOW(o);

        return o;
    }

    //ƬԪ��ɫ��---ForwardBase
    fixed4 frag (v2f i) : SV_Target
    {
        float3 nDir = normalize(i.wNormal);
        float3 vDir = normalize(UnityWorldSpaceViewDir(i.wPos));
        float3 lDir = normalize(UnityWorldSpaceLightDir(i.wPos));

        //���2��uv---ͨ��һ������ͼ����
        float2 maskUV1 = i.uv * _Noise1Params.x - frac(_Time.x * _Noise1Params.zw);
        float2 maskUV2 = i.uv * _Noise2Params.x - frac(_Time.x * _Noise2Params.zw);
                
        //uv����
        float noiseMask1 = tex2D(_NoiseMap, maskUV1);
        float noiseMask2 = tex2D(_NoiseMap, maskUV2);
        //�ϲ�����ͼ
        float finalMask = noiseMask1 * _Noise1Params.y + noiseMask2 * _Noise2Params.y;

        //ͨ������ͼ�ı�������uv
        float uvBias = finalMask * _Distortion;
        float2 mainUV = i.uv * _MainParams.xy - frac(_Time.x * _MainParams.zw) + uvBias;

        float3 baseRGB = tex2D(_BaseMap, mainUV) * _Color;

        float3 finalRGB = baseRGB;

        return float4(finalRGB, _Color.a);
    }
    //ƬԪ��ɫ��---ForwardAdd
    fixed4 fragAdd (v2f i) : SV_Target
    {
        float3 nDir = normalize(i.wNormal);
        float3 lDir = normalize(UnityWorldSpaceLightDir(i.wPos));
        float3 rlDir = normalize(reflect(-lDir, nDir));
        float3 vDir = normalize(UnityWorldSpaceViewDir(i.wPos));
                
        float3 diffuse = _LightColor0 * saturate(dot(lDir, nDir));
        float3 specular = _LightColor0 * pow(saturate(dot(rlDir, vDir)), 30);

        //�������˥��
        UNITY_LIGHT_ATTENUATION(atten, i, i.wPos.xyz);

        float3 finalRGB = (diffuse + specular) * atten;
        
        return float4(finalRGB, 1);
    }
    ENDCG

    SubShader
    {
        //SubShader Tags
		Tags{"Queue" = "Transparent+1" "RenderType" = "Transparent"}//͸�������к���Ⱦ(��֤������ˮ��)
        //Pass1---ForwardBase
        Pass
        {
            //Pass Tags
            Tags
            {
                "LightMode"="ForwardBase"
            }
            //��Ⱦ״̬
            Blend [_SrcBlend] [_DstBlend]
            Cull Off

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
            //��Ⱦ״̬
            Blend One One
            Cull Off
           
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragAdd
            #pragma multi_compile_fwdadd
            ENDCG
        }
    }
    Fallback "Diffuse"
}