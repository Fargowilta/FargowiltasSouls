using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
    public class SmallSparkle : Particle
    {
        public override string AtlasTextureName => "FargowiltasSouls.SmallSparkle";
        public Color BloomColor;
        public readonly bool UseBloom;

        public static int FadeTime => 15;

        public SmallSparkle(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, float rotation = 0f, float rotationSpeed = 0f, bool useBloom = true, Color? bloomColor = null)
        {
            Position = worldPosition;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = new(scale);
            Lifetime = lifetime;
            Rotation = rotation;
            RotationSpeed = rotationSpeed;
            UseBloom = useBloom;
            bloomColor ??= Color.White;
            BloomColor = bloomColor.Value;
        }

        public override void Update()
        {
            Opacity = Utils.GetLerpValue(Lifetime, Lifetime - FadeTime, Time, true);
            Velocity *= 0.99f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (UseBloom)
            {
                AtlasTexture bloomTexture = AtlasManager.GetTexture("FargowiltasSouls.Bloom");
                spriteBatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, 0f, null, Scale * 0.08f, SpriteEffects.None);
            }

            spriteBatch.Draw(Texture, Position - Main.screenPosition, null, DrawColor with { A = 0 } * Opacity, Rotation, null, Scale, SpriteEffects.None);

            if (UseBloom)
                spriteBatch.Draw(Texture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, Rotation, null, Scale, SpriteEffects.None);
        }
    }
}