﻿sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float globalTime;
matrix uWorldViewProjection;

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

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
	coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    // Get the pixel of the fade map. What coords.x is being multiplied by determines
    // how many times the uImage1 is copied to cover the entirety of the prim. 2, 2
	float fadeMapBrightness = tex2D(uImage1, float2(frac(coords.x * 0.2 - globalTime * .5), coords.y)).r;
    
    // You determine what this is by simply fucking around with changing stuff and seeing how it changes,
    // until you get something that looks cool.s
    float bloomOpacity = lerp(pow(abs(sin(coords.y * 3.141)), lerp(1, 2, coords.x)), 0.01, coords.x);

    float opacity = 1;
    // Fade out at the top and bottom of the streak.
    float y = 0.5 - abs(coords.y - 0.5);
    if (y < 0.25)
        opacity *= pow(y / 0.5, 4);
    
    if (coords.x < 0.05)
        opacity *= pow(coords.x / 0.05, 2);
    if (coords.x > 0.7)
        opacity *= pow(1 - ((coords.x - 0.7) / 0.3), 2);
    return color * opacity * pow(abs(bloomOpacity), 5) * lerp(1, 6, fadeMapBrightness);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
