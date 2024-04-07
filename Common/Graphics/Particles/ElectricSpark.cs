using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
    public class ElectricSpark : Particle
    {
        public override string AtlasTextureName => "FargowiltasSouls.ElectricSpark";
        public readonly bool UseBloom;
        public Color BloomColor;
        public override BlendState BlendState => BlendState.Additive;
        public static int FadeTime => 15;

        public ElectricSpark(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, bool useBloom = true, Color? bloomColor = null)
        {
            Position = worldPosition;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = new(scale);
            Lifetime = lifetime;
            UseBloom = useBloom;
            bloomColor ??= Color.White;
            BloomColor = bloomColor.Value;
        }

        public override void Update()
        {
            Velocity *= 0.95f;
            Scale *= 0.95f;
            Opacity = FargoSoulsUtil.SineInOut(1f - LifetimeRatio);
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            AtlasTexture lightningTexture = Texture;
            int lightningFrames = 5;

            Rectangle GetRandomLightningFrame()
            {
                int frameHeight = lightningTexture.Frame.Height / lightningFrames;
                int frame = Main.rand.Next(lightningFrames);
                return new(0, frameHeight * frame, lightningTexture.Frame.Width, frameHeight);
            }

            Frame = GetRandomLightningFrame();
            spriteBatch.Draw(lightningTexture, Position - Main.screenPosition, Frame, DrawColor * Opacity, Rotation, null, Scale, SpriteEffects.None);
            spriteBatch.Draw(lightningTexture, Position - Main.screenPosition, Frame, DrawColor * Opacity, Rotation, null, Scale * new Vector2(0.45f, 1f), SpriteEffects.None);

            if (UseBloom)
                spriteBatch.Draw(lightningTexture, Position - Main.screenPosition, Frame, BloomColor * 0.5f * Opacity, Rotation, null, Scale, SpriteEffects.None);
        }
    }
}