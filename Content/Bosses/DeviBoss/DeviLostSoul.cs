using FargowiltasSouls.Assets.ExtraTextures;


using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
	public class DeviLostSoul : ModProjectile, IPixelatedPrimitiveRenderer
	{

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lost Soul");
			ProjectileID.Sets.TrailingMode[Type] = 1;
			ProjectileID.Sets.TrailCacheLength[Type] = 20;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.LostSoulHostile);
			AIType = ProjectileID.LostSoulHostile;
			Projectile.timeLeft = 300;
			CooldownSlot = 1;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<Buffs.Masomode.HexedBuff>(), 240);
			target.AddBuff(ModContent.BuffType<Buffs.Masomode.ReverseManaFlowBuff>(), 600);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}

		public static Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.DarkSlateGray, Color.Transparent, completionRatio) * 0.7f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;

			SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Color color26 = lightColor;
			color26 = Projectile.GetAlpha(color26);

			Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
			return false;
		}
		public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
		{
			ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
			FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
			PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 25);
		}
	}
}