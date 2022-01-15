using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomLaser : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_466";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Laser");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.aiStyle = 1;
            aiType = ProjectileID.SaucerLaser;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 0.3f;
            cooldownSlot = 1;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 222);
                Main.dust[d].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(BuffID.Confused, 300);
            }
            target.AddBuff(BuffID.Electrified, 180);
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