using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
    public class Bubble : Particle
    {
        public override string AtlasTextureName => "FargowiltasSouls.Bubble";

        public readonly bool UseBloom;

        public readonly float BaseOpacity = 1;
        public override int FrameCount => 2;
        public int CurrentFrame = 0;
        public override BlendState BlendState => BlendState.NonPremultiplied;
        public Bubble(Vector2 worldPosition, Vector2 velocity, float scale, int lifetime, float baseOpacity = 1, float rotation = 0f, float rotationSpeed = 0f)
        {
            Position = worldPosition;
            Velocity = velocity;
            DrawColor = Color.White;
            Scale = new(scale);
            Lifetime = lifetime;
            Rotation = rotation;
            RotationSpeed = rotationSpeed;
            UseBloom = false;

            BaseOpacity = baseOpacity;
            CurrentFrame = Main.rand.Next(FrameCount);
        }
        public override void Update()
        {
            Opacity = Utils.GetLerpValue(Lifetime, Lifetime - 20, Time, true);
            Velocity *= 0.99f;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            int height = Texture.Frame.Height / FrameCount;
            Frame = new(0, CurrentFrame * height, Texture.Frame.Width, height);

            Vector2 screenPos = Position - Main.screenPosition;

            spriteBatch.Draw(Texture, screenPos, Frame, DrawColor * Opacity * BaseOpacity, Rotation, null, Scale, SpriteEffects.None);
        }
    }
}
