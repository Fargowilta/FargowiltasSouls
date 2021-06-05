using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    class DevRocket : ModProjectile
    {
		public override string Texture => "Terraria/Projectile_616";
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dev Rocket");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.VortexBeaterRocket);
            projectile.aiStyle = -1;
			//aiType = ProjectileID.VortexBeaterRocket;

			projectile.ranged = false;
			projectile.minion = true;

            projectile.penetrate = 2;

            projectile.timeLeft = 30 * (projectile.extraUpdates + 1);
		}

        private int myCounter;

        public override void AI()
        {
            //Main.NewText(projectile.ai[0] + " " + projectile.ai[1] + " " + projectile.localAI[0] + " " + projectile.localAI[1]);

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.velocity.Length();
                projectile.ai[1] = -1;
            }

            projectile.alpha -= 25;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            if (++projectile.ai[0] > 30)
            {
                projectile.ai[0] = 20;

                if (projectile.ai[1] == -1)
                {
                    float maxDistance = 1000f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy() && projectile.Distance(Main.npc[i].Center) < maxDistance
                            && Collision.CanHit(projectile.Center, 0, 0, Main.npc[i].Center, 0, 0))
                        {
                            maxDistance = projectile.Distance(Main.npc[i].Center);
                            projectile.ai[1] = i;
                        }
                    }
                }
            }

            int ai1 = (int)projectile.ai[1];
            if (ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].CanBeChasedBy())
            {
                Vector2 targetSpeed = projectile.DirectionTo(Main.npc[ai1].Center) * projectile.localAI[0];
                const int factor = 8;
                projectile.velocity = (projectile.velocity * (factor - 1) + targetSpeed) / factor;
            }

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
			if (projectile.owner == Main.myPlayer && projectile.localAI[1] == 1)
			{
				Projectile[] projs = FargoGlobalProjectile.XWay(Main.rand.Next(3, 7), projectile.Center, projectile.type, 6, projectile.damage, projectile.knockBack);
                foreach (Projectile proj in projs)
                {
                    proj.localAI[1] = 2;
                }
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

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                float scale = projectile.scale;
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}
