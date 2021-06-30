using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EridanusRitual : ModProjectile
    {
        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 57f;
        private const float threshold = 175f / 2f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Ritual");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead && !Main.player[projectile.owner].ghost && Main.player[projectile.owner].GetModPlayer<FargoPlayer>().EridanusEmpower)
            {
                projectile.alpha = 0;
            }
            else
            {
                projectile.Kill();
                return;
            }

            projectile.Center = Main.player[projectile.owner].Center;

            projectile.timeLeft = 2;
            projectile.scale = (1f - projectile.alpha / 255f) * 1.5f + (Main.mouseTextColor / 200f - 0.35f) * 0.5f; //throbbing
            projectile.scale /= 2f;
            if (projectile.scale < 0.1f)
                projectile.scale = 0.1f;
            /*projectile.ai[0] += rotationPerTick;
            if (projectile.ai[0] > PI)
            {
                projectile.ai[0] -= 2f * PI;
                projectile.netUpdate = true;
            }
            projectile.rotation = projectile.ai[0];*/
            projectile.rotation += rotationPerTick;
            if (projectile.rotation > PI)
                projectile.rotation -= 2f * PI;

            switch (Main.player[projectile.owner].GetModPlayer<FargoPlayer>().EridanusTimer / (60 * 10))
            {
                case 0: projectile.frame = 1; break;
                case 1: projectile.frame = 2; break;
                case 2: projectile.frame = 0; break;
                default: projectile.frame = 3; break;
            }
            
            //handle countdown between phase changes
            projectile.localAI[0] = Main.player[projectile.owner].GetModPlayer<FargoPlayer>().EridanusTimer % (float)(60 * 10) / (float)(60 * 10) * 12f - 1f;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = projectile.GetAlpha(lightColor);

            const int max = 12;
            for (int x = 0; x < max; x++)
            {
                if (x < projectile.localAI[0])
                    continue;
                Vector2 drawOffset = new Vector2(0f, -threshold * projectile.scale);
                drawOffset = drawOffset.RotatedBy((x + 1) * PI / max * 2).RotatedBy(projectile.ai[0]);
                spriteBatch.Draw(texture2D13, projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity * 0.8f;
        }
    }
}