using FargowiltasSouls.Assets.ExtraTextures;


using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
	public class FrostfireballHostile : ModProjectile, IPixelatedPrimitiveRenderer
	{
		public override string Texture => "Terraria/Images/Projectile_253";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frostfireball");
			ProjectileID.Sets.TrailingMode[Type] = 1;
			ProjectileID.Sets.TrailCacheLength[Type] = 20;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
			Projectile.coldDamage = true;
		}

		public override void AI()
		{
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.localAI[0] = 1f;
				SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
			}

			//int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
			//Main.dust[index2].noGravity = true;
			//Main.dust[index2].velocity.X *= 0.3f;
			//Main.dust[index2].velocity.Y *= 0.3f;

			if (--Projectile.ai[1] > -60f && Projectile.ai[1] < 0f) //homing for 1sec, with delay
			{
				Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
				if (player != null && player.active && !player.dead)
				{
					Vector2 dist = player.Center - Projectile.Center;
					dist.Normalize();
					dist *= 8f;
					Projectile.velocity.X = (Projectile.velocity.X * 40 + dist.X) / 41;
					Projectile.velocity.Y = (Projectile.velocity.Y * 40 + dist.Y) / 41;
				}
				else
				{
					Projectile.ai[0] = -1f;
					Projectile.netUpdate = true;
				}
			}

			Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
			Projectile.rotation += 0.3f * Projectile.direction;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
			for (int index1 = 0; index1 < 10; ++index1)
			{
				int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
				Main.dust[index2].noGravity = true;
				Main.dust[index2].velocity *= 2f;
				int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
				Main.dust[index3].velocity *= 2f;
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Frostburn, 180);
			target.AddBuff(ModContent.BuffType<Buffs.Masomode.HypothermiaBuff>(), 600);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 25);
		}

		public float WidthFunction(float completionRatio)
		{
			float baseWidth = Projectile.scale * Projectile.width * 1.3f;
			return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
		}

		public static Color ColorFunction(float completionRatio)
		{
			return Color.Lerp(new(0, 204, 244), Color.Transparent, completionRatio) * 0.7f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
		public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
		{
			ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
			FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
			PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
		}
	}
}