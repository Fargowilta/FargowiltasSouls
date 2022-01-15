using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SaucerLaser : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_466";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Laser");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.aiStyle = 1;
            aiType = ProjectileID.SaucerLaser;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 120;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 0.3f;
        }

        public override void Kill(int timeLeft)
        {
            int num = Main.rand.Next(3, 7);
            for (int index1 = 0; index1 < num; ++index1)
            {
                int index2 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 228, 0.0f, 0.0f, 100, new Color(), 2.1f);
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity * 2f;
                Main.dust[index2].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero || projectile.oldPos[i - 1] == projectile.oldPos[i])
                    continue;
                Vector2 offset = projectile.oldPos[i - 1] - projectile.oldPos[i];
                int length = (int)offset.Length();
                offset.Normalize();
                const int step = 3;
                Color color28 = color27;
                color28 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color28, projectile.rotation, origin2, projectile.scale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
            //Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}