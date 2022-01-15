using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CursedFireballHoming : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cursed Flame");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
        }
        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 1;
            }
            if (projectile.frame > 4)
            {
                projectile.frame = 0;
            }

            //Lighting.AddLight(projectile.Center, 0.35f * 0.8f, 0.8f, 0f);

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 20, 2, 0);
            }

            if (Main.rand.NextBool(3) && projectile.velocity.Length() > 0)
            {
                int index = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 75, projectile.velocity.X, projectile.velocity.Y, 100, default, 3f * projectile.scale);
                Main.dust[index].noGravity = true;
            }

            if (!(projectile.ai[0] > -1 && projectile.ai[0] < Main.maxPlayers))
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[1] > 20 && projectile.localAI[1] < 60)
            {
                float lerpspeed = 0.0235f + projectile.localAI[1] / 30000;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Zero, lerpspeed);
            }

            if (++projectile.localAI[1] == 60)
            {
                projectile.velocity = Vector2.Zero;
            }
            else if (projectile.localAI[1] == 120 + projectile.ai[1]) //shoot at player much faster
            {
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 20, 2, 0);
                float num = 24f;
                for (int index1 = 0; index1 < num; ++index1)
                {
                    Vector2 v = 2 * (Vector2.UnitX * 0.0f + -Vector2.UnitY.RotatedBy((double)index1 * (6.28318548202515 / (double)num), new Vector2()) * new Vector2(1f, 4f)).RotatedBy((double)(projectile.DirectionTo(Main.player[(int)projectile.ai[0]].Center).ToRotation()), new Vector2());
                    int index2 = Dust.NewDust(projectile.Center, 0, 0, 75, 0.0f, 0.0f, 200, default, 1f);
                    Main.dust[index2].scale = 2f;
                    Main.dust[index2].fadeIn = 1.3f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + v;
                    Main.dust[index2].velocity = v.SafeNormalize(Vector2.UnitY) * 1.5f;
                }

                projectile.velocity = projectile.DirectionTo(Main.player[(int)projectile.ai[0]].Center) * 16f;
            }

            if (projectile.localAI[1] < 120 + projectile.ai[1])
                projectile.alpha = 175;
            else
                projectile.alpha = 0;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2((float)projectile.position.X, (float)projectile.position.Y), projectile.width, projectile.height, 75, (float)(-projectile.velocity.X * 0.200000002980232), (float)(-projectile.velocity.Y * 0.200000002980232), 100, default, 2f * projectile.scale);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 2f;
                int index3 = Dust.NewDust(new Vector2((float)projectile.position.X, (float)projectile.position.Y), projectile.width, projectile.height, 75, (float)(-projectile.velocity.X * 0.200000002980232), (float)(-projectile.velocity.Y * 0.200000002980232), 100, default, 1f * projectile.scale);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool CanDamage()
        {
            if (projectile.localAI[1] < 120 + projectile.ai[1]) //prevent the projectile from being able to hurt the player before it's redirected at the player, since they move so fast initially it could cause cheap hits
                return false;

            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}