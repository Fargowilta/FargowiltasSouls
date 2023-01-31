using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class SkeletronPrime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronPrime);

        public int DungeonGuardianStartup;
        public int MemorizedTarget;

        public bool FullySpawnedLimbs;
        public bool HaveShotGuardians;

        public int ProjectileAttackTimer;
        public int RocketTimer;
        public bool DroppedSummon;
        public bool HasSaidEndure;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(DungeonGuardianStartup);
            binaryWriter.Write7BitEncodedInt(MemorizedTarget);
            bitWriter.WriteBit(FullySpawnedLimbs);
            bitWriter.WriteBit(HaveShotGuardians);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            DungeonGuardianStartup = binaryReader.Read7BitEncodedInt();
            MemorizedTarget = binaryReader.Read7BitEncodedInt();
            FullySpawnedLimbs = bitReader.ReadBit();
            HaveShotGuardians = bitReader.ReadBit();
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
            bool result = base.SafePreAI(npc);

            EModeGlobalNPC.primeBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            if (npc.ai[1] == 3) //despawn faster
            {
                if (npc.timeLeft > 60)
                    npc.timeLeft = 60;
            }

            if (npc.ai[1] == 0f)
            {
                HaveShotGuardians = false;

                if (npc.ai[2] == 600 - 90) //telegraph spin
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                }
                if (npc.ai[2] < 600 - 5)
                {
                    MemorizedTarget = npc.target;
                }
            }

            if (npc.ai[1] == 1f)
            {
                if (MemorizedTarget > -1 && MemorizedTarget < Main.maxPlayers)
                {
                    npc.target = MemorizedTarget;
                    npc.netUpdate = true;
                    MemorizedTarget = -1;
                    if (!npc.HasValidTarget)
                        npc.TargetClosest(false);
                }
            }

            if (npc.ai[0] != 2f || FargoSoulsWorld.MasochistModeReal)
            {
                if (!HaveShotGuardians && npc.ai[1] == 1f && npc.ai[2] > 2f) //spinning, do wave of guardians
                {
                    HaveShotGuardians = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = -2; j <= 2; j++)
                            {
                                Vector2 spawnPos = new Vector2(1200, 80 * j);
                                Vector2 vel = -10 * Vector2.UnitX;
                                spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * i);
                                vel = vel.RotatedBy(Math.PI / 2 * i);
                                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<PrimeGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 1200 / 10 + 1;
                            }
                        }
                    }
                }

                if (++RocketTimer >= 360)
                {
                    RocketTimer = 0;

                    if (npc.HasPlayerTarget) //skeleton commando rockets LUL
                    {
                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                        speed.X += Main.rand.Next(-20, 21);
                        speed.Y += Main.rand.Next(-20, 21);
                        speed.Normalize();

                        int damage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 3f * speed, ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(5f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(-5f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                        }

                        SoundEngine.PlaySound(SoundID.Item11, npc.Center);
                    }
                }
            }

            if (npc.ai[0] != 2f) //in phase 1
            {
                if (npc.life < npc.lifeMax * .8) //enter phase 2
                {
                    npc.ai[0] = 2f;

                    SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                    return result;
                }
            }
            else //in phase 2
            {
                npc.dontTakeDamage = false;

                if (npc.ai[1] == 1f && npc.ai[2] > 2f) //spinning
                {
                    //if (npc.HasValidTarget) npc.position += npc.DirectionTo(Main.player[npc.target].Center) * 5;

                    if (++ProjectileAttackTimer > 90) //projectile attack
                    {
                        ProjectileAttackTimer = -30;
                        int damage = npc.defDamage / 3;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            SoundEngine.PlaySound(SoundID.Item105 with { Volume = 2f }, npc.Center);

                            float modifier = (float)npc.life / npc.lifeMax;
                            if (FargoSoulsWorld.MasochistModeReal)
                                modifier = 0;
                            int starMax = (int)(7f - 6f * modifier);

                            const int max = 8;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 speed = 12f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(2 * Math.PI / max * i);
                                for (int j = -starMax; j <= starMax; j++)
                                {
                                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed.RotatedBy(MathHelper.ToRadians(2f) * j),
                                        ModContent.ProjectileType<DarkStar>(), damage, 0f, Main.myPlayer, -1, 180);
                                    Main.projectile[p].soundDelay = -1; //dont play sounds
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 300;
                                }
                            }
                        }
                    }
                }
                else if (npc.ai[1] == 2f) //dg phase
                {
                    while (npc.buffType[0] != 0)
                    {
                        npc.buffImmune[npc.buffType[0]] = true;
                        npc.DelBuff(0);
                    }

                    if (!Main.dayTime && !FargoSoulsWorld.MasochistModeReal)
                    {
                        npc.position -= npc.velocity * 0.1f;
                        if (++DungeonGuardianStartup < 120)
                            npc.position -= npc.velocity * (120 - DungeonGuardianStartup) / 120 * 0.9f;
                    }
                }
                else //not spinning
                {
                    ProjectileAttackTimer = 0; //buffer this for spin

                    npc.position += npc.velocity / 4f;
                }

                //spawn 4 more limbs
                if (!FullySpawnedLimbs && (npc.life < npc.lifeMax * 0.6 || FargoSoulsWorld.MasochistModeReal) && npc.ai[3] >= 0f)
                {
                    if (npc.ai[3] == 0)
                    {
                        npc.ai[1] = 0f; //revert to nonspin mode
                        npc.ai[2] = 600f - 90f - 2f; //but only for telegraph and then go back into spin

                        npc.ai[3] = 0f;
                        npc.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);

                        if (!NPC.AnyNPCs(NPCID.PrimeLaser)) //revive all dead limbs
                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeLaser, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                        if (!NPC.AnyNPCs(NPCID.PrimeSaw))
                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeSaw, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                        if (!NPC.AnyNPCs(NPCID.PrimeCannon))
                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeCannon, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                        if (!NPC.AnyNPCs(NPCID.PrimeVice))
                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeVice, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);

                        FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.SkeletronPrimeRegrow", new Color(175, 75, 255));
                    }

                    npc.ai[3]++;
                    if (npc.ai[3] == 60f) //first set of limb management
                    {
                        int[] limbs = { NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice };

                        foreach (NPC l in Main.npc.Where(l => l.active && l.ai[1] == npc.whoAmI && limbs.Contains(l.type)))
                        {
                            l.GetGlobalNPC<PrimeLimb>().IsSwipeLimb = true;
                            l.ai[2] = 0;

                            int heal = (l.lifeMax - l.life) / 2;
                            l.life += heal;
                            if (heal > 0)
                                l.HealEffect(heal);
                            l.dontTakeDamage = false;

                            l.netUpdate = true;
                            NetSync(l);
                        }

                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeLaser, npc.whoAmI, -1f, npc.whoAmI, 0f, 150f, npc.target);
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeSaw, npc.whoAmI, -1f, npc.whoAmI, 0f, 0f, npc.target);
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeCannon, npc.whoAmI, 1f, npc.whoAmI, 0f, 150f, npc.target);
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.PrimeVice, npc.whoAmI, 1f, npc.whoAmI, 0f, 0f, npc.target);
                    }
                    else if (npc.ai[3] >= 180f)
                    {
                        FullySpawnedLimbs = true;
                        npc.ai[3] = -1f;
                        npc.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int rangedArm = Main.rand.NextBool() ? NPCID.PrimeCannon : NPCID.PrimeLaser;
                            int meleeArm = Main.rand.NextBool() ? NPCID.PrimeSaw : NPCID.PrimeVice;

                            int[] limbs = { NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice };

                            foreach (NPC l in Main.npc.Where(l => l.active && l.ai[1] == npc.whoAmI && limbs.Contains(l.type) && !l.GetGlobalNPC<PrimeLimb>().IsSwipeLimb))
                            {
                                l.GetGlobalNPC<PrimeLimb>().RangedAttackMode = npc.type == rangedArm || npc.type == meleeArm;

                                int heal = l.lifeMax;
                                l.life = Math.Min(l.life + l.lifeMax / 2, l.lifeMax);
                                heal -= l.life;
                                if (heal > 0)
                                    l.HealEffect(heal);

                                l.dontTakeDamage = false;

                                l.netUpdate = true;
                                NetSync(l);
                            }
                        }
                    }
                }
            }

            //accel at player whenever out of range
            if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) > 900)
            {
                npc.velocity += 0.1f * npc.DirectionTo(Main.player[npc.target].Center);
            }

            EModeUtils.DropSummon(npc, "MechSkull", NPC.downedMechBoss3, ref DroppedSummon, Main.hardMode);

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Defenseless>(), 480);
            target.AddBuff(ModContent.BuffType<NanoInjection>(), 360);
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.ai[1] != 2f && !FargoSoulsWorld.SwarmActive)
            {
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                npc.life = npc.lifeMax / 630;
                if (npc.life < 100)
                    npc.life = 100;
                npc.defDefense = 9999;
                npc.defense = 9999;
                npc.defDamage *= 13;
                npc.damage *= 13;
                npc.ai[1] = 2f;
                npc.netUpdate = true;

                if (!HasSaidEndure)
                {
                    HasSaidEndure = true;
                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.SkeletronPrimeGuardian", new Color(175, 75, 255));
                }

                if (!FargoSoulsWorld.MasochistModeReal)
                {
                    for (int i = 0; i < Main.maxNPCs; i++) //kill limbs while going dg
                    {
                        if (Main.npc[i].active &&
                            (Main.npc[i].type == NPCID.PrimeCannon || Main.npc[i].type == NPCID.PrimeLaser || Main.npc[i].type == NPCID.PrimeSaw || Main.npc[i].type == NPCID.PrimeVice)
                            && Main.npc[i].ai[1] == npc.whoAmI)
                        {
                            Main.npc[i].life = 0;
                            Main.npc[i].HitEffect();
                            Main.npc[i].checkDead();
                        }
                    }
                }
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<ReinforcedPlating>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 18);
            LoadGoreRange(recolor, 147, 150);
            LoadSpecial(recolor, ref TextureAssets.BoneArm2, ref FargowiltasSouls.TextureBuffer.BoneArm2, "Arm_Bone_2");
        }
    }

    public class PrimeLimb : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice);

        public int IdleOffsetX;
        public int IdleOffsetY;
        public int AttackTimer;
        public int NoContactDamageTimer;

        public float SpinRotation;

        public bool RangedAttackMode;
        public bool IsSwipeLimb;
        public bool InSpinningMode;
        public bool ModeReset;
        public bool CardinalSwipe;

        public int DontActWhenSpawnedTimer = 180;



        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(IdleOffsetX);
            binaryWriter.Write7BitEncodedInt(IdleOffsetY);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(NoContactDamageTimer);
            binaryWriter.Write(SpinRotation);
            bitWriter.WriteBit(RangedAttackMode);
            bitWriter.WriteBit(IsSwipeLimb);
            bitWriter.WriteBit(InSpinningMode);
            bitWriter.WriteBit(ModeReset);
            bitWriter.WriteBit(CardinalSwipe);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            IdleOffsetX = binaryReader.Read7BitEncodedInt();
            IdleOffsetY = binaryReader.Read7BitEncodedInt();
            AttackTimer = binaryReader.Read7BitEncodedInt();
            NoContactDamageTimer = binaryReader.Read7BitEncodedInt();
            SpinRotation = binaryReader.ReadSingle();
            RangedAttackMode = bitReader.ReadBit();
            IsSwipeLimb = bitReader.ReadBit();
            InSpinningMode = bitReader.ReadBit();
            ModeReset = bitReader.ReadBit();
            CardinalSwipe = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (FargoSoulsWorld.MasochistModeReal)
                npc.lifeMax = (int)(npc.lifeMax * 1.5);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            npc.buffImmune[ModContent.BuffType<Lethargic>()] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            if (NoContactDamageTimer > 0)
                NoContactDamageTimer--;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronPrime);
            if (head == null)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.life = 0; //die if prime gone
                    npc.HitEffect();
                    npc.checkDead();
                }
                return false;
            }

            if (!head.HasValidTarget || head.ai[1] == 3) //return to default ai when death
                return true;

            if (head.ai[0] != 2f) //head in phase 1
            {
                if (DontActWhenSpawnedTimer > 0)
                {
                    DontActWhenSpawnedTimer--;
					
                    npc.Center = head.Center;
					
					//when into position above player, enter regular ai much more rapidly
					if (head.HasValidTarget)
					{
						Vector2 targetPos = Main.player[head.target].Center;
						targetPos.Y -= 16 * 16;
						if (npc.Distance(targetPos) < 16 * 16 && head.velocity.Length() < 8)
							DontActWhenSpawnedTimer = 0;
					}
					
                    return false;
                }
            }

            if (npc.timeLeft < 600)
                npc.timeLeft = 600;

            if (npc.dontTakeDamage)
            {
                if (npc.buffType[0] != 0)
                    npc.DelBuff(0);

                if (head.ai[0] != 2) //not in phase 2
                    npc.position -= npc.velocity / 2;

                int d = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.NextBool() ? DustID.Smoke : DustID.Torch, 0f, 0f, 100, default(Color), 2f);
                Main.dust[d].noGravity = Main.rand.NextBool();
                Main.dust[d].velocity *= 2f;
            }

            if (npc.type == NPCID.PrimeCannon)
            {
                if (npc.dontTakeDamage && npc.localAI[0] > 1)
                    npc.localAI[0] -= 0.5f;

                if (npc.ai[2] != 0f) //dark stars instead of cannonballs during super fast fire
                {
                    if (npc.localAI[0] > 30f)
                    {
                        npc.localAI[0] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 speed = new Vector2(16f, 0f).RotatedBy(npc.rotation + Math.PI / 2);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<DarkStar>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else if (npc.type == NPCID.PrimeLaser)
            {
                if (npc.dontTakeDamage && npc.localAI[0] > 1)
                    npc.localAI[0] -= 0.5f;

                if (npc.localAI[0] > 180) //vanilla threshold is 200
                {
                    npc.localAI[0] = 0;

                    Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center);
                    for (int j = -2; j <= 2; j++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 7f * baseVel.RotatedBy(MathHelper.ToRadians(1f) * j),
                                ProjectileID.DeathLaser, head.defDamage / 4, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            npc.damage = head.ai[0] == 2f ? (int)(head.defDamage * 1.25) : npc.defDamage;

            bool useNormalAi = false;

            if (head.ai[0] == 2f) //phase 2
            {
                npc.target = head.target;
                if (head.ai[1] == 3 || !npc.HasValidTarget) //return to normal AI
                    return true;

                if (IsSwipeLimb) //swipe AI
                {
                    npc.damage = (int)(head.defDamage * 1.25);

                    //only selfdestruct once prime is done spawning limbs
                    if (npc.life == 1 && head.GetGlobalNPC<SkeletronPrime>().FullySpawnedLimbs)
                    {
                        npc.dontTakeDamage = false; //for client side so you can hit the limb and update this
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.life = 0;
                            npc.HitEffect();
                            npc.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            return false;
                        }
                    }

                    if (!ModeReset)
                    {
                        ModeReset = true;
                        switch (npc.type)
                        {
                            case NPCID.PrimeCannon: IdleOffsetX = -1; IdleOffsetY = -1; break;
                            case NPCID.PrimeLaser: IdleOffsetX = 1; IdleOffsetY = -1; break;
                            case NPCID.PrimeSaw: IdleOffsetX = -1; IdleOffsetY = 1; break;
                            case NPCID.PrimeVice: IdleOffsetX = 1; IdleOffsetY = 1; break;
                            default: break;
                        }
                        npc.netUpdate = true;
                    }

                    if (++npc.ai[2] < 180)
                    {
                        Vector2 offset = new Vector2(400 * IdleOffsetX, 400 * IdleOffsetY);
                        if (CardinalSwipe)
                            offset = offset.RotatedBy(MathHelper.PiOver4);
                        
                        Vector2 target = Main.player[npc.target].Center + offset;
                        npc.velocity = (target - npc.Center) / 30;

                        if (npc.ai[2] == 140)
                        {
                            SoundEngine.PlaySound(SoundID.Item15 with { Volume = 1.5f }, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 0);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Souls.IronParry>(), 0, 0f, Main.myPlayer);
                            }
                        }

                        //npc.damage = 0;
                        if (NoContactDamageTimer < 2)
                            NoContactDamageTimer = 2;
                    }
                    else if (npc.ai[2] == 180)
                    {
                        SoundEngine.PlaySound(SoundID.Item18 with { Volume = 1.25f }, npc.Center);
                        npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 20f;
                        IdleOffsetX *= -1;
                        IdleOffsetY *= -1;

                        CardinalSwipe = !CardinalSwipe;

                        npc.netUpdate = true;
                    }
                    else if (npc.ai[2] < 210)
                    {

                    }
                    else
                    {
                        npc.ai[2] = head.ai[1] == 1 || head.ai[1] == 2 ? 0 : -90;

                        if (FargoSoulsWorld.MasochistModeReal)
                            npc.ai[2] += 60;

                        npc.netUpdate = true;
                    }

                    npc.rotation = head.DirectionTo(npc.Center).ToRotation() - (float)Math.PI / 2;
                }
                else if (head.ai[1] == 1 || head.ai[1] == 2) //other limbs while prime spinning
                {
                    //int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, npc.velocity.X * .4f, npc.velocity.Y * .4f, 0, Color.White, 2); Main.dust[d].noGravity = true;
                    if (!InSpinningMode) //AND STRETCH HIS ARMS OUT JUST FOR YOU
                    {
                        NoContactDamageTimer = 2; //no damage while moving into position

                        int rotation = 0;
                        switch (npc.type)
                        {
                            case NPCID.PrimeCannon: rotation = 0; break;
                            case NPCID.PrimeLaser: rotation = 1; break;
                            case NPCID.PrimeSaw: rotation = 2; break;
                            case NPCID.PrimeVice: rotation = 3; break;
                            default: break;
                        }
                        Vector2 offset = Main.player[head.target].Center - head.Center;
                        offset = offset.RotatedBy(Math.PI / 2 * rotation + Math.PI / 4);
                        offset = Vector2.Normalize(offset) * (offset.Length() + 200);
                        if (offset.Length() < 600)
                            offset = Vector2.Normalize(offset) * 600;
                        Vector2 target = head.Center + offset;

                        npc.velocity = (target - npc.Center) / 20;

                        if (IdleOffsetX == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 1f);

                        if (++IdleOffsetX > 60)
                        {
                            InSpinningMode = true;
                            IdleOffsetX = (int)offset.Length();
                            if (IdleOffsetX < 300)
                                IdleOffsetX = 300;
                            SpinRotation = head.DirectionTo(npc.Center).ToRotation();

                            npc.netUpdate = true;
                        }
                    }
                    else //spinning
                    {
                        ModeReset = true;

                        float range = IdleOffsetX; //extend further to hit player if beyond current range
                        if (head.HasValidTarget && head.Distance(Main.player[head.target].Center) > range)
                            range = head.Distance(Main.player[head.target].Center);

                        npc.Center = head.Center + new Vector2(range, 0f).RotatedBy(SpinRotation);
                        const float rotation = 0.1f;
                        SpinRotation += rotation;
                        if (SpinRotation > (float)Math.PI)
                        {
                            SpinRotation -= 2f * (float)Math.PI;

                            npc.netUpdate = true;
                        }
                    }
                    npc.rotation = head.DirectionTo(npc.Center).ToRotation() - (float)Math.PI / 2;

                    if (npc.type == NPCID.PrimeLaser)
                        npc.localAI[1] = 0;
                }
                else //regular limb ai
                {
                    if (ModeReset)
                    {
                        ModeReset = false;

                        RangedAttackMode = !RangedAttackMode; //resetting variables for next spin
                        InSpinningMode = false;
                        IdleOffsetX = 0;

                        AttackTimer = -30; //extra delay before initiating melee attacks after spin

                        NoContactDamageTimer = 60; //disable contact damage for 1sec after spin is over

                        npc.netUpdate = true;
                    }

                    if (RangedAttackMode)
                    {
                        if (npc.type == NPCID.PrimeCannon) //vanilla movement but more aggressive projectiles
                        {
                            useNormalAi = true;

                            if (NoContactDamageTimer == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 8, npc.whoAmI);

                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 2);
                                }
                            }

                            if (npc.ai[2] == 0f)
                            {
                                npc.localAI[0]++;
                                npc.ai[3]++;
                            }
                        }
                        else if (npc.type == NPCID.PrimeLaser) //vanilla movement but modified lasers
                        {
                            useNormalAi = true;

                            if (NoContactDamageTimer == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = -1; i <= 1; i += 2)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(20) * i),
                                              ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 8, npc.whoAmI);
                                    }

                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 2);
                                }
                            }

                            if (npc.Distance(head.Center) > 400) //drag back to prime if it drifts away
                            {
                                npc.velocity = (head.Center - npc.Center) / 30;
                            }

                            npc.localAI[0] = 0;
                            if (++npc.localAI[1] == 95) //v spread shot
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.ToRadians(20) * i);
                                    for (int j = -3; j <= 3; j++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 7.5f * baseVel.RotatedBy(MathHelper.ToRadians(1f) * j),
                                                  ProjectileID.DeathLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                            else if (npc.localAI[1] > 190) //direct spread shot
                            {
                                npc.localAI[1] = 0;

                                Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center);
                                for (int j = -3; j <= 3; j++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 7.5f * baseVel.RotatedBy(MathHelper.ToRadians(1f) * j),
                                            ProjectileID.DeathLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else //fold arms when not in use
                        {
                            Vector2 distance = head.Center - npc.Center;
                            distance /= 8f;
                            npc.velocity = (npc.velocity * 23f + distance) / 24f;

                            if (NoContactDamageTimer < 2)
                                NoContactDamageTimer = 2;
                        }
                    }
                    else
                    {
                        if (npc.type == NPCID.PrimeSaw)
                        {
                            if (NoContactDamageTimer == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 2);
                            }

                            if (++AttackTimer < 90) //try to relocate to near player
                            {
                                if (!npc.HasValidTarget)
                                    npc.TargetClosest(false);

                                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                                vel.X -= 450f * Math.Sign(vel.X);
                                vel.Y -= 150f;
                                vel.Normalize();
                                vel *= 20f;
                                const float moveSpeed = 0.75f;
                                if (npc.velocity.X < vel.X)
                                {
                                    npc.velocity.X += moveSpeed;
                                    if (npc.velocity.X < 0 && vel.X > 0)
                                        npc.velocity.X += moveSpeed;
                                }
                                else if (npc.velocity.X > vel.X)
                                {
                                    npc.velocity.X -= moveSpeed;
                                    if (npc.velocity.X > 0 && vel.X < 0)
                                        npc.velocity.X -= moveSpeed;
                                }
                                if (npc.velocity.Y < vel.Y)
                                {
                                    npc.velocity.Y += moveSpeed;
                                    if (npc.velocity.Y < 0 && vel.Y > 0)
                                        npc.velocity.Y += moveSpeed;
                                }
                                else if (npc.velocity.Y > vel.Y)
                                {
                                    npc.velocity.Y -= moveSpeed;
                                    if (npc.velocity.Y > 0 && vel.Y < 0)
                                        npc.velocity.Y -= moveSpeed;
                                }
                                npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - (float)Math.PI / 2;
                            }
                            else if (AttackTimer == 90)
                            {
                                SoundEngine.PlaySound(SoundID.Item18 with { Volume = 1.25f }, npc.Center);
                                npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * (npc.dontTakeDamage ? 20f : 25f);
                                npc.rotation = npc.velocity.ToRotation() - (float)Math.PI / 2;

                                npc.netUpdate = true;
                            }
                            else if (AttackTimer > 120)
                            {
                                AttackTimer = npc.dontTakeDamage ? -90 : 0;

                                npc.netUpdate = true;
                            }
                        }
                        else if (npc.type == NPCID.PrimeVice)
                        {
                            if (NoContactDamageTimer == 60) //indicate we're the active limbs
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<PrimeTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI, 2);
                                }
                            }

                            if (++AttackTimer < 90) //track to above player
                            {
                                if (!npc.HasValidTarget)
                                    npc.TargetClosest(false);

                                Vector2 target = Main.player[npc.target].Center;
                                target.X += npc.Center.X < target.X ? -50 : 50; //slightly to one side
                                target.Y -= 300;

                                npc.velocity = (target - npc.Center) / 40;
                            }
                            else if (AttackTimer == 90) //slash down
                            {
                                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 1.25f, Pitch = 0.5f }, npc.Center);
                                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                                vel.Y += Math.Abs(vel.X) * 0.25f;
                                vel.X *= 0.75f;
                                npc.velocity = Vector2.Normalize(vel) * (npc.dontTakeDamage ? 15f : 20f);

                                npc.netUpdate = true;
                            }
                            else if (AttackTimer > 120)
                            {
                                AttackTimer = npc.dontTakeDamage ? -90 : 0;

                                npc.netUpdate = true;
                            }

                            npc.rotation = npc.DirectionFrom(head.Center).ToRotation() - (float)Math.PI / 2;
                        }
                        else //fold arms when not in use
                        {
                            Vector2 distance = head.Center - npc.Center;
                            distance /= 8f;
                            npc.velocity = (npc.velocity * 23f + distance) / 24f;

                            if (NoContactDamageTimer < 2)
                                NoContactDamageTimer = 2;
                        }
                    }
                }

                if (npc.netUpdate)
                {
                    npc.netUpdate = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    NetSync(npc);
                }
                return useNormalAi;
            }

            return true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return NoContactDamageTimer <= 0;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<NanoInjection>(), 360);
        }

        public override bool CheckDead(NPC npc)
        {
            NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronPrime);
            if (head != null && head.ai[1] != 2)
            {
                npc.life = 1;
                npc.active = true;
                npc.dontTakeDamage = true;

                SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                for (int i = 0; i < 50; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Smoke, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }
                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[dust].velocity.Y -= Main.rand.NextFloat(2f);
                    Main.dust[dust].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.netUpdate = true;

                return false;
            }

            return base.CheckDead(npc);
        }


        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
