using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FragmentRitual : ModProjectile
    {
        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 140f;
        private const float threshold = 600f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Lunar Ritual");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.MoonLordCore);
            if (npc != null && npc.ai[0] != 2f)
            {
                Projectile.hide = false;

                Projectile.alpha -= 2;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.Center = npc.Center;

                Projectile.localAI[0] = (int)npc.GetGlobalNPC<MoonLordCore>().VulnerabilityTimer / 56.25f; //number to hide
                Projectile.localAI[0]--;

                Projectile.frame = npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState switch //match ML vulnerability to fragment
                {
                    0 => 1,
                    1 => 2,
                    2 => 0,
                    3 => 3,
                    _ => 4,
                };
            }
            else
            {
                Projectile.hide = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.alpha += 2;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.timeLeft = 2;
            Projectile.scale = (1f - Projectile.alpha / 255f) * 1.5f + (Main.mouseTextColor / 200f - 0.35f) * 0.25f; //throbbing
            if (Projectile.scale < 0.1f) //clamp scale
                Projectile.scale = 0.1f;
            Projectile.ai[0] += rotationPerTick;
            if (Projectile.ai[0] > PI)
            {
                Projectile.ai[0] -= 2f * PI;
                Projectile.netUpdate = true;
            }
            Projectile.rotation = Projectile.ai[0];

            /*Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }*/
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

            const int max = 32;
            for (int x = 0; x < max; x++)
            {
                if (x < Projectile.localAI[0])
                    continue;
                Vector2 drawOffset = new(threshold * Projectile.scale / 2f, 0);//.RotatedBy(Projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * PI / max * (x + 1) - PI / 2);
                /*const int max = 4;
                for (int i = 0; i < max; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(max - i) / max;
                    Vector2 value4 = Projectile.Center + drawOffset.RotatedBy(-rotationPerTick * i);
                    float num165 = Projectile.rotation;
                    Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }*/
                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity * .75f;
        }
    }
}