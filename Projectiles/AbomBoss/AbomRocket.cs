using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomRocket : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_448";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Rocket");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.alpha = 0;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.scale = 2.5f;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.ai[1] > 0) //when first spawned just move straight
            {
                projectile.timeLeft++; //don't expire while counting down

                if (--projectile.ai[1] == 0) //do for one tick right before homing
                {
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * (projectile.velocity.Length() + 6f);
                    projectile.netUpdate = true;
                    for (int index1 = 0; index1 < 36; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy((double)index1 * 3.14159274101257 / 36 * 2, new Vector2()) * new Vector2(2f, 8f)).RotatedBy((double)projectile.rotation - 1.57079637050629, new Vector2());
                        int index2 = Dust.NewDust(projectile.Center, 0, 0, 228, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = projectile.Center + vector2 * 6f;
                        Main.dust[index2].velocity = projectile.velocity * 0.0f;
                    }
                }
            }
            else //start homing
            {
                if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxPlayers) //have target
                {
                    if (--projectile.ai[1] > -45) //only home for a bit
                    {
                        double num4 = (Main.player[(int)projectile.ai[0]].Center - projectile.Center).ToRotation() - (double)projectile.velocity.ToRotation();
                        if (num4 > Math.PI)
                            num4 -= 2.0 * Math.PI;
                        if (num4 < -1.0 * Math.PI)
                            num4 += 2.0 * Math.PI;
                        projectile.velocity = projectile.velocity.RotatedBy(num4 * 0.05f, new Vector2());
                    }
                }
                else //retarget
                {
                    projectile.ai[0] = Player.FindClosest(projectile.Center, 0, 0);
                }

                projectile.tileCollide = true;
                if (++projectile.localAI[0] > 10)
                {
                    projectile.localAI[0] = 0f;
                    for (int index1 = 0; index1 < 36; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy((double)index1 * 3.14159274101257 / 36 * 2, new Vector2()) * new Vector2(2f, 4f)).RotatedBy((double)projectile.rotation - 1.57079637050629, new Vector2());
                        int index2 = Dust.NewDust(projectile.Center, 0, 0, 228, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = projectile.Center + vector2 * 6f;
                        Main.dust[index2].velocity = projectile.velocity * 0.0f;
                    }
                }
            }

            projectile.rotation = projectile.velocity.ToRotation() + 1.570796f;

            Vector2 vector21 = Vector2.UnitY.RotatedBy(projectile.rotation, new Vector2()) * 8f * 2;
            int index21 = Dust.NewDust(projectile.Center, 0, 0, 228, 0.0f, 0.0f, 0, new Color(), 1f);
            Main.dust[index21].position = projectile.Center + vector21;
            Main.dust[index21].scale = 1f;
            Main.dust[index21].noGravity = true;

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                    projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(mod.BuffType("Defenseless"), 300);
                //target.AddBuff(BuffID.Confused, 180);
            }
            target.AddBuff(BuffID.BrokenArmor, 300);
            projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 112;
            projectile.position.X -= (float)(projectile.width / 2);
            projectile.position.Y -= (float)(projectile.height / 2);
            for (int index = 0; index < 4; ++index)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 228, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 3f;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 228, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity = dust2.velocity * 2f;
                Main.dust[index3].noGravity = true;
            }
            for (int index1 = 0; index1 < 1; ++index1)
            {
                int index2 = Gore.NewGore(projectile.position + new Vector2((float)(projectile.width * Main.rand.Next(100)) / 100f, (float)(projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                Gore gore = Main.gore[index2];
                gore.velocity = gore.velocity * 0.3f;
                Main.gore[index2].velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[index2].velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}