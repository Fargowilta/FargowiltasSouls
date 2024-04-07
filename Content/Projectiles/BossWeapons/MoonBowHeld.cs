using FargowiltasSouls.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
	public class MoonBowHeld : ModProjectile
	{
		public override string Texture => "FargowiltasSouls/Content/Items/Weapons/BossDrops/MoonBow";

		private int syncTimer;
		private Vector2 mousePos;

		public override void SetDefaults()
		{
			Projectile.width = 28;
			Projectile.height = 62;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.FargoSouls().CanSplit = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hide = true;

			Projectile.netImportant = true;
		}

		public override bool? CanDamage() => false;

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(mousePos.X);
			writer.Write(mousePos.Y);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Vector2 buffer;
			buffer.X = reader.ReadSingle();
			buffer.Y = reader.ReadSingle();
			if (Projectile.owner != Main.myPlayer)
			{
				mousePos = buffer;
			}
		}

		const int theTime = 300;
		const int window = 60;

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.dead || !player.active || player.ghost || (player.whoAmI == Main.myPlayer && !player.controlUseTile))
			{
				Projectile.Kill();
				if (player.active && player.whoAmI == Main.myPlayer && Projectile.localAI[0] >= theTime && Projectile.localAI[0] <= theTime + window)
				{
					player.AddBuff(ModContent.BuffType<MoonBowBuff>(), 600);

					SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
					SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
					SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

					for (int i = 0; i < 30; i++)
					{
						int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, 3f);
						Main.dust[dust].velocity *= 1.4f;
					}

					for (int i = 0; i < 20; i++)
					{
						int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 7f;
						dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
						Main.dust[dust].velocity *= 3f;
					}

					for (int index1 = 0; index1 < 50; ++index1)
					{
						int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, new Color(), 2f);
						Main.dust[index2].noGravity = true;
						Main.dust[index2].velocity *= 21f * Projectile.scale;
						Main.dust[index2].noLight = true;
						int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, new Color(), 1f);
						Main.dust[index3].velocity *= 12f;
						Main.dust[index3].noGravity = true;
						Main.dust[index3].noLight = true;
					}

					for (int i = 0; i < 40; i++)
					{
						int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 5f));
						if (Main.rand.NextBool(3))
							Main.dust[d].noGravity = true;
						Main.dust[d].velocity *= Main.rand.NextFloat(12f, 18f);
						Main.dust[d].position = Projectile.Center;
					}
				}
				return;
			}

			Projectile.hide = false;

			Vector2 center = player.MountedCenter;

			Projectile.velocity = 22f * Vector2.Normalize(mousePos - player.MountedCenter);

			Projectile.Center = center;
			Projectile.rotation = Projectile.velocity.ToRotation();

			float extrarotate = Projectile.direction * player.gravDir < 0 ? MathHelper.Pi : 0;
			float itemrotate = Projectile.direction < 0 ? MathHelper.Pi : 0;
			player.itemRotation = Projectile.velocity.ToRotation() + itemrotate;
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 10;
			player.itemAnimation = 10;

			Projectile.spriteDirection = Projectile.direction * (int)player.gravDir;
			Projectile.rotation -= extrarotate;

			if (Projectile.owner == Main.myPlayer)
			{
				mousePos = Main.MouseWorld;

				if (++syncTimer > 20)
				{
					syncTimer = 0;
					Projectile.netUpdate = true;
				}
			}

			if (++Projectile.localAI[0] == theTime)
			{
				SoundEngine.PlaySound(SoundID.Item29 with { Volume = 1.5f }, Projectile.Center);
			}
			else if (Projectile.localAI[0] == theTime + window)
			{
				SoundEngine.PlaySound(SoundID.Item8 with { Volume = 1.5f }, Projectile.Center);
			}

			if (Projectile.localAI[0] < theTime + window)
			{
				if (Projectile.localAI[0] > theTime)
				{
					Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], 1f, 0.05f); //for rotation of sans eye
				}

				float ratio = Projectile.localAI[0] / (theTime + window);

				int d = Dust.NewDust(player.Center, 0, 0, DustID.BlueFairy, 0f, 0f, 200, new Color(0, 255, 255, 100), 0.5f);
				Main.dust[d].noGravity = true;
				Main.dust[d].velocity *= 0.75f;
				Main.dust[d].fadeIn = 1.3f;
				Vector2 vector = new(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
				vector.Normalize();
				vector *= Main.rand.Next(50, 100) * 0.04f;
				Main.dust[d].velocity = vector;
				vector.Normalize();
				vector *= 34f;

				Main.dust[d].scale *= ratio;
				vector *= ratio * 5;
				Main.dust[d].velocity *= ratio * 5;

				Main.dust[d].position = Projectile.Center - vector;
				Main.dust[d].velocity += player.velocity;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			Color color = lightColor;
			if (Projectile.localAI[0] < theTime + window)
			{
				float ratio = Math.Min(1f, Projectile.localAI[0] / theTime);
				color.A = (byte)(255 - 200 * ratio);
			}
			return color;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor);
			if (Projectile.localAI[0] >= theTime && Projectile.localAI[0] <= theTime + window)
			{
				Color color = new(51, 255, 191);

				const int maxTime = window;
				float effectiveTime = Projectile.localAI[0] - theTime;
				float rotation = MathHelper.TwoPi * Projectile.localAI[1];
				float modifier = Math.Min(1f, (float)Math.Sin(Math.PI * effectiveTime / maxTime) * 2f);
				float opacity = Math.Min(1f, modifier * 2f);
				float sansScale = Projectile.scale * modifier * Main.cursorScale * 1.25f;

				Texture2D star = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Rectangle rect = new(0, 0, star.Width, star.Height);
				Vector2 origin = new(star.Width / 2 + sansScale, star.Height / 2 + sansScale);

				Vector2 drawPos = Projectile.Center;

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

				Main.spriteBatch.Draw(star, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rect), color * opacity, rotation, origin, sansScale, SpriteEffects.None, 0);
				Main.spriteBatch.Draw(star, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rect), Color.White * opacity * 0.75f, rotation, origin, sansScale, SpriteEffects.None, 0);

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			}
			return false;
		}
	}
}