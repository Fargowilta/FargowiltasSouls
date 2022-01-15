using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs.Champions;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillShell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Will Shell");
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.timeLeft = 600;

            projectile.scale = 1.5f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], ModContent.NPCType<WillChampion>());
            if (npc != null && npc.ai[0] == -1f) //disappear when champ is vulnerable again
            {
                projectile.Center = npc.Center;
                projectile.direction = projectile.spriteDirection = npc.direction;
                projectile.rotation = npc.rotation;
                
                if (projectile.localAI[0] == 0)
                {
                    projectile.alpha += 17;
                    if (projectile.alpha > 255)
                    {
                        projectile.alpha = 255;
                        projectile.localAI[0] = 1;
                    }
                }
                else
                {
                    projectile.alpha -= 17;
                    if (projectile.alpha < 0)
                    {
                        projectile.alpha = 0;
                        projectile.localAI[0] = 0;
                    }
                }
            }
            else
            {
                projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            const int num226 = 80;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 40f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}