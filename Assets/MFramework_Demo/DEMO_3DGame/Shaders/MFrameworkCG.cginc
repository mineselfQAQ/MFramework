#ifndef MFRAMEWORKCG
#define MFRAMEWORKCG

float3 GammaToLinear(float3 gammaColor)
{
    return pow(gammaColor, 2.2);
}
float4 GammaToLinear(float4 gammaColor)
{
    return float4(pow(gammaColor.rgb, 2.2), gammaColor.a);
}

float3 LinearToGamma(float3 linearColor)
{
    return pow(linearColor, 1.0 / 2.2);
}
float3 LinearToGamma(float4 linearColor)
{
    return float4(pow(linearColor.rgb, 1.0 / 2.2), linearColor.a);
}

#endif