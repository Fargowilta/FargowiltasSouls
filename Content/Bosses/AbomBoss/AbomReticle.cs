using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominationn Reticle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 70;
            //CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>())
                && !Main.npc[EModeGlobalNPC.abomBoss].dontTakeDamage)
            {
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = Main.rand.NextBool() ? -1 : 1;
                    Projectile.rotation = Main.rand.NextFloat(2 * MathHelper.Pi);
                }

                int modifier = Math.Min(60, 70 - Projectile.timeLeft);

                Projectile.scale = 1.5f - 0.5f / 60f * modifier; //start big, shrink down

                Projectile.velocity = Vector2.Zero;

                if (++Projectile.localAI[1] < 15)
                    Projectile.rotation += MathHelper.ToRadians(12) * Projectile.localAI[0];
            }

            if (Projectile.timeLeft < 10) //fade in and out
                Projectile.alpha += 26;
            else
                Projectile.alpha -= 26;

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            else if (Projectile.alpha > 255)
                Projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}