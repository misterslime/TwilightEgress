sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float4 uShaderSpecificData;

float time;
float scrollSpeed;
float vignettePower;
float vignetteBrightness;

float4 primaryColor;
float4 secondaryColor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 noiseColor1 = tex2D(uImage1, coords * 2 + float2(time * -scrollSpeed, 0));
    float4 noiseColor2 = tex2D(uImage2, coords * 3 + float2(0, time * scrollSpeed));
    float4 noiseColor = noiseColor1 * primaryColor + noiseColor2 * secondaryColor;
    
    float vignetteInterpolant = saturate(pow(distance(coords, 0.5), vignettePower) * vignetteBrightness);
    return sampleColor * noiseColor * vignetteInterpolant;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
