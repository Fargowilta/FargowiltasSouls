using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
	public class CoffinWaveShot : ModProjectile, IPixelatedPrimitiveRenderer
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Banished Baron Scrap");
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.light = 1;
			Projectile.timeLeft = 60 * 3;
		}

		public override void AI()
		{
			if (Projectile.localAI[0] < 12)
			{
				Projectile.localAI[0]++;
				Projectile.scale = MathHelper.Lerp(0, 1, Projectile.localAI[0] / 12);
			}

			ProjectileID.Sets.TrailCacheLength[Type] = 8;
			float rotStr = Projectile.ai[0] == 0 ? 0.06f : 0.03f;
			float rot = MathHelper.PiOver2 * rotStr * MathF.Sin(MathF.Tau * (Projectile.ai[1] / 50f));
			Projectile.velocity = Projectile.velocity.RotatedBy(rot);

			float accel = WorldSavingSystem.MasochistModeReal ? 1.02f : 1.016f;
			if (Projectile.velocity.Length() < 15f)
				Projectile.velocity *= accel;

			Projectile.ai[1]++;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 60 * 4);
		}
		private static readonly Color GlowColor = new(224, 196, 252, 0);
		public override bool PreDraw(ref Color lightColor)
		{
			//draw projectile
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Vector2 drawOffset = Projectile.rotation.ToRotationVector2() * (texture2D13.Width - Projectile.width) / 2;

			SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			/*
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = GlowColor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + drawOffset + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }
            */

			Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);


			return false;
		}
		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.25f;
			return MathHelper.SmoothStep(baseWidth, 0f, completionRatio);
		}

		public static Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(Color.Lerp(Color.Lerp(Color.MediumPurple, Color.DeepPink, 0.5f), GlowColor, 0.5f), GlowColor with { A = 100 } * 0.5f, completionRatio);
			//return Color.Lerp(GlowColor, Color.Transparent, completionRatio) * 0.7f;
		}


		public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
		{
			ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
			FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
			PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
		}
	}
}
