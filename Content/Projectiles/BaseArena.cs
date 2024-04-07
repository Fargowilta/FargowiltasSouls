using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
	public abstract class BaseArena : ModProjectile
	{
		protected float rotationPerTick;
		protected readonly int npcType;
		protected readonly int dustType;
		protected readonly int increment;
		protected readonly int visualCount;
		protected float threshold;
		protected float targetPlayer;

		protected BaseArena(float rotationPerTick, float threshold, int npcType, int dustType = 135, int increment = 2, int visualCount = 32)
		{
			this.rotationPerTick = rotationPerTick;
			this.threshold = threshold;
			this.npcType = npcType;
			this.dustType = dustType;
			this.increment = increment;
			this.visualCount = visualCount;
		}

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
		}

		public override void SetDefaults() //MAKE SURE YOU CALL BASE.SETDEFAULTS IF OVERRIDING
		{
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.hostile = true;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;

			Projectile.netImportant = true;

			CooldownSlot = 0;

			Projectile.FargoSouls().GrazeCheck =
				projectile =>
				{
					return CanDamage() == true && targetPlayer == Main.myPlayer && Math.Abs((Main.LocalPlayer.Center - Projectile.Center).Length() - threshold) < Projectile.width / 2 * Projectile.scale + Player.defaultHeight + Main.LocalPlayer.FargoSouls().GrazeRadius;
				};

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

			Projectile.hide = true;
			Projectile.FargoSouls().DeletionImmuneRank = 3;
			Projectile.FargoSouls().TimeFreezeImmune = true;
		}

		public override bool? CanDamage()
		{
			return Projectile.alpha == 0;
		}

		public override bool CanHitPlayer(Player target)
		{
			return targetPlayer == target.whoAmI && target.hurtCooldowns[CooldownSlot] == 0;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return Math.Abs((targetHitbox.Center.ToVector2() - projHitbox.Center.ToVector2()).Length() - threshold) < Projectile.width / 2 * Projectile.scale;
		}

		protected virtual void Movement(NPC npc)
		{
			//this can also be used for general npc-reliant checks and killing the proj
		}
		float speed = 17;
		public override void AI()
		{
			NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], npcType);
			if (npc != null)
			{
				Projectile.alpha -= increment;
				if (Projectile.alpha < 0)
					Projectile.alpha = 0;

				Movement(npc);

				targetPlayer = npc.target;

				Player player = Main.LocalPlayer;
				if (player.active && !player.dead && !player.ghost)
				{
					float distance = player.Distance(Projectile.Center);
					if (distance > threshold && distance < threshold * 4f)
					{
						if (distance > threshold * 2f)
						{
							player.controlLeft = false;
							player.controlRight = false;
							player.controlUp = false;
							player.controlDown = false;
							player.controlUseItem = false;
							player.controlUseTile = false;
							player.controlJump = false;
							player.controlHook = false;
							if (player.grapCount > 0)
								player.RemoveAllGrapplingHooks();
							if (player.mount.Active)
								player.mount.Dismount(player);
							player.velocity.X = 0f;
							player.velocity.Y = -0.4f;
							player.FargoSouls().NoUsingItems = 2;
						}

						Vector2 movement = Projectile.Center - player.Center;
						float difference = movement.Length() - threshold;
						movement.Normalize();
						movement *= difference < speed ? difference : speed;
						player.position += movement;

						for (int i = 0; i < 20; i++)
						{
							int d = Dust.NewDust(player.position, player.width, player.height, dustType, 0f, 0f, 0, default, 2.5f);
							Main.dust[d].noGravity = true;
							Main.dust[d].velocity *= 5f;
						}
						if (speed <= 60f)
						{
							speed += 3 / 60f;
						}
					}
					else
					{
						speed = 17;
					}
				}
			}
			else
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.alpha += increment;
				if (Projectile.alpha > 255)
				{
					Projectile.Kill();
					return;
				}
			}

			Projectile.timeLeft = 2;
			Projectile.scale = (1f - Projectile.alpha / 255f) * 2f;
			Projectile.ai[0] += rotationPerTick;
			if (Projectile.ai[0] > MathHelper.Pi)
			{
				Projectile.ai[0] -= 2f * MathHelper.Pi;
				Projectile.netUpdate = true;
			}
			else if (Projectile.ai[0] < -MathHelper.Pi)
			{
				Projectile.ai[0] += 2f * MathHelper.Pi;
				Projectile.netUpdate = true;
			}

			Projectile.localAI[0] = threshold;
		}

		public override void PostAI()
		{
			Projectile.hide = false;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.velocity = target.SafeDirectionTo(Projectile.Center) * 4f;
		}

		public override void OnKill(int timeLeft)
		{
			float modifier = (255f - Projectile.alpha) / 255f;
			float offset = threshold * modifier;
			int max = (int)(300 * modifier);
			for (int i = 0; i < max; i++)
			{
				int d = Dust.NewDust(Projectile.Center, 0, 0, dustType, Scale: 4f);
				Main.dust[d].velocity *= 6f;
				Main.dust[d].noGravity = true;
				Main.dust[d].position = Projectile.Center + offset * Vector2.UnitX.RotatedByRandom(2 * Math.PI);
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity * (targetPlayer == Main.myPlayer ? 1f : 0.15f);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw

			Color color26 = Projectile.GetAlpha(lightColor);

			for (int x = 0; x < visualCount; x++)
			{
				int frame = (Projectile.frame + x) % Main.projFrames[Projectile.type];
				int y3 = num156 * frame; //ypos of upper left corner of sprite to draw
				Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
				Vector2 origin2 = rectangle.Size() / 2f;

				float rotation = 2f * MathHelper.Pi / visualCount * x + Projectile.ai[0];

				Vector2 drawOffset = new Vector2(threshold * Projectile.scale / 2f, 0f).RotatedBy(Projectile.ai[0]);
				drawOffset = drawOffset.RotatedBy(2f * MathHelper.Pi / visualCount * x);
				const int max = 4;
				for (int i = 0; i < max; i++)
				{
					Color color27 = color26;
					color27 *= (float)(max - i) / max;
					Vector2 value4 = Projectile.Center + drawOffset.RotatedBy(rotationPerTick * -i);
					float rot = rotation + Projectile.rotation + MathHelper.TwoPi / 4 * x;
					Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, rot, origin2, Projectile.scale, SpriteEffects.None, 0);
				}

				float finalRot = rotation + Projectile.rotation + MathHelper.TwoPi / 4 * x;
				Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, finalRot, origin2, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}
	}
}
