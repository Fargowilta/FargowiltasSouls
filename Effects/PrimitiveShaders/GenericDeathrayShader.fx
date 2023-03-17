sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
matrix uWorldViewProjection;
float4 uShaderSpecificData;

// These must be set or this will not work properly.
float stretchAmount;
float scrollSpeed;
float uColorFadeScaler;
bool useFadeIn;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
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

// This is a generic laser shader, that fades into a specified color into the middle and reads a fademap as uImage1. This can be used for any laser,
// with the parameters above set to the desired values to change how it looks without needing to create a new shader.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    
	// Get the fade map pixel.
    float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.x * stretchAmount - uTime * scrollSpeed), coords.y));
    
    // Calcuate the grayscale version of the pixel and use it as the opacity.
    float opacity = fadeMapColor.r;
    float4 colorCorrected = lerp(color, float4(uColor, 1), opacity * uColorFadeScaler);

    // Fade out at the end of the streak.
    if (coords.x < 0.075 && useFadeIn)
        opacity *= pow(coords.x / 0.075, 6);
    if (coords.x > 0.83 && useFadeIn)
        opacity *= pow(1 - (coords.x - 0.83) / 0.17, 10);
    
    return colorCorrected * opacity * (uColorFadeScaler * 1.3);
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}