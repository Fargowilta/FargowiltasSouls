using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class StardustKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Stardust Knife");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.alpha = 0;

            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            if (projectile.localAI[0] == 0)
                Main.PlaySound(SoundID.Item1, projectile.Center);

            if (++projectile.localAI[0] > FargoGlobalProjectile.TimeFreezeMoveDuration * projectile.MaxUpdates * 2)
            {
                projectile.tileCollide = true;
            }
            else if (projectile.localAI[0] == FargoGlobalProjectile.TimeFreezeMoveDuration * projectile.MaxUpdates + 1)
            {
                //fixed speed when time resumes
                projectile.velocity = 24f / projectile.MaxUpdates * Vector2.Normalize(projectile.velocity);
                projectile.localAI[1] = 1f;
            }
        }

        public override void Kill(int timeleft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 80;
            projectile.Center = projectile.position;

            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 7, 0.5f, 0.0f);

            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 176, 0.0f, 0.0f, 200, new Color(), 3.7f);
                Main.dust[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity * 3f;
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 180, 0.0f, 0.0f, 0, new Color(), 2.7f);
                Main.dust[index2].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2()) * (float)projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity * 3f;
            }
            for (int index1 = 0; index1 < 5; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 0, new Color(), 1.5f);
                Main.dust[index2].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2()) * (float)projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity * 3f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.localAI[1] == 1)
            {
                Texture2D glow = mod.GetTexture("Projectiles/MutantBoss/MutantEye_Glow");
                int rect1 = glow.Height / Main.projFrames[projectile.type];
                int rect2 = rect1 * projectile.frame;

                Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
                Vector2 gloworigin2 = glowrectangle.Size() / 2f;
                Color glowcolor = Color.Lerp(new Color(29, 171, 239, 0), Color.Transparent, 0.7f);

                Vector2 drawOffset = projectile.velocity.SafeNormalize(Vector2.UnitX) * 18;

                float scale = projectile.scale;

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color27 = glowcolor;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Main.spriteBatch.Draw(glow, projectile.oldPos[i] + projectile.Size / 2f - drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                        color27, projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0f);
                }

                Main.spriteBatch.Draw(glow, projectile.Center - drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}