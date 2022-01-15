using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class ForbiddenTornado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Forbidden Tornado");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 0;

            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
            projectile.timeLeft = 1200;
			projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
		}

        public override void AI()
        {
            projectile.velocity = Vector2.UnitY;
            projectile.position -= projectile.velocity;

			float num1123 = 900f;
			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = -1;
				Main.PlaySound(SoundID.Item82, projectile.Center);
			}
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= num1123)
			{
				projectile.Kill();
			}
			if (projectile.localAI[0] >= 30f)
			{
				projectile.damage = 0;
				if (projectile.ai[0] < num1123 - 120f)
				{
					float num1124 = projectile.ai[0] % 60f;
					projectile.ai[0] = num1123 - 120f + num1124;
					projectile.netUpdate = true;
				}
			}
			float num1125 = 15f;
			float num1126 = 15f;
			Point point8 = projectile.Center.ToTileCoordinates();
			int num1127;
			int num1128;
			Collision.ExpandVertically(point8.X, point8.Y, out num1127, out num1128, (int)num1125, (int)num1126);
			num1127++;
			num1128--;
			Vector2 value72 = new Vector2((float)point8.X, (float)num1127) * 16f + new Vector2(8f);
			Vector2 value73 = new Vector2((float)point8.X, (float)num1128) * 16f + new Vector2(8f);
			Vector2 vector145 = Vector2.Lerp(value72, value73, 0.5f);
			Vector2 value74 = new Vector2(0f, value73.Y - value72.Y);
			value74.X = value74.Y * 0.2f;
			projectile.width = (int)(value74.X * 0.65f);
			projectile.height = (int)value74.Y;
			projectile.Center = vector145;
			if (projectile.owner == Main.myPlayer)
			{
				bool flag75 = false;
				Vector2 center16 = Main.player[projectile.owner].Center;
				Vector2 top = Main.player[projectile.owner].Top;
				for (float num1129 = 0f; num1129 < 1f; num1129 += 0.05f)
				{
					Vector2 position2 = Vector2.Lerp(value72, value73, num1129);
					if (Collision.CanHitLine(position2, 0, 0, center16, 0, 0) || Collision.CanHitLine(position2, 0, 0, top, 0, 0))
					{
						flag75 = true;
						break;
					}
				}
				if (!flag75 && projectile.ai[0] < num1123 - 120f)
				{
					float num1130 = projectile.ai[0] % 60f;
					projectile.ai[0] = num1123 - 120f + num1130;
					projectile.netUpdate = true;
				}
			}
			if (projectile.ai[0] < num1123 - 120f)
			{
				for (int num1131 = 0; num1131 < 1; num1131++)
				{
					float value75 = -0.5f;
					float value76 = 0.9f;
					float amount3 = Main.rand.NextFloat();
					Vector2 value77 = new Vector2(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(value75, value76, amount3));
					value77.X *= MathHelper.Lerp(2.2f, 0.6f, amount3);
					value77.X *= -1f;
					Vector2 value78 = new Vector2(6f, 10f);
					Vector2 position3 = vector145 + value74 * value77 * 0.5f + value78;
					Dust dust34 = Main.dust[Dust.NewDust(position3, 0, 0, 269, 0f, 0f, 0, default(Color), 1f)];
					dust34.position = position3;
					dust34.customData = vector145 + value78;
					dust34.fadeIn = 1f;
					dust34.scale = 0.3f;
					if (value77.X > -1.2f)
					{
						dust34.velocity.X = 1f + Main.rand.NextFloat();
					}
					dust34.velocity.Y = Main.rand.NextFloat() * -0.5f - 1f;
				}
				return;
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float halfheight = 220;
            float density = 50f;
            for (float i = 0; i < (int)density; i++)
            {
                Color color = new Color(212, 192, 100);
                color.A /= 2;
                float lerpamount = (Math.Abs(density / 2 - i) > ((density/2) * 0.6f)) ? Math.Abs(density / 2 - i)/(density/2) : 0f; //if too low or too high up, start making it transparent
                color = Color.Lerp(color, Color.Transparent, lerpamount);
                Texture2D texture = Main.projectileTexture[projectile.type];
                Vector2 offset = Vector2.SmoothStep(projectile.Center + Vector2.Normalize(projectile.velocity) * halfheight, projectile.Center - Vector2.Normalize(projectile.velocity) * halfheight, i / density);
                float scale = MathHelper.Lerp(projectile.scale * 0.8f, projectile.scale * 2.5f, i / density);
                Main.spriteBatch.Draw(texture, offset - Main.screenPosition,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    projectile.GetAlpha(color),
                    i / 6f - Main.GlobalTime * 5f + projectile.rotation,
                    texture.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0);
            }
            return false;
        }
    }
}