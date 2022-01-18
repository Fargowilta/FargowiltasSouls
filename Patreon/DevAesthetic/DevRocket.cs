using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    class DevRocket : ModProjectile
    {
		public override void SetStaticDefaults()
		{
		    //DisplayName.SetDefault("Dev Rocket");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.VortexBeaterRocket);
            projectile.aiStyle = -1;
			//aiType = ProjectileID.VortexBeaterRocket;

			projectile.ranged = false;
			projectile.minion = true;

            projectile.penetrate = 2;

            projectile.timeLeft = 75 * (projectile.extraUpdates + 1);
		}

        private Color color;

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.velocity.Length() * (Main.rand.NextBool() ? 1 : -1);
                color = new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5);
                projectile.ai[0] = Main.rand.Next(-30, 30);
                projectile.ai[1] = -1;
            }

            projectile.alpha -= 25;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            if (++projectile.ai[0] > 30)
            {
                projectile.ai[0] = 20;

                if (projectile.timeLeft > 45 * projectile.MaxUpdates)
                    projectile.timeLeft = 45 * projectile.MaxUpdates;

                if (projectile.ai[1] == -1)
                {
                    projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000, true);
                    projectile.netUpdate = true;
                }
            }

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1]);
            if (npc != null && npc.CanBeChasedBy())
            {
                projectile.position += npc.velocity / 5;

                Vector2 targetPos = npc.Center;
                float offset = 120 * projectile.timeLeft / (30 * projectile.MaxUpdates) * 2;
                if (projectile.Distance(targetPos) > offset)
                    targetPos += projectile.DirectionTo(npc.Center).RotatedBy(MathHelper.PiOver2) * offset * Math.Sign(projectile.localAI[0]);
                Vector2 targetSpeed = projectile.DirectionTo(targetPos) * Math.Abs(projectile.localAI[0]);
                const int factor = 8;
                projectile.velocity = (projectile.velocity * (factor - 1) + targetSpeed) / factor;
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
			if (projectile.owner == Main.myPlayer && projectile.localAI[1] == 1)
			{
				Projectile[] projs = FargoSoulsUtil.XWay(Main.rand.Next(3, 7), projectile.Center, projectile.type, 6, projectile.damage, projectile.knockBack);
                foreach (Projectile proj in projs)
                {
                    if (proj != null)
                        proj.localAI[1] = 2;
                }
			}

            Color dustColor = color;
            dustColor.A = 100;
            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 76, projectile.velocity.X, projectile.velocity.Y, 100, dustColor, 2f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;
            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.timeLeft = 0;
            if (projectile.localAI[1] == 0)
                projectile.localAI[1] = 1;
            target.immune[projectile.owner] = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;
            
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27.A = 0;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                float scale = projectile.scale;
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}
