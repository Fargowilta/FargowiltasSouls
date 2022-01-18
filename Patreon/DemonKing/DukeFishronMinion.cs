using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DemonKing
{
    public class DukeFishronMinion : ModProjectile
    {
        private const float PI = (float)Math.PI;
        private float rotationOffset;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Duke Fishron");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 150;
            projectile.height = 100;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 10;
            projectile.GetGlobalProjectile<Projectiles.FargoGlobalProjectile>().CanSplit = false;
            projectile.scale *= 0.75f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(rotationOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            rotationOffset = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead
                && Main.player[projectile.owner].GetModPlayer<FargoPlayer>().DukeFishron)
            {
                projectile.timeLeft = 2;
            }

            if (projectile.Distance(Main.player[projectile.owner].Center) > 3000)
                projectile.Center = Main.player[projectile.owner].Center;

            if (projectile.localAI[0]++ > 30f) //timer handling everything else
            {
                projectile.localAI[0] = 0f;
                rotationOffset = Main.rand.NextFloat(-PI / 2, PI / 2);
                projectile.ai[1]++;
            }

            if (projectile.localAI[1] > 0) //timer for rings on hit
                projectile.localAI[1]--;

            if (projectile.ai[1] % 2 == 1) //dash
            {
                projectile.rotation = projectile.velocity.ToRotation();
                projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0 ? 1 : -1;
                projectile.frameCounter = 5;
                projectile.frame = 6;

                if (projectile.spriteDirection < 0)
                    projectile.rotation += (float)Math.PI;

                /*if (projectile.localAI[0] % 2 == 0 && Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(projectile.Center, 10f * Vector2.UnitX.RotatedByRandom(2 * Math.PI), 
                        ModContent.ProjectileType<DukeBubble>(), projectile.damage, projectile.knockBack, Main.myPlayer);
                }*/

                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((projectile.width + 50) / 2f, projectile.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + projectile.Center;
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    Vector2 vector2_3 = vector2_2;
                    int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= projectile.velocity;
                }
            }
            else //preparing to dash
            {
                int ai0 = (int)projectile.ai[0];
                float moveSpeed = 1f;
                if (projectile.localAI[0] == 30f) //just about to dash
                {
                    if (projectile.ai[0] >= 0 && Main.npc[ai0].CanBeChasedBy()) //has target
                    {
                        projectile.velocity = Main.npc[ai0].Center - projectile.Center + Main.npc[ai0].velocity * 10f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= 27f;
                        projectile.rotation = projectile.velocity.ToRotation();
                        projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0 ? 1 : -1;
                        projectile.frameCounter = 5;
                        projectile.frame = 6;

                        if (projectile.spriteDirection < 0)
                            projectile.rotation += (float)Math.PI;
                    }
                    else //no target
                    {
                        projectile.localAI[0] = -1f;
                        projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500);
                        projectile.netUpdate = true;
                        if (++projectile.frameCounter > 5)
                        {
                            projectile.frameCounter = 0;
                            if (++projectile.frame > 5)
                                projectile.frame = 0;
                        }
                    }
                }
                else //regular movement
                {
                    if (projectile.localAI[0] == 0)
                        projectile.localAI[0] = Main.rand.Next(10);

                    if (projectile.ai[0] >= 0 && Main.npc[ai0].CanBeChasedBy()) //has target
                    {
                        moveSpeed *= 1.5f;

                        Vector2 vel = Main.npc[ai0].Center - projectile.Center;
                        projectile.rotation = vel.ToRotation();
                        Vector2 offset = Vector2.Zero;
                        if (vel.X > 0) //projectile is on left side of target
                        {
                            offset.X = -360;
                            projectile.direction = projectile.spriteDirection = 1;
                        }
                        else //projectile is on right side of target
                        {
                            offset.X = 360;
                            projectile.direction = projectile.spriteDirection = -1;
                        }
                        offset = offset.RotatedBy(rotationOffset);
                        vel += offset;
                        vel.Normalize();
                        vel *= 24f;
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
                        }

                        if (projectile.spriteDirection < 0)
                            projectile.rotation += (float)Math.PI;
                    }
                    else //no target
                    {
                        Vector2 target = Main.player[projectile.owner].Center;
                        target.X -= 60 * Main.player[projectile.owner].direction * projectile.minionPos;
                        target.Y -= 40;
                        if (projectile.Distance(target) > 25)
                        {
                            moveSpeed *= 0.5f;

                            Vector2 vel = target - projectile.Center;
                            projectile.rotation = 0;
                            projectile.direction = projectile.spriteDirection = Main.player[projectile.owner].direction;
                            vel.Normalize();
                            vel *= 24f;
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
                            }
                        }

                        if (projectile.ai[0] != -1)
                        {
                            projectile.ai[0] = -1;
                            projectile.localAI[0] = 0;
                            projectile.netUpdate = true;
                        }

                        if (projectile.localAI[0] > 6)
                        {
                            projectile.localAI[0] = 0;
                            projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500);
                            projectile.netUpdate = true;
                        }
                    }
                    if (++projectile.frameCounter > 5)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame > 5)
                            projectile.frame = 0;
                    }
                }
            }
            projectile.position += projectile.velocity / 4f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.defense > 0)
                damage += target.defense / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("MutantNibble"), 900);

            if (projectile.localAI[1] <= 0)
            {
                projectile.localAI[1] = 60;

                Main.PlaySound(SoundID.Item84, projectile.Center); //rings on hit
                if (projectile.owner == Main.myPlayer)
                {
                    int modifier = Main.rand.NextBool() ? -1 : 1;
                    SpawnRazorbladeRing(8, 24f, -0.75f * modifier);
                    SpawnRazorbladeRing(8, 24f, 1.5f * modifier);
                }
            }
        }

        private void SpawnRazorbladeRing(int max, float speed, float rotationModifier)
        {
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = projectile.velocity;
            vel.Normalize();
            vel *= speed;
            int type = ModContent.ProjectileType<RazorbladeTyphoonFriendly2>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                Projectile.NewProjectile(projectile.Center, vel, type, projectile.damage,
                    projectile.knockBack / 4f, projectile.owner, rotationModifier * projectile.spriteDirection);
            }
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
                Color color27 = Color.Blue * projectile.Opacity * 0.2f;
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