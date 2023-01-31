using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Champions;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class QueenBee : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenBee);

        public int HiveThrowTimer;
        public int StingerRingTimer;
        public int BeeSwarmTimer = 600;
        public int ForgorDeathrayTimer;
        public int EnrageFactor;

        public bool SpawnedRoyalSubjectWave1;
        public bool SpawnedRoyalSubjectWave2;
        public bool InPhase2;

        public bool DroppedSummon;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(HiveThrowTimer);
            binaryWriter.Write7BitEncodedInt(StingerRingTimer);
            binaryWriter.Write7BitEncodedInt(BeeSwarmTimer);
            bitWriter.WriteBit(SpawnedRoyalSubjectWave1);
            bitWriter.WriteBit(SpawnedRoyalSubjectWave2);
            bitWriter.WriteBit(InPhase2);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            HiveThrowTimer = binaryReader.Read7BitEncodedInt();
            StingerRingTimer = binaryReader.Read7BitEncodedInt();
            BeeSwarmTimer = binaryReader.Read7BitEncodedInt();
            SpawnedRoyalSubjectWave1 = bitReader.ReadBit();
            SpawnedRoyalSubjectWave2 = bitReader.ReadBit();
            InPhase2 = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.4005);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            EModeGlobalNPC.beeBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;


            if (npc.HasPlayerTarget && npc.HasValidTarget && (!Main.player[npc.target].ZoneJungle
                || Main.player[npc.target].position.Y < Main.worldSurface * 16))
            {
                if (++EnrageFactor == 300)
                {
                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.QueenBeeEnrage", new Color(175, 75, 255));
                }

                if (EnrageFactor > 300)
                {
                    float rotation = Main.rand.NextFloat(0.03f, 0.18f);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + new Vector2(3 * npc.direction, 15), Main.rand.NextFloat(8f, 24f) * Main.rand.NextVector2Unit(),
                        ModContent.ProjectileType<Bee>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 1.5f), 0f, Main.myPlayer, npc.target, Main.rand.NextBool() ? -rotation : rotation);
                }
            }
            else
            {
                EnrageFactor = 0;
            }


            if (!SpawnedRoyalSubjectWave1 && npc.life < npc.lifeMax / 3 * 2 && npc.HasPlayerTarget)
            {
                SpawnedRoyalSubjectWave1 = true;

                Vector2 vector72 = new Vector2(npc.position.X + npc.width / 2 + Main.rand.Next(20) * npc.direction, npc.position.Y + npc.height * 0.8f);

                int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), vector72, ModContent.NPCType<RoyalSubject>(),
                    velocity: new Vector2(Main.rand.Next(-200, 201) * 0.1f, Main.rand.Next(-200, 201) * 0.1f));
                if (n != Main.maxNPCs)
                    Main.npc[n].localAI[0] = 60f;

                FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.RoyalSubject", new Color(175, 75, 255));

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (!SpawnedRoyalSubjectWave2 && npc.life < npc.lifeMax / 3 && npc.HasPlayerTarget)
            {
                SpawnedRoyalSubjectWave2 = true;

                if (FargoSoulsWorld.MasochistModeReal)
                    SpawnedRoyalSubjectWave1 = false; //do this again

                Vector2 vector72 = new Vector2(npc.position.X + npc.width / 2 + Main.rand.Next(20) * npc.direction, npc.position.Y + npc.height * 0.8f);

                int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), vector72, ModContent.NPCType<RoyalSubject>(),
                    velocity: new Vector2(Main.rand.Next(-200, 201) * 0.1f, Main.rand.Next(-200, 201) * 0.1f));
                if (n != Main.maxNPCs)
                    Main.npc[n].localAI[0] = 60f;

                FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.RoyalSubject", new Color(175, 75, 255));

                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<RoyalSubject>()); //so that both dont stack for being spawned from qb

                npc.netUpdate = true;
                NetSync(npc);
            }


            if (!InPhase2 && npc.life < npc.lifeMax / 2) //enable new attack and roar below 50%
            {
                InPhase2 = true;
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                if (FargoSoulsWorld.MasochistModeReal)
                    SpawnedRoyalSubjectWave1 = false; //do this again

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
            {
                npc.HitSound = SoundID.NPCHit4;
                npc.color = new Color(127, 127, 127);

                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustId].noGravity = true;
                int dustId3 = Dust.NewDust(npc.position, npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 2f);
                Main.dust[dustId3].noGravity = true;

                //if in dash mode, but not actually dashing right this second
                if (npc.ai[0] == 0 && npc.ai[1] % 2 == 0)
                {
                    npc.ai[0] = 3; //dont
                    npc.ai[1] = 0;
                    npc.netUpdate = true;
                }

                //shoot stingers mode
                if (npc.ai[0] == 3)
                {
                    if (npc.ai[1] > 1 && !FargoSoulsWorld.MasochistModeReal)
                        npc.ai[1] -= 0.5f; //slower stingers
                }
            }
            else
            {
                npc.HitSound = SoundID.NPCHit1;
                npc.color = default;

                if (InPhase2 && HiveThrowTimer % 2 == 0)
                    HiveThrowTimer++; //throw hives faster when no royal subjects alive
            }

            if (FargoSoulsWorld.MasochistModeReal)
            {
                HiveThrowTimer++;

                if (ForgorDeathrayTimer > 0 && --ForgorDeathrayTimer % 10 == 0 && npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(),
                        Main.player[npc.target].Center - 2000 * Vector2.UnitY, Vector2.UnitY,
                        ModContent.ProjectileType<WillDeathraySmall>(),
                        (int)(npc.damage * .75), 0f, Main.myPlayer,
                        Main.player[npc.target].Center.X, npc.whoAmI);
                }
            }

            if (InPhase2)
            {
                if (++HiveThrowTimer > 570 && BeeSwarmTimer <= 600 && (npc.ai[0] == 3f || npc.ai[0] == 1f)) //lobs hives below 50%, not dashing
                {
                    HiveThrowTimer = 0;

                    npc.netUpdate = true;
                    NetSync(npc);

                    const float gravity = 0.25f;
                    float time = 75f;
                    Vector2 distance = Main.player[npc.target].Center - Vector2.UnitY * 16 - npc.Center + Main.player[npc.target].velocity * 30f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance, ModContent.ProjectileType<Beehive>(),
                            FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, time - 5);
                    }
                }

                if (npc.ai[0] == 0 && npc.ai[1] == 1f) //if qb tries to start doing dashes of her own volition
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f; //don't
                    npc.netUpdate = true;
                }
            }

            //only while stationary mode
            if (npc.ai[0] == 3f || npc.ai[0] == 1f)
            {
                if (InPhase2 && ++BeeSwarmTimer > 600)
                {
                    if (BeeSwarmTimer < 720) //slow down
                    {
                        if (BeeSwarmTimer == 601)
                        {
                            npc.netUpdate = true;
                            NetSync(npc);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                            if (npc.HasValidTarget)
                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, Main.player[npc.target].Center); //eoc roar

                            if (FargoSoulsWorld.MasochistModeReal)
                                BeeSwarmTimer += 30;
                        }

                        if (Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            npc.velocity *= 0.975f;
                        }
                        else if (BeeSwarmTimer > 630)
                        {
                            BeeSwarmTimer--; //stall this section until has line of sight
                            return true;
                        }
                    }
                    else if (BeeSwarmTimer < 840) //spray bees
                    {
                        npc.velocity = Vector2.Zero;

                        if (BeeSwarmTimer % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const float rotation = 0.025f;
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + new Vector2(3 * npc.direction, 15), i * Main.rand.NextFloat(9f, 18f) * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))),
                                    ModContent.ProjectileType<Bee>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, FargoSoulsWorld.MasochistModeReal ? 4f / 3 : 1), 0f, Main.myPlayer, npc.target, Main.rand.NextBool() ? -rotation : rotation);
                            }
                        }
                    }
                    else if (BeeSwarmTimer > 870) //return to normal AI
                    {
                        BeeSwarmTimer = 0;
                        HiveThrowTimer -= 60;

                        npc.netUpdate = true;
                        NetSync(npc);

                        npc.ai[0] = 0f;
                        npc.ai[1] = 4f; //trigger dashes, but skip the first one
                        npc.ai[2] = -44f;
                        npc.ai[3] = 0f;
                    }

                    if (npc.netUpdate)
                    {
                        npc.netUpdate = false;

                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                    return false;
                }

                int threshold = FargoSoulsWorld.MasochistModeReal ? 90 : 120;

                if (++StingerRingTimer > threshold * 3)
                    StingerRingTimer = 0;

                if (StingerRingTimer % threshold == 0)
                {
                    float speed = FargoSoulsWorld.MasochistModeReal ? 6 : 5;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        FargoSoulsUtil.XWay(StingerRingTimer == threshold * 3 ? 16 : 8, npc.GetSource_FromThis(), npc.Center, ProjectileID.QueenBeeStinger, speed, 11, 1);
                }
            }

            if (npc.ai[0] == 0 && npc.ai[1] == 4) //when about to do dashes triggered by royal subjects/bee swarm, telegraph and stall
            {
                if (npc.ai[2] < 0)
                {
                    if (npc.ai[2] == -44) //telegraph
                    {
                        SoundEngine.PlaySound(SoundID.Item21, npc.Center);

                        for (int i = 0; i < 44; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.NextBool() ? 152 : 153, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f);
                            Main.dust[d].scale = Main.rand.NextFloat(1f, 3f);
                            Main.dust[d].velocity *= Main.rand.NextFloat(4.4f);
                            Main.dust[d].noGravity = Main.rand.NextBool();
                            if (Main.dust[d].noGravity)
                            {
                                Main.dust[d].scale *= 2.2f;
                                Main.dust[d].velocity *= 4.4f;
                            }
                        }

                        if (FargoSoulsWorld.MasochistModeReal)
                            npc.ai[2] = 0;

                        ForgorDeathrayTimer = 95;
                    }

                    npc.velocity *= 0.95f;
                    npc.ai[2]++;

                    return false;
                }
            }

            if (FargoSoulsWorld.MasochistModeReal)
            {
                //if in dash mode, but not actually dashing right this second
                if (npc.ai[0] == 0 && npc.ai[1] % 2 == 0)
                {
                    if (npc.HasValidTarget && Math.Abs(Main.player[npc.target].Center.Y - npc.Center.Y) > npc.velocity.Y * 2)
                        npc.position.Y += npc.velocity.Y;
                }
            }

            EModeUtils.DropSummon(npc, "Abeemination2", NPC.downedQueenBee, ref DroppedSummon);

            return result;
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);

            if (!npc.HasValidTarget || (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) > 3000))
            {
                if (npc.timeLeft > 60)
                    npc.timeLeft = 60;
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if ((int)(Main.time / 60 - 30) % 60 == 22) //COOMEDY
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ModContent.ItemType<TwentyTwoPainting>());
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<QueenStinger>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.JungleFishingCrate, 5));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HerbBag, 5));
            npcLoot.Add(emodeRule);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Infested>(), 300);
            target.AddBuff(ModContent.BuffType<Swarming>(), 600);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (!FargoSoulsWorld.SwarmActive && NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
                damage /= 2;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 14);
            LoadGoreRange(recolor, 303, 308);
        }
    }
}
