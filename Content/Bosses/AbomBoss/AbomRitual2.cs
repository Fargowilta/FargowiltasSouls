using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomRitual2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_274";

        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 57f;
        private const float threshold = 150;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Abominationn Seal");
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<AbomBoss>());
            if (npc != null)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

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
            Projectile.ai[0] += rotationPerTick;
            if (Projectile.ai[0] > PI)
            {
                Projectile.ai[0] -= 2f * PI;
                Projectile.netUpdate = true;
            }

            Projectile.rotation += 0.5f;
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

            for (int x = 0; x < 6; x++)
            {
                Vector2 drawOffset = new Vector2(threshold * Projectile.scale / 2f, 0f).RotatedBy(Projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * PI / 6f * x);
                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, x % 2 == 0 ? Projectile.rotation : -Projectile.rotation, origin2, Projectile.scale, x % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}