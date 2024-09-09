#ifndef MFRAMEWORK_PBR
#define MFRAMEWORK_PBR

//以下是BRDF中Cook-Torrance镜面反射的D/F/G项
//D项---使用Trowbridge-Reitz GGX
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
    //准备参数
    float3 H = normalize(L + V);
    float HdotV = saturate(dot(H, V));
    float NdotV = saturate(dot(N, V));
    float NdotL = saturate(dot(N, L));
    
    float3 var_baseMap = tex2D(baseMap, uv);
    float var_metallicMap = tex2D(metallicMap, uv);
    float var_roughnessMap = tex2D(roughnessMap, uv);
    
    float3 base = baseColor * var_baseMap; //主纹理
    float meta = metallic * var_metallicMap; //金属度
    float rough = roughness * var_roughnessMap; //粗糙度

    //Fresnel项中的F0值的计算
    //对于非金属物体使用默认的0.04
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, base, meta);

    //D/F/G项的计算
    float D = D_TRGGX(N, H, rough);
    float3 F = F_FresnelSchlick(HdotV, F0);
    float G = G_SchlickGGX(N, V, L, rough);

     //计算BRDF中的高光部分
    float3 nominator = D * F * G; //分子
    float denominator = max(4 * NdotV * NdotL, 0.001); //分母，且防止除0发生
    float3 specularTerm = nominator / denominator;

    //菲涅尔方程返回的是一个物体表面光线被反射的百分比
    //所以说高光反射的多少就是F，而漫反射就是1-Ks
    //要注意:F已经存在于specularTerm，不需要再次乘以Ks，也就是F
    float3 Ks = F;
    float3 Kd = 1 - Ks;
    Kd *= 1.0 - meta; //对于金属物体，根本不存在漫反射，所以metallic越大，Kd越小

    //计算BRDF中的漫反射部分
    float3 diffuseTerm = Kd * base / UNITY_PI;
    //计算radiance
    float3 radiance = lightColor * atten;//根据实际情况确定衰减且添加阴影
    //计算最终结果---出射radiance
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