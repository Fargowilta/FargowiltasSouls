using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantFishron : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Resprites/NPC_370";

        int p = -1;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spectral Fishron");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 11;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 90;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 240;
            projectile.alpha = 100;
            cooldownSlot = 1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return target.hurtCooldowns[1] == 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(p);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            p = reader.ReadInt32();
        }

        public override bool CanDamage()
        {
            return projectile.localAI[0] > 85;
        }

        public override bool PreAI()
        {
            if (projectile.localAI[0] > 85) //dust during dash
            {
                int num22 = 5;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((projectile.width + 50) / 2f, projectile.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + projectile.Center;
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    Vector2 vector2_3 = vector2_2;
                    int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].shader = GameShaders.Armor.GetSecondaryShader(41, Main.LocalPlayer);
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= projectile.velocity;
                }
            }
            return true;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1;
                Main.PlaySound(SoundID.Zombie, (int)projectile.Center.X, (int)projectile.Center.Y, 20);
                p = Player.FindClosest(projectile.Center, 0, 0);
                projectile.netUpdate = true;
            }

            if (++projectile.localAI[0] > 85) //dash
            {
                projectile.rotation = projectile.velocity.ToRotation();
                projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0 ? 1 : -1;
                projectile.frameCounter = 5;
                projectile.frame = 6;
            }
            else //preparing to dash
            {
                int ai0 = p;
                //const float moveSpeed = 1f;
                if (projectile.localAI[0] == 85) //just about to dash
                {
                    projectile.velocity = Main.player[ai0].Center - projectile.Center;
                    projectile.velocity.Normalize();
                    projectile.velocity *= projectile.type == ModContent.ProjectileType<MutantFishron>() ? 24f : 20f;
                    projectile.rotation = projectile.velocity.ToRotation();
                    projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0 ? 1 : -1;
                    projectile.frameCounter = 5;
                    projectile.frame = 6;
                }
                else //regular movement
                {
                    Vector2 vel = Main.player[ai0].Center - projectile.Center;
                    projectile.rotation = vel.ToRotation();
                    if (vel.X > 0) //projectile is on left side of target
                    {
                        vel.X -= 300;
                        projectile.direction = projectile.spriteDirection = 1;
                    }
                    else //projectile is on right side of target
                    {
                        vel.X += 300;
                        projectile.direction = projectile.spriteDirection = -1;
                    }
                    Vector2 targetPos = Main.player[ai0].Center + new Vector2(projectile.ai[0], projectile.ai[1]);
                    Vector2 distance = (targetPos - projectile.Center) / 4f;
                    projectile.velocity = (projectile.velocity * 19f + distance) / 20f;
                    projectile.position += Main.player[ai0].velocity / 2f;
                    /*vel.Y -= 200f;
                    vel.Normalize();
                    vel *= 12f;
                    if (projectile.velocity.X < vel.X)
                    {
                        projectile.velocity.X += moveSpeed;
                        if (projectile.velocity.X < 0 && vel.X > 0)
                            projectile.velocity.X += moveSpeed;
                    }
                    else if (projectile.velocity.X > vel.X)
                    {
                        projectile.velocity.X -= moveSpeed;
                        if (projectile.velocity.X > 0 && vel.X < 0)
                            projectile.velocity.X -= moveSpeed;
                    }
                    if (projectile.velocity.Y < vel.Y)
                    {
                        projectile.velocity.Y += moveSpeed;
                        if (projectile.velocity.Y < 0 && vel.Y > 0)
                            projectile.velocity.Y += moveSpeed;
                    }
                    else if (projectile.velocity.Y > vel.Y)
                    {
                        projectile.velocity.Y -= moveSpeed;
                        if (projectile.velocity.Y > 0 && vel.Y < 0)
                            projectile.velocity.Y -= moveSpeed;
                    }*/
                    if (++projectile.frameCounter > 5)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame > 5)
                            projectile.frame = 0;
                    }
                }
            }
        }


        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(mod.BuffType("OceanicMaul"), 5400);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
            target.AddBuff(mod.BuffType("MutantNibble"), 900);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 900);
        }

        /*public override void Kill(int timeleft)
        {
            Main.PlaySound(SoundID.Item84, projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                SpawnRazorbladeRing(6, 12f, 1f);
                SpawnRazorbladeRing(6, 12f, -1f);
            }
        }

        private void SpawnRazorbladeRing(int max, float speed, float rotationModifier)
        {
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = projectile.velocity;
            vel.Normalize();
            vel *= speed;
            int type = mod.ProjectileType("MutantTyphoon");
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                int p = Projectile.NewProjectile(projectile.Center, vel, type, projectile.damage, 0f, Main.myPlayer, rotationModifier * Math.Sign(projectile.velocity.X), speed);
                if (p != 1000)
                    Main.projectile[p].timeLeft = 240;
            }
            Main.PlaySound(SoundID.Item84, projectile.Center);
        }*/

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (projectile.localAI[0] > 85)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
                {
                    Color color27 = Color.Lerp(color26, Color.Pink, 0.25f);
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / (1.5f * ProjectileID.Sets.TrailCacheLength[projectile.type]);
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    if (projectile.spriteDirection < 0)
                        num165 += (float)Math.PI;
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
                }
            }

            float drawRotation = projectile.rotation;
            if (projectile.spriteDirection < 0)
                drawRotation += (float)Math.PI;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), drawRotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float ratio = (255 - projectile.alpha) / 255f;
            float white = MathHelper.Lerp(ratio, 1f, 0.25f);
            if (white > 1f)
                white = 1f;
            return new Color((int)(lightColor.R * white), (int)(lightColor.G * white), (int)(lightColor.B * white), (int)(lightColor.A * ratio));
        }
    }
}