using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyPhoenix : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_706";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantom Phoenix");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.DD2PhoenixBowShot];
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
            projectile.hostile = true;
        }

        public override void AI()
        {
            if (--projectile.ai[1] < 0 && projectile.ai[1] > -300)
            {
                Player p = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
                if (p != null)
                {
                    projectile.velocity.Normalize();
                    projectile.velocity *= 18f;

                    //if (projectile.localAI[0] == 0) projectile.localAI[0] = projectile.Center.X < p.Center.X ? 1 : -1;

                    Vector2 target = p.Center;
                    //target.X += projectile.localAI[0] * 200;

                    if (projectile.Distance(target) > 200)
                    {
                        Vector2 distance = target - projectile.Center;

                        double angle = distance.ToRotation() - projectile.velocity.ToRotation();
                        if (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        if (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        projectile.velocity = projectile.velocity.RotatedBy(angle * 0.2);
                    }
                    else
                    {
                        projectile.ai[1] = -300;
                    }
                }
                else
                {
                    projectile.ai[0] = Player.FindClosest(projectile.Center, 0, 0);
                }
            }

            if (projectile.alpha <= 0) //vanilla display code
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        Dust dust = Dust.NewDustDirect(projectile.Center - projectile.Size / 4f, projectile.width / 2, projectile.height / 2,
                            Utils.SelectRandom(Main.rand, new int[3] { 6, 31, 31 }), 0.0f, 0.0f, 0, default, 1f);
                        dust.noGravity = true;
                        dust.velocity *= 2.3f;
                        dust.fadeIn = 1.5f;
                        dust.noLight = true;
                    }
                }
                Vector2 vector2_1 = 16f * new Vector2(0f, (float)Math.Cos(projectile.frameCounter * 6.28318548202515 / 40.0 - 1.57079637050629)).RotatedBy(projectile.rotation);
                Vector2 vector2_2 = Vector2.Normalize(projectile.velocity);

                Dust dust1 = Dust.NewDustDirect(projectile.Center - projectile.Size / 4f, projectile.width / 2, projectile.height / 2, 6, 0.0f, 0.0f, 0, default, 1f);
                dust1.noGravity = true;
                dust1.position = projectile.Center + vector2_1;
                dust1.velocity = Vector2.Zero;
                dust1.fadeIn = 1.4f;
                dust1.scale = 1.15f;
                dust1.noLight = true;
                dust1.position += projectile.velocity * 1.2f;
                dust1.velocity += vector2_2 * 2f;

                Dust dust2 = Dust.NewDustDirect(projectile.Center - projectile.Size / 4f, projectile.width / 2, projectile.height / 2, 6, 0.0f, 0.0f, 0, default, 1f);
                dust2.noGravity = true;
                dust2.position = projectile.Center + vector2_1;
                dust2.velocity = Vector2.Zero;
                dust2.fadeIn = 1.4f;
                dust2.scale = 1.15f;
                dust2.noLight = true;
                dust2.position += projectile.velocity * 0.5f;
                dust2.position += projectile.velocity * 1.2f;
                dust2.velocity += vector2_2 * 2f;
            }
            int num9 = projectile.frameCounter + 1;
            projectile.frameCounter = num9;
            if (num9 >= 40)
                projectile.frameCounter = 0;
            projectile.frame = projectile.frameCounter / 5;
            if (projectile.alpha > 0)
            {
                projectile.alpha = projectile.alpha - 55;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                    float num1 = 16f;
                    for (int index1 = 0; index1 < num1; ++index1)
                    {
                        Vector2 vector2 = -Vector2.UnitY.RotatedBy(index1 * (6.28318548202515 / num1)) * new Vector2(1f, 4f);
                        vector2 = vector2.RotatedBy(projectile.velocity.ToRotation());
                        int index2 = Dust.NewDust(projectile.Center, 0, 0, 6, 0.0f, 0.0f, 0, default, 1f);
                        Main.dust[index2].scale = 1.5f;
                        Main.dust[index2].noLight = true;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = projectile.Center + vector2;
                        Main.dust[index2].velocity = Main.dust[index2].velocity * 4f + projectile.velocity * 0.3f;
                    }
                }
            }
            DelegateMethods.v3_1 = new Vector3(1f, 0.6f, 0.2f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 4f, 40f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));

            projectile.direction = projectile.spriteDirection = projectile.velocity.X < 0 ? -1 : 1;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.direction < 0)
                projectile.rotation += (float)Math.PI;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, 300);
            target.AddBuff(BuffID.WitheredWeapon, 300);
            target.AddBuff(BuffID.Burning, 300);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}