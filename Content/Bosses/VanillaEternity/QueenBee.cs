using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Content.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Items.Placables;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using Terraria.Localization;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
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

            if (WorldSavingSystem.SwarmActive)
                return result;


            if (npc.HasPlayerTarget && npc.HasValidTarget && (!Main.player[npc.target].ZoneJungle
                || Main.player[npc.target].position.Y < Main.worldSurface * 16))
            {
                if (++EnrageFactor == 300)
                {
                    FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.NPCs.EMode.QueenBeeEnrage", new Color(175, 75, 255));
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

                Vector2 vector72 = new(npc.position.X + npc.width / 2 + Main.rand.Next(20) * npc.direction, npc.position.Y + npc.height * 0.8f);

                int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), vector72, ModContent.NPCType<RoyalSubject>(),
                    velocity: new Vector2(Main.rand.Next(-200, 201) * 0.1f, Main.rand.Next(-200, 201) * 0.1f));
                if (n != Main.maxNPCs)
                    Main.npc[n].localAI[0] = 60f;

                FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue($"Mods.{Mod.Name}.NPCs.RoyalSubject.DisplayName"));

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (!SpawnedRoyalSubjectWave2 && npc.life < npc.lifeMax / 3 && npc.HasPlayerTarget)
            {
                SpawnedRoyalSubjectWave2 = true;

                if (WorldSavingSystem.MasochistModeReal)
                    SpawnedRoyalSubjectWave1 = false; //do this again

                Vector2 vector72 = new(npc.position.X + npc.width / 2 + Main.rand.Next(20) * npc.direction, npc.position.Y + npc.height * 0.8f);

                int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), vector72, ModContent.NPCType<RoyalSubject>(),
                    velocity: new Vector2(Main.rand.Next(-200, 201) * 0.1f, Main.rand.Next(-200, 201) * 0.1f));
                if (n != Main.maxNPCs)
                    Main.npc[n].localAI[0] = 60f;

                FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue($"Mods.{Mod.Name}.NPCs.RoyalSubject.DisplayName"));

                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<RoyalSubject>()); //so that both dont stack for being spawned from qb

                npc.netUpdate = true;
                NetSync(npc);
            }


            if (!InPhase2 && npc.life < npc.lifeMax / 2) //enable new attack and roar below 50%
            {
                InPhase2 = true;
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                if (WorldSavingSystem.MasochistModeReal)
                    SpawnedRoyalSubjectWave1 = false; //do this again

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
            {
                npc.HitSound = SoundID.NPCHit4;
                npc.color = new Color(127, 127, 127);

                int dustId = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 2f);
                Main.dust[dustId].noGravity = true;
                int dustId3 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Stone, 0f, 0f, 100, default, 2f);
                Main.dust[dustId3].noGravity = true;

                if (!Main.getGoodWorld)
                {
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
                        if (npc.ai[1] > 1 && !WorldSavingSystem.MasochistModeReal)
                            npc.ai[1] -= 0.5f; //slower stingers
                    }
                }
            }
            else
            {
                npc.HitSound = SoundID.NPCHit1;
                npc.color = default;

                if (InPhase2 && HiveThrowTimer % 2 == 0)
                    HiveThrowTimer++; //throw hives faster when no royal subjects alive
            }

            if (WorldSavingSystem.MasochistModeReal)
            {
                HiveThrowTimer++;

                if (ForgorDeathrayTimer > 0 && --ForgorDeathrayTimer % 10 == 0 && npc.HasValidTarget && FargoSoulsUtil.HostCheck)
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
                    distance.X /= time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    if (FargoSoulsUtil.HostCheck)
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

                            if (FargoSoulsUtil.HostCheck)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);

                            if (npc.HasValidTarget)
                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, Main.player[npc.target].Center); //eoc roar

                            if (WorldSavingSystem.MasochistModeReal)
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

                        if (BeeSwarmTimer % 2 == 0 && FargoSoulsUtil.HostCheck)
                        {
                            const float rotation = 0.025f;
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + new Vector2(3 * npc.direction, 15), i * Main.rand.NextFloat(9f, 18f) * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-45, 45))),
                                    ModContent.ProjectileType<Bee>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, WorldSavingSystem.MasochistModeReal ? 4f / 3 : 1), 0f, Main.myPlayer, npc.target, Main.rand.NextBool() ? -rotation : rotation);
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

                int threshold = WorldSavingSystem.MasochistModeReal ? 90 : 120;

                if (++StingerRingTimer > threshold * 3)
                    StingerRingTimer = 0;

                if (StingerRingTimer % threshold == 0)
                {
                    float speed = WorldSavingSystem.MasochistModeReal ? 6 : 5;

                    if (FargoSoulsUtil.HostCheck)
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

                        if (WorldSavingSystem.MasochistModeReal)
                            npc.ai[2] = 0;

                        ForgorDeathrayTimer = 95;
                        if (Main.getGoodWorld)
                            ForgorDeathrayTimer += 60;
                    }

                    npc.velocity *= 0.95f;
                    npc.ai[2]++;

                    return false;
                }
            }

            if (WorldSavingSystem.MasochistModeReal)
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

        public override void SafePostAI(NPC npc)
        {
            base.SafePostAI(npc);

            if (!npc.HasValidTarget || npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) > 3000)
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

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
            target.AddBuff(ModContent.BuffType<SwarmingBuff>(), 600);

            if (npc.ai[0] == 0) //in dash mode
            {
                target.AddBuff(BuffID.BrokenArmor, 60 * 5);
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!WorldSavingSystem.SwarmActive && NPC.AnyNPCs(ModContent.NPCType<RoyalSubject>()))
                modifiers.FinalDamage /= 2;

            base.ModifyIncomingHit(npc, ref modifiers);
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
