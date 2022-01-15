using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class TrueEyeL : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_650";

        private float localAI0;
        private float localAI1;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("True Eye of Cthulhu");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.Homing[projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 32;
            projectile.height = 42;
            projectile.timeLeft *= 5;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;

            projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && player.GetModPlayer<FargoPlayer>().TrueEyes)
            {
                projectile.timeLeft = 2;
                projectile.netUpdate = true;
            }

            if (projectile.damage == 0)
                projectile.damage = (int)(60f * player.minionDamage);

            //lighting effect?
            DelegateMethods.v3_1 = new Vector3(0.5f, 0.9f, 1f) * 1.5f;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 6f, 20f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(projectile.Left, projectile.Right, 20f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(player.Center, player.Center + player.velocity * 6f, 40f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));
            Utils.PlotTileLine(player.Left, player.Right, 40f, new Utils.PerLinePoint(DelegateMethods.CastLightOpen));

            if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
                    projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                projectile.localAI[0]++;
                NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc.CanBeChasedBy(projectile) || projectile.ai[1] == 2f)
                {
                    switch ((int)projectile.ai[1])
                    {
                        case 0: //true eye movement code
                            Vector2 newVel = npc.Center - projectile.Center + new Vector2(0f, -300f);
                            if (newVel != Vector2.Zero)
                            {
                                newVel.Normalize();
                                newVel *= 24f;
                                projectile.velocity.X = (projectile.velocity.X * 29 + newVel.X) / 30;
                                projectile.velocity.Y = (projectile.velocity.Y * 29 + newVel.Y) / 30;
                            }
                            if (projectile.Distance(npc.Center) < 150f)
                            {
                                if (projectile.Center.X < npc.Center.X)
                                    projectile.velocity.X -= 0.25f;
                                else
                                    projectile.velocity.X += 0.25f;

                                if (projectile.Center.Y < npc.Center.Y)
                                    projectile.velocity.Y -= 0.25f;
                                else
                                    projectile.velocity.Y += 0.25f;
                            }

                            if (projectile.localAI[0] > 120f)
                            {
                                if (projectile.Distance(npc.Center) > 1500f) //give up if too far
                                {
                                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000);
                                    projectile.ai[1] = 0f;
                                    projectile.localAI[1] = 0f;
                                }
                                projectile.localAI[0] = 0f;
                                projectile.ai[1]++;
                            }
                            break;

                        case 1: //slow down
                            projectile.velocity *= 0.95f;
                            if (projectile.velocity.Length() < 1f) //stop
                            {
                                projectile.velocity = Vector2.Zero;
                                projectile.localAI[0] = 0f;
                                projectile.ai[1]++;
                            }
                            break;

                        case 2: //firing laser
                            if (projectile.localAI[0] == 1f)
                            {
                                const float PI = (float)Math.PI;
                                float rotationDirection = PI * 2f / 3f / 90f; //positive is CW, negative is CCW
                                if (projectile.Center.X < npc.Center.X)
                                    rotationDirection *= -1;
                                localAI0 -= rotationDirection * 45f;
                                Vector2 speed = -Vector2.UnitX.RotatedBy(localAI0);
                                if (projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(projectile.Center - Vector2.UnitY * 6f, speed, mod.ProjectileType("PhantasmalDeathrayTrueEye"),
                                        projectile.damage / 3 * 10, 6f, projectile.owner, rotationDirection, projectile.identity);
                                projectile.localAI[1] = rotationDirection;
                            }
                            else if (projectile.localAI[0] > 90f)
                            {
                                projectile.localAI[0] = 0f;
                                projectile.ai[1]++;
                            }
                            else
                            {
                                localAI0 += projectile.localAI[1];
                            }
                            break;

                        default:
                            projectile.ai[1] = 0f;
                            goto case 0;
                    }
                }
                else //forget target
                {
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000);
                    projectile.ai[1] = 0f;
                    projectile.localAI[1] = 0f;
                }

                if (projectile.rotation > 3.14159274101257)
                    projectile.rotation = projectile.rotation - 6.283185f;
                projectile.rotation = projectile.rotation <= -0.005 || projectile.rotation >= 0.005 ? projectile.rotation * 0.96f : 0.0f;
                if (++projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                        projectile.frame = 0;
                }
                if (projectile.ai[1] != 2f) //custom pupil when attacking
                    UpdatePupil();
            }
            else
            {
                if (projectile.localAI[1]++ > 15f)
                {
                    projectile.localAI[0] = 0f;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000);
                    projectile.ai[1] = 0f;
                    projectile.localAI[1] = 0f;
                }

                Vector2 vector2_1 = player.GetModPlayer<FargoPlayer>().PungentEyeballMinion
                    ? new Vector2(0f, 75f) : new Vector2(0f, -75f); //vanilla movement code
                Vector2 vector2_2 = player.MountedCenter + vector2_1;
                float num1 = Vector2.Distance(projectile.Center, vector2_2);
                if (num1 > 1500) //teleport when out of range
                    projectile.Center = player.Center + vector2_1;
                Vector2 vector2_3 = vector2_2 - projectile.Center;
                float num2 = 4f;
                if (num1 < num2)
                    projectile.velocity *= 0.25f;
                if (vector2_3 != Vector2.Zero)
                {
                    if (vector2_3.Length() < num2)
                        projectile.velocity = vector2_3;
                    else
                        projectile.velocity = vector2_3 * 0.1f;
                }
                if (projectile.velocity.Length() > 6) //when moving fast, rotate in direction of velocity
                {
                    float num3 = projectile.velocity.ToRotation() + 1.570796f;
                    if (Math.Abs(projectile.rotation - num3) >= 3.14159274101257)
                        projectile.rotation = num3 >= projectile.rotation ? projectile.rotation + 6.283185f : projectile.rotation - 6.283185f;
                    projectile.rotation = (projectile.rotation * 11f + num3) / 12f;
                    if (++projectile.frameCounter >= 4)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame >= Main.projFrames[projectile.type])
                            projectile.frame = 0;
                    }
                }
                else //when moving slow, calm down
                {
                    if (projectile.rotation > 3.14159274101257)
                        projectile.rotation = projectile.rotation - 6.283185f;
                    projectile.rotation = projectile.rotation <= -0.005 || projectile.rotation >= 0.005 ? projectile.rotation * 0.96f : 0f;
                    if (++projectile.frameCounter >= 6)
                    {
                        projectile.frameCounter = 0;
                        if (++projectile.frame >= Main.projFrames[projectile.type])
                            projectile.frame = 0;
                    }
                }

                UpdatePupil();
            }
            /*Main.NewText("local0 " + localAI0.ToString()
                + " local1 " + localAI1.ToString()
                + " ai0 " + projectile.ai[0].ToString()
                + " ai1 " + projectile.ai[1].ToString());*/
        }

        private void UpdatePupil()
        {
            Player player = Main.player[projectile.owner];
            float f1 = (float)(localAI0 % 6.28318548202515 - 3.14159274101257);
            float num13 = (float)Math.IEEERemainder(localAI1, 1.0);
            if (num13 < 0.0)
                ++num13;
            float num14 = (float)Math.Floor(localAI1);
            float max = 0.999f;
            int num15 = 0;
            float amount = 0.1f;
            float f2;
            float num18;
            float num19;
            if (projectile.ai[0] != -1f) //targeted an enemy
            {
                f2 = projectile.AngleTo(Main.npc[(int)projectile.ai[0]].Center);
                num15 = 2;
                num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
                num19 = num14 + Math.Sign(-12f - num14);
            }
            else if (player.velocity.Length() > 3)
            {
                f2 = projectile.AngleTo(projectile.Center + player.velocity);
                num15 = 1;
                num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
                num19 = num14 + Math.Sign(-10f - num14);
            }
            else
            {
                f2 = player.direction == 1 ? 0.0f : 3.141603f;
                num18 = MathHelper.Clamp(num13 + Math.Sign(0.75f - num13) * 0.05f, 0.0f, max);
                num19 = num14 + Math.Sign(0.0f - num14);
                amount = 0.12f;
            }
            Vector2 rotationVector2 = f2.ToRotationVector2();
            localAI0 = (float)(Vector2.Lerp(f1.ToRotationVector2(), rotationVector2, amount).ToRotation() + num15 * 6.28318548202515 + 3.14159274101257);
            localAI1 = num19 + num18;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 360);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            int r = lightColor.R * 3 / 2;
            int g = lightColor.G * 3 / 2;
            int b = lightColor.B * 3 / 2;
            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;
            return new Color(r, g, b);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            Texture2D pupil = mod.GetTexture("Projectiles/Minions/TrueEyePupil");
            Vector2 pupilOffset = new Vector2(localAI1 / 2f, 0f).RotatedBy(localAI0);
            pupilOffset += new Vector2(0f, -6f).RotatedBy(projectile.rotation);
            Vector2 pupilOrigin = pupil.Size() / 2f;
            Main.spriteBatch.Draw(pupil, pupilOffset + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(pupil.Bounds), projectile.GetAlpha(lightColor), 0f, pupilOrigin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}