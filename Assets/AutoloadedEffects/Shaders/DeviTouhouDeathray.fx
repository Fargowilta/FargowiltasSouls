﻿sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float globalTime;
float3 mainColor;

matrix uWorldViewProjection;

// These 3 are required if using primitives. They are the same for any shader being applied to them so you
// can copy paste them to any other primitive shaders and use the VertexShaderOutput for the input in the
// PixelShaderFunction.
// -->
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
// <--

// The X coordinate is the trail completion, the Y coordinate is the same as any other.
// This is simply how the primitive TextCoord is layed out in the C# code.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // This can also be copy pasted along with the above.
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
	coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    // Note: This doesn't actually work properly, as it causes the texture to wrap around on the Y axis.
    // The clamping basically stops it before it shrinks below the point where it wraps around. Change the max
    // clamp value if you copy this and it does weird artifacts at the top and bottom of the trail.
    // ->
	float y = sin(19.95 * globalTime - 5.2 * coords.x) * 0.2;

    float widthScale = float((y + (1 - coords.x * 0.25)) / 2);
    
    if (coords.x < 0.15)
        widthScale /= pow(coords.x / 0.15, 0.61);
    
    coords.y = ((coords.y - 0.5) * clamp(widthScale, 0, 2)) + 0.5;
    // <-
    
    // Get the pixel of the fade map. What coords.x is being multiplied by determines
    // how many times the uImage1 is copied to cover the entirety of the prim. 2, 2
	float4 outerColor = tex2D(uImage1, float2(frac(coords.x * 0.7 - globalTime * 1.5), coords.y));
    // Do the same, but for the second image
	float4 innerColor = tex2D(uImage2, float2(frac(coords.x * 0.7 - globalTime * 2.5), coords.y));
    // Use the secondary color for the inner.
    float4 innerColorFinal = lerp(color, float4(mainColor, 1), 0.85);
    
    float finalOpacity = max(outerColor.r, innerColor.r);
    float4 finalColor;
    
    // If the inner color is sufficiently faded in, lerp between the inner and outer to make them connect.
    if (innerColorFinal.a < 0.1)
    {
        float interpolant = innerColorFinal.a * 5;
        finalColor = lerp(color, innerColorFinal, interpolant);
    }
    // Else, just lerp between the two colors but make it brighter.
    else
        finalColor = lerp(color, innerColorFinal, innerColor.r) * 1.3;  
    
    //// Fade out at the top and bottom of the streak.
    if (coords.x < 0.05)
        finalOpacity *= pow(coords.x / 0.05, 3);
    if (coords.x > 0.8)
        finalOpacity *= pow(1 - (coords.x - 0.8) / 0.2, 3);
    
    return finalColor * finalOpacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}