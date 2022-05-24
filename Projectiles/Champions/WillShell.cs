using FargowiltasSouls.NPCs.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillShell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Will Shell");
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;

            Projectile.scale = 1.5f;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<WillChampion>());
            if (npc != null && npc.ai[0] == -1f) //disappear when champ is vulnerable again
            {
                Projectile.Center = npc.Center;
                Projectile.direction = Projectile.spriteDirection = npc.direction;
                Projectile.rotation = npc.rotation;

                if (Projectile.localAI[0] == 0)
                {
                    Projectile.alpha += 17;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                        Projectile.localAI[0] = 1;
                    }
                }
                else
                {
                    Projectile.alpha -= 17;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                        Projectile.localAI[0] = 0;
                    }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            const int num226 = 80;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 40f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 174, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}