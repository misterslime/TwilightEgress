sampler blurTexture : register(s0);

float textureSize;
float2 direction;

const float weight[] = { 0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162 };

float4 GaussianBlur(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float offset = 1 / textureSize;
    float3 color = tex2D(blurTexture, coords) * weight[0];
    float3 resultantColor = float3(0, 0, 0);

    for (int i = 0; i < 5; i++)
    {
        resultantColor += tex2D(blurTexture, coords + float2(direction.x * offset, direction.y * offset)) * weight[i] +
                tex2D(blurTexture, coords - float2(direction.x * offset, direction.y * offset)) * weight[i];
    }
    
    color += resultantColor;
    return float4(color, 1);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 GaussianBlur();
    }
}
