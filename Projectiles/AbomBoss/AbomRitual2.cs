using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomRitual2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_274";

        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 57f;
        private const float threshold = 150;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn Seal");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
            if (npc != null)
            {
                projectile.alpha -= 2;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                projectile.Center = npc.Center;
            }
            else
            {
                projectile.velocity = Vector2.Zero;
                projectile.alpha += 2;
                if (projectile.alpha > 255)
                {
                    projectile.Kill();
                    return;
                }
            }

            projectile.timeLeft = 2;
            projectile.scale = 1f - projectile.alpha / 255f;
            projectile.ai[0] += rotationPerTick;
            if (projectile.ai[0] > PI)
            {
                projectile.ai[0] -= 2f * PI;
                projectile.netUpdate = true;
            }

            projectile.rotation += 0.5f;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = projectile.GetAlpha(lightColor);

            for (int x = 0; x < 6; x++)
            {
                Vector2 drawOffset = new Vector2(threshold * projectile.scale / 2f, 0f).RotatedBy(projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * PI / 6f * x);
                Main.spriteBatch.Draw(texture2D13, projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, x % 2 == 0 ? projectile.rotation : -projectile.rotation, origin2, projectile.scale, x % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}