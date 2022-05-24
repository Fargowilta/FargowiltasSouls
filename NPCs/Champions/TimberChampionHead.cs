using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Challengers;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class TimberChampionHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Timber");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "木英灵");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

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
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.damage = 140;
            NPC.defense = 50;
            NPC.lifeMax = 160000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 15);
            NPC.boss = true;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;

            if (noHurt)
                return false;

            return true;
        }

        bool haveGottenInRange;
        bool noHurt;

        public override void AI()
        {
            if (NPC.localAI[2] == 0)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                NPC.TargetClosest(false);
                NPC.localAI[2] = 1;
            }

            EModeGlobalNPC.championBoss = NPC.whoAmI;

            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
            Vector2 targetPos;

            if (NPC.Distance(player.Center) < 1200)
                haveGottenInRange = true;

            noHurt = false;

            switch ((int)NPC.ai[0])
            {
                case 0: //just move
                    if (haveGottenInRange //despawn code
                        && (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > (FargoSoulsWorld.EternityMode ? 1600f : 2400f)))
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

                case 1: //laser rain
                    if (NPC.ai[1] == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (player.Center - NPC.Center) / 120,
                                ModContent.ProjectileType<TimberSquirrel>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.ai[3], NPC.whoAmI);
                    }

                    if (NPC.ai[1] < 60)
                    {
                        targetPos = player.Center;
                        targetPos.X += NPC.Center.X < player.Center.X ? -200 : 200;
                        targetPos.Y -= 200;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.2f, 24f);
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                    }

                    if (++NPC.ai[1] < 120)
                    {
                        if (NPC.ai[3] == 0)
                        {
                            if (NPC.ai[1] == 90) //telegraphs
                            {
                                NPC.velocity = Vector2.Zero;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 19);
                            }

                            if (NPC.ai[1] > 90 && NPC.ai[1] % 3 == 0) //glow line tells
                            {
                                float current = NPC.ai[1] - 90;
                                current /= 3;

                                float offset = 16 * 12 * current;
                                for (int i = -1; i <= 1; i += 2)
                                {
                                    Vector2 spawnPos = new Vector2(NPC.Center.X + offset * i, player.Center.Y + 1500);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, -Vector2.UnitY, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 19);
                                }
                            }
                        }
                    }
                    else if (NPC.ai[1] == 120)
                    {

                    }
                    else if (NPC.ai[1] < 270) //spam lasers everywhere
                    {
                        if (NPC.ai[3] == 0) //only if not flagged
                        {
                            if (NPC.ai[1] % 3 == 0)
                                SoundEngine.PlaySound(SoundID.Item157, NPC.Center);

                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 spawnPos = player.Center;
                                spawnPos.X += Main.rand.NextFloat(-1000, 1000);
                                spawnPos.Y -= Main.rand.NextFloat(600, 800);
                                Vector2 speed = Main.rand.NextFloat(7.5f, 12.5f) * Vector2.UnitY;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, speed, ModContent.ProjectileType<TimberLaser>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                                }
                            }
                        }
                    }
                    else if (!Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<TimberSquirrel>() && NPC.whoAmI == p.ai[1]))
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

                case 3: //shoot acorns
                    {
                        targetPos = player.Center;
                        targetPos.X += NPC.Center.X < player.Center.X ? -400 : 400;
                        targetPos.Y -= 100;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.25f, 32f);

                        NPC.ai[2]++;

                        const int snowballThreshold = 2;

                        if (NPC.ai[3] > snowballThreshold && FargoSoulsWorld.EternityMode) //snowball shotgun
                        {
                            bool feedbackFX = NPC.ai[2] == 1;

                            NPC.velocity *= 0.9f;

                            NPC.ai[1] -= 0.5f; //slower to proceed

                            if (NPC.ai[2] > 60)
                            {
                                NPC.ai[2] -= 15;

                                Vector2 vel = 20f * player.DirectionFrom(NPC.Center);
                                int max = (int)NPC.ai[3]++ - snowballThreshold; //more snowballs the more times its used
                                for (int i = -max; i <= max; i++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.RotatedBy(MathHelper.ToRadians(75) / max * i), ModContent.ProjectileType<TimberSnowball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1f);
                                }

                                feedbackFX = true;
                            }

                            if (feedbackFX)
                            {
                                SoundEngine.PlaySound(SoundID.Item36, NPC.Center); //shotgun sfx
                                SoundEngine.PlaySound(SoundID.Item11, NPC.Center); //snowball cannon sfx

                                for (int j = 0; j < 20; j++)
                                {
                                    int d = Dust.NewDust(NPC.Center, 0, 0, DustID.SnowBlock, Scale: 3f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 4f;
                                    Main.dust[d].velocity.X += NPC.direction * Main.rand.NextFloat(6f, 24f);
                                }
                            }
                        }
                        else
                        {
                            if (NPC.ai[2] > 35) //acorn
                            {
                                NPC.ai[2] = 0;
                                NPC.ai[3]++;
                                const float gravity = 0.2f;
                                float time = FargoSoulsWorld.MasochistModeReal ? 40f : 30f;
                                Vector2 distance = player.Center - NPC.Center;
                                if (FargoSoulsWorld.MasochistModeReal)
                                    distance.X += player.velocity.X * time;
                                distance.X = distance.X / time;
                                distance.Y = distance.Y / time - 0.5f * gravity * time;
                                for (int i = 0; i < 20; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 3,
                                        ModContent.ProjectileType<TimberAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (++NPC.ai[1] > 200)
                        {
                            NPC.TargetClosest();
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 4:
                    goto case 0;

                case 5: //trees that shoot acorns
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 300;
                    if (targetPos.Y > player.position.Y - 100)
                        targetPos.Y = player.position.Y - 100;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.1f, 12f);

                    if (--NPC.ai[2] < 0)
                    {
                        NPC.ai[2] = 70;

                        if (NPC.ai[1] < 300)
                        {
                            for (int i = 0; i < 5; i++) //spawn trees
                            {
                                Vector2 spawnPos = player.Center;
                                spawnPos.X += Main.rand.NextFloat(-1500, 1500) + player.velocity.X * 75;
                                spawnPos.Y -= Main.rand.NextFloat(300);
                                for (int j = 0; j < 100; j++) //go down until solid tile found
                                {
                                    Tile tile = Main.tile[(int)spawnPos.X / 16, (int)spawnPos.Y / 16];
                                    if (tile == null)
                                        tile = new Tile();
                                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                                        break;
                                    spawnPos.Y += 16;
                                }
                                for (int j = 0; j < 50; j++) //go up until non-solid tile found
                                {
                                    Tile tile = Main.tile[(int)spawnPos.X / 16, (int)spawnPos.Y / 16];
                                    if (tile == null)
                                        tile = new Tile();
                                    if (!(tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])))
                                        break;
                                    spawnPos.Y -= 16;
                                }

                                const float gravity = 0.2f;
                                float time = 90f;
                                Vector2 distance = spawnPos - NPC.Center;
                                distance.X = distance.X / time;
                                distance.Y = distance.Y / time - 0.5f * gravity * time;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance, ModContent.ProjectileType<TimberTreeAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 390)
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
                    if (!FargoSoulsWorld.MasochistModeReal)
                        NPC.ai[1] -= 0.5f; //more time to kill lesser squrrls
                    goto case 0;

                case 7: //chains
                    {
                        noHurt = true;

                        int noMoreChainsTime = 240;
                        int endlag = 60;

                        if (NPC.ai[1] < noMoreChainsTime)
                        {
                            NPC.position += (player.position - player.oldPosition) / 2;
                            if (FargoSoulsWorld.EternityMode)
                            {
                                targetPos = player.Center + 150 * NPC.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(10));
                                NPC.velocity = NPC.DirectionTo(targetPos) * NPC.velocity.Length();
                                Movement(targetPos, 0.25f, 24f);
                            }
                            else
                            {
                                targetPos = player.Center + 150 * NPC.DirectionFrom(player.Center);
                                Movement(targetPos, 0.25f, 24f);
                                if (NPC.Distance(player.Center) < 150)
                                    Movement(targetPos, 0.5f, 24f);
                            }

                            if (++NPC.ai[2] > 8)
                            {
                                NPC.ai[2] = 0;

                                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                                Vector2 speed = 32f * NPC.DirectionTo(player.Center).RotatedByRandom(Math.PI / 2);
                                float ai1 = noMoreChainsTime + endlag - NPC.ai[1];
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed,
                                        ModContent.ProjectileType<TimberHook2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, ai1);
                                }
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.9f;
                        }

                        if (++NPC.ai[1] > noMoreChainsTime + endlag)
                        {
                            NPC.TargetClosest();
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 8: //electrify chains
                    NPC.velocity = Vector2.Zero;

                    if (NPC.ai[1] == 0)
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    if (++NPC.ai[1] > 120)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 9:
                    goto case 0;

                case 10: //repeat squrrl attack but evil
                    NPC.ai[3] = 1;
                    goto case 1;

                case 11:
                    goto case 0;

                case 12:
                    goto case 3;

                case 13:
                    goto case 0;

                case 14: //noah snowballs or SQURRL MASSACRE
                    {
                        if (++NPC.ai[3] < 180) //ensure correct positioning
                        {
                            targetPos = player.Center;
                            targetPos.Y -= 200;
                            Movement(targetPos, 0.6f, 32f);
                            if (NPC.Center.Y - player.Center.Y < -200)
                            {
                                NPC.ai[3] = 180;
                                NPC.velocity *= 0.5f;
                            }
                            break;
                        }

                        int attackInterval = 6;

                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            targetPos = player.Center;
                            targetPos.X += player.velocity.X * 45f;
                            targetPos.Y -= 200;
                            Movement(targetPos, 0.5f, 32f);

                            if (++NPC.ai[2] > 5 && NPC.ai[1] < 420)
                            {
                                NPC.ai[2] = 0;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int damage = NPC.ai[1] > 120 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 4) : 0;
                                    for (int i = -2; i <= 2; i++)
                                    {
                                        Vector2 speed = new Vector2(5f * i, -20f);
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<TimberSnowball2>(), damage, 0f, Main.myPlayer, NPC.target, NPC.whoAmI);
                                    }
                                }
                            }

                            if (++NPC.ai[1] > 510)
                            {
                                NPC.TargetClosest();
                                NPC.ai[0]++;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                                NPC.ai[3] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else //kamikaze squrrl streams
                        {
                            targetPos = player.Center;
                            targetPos.X += NPC.Center.X < player.Center.X ? -200 : 200;
                            targetPos.Y -= 200;
                            Movement(targetPos, 0.25f, 32f);

                            NPC.velocity *= 0.9f;

                            NPC.ai[1]++;

                            int end = FargoSoulsWorld.MasochistModeReal ? 180 : 240;

                            if (NPC.ai[1] % attackInterval == 0)
                            {
                                Vector2 basePos = new Vector2(NPC.Center.X, Main.player[NPC.target].Center.Y);

                                const int max = 3;
                                for (int i = -max; i <= max; i++)
                                {
                                    int timeFactor = Math.Abs(i) + 1; //delayed appearance, outers start faster
                                    if (NPC.ai[1] * timeFactor < 60)
                                        continue;

                                    Vector2 target = basePos;

                                    if (i != 0)
                                    {
                                        target.X += 16 * 15 * i; //final spacing at end of attack

                                        float ratio = 1f - NPC.ai[1] / end;
                                        target.X += 1200f * i / 2 * ratio;
                                    }

                                    ShootSquirrelAt(target); //streams start aiming far out, then move inwards
                                }
                            }

                            if (NPC.ai[1] > end)
                            {
                                if (NPC.ai[1] > end + attackInterval)
                                    NPC.ai[1] -= attackInterval;

                                if (++NPC.ai[2] > 60)
                                {
                                    NPC.TargetClosest();
                                    NPC.ai[0]++;
                                    NPC.ai[1] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.ai[3] = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
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

        private void ShootSquirrelAt(Vector2 target)
        {
            float gravity = 0.6f;
            const float origTime = 75;
            float time = 60;
            if (FargoSoulsWorld.MasochistModeReal)
                time /= 2;

            gravity *= origTime / time;

            Vector2 distance = target - NPC.Center;
            distance.X += Main.rand.NextFloat(-32, 32);
            distance.X = distance.X / time;
            distance.Y = distance.Y / time - 0.5f * gravity * time;

            SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float ai1 = time + Main.rand.Next(-10, 11) - 1;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance,
                    ModContent.ProjectileType<TrojanSquirrelProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, gravity, ai1);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 3)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    NPC.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Guilty>(), 600);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, "TimberGore1").Type, NPC.scale);
                pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, "TimberGore2").Type, NPC.scale);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TimberChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(TimberForce.Enchants));
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            spriteEffects = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D15 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/TimberChampionHead_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = Color.Red * NPC.Opacity;
            color26.A = 20;

            for (float i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i += 0.25f)
            {
                Color color27 = color26 * 0.4f;
                float fade = (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = NPC.rotation;
                Vector2 center = Vector2.Lerp(NPC.oldPos[(int)i], NPC.oldPos[max0], 1 - i % 1);
                center += NPC.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(texture2D15, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
