sampler maskTexture : register(s0);
sampler noiseTexture : register(s1);
sampler distortionTexture : register(s2);

float globalTime;
float galaxyOpacity;
float3 galaxyColor;
float fadeOutMargin;
float pixelationFactor;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate the coords.
    //coords = round(coords / pixelationFactor) * pixelationFactor;
    
    // Calculate distortion for the main noise texture.
    float2 distortionCoords = float2(coords.x + globalTime * 0.007, coords.y + globalTime * -0.002);
    distortionCoords *= 0.4;
    float distortion = tex2D(distortionTexture, distortionCoords).r;
    
    // Create the noise map with the distortion applied.
    float2 noiseCoords = coords + distortion * 0.23;
    noiseCoords.x -= globalTime * 0.006;
    noiseCoords *= 0.3;
    float4 noise = tex2D(noiseTexture, noiseCoords);
    
    // Mask the noise map onto another texture.
    float2 maskCoords = float2(coords.x + globalTime * 0.003, coords.y + globalTime * 0.001);
    float4 mask = tex2D(maskTexture, coords);
    float fadeOutRegion = 1 - (coords.y - fadeOutMargin) / (1 - fadeOutMargin);
    return mask * 2 * noise * float4(galaxyColor, galaxyOpacity) * pow(fadeOutRegion, 2);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

