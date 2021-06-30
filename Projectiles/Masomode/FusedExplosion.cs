using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FusedExplosion : MoonLordSunBlast
    {
        public override string Texture => "Terraria/Projectile_687";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.scale = 5f;
            projectile.friendly = true;
            cooldownSlot = -1;
        }

        public override void AI()
        {
            if (projectile.position.HasNaNs())
            {
                projectile.Kill();
                return;
            }

            if (++projectile.frameCounter >= 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame--;
                    projectile.Kill();
                }
            }

            if (projectile.localAI[1] == 0)
                Main.PlaySound(SoundID.Item88, projectile.Center);

            if (++projectile.localAI[1] == 6)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 14);

                for (int num615 = 0; num615 < 45; num615++)
                {
                    int num616 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num616].velocity *= 1.4f;
                }

                for (int num617 = 0; num617 < 30; num617++)
                {
                    int num618 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[num618].noGravity = true;
                    Main.dust[num618].velocity *= 7f;
                    num618 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num618].velocity *= 3f;
                }

                for (int num619 = 0; num619 < 3; num619++)
                {
                    float scaleFactor9 = 0.4f;
                    if (num619 == 1) scaleFactor9 = 0.8f;
                    int num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore97 = Main.gore[num620];
                    gore97.velocity.X = gore97.velocity.X + 1f;
                    Gore gore98 = Main.gore[num620];
                    gore98.velocity.Y = gore98.velocity.Y + 1f;
                    num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore99 = Main.gore[num620];
                    gore99.velocity.X = gore99.velocity.X - 1f;
                    Gore gore100 = Main.gore[num620];
                    gore100.velocity.Y = gore100.velocity.Y + 1f;
                    num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore101 = Main.gore[num620];
                    gore101.velocity.X = gore101.velocity.X + 1f;
                    Gore gore102 = Main.gore[num620];
                    gore102.velocity.Y = gore102.velocity.Y - 1f;
                    num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore103 = Main.gore[num620];
                    gore103.velocity.X = gore103.velocity.X - 1f;
                    Gore gore104 = Main.gore[num620];
                    gore104.velocity.Y = gore104.velocity.Y - 1f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //overwrite debuffs to not inflict any
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;
            //color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (4 - projectile.ai[1]) / 4);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color,
                projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

