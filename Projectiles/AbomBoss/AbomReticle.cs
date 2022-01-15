using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn Reticle");
        }

        public override void SetDefaults()
        {
            projectile.width = 110;
            projectile.height = 110;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.timeLeft = 70;
            //cooldownSlot = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>())
                && !Main.npc[EModeGlobalNPC.abomBoss].dontTakeDamage)
            {
                if (projectile.localAI[0] == 0)
                {
                    projectile.localAI[0] = Main.rand.NextBool() ? -1 : 1;
                    projectile.rotation = Main.rand.NextFloat(2 * MathHelper.Pi);
                }

                int modifier = Math.Min(60, 70 - projectile.timeLeft);

                projectile.scale = 1.5f - 0.5f / 60f * modifier; //start big, shrink down

                projectile.velocity = Vector2.Zero;

                if (++projectile.localAI[1] < 15)
                    projectile.rotation += MathHelper.ToRadians(12) * projectile.localAI[0];
            }

            if (projectile.timeLeft < 10) //fade in and out
                projectile.alpha += 26;
            else
                projectile.alpha -= 26;

            if (projectile.alpha < 0)
                projectile.alpha = 0;
            else if (projectile.alpha > 255)
                projectile.alpha = 255;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}