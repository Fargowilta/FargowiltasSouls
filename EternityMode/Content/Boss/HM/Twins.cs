using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
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
        public int DarkStarTimer;

        public bool StoredDirectionToPlayer;

        public bool ForcedPhase2OnSpawn;
        public bool DroppedSummon;

        public bool HasSaidEndure;
        public bool Resist;
        public int RespawnTimer;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(DeathrayState);
            binaryWriter.Write7BitEncodedInt(AuraRadiusCounter);
            binaryWriter.Write7BitEncodedInt(DarkStarTimer);
            bitWriter.WriteBit(StoredDirectionToPlayer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            DeathrayState = binaryReader.Read7BitEncodedInt();
            AuraRadiusCounter = binaryReader.Read7BitEncodedInt();
            DarkStarTimer = binaryReader.Read7BitEncodedInt();
            StoredDirectionToPlayer = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)(npc.lifeMax * 1.2);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.retiBoss = npc.whoAmI;

            Resist = false;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            //have some dr during phase transition animation
            if (npc.ai[0] == 1 || npc.ai[0] == 2)
                Resist = true;

            NPC spazmatism = FargoSoulsUtil.NPCExists(EModeGlobalNPC.spazBoss, NPCID.Spazmatism);

            if (FargoSoulsWorld.MasochistModeReal && spazmatism == null && npc.HasValidTarget && ++RespawnTimer > 600)
            {
                RespawnTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), Main.rand.NextFloat(-800, -600)), NPCID.Spazmatism, target: npc.target);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].life = Main.npc[n].lifeMax / 4;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, number: n);
                        string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.TwinsRevive");
                        if (FargoSoulsUtil.IsChinese())
                        {
                            FargoSoulsUtil.PrintText($"{Main.npc[n].FullName}{text}", new Color(175, 75, 255));
                        }
                        else
                        {
                            FargoSoulsUtil.PrintText($"{Main.npc[n].FullName} {text}", new Color(175, 75, 255));
                        }
                    }
                }
            }

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
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 11, npc.whoAmI);
                }
            }
            else //in phase 3
            {

                if (FargoSoulsWorld.MasochistModeReal && spazmatism == null && --DarkStarTimer < 0) //when twin dead, begin shooting dark stars
                {
                    DarkStarTimer = 240;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.Normalize();
                        distance *= 10f;
                        for (int i = 0; i < 12; i++)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance.RotatedBy(2 * Math.PI / 12 * i),
                                ModContent.ProjectileType<DarkStar>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                    }
                }

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
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + (npc.width - 24) * vel, vel, ModContent.ProjectileType<DarkStarTwins>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.target);
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
                if (auraDistance < 2000 - 1)
                    EModeGlobalNPC.Aura(npc, auraDistance, true, DustID.Torch, default, ModContent.BuffType<Oiled>(), BuffID.OnFire, BuffID.Burning);

                //2*pi * (# of full circles) / (seconds to finish rotation) / (ticks per sec)
                float rotationInterval = 2f * (float)Math.PI * 1.2f / 4f / 60f;
                if (FargoSoulsWorld.MasochistModeReal)
                    rotationInterval *= 1.05f;

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
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center); //eoc roar
                            }

                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetSync(npc);
                        }
                        break;

                    case 1: //slowing down, beginning rotation
                        npc.velocity *= 1f - (npc.ai[0] - 4f) / 120f;
                        npc.localAI[1] = 0f;
                        //if (--npc.ai[2] > 295f) npc.ai[2] = 295f;
                        npc.ai[3] -= (npc.ai[0] - 4f) / 120f * rotationInterval * (StoredDirectionToPlayer ? 1f : -1f);
                        npc.rotation = -npc.ai[3];

                        if (npc.ai[0] == 35f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 9f, npc.whoAmI);
                            }
                        }

                        if (npc.ai[0] >= 155f) //FIRE LASER
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 speed = Vector2.UnitX.RotatedBy(npc.rotation);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<RetinazerDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 3), 0f, Main.myPlayer, 0f, npc.whoAmI);
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
                //    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vector200, ModContent.ProjectileType<PhantasmalDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 2), 0f, Main.myPlayer, num1225 * 0.0104719755f, npc.whoAmI);
                //    npc.netUpdate = true;
                //}
            }

            if (DeathrayState > 0)
                Resist = true;

            EModeUtils.DropSummon(npc, "MechEye", NPC.downedMechBoss2, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Resist)
                damage *= 0.5;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            return npc.ai[0] < 4 ? base.GetAlpha(npc, drawColor) : new Color(255, drawColor.G / 2, drawColor.B / 2);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            LeadingConditionRule noTwin = new LeadingConditionRule(new Conditions.MissingTwin());
            emodeRule.OnSuccess(noTwin);
            noTwin.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<FusedLens>()));
            noTwin.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive || FargoSoulsWorld.MasochistModeReal)
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
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.TwinsEndure");
                    if (FargoSoulsUtil.IsChinese())
                    {
                        FargoSoulsUtil.PrintText($"{npc.FullName}{text}", new Color(175, 75, 255));
                    }
                    else
                    {
                        FargoSoulsUtil.PrintText($"{npc.FullName} {text}", new Color(175, 75, 255));
                    }
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

            LoadSpecial(recolor, ref TextureAssets.Chain12, ref FargowiltasSouls.TextureBuffer.Chain12, "Chain12");
        }
    }

    public class Spazmatism : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Spazmatism);

        public int ProjectileTimer;
        public int FlameWheelSpreadTimer;
        public int FlameWheelCount;
        public int DarkStarTimer;
        public int P3DashPhaseDelay;

        public bool ForcedPhase2OnSpawn;
        public bool HasSaidEndure;
        public bool Resist;
        public int RespawnTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(ProjectileTimer);
            binaryWriter.Write7BitEncodedInt(FlameWheelSpreadTimer);
            binaryWriter.Write7BitEncodedInt(FlameWheelCount);
            binaryWriter.Write7BitEncodedInt(DarkStarTimer);
            binaryWriter.Write7BitEncodedInt(P3DashPhaseDelay);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            ProjectileTimer = binaryReader.Read7BitEncodedInt();
            FlameWheelSpreadTimer = binaryReader.Read7BitEncodedInt();
            FlameWheelCount = binaryReader.Read7BitEncodedInt();
            DarkStarTimer = binaryReader.Read7BitEncodedInt();
            P3DashPhaseDelay = binaryReader.Read7BitEncodedInt();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.spazBoss = npc.whoAmI;

            Resist = false;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            //have some dr during phase transition animation
            if (npc.ai[0] == 1 || npc.ai[0] == 2)
                Resist = true;

            NPC retinazer = FargoSoulsUtil.NPCExists(EModeGlobalNPC.retiBoss, NPCID.Retinazer);

            if (FargoSoulsWorld.MasochistModeReal && retinazer == null && npc.HasValidTarget && ++RespawnTimer > 600)
            {
                RespawnTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), Main.rand.NextFloat(-800, -600)), NPCID.Retinazer, target: npc.target);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].life = Main.npc[n].lifeMax / 4;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, number: n);
                        string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.TwinsRevive");
                        if (FargoSoulsUtil.IsChinese())
                        {
                            FargoSoulsUtil.PrintText($"{Main.npc[n].FullName}{text}", new Color(175, 75, 255));
                        }
                        else
                        {
                            FargoSoulsUtil.PrintText($"{Main.npc[n].FullName} {text}", new Color(175, 75, 255));
                        }
                    }
                }
            }

            float modifier = (float)npc.life / npc.lifeMax;
            if (FargoSoulsWorld.MasochistModeReal)
                modifier *= modifier;

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

            const int P3DashDelayLength = 75;

            if (npc.ai[0] < 4f)
            {
                if (npc.life <= npc.lifeMax / 2) //going to phase 3
                {
                    npc.ai[0] = 4f;
                    npc.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                    if (!FargoSoulsWorld.MasochistModeReal)
                        P3DashPhaseDelay = P3DashDelayLength;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, NPCID.MoonLordCore);

                    int index = npc.FindBuffIndex(BuffID.CursedInferno);
                    if (index != -1)
                        npc.DelBuff(index); //remove cursed inferno debuff if i have it

                    npc.buffImmune[BuffID.CursedInferno] = true;
                    npc.buffImmune[BuffID.OnFire] = true;
                    npc.buffImmune[BuffID.OnFire3] = true;
                    npc.buffImmune[BuffID.ShadowFlame] = true;
                    npc.buffImmune[BuffID.Frostburn] = true;
                    npc.buffImmune[BuffID.Frostburn2] = true;
                }

                //reti is doing the spin
                if (retinazer != null && retinazer.ai[0] >= 4f && retinazer.GetGlobalNPC<Retinazer>().DeathrayState != 0 && retinazer.GetGlobalNPC<Retinazer>().DeathrayState != 3)
                {
                    if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        npc.velocity *= 0.98f;
                        if (!TryWatchHarmlessly(npc))
                            return false;
                    }
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
                    Resist = true;

                    if (retinazer != null && (retinazer.ai[0] < 4f || retinazer.GetGlobalNPC<Retinazer>().DeathrayState == 0
                        || retinazer.GetGlobalNPC<Retinazer>().DeathrayState == 3)) //reti is in normal AI
                    {
                        npc.ai[1] = 1; //switch to dashing
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                        return false;
                    }

                    if (npc.HasValidTarget && retinazer != null)
                    {
                        Vector2 target = retinazer.Center + retinazer.DirectionTo(npc.Center) * 100;
                        npc.velocity = (target - npc.Center) / 60;

                        float rotationInterval = 2f * (float)Math.PI * 1.2f / 4f / 60f * 0.65f;
                        if (FargoSoulsWorld.MasochistModeReal)
                            rotationInterval *= -1f;
                        npc.rotation += rotationInterval * (retinazer.GetGlobalNPC<Retinazer>().StoredDirectionToPlayer ? 1f : -1f);

                        if (FlameWheelSpreadTimer < 0)
                            FlameWheelSpreadTimer = 0;

                        if (FlameWheelCount == 0) //i can't be bothered to figure out the formula for this rn
                        {
                            FlameWheelCount = 2;
                            if (modifier < 0.5f / 4 * 3)
                                FlameWheelCount = 3;
                            if (modifier < 0.5f / 4 * 2)
                                FlameWheelCount = 4;
                            if (modifier < 0.5f / 4 * 1 || FargoSoulsWorld.MasochistModeReal)
                                FlameWheelCount = 5;
                        }

                        if (++FlameWheelSpreadTimer < 30) //snap to reti, don't do contact damage
                        {
                            npc.rotation = npc.DirectionTo(retinazer.Center).ToRotation() - (float)Math.PI / 2;
                        }
                        else if (++ProjectileTimer > 15) //rings of stars
                        {
                            ProjectileTimer = 0;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = 12f * Math.Min((FlameWheelSpreadTimer - 30) / 120f, 1f); //fan out gradually
                                int timeLeft = (int)(speed / 12f * 90f);
                                float baseRotation = npc.rotation + (float)Math.PI / 2;
                                if (timeLeft > 5)
                                {
                                    for (int i = 0; i < FlameWheelCount; i++)
                                    {
                                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed * (baseRotation + MathHelper.TwoPi / FlameWheelCount * i).ToRotationVector2(),
                                            ModContent.ProjectileType<DarkStar>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
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
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + Speed * 4f, Speed, ProjectileID.CursedFlameHostile, Damage, 0f, Main.myPlayer);
                            }
                        }*/

                        return false;
                    }
                }
                else //dashing
                {
                    if (retinazer != null && retinazer.ai[0] >= 4f && retinazer.GetGlobalNPC<Retinazer>().DeathrayState != 0 && retinazer.GetGlobalNPC<Retinazer>().DeathrayState != 3) //reti is doing the spin
                    {
                        npc.ai[1] = 0; //switch to not dashing

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                        return false;
                    }

                    FlameWheelCount = 0;

                    if (FlameWheelSpreadTimer > 0) //cooldown before attacking again
                    {
                        P3DashPhaseDelay = Math.Min(FlameWheelSpreadTimer, 75);
                        FlameWheelSpreadTimer = 0;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                    }

                    if (P3DashPhaseDelay > 0)
                    {
                        P3DashPhaseDelay--;
                        if (!TryWatchHarmlessly(npc))
                            return false;
                    }

                    if (npc.ai[2] > 50)
                    {
                        npc.ai[2] -= modifier;
                    }
                    else
                    {
                        if (npc.HasValidTarget && ++ProjectileTimer > 3) //cursed flamethrower when dashing
                        {
                            ProjectileTimer = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = (1f - modifier) * 0.8f;
                                float rotationVariance = 9f * modifier * 2;
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed * npc.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-rotationVariance, rotationVariance))), ProjectileID.EyeFire, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                if (FargoSoulsWorld.MasochistModeReal && retinazer == null && --DarkStarTimer < 0) //when twin dead, begin shooting dark stars
                {
                    DarkStarTimer = 150;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;
                        distance.Normalize();
                        distance *= 14f;
                        for (int i = 0; i < 8; i++)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance.RotatedBy(2 * Math.PI / 8 * i),
                                ModContent.ProjectileType<DarkStar>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                    }
                }
            }

            return true;
        }

        private bool TryWatchHarmlessly(NPC npc)
        {
            Resist = true;

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

                return false;
            }
            return true;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Resist)
                damage *= 0.5;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
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

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            LeadingConditionRule noTwin = new LeadingConditionRule(new Conditions.MissingTwin());
            emodeRule.OnSuccess(noTwin);
            noTwin.OnSuccess(ItemDropRule.BossBag(ModContent.ItemType<FusedLens>()));
            noTwin.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive || FargoSoulsWorld.MasochistModeReal)
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
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.TwinsEndure");
                    if (FargoSoulsUtil.IsChinese())
                    {
                        FargoSoulsUtil.PrintText($"{npc.FullName}{text}", new Color(175, 75, 255));
                    }
                    else
                    {
                        FargoSoulsUtil.PrintText($"{npc.FullName} {text}", new Color(175, 75, 255));
                    }
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

            LoadSpecial(recolor, ref TextureAssets.Chain12, ref FargowiltasSouls.TextureBuffer.Chain12, "Chain12");
        }
    }
}
