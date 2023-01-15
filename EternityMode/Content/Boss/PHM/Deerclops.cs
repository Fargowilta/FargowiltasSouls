using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Consumables;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class Deerclops : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Deerclops);

        public int BerserkSpeedupTimer;
        public int TeleportTimer;
		public int WalkingSpeedUpTimer;

        public bool EnteredPhase2;
        public bool EnteredPhase3;
        public bool DoLaserAttack;

        public bool DroppedSummon;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(BerserkSpeedupTimer);
            binaryWriter.Write7BitEncodedInt(TeleportTimer);
            binaryWriter.Write7BitEncodedInt(WalkingSpeedUpTimer);
            bitWriter.WriteBit(EnteredPhase2);
            bitWriter.WriteBit(EnteredPhase3);
            bitWriter.WriteBit(DoLaserAttack);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            BerserkSpeedupTimer = binaryReader.Read7BitEncodedInt();
            TeleportTimer = binaryReader.Read7BitEncodedInt();
            WalkingSpeedUpTimer = binaryReader.Read7BitEncodedInt();
            EnteredPhase2 = bitReader.ReadBit();
            EnteredPhase3 = bitReader.ReadBit();
            DoLaserAttack = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.25, MidpointRounding.ToEven);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Frostburn] = true;
            npc.buffImmune[BuffID.Frostburn2] = true;
            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.Frozen] = true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            if (npc.alpha > 0)
                return false;

            return base.CanHitPlayer(npc, target, ref CooldownSlot);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            EModeGlobalNPC.deerBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            const int MaxBerserkTime = 600;

            BerserkSpeedupTimer -= 1;

            if (npc.localAI[3] > 0 || EnteredPhase3)
                npc.localAI[2]++; //cry about it

            const int TeleportThreshold = 780;

            if (npc.ai[0] != 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                if (EnteredPhase3)
                    npc.localAI[2]++;
            }

            TeleportTimer++;
            if (EnteredPhase3)
                TeleportTimer++;

            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 1000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGround>(), 2);

            switch ((int)npc.ai[0])
            {
                case 0: //walking at player
                    if (++WalkingSpeedUpTimer > 900) //scaling capped for edge case sanity
                        WalkingSpeedUpTimer = 900;
                    //after walking for a bit, begin walking faster to catch up to outrunning player
                    npc.position.X += npc.velocity.X * Math.Max(0, WalkingSpeedUpTimer - 90) / 90f;

                    if (TeleportTimer < TeleportThreshold)
                    {
                        if (EnteredPhase3)
                            npc.position.X += npc.velocity.X;

                        if (npc.velocity.Y == 0)
                        {
                            if (EnteredPhase2)
                                npc.position.X += npc.velocity.X;
                            if (BerserkSpeedupTimer > 0)
                                npc.position.X += npc.velocity.X * 4f * BerserkSpeedupTimer / MaxBerserkTime;
                        }
                    }

                    if (EnteredPhase2)
                    {
                        if (!EnteredPhase3 && npc.life < npc.lifeMax * .33)
                        {
                            npc.ai[0] = 3;
                            npc.ai[1] = 0;
                            npc.netUpdate = true;
                            break;
                        }

                        if (TeleportTimer > TeleportThreshold)
                        {
                            WalkingSpeedUpTimer = 0;

                            npc.velocity.X *= 0.9f;
                            npc.dontTakeDamage = true;
                            npc.localAI[1] = 0; //reset walls attack counter

                            if (EnteredPhase2 && Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 1600)
                            {
                                FargoSoulsUtil.AddDebuffFixedDuration(Main.LocalPlayer, BuffID.Darkness, 2);
                                FargoSoulsUtil.AddDebuffFixedDuration(Main.LocalPlayer, BuffID.Blackout, 2);
                            }

                            if (npc.alpha == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                                SpawnFreezeHands(npc);
                            }

                            npc.alpha += 5;
                            if (npc.alpha > 255)
                            {
                                npc.alpha = 255;

                                npc.localAI[3] = 30;

                                if (npc.HasPlayerTarget) //teleport
                                {
                                    float distance = 16 * 14 * Math.Sign(npc.Center.X - Main.player[npc.target].Center.X);
                                    distance *= -1f; //alternate back and forth

                                    if (TeleportTimer == TeleportThreshold + 10) //introduce randomness
                                    {
                                        if (Main.rand.NextBool())
                                            distance *= -1f;

                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);

                                        DoLaserAttack = !DoLaserAttack; //guarantee he alternates wall attacks at some point in the fight
                                        NetSync(npc);
                                    }

                                    npc.Bottom = Main.player[npc.target].Bottom + distance * Vector2.UnitX;

                                    npc.direction = Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);
                                    npc.velocity.X = 3.4f * npc.direction;
                                    npc.velocity.Y = 0;

                                    int addedThreshold = 180;
                                    if (EnteredPhase3)
                                        addedThreshold -= 30;
                                    if (FargoSoulsWorld.MasochistModeReal)
                                        addedThreshold -= 30;

                                    if (TeleportTimer > TeleportThreshold + addedThreshold)
                                    {
                                        TeleportTimer = 0;
                                        npc.velocity.X = 0;
                                        npc.ai[0] = 4;
                                        npc.ai[1] = 0;
                                        NetSync(npc);
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                                    }
                                }
                            }
                            else
                            {
                                TeleportTimer = TeleportThreshold;

                                if (npc.localAI[3] > 0)
                                    npc.localAI[3] -= 3; //remove visual effect
                            }

                            return false;
                        }
                    }
                    else if (npc.life < npc.lifeMax * .66)
                    {
                        npc.ai[0] = 3;
                        npc.ai[1] = 0;
                        npc.netUpdate = true;
                    }
					
                    break;

                case 1: //ice wave, npc.localai[1] counts them, attacks at ai1=30, last spike 52, ends at ai1=80
                    WalkingSpeedUpTimer = 0;
					
					if (npc.ai[1] < 30)
                    {
                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            npc.ai[1] += 0.5f;
                            npc.frameCounter += 0.5;
                        }
                    }
                    break;

                case 2: //debris attack
                    break;

                case 3: //roar at 30, ends at ai1=60
					WalkingSpeedUpTimer = 0;
				
                    if (!FargoSoulsWorld.MasochistModeReal && npc.ai[1] < 30)
                    {
                        npc.ai[1] -= 0.5f;
                        npc.frameCounter -= 0.5;
                    }

                    if (EnteredPhase2)
                    {
                        npc.localAI[1] = 0; //ensure this is always the same
                        npc.localAI[3] = 30; //go invul

                        if (npc.ai[1] > 30)
                        {
                            Main.dayTime = false;
                            Main.time = 16200; //midnight, to help teleport visual
                        }
                    }
                    else if (npc.life < npc.lifeMax * .66)
                    {
                        EnteredPhase2 = true;
                        NetSync(npc);
                    }

                    if (EnteredPhase3)
                    {
                        if (!Main.dedServ)
                            Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

                        if (npc.ai[1] > 30) //roaring
                        {
                            if (npc.HasValidTarget) //fly over player
                                npc.position = Vector2.Lerp(npc.position, Main.player[npc.target].Center - 450 * Vector2.UnitY, 0.2f);
                        }
                    }
                    else if (npc.life < npc.lifeMax * .33)
                    {
                        EnteredPhase3 = true;
                        NetSync(npc);
                    }

                    if (EnteredPhase3 || FargoSoulsWorld.MasochistModeReal)
                        BerserkSpeedupTimer = MaxBerserkTime;
                    break;

                case 4: //both sides ice wave, attacks at ai1=50, last spike 70, ends at ai1=90
                    {
						WalkingSpeedUpTimer = 0;
						
                        int cooldown = 100; //stops deerclops from teleporting while old ice walls are still there
                        if (EnteredPhase3)
                            cooldown *= 2;
                        if (TeleportTimer > TeleportThreshold - cooldown)
                            TeleportTimer = TeleportThreshold - cooldown;

                        if (EnteredPhase2 && npc.ai[1] == 0)
                        {
                            if (npc.alpha == 0) //i.e. dont randomize when coming out of tp
                                DoLaserAttack = Main.rand.NextBool();
                            NetSync(npc);

                            if (DoLaserAttack && Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                        }

                        Vector2 eye = npc.Center + new Vector2(64 * npc.direction, -24f) * npc.scale;

                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            const int desiredStartup = 30; //effectively changes startup from 50 to this value
                            const int threshold = 50 - desiredStartup / 2;
                            if (npc.ai[1] < threshold)
                                npc.ai[1]++;
                        }

                        if (DoLaserAttack && npc.ai[1] >= 70)
                        {
                            if (EnteredPhase3)
                            {
                                const float baseIncrement = 0.33f;
                                float increment = baseIncrement;
                                //if (FargoSoulsWorld.MasochistModeReal) increment *= 2;

                                if (npc.ai[1] == 70) //shoot laser
                                {
                                    float time = (90 - 70) / baseIncrement - 5;
                                    time *= 5; //account for the ray having extra updates
                                    float rotation = MathHelper.Pi * (FargoSoulsWorld.MasochistModeReal ? 1f : 0.8f) / time * -npc.direction;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), eye, Vector2.UnitY, ModContent.ProjectileType<DeerclopsDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 2f), 0f, Main.myPlayer, rotation, time);
                                }

                                npc.ai[1] += increment; //more endlag than normal

                                if (npc.ai[1] < 90)
                                    return false; //stop deerclops from turning around
                            }
                            else
                            {
                                npc.ai[1] += 0.33f; //more endlag than normal

                                if (npc.ai[1] >= 89)
                                {
                                    npc.ai[0] = 2; //force debris attack instead
                                    npc.ai[1] = 0;
                                    npc.frameCounter = 0;
                                    npc.netUpdate = true;
                                    break;
                                }
                            }

                            if (npc.ai[1] < 90)
                                return false; //stop deerclops from turning around
                        }
                    }
                    break;

                case 5: //another roar?
                    if (npc.ai[1] == 30)
                    {
                        //if player is somehow directly above deerclops at moment of roar
                        if (npc.HasValidTarget && Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 16 * 3
                            && Main.player[npc.target].Bottom.Y < npc.Top.Y - 16 * 5)
                        {
                            //freeze them and drag them down
                            SpawnFreezeHands(npc);
                        }
                    }
                    break;

                case 6: //trying to return home
                    npc.TargetClosest();

                    if (npc.ai[1] > 120 && (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 1600))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient) //force despawn
                        {
                            npc.ai[0] = 8f;
                            npc.ai[1] = 0.0f;
                            npc.localAI[1] = 0.0f;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                default:
                    break;
            }

            //FargoSoulsUtil.PrintAI(npc);

            if (EnteredPhase3 && !(npc.ai[0] == 0 && npc.alpha > 0))
            {
                npc.localAI[3] += 3;
                if (npc.localAI[3] > 30)
                    npc.localAI[3] = 30;
            }

            //FargoSoulsUtil.PrintAI(npc);

            EModeUtils.DropSummon(npc, "DeerThing2", NPC.downedDeerclops, ref DroppedSummon);

            return result;
        }
		
		void SpawnFreezeHands(NPC npc)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				const int max = 12;
				for (int i = 0; i < 12; i++)
				{
					Vector2 spawnPos = Main.player[npc.target].Center + 16 * Main.rand.NextFloat(6, 36) * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * (i + Main.rand.NextFloat()));
					Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<DeerclopsHand>(), 0, 0f, Main.myPlayer, npc.target);
				}
			}
		}

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<Deerclawps>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<DeerSinew>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.FrozenCrate, 5));
            npcLoot.Add(emodeRule);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Frostburn, 90);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
            target.AddBuff(ModContent.BuffType<Hypothermia>(), 1200);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 39);
            LoadGore(recolor, 1270);
            LoadGore(recolor, 1272);
            LoadGore(recolor, 1273);
            LoadGore(recolor, 1274);
        }
    }
}
