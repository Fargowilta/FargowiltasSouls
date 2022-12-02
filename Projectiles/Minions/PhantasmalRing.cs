using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class PhantasmalRing : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        private const float PI = (float)Math.PI;
        private const float rotationPerTick = PI / 57f;
        private const float threshold = 350;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantasmal Ring");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.scale *= 2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead && Main.player[Projectile.owner].GetModPlayer<FargoSoulsPlayer>().PhantasmalRing)
            {
                Projectile.alpha = 0;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Main.player[Projectile.owner].Center;

            Projectile.timeLeft = 2;
            Projectile.scale = (1f - Projectile.alpha / 255f) * 0.5f;
            Projectile.ai[0] += rotationPerTick;
            if (Projectile.ai[0] > PI)
            {
                Projectile.ai[0] -= 2f * PI;
                Projectile.netUpdate = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }

            if (++Projectile.localAI[0] > 20)
            {
                Projectile.localAI[0] = 0;
                if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].GetToggleValue("MasoRing"))
                {
                    if (++Projectile.localAI[1] >= 7)
                        Projectile.localAI[1] = 0;
                    if (++Projectile.localAI[1] >= 7)
                        Projectile.localAI[1] = 0;
                    NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500, true));
                    if (npc != null)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(npc.Center), ModContent.ProjectileType<RingDeathray>(), Projectile.damage, 0f, Projectile.owner, Projectile.identity, Projectile.localAI[1]);
                    }
                }
            }
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
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Projectile.GetAlpha(lightColor);

            for (int x = 0; x < 7; x++)
            {
                Vector2 drawOffset = new Vector2(threshold * Projectile.scale / 2f, 0f).RotatedBy(Projectile.ai[0]);
                drawOffset = drawOffset.RotatedBy(2f * PI / 7f * x);
                const int max = 4;
                for (int i = 0; i < max; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(max - i) / max;
                    Vector2 value4 = Projectile.Center + drawOffset.RotatedBy(rotationPerTick * -i);
                    float num165 = Projectile.rotation;
                    Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }
                Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity * .3f;
        }
    }
}