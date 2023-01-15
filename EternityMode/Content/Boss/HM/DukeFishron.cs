using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Consumables;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
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

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class DukeFishron : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DukeFishron);

        public int GeneralTimer;
        public int P3Timer;
        public int EXTornadoTimer;

        public bool RemovedInvincibility;
        public bool TakeNoDamageOnHit;
        public bool IsEX;

        public bool SpectralFishronRandom; //only for spawning projs (server-side only), no mp sync needed
        public bool DroppedSummon;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(GeneralTimer);
            binaryWriter.Write7BitEncodedInt(P3Timer);
            binaryWriter.Write7BitEncodedInt(EXTornadoTimer);
            bitWriter.WriteBit(RemovedInvincibility);
            bitWriter.WriteBit(TakeNoDamageOnHit);
            bitWriter.WriteBit(IsEX);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            GeneralTimer = binaryReader.Read7BitEncodedInt();
            P3Timer = binaryReader.Read7BitEncodedInt();
            EXTornadoTimer = binaryReader.Read7BitEncodedInt();
            RemovedInvincibility = bitReader.ReadBit();
            TakeNoDamageOnHit = bitReader.ReadBit();
            IsEX = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (EModeGlobalNPC.spawnFishronEX)
            {
                IsEX = true;
                npc.GivenName = "Duke Fishron EX";
                npc.damage = (int)(npc.damage * 3);// 1.5);
                npc.defense *= 30;
            }
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;

            if (IsEX)
            {
                npc.buffImmune[ModContent.BuffType<GodEater>()] = true;
                npc.buffImmune[ModContent.BuffType<Sadism>()] = true;
                npc.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
                npc.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            }
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            EModeGlobalNPC.fishBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            void SpawnRazorbladeRing(int max, float speed, int damage, float rotationModifier, bool reduceTimeleft = false)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;
                float rotation = 2f * (float)Math.PI / max;
                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                vel.Normalize();
                vel *= speed;
                int type = ModContent.ProjectileType<RazorbladeTyphoon>();
                for (int i = 0; i < max; i++)
                {
                    vel = vel.RotatedBy(rotation);
                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * npc.spriteDirection, speed);
                    if (reduceTimeleft && p < 1000)
                        Main.projectile[p].timeLeft /= 2;
                }
                SoundEngine.PlaySound(SoundID.Item84, npc.Center);
            }

            void EnrageDust()
            {
                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    int d;
                    if (npc.velocity.Length() > 10)
                    {
                        Vector2 vector2_1 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + npc.Center;
                        Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                        d = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 88, vector2_2.X * 2f, vector2_2.Y * 2f, 0, default, 1.7f);
                    }
                    else
                    {
                        d = Dust.NewDust(npc.position, npc.width, npc.height, 88, npc.velocity.X * 2f, npc.velocity.Y * 2f, 0, default, 1.7f);
                    }
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity /= 4f;
                    Main.dust[d].velocity -= npc.velocity;
                }
            }


            #region duke ex ai

            if (IsEX) //fishron EX
            {
                npc.GetGlobalNPC<FargoSoulsGlobalNPC>().MutantNibble = false;
                npc.GetGlobalNPC<FargoSoulsGlobalNPC>().LifePrevious = int.MaxValue; //cant stop the healing
                while (npc.buffType[0] != 0)
                    npc.DelBuff(0);

                if (npc.Distance(Main.LocalPlayer.Center) < 3000f)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<OceanicSeal>(), 2);
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<Buffs.Boss.MutantPresence>(), 2); //LUL
                }
                EModeGlobalNPC.fishBossEX = npc.whoAmI;
                npc.position += npc.velocity * 0.5f;
                switch ((int)npc.ai[0])
                {
                    case -1: //just spawned
                        if (npc.ai[2] == 2 && Main.netMode != NetmodeID.MultiplayerClient) //create spell circle
                        {
                            int ritual1 = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                ModContent.ProjectileType<FishronRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax, npc.whoAmI);
                            if (ritual1 == Main.maxProjectiles) //failed to spawn projectile, abort spawn
                                npc.active = false;
                            SoundEngine.PlaySound(SoundID.Item84, npc.Center);
                        }
                        TakeNoDamageOnHit = true;
                        break;

                    case 0: //phase 1
                        if (!RemovedInvincibility)
                            npc.dontTakeDamage = false;
                        TakeNoDamageOnHit = false;
                        npc.ai[2]++;
                        break;

                    case 1: //p1 dash
                        GeneralTimer++;
                        if (GeneralTimer > 5)
                        {
                            GeneralTimer = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 spawnPos = new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height));
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), spawnPos, NPCID.DetonatingBubble);

                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                    ModContent.NPCType<DetonatingBubbleEX>(),
                                    velocity: npc.DirectionTo(Main.player[npc.target].Center));
                            }
                        }
                        break;

                    case 2: //p1 bubbles
                        if (npc.ai[2] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        break;

                    case 3: //p1 drop nados
                        if (npc.ai[2] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 32;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                    ModContent.NPCType<DetonatingBubbleEX>(),
                                    velocity: Vector2.Normalize(Vector2.UnitY.RotatedBy(rotation * i)));
                            }

                            SpawnRazorbladeRing(18, 10f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 6), 1f);
                        }
                        break;

                    case 4: //phase 2 transition
                        RemovedInvincibility = false;
                        TakeNoDamageOnHit = true;
                        if (npc.ai[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax / 4, npc.whoAmI);
                        if (npc.ai[2] >= 114)
                        {
                            GeneralTimer++;
                            if (GeneralTimer > 6) //display healing effect
                            {
                                GeneralTimer = 0;
                                int heal = (int)(npc.lifeMax * Main.rand.NextFloat(0.1f, 0.12f));
                                npc.life += heal;
                                int max = npc.ai[0] == 9 /*&& !Fargowiltas.Instance.MasomodeEXLoaded*/ ? npc.lifeMax / 2 : npc.lifeMax;
                                if (npc.life > max)
                                    npc.life = max;
                                CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                            }
                        }
                        break;

                    case 5: //phase 2
                        if (!RemovedInvincibility)
                            npc.dontTakeDamage = false;
                        TakeNoDamageOnHit = false;
                        npc.ai[2]++;
                        break;

                    case 6: //p2 dash
                        goto case 1;

                    case 7: //p2 spin & bubbles
                        npc.position -= npc.velocity * 0.5f;
                        GeneralTimer++;
                        if (GeneralTimer > 1)
                        {
                            //Counter0 = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                    ModContent.NPCType<DetonatingBubbleEX>(),
                                    velocity: Vector2.Normalize(npc.velocity.RotatedBy(Math.PI / 2)));
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                    ModContent.NPCType<DetonatingBubbleEX>(),
                                    velocity: Vector2.Normalize(npc.velocity.RotatedBy(-Math.PI / 2)));
                            }
                        }
                        break;

                    case 8: //p2 cthulhunado
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 60)
                        {
                            Vector2 spawnPos = Vector2.UnitX * npc.direction;
                            spawnPos = spawnPos.RotatedBy(npc.rotation);
                            spawnPos *= npc.width + 20f;
                            spawnPos /= 2f;
                            spawnPos += npc.Center;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos.X, spawnPos.Y, npc.direction * 2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos.X, spawnPos.Y, npc.direction * -2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos.X, spawnPos.Y, 0f, 2f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                            SpawnRazorbladeRing(12, 12.5f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 6), 0.75f);
                            SpawnRazorbladeRing(12, 10f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 6), -2f);
                        }
                        break;

                    case 9: //phase 3 transition
                        if (npc.ai[2] == 1f)
                        {
                            for (int i = 0; i < npc.buffImmune.Length; i++)
                                npc.buffImmune[i] = true;
                            while (npc.buffTime[0] != 0)
                                npc.DelBuff(0);
                            npc.defDamage = (int)(npc.defDamage * 1.2f);
                        }
                        goto case 4;

                    case 10: //phase 3
                        TakeNoDamageOnHit = false;
                        //if (Timer >= 60 + (int)(540.0 * npc.life / npc.lifeMax)) //yes that needs to be a double
                        /*Counter2++;
                        if (Counter2 >= 900)
                        {
                            Counter2 = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn cthulhunado
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        }*/
                        break;

                    case 11: //p3 dash
                        if (GeneralTimer > 2)
                            GeneralTimer = 2;
                        if (GeneralTimer == 2)
                        {
                            //Counter0 = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                    ModContent.NPCType<DetonatingBubbleEX>(),
                                    velocity: Vector2.Normalize(npc.velocity.RotatedBy(Math.PI / 2)));
                                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                    ModContent.NPCType<DetonatingBubbleEX>(),
                                    velocity: Vector2.Normalize(npc.velocity.RotatedBy(-Math.PI / 2)));
                            }
                        }
                        goto case 10;

                    case 12: //p3 *teleports behind you*
                        if (npc.ai[2] == 15f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SpawnRazorbladeRing(5, 9f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 6), 1f, true);
                                SpawnRazorbladeRing(5, 9f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 6), -0.5f, true);
                            }
                        }
                        else if (npc.ai[2] == 16f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 spawnPos = Vector2.UnitX * npc.direction; //GODLUL
                                spawnPos = spawnPos.RotatedBy(npc.rotation);
                                spawnPos *= npc.width + 20f;
                                spawnPos /= 2f;
                                spawnPos += npc.Center;
                                Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos.X, spawnPos.Y, npc.direction * 2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos.X, spawnPos.Y, npc.direction * -2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                                const int max = 24;
                                float rotation = 2f * (float)Math.PI / max;
                                for (int i = 0; i < max; i++)
                                {
                                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                        ModContent.NPCType<DetonatingBubbleEX>(),
                                        velocity: Vector2.Normalize(npc.velocity.RotatedBy(rotation * i)));
                                }
                            }
                        }
                        goto case 10;

                    default:
                        break;
                }
            }

            #endregion


            npc.position += npc.velocity * 0.25f; //fishron regular
            const int spectralFishronDelay = 3;
            switch ((int)npc.ai[0])
            {
                case -1: //just spawned
                    /*if (npc.ai[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient) //create spell circle
                    {
                        int p2 = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
                            ModContent.ProjectileType<FishronRitual2>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
                        if (p2 == 1000) //failed to spawn projectile, abort spawn
                            npc.active = false;
                    }*/
                    if (!IsEX)
                        npc.dontTakeDamage = true;
                    break;

                case 0: //phase 1
                    if (!RemovedInvincibility)
                        npc.dontTakeDamage = false;
                    if (!Main.player[npc.target].ZoneBeach)
                        npc.ai[2]++;
                    break;

                case 1: //p1 dash
                    if (++GeneralTimer > 5)
                    {
                        GeneralTimer = 0;

                        if (FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height));
                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), spawnPos, NPCID.DetonatingBubble);
                        }
                    }
                    break;

                case 2: //p1 bubbles
                    if (npc.ai[2] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool random = Main.rand.NextBool(); //fan above or to sides
                        for (int j = -1; j <= 1; j++) //to both sides of player
                        {
                            if (j == 0)
                                continue;

                            Vector2 offset = random ? Vector2.UnitY * -450f * j : Vector2.UnitX * 600f * j;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, offset.X, offset.Y);
                        }
                    }
                    break;

                case 3: //p1 drop nados
                    if (npc.ai[2] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SpawnRazorbladeRing(12, 10f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f);
                    }
                    break;

                case 4: //phase 2 transition
                    if (IsEX)
                        break;
                    npc.dontTakeDamage = true;
                    RemovedInvincibility = false;
                    if (npc.ai[2] == 120)
                    {
                        int heal = npc.lifeMax - npc.life;
                        npc.life = npc.lifeMax;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                    }
                    break;

                case 5: //phase 2
                    if (!RemovedInvincibility)
                        npc.dontTakeDamage = false;
                    if (!Main.player[npc.target].ZoneBeach)
                        npc.ai[2]++;
                    break;

                case 6: //p2 dash
                    goto case 1;

                case 7: //p2 spin & bubbles
                    npc.position -= npc.velocity * 0.25f;

                    if (++GeneralTimer > 1)
                    {
                        GeneralTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2),
                                ModContent.ProjectileType<RazorbladeTyphoon2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, .03f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 0.014035f * Vector2.Normalize(npc.velocity).RotatedBy(-Math.PI / 2),
                                ModContent.ProjectileType<RazorbladeTyphoon2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, .08f);

                            //if (/*Fargowiltas.Instance.MasomodeEXLoaded ||*/ FargoSoulsWorld.MasochistModeReal) //lol
                            //{
                            //    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                            //        ModContent.NPCType<NPCs.EternityMode.DetonatingBubble>(),
                            //        velocity: Vector2.Normalize(npc.velocity.RotatedBy(Math.PI / 2)) * -npc.spriteDirection);
                            //}
                        }
                    }
                    break;

                case 8: //p2 cthulhunado
                    {
                        const int delayForTornadoSpawn = 60;

                        if (npc.ai[2] == 0f)
                        {
                            SpectralFishronRandom = Main.rand.NextBool(); //fan above or to sides
                        }
                        if (npc.ai[2] >= delayForTornadoSpawn && npc.ai[2] % spectralFishronDelay == 0 && npc.ai[2] <= spectralFishronDelay * 2 + delayForTornadoSpawn)
                        {
                            for (int j = -1; j <= 1; j += 2) //to both sides of player
                            {
                                int max = (int)(npc.ai[2] - delayForTornadoSpawn) / spectralFishronDelay;
                                for (int i = -max; i <= max; i++) //fan of fishron
                                {
                                    if (Math.Abs(i) != max) //only spawn the outmost ones
                                        continue;
                                    Vector2 offset = SpectralFishronRandom ? Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / 3 / 3 * i) * -500f * j : Vector2.UnitX.RotatedBy(MathHelper.PiOver2 / 3 / 3 * i) * 500f * j;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, offset.X, offset.Y);
                                }
                            }
                        }

                        if (npc.ai[2] == delayForTornadoSpawn && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (FargoSoulsWorld.MasochistModeReal)
                            {
                                Vector2 spawnPos = Vector2.UnitX * npc.direction;
                                spawnPos = spawnPos.RotatedBy(npc.rotation);
                                spawnPos *= npc.width + 20f;
                                spawnPos /= 2f;
                                spawnPos += npc.Center;
                                Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos.X, spawnPos.Y, 0f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                                SpawnRazorbladeRing(12, 12.5f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0.75f);
                                SpawnRazorbladeRing(12, 10f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 2f * npc.direction);
                            }
                        }
                    }
                    break;

                case 9: //phase 3 transition
                    if (IsEX)
                        break;
                    npc.dontTakeDamage = true;
                    //npc.defDefense = 0;
                    //npc.defense = 0;
                    RemovedInvincibility = false;

                    if (npc.ai[2] == 90) //first purge the bolts
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<RazorbladeTyphoon2>();
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.SharknadoBolt || Main.projectile[i].type == type))
                                {
                                    Main.projectile[i].Kill();
                                }
                            }
                        }
                    }

                    if (npc.ai[2] == 120)
                    {
                        int max = FargoSoulsWorld.MasochistModeReal ? npc.lifeMax / 2 : npc.lifeMax / 3;
                        int heal = max - npc.life;
                        npc.life = max;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);

                        if (Main.netMode != NetmodeID.MultiplayerClient) //purge nados
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && (Main.projectile[i].type == ProjectileID.Sharknado || Main.projectile[i].type == ProjectileID.Cthulunado))
                                {
                                    Main.projectile[i].Kill();
                                }
                            }

                            for (int i = 0; i < Main.maxNPCs; i++) //purge sharks
                            {
                                if (Main.npc[i].active && (Main.npc[i].type == NPCID.Sharkron || Main.npc[i].type == NPCID.Sharkron2))
                                {
                                    Main.npc[i].life = 0;
                                    Main.npc[i].HitEffect();
                                    Main.npc[i].active = false;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                                }
                            }
                        }
                    }
                    break;

                case 10: //phase 3
                    if (!Main.player[npc.target].ZoneBeach || (npc.ai[3] > 5 && npc.ai[3] < 8))
                    {
                        npc.position += npc.velocity;
                        npc.ai[2]++;
                        EnrageDust();
                    }

                    if (npc.ai[3] == 1) //after 1 dash, before teleporting
                    {
                        if (P3Timer == 0)
                        {
                            SpectralFishronRandom = Main.rand.NextBool();

                            //if (FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                            //    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        }

                        if (++P3Timer < 150)
                        {
                            void Checks(int delay)
                            {
                                int max = FargoSoulsWorld.MasochistModeReal ? 5 : 4;
                                int P3TimerOffset = P3Timer - 30;
                                if (P3TimerOffset >= delay && P3TimerOffset < spectralFishronDelay * max + delay && P3TimerOffset % spectralFishronDelay == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 offset = 450 * -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / max * (P3TimerOffset / spectralFishronDelay + Main.rand.NextFloat(0.5f)));
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, offset.X, offset.Y);
                                }
                            }

                            npc.ai[2] = 0; //stay in this ai mode for a bit
                            npc.position.Y -= npc.velocity.Y * 0.5f;

                            Checks(0);
                            //Checks(90);
                        }
                    }
                    else if (npc.ai[3] == 5)
                    {
                        if (npc.ai[2] == 0)
                            SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                        npc.ai[2] -= 0.5f;
                        npc.velocity *= 0.5f;
                        EnrageDust();
                    }

                    /*if (npc.ai[0] == 10)
                    {
                        if (++Counter1 == 15)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const float delay = 15;
                                Vector2 baseVel = 100f / delay * npc.DirectionTo(Main.player[npc.target].Center);

                                const int max = 10;
                                for (int i = 0; i < max; i++)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, baseVel.RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<FishronBubble>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, delay);
                                }
                            }
                        }
                    }*/
                    break;

                case 11: //p3 dash
                    if (!Main.player[npc.target].ZoneBeach || npc.ai[3] >= 5)
                    {
                        if (npc.ai[2] == 0 && !Main.dedServ)
                            SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Monster70"), npc.Center);

                        if (Main.player[npc.target].ZoneBeach)
                        {
                            npc.position += npc.velocity * 0.5f;
                        }
                        else //enrage
                        {
                            npc.position += npc.velocity;
                            npc.ai[2]++;

                            int playerTileX = (int)Main.player[npc.target].Center.X / 16;
                            bool customBeach = playerTileX < 500 || playerTileX > Main.maxTilesX - 500;
                            if (!customBeach)
                                EXTornadoTimer -= 2; //enable EX tornado
                        }
                        EnrageDust();
                    }

                    P3Timer = 0;
                    if (--GeneralTimer < 0)
                    {
                        GeneralTimer = 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.ai[3] == 2 || npc.ai[3] == 3) //spawn destructible bubbles on 2-dash
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    for (int j = 1; j <= 2; j++)
                                    {
                                        FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                            ModContent.NPCType<NPCs.EternityMode.DetonatingBubble>(),
                                            velocity: Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i) * j * 0.5f);
                                    }
                                }
                            }

                            if (!Main.player[npc.target].ZoneBeach) //enraged, spawn bubbles
                            {
                                float range = MathHelper.ToRadians(Main.rand.NextFloat(1f, 15f));
                                for (int i = -1; i <= 1; i++)
                                {
                                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 8f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(range * i),
                                        ModContent.ProjectileType<FishronBubble>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 90;
                                }

                                for (int i = -1; i <= 1; i += 2)
                                {
                                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 8f * Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i),
                                        ModContent.ProjectileType<FishronBubble>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 90;
                                }
                            }
                            else if (FargoSoulsWorld.MasochistModeReal)
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromThis(), npc.Center,
                                        ModContent.NPCType<NPCs.EternityMode.DetonatingBubble>(),
                                        velocity: 1.5f * Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i));
                                }
                            }
                        }
                    }
                    break;

                case 12: //p3 *teleports behind you*
                    if (!Main.player[npc.target].ZoneBeach || (npc.ai[3] > 5 && npc.ai[3] < 8))
                    {
                        if (!Main.player[npc.target].ZoneBeach)
                            npc.position += npc.velocity;
                        npc.ai[2]++;
                        EnrageDust();
                    }

                    GeneralTimer = 0;
                    if (npc.ai[2] == 15f)
                    {
                        SpawnRazorbladeRing(6, 8f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), -0.75f);
                    }
                    else if (npc.ai[2] == 16f)
                    {
                        const int max = 5;
                        for (int j = -max; j <= max; j++)
                        {
                            Vector2 vel = npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(MathHelper.PiOver2 / max * j);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<FishronBubble>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
                        }
                    }
                    break;

                default:
                    break;
            }

            if (FargoSoulsWorld.MasochistModeReal || EModeGlobalNPC.fishBossEX == npc.whoAmI)// && npc.ai[0] >= 10 || (npc.ai[0] == 9 && npc.ai[2] > 120)) //in phase 3, do this check in all stages
            {
                EXTornadoTimer--;
            }

            if (EXTornadoTimer < 0)
            {
                EXTornadoTimer = 10 * 60;

                SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);

                for (int i = -1; i <= 1; i += 2)
                {
                    int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                    int tilePosY = (int)Main.player[npc.target].Center.Y / 16;
                    tilePosX += 75 * i;

                    if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                        continue;

                    //first move up through solid tiles
                    while (Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolid[Main.tile[tilePosX, tilePosY].TileType])
                    {
                        tilePosY--;
                        if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                            break;
                    }

                    tilePosY--;

                    //then move down through air until solid tile/platform reached
                    int tilesMovedDown = 0;
                    while (!(Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolidTop[Main.tile[tilePosX, tilePosY].TileType]))
                    {
                        tilePosY++;
                        if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                            break;
                        if (++tilesMovedDown > 32)
                        {
                            tilePosY -= 28; //give up, reset
                            break;
                        }
                    }

                    tilePosY--;

                    Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.UnitX * -i * 6f, ProjectileID.Cthulunado, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 10, 25);
                }
            }

            EModeUtils.DropSummon(npc, "TruffleWorm2", NPC.downedFishron, ref DroppedSummon);

            return result;
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);

            if (IsEX || FargoSoulsWorld.MasochistModeReal)
            {
                //vanilla fishron has x1.1 damage in p3. p2 has x1.2 damage...
                //npc.damage = (int)(npc.defDamage * 1.2f * (Main.expertMode ? 0.6f * Main.damageMultiplier : 1f));
                if (npc.ai[0] >= 9) //phase 3
                    npc.damage = Math.Max(npc.damage, (int)(npc.defDamage * 1.3));
            }

            if (npc.ai[0] > 9)
            {
                npc.dontTakeDamage = false;
                npc.chaseable = true;
            }

            npc.defense = Math.Max(npc.defense, npc.defDefense);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Anticoagulation>(), 600);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.Rabies, 3600);
            target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 20 * 60);
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (TakeNoDamageOnHit)
                damage = 0;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.SwarmActive)
                return base.CheckDead(npc);

            if (npc.ai[0] <= 9)
            {
                npc.life = 1;
                npc.active = true;
                if (Main.netMode != NetmodeID.MultiplayerClient) //something about wack ass MP
                {
                    npc.netUpdate = true;
                    npc.dontTakeDamage = true;
                    RemovedInvincibility = true;
                    NetSync(npc);
                }

                for (int index1 = 0; index1 < 100; ++index1) //gross vanilla dodge dust
                {
                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0.0f, 0.0f, 100, new Color(), 2f);
                    Main.dust[index2].position.X += Main.rand.Next(-20, 21);
                    Main.dust[index2].position.Y += Main.rand.Next(-20, 21);
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 0.5f;
                    Main.dust[index2].scale *= 1f + Main.rand.Next(50) * 0.01f;
                    //Main.dust[index2].shader = GameShaders.Armor.GetSecondaryShader(npc.cWaist, npc);
                    if (Main.rand.NextBool())
                    {
                        Main.dust[index2].scale *= 1f + Main.rand.Next(50) * 0.01f;
                        Main.dust[index2].noGravity = true;
                    }
                }
                for (int i = 0; i < 5; i++) //gross vanilla dodge dust
                {
                    int index3 = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index3].scale = 2f;
                    Main.gore[index3].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index3].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index3].velocity *= 0.5f;

                    int index4 = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index4].scale = 2f;
                    Main.gore[index4].velocity.X = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index4].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index4].velocity *= 0.5f;

                    int index5 = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index5].scale = 2f;
                    Main.gore[index5].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index5].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index5].velocity *= 0.5f;

                    int index6 = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index6].scale = 2f;
                    Main.gore[index6].velocity.X = 1.5f - Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index6].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index6].velocity *= 0.5f;

                    int index7 = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index7].scale = 2f;
                    Main.gore[index7].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index7].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index7].velocity *= 0.5f;
                }

                return false;
            }
            else
            {
                if (EModeGlobalNPC.fishBossEX == npc.whoAmI) //drop loot here (avoids the vanilla "fishron defeated" message)
                {
                    FargoSoulsWorld.downedFishronEX = true;
                    //FargoSoulsUtil.PrintText("Duke Fishron EX has been defeated!", new Color(50, 100, 255));

                    //SoundEngine.PlaySound(npc.DeathSound, npc.Center);
                    //npc.DropBossBags();
                    //npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<AbominableWand>());

                    //for (int i = 0; i < 5; i++)
                    //    Item.NewItem(npc.Hitbox, ItemID.Heart);
                    //return false;
                }
            }

            return base.CheckDead(npc);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantAntibodies>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantsCreditCard>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.OceanCrateHard, 5));
            emodeRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ItemID.FuzzyCarrot,
                ItemID.AnglerHat,
                ItemID.AnglerVest,
                ItemID.AnglerPants,
                ItemID.GoldenFishingRod,
                ItemID.GoldenBugNet,
                ItemID.FishHook,
                ItemID.HighTestFishingLine,
                ItemID.AnglerEarring,
                ItemID.TackleBox,
                ItemID.FishermansGuide,
                ItemID.WeatherRadio,
                ItemID.Sextant,
                ItemID.FinWings,
                ItemID.BottomlessBucket,
                ItemID.SuperAbsorbantSponge,
                ItemID.HotlineFishingHook
            ));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 4);
            LoadGoreRange(recolor, 573, 579);
        }
    }

    public class Sharkron : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.Sharkron, NPCID.Sharkron2);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 5;
            npc.lavaImmune = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
            {
                npc.lifeMax *= 5000;//20;//2;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            //alt behaviour cthulunado no spawn sharky
            if (source is EntitySource_Parent parent && parent.Entity is Projectile sourceProj
                && sourceProj.type == ProjectileID.Cthulunado && sourceProj.GetGlobalProjectile<Projectiles.EModeGlobalProjectile>().altBehaviour)
            {
                npc.type = NPCID.None;
                npc.active = false;
            }
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
            {
                npc.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
                npc.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            }

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Anticoagulation>(), 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 20 * 60);
            target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class DetonatingBubble : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DetonatingBubble);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lavaImmune = true;
            if (!NPC.downedBoss3)
                npc.noTileCollide = false;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 20 * 60);
            target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
        }
    }
}
