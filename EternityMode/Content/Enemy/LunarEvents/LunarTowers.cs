using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.LunarEvents
{
    public abstract class LunarTowers : EModeNPCBehaviour
    {
        public abstract int ShieldStrength { get; set; }

        protected readonly int DebuffNotToInflict;
        protected readonly int AuraDust;

        protected LunarTowers(int debuffNotToInflict, int auraDust)
        {
            DebuffNotToInflict = debuffNotToInflict;
            AuraDust = auraDust;
        }

        public abstract void ShieldsDownAI(NPC npc);

        public int AttackTimer;
        public int HealCounter;
        public int AuraSync;
        public bool SpawnedDuringLunarEvent;

        public bool spawned;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
            bitWriter.WriteBit(SpawnedDuringLunarEvent);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
            SpawnedDuringLunarEvent = bitReader.ReadBit();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!FargoSoulsWorld.EternityMode)
                return;

            if (!spawned)
            {
                spawned = true;
                SpawnedDuringLunarEvent = NPC.LunarApocalypseIsUp;
                npc.damage += 150;
                npc.defDamage += 150;
                npc.netUpdate = true;
                npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            }

            if (SpawnedDuringLunarEvent && ShieldStrength > NPC.LunarShieldPowerExpert)
                ShieldStrength = NPC.LunarShieldPowerExpert;

            void Aura(int debuff)
            {
                if (DebuffNotToInflict != debuff)
                    EModeGlobalNPC.Aura(npc, 5000, debuff, dustid: AuraDust);
            }

            if (SpawnedDuringLunarEvent)
            {
                Aura(ModContent.BuffType<Atrophied>());
                Aura(ModContent.BuffType<Jammed>());
                Aura(ModContent.BuffType<ReverseManaFlow>());
                Aura(ModContent.BuffType<Antisocial>());

                if (++AuraSync > 60)
                {
                    AuraSync -= 600;
                    NetSync(npc);
                }
            }

            if (npc.dontTakeDamage)
            {
                npc.life = npc.lifeMax;
            }
            else
            {
                ShieldsDownAI(npc);

                if (++HealCounter > 60)
                {
                    HealCounter = 0;
                    npc.TargetClosest(false);
                    if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 4000)
                    {
                        const int heal = 2000;
                        npc.life += heal;
                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            damage = npc.Distance(player.Center) > 2500 ? 0 : damage / 2;
        }
    }

    public class LunarTowerSolar : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerSolar;
            set => NPC.ShieldStrengthTowerSolar = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerSolar);

        public LunarTowerSolar() : base(ModContent.BuffType<Atrophied>(), DustID.SolarFlare) { }

        public override void ShieldsDownAI(NPC npc)
        {
            if (++AttackTimer > 240)
            {
                AttackTimer = 0;

                npc.TargetClosest(false);
                NetSync(npc);

                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const float rotate = (float)Math.PI / 4f;
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Normalize();
                    speed *= 5f;
                    for (int i = -2; i <= 2; i++)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed.RotatedBy(i * rotate), ProjectileID.CultistBossFireBall, 40, 0f, Main.myPlayer);
                }
            }
        }
    }

    public class LunarTowerVortex : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerVortex;
            set => NPC.ShieldStrengthTowerVortex = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerVortex);

        public LunarTowerVortex() : base(ModContent.BuffType<Jammed>(), DustID.Vortex) { }

        public override void ShieldsDownAI(NPC npc)
        {
            if (++AttackTimer > 360) //triggers "shield going down" animation
            {
                AttackTimer = 0;
                npc.ai[3] = 1f;
                npc.netUpdate = true;
                NetSync(npc);
            }

            npc.reflectsProjectiles = npc.ai[3] != 0f;
            if (npc.reflectsProjectiles) //dust
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * npc.height / 2);
                    offset.Y += (float)(Math.Cos(angle) * npc.height / 2);
                    Dust dust = Main.dust[Dust.NewDust(
                        npc.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.Vortex, 0, 0, 100, Color.White, 1f
                        )];
                    dust.noGravity = true;
                }
            }

            //if (++Counter[2] > 240)
            //{
            //    Counter[2] = 0;
            //    npc.TargetClosest(false);
            //    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
            //    {
            //        Vector2 speed = Main.player[npc.target].Center + Main.player[npc.target].velocity * 15f - npc.Center;
            //        speed.Normalize();
            //        speed *= 4f;
            //        Projectile.NewProjectile(npc.Center, speed, ProjectileID.CultistBossLightningOrb, 30, 0f, Main.myPlayer);
            //    }
            //}
        }
    }

    public class LunarTowerNebula : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerNebula;
            set => NPC.ShieldStrengthTowerNebula = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerNebula);

        public LunarTowerNebula() : base(ModContent.BuffType<ReverseManaFlow>(), 58) { }

        public override void ShieldsDownAI(NPC npc)
        {
            if (--AttackTimer < 0)
            {
                AttackTimer = 300;
                npc.TargetClosest(false);
                NetSync(npc);
                for (int i = 0; i < 40; ++i)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 242, 0.0f, 0.0f, 0, new Color(), 1f);
                    Dust dust = Main.dust[d];
                    dust.velocity *= 4f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale += 1.5f;
                }
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) < 3000)
                {
                    int x = (int)Main.player[npc.target].Center.X / 16;
                    int y = (int)Main.player[npc.target].Center.Y / 16;
                    for (int i = 0; i < 100; i++)
                    {
                        int newX = x + Main.rand.Next(10, 31) * (Main.rand.NextBool() ? 1 : -1);
                        int newY = y + Main.rand.Next(-15, 16);
                        Vector2 newPos = new Vector2(newX * 16, newY * 16);
                        if (!Collision.SolidCollision(newPos, npc.width, npc.height))
                        {
                            //npc.Center = newPos;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), newPos, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 10, npc.whoAmI);
                            break;
                        }
                    }
                }
                for (int i = 0; i < 40; ++i)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 242, 0.0f, 0.0f, 0, new Color(), 1f);
                    Dust dust = Main.dust[d];
                    dust.velocity *= 4f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale += 1.5f;
                }
                SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                npc.netUpdate = true;
            }

            //if (++Counter[2] > 60)
            //{
            //    Counter[2] = 0;
            //    npc.TargetClosest(false);
            //    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) < 5000)
            //    {
            //        for (int i = 0; i < 3; i++)
            //        {
            //            Vector2 position = Main.player[npc.target].Center;
            //            position.X += Main.rand.Next(-150, 151);
            //            position.Y -= Main.rand.Next(600, 801);
            //            Vector2 speed = Main.player[npc.target].Center - position;
            //            speed.Normalize();
            //            speed *= 10f;
            //            Projectile.NewProjectile(position, speed, ProjectileID.NebulaLaser, 40, 0f, Main.myPlayer);
            //        }
            //    }
            //}
        }
    }

    public class LunarTowerStardust : LunarTowers
    {
        public override int ShieldStrength
        {
            get => NPC.ShieldStrengthTowerStardust;
            set => NPC.ShieldStrengthTowerStardust = value;
        }

        public override NPCMatcher CreateMatcher() =>
            new NPCMatcher().MatchType(NPCID.LunarTowerStardust);

        public LunarTowerStardust() : base(ModContent.BuffType<Antisocial>(), 20) { }

        public override void ShieldsDownAI(NPC npc)
        {
            const int attackTime = 420;

            //if (AttackTimer > attackTime / 2)
            //{
            //    float scale = 4f * (attackTime - attackTime / 2) / (attackTime / 2);
            //    int d = Dust.NewDust(npc.Center, 0, 0, AuraDust, Scale: scale);
            //    Main.dust[d].velocity *= 12f;
            //    Main.dust[d].noGravity = true;
            //}

            if (++AttackTimer > attackTime)
            {
                AttackTimer = 0;

                npc.TargetClosest(false);
                NetSync(npc);

                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const float rotate = (float)Math.PI / 12f;
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Normalize();
                    speed *= 8f;
                    for (int i = 0; i < 24; i++)
                    {
                        Vector2 vel = speed.RotatedBy(rotate * i);
                        int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.AncientLight, 0,
                            0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f, vel.X, vel.Y);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].velocity = vel;
                            Main.npc[n].netUpdate = true;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }
            }
        }
    }
}
