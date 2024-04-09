using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasSouls.Common.Graphics.Particles
{
    public abstract class BaseExpandingParticle : Particle
    {
        public override string AtlasTextureName => "FargowiltasSouls.Bloom";

        public readonly Vector2 StartScale;

        public readonly Vector2 EndScale;

        public Color BloomColor;

        public readonly bool UseBloom;


        /// <summary>
        /// The scale for a texture of size 100x100px.
        /// </summary>
        public virtual Vector2 DrawScale => Scale * 0.3f;

        public BaseExpandingParticle(Vector2 position, Vector2 velocity, Color drawColor, Vector2 startScale, Vector2 endScale, int lifetime, bool useExtraBloom = false, Color? extraBloomColor = null)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = StartScale = startScale;
            EndScale = endScale;
            Lifetime = lifetime;
            UseBloom = useExtraBloom;
            extraBloomColor ??= Color.White;

            BloomColor = extraBloomColor.Value;
        }

        public sealed override void Update()
        {
            Opacity = MathHelper.Lerp(1f, 0f, FargoSoulsUtil.SineInOut(LifetimeRatio));
            Scale = Vector2.Lerp(StartScale, EndScale, FargoSoulsUtil.SineInOut(LifetimeRatio));
        }

		public sealed override void Draw(SpriteBatch spriteBatch)
		{
            spriteBatch.Draw(Texture, Position - Main.screenPosition, Frame, DrawColor with { A = 0 } * Opacity, Rotation, null, DrawScale, Direction.ToSpriteDirection());

			if (UseBloom)
				spriteBatch.Draw(Texture, Position - Main.screenPosition, null, BloomColor with { A = 0 } * 0.4f * Opacity, Rotation, null, DrawScale * 0.66f, Direction.ToSpriteDirection());
		}
	}
}
