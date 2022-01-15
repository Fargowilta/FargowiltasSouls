using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class HostileHealingHeart : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hostile Healing Heart");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.timeLeft = 600;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.extraUpdates = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (--projectile.ai[1] < 0)
            {
                NPC n = FargoSoulsUtil.NPCExists(projectile.ai[0]);
                if (n != null && !n.friendly)
                {
                    for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                    {
                        Vector2 change = projectile.DirectionTo(n.Center) * 2f;
                        projectile.velocity = (projectile.velocity * 29f + change) / 30f;
                    }

                    if (projectile.Colliding(projectile.Hitbox, n.Hitbox)) //die and feed it
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            n.life += projectile.damage;
                            n.HealEffect(projectile.damage);
                            if (n.life > n.lifeMax)
                                n.life = n.lifeMax;
                            projectile.Kill();
                        }
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }

            for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
            {
                projectile.position += projectile.velocity;
            }

            if (projectile.velocity != Vector2.Zero)
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 86, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A) * projectile.Opacity;
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}