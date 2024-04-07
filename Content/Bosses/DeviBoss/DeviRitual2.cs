using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviRitual2 : ModProjectile
    {
        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 57f;
        private const float threshold = 150;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deviantt Seal");
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<DeviBoss>());
            if (npc != null)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                /*float distance = threshold * Projectile.scale / 2f;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * distance);
                    offset.Y += (float)(Math.Cos(angle) * distance);
                    Dust dust = Main.dust[Dust.NewDust(
                        Main.npc[ai1].Center + offset - new Vector2(4, 4), 0, 0,
                        86, 0, 0, 100, Color.White, 1f)];
                    dust.velocity = Main.npc[ai1].velocity;
                    dust.noGravity = true;
                }*/

                Projectile.Center = npc.Center;
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.alpha += 2;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.timeLeft = 2;
            Projectile.scale = 1f - Projectile.alpha / 255f;
            Projectile.ai[0] -= rotationPerTick;
            if (Projectile.ai[0] < -PI)
            {
                Projectile.ai[0] += 2f * PI;
                Projectile.netUpdate = true;
            }

            //Projectile.rotation += 0.5f;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Projectile.GetAlpha(lightColor);

            for (int x = 0; x < 9; x++)
            {
                Vector2 drawOffset = new Vector2(threshold / (Main.getGoodWorld ? 2 : 1) * Projectile.scale / 2f, 0f).RotatedBy(Projectile.ai[0]);
                float rotation = 2f * PI / 9f * x;
                drawOffset = drawOffset.RotatedBy(rotation);
                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, rotation + Projectile.ai[0] + (float)Math.PI / 2, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}