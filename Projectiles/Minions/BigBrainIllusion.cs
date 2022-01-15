using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class BigBrainIllusion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Minions/BigBrainProj";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Big Brain");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        private const int maxTime = 60;

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = maxTime;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            
            projectile.extraUpdates = 1;
            projectile.alpha = 125;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.position = projectile.Center;
                projectile.width = (int)(projectile.width * projectile.ai[0] / projectile.scale);
                projectile.height = (int)(projectile.height * projectile.ai[0] / projectile.scale);
                projectile.scale = projectile.ai[0];
                projectile.Center = projectile.position;

                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 104, 0.5f, -0.2f);
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 12;
            }
            
            projectile.alpha = 255 - (int)(Math.Cos(++projectile.localAI[0] * MathHelper.Pi / 2 / maxTime) * 200);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
        }

        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }*/

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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}