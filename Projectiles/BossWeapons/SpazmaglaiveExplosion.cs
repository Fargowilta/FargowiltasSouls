using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SpazmaglaiveExplosion : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			//projectile.aiStyle = 136;
			projectile.alpha = 0;
			projectile.penetrate = -1;
			projectile.friendly = true;
			//projectile.usesLocalNPCImmunity = true;
			//projectile.localNPCHitCooldown = 8;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
		}

		public int timer;
		public float lerp = 0.12f;

		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Cursed Inferno");
		}

		public override void AI()
		{
			NPC target = Main.npc[(int)projectile.ai[1]];
			if (!target.active)
				projectile.Kill();

			Vector2 center = target.Center;
			projectile.Center = center;
			projectile.rotation = projectile.velocity.ToRotation();

			DelegateMethods.v3_1 = new Vector3(1.2f, 1f, 0.3f);
			float num2 = projectile.localAI[0] / 40f;
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			float num3 = (projectile.localAI[0] - 38f) / 40f;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			Utils.PlotTileLine(projectile.Center + projectile.rotation.ToRotationVector2() * 100f * num3, projectile.Center + projectile.rotation.ToRotationVector2() * 100f * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));
			Utils.PlotTileLine(projectile.Center + projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583, default(Vector2)) * 100f * num3, projectile.Center + projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583, default(Vector2)) * 100f * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));
			Utils.PlotTileLine(projectile.Center + projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583, default(Vector2)) * 100f * num3, projectile.Center + projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583, default(Vector2)) * 100f * num2, 16f, new Utils.PerLinePoint(DelegateMethods.CastLight));

			/*if (Main.rand.NextBool(4) && projectile.localAI[0] >= 25f)
			{
				Vector2 vector = projectile.Center + projectile.rotation.ToRotationVector2() * 600f;
				vector -= Utils.RandomVector2(Main.rand, -40f, 40f);
				Gore gore = Gore.NewGoreDirect(vector, Vector2.Zero, 61 + Main.rand.Next(3), 1f);
				gore.velocity *= 0.6f;
				gore.velocity += projectile.rotation.ToRotationVector2() * 4f;
			}*/
			//projectile.frameCounter++;
			projectile.localAI[0] += 1f;

			if (projectile.localAI[0] >= projectile.ai[0])
			{
				projectile.Kill();
			}
		}

		public override bool? Colliding(Rectangle myRect, Rectangle targetRect)
		{
			float num11 = 0f;
			float num12 = projectile.localAI[0] / 25f;
			if (num12 > 1f)
			{
				num12 = 1f;
			}
			float num13 = (projectile.localAI[0] - 38f) / 40f;
			if (num13 < 0f)
			{
				num13 = 0f;
			}
			Vector2 lineStart = projectile.Center + projectile.rotation.ToRotationVector2() * 100f * num13;
			Vector2 lineEnd = projectile.Center + projectile.rotation.ToRotationVector2() * 200f * num12;
			if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), lineStart, lineEnd, 40f * projectile.scale, ref num11))
			{
				return true;
			}
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(13, 219, 83);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 value10 = projectile.Center;
			value10 -= Main.screenPosition;
			float num178 = 40f;
			float num179 = num178 * 2f;
			float num180 = (float)projectile.localAI[0] / num178;
			Texture2D texture2D5 = Main.projectileTexture[projectile.type];
			Color color33 = Color.White;
			Color color34 = new Color(255, 255, 255, 0);
			Color color35 = new Color(30, 180, 30, 0);
			Color color36 = new Color(0, 30, 0, 0);
			ulong num181 = 1uL;
			for (float num182 = 0f; num182 < 15f; num182 += 0.66f)
			{
				float num183 = Utils.RandomFloat(ref num181) * 0.25f - 0.125f;
				Vector2 value11 = (projectile.rotation + num183).ToRotationVector2();
				Vector2 value12 = value10 + value11 * 200f;
				float num184 = num180 + num182 * 0.06666667f;
				int num185 = (int)(num184 / 0.06666667f);
				num184 %= 1f;
				if ((num184 <= num180 % 1f || (float)projectile.localAI[0] >= num178) && (num184 >= num180 % 1f || (float)projectile.localAI[0] < num179 - num178))
				{
					if (num184 < 0.1f)
					{
						color33 = Color.Lerp(Color.Transparent, color34, Utils.InverseLerp(0f, 0.1f, num184, true));
					}
					else if (num184 < 0.35f)
					{
						color33 = color34;
					}
					else if (num184 < 0.7f)
					{
						color33 = Color.Lerp(color34, color35, Utils.InverseLerp(0.35f, 0.7f, num184, true));
					}
					else if (num184 < 0.9f)
					{
						color33 = Color.Lerp(color35, color36, Utils.InverseLerp(0.7f, 0.9f, num184, true));
					}
					else if (num184 < 1f)
					{
						color33 = Color.Lerp(color36, Color.Transparent, Utils.InverseLerp(0.9f, 1f, num184, true));
					}
					else
					{
						color33 = Color.Transparent;
					}
					color33.A = (byte)(255 - (num184 * 255));
					float num186 = 0.9f + num184 * 0.8f;
					num186 *= num186;
					num186 *= 0.8f;
					Vector2 position = Vector2.SmoothStep(value10, value12, num184);
					Rectangle rectangle2 = texture2D5.Frame(1, 7, 0, (int)(num184 * 7f));
					Main.spriteBatch.Draw(texture2D5, position, new Microsoft.Xna.Framework.Rectangle?(rectangle2), color33, projectile.rotation + 6.28318548f * (num184 + Main.GlobalTime * 1.2f) * 0.2f + (float)num185 * 1.2566371f, rectangle2.Size() / 2f, num186 / 2, SpriteEffects.None, 0f);
				}
			}
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.CursedInferno, 180, false);
		}
	}
}