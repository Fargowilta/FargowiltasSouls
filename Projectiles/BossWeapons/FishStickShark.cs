using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class FishStickShark : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_408";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Shark");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.MiniSharkron];
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.MiniSharkron);
            aiType = ProjectileID.MiniSharkron;
            projectile.penetrate = 2;
            projectile.timeLeft = 90;

            projectile.tileCollide = false;
            projectile.minion = false;
            projectile.ranged = true;
        }

        public override void AI()
        {
            projectile.position += projectile.velocity * 0.5f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            for (int num321 = 0; num321 < 15; num321++)
            {
                int num322 = Dust.NewDust(projectile.Center - Vector2.One * 10f, 50, 50, 5, 0f, -2f, 0, default(Color), 1f);
                Main.dust[num322].velocity /= 2f;
            }
            int num323 = 10;
            int num324 = Gore.NewGore(projectile.Center, projectile.velocity * 0.8f, 584, 1f);
            Main.gore[num324].timeLeft /= num323;
            num324 = Gore.NewGore(projectile.Center, projectile.velocity * 0.9f, 585, 1f);
            Main.gore[num324].timeLeft /= num323;
            num324 = Gore.NewGore(projectile.Center, projectile.velocity * 1f, 586, 1f);
            Main.gore[num324].timeLeft /= num323;
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotationModifier = 0;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.25f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i] + rotationModifier;
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation + rotationModifier, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}