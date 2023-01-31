using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.MutantBoss;
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
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public abstract class PlanteraPart : EModeNPCBehaviour
    {
        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<IvyVenom>(), 240);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Plantera : PlanteraPart
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Plantera);

        public int DicerTimer;
        public int RingTossTimer;
        public int TentacleTimer = 480; //line up first tentacles with ring toss lmao, 600
        //public int TentacleTimerMaso;

        public float TentacleAttackAngleOffset;

        public bool IsVenomEnraged;
        public bool InPhase2;
        public bool EnteredPhase2;

        public bool DroppedSummon;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(DicerTimer);
            binaryWriter.Write7BitEncodedInt(RingTossTimer);
            binaryWriter.Write7BitEncodedInt(TentacleTimer);
            //binaryWriter.Write7BitEncodedInt(TentacleTimerMaso);
            bitWriter.WriteBit(IsVenomEnraged);
            bitWriter.WriteBit(InPhase2);
            bitWriter.WriteBit(EnteredPhase2);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            DicerTimer = binaryReader.Read7BitEncodedInt();
            RingTossTimer = binaryReader.Read7BitEncodedInt();
            TentacleTimer = binaryReader.Read7BitEncodedInt();
            //TentacleTimerMaso = binaryReader.Read7BitEncodedInt();
            IsVenomEnraged = bitReader.ReadBit();
            InPhase2 = bitReader.ReadBit();
            EnteredPhase2 = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.75);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            IsVenomEnraged = false;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            if (!npc.HasValidTarget)
                npc.velocity.Y++;

            const float innerRingDistance = 130f;
            const int delayForRingToss = 360 + 120;

            if (--RingTossTimer < 0)
            {
                RingTossTimer = delayForRingToss;
                if (Main.netMode != NetmodeID.MultiplayerClient && !Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
                {
                    const int max = 5;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(rotation * i);
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, rotation * i);
                    }
                }
            }
            else if (RingTossTimer == 120)
            {
                if (FargoSoulsWorld.MasochistModeReal)
                    RingTossTimer = 0; //instantly spawn next set of crystals

                npc.netUpdate = true;
                NetSync(npc);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float speed = 8f;
                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed * npc.DirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<MutantMark2>(), npc.defDamage / 4, 0f, Main.myPlayer);
                    if (p != Main.maxProjectiles)
                    {
                        Main.projectile[p].timeLeft -= 300;

                        foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance)) //my crystal leaves
                        {
                            SoundEngine.PlaySound(SoundID.Grass, n.Center);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<PlanteraCrystalLeafRing>(), npc.defDamage / 4, 0f, Main.myPlayer, Main.projectile[p].identity, n.ai[3]);

                            n.life = 0;
                            n.HitEffect();
                            n.checkDead();
                            n.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                    }
                }
            }

            if (npc.life > npc.lifeMax / 2)
            {
                if (--DicerTimer < 0)
                {
                    DicerTimer = 150 * 4 + 25;
                    if (FargoSoulsWorld.MasochistModeReal && npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, Vector2.Zero, ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 0, 0);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, 30f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(2 * (float)Math.PI / 3 * i),
                              ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 1, 1);
                        }
                    }
                }
            }
            else
            {
                if (!InPhase2)
                {
                    InPhase2 = true;
                    DicerTimer = 0;
                }

                void SpawnOuterLeafRing()
                {
                    const int max = 12;
                    const float distance = 250;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, distance, 0, rotation * i);
                    }
                }

                if (!EnteredPhase2)
                {
                    EnteredPhase2 = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
                        {
                            const int innerMax = 5;
                            float innerRotation = 2f * (float)Math.PI / innerMax;
                            for (int i = 0; i < innerMax; i++)
                            {
                                Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(innerRotation * i);
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, innerRotation * i);
                            }
                        }

                        SpawnOuterLeafRing();

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].hostile &&
                                (Main.projectile[i].type == ProjectileID.ThornBall
                                || Main.projectile[i].type == ModContent.ProjectileType<DicerPlantera>()
                                || Main.projectile[i].type == ModContent.ProjectileType<PlanteraCrystalLeafRing>()
                                || Main.projectile[i].type == ModContent.ProjectileType<CrystalLeafShot>()))
                            {
                                Main.projectile[i].Kill();
                            }
                        }
                    }
                }

                //explode time * explode repetitions + spread delay * propagations
                const int delayForDicers = 150 * 4 + 25 * 8;

                if (--DicerTimer < -120)
                {
                    DicerTimer = delayForDicers + delayForRingToss + 240;
                    //Counter3 = delayForDicers + 120; //extra compensation for the toss offset

                    npc.netUpdate = true;
                    NetSync(npc);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer);
                        for (int i = 0; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 25f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(2 * (float)Math.PI / 3 * i),
                              ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 1, 8);
                        }
                    }
                }

                if (DicerTimer > delayForDicers || DicerTimer < 0)
                {
                    if (RingTossTimer > 120) //to still respawn the leaf ring if it's missing but disable throwing it
                        RingTossTimer = 120;
                }
                else if (DicerTimer < delayForDicers)
                {
                    RingTossTimer -= 1;

                    if (RingTossTimer % 2 == 0) //make sure plantera can get the timing for its check
                        RingTossTimer--;
                }
                else if (DicerTimer == delayForDicers)
                {
                    RingTossTimer = 121; //activate it immediately as the mines fade
                }

                IsVenomEnraged = npc.HasPlayerTarget && Main.player[npc.target].venom;

                if (--TentacleTimer <= 0)
                {
                    float slowdown = Math.Min(0.9f, -TentacleTimer / 60f);
                    if (FargoSoulsWorld.MasochistModeReal && slowdown > 0.75f)
                        slowdown = 0.75f;
                    npc.position -= npc.velocity * slowdown;

                    if (TentacleTimer == 0)
                    {
                        TentacleAttackAngleOffset = Main.rand.NextFloat(MathHelper.TwoPi);

                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                        npc.netUpdate = true;
                        NetSync(npc);

                        foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] > innerRingDistance)) //my crystal leaves
                        {
                            SoundEngine.PlaySound(SoundID.Grass, n.Center);

                            n.life = 0;
                            n.HitEffect();
                            n.checkDead();
                            n.active = false;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
                        }
                    }

                    const int maxTime = 30;
                    const int interval = 3;
                    float maxDegreeCoverage = 45f; //on either side of the middle, the full coverage of one side is x2 this
                    if (TentacleTimer >= -maxTime && TentacleTimer % interval == 0)
                    {
                        int tentacleSpawnOffset = Math.Abs(TentacleTimer) / interval;
                        for (int i = -tentacleSpawnOffset; i <= tentacleSpawnOffset; i += tentacleSpawnOffset * 2)
                        {
                            float attackAngle = MathHelper.WrapAngle(
                                TentacleAttackAngleOffset
                                + MathHelper.ToRadians(maxDegreeCoverage / (maxTime / interval)) * (i + Main.rand.NextFloat(-0.5f, 0.5f))
                            );

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
                                    ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
                                    ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle + MathHelper.Pi);
                            }

                            if (i == 0)
                                break;
                        }
                    }

                    if (TentacleTimer < -390)
                    {
                        TentacleTimer = 600 + Main.rand.Next(120);

                        if (!FargoSoulsWorld.MasochistModeReal)
                            npc.velocity = Vector2.Zero;

                        npc.netUpdate = true;
                        NetSync(npc);

                        SpawnOuterLeafRing();
                    }
                }
                else
                {
                    npc.position -= npc.velocity * (IsVenomEnraged ? 0.1f : 0.2f);
                }

                //if (FargoSoulsWorld.MasochistModeReal && --TentacleTimerMaso < 0)
                //{
                //    TentacleTimerMaso = 420;
                //    if (Main.netMode != NetmodeID.MultiplayerClient)
                //    {
                //        float angle = npc.DirectionTo(Main.player[npc.target].Center).ToRotation();
                //        for (int i = -1; i <= 1; i++)
                //        {
                //            float offset = MathHelper.ToRadians(6) * i;
                //            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
                //              ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, angle + offset);
                //            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
                //                ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, -angle + offset);
                //        }
                //    }

                //}
            }

            EModeUtils.DropSummon(npc, "PlanterasFruit", NPC.downedPlantBoss, ref DroppedSummon, NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3);

            return result;
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);

            npc.defense = Math.Max(npc.defense, npc.defDefense);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            return IsVenomEnraged ? base.GetAlpha(npc, drawColor) : new Color(255, drawColor.G / 2, drawColor.B / 2);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MagicalBulb>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.JungleFishingCrateHard, 5));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LifeFruit, 3));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.ChlorophyteOre, 200));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadBossHeadSprite(recolor, 11);
            LoadBossHeadSprite(recolor, 12);
            LoadGoreRange(recolor, 378, 391);
            LoadSpecial(recolor, ref TextureAssets.Chain26, ref FargowiltasSouls.TextureBuffer.Chain12, "Chain26");
            LoadSpecial(recolor, ref TextureAssets.Chain27, ref FargowiltasSouls.TextureBuffer.Chain12, "Chain27");
        }
    }

    public class PlanterasHook : PlanteraPart
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PlanterasHook);

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            npc.damage = 0;
            npc.defDamage = 0;

            NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
            if (plantera != null && plantera.life < plantera.lifeMax / 2 && plantera.HasValidTarget)
            {
                if (npc.Distance(Main.player[plantera.target].Center) > 600)
                {
                    Vector2 targetPos = Main.player[plantera.target].Center / 16; //pick a new target pos near player
                    targetPos.X += Main.rand.Next(-25, 26);
                    targetPos.Y += Main.rand.Next(-25, 26);

                    Tile tile = Framing.GetTileSafely((int)targetPos.X, (int)targetPos.Y);
                    npc.localAI[0] = 600; //reset vanilla timer for picking new block
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        npc.netUpdate = true;

                    npc.ai[0] = targetPos.X;
                    npc.ai[1] = targetPos.Y;
                }

                if (npc.Distance(new Vector2(npc.ai[0] * 16 + 8, npc.ai[1] * 16 + 8)) > 32)
                    npc.position += npc.velocity;
            }

            return result;
        }
    }

    public class PlanterasTentacle : PlanteraPart
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PlanterasTentacle);

        public int ChangeDirectionTimer;
        public int RotationDirection;
        public int MaxDistanceFromPlantera;
        public int CanHitTimer;

        public bool DroppedSummon;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(ChangeDirectionTimer);
            binaryWriter.Write7BitEncodedInt(RotationDirection);
            binaryWriter.Write7BitEncodedInt(MaxDistanceFromPlantera);
            binaryWriter.Write7BitEncodedInt(CanHitTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            ChangeDirectionTimer = binaryReader.Read7BitEncodedInt();
            RotationDirection = binaryReader.Read7BitEncodedInt();
            MaxDistanceFromPlantera = binaryReader.Read7BitEncodedInt();
            CanHitTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            MaxDistanceFromPlantera = 200;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return base.CanHitPlayer(npc, target, ref CooldownSlot) && CanHitTimer > 60;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
            if (plantera != null)
            {
                npc.position += plantera.velocity / 3;
                if (npc.Distance(plantera.Center) > MaxDistanceFromPlantera) //snap back in really fast if too far
                {
                    Vector2 vel = plantera.Center - npc.Center;
                    vel += MaxDistanceFromPlantera * plantera.DirectionFrom(npc.Center).RotatedBy(MathHelper.ToRadians(45) * RotationDirection);
                    npc.velocity = Vector2.Lerp(npc.velocity, vel / 15, 0.05f);
                }
            }

            if (++ChangeDirectionTimer > 120)
            {
                ChangeDirectionTimer = Main.rand.Next(30);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    RotationDirection = Main.rand.NextBool() ? -1 : 1;
                    MaxDistanceFromPlantera = 50 + Main.rand.Next(150);
                    npc.netUpdate = true;
                    NetSync(npc);
                }
            }

            ++CanHitTimer;

            return result;
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Spore : PlanteraPart
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Spore);
    }
}
