using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class BrainofCthulhu : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BrainofCthulhu);

        public int ConfusionTimer;
        public int IllusionTimer;
        public int ForceDespawnTimer;

        public bool EnteredPhase2;

        public bool DroppedSummon;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(ConfusionTimer);
            binaryWriter.Write7BitEncodedInt(IllusionTimer);
            bitWriter.WriteBit(EnteredPhase2);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            ConfusionTimer = binaryReader.Read7BitEncodedInt();
            IllusionTimer = binaryReader.Read7BitEncodedInt();
            EnteredPhase2 = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.25);
            npc.scale += 0.25f;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Ichor] = true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return npc.alpha == 0;
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.brainBoss = npc.whoAmI;

            if (WorldSavingSystem.SwarmActive)
                return base.SafePreAI(npc);

            if (Main.LocalPlayer.active && Main.LocalPlayer.Eternity().ShorterDebuffsTimer < 2)
                Main.LocalPlayer.Eternity().ShorterDebuffsTimer = 2;

            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
            {
                if (++ForceDespawnTimer > 60)
                {
                    npc.velocity.Y += 0.75f;
                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;
                }
            }
            else
            {
                ForceDespawnTimer = 0;
            }

            if (npc.alpha > 0 && (npc.ai[0] == 2 || npc.ai[0] == -3) && npc.HasValidTarget) //stay at a minimum distance
            {
                const float safeRange = 360;
                /*Vector2 stayAwayFromHere = Main.player[npc.target].Center + Main.player[npc.target].velocity * 30f;
                if (npc.Distance(stayAwayFromHere) < safeRange)
                    npc.Center = stayAwayFromHere + npc.DirectionFrom(stayAwayFromHere) * safeRange;*/
                Vector2 stayAwayFromHere = Main.player[npc.target].Center;
                if (npc.Distance(stayAwayFromHere) < safeRange)
                    npc.Center = stayAwayFromHere + npc.DirectionFrom(stayAwayFromHere) * safeRange;
            }

            if (EnteredPhase2)
            {
                //debuff cleanse when tp'ing
                if (npc.alpha > 0 && npc.buffType[0] != 0)
                {
                    npc.DelBuff(0);
                }

                void TelegraphConfusion(Vector2 spawn)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 180);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 200);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 8, 220);
                };

                void LaserSpread(Vector2 spawn)
                {
                    if (npc.life > npc.lifeMax / 2 && !WorldSavingSystem.MasochistModeReal)
                        return;

                    if (npc.HasValidTarget && FargoSoulsUtil.HostCheck) //laser spreads from each illusion
                    {
                        int max = WorldSavingSystem.MasochistModeReal ? 7 : 3;
                        int degree = WorldSavingSystem.MasochistModeReal ? 2 : 3;
                        int laserDamage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 3);

                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, new Vector2(0, -4), ModContent.ProjectileType<BrainofConfusion>(), 0, 0, Main.myPlayer);
                        for (int i = -max; i <= max; i++)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, 0.2f * Main.player[npc.target].DirectionFrom(spawn).RotatedBy(MathHelper.ToRadians(degree) * i), ModContent.ProjectileType<DestroyerLaser>(), laserDamage, 0f, Main.myPlayer);
                    }
                };

                int confusionThreshold = WorldSavingSystem.MasochistModeReal ? 240 : 300;
                int confusionThreshold2 = confusionThreshold - 60;

                if (--ConfusionTimer < 0)
                {
                    ConfusionTimer = confusionThreshold;

                    if (!Main.player[npc.target].HasBuff(BuffID.Confused))
                    {
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                        Vector2 offset = npc.Center - Main.player[npc.target].Center;
                        Vector2 spawnPos = Main.player[npc.target].Center;

                        TelegraphConfusion(new Vector2(spawnPos.X + offset.X, spawnPos.Y + offset.Y));
                        TelegraphConfusion(new Vector2(spawnPos.X + offset.X, spawnPos.Y - offset.Y));
                        TelegraphConfusion(new Vector2(spawnPos.X - offset.X, spawnPos.Y + offset.Y));
                        TelegraphConfusion(new Vector2(spawnPos.X - offset.X, spawnPos.Y - offset.Y));
                    }

                    npc.netUpdate = true;
                    NetSync(npc);
                }
                else if (ConfusionTimer == confusionThreshold2)
                {
                    //npc.netUpdate = true; //disabled because might be causing mp issues???
                    //NetSync(npc);

                    if (Main.player[npc.target].HasBuff(BuffID.Confused))
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);
                        TelegraphConfusion(npc.Center);

                        IllusionTimer = 120 + 90;

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int type = ModContent.ProjectileType<BrainIllusionProj>(); //make illusions attack
                            int alpha = (int)(255f * npc.life / npc.lifeMax);

                            void SpawnClone(Vector2 center)
                            {
                                int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)center.X, (int)center.Y, ModContent.NPCType<BrainIllusionAttack>(), npc.whoAmI, npc.whoAmI, alpha);
                                if (n != Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }

                            foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == type && p.ai[0] == npc.whoAmI && p.ai[1] == 0f))
                            {
                                if (p.Distance(Main.player[npc.target].Center) < 1000)
                                {
                                    //p.ai[1] = 1f;
                                    //p.netUpdate = true;

                                    SpawnClone(p.Center);
                                }
                                p.Kill();
                            }

                            Vector2 offset = npc.Center - Main.player[npc.target].Center;
                            Vector2 spawnPos = Main.player[npc.target].Center;

                            SpawnClone(new Vector2(spawnPos.X + offset.X, spawnPos.Y + offset.Y));
                            SpawnClone(new Vector2(spawnPos.X + offset.X, spawnPos.Y - offset.Y));
                            SpawnClone(new Vector2(spawnPos.X - offset.X, spawnPos.Y + offset.Y));
                            SpawnClone(new Vector2(spawnPos.X - offset.X, spawnPos.Y - offset.Y));
                        }
                    }
                    else
                    {
                        Vector2 offset = npc.Center - Main.player[npc.target].Center;
                        Vector2 spawnPos = Main.player[npc.target].Center;

                        LaserSpread(new Vector2(spawnPos.X + offset.X, spawnPos.Y + offset.Y));
                        LaserSpread(new Vector2(spawnPos.X + offset.X, spawnPos.Y - offset.Y));
                        LaserSpread(new Vector2(spawnPos.X - offset.X, spawnPos.Y + offset.Y));
                        LaserSpread(new Vector2(spawnPos.X - offset.X, spawnPos.Y - offset.Y));
                    }

                    if (npc.Distance(Main.LocalPlayer.Center) < 3000 && !Main.LocalPlayer.HasBuff(BuffID.Confused)) //inflict confusion
                    {
                        FargoSoulsUtil.AddDebuffFixedDuration(Main.LocalPlayer, BuffID.Confused, confusionThreshold + 10, false);
                    }
                }

                if (--IllusionTimer < 0) //spawn illusions
                {
                    IllusionTimer = Main.rand.Next(5, 11);
                    if (npc.life > npc.lifeMax / 2)
                        IllusionTimer += 5;
                    if (npc.life < npc.lifeMax / 10)
                        IllusionTimer -= 2;
                    if (WorldSavingSystem.MasochistModeReal)
                        IllusionTimer -= 2;

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 spawn = Main.player[npc.target].Center + Main.rand.NextVector2CircularEdge(1200f, 1200f);
                        Vector2 speed = Main.player[npc.target].Center + Main.player[npc.target].velocity * 45f + Main.rand.NextVector2Circular(-600f, 600f) - spawn;
                        speed = Vector2.Normalize(speed) * Main.rand.NextFloat(12f, 48f);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, speed, ModContent.ProjectileType<BrainIllusionProj>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 4f / 3), 0f, Main.myPlayer, npc.whoAmI);
                    }
                }

                if (IllusionTimer > 60)
                {
                    if (npc.ai[0] == -1f && npc.localAI[1] < 80) //force a tp
                    {
                        npc.localAI[1] = 80f;
                    }
                    if (npc.ai[0] == -3f && npc.ai[3] > 200) //stay invis
                    {
                        npc.dontTakeDamage = true;
                        npc.ai[0] = -3f;
                        npc.ai[3] = 255;
                        npc.alpha = 255;
                        return false;
                    }
                }
            }
            else if (!npc.dontTakeDamage)
            {
                EnteredPhase2 = true;

                if (FargoSoulsUtil.HostCheck) //spawn illusions
                {
                    bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
                    int type = recolor ? ModContent.NPCType<BrainIllusion2>() : ModContent.NPCType<BrainIllusion>();

                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, type, npc.whoAmI, npc.whoAmI, -1, 1);
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, type, npc.whoAmI, npc.whoAmI, 1, -1);
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, type, npc.whoAmI, npc.whoAmI, 1, 1);

                    if (WorldSavingSystem.MasochistModeReal)
                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<BrainClone>(), npc.whoAmI);

                    for (int i = 0; i < Main.maxProjectiles; i++) //clear old golden showers
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GoldenShowerHoming>())
                            Main.projectile[i].Kill();
                    }
                }
            }

            EModeUtils.DropSummon(npc, "GoreySpine", NPC.downedBoss2, ref DroppedSummon);

            npc.defense = 0;
            npc.defDefense = 0;

            return base.SafePreAI(npc);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.life > 0)
                modifiers.FinalDamage *= Math.Max(0.2f, (float)Math.Sqrt((double)npc.life / npc.lifeMax));

            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Poisoned, 120);
                target.AddBuff(BuffID.Darkness, 120);
                target.AddBuff(BuffID.Bleeding, 120);
                target.AddBuff(BuffID.Slow, 120);
                target.AddBuff(BuffID.Weak, 120);
                target.AddBuff(BuffID.BrokenArmor, 120);
            }
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 23);
            LoadGoreRange(recolor, 392, 402);
        }
    }

    public class Creeper : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Creeper);

        public int IchorAttackTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(IchorAttackTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            IchorAttackTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.25);

            IchorAttackTimer = Main.rand.Next(60 * NPC.CountNPCS(NPCID.Creeper)) + Main.rand.Next(61) + 60;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Ichor] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (WorldSavingSystem.SwarmActive)
                return result;

            if (--IchorAttackTimer < 0)
            {
                IchorAttackTimer = 60 * NPC.CountNPCS(NPCID.Creeper) - 30;
                if (IchorAttackTimer >= 60)
                    IchorAttackTimer += Main.rand.Next(-30, 31);

                if (npc.HasPlayerTarget && FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 10f * npc.DirectionFrom(Main.player[npc.target].Center).RotatedByRandom(Math.PI),
                        ModContent.ProjectileType<GoldenShowerHoming>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer, npc.target, -60f);
                }

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (IchorAttackTimer % 60 == 0) //update timer periodically for if player suddenly kills a lot of creepers at once
            {
                IchorAttackTimer = Math.Min(IchorAttackTimer, 60 * NPC.CountNPCS(npc.type));
            }

            return result;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Poisoned, 120);
                target.AddBuff(BuffID.Darkness, 120);
                target.AddBuff(BuffID.Bleeding, 120);
                target.AddBuff(BuffID.Slow, 120);
                target.AddBuff(BuffID.Weak, 120);
                target.AddBuff(BuffID.BrokenArmor, 120);
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.DeathSound != null)
                SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
            npc.active = false;
            return false;
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
