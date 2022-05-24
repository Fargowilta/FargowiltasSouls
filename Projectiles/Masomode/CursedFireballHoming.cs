using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CursedFireballHoming : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Flame");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 1;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }

            //Lighting.AddLight(Projectile.Center, 0.35f * 0.8f, 0.8f, 0);

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item20 with { Volume = 2, Pitch = 0 }, Projectile.Center);
            }

            if (Main.rand.NextBool(3) && Projectile.velocity.Length() > 0)
            {
                int index = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 75, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 3f * Projectile.scale);
                Main.dust[index].noGravity = true;
            }

            if (!(Projectile.ai[0] > -1 && Projectile.ai[0] < Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[1] > 20 && Projectile.localAI[1] < 60)
            {
                float lerpspeed = 0.0235f + Projectile.localAI[1] / 30000;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, lerpspeed);
            }

            if (++Projectile.localAI[1] == 60)
            {
                Projectile.velocity = Vector2.Zero;
            }
            else if (Projectile.localAI[1] == 120 + Projectile.ai[1]) //shoot at player much faster
            {
                SoundEngine.PlaySound(SoundID.Item20 with { Volume = 2, Pitch = 0 }, Projectile.Center);
                float num = 24f;
                for (int index1 = 0; index1 < num; ++index1)
                {
                    Vector2 v = 2 * (Vector2.UnitX * 0.0f + -Vector2.UnitY.RotatedBy((double)index1 * (6.28318548202515 / (double)num), new Vector2()) * new Vector2(1f, 4f)).RotatedBy((double)(Projectile.DirectionTo(Main.player[(int)Projectile.ai[0]].Center).ToRotation()), new Vector2());
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, 75, 0.0f, 0.0f, 200, default, 1f);
                    Main.dust[index2].scale = 2f;
                    Main.dust[index2].fadeIn = 1.3f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + v;
                    Main.dust[index2].velocity = v.SafeNormalize(Vector2.UnitY) * 1.5f;
                }

                Projectile.velocity = Projectile.DirectionTo(Main.player[(int)Projectile.ai[0]].Center) * 16f;
            }

            if (Projectile.localAI[1] < 120 + Projectile.ai[1])
                Projectile.alpha = 175;
            else
                Projectile.alpha = 0;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2((float)Projectile.position.X, (float)Projectile.position.Y), Projectile.width, Projectile.height, 75, (float)(-Projectile.velocity.X * 0.200000002980232), (float)(-Projectile.velocity.Y * 0.200000002980232), 100, default, 2f * Projectile.scale);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 2f;
                int index3 = Dust.NewDust(new Vector2((float)Projectile.position.X, (float)Projectile.position.Y), Projectile.width, Projectile.height, 75, (float)(-Projectile.velocity.X * 0.200000002980232), (float)(-Projectile.velocity.Y * 0.200000002980232), 100, default, 1f * Projectile.scale);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool? CanDamage()
        {
            if (Projectile.localAI[1] < 120 + Projectile.ai[1]) //prevent the Projectile from being able to hurt the player before it's redirected at the player, since they move so fast initially it could cause cheap hits
                return false;

            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}