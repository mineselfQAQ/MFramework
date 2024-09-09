#ifndef MFRAMEWORK_PROPERTIES
#define MFRAMEWORK_PROPERTIES

uniform sampler2D _BaseColorMap;
uniform sampler2D _MetallicMap;
uniform sampler2D _RoughnessMap;
uniform sampler2D _NormalMap;
uniform float _LightInt;
uniform fixed4 _BaseColor;
uniform float _Metallic;
uniform float _Roughness;
uniform float _BumpScale;

uniform sampler2D _AOMap;
uniform samplerCUBE _Cubemap;
uniform float _AmbientInt;
uniform float _CubemapInt;
uniform float _CubemapMip;
uniform fixed3 _CubemapColor;

uniform fixed4 _FresnelCol;
uniform float _FresnelPow;
uniform float _FresnelInt;

#endif