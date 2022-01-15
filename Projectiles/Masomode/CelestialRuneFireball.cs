using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CelestialRuneFireball : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_467";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fireball");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 360;
            projectile.penetrate = 1;

            if (ModLoader.GetMod("Fargowiltas") != null)
                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", projectile);
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1]);
            if (npc != null)
            {
                if (npc.CanBeChasedBy(projectile))
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = npc.Center - projectile.Center;
                    if (vel.Length() < 20f)
                    {
                        projectile.Kill();
                        return;
                    }
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.008f + 0.08f * Math.Min(1f, projectile.ai[0] / 180)));

                    if (projectile.timeLeft % projectile.MaxUpdates == 0)
                        projectile.ai[0]++;
                }
                else
                {
                    projectile.ai[1] = -1f;
                }
            }
            else if (projectile.localAI[0] == 0f) //also used for dust timer btw
            {
                projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000, true);
                projectile.netUpdate = true;
            }

            projectile.alpha -= 40;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            projectile.frameCounter++;
            if (projectile.frameCounter > 2)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                    projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, 1.1f, 0.9f, 0.4f);
            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;

            ++projectile.localAI[0];
            if ((double)projectile.localAI[0] == 12.0) //loads of vanilla dust :echprime:
            {
                projectile.localAI[0] = 0.0f;
                for (int index1 = 0; index1 < 12; ++index1)
                {
                    Vector2 vector2 = (Vector2.UnitX * (float)-projectile.width / 2f + -Vector2.UnitY.RotatedBy((double)index1 * 3.14159274101257 / 6.0, new Vector2()) * new Vector2(8f, 16f)).RotatedBy((double)projectile.rotation - 1.57079637050629, new Vector2());
                    int index2 = Dust.NewDust(projectile.Center, 0, 0, 6, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = 1.1f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2;
                    Main.dust[index2].velocity = projectile.velocity * 0.1f;
                    Main.dust[index2].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[index2].position) * 1.25f;
                }
            }
            if (Main.rand.NextBool(4))
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1f);
                    Main.dust[index2].velocity *= 0.1f;
                    Main.dust[index2].position = projectile.Center + vector2 * (float)projectile.width / 2f;
                    Main.dust[index2].fadeIn = 0.9f;
                }
            }
            if (Main.rand.NextBool(32))
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.392699092626572).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 155, new Color(), 0.8f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].position = projectile.Center + vector2 * (float)projectile.width / 2f;
                    if (Main.rand.NextBool())
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
            if (Main.rand.NextBool())
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    Vector2 vector2 = -Vector2.UnitX.RotatedByRandom(0.785398185253143).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0.0f, 0.0f, 0, new Color(), 1.2f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2 * (float)projectile.width / 2f;
                    if (Main.rand.NextBool())
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 240);
            if (projectile.penetrate == -1)
                target.immune[projectile.owner] = 0;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                projectile.penetrate = -1;
                projectile.position = projectile.Center;
                Main.PlaySound(SoundID.Item14, projectile.position);
                projectile.width = projectile.height = 176;
                projectile.Center = projectile.position;
                projectile.Damage();
                for (int index1 = 0; index1 < 4; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                }
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0.0f, 0.0f, 200, new Color(), 3.7f);
                    Main.dust[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity = dust1.velocity * 3f;
                    int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index3].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Dust dust2 = Main.dust[index3];
                    dust2.velocity = dust2.velocity * 2f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].fadeIn = 2.5f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0.0f, 0.0f, 0, new Color(), 2.7f);
                    Main.dust[index2].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2()) * (float)projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity = dust.velocity * 3f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 0, new Color(), 1.5f);
                    Main.dust[index2].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2()) * (float)projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity = dust.velocity * 3f;
                }
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    int index2 = Gore.NewGore(projectile.position + new Vector2((float)(projectile.width * Main.rand.Next(100)) / 100f, (float)(projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Gore gore = Main.gore[index2];
                    gore.velocity = gore.velocity * 0.3f;
                    Main.gore[index2].velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                    Main.gore[index2].velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 150, 150, 255) * (1f - projectile.alpha / 255f);
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