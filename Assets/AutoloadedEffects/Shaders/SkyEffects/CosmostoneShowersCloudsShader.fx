sampler maskTexture : register(s0);
sampler noiseTetxure : register(s1);
sampler distortionTexture : register(s2);
sampler erosionTexture : register(s3);

float globalTime;
float2 textureSize;
float cloudOpacity;
float erosionStrength;
float fadeOutMargin;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelation.
    float2 pixelSize = 1.1 / textureSize;
    coords = round(coords / pixelSize) * pixelSize;
    
    // Calculate distortion for the main noise texture.
    float2 distortionCoords = float2(coords.x - globalTime * 0.02, coords.y + globalTime * -0.001);
    distortionCoords *= 0.2;
    float distortion = tex2D(distortionTexture, distortionCoords).r;
    
    // Apply erosion to the noise coordinates.
    float2 erosionCoords = float2(coords.x - globalTime * 0.025, coords.y - globalTime * 0.003);
    erosionCoords *= float2(0.5, 1);
    float4 erosion = tex2D(erosionTexture, erosionCoords + distortion * 0.32);
    float erosionLevel = (1 - erosionStrength) / erosionStrength;
    
    // Create two noise maps with the distortion applied.
    float2 cloudCoords = coords + distortion * 0.28;
    cloudCoords.x -= globalTime * 0.008;
    cloudCoords *= float2(0.5, 1);
    float4 clouds = tex2D(noiseTetxure, cloudCoords);
    
    // Mask the clouds over a primary texture.
    float2 maskCoords = float2(coords.x - globalTime * 0.01, coords.y + globalTime * 0.006);
    maskCoords *= float2(0.5, 1);
    float4 mask = tex2D(maskTexture, maskCoords + distortion * 0.18);

    // Fade out and erode the coordinates.
    float fadeOutRegion = pow(coords.y, 1.8) * smoothstep(1, fadeOutMargin, coords.y + erosion.g * erosionStrength);
    return (mask * 15 * clouds * fadeOutRegion * cloudOpacity) * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
