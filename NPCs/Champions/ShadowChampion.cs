using FargowiltasSouls.BossBars;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Pets;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Projectiles.Champions;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class ShadowChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Shadow");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影英灵");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
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
                Position = new Vector2(32f, -8f),
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 110;
            NPC.damage = 130;
            NPC.defense = 60;
            NPC.lifeMax = 330000;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(1);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.dontTakeDamage = true;

            NPC.BossBar = ModContent.GetInstance<CompositeBossBar>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        /*public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return true;
        }*/

        /*public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }*/

        public override void AI()
        {
            if (NPC.localAI[3] == 0) //spawn friends
            {
                NPC.TargetClosest(false);
                Movement(Main.player[NPC.target].Center, 0.8f, 32f);
                if (NPC.Distance(Main.player[NPC.target].Center) < 1500)
                    NPC.localAI[3] = 1;
                else
                    return;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 8;
                    const float distance = 110f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<ShadowOrb>(), 0, NPC.whoAmI, distance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
            }

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (NPC.HasValidTarget && NPC.Distance(player.Center) < 2500 && !Main.dayTime)
                NPC.timeLeft = 600;

            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

            if (NPC.localAI[3] == 1 && NPC.life < NPC.lifeMax * (FargoSoulsWorld.EternityMode ? 0.66 : .5))
            {
                NPC.localAI[3] = 2;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                float buffer = NPC.ai[0];
                NPC.ai[0] = -1;
                NPC.ai[1] = 0;
                NPC.ai[2] = buffer;
                NPC.ai[3] = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 16;
                    const float distance = 700f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<ShadowOrb>(), 0, NPC.whoAmI, distance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<ShadowClone>())
                        Main.projectile[i].Kill();
                }
            }
            else if (NPC.localAI[3] == 2 && NPC.life < NPC.lifeMax * .33 && FargoSoulsWorld.EternityMode)
            {
                NPC.localAI[3] = 3;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                float buffer = NPC.ai[0];
                NPC.ai[0] = -1;
                NPC.ai[1] = 0;
                NPC.ai[2] = buffer;
                NPC.ai[3] = 0;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    const int max = 24;
                    const float distance = 350f;
                    float rotation = 2f * (float)Math.PI / max;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.X, (int)spawnPos.Y, ModContent.NPCType<ShadowOrb>(), 0, NPC.whoAmI, distance, 0, rotation * i);
                        if (Main.netMode == NetmodeID.Server && n < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<ShadowClone>())
                        Main.projectile[i].Kill();
                }
            }

            if (NPC.dontTakeDamage && NPC.ai[0] != -1)
            {
                bool anyBallInvulnerable = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ShadowOrb>() && Main.npc[i].ai[0] == NPC.whoAmI
                        && !Main.npc[i].dontTakeDamage)
                    {
                        anyBallInvulnerable = true;
                        break;
                    }
                }

                if (!anyBallInvulnerable)
                {
                    SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                    const int num226 = 80;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.UnitX * 40f;
                        vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + NPC.Center;
                        Vector2 vector7 = vector6 - NPC.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 27, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].velocity = vector7;
                    }

                    NPC.dontTakeDamage = false;
                }
            }

            switch ((int)NPC.ai[0])
            {
                case -1: //trails for orbs
                    NPC.dontTakeDamage = true;
                    NPC.velocity *= 0.97f;

                    if (NPC.ai[1] == 120)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    }

                    if (++NPC.ai[3] > 9 && NPC.ai[1] > 120)
                    {
                        NPC.ai[3] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ShadowOrb>() && Main.npc[i].ai[0] == NPC.whoAmI)
                                {
                                    Vector2 vel = NPC.DirectionTo(Main.npc[i].Center).RotatedBy(Math.PI / 2);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Main.npc[i].Center, vel, ProjectileID.DemonSickle, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    if (++NPC.ai[1] > 300)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0] = NPC.ai[2];
                        if (NPC.ai[0] % 2 == 1) //always delay before resuming attack
                            NPC.ai[0]--;
                        if (NPC.ai[0] == 6) //skip shadow dash
                            NPC.ai[0] += 2;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 0: //float over player
                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f || Main.dayTime) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y -= 1f;

                        break;
                    }
                    else
                    {
                        targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400f;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.2f, 24f);
                    }

                    if (++NPC.ai[1] > 60)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 1: //dungeon guardians
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400f;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.2f, 24f);

                    //warning dust
                    Main.dust[Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 2f)].velocity *= 7f;

                    if (NPC.ai[1] == 90 && Main.netMode != NetmodeID.MultiplayerClient) //telegraph
                    {
                        for (int i = -1; i <= 1; i++) //on both sides
                        {
                            if (i == 0)
                                continue;

                            //p2 fires from above/below, others fire from sides
                            Vector2 spawnPos = player.Center + i * (NPC.localAI[3] == 2 ? Vector2.UnitY * 1000 : Vector2.UnitX * 1000);

                            for (int j = -1; j <= 1; j++) //three angles
                            {
                                Vector2 vel = Vector2.Normalize(player.Center - spawnPos);
                                vel = vel.RotatedBy(MathHelper.ToRadians(25) * j); //offset between three streams
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, NPC.target, -1f);
                            }

                            if (NPC.localAI[3] == 3) //p3 also spawns one stream from above/below
                            {
                                Vector2 wallSpawn = player.Center + i * Vector2.UnitY * 1000;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), wallSpawn, Vector2.Normalize(player.Center - wallSpawn),
                                    ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, NPC.target, -1f);
                            }
                        }
                    }

                    if (++NPC.ai[2] > 5 && NPC.ai[1] > 120)
                    {
                        NPC.ai[2] = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = -1; i <= 1; i++) //on both sides
                            {
                                if (i == 0)
                                    continue;

                                //p2 fires from above/below, others fire from sides
                                Vector2 spawnPos = player.Center + i * (NPC.localAI[3] == 2 ? Vector2.UnitY * 1000 : Vector2.UnitX * 1000);

                                for (int j = -1; j <= 1; j++) //three angles
                                {
                                    Vector2 vel = Main.rand.NextFloat(20f, 25f) * Vector2.Normalize(player.Center - spawnPos);
                                    vel = vel.RotatedBy(MathHelper.ToRadians(25) * j); //offset between three streams
                                    vel = vel.RotatedBy(MathHelper.ToRadians(5) * (Main.rand.NextDouble() - 0.5)); //random variation
                                    if (j != 0)
                                        vel *= 1.75f;
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<ShadowGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 240;
                                }

                                if (NPC.localAI[3] == 3) //p3 also spawns one stream from above/below
                                {
                                    Vector2 wallSpawn = player.Center + i * Vector2.UnitY * 1000;
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), wallSpawn, Main.rand.NextFloat(20, 25f) * Vector2.Normalize(player.Center - wallSpawn),
                                        ModContent.ProjectileType<ShadowGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 240;
                                }
                            }
                        }
                    }

                    if (++NPC.ai[1] == 120)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    }
                    else if (NPC.ai[1] > 300)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2:
                    goto case 0;

                case 3: //curving flamebursts
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 600f;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.1f, 24f);

                    if (NPC.localAI[3] == 2) //faster in p2 only
                        NPC.ai[2] += 0.5f;

                    if (++NPC.ai[2] > 60)
                    {
                        NPC.ai[2] = 0;

                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (NPC.localAI[3] == 3) //p3, triangle fire
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 vel = NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5) + 2 * Math.PI / 3 * j);
                                        float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                        float ai1 = Main.rand.NextFloat(0.05f);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<ShadowFlameburst>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer, ai0, ai1);
                                    }
                                }
                            }
                            else if (NPC.localAI[3] == 2) //p2, fire them to both sides
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (j == 0)
                                        continue;

                                    for (int i = 0; i < 25; i++)
                                    {
                                        Vector2 vel = NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5) + Math.PI / 2 * j);
                                        float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                        float ai1 = Main.rand.NextFloat(0.06f);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<ShadowFlameburst>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer, ai0, ai1);
                                    }
                                }
                            }
                            else //p1
                            {
                                for (int i = 0; i < 40; i++)
                                {
                                    Vector2 vel = 3f * NPC.DirectionTo(player.Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5));
                                    float max = 0.0075f;
                                    float ai0 = Main.rand.NextFloat(1.04f, 1.06f);
                                    float ai1 = Main.rand.NextFloat(-max, max);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<ShadowFlameburst>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer, ai0, ai1);
                                }
                            }
                        }
                    }

                    if (++NPC.ai[1] > 300)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //flaming scythe shadow orbs
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400f;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f, 24f);

                    if (++NPC.ai[2] > (NPC.localAI[3] > 1 ? 90 : 120) && NPC.ai[1] < 330) //fire a little faster depending on phase
                    {
                        NPC.ai[2] = 0;

                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = (player.Center - NPC.Center) / 30;
                            if (NPC.localAI[3] == 3) //p3 fires them to both sides instead
                            {
                                vel = vel.RotatedBy(Math.PI / 2) * 0.75f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -vel, ModContent.ProjectileType<Projectiles.Champions.ShadowOrb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<Projectiles.Champions.ShadowOrb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[1] > (NPC.localAI[3] == 3 ? 450 : 420))
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 6:
                    if (NPC.ai[1] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -5);
                    }
                    goto case 0;

                case 7: //dash for tentacles
                    if (++NPC.ai[2] == 1)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit6, NPC.Center);
                        NPC.velocity = (player.Center - NPC.Center) / 30f * (1f + NPC.localAI[3] / 3f * 0.75f);
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[2] == 31)
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[2] == 38)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 speed = vel.RotatedBy(2 * Math.PI / 6 * (i + Main.rand.NextDouble() - 0.5));
                                float ai1 = Main.rand.Next(10, 80) * (1f / 1000f);
                                if (Main.rand.NextBool())
                                    ai1 *= -1f;
                                float ai0 = Main.rand.Next(10, 80) * (1f / 1000f);
                                if (Main.rand.NextBool())
                                    ai0 *= -1f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ProjectileID.ShadowFlame, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, ai0, ai1);
                            }
                        }
                    }
                    else if (NPC.ai[2] > 60)
                    {
                        NPC.ai[2] = 0;
                    }

                    if (++NPC.ai[1] > 330)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 8:
                    goto case 0;

                case 9: //shadow clones
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400f;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.3f, 24f);

                    if (NPC.ai[2] == 0)
                    {
                        NPC.ai[2] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (NPC.localAI[3] == 1 && i % 2 == 0) //dont do half of them in p1
                                    continue;
                                for (int j = 0; j < (NPC.localAI[3] == 3 ? 2 : 1); j++) //do twice as many in p3
                                {
                                    Vector2 spawnPos = player.Center + Main.rand.NextFloat(500, 700) * Vector2.UnitX.RotatedBy(Main.rand.NextDouble() * 2 * Math.PI);
                                    Vector2 vel = NPC.velocity.RotatedBy(Main.rand.NextDouble() * Math.PI * 2);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<ShadowClone>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target, 60 + 30 * i);
                                }
                            }
                        }
                    }

                    if (++NPC.ai[1] > 360)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }

            if (NPC.dontTakeDamage)
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 27, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 54, 0f, 0f, 0, default(Color), 5f);
                    Main.dust[d].noGravity = true;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * 5)
                    NPC.frame.Y = 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
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
                NPC.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                target.AddBuff(BuffID.Blackout, 300);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.ShadowChampion], -1);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<ShadowClone>())
                        Main.projectile[i].Kill();
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(ShadowForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ShadowChampionRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<PortableFogMachine>(), 4));
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.dontTakeDamage && !NPC.IsABestiaryIconDummy)
            {
                return Color.Black;
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/ShadowChampion_Trail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = NPC.GetAlpha(drawColor);

            if (!NPC.IsABestiaryIconDummy)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp/*.PointWrap*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }

            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.VoidDye);
            shader.Apply(NPC, new DrawData?());

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = Color.White * 0.25f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D14, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            if (!NPC.IsABestiaryIconDummy)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            return false;
        }
    }
}
