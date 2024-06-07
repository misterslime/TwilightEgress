sampler trailTexture : register(s1);
matrix uWorldViewProjection;

float time;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.TextureCoordinates.y = (output.TextureCoordinates.y - 0.5) / input.TextureCoordinates.z + 0.5;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 baseColor = input.Color;
    float2 baseCoords = input.TextureCoordinates;
    
    // Define the texture map and use it as opacity.
    float4 textureMap = tex2D(trailTexture, float2(frac(baseCoords.x + time), baseCoords.y));
    float opacity = textureMap.r;
    
    // Fade out at the edges of the trail to make a smooth effect.
    float exponent = lerp(3, 10, baseCoords.x);
    opacity = lerp(pow(sin(baseCoords.y * 3.141), exponent), opacity, baseCoords.x);

    // Fade out at the end of the trail.
    if (baseCoords.x > 0.7)
        opacity *= pow(1 - (baseCoords.x - 0.7) / 0.3, 6);
    return baseColor * opacity * 1.5;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
