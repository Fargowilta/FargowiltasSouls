using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FragmentRitual : ModProjectile
    {
        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 140f;
        private const float threshold = 600f;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lunar Ritual");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.MoonLordCore);
            if (npc != null && npc.ai[0] != 2f)
            {
                projectile.hide = false;

                projectile.alpha -= 2;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;

                projectile.Center = npc.Center;

                projectile.localAI[0] = npc.GetEModeNPCMod<MoonLordCore>().VulnerabilityTimer / 56.25f; //number to hide
                projectile.localAI[0]--;

                switch (npc.GetEModeNPCMod<MoonLordCore>().VulnerabilityState) //match ML vulnerability to fragment
                {
                    case 0: projectile.frame = 1; break;
                    case 1: projectile.frame = 2; break;
                    case 2: projectile.frame = 0; break;
                    case 3: projectile.frame = 3; break;
                    default: projectile.frame = 4; break;
                }
            }
            else
            {
                projectile.hide = true;
                projectile.velocity = Vector2.Zero;
                projectile.alpha += 2;
                if (projectile.alpha > 255)
                {
                    projectile.Kill();
                    return;
                }
            }

            projectile.timeLeft = 2;
            projectile.scale = (1f - projectile.alpha / 255f) * 1.5f + (Main.mouseTextColor / 200f - 0.35f) * 0.25f; //throbbing
            if (projectile.scale < 0.1f) //clamp scale
                projectile.scale = 0.1f;
            projectile.ai[0] += rotationPerTick;
            if (projectile.ai[0] > PI)
            {
                projectile.ai[0] -= 2f * PI;
                projectile.netUpdate = true;
            }
            projectile.rotation = projectile.ai[0];
            
            /*projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 1)
                    projectile.frame = 0;
            }*/
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

            const int max = 32;
            for (int x = 0; x < max; x++)
            {
                if (x < projectile.localAI[0])
                    continue;
                Vector2 drawOffset = new Vector2(threshold * projectile.scale / 2f, 0f);//.RotatedBy(projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * PI / max * (x + 1) - PI / 2);
                /*const int max = 4;
                for (int i = 0; i < max; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(max - i) / max;
                    Vector2 value4 = projectile.Center + drawOffset.RotatedBy(-rotationPerTick * i);
                    float num165 = projectile.rotation;
                    Main.spriteBatch.Draw(texture2D13, value4 - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
                }*/
                Main.spriteBatch.Draw(texture2D13, projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity * .75f;
        }
    }
}