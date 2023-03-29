using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Placeables.Relics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class NatureChampion : ModNPC
    {
        /* order of heads:
         * 0 = crimson
         * 1 = molten
         * 2 = rain
         * 3 = frost
         * 4 = chloro
         * 5 = shroomite
         */
        public int[] heads = { -1, -1, -1, -1, -1, -1 };
        public int lastSet = 0;
        public static readonly KeyValuePair<int, int>[] configurations = {
            new KeyValuePair<int, int>(0, 1),
            new KeyValuePair<int, int>(1, 3),
            new KeyValuePair<int, int>(3, 5),
            new KeyValuePair<int, int>(3, 4),
            new KeyValuePair<int, int>(2, 4),
            new KeyValuePair<int, int>(0, 5),
            new KeyValuePair<int, int>(1, 2),
            new KeyValuePair<int, int>(2, 5),
            new KeyValuePair<int, int>(0, 4)
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Nature");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "自然英灵");
            Main.npcFrameCount[NPC.type] = 14;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = $"FargowiltasSouls/NPCs/Champions/{Name}_Still",
                Scale = 0.3f,
                Position = new Vector2(48f, 16 * 4),
                PortraitScale = 0.3f,
                PortraitPositionXOverride = 48,
                PortraitPositionYOverride = 24
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 180;
            NPC.height = 120;
            NPC.damage = 110;
            NPC.defense = 100;
            NPC.lifeMax = 900000;
            NPC.HitSound = SoundID.NPCHit6;
            NPC.DeathSound = SoundID.NPCDeath1;
            //NPC.noGravity = true;
            //NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(1);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < heads.Length; i++)
                writer.Write(heads[i]);
            writer.Write(lastSet);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < heads.Length; i++)
                heads[i] = reader.ReadInt32();
            lastSet = reader.ReadInt32();
        }

        public override void AI()
        {
            if (NPC.localAI[3] == 0) //spawn friends
            {
                NPC.TargetClosest(false);
                NPC.localAI[3] = 1;
                /*if (NPC.Distance(Main.player[NPC.target].Center) < 1500)
                {
                    NPC.localAI[3] = 1;
                }
                else
                {
                    Movement(Main.player[NPC.target].Center, 0.8f, 32f);
                    return;
                }*/

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NatureChampionHead>(), NPC.whoAmI, 0f, NPC.whoAmI, 0f, -3f, NPC.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[0] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NatureChampionHead>(), NPC.whoAmI, 0f, NPC.whoAmI, 0f, -2f, NPC.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[1] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NatureChampionHead>(), NPC.whoAmI, 0f, NPC.whoAmI, 0f, -1f, NPC.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[2] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NatureChampionHead>(), NPC.whoAmI, 0f, NPC.whoAmI, 0f, 1f, NPC.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[3] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NatureChampionHead>(), NPC.whoAmI, 0f, NPC.whoAmI, 0f, 2f, NPC.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[4] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                    n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NatureChampionHead>(), NPC.whoAmI, 0f, NPC.whoAmI, 0f, 3f, NPC.target);
                    if (n != Main.maxNPCs)
                    {
                        heads[5] = n;
                        Main.npc[n].velocity.X = Main.rand.NextFloat(-24f, 24f);
                        Main.npc[n].velocity.Y = Main.rand.NextFloat(-24f, 24f);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }

                    for (int i = 0; i < heads.Length; i++) //failsafe, die if couldnt spawn heads
                    {
                        if (heads[i] == -1 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.active = false;
                            return;
                        }
                    }
                }
            }

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (NPC.HasValidTarget && NPC.Distance(player.Center) < 3000 && player.Center.Y >= Main.worldSurface * 16 && !player.ZoneUnderworldHeight)
                NPC.timeLeft = 600;

            if (player.Center.X < NPC.position.X)
                NPC.direction = NPC.spriteDirection = -1;
            else if (player.Center.X > NPC.position.X + NPC.width)
                NPC.direction = NPC.spriteDirection = 1;

            switch ((int)NPC.ai[0])
            {
                case -1: //mourning wood movement
                    {
                        NPC.noTileCollide = true;
                        NPC.noGravity = true;

                        if (NPC.position.X < player.Center.X && player.Center.X < NPC.position.X + NPC.width)
                        {
                            NPC.velocity.X *= 0.92f;
                            if (Math.Abs(NPC.velocity.X) < 0.1f)
                                NPC.velocity.X = 0f;
                        }
                        else
                        {
                            float accel = 2f;
                            /*if (Math.Abs(player.Center.X - NPC.Center.X) > 1200) //secretly fast run
                            {
                                accel = 24f;
                            }
                            else
                            {
                                if (Math.Abs(NPC.velocity.X) > 2)
                                    NPC.velocity.X *= 0.97f;
                            }*/
                            if (player.Center.X > NPC.Center.X)
                                NPC.velocity.X = (NPC.velocity.X * 20 + accel) / 21;
                            else
                                NPC.velocity.X = (NPC.velocity.X * 20 - accel) / 21;
                        }

                        bool onPlatforms = false;
                        for (int i = (int)NPC.position.X; i <= NPC.position.X + NPC.width; i += 16)
                        {
                            if (Framing.GetTileSafely(new Vector2(i, NPC.position.Y + NPC.height + NPC.velocity.Y + 1)).TileType == TileID.Platforms)
                            {
                                onPlatforms = true;
                                break;
                            }
                        }

                        bool onCollision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);

                        if (NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width
                            && NPC.position.Y + NPC.height < player.position.Y + player.height - 16)
                        {
                            NPC.velocity.Y += 0.5f;
                        }
                        else if (onCollision || (onPlatforms && player.position.Y + player.height <= NPC.position.Y + NPC.height))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y = 0f;

                            if (onCollision)
                            {
                                if (NPC.velocity.Y > -0.2f)
                                    NPC.velocity.Y -= 0.025f;
                                else
                                    NPC.velocity.Y -= 0.2f;

                                if (NPC.velocity.Y < -4f)
                                    NPC.velocity.Y = -4f;
                            }
                        }
                        else
                        {
                            if (NPC.velocity.Y < 0f)
                                NPC.velocity.Y = 0f;

                            if (NPC.velocity.Y < 0.1f)
                                NPC.velocity.Y += 0.025f;
                            else
                                NPC.velocity.Y += 0.5f;
                        }

                        if (NPC.velocity.Y > 10f)
                            NPC.velocity.Y = 10f;
                    }
                    break;

                case 0: //think
                    NPC.noTileCollide = false;
                    NPC.noGravity = false;

                    if (++NPC.ai[1] > 45)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    goto case -1;

                case 1: //stomp
                    {
                        void StompDust()
                        {
                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                            for (int k = -2; k <= 2; k++) //explosions
                            {
                                Vector2 dustPos = NPC.Center;
                                int width = NPC.width / 5;
                                dustPos.X += width * k + Main.rand.NextFloat(-width, width);
                                dustPos.Y += Main.rand.NextFloat(NPC.height / 2);

                                for (int i = 0; i < 30; i++)
                                {
                                    int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                                    Main.dust[dust].velocity *= 1.4f;
                                }

                                for (int i = 0; i < 20; i++)
                                {
                                    int dust = Dust.NewDust(dustPos, 32, 32, 6, 0f, 0f, 100, default(Color), 3.5f);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].velocity *= 7f;
                                    dust = Dust.NewDust(dustPos, 32, 32, 6, 0f, 0f, 100, default(Color), 1.5f);
                                    Main.dust[dust].velocity *= 3f;
                                }

                                float scaleFactor9 = 0.5f;
                                for (int j = 0; j < 4; j++)
                                {
                                    int gore = Gore.NewGore(NPC.GetSource_FromThis(), dustPos, default(Vector2), Main.rand.Next(61, 64));
                                    Main.gore[gore].velocity *= scaleFactor9;
                                    Main.gore[gore].velocity.X += 1f;
                                    Main.gore[gore].velocity.Y += 1f;
                                }
                            }
                        }

                        int jumpTime = 60;
                        if (NPC.ai[3] == 1)
                            jumpTime = 30;

                        NPC.noGravity = true;
                        NPC.noTileCollide = true;

                        if (NPC.ai[2] == 0) //move over player
                        {
                            StompDust();

                            NPC.ai[2] = 1;
                            NPC.netUpdate = true;

                            targetPos = player.Center;
                            targetPos.Y -= NPC.ai[3] == 1 ? 300 : 600;

                            NPC.velocity = (targetPos - NPC.Center) / jumpTime;
                        }

                        if (++NPC.ai[1] > jumpTime + (NPC.ai[3] == 1 ? 1 : 18)) //do the stomp
                        {
                            NPC.noGravity = false;
                            NPC.noTileCollide = false;

                            if (NPC.velocity.Y == 0 || NPC.ai[3] == 1) //landed, now stomp
                            {
                                StompDust();

                                if (NPC.ai[3] == 1) //enraged
                                {
                                    for (int i = Main.rand.Next(2); i < heads.Length; i += 2) //activate alternating heads for deathray
                                    {
                                        if (Main.npc[heads[i]].ai[0] != 0) //don't act on a head currently doing something
                                            continue;

                                        Main.npc[heads[i]].ai[0] = 4f;
                                        Main.npc[heads[i]].localAI[0] = 0;
                                        Main.npc[heads[i]].ai[2] = 0;
                                        Main.npc[heads[i]].localAI[1] = 0;
                                        Main.npc[heads[i]].netUpdate = true;

                                        int glowType;
                                        switch ((int)Main.npc[heads[i]].ai[3])
                                        {
                                            case -3: glowType = -7; break;
                                            case -2: glowType = -8; break;
                                            case -1: glowType = -9; break;
                                            case 1: glowType = -10; break;
                                            case 2: glowType = -11; break;
                                            case 3: glowType = -12; break;
                                            default: glowType = 0; break;
                                        }

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, heads[i], glowType);
                                    }
                                }

                                NPC.TargetClosest();
                                NPC.ai[0]++;
                                NPC.ai[1] = NPC.ai[3] == 1 ? 40 : 0;
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else if (NPC.ai[1] > jumpTime) //falling
                        {
                            if (NPC.velocity.X > 2)
                                NPC.velocity.X = 2;
                            if (NPC.velocity.X < -2)
                                NPC.velocity.X = -2;
                            NPC.velocity.Y = 30f;
                        }
                    }
                    break;

                case 2:
                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 3000f
                        || player.Center.Y < Main.worldSurface * 16 || player.ZoneUnderworldHeight) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y += 1f;

                        break;
                    }
                    goto case 0;

                case 3: //decide an attack
                    if (NPC.ai[2] == 0)
                    {
                        void ActivateHead(int targetHead)
                        {
                            if (Main.npc[targetHead].ai[0] != 0) //don't act on a head currently doing something
                                return;

                            Main.npc[targetHead].ai[0] += Main.npc[targetHead].ai[3];
                            Main.npc[targetHead].localAI[0] = 0;
                            Main.npc[targetHead].ai[2] = 0;
                            Main.npc[targetHead].localAI[1] = 0;
                            Main.npc[targetHead].netUpdate = true;

                            SoundEngine.PlaySound(SoundID.ForceRoarPitched, Main.npc[targetHead].Center);

                            int glowType;
                            switch ((int)Main.npc[targetHead].ai[3])
                            {
                                case -3: glowType = -7; break;
                                case -2: glowType = -8; break;
                                case -1: glowType = -9; break;
                                case 1: glowType = -10; break;
                                case 2: glowType = -11; break;
                                case 3: glowType = -12; break;
                                default: glowType = 0; break;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, targetHead, glowType);
                        };

                        NPC.ai[2] = 1;
                        NPC.netUpdate = true;

                        int set = Main.rand.Next(configurations.Length);
                        while (heads[configurations[set].Key] == heads[configurations[lastSet].Key] //don't reuse heads you just attacked with
                            || heads[configurations[set].Key] == heads[configurations[lastSet].Value]
                            || heads[configurations[set].Value] == heads[configurations[lastSet].Key]
                            || heads[configurations[set].Value] == heads[configurations[lastSet].Value])
                            set = Main.rand.Next(configurations.Length);
                        lastSet = set;

                        if (Main.expertMode) //activate both in expert
                        {
                            ActivateHead(heads[configurations[set].Key]);
                            ActivateHead(heads[configurations[set].Value]);
                        }
                        else //only activate one in normal
                        {
                            if (Main.rand.NextBool())
                                ActivateHead(heads[configurations[set].Key]);
                            else
                                ActivateHead(heads[configurations[set].Value]);
                        }
                    }

                    if (++NPC.ai[1] > 300) //wait
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    goto case -1;

                case 4:
                    goto case 2;

                case 5:
                    goto case 3;

                case 6:
                    goto case 2;

                case 7:
                    goto case 3;

                case 8:
                    goto case 2;

                case 9:
                    goto case 1;

                case 10:
                    goto case 2;

                case 11: //deathrays
                    if (NPC.ai[2] == 0 && FargoSoulsWorld.EternityMode)
                    {
                        NPC.ai[2] = 1;

                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        for (int i = 0; i < heads.Length; i++) //activate all heads
                        {
                            Main.npc[heads[i]].ai[0] = 4f;
                            Main.npc[heads[i]].localAI[0] = 0;
                            Main.npc[heads[i]].ai[2] = 0;
                            Main.npc[heads[i]].localAI[1] = 0;
                            Main.npc[heads[i]].netUpdate = true;

                            int glowType;
                            switch ((int)Main.npc[heads[i]].ai[3])
                            {
                                case -3: glowType = -7; break;
                                case -2: glowType = -8; break;
                                case -1: glowType = -9; break;
                                case 1: glowType = -10; break;
                                case 2: glowType = -11; break;
                                case 3: glowType = -12; break;
                                default: glowType = 0; break;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, heads[i], glowType);
                        }
                    }

                    if (++NPC.ai[1] > 330 || !FargoSoulsWorld.EternityMode) //wait
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    goto case -1;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }

            if (FargoSoulsWorld.EternityMode)
            {
                if (NPC.HasValidTarget && NPC.Distance(player.Center) > 1400 && Vector2.Distance(NPC.Center, player.Center) < 3000f
                  && player.Center.Y > Main.worldSurface * 16 && !player.ZoneUnderworldHeight && NPC.ai[0] > 1)// && NPC.ai[0] != 9) //enrage
                {
                    NPC.ai[0] = 1;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 1; //marks enrage jump
                    NPC.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, player.Center);
                }

                Vector2 dustOffset = Vector2.Normalize(player.Center - NPC.Center) * 1400;
                for (int i = 0; i < 20; i++) //dust ring for enrage range
                {
                    int d = Dust.NewDust(NPC.Center + dustOffset.RotatedByRandom(2 * Math.PI), 0, 0, 59, Scale: 2f);
                    Main.dust[d].velocity = NPC.velocity;
                    Main.dust[d].noGravity = true;
                }
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (NPC.Center.X < targetPos.X)
            {
                NPC.velocity.X += speedModifier;
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += speedModifier * 2;
            }
            else
            {
                NPC.velocity.X -= speedModifier;
                if (NPC.velocity.X > 0)
                    NPC.velocity.X -= speedModifier * 2;
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity == Vector2.Zero)
            {
                if (NPC.frame.Y < frameHeight * 8)
                    NPC.frame.Y = frameHeight * 8;

                if (++NPC.frameCounter > 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                }

                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = frameHeight * 8;
            }
            else
            {
                if (++NPC.frameCounter > 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * 7)
                    NPC.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Frostburn, 300);
                target.AddBuff(BuffID.OnFire, 300);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 6; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"NatureGore{i}").Type, NPC.scale);
                }

                for (int i = 0; i < Main.maxNPCs; i++) //find neck segments, place gores there
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<NatureChampionHead>() && Main.npc[i].ai[1] == NPC.whoAmI)
                    {
                        Vector2 connector = Main.npc[i].Center;
                        Vector2 neckOrigin = NPC.Center + new Vector2(54 * NPC.spriteDirection, -10);
                        float chainsPerUse = 0.05f;
                        bool spawnNeck = false;
                        for (float j = 0; j <= 1; j += chainsPerUse)
                        {
                            if (j == 0)
                                continue;
                            Vector2 distBetween = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) -
                            X(j - chainsPerUse, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X),
                            Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) -
                            Y(j - chainsPerUse, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                            if (distBetween.Length() > 36 && chainsPerUse > 0.01f)
                            {
                                chainsPerUse -= 0.01f;
                                j -= chainsPerUse;
                                continue;
                            }
                            float projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2;
                            Vector2 lightPos = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X), Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));

                            spawnNeck = !spawnNeck;
                            if (spawnNeck)
                                if (!Main.dedServ)
                                    Gore.NewGore(NPC.GetSource_FromThis(), lightPos, Main.npc[i].velocity, ModContent.Find<ModGore>(Mod.Name, "NatureGore7").Type, Main.npc[i].scale);
                        }

                        for (int j = 8; j <= 10; j++) //head gores
                        {
                            Vector2 pos = Main.npc[i].position + new Vector2(Main.rand.NextFloat(Main.npc[i].width), Main.rand.NextFloat(Main.npc[i].height));
                            if (!Main.dedServ)
                                Gore.NewGore(NPC.GetSource_FromThis(), pos, Main.npc[i].velocity, ModContent.Find<ModGore>(Mod.Name, $"NatureGore{j}").Type, Main.npc[i].scale);
                        }
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.NatureChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(NatureForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<NatureChampionRelic>()));
        }

        public Vector2 position, oldPosition;
        private static float X(float t, float x0, float x1, float x2)
        {
            return (float)(
                x0 * Math.Pow((1 - t), 2) +
                x1 * 2 * t * Math.Pow((1 - t), 1) +
                x2 * Math.Pow(t, 2)
            );
        }
        private static float Y(float t, float y0, float y1, float y2)
        {
            return (float)(
                 y0 * Math.Pow((1 - t), 2) +
                 y1 * 2 * t * Math.Pow((1 - t), 1) +
                 y2 * Math.Pow(t, 2)
             );
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 0.8f;
            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, origin2, NPC.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<NatureChampionHead>() && Main.npc[i].ai[1] == NPC.whoAmI)
                {
                    if (NPC.Distance(Main.LocalPlayer.Center) <= 1200)
                    {
                        string neckTex = "NPCs/Champions/NatureChampion_Neck";
                        Texture2D neckTex2D = FargowiltasSouls.Instance.Assets.Request<Texture2D>(neckTex, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        Vector2 connector = Main.npc[i].Center;
                        Vector2 neckOrigin = NPC.Center + new Vector2(54 * NPC.spriteDirection, -10);
                        float chainsPerUse = 0.05f;
                        for (float j = 0; j <= 1; j += chainsPerUse)
                        {
                            if (j == 0)
                                continue;
                            Vector2 distBetween = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) -
                            X(j - chainsPerUse, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X),
                            Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) -
                            Y(j - chainsPerUse, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                            if (distBetween.Length() > 36 && chainsPerUse > 0.01f)
                            {
                                chainsPerUse -= 0.01f;
                                j -= chainsPerUse;
                                continue;
                            }
                            float projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2;
                            Vector2 lightPos = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X), Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                            spriteBatch.Draw(neckTex2D, new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) - screenPos.X, Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) - screenPos.Y),
                            new Rectangle(0, 0, neckTex2D.Width, neckTex2D.Height), NPC.GetAlpha(Lighting.GetColor((int)lightPos.X / 16, (int)lightPos.Y / 16)), projTrueRotation,
                            new Vector2(neckTex2D.Width * 0.5f, neckTex2D.Height * 0.5f), 1f, connector.X < neckOrigin.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        }
                    }

                    /*Texture2D texture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/NatureChampion_Neck", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Vector2 position = Main.npc[i].Center;
                    Vector2 mountedCenter = NPC.Center + new Vector2(54 * NPC.spriteDirection, -10);
                    Rectangle? sourceRectangle = new Rectangle?();
                    Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                    float num1 = texture.Height;
                    Vector2 vector24 = mountedCenter - position;
                    float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                    bool flag = true;
                    if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                        flag = false;
                    if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                        flag = false;
                    while (flag)
                        if (vector24.Length() < num1 + 1.0)
                        {
                            flag = false;
                        }
                        else
                        {
                            Vector2 vector21 = vector24;
                            vector21.Normalize();
                            position += vector21 * num1;
                            vector24 = mountedCenter - position;
                            Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                            color2 = NPC.GetAlpha(color2);
                            Main.EntitySpriteDraw(texture, position - screenPos, sourceRectangle, color2, rotation, origin, 1f, 
                                position.X < mountedCenter.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                        }*/

                    DrawHead(spriteBatch, screenPos, Lighting.GetColor((int)Main.npc[i].Center.X / 16, (int)Main.npc[i].Center.Y / 16), Main.npc[i]);
                }
            }
            return false;
        }

        private void DrawHead(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, NPC head)
        {
            if (!Terraria.GameContent.TextureAssets.Npc[NPC.type].IsLoaded)
                return;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[head.type].Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = head.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = head.GetAlpha(color26);

            SpriteEffects effects = head.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[head.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[head.type] - i) / NPCID.Sets.TrailCacheLength[head.type];
                Vector2 value4 = head.oldPos[i];
                float num165 = head.rotation; //head.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + head.Size / 2f - screenPos + new Vector2(0, head.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, head.scale, effects, 0);
            }

            int glow = (int)head.ai[3];
            if (glow > 0)
                glow--;
            glow += 3;
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/NatureChampionHead_Glow" + glow.ToString(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Main.EntitySpriteDraw(texture2D13, head.Center - screenPos + new Vector2(0f, head.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), head.GetAlpha(drawColor), head.rotation, origin2, head.scale, effects, 0);
            Main.EntitySpriteDraw(texture2D14, head.Center - screenPos + new Vector2(0f, head.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, head.rotation, origin2, head.scale, effects, 0);
        }
    }
}
