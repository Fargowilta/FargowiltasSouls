﻿using Fargowiltas.Items.Summons;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class Retinazer : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Retinazer);
        
        public int DeathrayState;
        public int AuraRadiusCounter;
        
        public bool StoredDirectionToPlayer;

        public bool ForcedPhase2OnSpawn;
        public bool DroppedSummon;

        public bool HasSaidEndure;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(DeathrayState), IntStrategies.CompoundStrategy },
                { new Ref<object>(AuraRadiusCounter), IntStrategies.CompoundStrategy },
                
                { new Ref<object>(StoredDirectionToPlayer), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)(npc.lifeMax * 1.2);
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.retiBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;
            
            NPC spazmatism = FargoSoulsUtil.NPCExists(EModeGlobalNPC.spazBoss, NPCID.Spazmatism);

            if (!ForcedPhase2OnSpawn) //start phase 2
            {
                ForcedPhase2OnSpawn = true;
                npc.ai[0] = 1f;
                npc.ai[1] = 0.0f;
                npc.ai[2] = 0.0f;
                npc.ai[3] = 0.0f;
                npc.netUpdate = true;
            }

            if (npc.life <= npc.lifeMax / 2 || npc.dontTakeDamage)
            {
                npc.dontTakeDamage = npc.life == 1 || !npc.HasValidTarget;
                if (npc.life != 1 && npc.HasValidTarget)
                    npc.dontTakeDamage = false;
                //become vulnerable again when both twins at low life
                if (npc.dontTakeDamage && npc.HasValidTarget && (spazmatism == null || spazmatism.life == 1))
                    npc.dontTakeDamage = false;
            }

            if (Main.dayTime)
            {
                if (npc.velocity.Y > 0)
                    npc.velocity.Y = 0;

                npc.velocity.Y -= 0.5f;
                npc.dontTakeDamage = true;

                if (spazmatism != null)
                {
                    if (npc.timeLeft < 60)
                        npc.timeLeft = 60;

                    if (spazmatism.timeLeft < 60)
                        spazmatism.timeLeft = 60;

                    npc.TargetClosest(false);
                    spazmatism.TargetClosest(false);
                    if (npc.Distance(Main.player[npc.target].Center) > 2000 && spazmatism.Distance(Main.player[spazmatism.target].Center) > 2000)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            spazmatism.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, EModeGlobalNPC.spazBoss);
                        }
                    }
                }

                return true;
            }

            if (npc.ai[0] < 4f) //going to phase 3
            {
                if (npc.life <= npc.lifeMax / 2)
                {
                    //npc.ai[0] = 4f;
                    npc.ai[0] = 604f; //initiate spin immediately
                    npc.netUpdate = true;
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 11, npc.whoAmI);
                }
            }
            else //in phase 3
            {

                /*if (npc.life == 1 && --Counter1 < 0) //when brought to 1hp, begin shooting dark stars
                {
                    Counter1 = 240;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.Normalize();
                        distance *= 10f;
                        for (int i = 0; i < 12; i++)
                            Projectile.NewProjectile(npc.Center, distance.RotatedBy(2 * Math.PI / 12 * i),
                                ModContent.ProjectileType<DarkStar>(), npc.damage / 5, 0f, Main.myPlayer);
                    }
                }*/

                //dust code
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 90, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }

                if (npc.localAI[1] >= (npc.ai[1] == 0 ? 175 : 55)) //hijacking vanilla laser code
                {
                    npc.localAI[1] = 0;
                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center);
                    int proj = Projectile.NewProjectile(npc.Center + (npc.width - 24) * vel, vel, ModContent.ProjectileType<DarkStarHoming>(), npc.damage / 4, 0f, Main.myPlayer, -1, 1f);
                    if (proj != Main.maxProjectiles)
                        Main.projectile[proj].timeLeft = 120;
                }

                if (DeathrayState == 0 || DeathrayState == 3) //not doing deathray, grow arena
                {
                    AuraRadiusCounter--;
                    if (AuraRadiusCounter < 0)
                        AuraRadiusCounter = 0;
                }
                else //doing deathray, shrink arena
                {
                    AuraRadiusCounter++;
                    if (AuraRadiusCounter > 180)
                        AuraRadiusCounter = 180;
                }

                float auraDistance = 2000 - 1200 * AuraRadiusCounter / 180f;
                if (FargoSoulsWorld.MasochistModeReal)
                    auraDistance *= 0.75f;
                EModeGlobalNPC.Aura(npc, auraDistance, true, DustID.Fire, default, ModContent.BuffType<Oiled>(), BuffID.OnFire, BuffID.Burning);

                Player p = Main.LocalPlayer;

                //2*pi * (# of full circles) / (seconds to finish rotation) / (ticks per sec)
                float rotationInterval = 2f * (float)Math.PI * 1.2f / 4f / 60f;
                if (FargoSoulsWorld.MasochistModeReal)
                    rotationInterval *= 1.1f;

                npc.ai[0]++; //base value is 4
                switch (DeathrayState) //laser code idfk
                {
                    case 0:
                        if (!npc.HasValidTarget)
                        {
                            npc.ai[0]--; //stop counting up if player is dead
                            if (spazmatism == null) //despawn REALLY fast
                                npc.velocity.Y -= 0.5f;
                        }
                        if (npc.ai[0] > 604f)
                        {
                            npc.ai[0] = 4f;
                            if (npc.HasPlayerTarget)
                            {
                                npc.rotation = npc.Center.X < Main.player[npc.target].Center.X ? 0 : (float)Math.PI;
                                npc.rotation -= (float)Math.PI / 2;

                                DeathrayState++;
                                npc.ai[3] = -npc.rotation;
                                if (--npc.ai[2] > 295f)
                                    npc.ai[2] = 295f;
                                StoredDirectionToPlayer = (Main.player[npc.target].Center.X - npc.Center.X < 0);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                                Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f); //eoc roar
                            }
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                        break;

                    case 1: //slowing down, beginning rotation
                        npc.velocity *= 1f - (npc.ai[0] - 4f) / 120f;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= (npc.ai[0] - 4f) / 120f * rotationInterval * (StoredDirectionToPlayer ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] == 5f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 9f, npc.whoAmI);
                            }
                        }

                        if (npc.ai[0] >= 125f) //FIRE LASER
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = Vector2.UnitX.RotatedBy(npc.rotation);
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PhantasmalDeathray>(), npc.damage / 3, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                            DeathrayState++;
                            npc.ai[0] = 4f;

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetSync(npc);
                        }
                        return false;

                    case 2: //spinning full speed
                        npc.velocity = Vector2.Zero;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= rotationInterval * (StoredDirectionToPlayer ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] >= 244f)
                        {
                            DeathrayState++;
                            npc.ai[0] = 4f;

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetSync(npc);
                        }
                        else if (!npc.HasValidTarget) //end spin immediately if player dead
                        {
                            npc.TargetClosest(false);
                            if (!npc.HasValidTarget)
                                npc.ai[0] = 244f;
                        }
                        return false;

                    case 3: //laser done, slowing down spin, moving again
                        npc.velocity *= (npc.ai[0] - 4f) / 60f;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= (1f - (npc.ai[0] - 4f) / 60f) * rotationInterval * (StoredDirectionToPlayer ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] >= 64f)
                        {
                            DeathrayState = 0;
                            npc.ai[0] = 4f;

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetSync(npc);
                        }
                        return false;

                    default:
                        DeathrayState = 0;
                        npc.ai[0] = 4f;
                        npc.netUpdate = true;
                        NetSync(npc);
                        break;
                }

                //npc.position += npc.velocity / 4f;

                //if (Counter == 600 && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                //{
                //    Vector2 vector200 = Main.player[npc.target].Center - npc.Center;
                //    vector200.Normalize();
                //    float num1225 = -1f;
                //    if (vector200.X < 0f)
                //    {
                //        num1225 = 1f;
                //    }
                //    vector200 = vector200.RotatedBy(-num1225 * 1.04719755f, default(Vector2));
                //    Projectile.NewProjectile(npc.Center, vector200, ModContent.ProjectileType<PhantasmalDeathray>(), npc.damage / 2, 0f, Main.myPlayer, num1225 * 0.0104719755f, npc.whoAmI);
                //    npc.netUpdate = true;
                //}
            }

            /*if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.spazBoss, NPCID.Spazmatism) && targetAlive)
            {
                Timer--;

                if (Timer <= 0)
                {
                    Timer = 600;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spawn = NPC.NewNPC((int)npc.position.X + Main.rand.Next(-1000, 1000), (int)npc.position.Y + Main.rand.Next(-400, -100), NPCID.Spazmatism);
                        if (spawn != 200)
                        {
                            Main.npc[spawn].life = Main.npc[spawn].lifeMax / 4;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Spazmatism has been revived!"), new Color(175, 75, 255));
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                            }
                            else
                            {
                                Main.NewText("Spazmatism has been revived!", 175, 75, 255);
                            }
                        }
                    }
                }
            }*/

            EModeUtils.DropSummon(npc, ModContent.ItemType<MechEye>(), NPC.downedMechBoss2, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            return npc.ai[0] < 4 ? base.GetAlpha(npc, drawColor) : new Color(255, drawColor.G / 2, drawColor.B / 2);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.spazBoss, NPCID.Spazmatism))
            {
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<FusedLens>());
                npc.DropItemInstanced(npc.position, npc.Size, ItemID.IronCrate, 5);
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive)
                return base.CheckDead(npc);

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.spazBoss, NPCID.Spazmatism) && Main.npc[EModeGlobalNPC.spazBoss].life > 1) //spaz still active
            {
                npc.life = 1;
                npc.active = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.netUpdate = true;

                if (!HasSaidEndure)
                {
                    HasSaidEndure = true;
                    FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.RetinazerEndure"), new Color(175, 75, 255));
                }
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 15);
            LoadBossHeadSprite(recolor, 20);
            LoadGoreRange(recolor, 143, 146);
        }
    }

    public class Spazmatism : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Spazmatism);

        public int CursedFlameTimer;
        public int FlameWheelSpreadTimer;
        public int FlameWheelCount;

        public bool ForcedPhase2OnSpawn;
        public bool HasSaidEndure;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(CursedFlameTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(FlameWheelSpreadTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(FlameWheelCount), IntStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.spazBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;
            
            NPC retinazer = FargoSoulsUtil.NPCExists(EModeGlobalNPC.retiBoss, NPCID.Retinazer);

            float modifier = (float)npc.life / npc.lifeMax;
            if (FargoSoulsWorld.MasochistModeReal)
                modifier /= 2;

            if (!ForcedPhase2OnSpawn) //spawn in phase 2
            {
                ForcedPhase2OnSpawn = true;
                npc.ai[0] = 1f;
                npc.ai[1] = 0.0f;
                npc.ai[2] = 0.0f;
                npc.ai[3] = 0.0f;
                npc.netUpdate = true;
            }

            if (npc.life <= npc.lifeMax / 2 || npc.dontTakeDamage)
            {
                npc.dontTakeDamage = npc.life == 1 || !npc.HasValidTarget;
                if (npc.life != 1 && npc.HasValidTarget)
                    npc.dontTakeDamage = false;
                //become vulnerable again when both twins low
                if (npc.dontTakeDamage && npc.HasValidTarget && (retinazer == null || retinazer.life == 1))
                    npc.dontTakeDamage = false;
            }

            if (Main.dayTime)
            {
                if (npc.velocity.Y > 0)
                    npc.velocity.Y = 0;

                npc.velocity.Y -= 0.5f;
                npc.dontTakeDamage = true;

                if (retinazer != null)
                {
                    if (npc.timeLeft < 60)
                        npc.timeLeft = 60;

                    if (retinazer.timeLeft < 60)
                        retinazer.timeLeft = 60;

                    npc.TargetClosest(false);
                    retinazer.TargetClosest(false);
                    if (npc.Distance(Main.player[npc.target].Center) > 2000 && retinazer.Distance(Main.player[retinazer.target].Center) > 2000)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            retinazer.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, EModeGlobalNPC.retiBoss);
                        }
                    }
                }

                return true;
            }

            if (npc.ai[0] < 4f)
            {
                if (npc.life <= npc.lifeMax / 2) //going to phase 3
                {
                    npc.ai[0] = 4f;
                    npc.netUpdate = true;
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);

                    int index = npc.FindBuffIndex(BuffID.CursedInferno);
                    if (index != -1)
                        npc.DelBuff(index); //remove cursed inferno debuff if i have it

                    npc.buffImmune[BuffID.CursedInferno] = true;
                    npc.buffImmune[BuffID.OnFire] = true;
                    npc.buffImmune[BuffID.ShadowFlame] = true;
                    npc.buffImmune[BuffID.Frostburn] = true;
                }
            }
            else //in phase 3
            {
                //npc.position += npc.velocity / 10f;

                //dust code
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 89, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }

                if (npc.ai[1] == 0f) //not dashing
                {
                    if (retinazer != null && (retinazer.ai[0] < 4f || retinazer.GetEModeNPCMod<Retinazer>().DeathrayState == 0
                        || retinazer.GetEModeNPCMod<Retinazer>().DeathrayState == 3)) //reti is in normal AI
                    {
                        npc.ai[1] = 1; //switch to dashing
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;

                        npc.netUpdate = true;
                        NetSync(npc);
                        return false;
                    }

                    if (npc.HasValidTarget && retinazer != null)
                    {
                        Vector2 target = retinazer.Center + retinazer.DirectionTo(npc.Center) * 100;
                        npc.velocity = (target - npc.Center) / 60;

                        float rotationInterval = 2f * (float)Math.PI * 1.2f / 4f / 60f * 0.65f;
                        if (FargoSoulsWorld.MasochistModeReal)
                            rotationInterval *= 1.1f;
                        npc.rotation += rotationInterval * (retinazer.GetEModeNPCMod<Retinazer>().StoredDirectionToPlayer ? 1f : -1f);

                        if (FlameWheelSpreadTimer < 0)
                            FlameWheelSpreadTimer = 0;

                        if (FlameWheelCount == 0) //i can't be bothered to figure out the formula for this rn
                        {
                            FlameWheelCount = 2;
                            if (modifier < 0.5f / 4 * 3)
                                FlameWheelCount = 3;
                            if (modifier < 0.5f / 4 * 2)
                                FlameWheelCount = 4;
                            if (modifier < 0.5f / 4 * 1)
                                FlameWheelCount = 4;

                            if (FargoSoulsWorld.MasochistModeReal)
                                FlameWheelCount++;
                        }
                        
                        if (++FlameWheelSpreadTimer < 30) //snap to reti, don't do contact damage
                        {
                            npc.rotation = npc.DirectionTo(retinazer.Center).ToRotation() - (float)Math.PI / 2;
                        }
                        else if (++CursedFlameTimer > 3) //rings of fire
                        {
                            CursedFlameTimer = 0;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = 24f * Math.Min((FlameWheelSpreadTimer - 30) / 120f, 1f); //fan out gradually
                                int timeLeft = (int)(speed / 24f * 45f);
                                float baseRotation = npc.rotation + (float)Math.PI / 2;
                                if (timeLeft > 5)
                                {
                                    for (int i = 0; i < FlameWheelCount; i++)
                                    {
                                        int p = Projectile.NewProjectile(npc.Center, speed * (baseRotation + MathHelper.TwoPi / FlameWheelCount * i).ToRotationVector2(),
                                            ModContent.ProjectileType<DarkStar>(), npc.damage / 4, 0f, Main.myPlayer);
                                        if (p != Main.maxProjectiles)
                                            Main.projectile[p].timeLeft = timeLeft;
                                    }
                                }
                            }
                        }

                        /*if (++Counter0 > 40)
                        {
                            Counter0 = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget) //vanilla spaz p1 shoot fireball code
                            {
                                Vector2 Speed = Main.player[npc.target].Center - npc.Center;
                                Speed.Normalize();
                                int Damage;
                                if (Main.expertMode)
                                {
                                    Speed *= 14f;
                                    Damage = 22;
                                }
                                else
                                {
                                    Speed *= 12f;
                                    Damage = 25;
                                }
                                Projectile.NewProjectile(npc.Center + Speed * 4f, Speed, ProjectileID.CursedFlameHostile, Damage, 0f, Main.myPlayer);
                            }
                        }*/

                        return false;
                    }
                }
                else //dashing
                {
                    if (retinazer != null && retinazer.ai[0] >= 4f && retinazer.GetEModeNPCMod<Retinazer>().DeathrayState != 0 && retinazer.GetEModeNPCMod<Retinazer>().DeathrayState != 3) //reti is doing the spin
                    {
                        npc.ai[1] = 0; //switch to not dashing

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                        return false;
                    }

                    FlameWheelCount = 0;

                    if (FlameWheelSpreadTimer > 75) //cooldown before attacking again
                        FlameWheelSpreadTimer = 75;
                    if (FlameWheelSpreadTimer > 0)
                    {
                        FlameWheelSpreadTimer--;
                        if (npc.HasValidTarget)
                        {
                            const float PI = (float)Math.PI;
                            if (npc.rotation > PI)
                                npc.rotation -= 2 * PI;
                            if (npc.rotation < -PI)
                                npc.rotation += 2 * PI;

                            float targetRotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                            if (targetRotation > PI)
                                targetRotation -= 2 * PI;
                            if (targetRotation < -PI)
                                targetRotation += 2 * PI;
                            npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);
                        }
                        return false;
                    }

                    if (npc.ai[2] > 50)
                    {
                        npc.ai[2] -= modifier;
                    }
                    else
                    {
                        if (npc.HasValidTarget && ++CursedFlameTimer > 3) //cursed flamethrower when dashing
                        {
                            CursedFlameTimer = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = (1f - modifier) * 0.8f;
                                float rotationVariance = 9f * modifier * 2;
                                Projectile.NewProjectile(npc.Center, speed * npc.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-rotationVariance, rotationVariance))), ProjectileID.EyeFire, npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                /*if (npc.life == 1 && --Counter1 < 0) //when brought to 1hp, begin shooting dark stars
                {
                    Counter1 = 120;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.Normalize();
                        distance *= 14f;
                        for (int i = 0; i < 8; i++)
                            Projectile.NewProjectile(npc.Center, distance.RotatedBy(2 * Math.PI / 8 * i),
                                ModContent.ProjectileType<DarkStar>(), npc.damage / 5, 0f, Main.myPlayer);
                    }
                }*/
            }

            /*if (!retiAlive && npc.HasPlayerTarget && Main.player[npc.target].active)
            {
                Timer--;

                if (Timer <= 0)
                {
                    Timer = 600;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spawn = NPC.NewNPC((int)npc.position.X + Main.rand.Next(-1000, 1000), (int)npc.position.Y + Main.rand.Next(-400, -100), NPCID.Retinazer);
                        if (spawn != 200)
                        {
                            Main.npc[spawn].life = Main.npc[spawn].lifeMax / 4;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Retinazer has been revived!"), new Color(175, 75, 255));
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                            }
                            else
                            {
                                Main.NewText("Retinazer has been revived!", 175, 75, 255);
                            }
                        }
                    }
                }
            }*/

            return true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return !(npc.ai[1] == 0f && FlameWheelSpreadTimer < 30);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (npc.ai[0] >= 4f)
                target.AddBuff(BuffID.CursedInferno, 300);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            return npc.ai[0] < 4 ? base.GetAlpha(npc, drawColor) : new Color(drawColor.R / 2, 255, drawColor.B / 2);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer))
            {
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<FusedLens>());
                npc.DropItemInstanced(npc.position, npc.Size, ItemID.IronCrate, 5);
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive)
                return base.CheckDead(npc);

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer) && Main.npc[EModeGlobalNPC.retiBoss].life > 1) //reti still active
            {
                npc.life = 1;
                npc.active = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.netUpdate = true;

                if (!HasSaidEndure)
                {
                    HasSaidEndure = true;
                    FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.SpazmatismEndure"), new Color(175, 75, 255));
                }
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 16);
            LoadBossHeadSprite(recolor, 21);
            LoadGoreRange(recolor, 143, 146);
        }
    }
}
