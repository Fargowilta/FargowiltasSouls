using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SaucerRocket : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_448";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Rocket");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.minion = true;
            projectile.tileCollide = false;
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
                    for (int index1 = 0; index1 < 8; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy((double)index1 * 3.14159274101257 / 4.0, new Vector2()) * new Vector2(2f, 8f)).RotatedBy((double)projectile.rotation - 1.57079637050629, new Vector2());
                        int index2 = Dust.NewDust(projectile.Center, 0, 0, 228, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1.5f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = projectile.Center + vector2;
                        Main.dust[index2].velocity = projectile.velocity * 0.0f;
                    }
                }
            }
            else //start homing
            {
                if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs && Main.npc[(int)projectile.ai[0]].CanBeChasedBy()) //have target
                {
                    double num4 = (double)(Main.npc[(int)projectile.ai[0]].Center - projectile.Center).ToRotation() - (double)projectile.velocity.ToRotation();
                    if (num4 > Math.PI)
                        num4 -= 2.0 * Math.PI;
                    if (num4 < -1.0 * Math.PI)
                        num4 += 2.0 * Math.PI;
                    projectile.velocity = projectile.velocity.RotatedBy(num4 * 0.2f, new Vector2());
                }
                else //retarget
                {
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000, true);
                    projectile.netUpdate = true;
                    if (projectile.ai[0] == -1) //no valid targets, selfdestruct
                        projectile.Kill();
                }

                projectile.tileCollide = true;
                if (++projectile.localAI[0] > 5)
                {
                    projectile.localAI[0] = 0f;
                    for (int index1 = 0; index1 < 4; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy((double)index1 * 3.14159274101257 / 4.0, new Vector2()) * new Vector2(2f, 4f)).RotatedBy((double)projectile.rotation - 1.57079637050629, new Vector2());
                        int index2 = Dust.NewDust(projectile.Center, 0, 0, 228, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1.5f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = projectile.Center + vector2;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.penetrate < 0)
                target.immune[projectile.owner] = 0;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.penetrate > -1)
            {
                projectile.penetrate = -1;
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
                projectile.Damage();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}