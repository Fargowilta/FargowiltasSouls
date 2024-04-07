using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

// Base namespace for convinience
namespace FargowiltasSouls.Assets.ExtraTextures
{
    public static class FargosTextureRegistry
    {
        #region Additive Textures
        public static Asset<Texture2D> BlobBloomTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/BlobGlow");
        public static Asset<Texture2D> BloomTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/Bloom");
        public static Asset<Texture2D> BloomParticleTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/BloomParticle");
        public static Asset<Texture2D> BloomFlareTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/BloomFlare");
        public static Asset<Texture2D> DeviBorderTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/DeviBorder");
        public static Asset<Texture2D> HardEdgeRing => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/HardEdgeRing");
        public static Asset<Texture2D> SoftEdgeRing => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/AdditiveTextures/SoftEdgeRing");
        #endregion

        #region Misc Shader Textures
        public static Asset<Texture2D> DeviRingTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring1");
        public static Asset<Texture2D> DeviRing2Texture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring2");
        public static Asset<Texture2D> DeviRing3Texture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring3");
        public static Asset<Texture2D> DeviRing4Texture => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/MiscShaderTextures/Ring4");
        #endregion

        #region Noise
        public static Asset<Texture2D> PerlinNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/BlurryPerlinNoise");
        public static Asset<Texture2D> ColorNoiseMap => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/ColorNoiseMap");
        public static Asset<Texture2D> CrustyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/CrustyNoise");
        public static Asset<Texture2D> HarshNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/HarshNoise");
        public static Asset<Texture2D> HoneycombNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/HoneycombNoise");
        public static Asset<Texture2D> LessCrustyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/LessCrustyNoise");
        public static Asset<Texture2D> SmokyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/SmokyNoise");
        public static Asset<Texture2D> TurbulentNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/TurbulentNoise");
        public static Asset<Texture2D> WavyNoise => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Noise/WavyNoise");

        #endregion

        #region Trails
        public static Asset<Texture2D> DeviBackStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/DevBackStreak");
        public static Asset<Texture2D> DeviInnerStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/DevInnerStreak");
        public static Asset<Texture2D> FadedGlowStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/FadedGlowStreak");
        public static Asset<Texture2D> FadedStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/FadedStreak");
        public static Asset<Texture2D> FadedThinGlowStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/FadedThinGlowStreak");
        public static Asset<Texture2D> GenericStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/GenericStreak");
        public static Asset<Texture2D> MagmaStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/MagmaStreak");
        public static Asset<Texture2D> MutantStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/MutantStreak");
        public static Asset<Texture2D> WillStreak => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Trails/WillStreak");
        #endregion
    }
}
