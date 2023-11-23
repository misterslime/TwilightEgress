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

float distortionAmount;
float2 impactPosition;

// Based on this shader: https://www.shadertoy.com/view/wsdBWM

// May use this somewhere else. The effect is actually quite nice.
/*float2 PinchusionDistortion(float2 coords, float strength)
{
    // Creates a stretched distortion like effect at the edges of the screen.
    float2 screenUVCoordinates = coords;
    float uvArctan = atan2(screenUVCoordinates.x, screenUVCoordinates.y);
    float uvDot = dot(screenUVCoordinates, screenUVCoordinates);
    return 0.5 + float2(sin(uvArctan), cos(uvArctan)) * sqrt(uvDot) * (1.0 - strength * uvDot);
}*/

float3 ChromaticAbberation(sampler2D samplerTexture, float2 coords)
{
    // Calculate the distortion level of each color.
    float separation = distortionAmount * 0.3 / (distance(coords, impactPosition) + 1);
    float rColor = tex2D(samplerTexture, coords + float2(-0.832, -0.832) * separation).r;
    float gColor = tex2D(samplerTexture, coords + float2(0.832, -0.832) * separation).g;
    float bColor = tex2D(samplerTexture, coords + float2(0, 1) * separation).b;
    // Return the three calculations as one singular color.
    float3 returnColor = float3(rColor, gColor, bColor);
    return returnColor;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float3 color = ChromaticAbberation(uImage0, coords);
    return sampleColor * float4(color, 1.0);
}

technique Technique1
{
    pass ChromaAbberationPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
