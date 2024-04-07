using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FusedExplosion : MoonLordSunBlast
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 5f;
            Projectile.friendly = true;
            CooldownSlot = -1;
        }

        public override void AI()
        {
            if (Projectile.position.HasNaNs())
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }

            if (Projectile.localAI[1] == 0)
                SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

            if (++Projectile.localAI[1] == 6)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                for (int num615 = 0; num615 < 45; num615++)
                {
                    int num616 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num616].velocity *= 1.4f;
                }

                for (int num617 = 0; num617 < 30; num617++)
                {
                    int num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                    Main.dust[num618].noGravity = true;
                    Main.dust[num618].velocity *= 7f;
                    num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num618].velocity *= 3f;
                }

                for (int num619 = 0; num619 < 3; num619++)
                {
                    float scaleFactor9 = 0.4f;
                    if (num619 == 1) scaleFactor9 = 0.8f;
                    int num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore97 = Main.gore[num620];
                    gore97.velocity.X++;
                    Gore gore98 = Main.gore[num620];
                    gore98.velocity.Y++;
                    num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore99 = Main.gore[num620];
                    gore99.velocity.X--;
                    Gore gore100 = Main.gore[num620];
                    gore100.velocity.Y++;
                    num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore101 = Main.gore[num620];
                    gore101.velocity.X++;
                    Gore gore102 = Main.gore[num620];
                    gore102.velocity.Y--;
                    num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore103 = Main.gore[num620];
                    gore103.velocity.X--;
                    Gore gore104 = Main.gore[num620];
                    gore104.velocity.Y--;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //overwrite debuffs to not inflict any
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;
            //color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (4 - Projectile.ai[1]) / 4);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color,
                Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

