using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
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

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public abstract class BiomeMimics : EModeNPCBehaviour
    {
        public int AttackCycleTimer;
        public int IndividualAttackTimer;

        public bool DoStompAttack;
        public bool CanDoAttack;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackCycleTimer);
            binaryWriter.Write7BitEncodedInt(IndividualAttackTimer);
            bitWriter.WriteBit(DoStompAttack);
            bitWriter.WriteBit(CanDoAttack);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackCycleTimer = binaryReader.Read7BitEncodedInt();
            IndividualAttackTimer = binaryReader.Read7BitEncodedInt();
            DoStompAttack = bitReader.ReadBit();
            CanDoAttack = bitReader.ReadBit();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (DoStompAttack)
            {
                if (npc.velocity.Y == 0f) //spawn smash
                {
                    DoStompAttack = false;
                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Bottom + new Vector2(i * 16 * 4, -48), Vector2.Zero, ModContent.ProjectileType<BigMimicExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                        }
                    }
                }
            }
            else if (npc.velocity.Y > 0 && npc.noTileCollide) //mega jump
            {
                DoStompAttack = true;
            }

            if (!npc.dontTakeDamage && npc.ai[0] != 0)
            {
                AttackCycleTimer++;
                IndividualAttackTimer++;

                if (AttackCycleTimer == 240 && !CanDoAttack && Main.netMode != NetmodeID.MultiplayerClient) //telegraph
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Souls.IronParry>(), 0, 0f, Main.myPlayer);
                    NetSync(npc);
                }
            }
            if (AttackCycleTimer > 300)
            {
                AttackCycleTimer = 0;
                CanDoAttack = !CanDoAttack;
            }
        }
    }

    public class CorruptMimic : BiomeMimics
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BigMimicCorruption);

        public override void AI(NPC npc)
        {
            if (CanDoAttack && IndividualAttackTimer > 90)
            {
                IndividualAttackTimer = 0;
                if (npc.HasValidTarget)
                {
                    float distance = 16 * Main.rand.NextFloat(5, 35);
                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 spawnPos = Main.player[npc.target].Bottom + new Vector2(i * distance, -100 - 8);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.UnitY, ModContent.ProjectileType<ClingerFlame>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                    }
                }
            }

            base.AI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CorruptFishingCrateHard));
        }
    }

    public class CrimsonMimic : BiomeMimics
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BigMimicCrimson);

        public override void AI(NPC npc)
        {
            if (CanDoAttack)
            {
                npc.position -= npc.velocity * (npc.HasValidTarget && Main.player[npc.target].ZoneRockLayerHeight ? 0.8f : 0.5f);

                if (IndividualAttackTimer > 10)
                {
                    IndividualAttackTimer = 0;
                    if (npc.HasValidTarget)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 target = npc.Center;
                            target.X += Math.Sign(npc.direction) * 600f * (AttackCycleTimer + 60) / 360f; //gradually targets further and further
                            target.X += Main.rand.NextFloat(-100, 100);
                            target.Y += Main.rand.NextFloat(-450, 450);
                            const float gravity = 0.5f;
                            float time = 60f;
                            Vector2 distance = target - npc.Center;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance, ModContent.ProjectileType<GoldenShowerWOF>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, time);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = Main.rand.Next(60, 75) * 3;
                            }
                        }
                    }
                }
            }

            base.AI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Ichor, 180);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CrimsonFishingCrateHard));
        }
    }

    public class HallowMimic : BiomeMimics
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BigMimicHallow);

        public override void AI(NPC npc)
        {
            if (CanDoAttack && npc.HasValidTarget)
            {
                npc.position -= npc.velocity / 4f;

                if (IndividualAttackTimer == 10)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 9f * npc.DirectionTo(Main.player[npc.target].Center), ProjectileID.JestersArrow, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                }

                if (IndividualAttackTimer % 10 == 0 && !Main.player[npc.target].ZoneRockLayerHeight)
                {
                    SoundEngine.PlaySound(SoundID.Item5, npc.Center);

                    Vector2 spawn = new Vector2(npc.Center.X + Main.rand.NextFloat(-100, 100), Main.player[npc.target].Center.Y - Main.rand.Next(600, 801));
                    Vector2 speed = 10f * Vector2.Normalize(Main.player[npc.target].Center + Main.rand.NextVector2Square(-100, 100) - spawn);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, speed, ProjectileID.JestersArrow, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                    }

                    for (int i = 0; i < 40; i++)
                    {
                        int type = Main.rand.Next(new int[] { 15, 57, 58 });
                        int d = Dust.NewDust(npc.Center, 0, 0, type, speed.X / 2f, -speed.Y / 2f, 100, default(Color), 1.2f);
                        Main.dust[d].velocity *= 2f;
                        Main.dust[d].noGravity = Main.rand.NextBool();
                    }
                }

                if (IndividualAttackTimer > 30)
                    IndividualAttackTimer = 0;
            }

            base.AI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Confused, 180);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.HallowedFishingCrateHard));
        }
    }

    public class JungleMimic : BiomeMimics
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BigMimicJungle);

        public override void AI(NPC npc)
        {
            if (DoStompAttack)
            {
                if (npc.velocity.Y == 0f) //landed from ANY jump
                {
                    //DoStompAttack = false; //this will be handled by base ai anyway

                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                              Main.rand.Next(-30, 31) * .1f, Main.rand.Next(-40, -15) * .1f, ModContent.ProjectileType<FakeHeart>(), 20, 0f, Main.myPlayer);
                    }

                    SoundEngine.PlaySound(SoundID.Item14, npc.Center);

                    for (int i = 0; i < 30; i++)
                    {
                        int dust = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    float scaleFactor9 = 0.5f;
                    for (int j = 0; j < 4; j++)
                    {
                        int gore = Gore.NewGore(npc.GetSource_FromThis(), npc.Center, default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[gore].velocity *= scaleFactor9;
                        Main.gore[gore].velocity.X += 1f;
                        Main.gore[gore].velocity.Y += 1f;
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                DoStompAttack = true;
            }

            if (CanDoAttack && IndividualAttackTimer > 10)
            {
                IndividualAttackTimer = 0;
                if (npc.HasValidTarget)
                {
                    SoundEngine.PlaySound(SoundID.Grass, npc.Center);
                    float speed = Main.player[npc.target].ZoneRockLayerHeight ? 9f : 14f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = speed * npc.DirectionTo(Main.player[npc.target].Center).RotatedByRandom(MathHelper.ToRadians(5));
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<JungleTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, npc.whoAmI);
                    }
                }
            }

            base.AI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Purified>(), 360);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.JungleFishingCrateHard));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<TribalCharm>(), 5));
        }
    }
}
