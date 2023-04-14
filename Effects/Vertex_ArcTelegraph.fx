matrix WorldViewProjection;

struct VertexShaderInput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
};

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float outlineMultiplier = 1;
    if(input.TextureCoordinates.y > 0.95f)
        outlineMultiplier = 2;
    return input.Color * lerp(0.2, 0.8, input.TextureCoordinates.y) * 0.75f * outlineMultiplier;
}


technique Technique1
{
    pass VertexTelegraphPass
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();

    }
}