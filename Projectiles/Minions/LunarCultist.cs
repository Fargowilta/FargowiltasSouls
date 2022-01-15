using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class LunarCultist : ModProjectile
    {
        Vector2 target;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lunar Cultist");
            Main.projFrames[projectile.type] = 12;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            //ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 60;
            projectile.timeLeft *= 5;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.WriteVector2(target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            target = reader.ReadVector2();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().LunarCultist)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
                projectile.damage = (int)(80f * player.minionDamage);

            if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
                    projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                projectile.localAI[0]++;
                NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc.CanBeChasedBy(projectile))
                {
                    if (projectile.ai[1] % 2 != 0) //when attacking, check for emode ml
                    {
                        NPC moonLord = FargoSoulsUtil.NPCExists(NPCs.EModeGlobalNPC.moonBoss, NPCID.MoonLordCore);
                        if (moonLord != null)
                        {
                            switch (moonLord.GetEModeNPCMod<MoonLordCore>().VulnerabilityState)
                            {
                                case 0: projectile.ai[1] = 1; break;
                                case 1: projectile.ai[1] = 3; break;
                                case 2: projectile.ai[1] = 5; break;
                                case 3: projectile.ai[1] = 7; break;
                                default: break;
                            }
                        }
                    }

                    projectile.localAI[1] = projectile.ai[1] + 1;
                    switch ((int)projectile.ai[1])
                    {
                        case 0: //chase
                            projectile.localAI[0] = 0f;
                            projectile.velocity = target - projectile.Center;
                            float length = projectile.velocity.Length();
                            if (length > 1000f) //too far, lose target
                            {
                                projectile.ai[0] = -1f;
                                projectile.ai[1] = 1f;
                                projectile.netUpdate = true;
                            }
                            else if (length > 24f)
                            {
                                projectile.velocity.Normalize();
                                projectile.velocity *= 24f;
                            }
                            else
                            {
                                projectile.ai[1]++;
                            }
                            break;

                        case 1: //shoot fireballs
                            projectile.velocity = Vector2.Zero;
                            if (projectile.localAI[0] <= 30 && projectile.localAI[0] % 10 == 0)
                            {
                                Main.PlaySound(SoundID.Item34, projectile.position);
                                Vector2 spawn = projectile.Center;
                                spawn.X -= 30 * projectile.spriteDirection;
                                spawn.Y += 12f;
                                Vector2 vel = (npc.Center - spawn).RotatedByRandom(Math.PI / 6);
                                vel.Normalize();
                                vel *= Main.rand.NextFloat(6f, 10f);
                                if (projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(spawn, vel, mod.ProjectileType("LunarCultistFireball"), projectile.damage, 9f, projectile.owner, 0f, projectile.ai[0]);
                            }
                            if (projectile.localAI[0] > 60f)
                            {
                                projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        case 2: goto case 0;
                        case 3: //lightning orb
                            projectile.velocity = Vector2.Zero;
                            if (projectile.localAI[0] == 15f)
                            {
                                Main.PlaySound(SoundID.Item121, projectile.position);
                                Vector2 spawn = projectile.Center;
                                spawn.Y -= 100;
                                if (projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(spawn, Vector2.Zero, mod.ProjectileType("LunarCultistLightningOrb"), projectile.damage, 8f, projectile.owner, projectile.whoAmI);
                            }
                            if (projectile.localAI[0] > 90f)
                            {
                                projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        case 4: goto case 0;
                        case 5: //ice mist
                            projectile.velocity = Vector2.Zero;
                            if (projectile.localAI[0] == 20f)
                            {
                                Vector2 spawn = projectile.Center;
                                spawn.X -= 30 * projectile.spriteDirection;
                                spawn.Y += 12f;
                                Vector2 vel = npc.Center - spawn;
                                vel.Normalize();
                                vel *= 4.25f;
                                if (projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(spawn, vel, mod.ProjectileType("LunarCultistIceMist"), projectile.damage, projectile.knockBack * 2f, projectile.owner);
                            }
                            if (projectile.localAI[0] > 60f)
                            {
                                projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        case 6: goto case 0;
                        case 7: //ancient visions
                            projectile.velocity = Vector2.Zero;
                            if (projectile.localAI[0] == 30f)
                            {
                                Vector2 spawn = projectile.Center;
                                spawn.Y -= projectile.height;
                                if (projectile.owner == Main.myPlayer)
                                    Projectile.NewProjectile(spawn, Vector2.UnitX * -projectile.spriteDirection * 12f, mod.ProjectileType("AncientVisionLunarCultist"), projectile.damage, projectile.knockBack * 3f, projectile.owner);
                            }
                            if (projectile.localAI[0] > 90f)
                            {
                                projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;

                        /*case 8: goto case 0;
                        case 9: //ancient light
                            projectile.velocity = Vector2.Zero;
                            if (projectile.localAI[0] == 30f)
                            {
                                Vector2 spawn = projectile.Center;
                                spawn.X -= 30 * projectile.spriteDirection;
                                spawn.Y += 12f;
                                Vector2 vel = npc.Center - spawn;
                                vel.Normalize();
                                vel *= 9f;
                                for (int i = -2; i <= 2; i++)
                                {
                                    if (projectile.owner == Main.myPlayer)
                                        Projectile.NewProjectile(spawn, vel.RotatedBy(Math.PI / 7 * i), mod.ProjectileType("LunarCultistLight"), projectile.damage, projectile.knockBack, projectile.owner, 0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f);
                                }
                            }
                            if (projectile.localAI[0] > 60f)
                            {
                                projectile.ai[1]++;
                                target = npc.Center;
                                target.Y -= npc.height + 100;
                            }
                            break;*/

                        default:
                            projectile.ai[1] = 0f;
                            goto case 0;
                    }

                    if (projectile.velocity.X == 0)
                    {
                        float distance = npc.Center.X - projectile.Center.X;
                        if (distance != 0)
                            projectile.spriteDirection = distance < 0 ? 1 : -1;
                    }
                    else
                    {
                        projectile.spriteDirection = (projectile.velocity.X < 0) ? 1 : -1;
                    }
                }
                else //forget target
                {
                    TargetEnemies();
                }
            }
            else //no target
            {
                if (projectile.ai[1] == 0f) //follow player
                {
                    if (target == Vector2.Zero)
                    {
                        target = Main.player[projectile.owner].Center;
                        target.Y -= 100f;
                    }

                    projectile.velocity = target - projectile.Center;
                    float length = projectile.velocity.Length();
                    if (length > 1500f) //teleport when too far away
                    {
                        projectile.Center = Main.player[projectile.owner].Center;
                        projectile.velocity = Vector2.Zero;
                        projectile.ai[1] = 1f;
                    }
                    else if (length > 24f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 24f;
                    }
                    else //in close enough range to stop
                    {
                        projectile.ai[1] = 1f;
                    }
                }
                else //now above player, wait
                {
                    projectile.velocity = Vector2.Zero;

                    projectile.localAI[0]++;
                    if (projectile.localAI[0] > 30)
                    {
                        TargetEnemies();
                        projectile.localAI[0] = 0f;
                    }
                }

                if (projectile.velocity.X == 0)
                {
                    float distance = Main.player[projectile.owner].Center.X - projectile.Center.X;
                    if (distance != 0)
                        projectile.spriteDirection = distance < 0 ? 1 : -1;
                }
                else
                {
                    projectile.spriteDirection = (projectile.velocity.X < 0) ? 1 : -1;
                }
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 3;
                if (projectile.ai[0] > -1f && projectile.ai[0] < 200f)
                {
                    switch ((int)projectile.ai[1])
                    {
                        case 1: projectile.frame += 6; break;
                        case 3: projectile.frame += 3; break;
                        case 5: projectile.frame += 6; break;
                        case 7: projectile.frame += 3; break;
                        case 9: projectile.frame += 6; break;
                        default: break;
                    }
                }
            }
        }

        private void TargetEnemies()
        {
            projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000f, true, Main.player[projectile.owner].Center);
            if (projectile.ai[0] != -1)
            {
                target = Main.npc[(int)projectile.ai[0]].Center;
                target.Y -= Main.npc[(int)projectile.ai[0]].height + 100;
                projectile.ai[1] = projectile.localAI[1];
                if (projectile.ai[1] % 2 != 0)
                    projectile.ai[1]--;
            }
            else
            {
                target = Main.player[projectile.owner].Center;
                target.Y -= Main.player[projectile.owner].height + 100;
                projectile.ai[1] = 0f;
            }
            projectile.netUpdate = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
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

            Texture2D texture2D14 = mod.GetTexture("Projectiles/Minions/LunarCultistTrail");
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 3)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D14, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}