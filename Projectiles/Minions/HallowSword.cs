using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class HallowSword : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HallowSword");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.CloneDefaults(ProjectileID.EmpressBlade);
            AIType = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.timeLeft = 18000;
            Projectile.minionSlots = 0;
            Projectile.hide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.whoAmI == Main.myPlayer && (player.dead || !modPlayer.AncientHallowEnchantActive || !player.GetToggleValue("Hallowed")))
            {
                Projectile.Kill();
                return;
            }

            List<int> ai156_blacklistedTargets = new List<int>();

            DelegateMethods.v3_1 = Color.Transparent.ToVector3();
            Point point2 = Projectile.Center.ToTileCoordinates();
            DelegateMethods.CastLightOpen(point2.X, point2.Y);

            ai156_blacklistedTargets.Clear();

            //AI_156_Think
            int num;// = 60;
            int num2;// = num - 1;
            int num3;// = num + 60;
            int num4;// = num3 - 1;
            int num5;// = num + 1;

            num = 40;
            num2 = num - 1;
            num3 = num + 40;
            num4 = num3 - 1;
            num5 = num + 1;

            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 1000f)
            {
                Projectile.ai[0] = 0f;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == -1f)
            {
                //int stackedIndex;
                //int totalIndexes;
                //AI_GetMyGroupIndexAndFillBlackList(ai156_blacklistedTargets, out stackedIndex, out totalIndexes);
                Vector2 vector;
                float targetAngle;
                AI_156_GetIdlePosition(1, 1, out vector, out targetAngle);
                Projectile.velocity = Vector2.Zero;

                if (player.Center.X > Projectile.Center.X)
                {
                    Projectile.Center = Projectile.Center.MoveTowards(vector, 15f);
                }
                else
                {
                    Projectile.Center = Projectile.Center.MoveTowards(vector, 32f);
                }

                Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle, 0.2f);
                if (Projectile.Distance(vector) < 2f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }
            }
            else if (Projectile.ai[0] == 0f)
            {
                int stackedIndex3;
                int totalIndexes3;
                //AI_GetMyGroupIndexAndFillBlackList(ai156_blacklistedTargets, out stackedIndex3, out totalIndexes3);
                Vector2 value2;
                float targetAngle2;
                AI_156_GetIdlePosition(1, 1, out value2, out targetAngle2);
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Vector2.SmoothStep(Projectile.Center, value2, 0.45f);
                Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle2, 0.45f);
                if (Main.rand.Next(20) == 0)
                {
                    int num8 = AI_156_TryAttackingNPCs(ai156_blacklistedTargets, false);
                    if (num8 != -1)
                    {
                        Projectile.ai[0] = (float)Main.rand.NextFromList(new int[]
                        {
                                             num,
                                             num3
                        });
                        Projectile.ai[0] = (float)num3;
                        Projectile.ai[1] = (float)num8;
                        Projectile.netUpdate = true;
                        return;
                    }
                }
            }
            else
            {
                bool skipBodyCheck = true;
                int num14 = 0;
                int num15 = num2;
                int num16 = 0;
                if (Projectile.ai[0] >= (float)num5)
                {
                    num14 = 1;
                    num15 = num4;
                    num16 = num5;
                }
                int num17 = (int)Projectile.ai[1];
                if (!Main.npc.IndexInRange(num17))
                {
                    int num18 = AI_156_TryAttackingNPCs(ai156_blacklistedTargets, skipBodyCheck);
                    if (num18 != -1)
                    {
                        Projectile.ai[0] = (float)Main.rand.NextFromList(new int[]
                        {
                                                 num,
                                                 num3
                        });
                        Projectile.ai[1] = (float)num18;
                        Projectile.netUpdate = true;
                        return;
                    }
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }
                else
                {
                    NPC npc2 = Main.npc[num17];
                    if (!npc2.CanBeChasedBy(Projectile, false))
                    {
                        int num19 = AI_156_TryAttackingNPCs(ai156_blacklistedTargets, skipBodyCheck);
                        if (num19 != -1)
                        {
                            Projectile.ai[0] = (float)Main.rand.NextFromList(new int[]
                            {
                                                     num,
                                                     num3
                            });
                            Projectile.ai[1] = (float)num19;
                            Projectile.netUpdate = true;
                            return;
                        }
                        Projectile.ai[0] = -1f;
                        Projectile.ai[1] = 0f;
                        Projectile.netUpdate = true;
                        return;
                    }
                    else
                    {
                        //actual attacking is here
                        Projectile.ai[0] -= 1f;
                        if (Projectile.ai[0] >= (float)num15)
                        {
                            Projectile.direction = ((Projectile.Center.X < npc2.Center.X) ? 1 : -1);
                            if (Projectile.ai[0] == (float)num15)
                            {
                                Projectile.localAI[0] = Projectile.Center.X;
                                Projectile.localAI[1] = Projectile.Center.Y;
                            }
                        }
                        float lerpValue2 = Utils.GetLerpValue((float)num15, (float)num16, Projectile.ai[0], true);

                        //spinning attack
                        if (num14 == 0)
                        {
                            Vector2 vector3 = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                            if (lerpValue2 >= 0.5f)
                            {
                                vector3 = Vector2.Lerp(npc2.Center, Main.player[Projectile.owner].Center, 0.5f);
                            }
                            Vector2 center4 = npc2.Center;
                            float num20 = (center4 - vector3).ToRotation();
                            float num21 = (Projectile.direction == 1) ? -3.14159274f : 3.14159274f;
                            float num22 = num21 + -num21 * lerpValue2 * 2f;
                            Vector2 vector4 = num22.ToRotationVector2();
                            vector4.Y *= 0.5f;
                            vector4.Y *= 0.8f + (float)Math.Sin((double)((float)Projectile.identity * 2.3f)) * 0.2f;
                            vector4 = vector4.RotatedBy((double)num20, default(Vector2));
                            float scaleFactor2 = (center4 - vector3).Length() / 2f;
                            Vector2 center5 = Vector2.Lerp(vector3, center4, 0.5f) + vector4 * scaleFactor2;
                            Projectile.Center = center5;
                            float num23 = MathHelper.WrapAngle(num20 + num22 + 0f);
                            Projectile.rotation = num23 + 1.57079637f;
                            Vector2 velocity2 = num23.ToRotationVector2() * 10f;
                            Projectile.velocity = velocity2;
                            Projectile.position -= Projectile.velocity;
                        }
                        //stab attack
                        if (num14 == 1)
                        {
                            Vector2 vector5 = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                            vector5 += new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, lerpValue2, true) * -100f);
                            Vector2 v = npc2.Center - vector5;
                            Vector2 value3 = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
                            Vector2 value4 = npc2.Center + value3;
                            float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, lerpValue2, true);
                            float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, lerpValue2, true);
                            float targetAngle3 = v.SafeNormalize(Vector2.Zero).ToRotation() + 1.57079637f;
                            Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle3, 0.628318548f);
                            Projectile.Center = Vector2.Lerp(vector5, npc2.Center, lerpValue3);
                            if (lerpValue4 > 0f)
                            {
                                Projectile.Center = Vector2.Lerp(npc2.Center, value4, lerpValue4);
                            }
                        }
                        if (Projectile.ai[0] == (float)num16)
                        {
                            int num24 = AI_156_TryAttackingNPCs(ai156_blacklistedTargets, skipBodyCheck);
                            if (num24 != -1)
                            {
                                Projectile.ai[0] = (float)Main.rand.NextFromList(new int[]
                                {
                                                             num,
                                                             num3
                                });
                                Projectile.ai[1] = (float)num24;
                                Projectile.netUpdate = true;
                                return;
                            }
                            Projectile.ai[0] = -1f;
                            Projectile.ai[1] = 0f;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }

        }

        //private void AI_GetMyGroupIndexAndFillBlackList(List<int> blackListedTargets, out int index, out int totalIndexesInGroup)
        //{
        //    index = 0;
        //    totalIndexesInGroup = 0;
        //    for (int i = 0; i < 1000; i++)
        //    {
        //        Projectile projectile = Main.projectile[i];
        //        if (projectile.active && projectile.owner == Projectile.owner && projectile.type == Projectile.type && (projectile.type != 759 || projectile.frame == Main.projFrames[projectile.type] - 1))
        //        {
        //            if (Projectile.whoAmI > i)
        //            {
        //                index++;
        //            }
        //            totalIndexesInGroup++;
        //        }
        //    }
        //}

        private void AI_156_GetIdlePosition(int stackedIndex, int totalIndexes, out Vector2 idleSpot, out float idleRotation)
        {
            Player player = Main.player[Projectile.owner];
            //idleRotation = 0f;
            //idleSpot = Vector2.Zero;

            int num2 = stackedIndex + 1;
            idleRotation = (float)num2 * 6.28318548f * 0.0166666675f * (float)player.direction + 1.57079637f;
            idleRotation = MathHelper.WrapAngle(idleRotation);
            int num3 = num2 % totalIndexes;
            Vector2 value = new Vector2(0f, 0.5f).RotatedBy((double)((player.miscCounterNormalized * (2f + (float)num3) + (float)num3 * 0.5f + (float)player.direction * 1.3f) * 6.28318548f), default(Vector2)) * 4f;
            idleSpot = idleRotation.ToRotationVector2() * 10f + player.MountedCenter + new Vector2((float)(player.direction * (num2 * -6 - 16)), player.gravDir * -15f);
            idleSpot += value;
            idleRotation += 1.57079637f;
        }

        private int AI_156_TryAttackingNPCs(List<int> blackListedTargets, bool skipBodyCheck = false)
        {
            float attackDistance = 750;

            Vector2 center = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this, false))
            {
                bool flag = true;
                if (!ownerMinionAttackTargetNPC.boss && blackListedTargets.Contains(ownerMinionAttackTargetNPC.whoAmI))
                {
                    flag = false;
                }
                if (ownerMinionAttackTargetNPC.Distance(center) > attackDistance)
                {
                    flag = false;
                }
                if (!skipBodyCheck && !Projectile.CanHitWithOwnBody(ownerMinionAttackTargetNPC))
                {
                    flag = false;
                }
                if (flag)
                {
                    return ownerMinionAttackTargetNPC.whoAmI;
                }
            }
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this, false) && (npc.boss || !blackListedTargets.Contains(i)))
                {
                    float num2 = npc.Distance(center);
                    if (num2 <= attackDistance && (num2 <= num || num == -1f) && (skipBodyCheck || Projectile.CanHitWithOwnBody(npc)))
                    {
                        num = num2;
                        result = i;
                    }
                }
            }
            return result;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EmpressBladeDrawer empressBladeDrawer = default(EmpressBladeDrawer);
            float num14 = Main.GlobalTimeWrappedHourly % 3f / 3f;
            Player player = Main.player[Projectile.owner];
            float num15 = MathHelper.Max(1f, (float)player.maxMinions);
            float num16 = (float)Projectile.identity % num15 / num15 + num14;
            Microsoft.Xna.Framework.Color fairyQueenWeaponsColor = Projectile.GetFairyQueenWeaponsColor(0f, 0f, new float?(num16 % 1f));
            Microsoft.Xna.Framework.Color fairyQueenWeaponsColor2 = Projectile.GetFairyQueenWeaponsColor(0f, 0f, new float?((num16 + 0.5f) % 1f));
            empressBladeDrawer.ColorStart = fairyQueenWeaponsColor;
            empressBladeDrawer.ColorEnd = fairyQueenWeaponsColor2;
            empressBladeDrawer.Draw(Projectile);
            DrawProj_EmpressBlade(Projectile, num16);

            return false;
        }

        private void DrawProj_EmpressBlade(Projectile proj, float hueOverride)
        {
            Main.CurrentDrawnEntityShader = -1;
            PrepareDrawnEntityDrawing(proj, Main.GetProjectileDesiredShader(proj.whoAmI));
            Vector2 vector = proj.Center - Main.screenPosition;
            proj.GetFairyQueenWeaponsColor(0f, 0f, new float?(hueOverride));
            Microsoft.Xna.Framework.Color fairyQueenWeaponsColor = proj.GetFairyQueenWeaponsColor(0.5f, 0f, new float?(hueOverride));
            Texture2D value = TextureAssets.Projectile[proj.type].Value;
            Vector2 origin = value.Frame(1, 1, 0, 0, 0, 0).Size() / 2f;
            Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White * proj.Opacity;
            color.A = (byte)((float)color.A * 0.7f);
            fairyQueenWeaponsColor.A /= 2;
            float scale = proj.scale;
            float num = proj.rotation - 1.57079637f;
            float num2 = proj.Opacity * 0.3f;
            if (num2 > 0f)
            {
                float lerpValue = Utils.GetLerpValue(60f, 50f, proj.ai[0], true);
                float scale2 = Utils.GetLerpValue(70f, 50f, proj.ai[0], true) * Utils.GetLerpValue(40f, 45f, proj.ai[0], true);
                for (float num3 = 0f; num3 < 1f; num3 += 0.166666672f)
                {
                    Vector2 value2 = num.ToRotationVector2() * -120f * num3 * lerpValue;
                    Main.EntitySpriteDraw(value, vector + value2, null, fairyQueenWeaponsColor * num2 * (1f - num3) * scale2, num, origin, scale * 1.5f, SpriteEffects.None, 0);
                }
                for (float num4 = 0f; num4 < 1f; num4 += 0.25f)
                {
                    Vector2 value3 = (num4 * 6.28318548f + num).ToRotationVector2() * 4f * scale;
                    Main.EntitySpriteDraw(value, vector + value3, null, fairyQueenWeaponsColor * num2, num, origin, scale, SpriteEffects.None, 0);
                }
            }
            Main.EntitySpriteDraw(value, vector, null, color, num, origin, scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(value, vector, null, fairyQueenWeaponsColor * num2 * 0.5f, num, origin, scale, SpriteEffects.None, 0);
        }

        public void PrepareDrawnEntityDrawing(Entity entity, int intendedShader)
        {
            Main.CurrentDrawnEntity = entity;
            if (intendedShader != 0)
            {
                if (Main.CurrentDrawnEntityShader == 0 || Main.CurrentDrawnEntityShader == -1)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                }
            }
            else if (Main.CurrentDrawnEntityShader != 0 && Main.CurrentDrawnEntityShader != -1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            Main.CurrentDrawnEntityShader = intendedShader;
        }
    }
}