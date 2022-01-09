using Fargowiltas.Items.Summons;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(GeneralTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(P3Timer), IntStrategies.CompoundStrategy },
                { new Ref<object>(EXTornadoTimer), IntStrategies.CompoundStrategy },
                
                { new Ref<object>(RemovedInvincibility), BoolStrategies.CompoundStrategy },
                { new Ref<object>(TakeNoDamageOnHit), BoolStrategies.CompoundStrategy },
                { new Ref<object>(IsEX), BoolStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.Suffocation] = true;

            if (EModeGlobalNPC.spawnFishronEX)
            {
                IsEX = true;
                npc.GivenName = Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.NameFishronEX");
                npc.damage = (int)(npc.damage * 3);// 1.5);
                npc.defense *= 30;
                npc.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
                npc.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            }
        }

        public override void AI(NPC npc)
        {
            EModeGlobalNPC.fishBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

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
                    int p = Projectile.NewProjectile(npc.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * npc.spriteDirection, speed);
                    if (reduceTimeleft && p < 1000)
                        Main.projectile[p].timeLeft /= 2;
                }
                Main.PlaySound(SoundID.Item84, npc.Center);
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
                            int ritual1 = Projectile.NewProjectile(npc.Center, Vector2.Zero,
                                ModContent.ProjectileType<FishronRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax, npc.whoAmI);
                            if (ritual1 == Main.maxProjectiles) //failed to spawn projectile, abort spawn
                                npc.active = false;
                            Main.PlaySound(SoundID.Item84, npc.Center);
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
                                int n = NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), NPCID.DetonatingBubble);
                                if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.DirectionTo(Main.player[npc.target].Center);
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        break;

                    case 2: //p1 bubbles
                        if (npc.ai[2] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        break;

                    case 3: //p1 drop nados
                        if (npc.ai[2] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 32;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = Vector2.UnitY.RotatedBy(rotation * i);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }

                            SpawnRazorbladeRing(18, 10f, npc.damage / 6, 1f);
                        }
                        break;

                    case 4: //phase 2 transition
                        RemovedInvincibility = false;
                        TakeNoDamageOnHit = true;
                        if (npc.ai[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax / 4, npc.whoAmI);
                        if (npc.ai[2] >= 114)
                        {
                            GeneralTimer++;
                            if (GeneralTimer > 6) //display healing effect
                            {
                                GeneralTimer = 0;
                                int heal = (int)(npc.lifeMax * Main.rand.NextFloat(0.1f, 0.12f));
                                npc.life += heal;
                                int max = npc.ai[0] == 9 && !Fargowiltas.Instance.MasomodeEXLoaded ? npc.lifeMax / 2 : npc.lifeMax;
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
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                                n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(-Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
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
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * 2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * -2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, 0f, 2f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                            SpawnRazorbladeRing(12, 12.5f, npc.damage / 6, 0.75f);
                            SpawnRazorbladeRing(12, 10f, npc.damage / 6, -2f);
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
                             //vanilla fishron has x1.1 damage in p3. p2 has x1.2 damage...
                             //npc.damage = (int)(npc.defDamage * 1.2f * (Main.expertMode ? 0.6f * Main.damageMultiplier : 1f));
                        TakeNoDamageOnHit = false;
                        //if (Timer >= 60 + (int)(540.0 * npc.life / npc.lifeMax)) //yes that needs to be a double
                        /*Counter2++;
                        if (Counter2 >= 900)
                        {
                            Counter2 = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient) //spawn cthulhunado
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
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
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                                n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(-Math.PI / 2);
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        goto case 10;

                    case 12: //p3 *teleports behind you*
                        if (npc.ai[2] == 15f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SpawnRazorbladeRing(5, 9f, npc.damage / 6, 1f, true);
                                SpawnRazorbladeRing(5, 9f, npc.damage / 6, -0.5f, true);
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
                                Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * 2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);
                                Projectile.NewProjectile(spawnPos.X, spawnPos.Y, npc.direction * -2f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                                const int max = 24;
                                float rotation = 2f * (float)Math.PI / max;
                                for (int i = 0; i < max; i++)
                                {
                                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DetonatingBubbleEX>());
                                    if (n != Main.maxNPCs)
                                    {
                                        Main.npc[n].velocity = npc.velocity.RotatedBy(rotation * i);
                                        Main.npc[n].velocity.Normalize();
                                        Main.npc[n].netUpdate = true;
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                    }
                                }
                            }
                        }
                        goto case 10;

                    default:
                        break;
                }
            }

            npc.position += npc.velocity * 0.25f; //fishron regular
            const int spectralFishronDelay = 3;
            switch ((int)npc.ai[0])
            {
                case -1: //just spawned
                         /*if (npc.ai[2] == 1 && Main.netMode != NetmodeID.MultiplayerClient) //create spell circle
                         {
                             int p2 = Projectile.NewProjectile(npc.Center, Vector2.Zero,
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
                            int n = NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), NPCID.DetonatingBubble);
                            if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
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
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                        }
                    }
                    break;

                case 3: //p1 drop nados
                    if (npc.ai[2] == 60f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SpawnRazorbladeRing(12, 10f, npc.damage / 4, 1f);
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
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2),
                                ModContent.ProjectileType<RazorbladeTyphoon2>(), npc.damage / 4, 0f, Main.myPlayer, .03f);
                            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.velocity).RotatedBy(-Math.PI / 2),
                                ModContent.ProjectileType<RazorbladeTyphoon2>(), npc.damage / 4, 0f, Main.myPlayer, .02f);

                            if (Fargowiltas.Instance.MasomodeEXLoaded || FargoSoulsWorld.MasochistModeReal) //lol
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NPCs.EternityMode.DetonatingBubble>());
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].velocity = npc.velocity.RotatedBy(Math.PI / 2);
                                    Main.npc[n].velocity *= -npc.spriteDirection;
                                    Main.npc[n].velocity.Normalize();
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
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
                                    Vector2 offset = SpectralFishronRandom ? Vector2.UnitY.RotatedBy(Math.PI / 3 / 3 * i) * -500f * j : Vector2.UnitX.RotatedBy(Math.PI / 3 / 3 * i) * 500f * j;
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                                }
                            }
                        }

                        if (npc.ai[2] == delayForTornadoSpawn && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 spawnPos = Vector2.UnitX * npc.direction;
                            spawnPos = spawnPos.RotatedBy(npc.rotation);
                            spawnPos *= npc.width + 20f;
                            spawnPos /= 2f;
                            spawnPos += npc.Center;
                            Projectile.NewProjectile(spawnPos.X, spawnPos.Y, 0f, 8f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer);

                            if (FargoSoulsWorld.MasochistModeReal)
                            {
                                SpawnRazorbladeRing(12, 12.5f, npc.damage / 4, 0.75f);
                                SpawnRazorbladeRing(12, 10f, npc.damage / 4, 2f * npc.direction);
                            }
                        }
                    }
                    break;

                case 9: //phase 3 transition
                    if (IsEX)
                        break;
                    npc.dontTakeDamage = true;
                    npc.defDefense = 0;
                    npc.defense = 0;
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
                        int max = Fargowiltas.Instance.MasomodeEXLoaded || FargoSoulsWorld.MasochistModeReal ? npc.lifeMax : npc.lifeMax / 2; //heal
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

                            if (FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);
                        }

                        if (++P3Timer < 180)
                        {
                            npc.ai[2] = 0; //stay in this ai mode for a bit
                            npc.position.Y -= npc.velocity.Y * 0.5f;

                            const int max = 4;
                            int P3TimerOffset = P3Timer - 30;
                            if (P3TimerOffset >= 0 && P3TimerOffset < spectralFishronDelay * max && P3TimerOffset % spectralFishronDelay == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 offset = 450 * -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / max * (P3TimerOffset / spectralFishronDelay + Main.rand.NextFloat()));
                                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FishronFishron>(), npc.damage / 4, 0f, Main.myPlayer, offset.X, offset.Y);
                            }
                        }
                    }
                    else if (npc.ai[3] == 5)
                    {
                        if (npc.ai[2] == 0)
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);

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
                                    Projectile.NewProjectile(npc.Center, baseVel.RotatedBy(2 * Math.PI / max * i),
                                        ModContent.ProjectileType<FishronBubble>(), npc.damage / 5, 0f, Main.myPlayer, delay);
                                }
                            }
                        }
                    }*/
                    break;

                case 11: //p3 dash
                    if (!Main.player[npc.target].ZoneBeach || npc.ai[3] >= 5)
                    {
                        if (npc.ai[2] == 0 && !Main.dedServ)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Monster70"), npc.Center);

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
                                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NPCs.EternityMode.DetonatingBubble>());
                                        if (n < Main.maxNPCs)
                                        {
                                            Main.npc[n].velocity = Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i) * j * 0.5f;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                        }
                                    }
                                }
                            }

                            if (!Main.player[npc.target].ZoneBeach) //enraged, spawn bubbles
                            {
                                float range = MathHelper.ToRadians(Main.rand.NextFloat(1f, 15f));
                                for (int i = -1; i <= 1; i++)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, 8f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(range * i),
                                        ModContent.ProjectileType<FishronBubble>(), npc.damage / 4, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 90;
                                }

                                for (int i = -1; i <= 1; i += 2)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, 8f * Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i),
                                        ModContent.ProjectileType<FishronBubble>(), npc.damage / 4, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 90;
                                }
                            }
                            else if (FargoSoulsWorld.MasochistModeReal)
                            {
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Projectile.NewProjectile(npc.Center, 1.5f * Vector2.Normalize(npc.velocity).RotatedBy(Math.PI / 2 * i),
                                        ModContent.ProjectileType<FishronBubble>(), npc.damage / 4, 0f, Main.myPlayer);
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
                        SpawnRazorbladeRing(6, 8f, npc.damage / 4, -0.75f);
                    }
                    else if (npc.ai[2] == 16f)
                    {
                        const int max = 5;
                        for (int j = -max; j <= max; j++)
                        {
                            Vector2 vel = npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(MathHelper.PiOver2 / max * j);
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<FishronBubble>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                    break;

                default:
                    break;
            }

            if (EModeGlobalNPC.fishBossEX == npc.whoAmI)// && npc.ai[0] >= 10 || (npc.ai[0] == 9 && npc.ai[2] > 120)) //in phase 3, do this check in all stages
            {
                EXTornadoTimer--;
            }

            if (EXTornadoTimer < 0)
            {
                EXTornadoTimer = 10 * 60;
                for (int i = -1; i <= 1; i += 2)
                {
                    int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                    int tilePosY = (int)Main.player[npc.target].Center.Y / 16;
                    tilePosX += 75 * i;

                    if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                        continue;

                    if (Main.tile[tilePosX, tilePosY] == null)
                        Main.tile[tilePosX, tilePosY] = new Tile();

                    //first move up through solid tiles
                    while (Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type])
                    {
                        tilePosY--;
                        if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                            break;
                        if (Main.tile[tilePosX, tilePosY] == null)
                            Main.tile[tilePosX, tilePosY] = new Tile();
                    }

                    tilePosY--;

                    //then move down through air until solid tile/platform reached
                    int tilesMovedDown = 0;
                    while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolidTop[Main.tile[tilePosX, tilePosY].type]))
                    {
                        tilePosY++;
                        if (tilePosX < 0 || tilePosX >= Main.maxTilesX || tilePosY < 0 || tilePosY >= Main.maxTilesY)
                            break;
                        if (Main.tile[tilePosX, tilePosY] == null)
                            Main.tile[tilePosX, tilePosY] = new Tile();
                        if (++tilesMovedDown > 32)
                        {
                            tilePosY -= 28; //give up, reset
                            break;
                        }
                    }

                    tilePosY--;

                    Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                    Projectile.NewProjectile(spawn, Vector2.UnitX * -i * 6f, ProjectileID.Cthulunado, npc.damage / 4, 0f, Main.myPlayer, 10, 24);
                }
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<TruffleWorm2>(), NPC.downedFishron, ref DroppedSummon);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.Rabies, 3600);
            target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 3600);
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
                    int index3 = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index3].scale = 2f;
                    Main.gore[index3].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index3].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index3].velocity *= 0.5f;

                    int index4 = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index4].scale = 2f;
                    Main.gore[index4].velocity.X = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index4].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index4].velocity *= 0.5f;

                    int index5 = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index5].scale = 2f;
                    Main.gore[index5].velocity.X = -1.5f - Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index5].velocity.Y = 1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index5].velocity *= 0.5f;

                    int index6 = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
                    Main.gore[index6].scale = 2f;
                    Main.gore[index6].velocity.X = 1.5f - Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index6].velocity.Y = -1.5f + Main.rand.Next(-50, 51) * 0.01f;
                    Main.gore[index6].velocity *= 0.5f;

                    int index7 = Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), new Vector2(), Main.rand.Next(61, 64), 1f);
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
                    FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.KillFishronEX"), new Color(50, 100, 255));

                    Main.PlaySound(npc.DeathSound, npc.Center);
                    npc.DropBossBags();
                    npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<CyclonicFin>());

                    for (int i = 0; i < 5; i++)
                        Item.NewItem(npc.Hitbox, ItemID.Heart);
                    return false;
                }
            }

            return base.CheckDead(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<MutantAntibodies>());
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.Bacon, Main.rand.Next(10) + 1);
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.GoldenCrate, 5);
            if (!Main.player[Main.myPlayer].GetModPlayer<FargoPlayer>().MutantsPact)
                npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<MutantsPact>());

            int[] fishingDrops = {
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
            };
            for (int i = 0; i < 3; i++)
                Item.NewItem(npc.Hitbox, fishingDrops[Main.rand.Next(fishingDrops.Length)]);
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
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
            {
                npc.lifeMax *= 5000;//20;//2;
                npc.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
                npc.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            }
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.lavaImmune = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 1800);
            target.GetModPlayer<FargoPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
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
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            if (!NPC.downedBoss3)
                npc.noTileCollide = false;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 1800);
            target.GetModPlayer<FargoPlayer>().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
        }
    }
}
