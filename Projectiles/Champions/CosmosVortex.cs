using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosVortex : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_578";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Vortex");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool CanDamage()
        {
            return projectile.scale >= 2;
        }

        public override void AI()
        {
            int time = 360;
            int maxScale = 3;

            if (projectile.ai[1] == 0)
                time = 30;

            void Suck()
            {
                Player player = Main.LocalPlayer;
                if (player.active && !player.dead && !player.ghost && projectile.Center != player.Center && projectile.Distance(player.Center) < 3000)
                {
                    float dragSpeed = projectile.Distance(player.Center) / 60;
                    player.position += projectile.DirectionFrom(player.Center) * dragSpeed;
                }
            };

            projectile.ai[0]++;
            if (projectile.ai[0] <= 50)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 4f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 2f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
            }
            else if (projectile.ai[0] <= 90)
            {
                projectile.scale = (projectile.ai[0] - 50) / 40 * maxScale;
                projectile.alpha = 255 - (int)(255 * projectile.scale / maxScale);
                projectile.rotation = projectile.rotation - 0.1570796f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }

                if (projectile.ai[1] != 0)
                    Suck();

                int p = Player.FindClosest(projectile.Center, 0, 0);
                if (p != -1)
                {
                    projectile.localAI[1] =
                        projectile.Center == Main.player[p].Center ? 0 : projectile.DirectionTo(Main.player[p].Center).ToRotation();
                    projectile.localAI[1] += (float)Math.PI * 2 / 3 / 2;
                }
            }
            else if (projectile.ai[0] <= 90 + time)
            {
                projectile.scale = maxScale;
                projectile.alpha = 0;
                projectile.rotation = projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515) * projectile.scale;
                    Dust dust = Main.dust[Dust.NewDust(projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.noGravity = true;
                    dust.position = projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = projectile.Center;
                }

                if (projectile.ai[1] != 0)
                {
                    Suck();

                    int lightningTime = (FargoSoulsWorld.EternityMode && projectile.ai[1] != 1f) ? 6 : 15;
                    if (projectile.localAI[0] % lightningTime == 0) //shoot lightning out, rotate 48 degrees per second by default
                    {
                        //projectile.localAI[0] = 0;

                        Main.PlaySound(SoundID.Item82, projectile.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 3;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 dir = Vector2.UnitX.RotatedBy(projectile.localAI[1] + 2 * (float)Math.PI / max * i);
                                float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                                Vector2 vel = Vector2.Normalize(dir) * 6f;
                                Projectile.NewProjectile(projectile.Center, vel * 6, ModContent.ProjectileType<CosmosLightning>(),
                                    projectile.damage, 0, Main.myPlayer, dir.ToRotation(), ai1New);
                            }
                        }

                        projectile.localAI[1] += MathHelper.ToRadians(48f / 60f * lightningTime) * projectile.ai[1];
                    }

                    //emode, ai1 check is a phase 2 check
                    if (FargoSoulsWorld.EternityMode && projectile.ai[1] != 1f && projectile.localAI[0] % 75 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 7;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 dir = Vector2.UnitX.RotatedBy(-projectile.localAI[1] + 2 * (float)Math.PI / max * i);
                                int p = Projectile.NewProjectile(projectile.Center, dir * 4, ModContent.ProjectileType<CosmosLightningOrb>(), projectile.damage, 0, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 90 + time - (int)projectile.ai[0] + 60;
                            }
                        }
                    }

                    projectile.localAI[0]++;
                }
            }
            else
            {
                projectile.scale = (float)(1.0 - (projectile.ai[0] - time) / 60.0) * maxScale;
                projectile.alpha = 255 - (int)(255 * projectile.scale / maxScale);
                projectile.rotation = projectile.rotation - (float)Math.PI / 30f;
                if (projectile.alpha >= 255)
                    projectile.Kill();
                
                Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * projectile.scale;
                Dust dust1 = Main.dust[Dust.NewDust(projectile.Center - spinningpoint1 * 30f, 0, 0, 229, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust1.noGravity = true;
                dust1.position = projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                dust1.velocity = spinningpoint1.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                dust1.scale = 0.5f + Main.rand.NextFloat();
                dust1.fadeIn = 0.5f;
                dust1.customData = projectile.Center;
            }

            if (Main.rand.NextBool())
            {
                Dust dust3 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 0, new Color(), 1f)];
                dust3.velocity *= 5f;
                dust3.fadeIn = 1f;
                dust3.scale = 1f + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                dust3.noGravity = true;
            }

            float num1 = 0.5f;
            for (int i = 0; i < 5; ++i)
            {
                if (Main.rand.NextFloat() >= num1)
                {
                    float f = Main.rand.NextFloat() * 6.283185f;
                    float num2 = Main.rand.NextFloat();
                    Dust dust = Dust.NewDustPerfect(projectile.Center + f.ToRotationVector2() * (110 + 600 * num2), 229, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                    dust.scale = 0.9f;
                    dust.fadeIn = 1.15f + num2 * 0.3f;
                    //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                    dust.noGravity = true;
                    //dust.noLight = true;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 360);
            //if (FargoSoulsWorld.MasochistMode) target.AddBuff(ModContent.BuffType<Buffs.Masomode.LightningRod>(), 360);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item92, projectile.Center);
            int type = 229;
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (Main.rand.Next(3) != 0)
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}