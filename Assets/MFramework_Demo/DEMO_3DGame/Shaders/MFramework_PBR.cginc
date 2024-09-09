#ifndef MFRAMEWORK_PBR
#define MFRAMEWORK_PBR

//������BRDF��Cook-Torrance���淴���D/F/G��
//D��---ʹ��Trowbridge-Reitz GGX
float D_TRGGX(float3 N, float3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
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
    float k = (r * r) / 8.0;

    float nom = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}
float G_SchlickGGX(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

float3 CalculateDirTerm(float3 baseColor, float3 lightColor, float3 L, float3 N, float3 V, sampler2D baseMap, sampler2D metallicMap, sampler2D roughnessMap, float atten, float2 uv, float metallic = 0.5, float roughness = 0.5, float intensity = 1)
{
    //׼������
    float3 H = normalize(L + V);
    float HdotV = saturate(dot(H, V));
    float NdotV = saturate(dot(N, V));
    float NdotL = saturate(dot(N, L));
    
    float3 var_baseMap = tex2D(baseMap, uv);
    float var_metallicMap = tex2D(metallicMap, uv);
    float var_roughnessMap = tex2D(roughnessMap, uv);
    
    float3 base = baseColor * var_baseMap; //������
    float meta = metallic * var_metallicMap; //������
    float rough = roughness * var_roughnessMap; //�ֲڶ�

    //Fresnel���е�F0ֵ�ļ���
    //���ڷǽ�������ʹ��Ĭ�ϵ�0.04
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, base, meta);

    //D/F/G��ļ���
    float D = D_TRGGX(N, H, rough);
    float3 F = F_FresnelSchlick(HdotV, F0);
    float G = G_SchlickGGX(N, V, L, rough);

     //����BRDF�еĸ߹ⲿ��
    float3 nominator = D * F * G; //����
    float denominator = max(4 * NdotV * NdotL, 0.001); //��ĸ���ҷ�ֹ��0����
    float3 specularTerm = nominator / denominator;

    //���������̷��ص���һ�����������߱�����İٷֱ�
    //����˵�߹ⷴ��Ķ��پ���F�������������1-Ks
    //Ҫע��:F�Ѿ�������specularTerm������Ҫ�ٴγ���Ks��Ҳ����F
    float3 Ks = F;
    float3 Kd = 1 - Ks;
    Kd *= 1.0 - meta; //���ڽ������壬���������������䣬����metallicԽ��KdԽС

    //����BRDF�е������䲿��
    float3 diffuseTerm = Kd * base / UNITY_PI;
    //����radiance
    float3 radiance = lightColor * atten;//����ʵ�����ȷ��˥���������Ӱ
    //�������ս��---����radiance
    float3 Lo = (diffuseTerm + specularTerm) * radiance * NdotL;
    
    return Lo * intensity;
}

float3 CalculateAmbient(float3 N, float3 base, float intensity = 1)
{
    float topMask = saturate(N.y);
    float bottomMask = saturate(-N.y);
    float sideMask = 1 - topMask - bottomMask;
    float3 ambientColor = (unity_AmbientSky * topMask) +
                    (unity_AmbientEquator * sideMask) + (unity_AmbientGround * bottomMask);
    
    return base * ambientColor * intensity;
}
float3 CalculateCubemap(float3 RV, float3 base, float3 cubemap, int lod = 1, float intensity = 1, float3 color = float3(1, 1, 1))
{
    return base * cubemap * intensity * color;
}
float3 CalculateIndirTerm(float3 baseColor, float3 N, float3 V, sampler2D baseMap, sampler2D aoMap, samplerCUBE cubemap, float2 uv, float ambientInt = 1, int lod = 1, float cubemapInt = 1, float3 cubemapColor = float3(1,1,1))
{
    float3 RV = normalize(reflect(-V, N));
    
    float3 var_baseMap = tex2D(baseMap, uv);
    float3 var_cubemap = texCUBElod(cubemap, float4(RV, lod));
    float var_aoMap = tex2D(aoMap, uv);
    
    float3 base = baseColor * var_baseMap;
    
    float3 ambientTerm = CalculateAmbient(N, base, ambientInt);
    float3 cubemapTerm = CalculateCubemap(RV, base, var_cubemap, lod, cubemapInt, cubemapColor);
    
    float3 inDirTerm = (ambientTerm + cubemapTerm) * var_aoMap;
    
    return inDirTerm;
}

float3 LightingPBR_BASE(float3 baseColor, float3 lightColor, float3 L, float3 N, float3 V, sampler2D baseMap, sampler2D metallicMap, sampler2D roughnessMap, sampler2D aoMap, samplerCUBE cubemap, float atten, float2 uv, float metallic = 0.5, float roughness = 0.5, float lightInt = 1, float ambientInt = 1, int lod = 1, float cubemapInt = 1, float3 cubemapColor = float3(1, 1, 1))
{
    float3 dirTerm = CalculateDirTerm(baseColor, lightColor, L, N, V, baseMap, metallicMap, roughnessMap, atten, uv, metallic, roughness, lightInt);
#ifdef UNITY_COLORSPACE_GAMMA
    dirTerm = LinearToGamma(dirTerm);
#endif
    float3 inDirTerm = CalculateIndirTerm(baseColor, N, V,baseMap, aoMap, cubemap, uv, ambientInt, lod, cubemapInt, cubemapColor);
    
    return dirTerm + inDirTerm;
}
float3 LightingPBR_ADD(float3 baseColor, float3 lightColor, float3 L, float3 N, float3 V, sampler2D baseMap, sampler2D metallicMap, sampler2D roughnessMap, float atten, float2 uv, float metallic = 0.5, float roughness = 0.5, float lightInt = 1)
{
    float3 dirTerm = CalculateDirTerm(baseColor, lightColor, L, N, V, baseMap, metallicMap, roughnessMap, atten, uv, metallic, roughness, lightInt);
#ifdef UNITY_COLORSPACE_GAMMA
    dirTerm = LinearToGamma(dirTerm);
#endif

    return dirTerm;
}
#endif