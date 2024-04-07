using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
    public class BigSparkle : Particle
    {
        public override string AtlasTextureName => "FargowiltasSouls.BigSparkle";

        public readonly bool UseBloom;

        public Color BloomColor;

        public BigSparkle(Vector2 worldPosition, Vector2 velocity, Color drawColor, float scale, int lifetime, float rotation = 0f, float rotationSpeed = 0f, bool useBloom = true, Color? bloomColor = null)
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
            Opacity = Utils.GetLerpValue(Lifetime, Lifetime - 20, Time, true);
            Velocity *= 0.99f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (UseBloom)
            {
                AtlasTexture bloomTexture = AtlasManager.GetTexture("FargowiltasSouls.Bloom");
                spriteBatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f * Opacity, 0f, null, Scale * 0.17f, SpriteEffects.None);
            }


            spriteBatch.Draw(Texture, Position - Main.screenPosition, null, DrawColor with { A = 0 }, Rotation, null, Scale, SpriteEffects.None);

            if (UseBloom)
                spriteBatch.Draw(Texture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.5f, Rotation, null, Scale, SpriteEffects.None);
        }
    }
}
