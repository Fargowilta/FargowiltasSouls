using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
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
    public class TimberChampion : ModNPC
    {
        private const float BaseWalkSpeed = 4f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Timber");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "木英灵");
            Main.npcFrameCount[NPC.type] = 8;
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
                Position = new Vector2(16 * 4, 16 * 9),
                PortraitPositionXOverride = 16,
                PortraitPositionYOverride = 16 * 7
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasSouls.Bestiary.TimberChampion")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 234;
            NPC.damage = 110;
            NPC.defense = 50;
            NPC.lifeMax = 240000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
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
            CooldownSlot = 1;
            return true;
        }

        int JumpTreshold => FargoSoulsWorld.MasochistModeReal ? 30 : 60;

        bool drawTrail;

        public override void AI()
        {
            if (NPC.localAI[3] == 0)
            {
                NPC.TargetClosest(false);
                NPC.localAI[3] = 1;
            }

            drawTrail = false;

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = NPC.position.X < player.position.X ? 1 : -1;

            switch ((int)NPC.ai[0])
            {
                case -2: //collision
                    TileCollision(player.Center.Y > NPC.Bottom.Y, Math.Abs(player.Center.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < player.Center.Y);
                    break;

                case -1: //mourning wood movement
                    Movement(player.Center);
                    break;

                case 0: //jump at player
                    {
                        NPC.noTileCollide = false;
                        NPC.noGravity = false;

                        float time = FargoSoulsWorld.EternityMode ? 60 : 90;
                        float gravity = FargoSoulsWorld.EternityMode ? 0.8f : 0.4f;

                        if (++NPC.ai[1] == JumpTreshold)
                        {
                            NPC.TargetClosest();

                            if (FargoSoulsWorld.MasochistModeReal)
                            {
                                if (NPC.localAI[1] == 0 && NPC.life < NPC.lifeMax * .66f) //spawn palm tree supports
                                {
                                    NPC.localAI[1] = 1;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TimberPalmTree>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                                }

                                if (NPC.localAI[2] == 0 && NPC.life < NPC.lifeMax * .33f) //spawn palm tree supports
                                {
                                    NPC.localAI[2] = 1;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TimberPalmTree>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                                }
                            }

                            Vector2 distance = player.Top - NPC.Bottom;
                            distance.X += FargoSoulsWorld.MasochistModeReal ? player.velocity.X * time : 420 * Math.Sign(distance.X);

                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            NPC.velocity = distance;

                            NPC.noTileCollide = true;
                            NPC.noGravity = true;
                            NPC.netUpdate = true;

                            if (Main.netMode != NetmodeID.MultiplayerClient) //ogre smash jump
                            {
                                int dam = FargoSoulsWorld.MasochistModeReal ? FargoSoulsUtil.ScaledProjectileDamage(NPC.damage) : 0;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileID.DD2OgreSmash, dam, 0, Main.myPlayer);
                            }

                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                            //visual explosions
                            for (int k = -2; k <= 2; k++)
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

                            //chain blast jump
                            if (FargoSoulsWorld.EternityMode)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom - 65 * Vector2.UnitY, Vector2.Zero, ModContent.ProjectileType<TimberJumpMark>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, NPC.width);
                            }
                        }
                        else if (NPC.ai[1] > JumpTreshold)
                        {
                            NPC.noTileCollide = true;
                            NPC.noGravity = true;
                            NPC.velocity.Y += gravity;
                            drawTrail = true;

                            if (NPC.ai[1] > JumpTreshold + time)
                            {
                                NPC.TargetClosest();
                                NPC.ai[1] = JumpTreshold - 1;
                                NPC.netUpdate = true;

                                if (--NPC.ai[2] <= 0)
                                {
                                    NPC.ai[0]++;
                                    NPC.ai[1] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.ai[3] = 0;
                                    goto case -2; //ensures no tile clipping after
                                }
                            }
                        }
                        else //pre jump
                        {
                            if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                                NPC.velocity.X = Math.Abs(NPC.velocity.Y) * Math.Sign(NPC.velocity.X);

                            NPC.velocity.X *= 0.99f;

                            if (NPC.ai[0] != 0 && (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f))
                            {
                                NPC.TargetClosest();
                                if (NPC.timeLeft > 30)
                                    NPC.timeLeft = 30;

                                NPC.noTileCollide = true;
                                NPC.noGravity = true;
                                NPC.velocity.Y += 1f;

                                NPC.ai[1] = 0; //prevent proceeding to next steps of ai while despawning
                                return;
                            }
                            else
                            {
                                NPC.timeLeft = 600;
                            }

                            goto case -2;
                        }
                    }
                    break;

                case 1: //acorn sprays
                    if (++NPC.ai[2] > 35)
                    {
                        NPC.ai[2] = 0;
                        NPC.ai[3]++;
                        const float gravity = 0.2f;
                        float time = 60f;
                        Vector2 distance = player.Center - NPC.Center;// + player.velocity * 30f;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 15; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 3,
                                ModContent.ProjectileType<TimberAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                        }
                    }

                    NPC.localAI[0] = 0;

                    if (FargoSoulsWorld.EternityMode && NPC.ai[3] == 2)
                    {
                        if (NPC.ai[2] == 0) //shotgun
                            SoundEngine.PlaySound(SoundID.Item36, NPC.Center);

                        NPC.velocity.X *= 0.9f;

                        NPC.localAI[0] = 1;

                        if (!FargoSoulsWorld.MasochistModeReal)
                        {
                            NPC.ai[1] -= 0.4f;
                            NPC.ai[2] -= 0.4f;
                        }

                        if (NPC.ai[2] > 30)
                        {
                            NPC.ai[2] = -1000;
                            NPC.ai[3]++;

                            NPC.velocity.Y -= 12f;

                            SoundEngine.PlaySound(SoundID.Item36, NPC.Center);

                            const float gravity = 0.2f;
                            float time = 45f;
                            Vector2 distance = player.Center - NPC.Center + player.velocity * 15f;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            for (int i = 0; i < 30; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 4.5f,
                                    ModContent.ProjectileType<TimberAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 120)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                        NPC.TargetClosest();
                    }
                    goto case -1;

                case 2:
                    if (NPC.ai[3] == 0) //flags to repeat jumps
                    {
                        NPC.ai[3] = 1;
                        NPC.ai[2] = 4;
                    }
                    goto case 0;

                case 3: //snowball barrage
                    if (NPC.ai[1] == 0) //telegraph
                    {
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 pos = GetArmPos(i);

                            SoundEngine.PlaySound(SoundID.Item11, pos);

                            for (int j = 0; j < 20; j++)
                            {
                                int d = Dust.NewDust(pos, 0, 0, DustID.SnowBlock, Scale: 3f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity *= 4f;
                                Main.dust[d].velocity.X += NPC.direction * Main.rand.NextFloat(6f, 24f);
                            }
                        }
                    }

                    if (FargoSoulsWorld.EternityMode)
                    {
                        if (++NPC.ai[2] > 60) //spread shot of snowballs
                        {
                            NPC.ai[2] = 40;

                            NPC.ai[3] = NPC.ai[3] == 1 ? -1 : 1;

                            Vector2 spawnPos = GetArmPos((int)NPC.ai[3]);
                            Vector2 vel = 16f * player.DirectionFrom(spawnPos);

                            SoundEngine.PlaySound(SoundID.Item11, spawnPos);

                            const int max = 3;
                            for (int i = -max; i <= max; i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel.RotatedBy(MathHelper.ToRadians(60) / max * i), ModContent.ProjectileType<TimberSnowball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1f);
                            }
                        }
                    }
                    else if (++NPC.ai[2] > 5) //straight aim
                    {
                        NPC.ai[2] = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] > 30 && NPC.ai[1] < 120)
                        {
                            Vector2 offset;
                            offset.X = Main.rand.NextFloat(0, NPC.width / 2) * NPC.direction;
                            offset.Y = 16;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + offset,
                                Vector2.UnitY * -12f, ModContent.ProjectileType<TimberSnowball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.ai[1] > 150)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                        NPC.TargetClosest();
                    }
                    goto case -1;

                case 4:
                    goto case 0;

                case 5: //spray squirrels
                    if (++NPC.ai[2] > 6)
                    {
                        NPC.ai[2] = 0;
                        FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<LesserSquirrel>(),
                            velocity: new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-20, -10)));
                    }

                    if (++NPC.ai[1] > 180)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.netUpdate = true;
                        NPC.TargetClosest();
                    }
                    goto case -1;

                case 6:
                    goto case 2;

                case 7:
                    goto case 3;

                case 8:
                    goto case 0;

                case 9: //prepare to hook
                    NPC.velocity.X *= 0.9f;
                    NPC.localAI[0] = 0; //out here for mp sync

                    if (++NPC.ai[1] > 60)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    goto case -2;

                case 10: //grappling hook
                    if (NPC.ai[1] == 0)
                    {
                        NPC.ai[3] = Math.Sign(player.Center.X - NPC.Center.X);
                        NPC.direction = NPC.spriteDirection = (int)NPC.ai[3]; //make sure this is correct before spawning chains
                        NPC.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 spawnPos = GetArmPos(i);
                            Vector2 vel = 15f * player.DirectionFrom(spawnPos);
                            const float ai1 = 30f * 3;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<TimberHook>(), 0, 0f, Main.myPlayer, NPC.whoAmI, ai1);
                        }
                    }

                    if (++NPC.ai[1] > 180 || !Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<TimberHook>() && p.ai[0] == NPC.whoAmI))
                    {
                        if (++NPC.ai[2] > 3)
                        {
                            NPC.ai[0]++;
                            NPC.ai[2] = 0;
                        }
                        NPC.ai[1] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }

                    drawTrail = true;

                    //do collision until pulled
                    if (NPC.localAI[0] == 0)
                    {
                        NPC.direction = NPC.spriteDirection = (int)NPC.ai[3];
                        goto case -2;
                    }
                    else
                    {
                        NPC.direction = NPC.spriteDirection = (int)NPC.localAI[0];
                    }
                    break;

                case 11: //walk back in range, reduces despawn issue
                    if (++NPC.ai[1] > 120)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                    }
                    goto case -1;

                //NOTE: make sure the grapple and the walk afterwards is the final attack!
                //this is so it uses the ai0=0 jump, which cant despawn

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }
        }

        private void TileCollision(bool fallthrough = false, bool dropDown = false)
        {
            bool onPlatforms = false;
            for (int i = (int)NPC.position.X; i <= NPC.position.X + NPC.width; i += 16)
            {
                if (Framing.GetTileSafely(new Vector2(i, NPC.Bottom.Y)).TileType == TileID.Platforms)
                {
                    onPlatforms = true;
                    break;
                }
            }

            bool onCollision = Collision.SolidCollision(NPC.position, NPC.width, NPC.height);

            if (dropDown)
            {
                NPC.velocity.Y += 0.5f;
            }
            else if (onCollision || (onPlatforms && !fallthrough))
            {
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y = 0f;

                if (NPC.velocity.Y > -0.2f)
                    NPC.velocity.Y -= 0.025f;
                else
                    NPC.velocity.Y -= 0.2f;

                if (NPC.velocity.Y < -4f)
                    NPC.velocity.Y = -4f;
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

        private void Movement(Vector2 target)
        {
            NPC.direction = NPC.spriteDirection = NPC.Center.X < target.X ? 1 : -1;

            if (Math.Abs(target.X - NPC.Center.X) < NPC.width / 2)
            {
                NPC.velocity.X *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < 0.1f)
                    NPC.velocity.X = 0f;
            }
            else
            {
                float maxwalkSpeed = BaseWalkSpeed;
                if (FargoSoulsWorld.MasochistModeReal)
                    maxwalkSpeed *= 2;

                int walkModifier = 30;

                if (NPC.direction > 0)
                    NPC.velocity.X = (NPC.velocity.X * walkModifier + maxwalkSpeed) / (walkModifier + 1);
                else
                    NPC.velocity.X = (NPC.velocity.X * walkModifier - maxwalkSpeed) / (walkModifier + 1);
            }

            TileCollision(target.Y > NPC.Bottom.Y, Math.Abs(target.X - NPC.Center.X) < NPC.width / 2 && NPC.Bottom.Y < target.Y);
        }

        private Vector2 GetArmPos(int arm)
        {
            const float midpoint = 47;
            const float xOffset = 19;

            Vector2 offset = new Vector2(midpoint + xOffset * arm, 28);

            offset.X *= NPC.direction;

            return NPC.Center + offset;
        }

        public override void FindFrame(int frameHeight)
        {
            switch ((int)NPC.ai[0])
            {
                case 0:
                    if (NPC.IsABestiaryIconDummy) //do walk animation in bestiary
                        goto default;

                    if (NPC.ai[1] <= JumpTreshold)
                        NPC.frame.Y = frameHeight * 6; //crouching for jump
                    else
                        NPC.frame.Y = frameHeight * 7; //jumping
                    break;

                case 1:
                    if (NPC.localAI[0] == 1)
                    {
                        NPC.frame.Y = frameHeight * 6; //crouching
                        break;
                    }
                    goto default;

                case 2:
                case 4:
                case 6:
                case 8:
                    goto case 0;

                case 9:
                    NPC.frame.Y = frameHeight * 6; //crouching
                    break;

                case 10:
                    if (NPC.localAI[0] == 0)
                        goto default;

                    NPC.frame.Y = frameHeight * 7;
                    break;

                default:
                    {
                        NPC.frameCounter += 1f / BaseWalkSpeed * Math.Abs(NPC.velocity.X);

                        if (NPC.frameCounter > 2.5f) //walking animation
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += frameHeight;
                        }

                        if (NPC.frame.Y >= frameHeight * 6)
                            NPC.frame.Y = 0;

                        if (NPC.velocity.X == 0)
                            NPC.frame.Y = frameHeight; //stationary sprite if standing still

                        if (NPC.velocity.Y > 4)
                            NPC.frame.Y = frameHeight * 7; //jumping
                    }
                    break;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Guilty>(), 600);
        }

        bool spawnPhase2 => Main.expertMode;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 3; i <= 10; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TimberGore{i}").Type, NPC.scale);
                }

                FargoSoulsUtil.GrossVanillaDodgeDust(NPC);

                for (int i = 0; i < 6; i++)
                    ExplodeDust(NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)));
            }
        }

        private void ExplodeDust(Vector2 center)
        {
            SoundEngine.PlaySound(SoundID.Item14, center);

            const int width = 32;
            const int height = 32;

            Vector2 pos = center - new Vector2(width, height) / 2f;

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(pos, width, height, DustID.Smoke, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(pos, width, height, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;

                dust = Dust.NewDust(pos, width, height, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 3; j++)
            {
                int gore = Gore.NewGore(NPC.GetSource_FromThis(), center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override bool PreKill()
        {
            if (spawnPhase2)
            {
                NPC.value = 0;
                FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<TimberChampionHead>(), NPC.whoAmI, target: NPC.target);
                return false;
            }

            return base.PreKill();
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TimberChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(TimberForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TimberChampionRelic>()));
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (drawTrail)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Color color27 = color26 * 0.33f;
                    color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                    Vector2 value4 = NPC.oldPos[i];
                    float num165 = NPC.rotation; //NPC.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY + 2), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY + 2), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
