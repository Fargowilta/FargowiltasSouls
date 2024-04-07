using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
	public class AbomScytheSplit : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_274";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Abominationn Scythe");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			CooldownSlot = 1;

			Projectile.scale = 2f;
		}

		public override void AI()
		{
			/*if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }*/

			Projectile.rotation += 1f;

			if (--Projectile.ai[0] <= 0)
				Projectile.Kill();
		}

		public override void OnKill(int timeLeft)
		{
			int dustMax = Projectile.ai[1] >= 0 ? 50 : 25;
			float speed = Projectile.ai[1] >= 0 ? 15 : 6;
			for (int i = 0; i < dustMax; i++)
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Scale: 3.5f);
				Main.dust[d].velocity *= speed;
				Main.dust[d].noGravity = true;
			}

			if (Projectile.ai[1] >= 0)
			{
				if (FargoSoulsUtil.HostCheck)
				{
					int p = Player.FindClosest(Projectile.Center, 0, 0);
					if (p != -1)
					{
						Vector2 vel = Projectile.ai[1] == 0 ? Vector2.Normalize(Projectile.velocity) : Projectile.SafeDirectionTo(Main.player[p].Center);
						vel *= 30f;
						int max = Projectile.ai[1] == 0 ? 6 : WorldSavingSystem.MasochistModeReal ? 10 : 8;
						for (int i = 0; i < max; i++)
						{
							Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.TwoPi / max * i), ModContent.ProjectileType<AbomSickle3>(), Projectile.damage, Projectile.knockBack, Projectile.owner, p);
						}
					}
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (WorldSavingSystem.EternityMode)
			{
				target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFangBuff>(), 300);
				//target.AddBuff(ModContent.BuffType<Unstable>(), 240);
				target.AddBuff(ModContent.BuffType<Buffs.Masomode.BerserkedBuff>(), 120);
			}
			target.AddBuff(BuffID.Bleeding, 600);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;

			Color color26 = lightColor;
			color26 = Projectile.GetAlpha(color26);

			int max = ProjectileID.Sets.TrailCacheLength[Projectile.type];
			if (Projectile.velocity == Vector2.Zero)
				max /= 2;
			for (int i = 0; i < max; i++)
			{
				Color color27 = color26;
				color27 *= (float)(max - i) / max;
				Vector2 value4 = Projectile.oldPos[i];
				float num165 = Projectile.oldRot[i];
				Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0) * Projectile.Opacity;
		}
	}
}