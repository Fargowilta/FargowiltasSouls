using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
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
    public class LifeChampion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Life");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "生命英灵");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Chilled,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>(),
                }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.5f,
                PortraitScale = 0.5f
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                return NPC.GetBestiaryEntryColor();
            }
            return base.GetAlpha(drawColor);
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 130;
            NPC.damage = 160;
            NPC.defense = 0;
            NPC.lifeMax = 35000;
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

            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (NPC.localAI[3] == 0 || ((NPC.ai[0] == 2 || NPC.ai[0] == 8) && NPC.ai[3] == 0))
                return false;

            CooldownSlot = 1;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            EModeGlobalNPC.championBoss = NPC.whoAmI;

            if (NPC.localAI[3] == 0) //just spawned
            {
                if (!NPC.HasValidTarget)
                    NPC.TargetClosest(false);

                if (NPC.ai[2] < 0.1f)
                    NPC.Center = Main.player[NPC.target].Center - Vector2.UnitY * 300;

                NPC.ai[2] += 1f / 180f;

                NPC.alpha = (int)(255f * (1 - NPC.ai[2]));
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
                if (NPC.alpha > 255)
                    NPC.alpha = 255;

                if (NPC.ai[2] > 1f)
                {
                    NPC.localAI[3] = 1;
                    NPC.ai[2] = 0;
                    NPC.netUpdate = true;

                    NPC.velocity = -20f * Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2);

                    SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center); //arte scream

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, -1, -4);

                        if (FargoSoulsWorld.EternityMode)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LifeRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                    }
                }
                return;
            }

            NPC.dontTakeDamage = false;
            NPC.alpha = 0;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (NPC.HasValidTarget && NPC.Distance(player.Center) < 2500)
                NPC.timeLeft = 600;

            switch ((int)NPC.ai[0])
            {
                case -3: //final phase
                    if (!Main.dayTime || !player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y -= 1f;

                        break;
                    }

                    NPC.velocity = Vector2.Zero;

                    NPC.ai[1] -= (float)Math.PI * 2 / 447;
                    NPC.ai[3] += (float)Math.PI * 2 / 447; //spin deathrays both ways

                    if (--NPC.ai[2] < 0)
                    {
                        NPC.localAI[1] = NPC.localAI[1] == 0 ? 1 : 0;
                        NPC.ai[2] = NPC.localAI[1] == 1 ? 90 : 30;

                        if (NPC.ai[1] < 360 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = NPC.localAI[1] == 1 ? ModContent.ProjectileType<LifeDeathraySmall2>() : ModContent.ProjectileType<LifeDeathray2>();
                            int max = 3;
                            for (int i = 0; i < max; i++)
                            {
                                float offset = (float)Math.PI * 2 / max * i;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3] + offset),
                                    type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, (float)Math.PI * 2 / 447, NPC.whoAmI);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[1] + offset),
                                    type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, -(float)Math.PI * 2 / 447, NPC.whoAmI);
                            }
                        }
                    }

                    if (--NPC.localAI[0] < 0)
                    {
                        NPC.localAI[0] = 47;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = 14;
                            float rotation = Main.rand.NextFloat((float)Math.PI * 2);
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(4f, 0).RotatedBy(rotation + Math.PI / max * 2 * i),
                                    ModContent.ProjectileType<ChampionBee>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }
                    break;

                case -2: //final phase transition
                    NPC.velocity *= 0.97f;

                    if (NPC.ai[1] > 180)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[2] = 2;
                    }

                    if (++NPC.ai[1] == 180) //heal up
                    {
                        SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center); //arte scream

                        int heal = NPC.lifeMax / 3 - NPC.life;
                        NPC.life += heal;
                        CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -4);
                    }
                    else if (NPC.ai[1] > 240)
                    {
                        NPC.ai[0] = -3;
                        NPC.ai[1] = NPC.DirectionTo(player.Center).ToRotation();
                        NPC.ai[2] = 0;
                        NPC.ai[3] = NPC.DirectionTo(player.Center).ToRotation();
                        NPC.netUpdate = true;
                    }
                    break;

                case -1: //heal
                    NPC.velocity *= 0.97f;

                    if (NPC.ai[1] > 180)
                        NPC.localAI[2] = 1;

                    if (++NPC.ai[1] == 180) //heal up
                    {
                        SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center); //arte scream

                        int heal = NPC.lifeMax - NPC.life;
                        NPC.life += heal;
                        CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -4);
                    }
                    else if (NPC.ai[1] > 240)
                    {
                        NPC.ai[0] = NPC.ai[3];
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 0: //float over player
                    if (!Main.dayTime || !player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f) //despawn code
                    {
                        NPC.TargetClosest(false);
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;

                        NPC.noTileCollide = true;
                        NPC.noGravity = true;
                        NPC.velocity.Y -= 1f;

                        break;
                    }

                    targetPos = player.Center;
                    targetPos.Y -= 300;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.18f, 24f, true);
                    if (NPC.Distance(player.Center) < 200) //try to avoid contact damage
                        Movement(targetPos, 0.24f, 24f, true);

                    if (++NPC.ai[1] > 150)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[2] == 0 && NPC.life < NPC.lifeMax / 3)
                    {
                        float buffer = NPC.ai[0];
                        NPC.ai[0] = -1;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = buffer;
                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[2] == 1 && NPC.life < NPC.lifeMax / 3 && FargoSoulsWorld.EternityMode)
                    {
                        NPC.ai[0] = -2;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 1: //boundary
                    NPC.velocity *= 0.95f;
                    if (++NPC.ai[1] > (NPC.localAI[2] == 1 ? 2 : 3))
                    {
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        NPC.ai[1] = 0;
                        NPC.ai[2] -= (float)Math.PI / 4 / 457 * NPC.ai[3];
                        if (NPC.ai[2] < -(float)Math.PI)
                            NPC.ai[2] += (float)Math.PI * 2;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = NPC.localAI[2] == 1 ? 4 : 3;
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(6f, 0).RotatedBy(NPC.ai[2] + Math.PI / max * 2 * i),
                                    ModContent.ProjectileType<ChampionBee>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }
                    if (++NPC.ai[3] > 300)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2: //dash attack
                    if (NPC.ai[3] == 0)
                    {
                        if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 2500f) //despawn code
                        {
                            NPC.TargetClosest(false);
                            if (NPC.timeLeft > 30)
                                NPC.timeLeft = 30;

                            NPC.noTileCollide = true;
                            NPC.noGravity = true;
                            NPC.velocity.Y -= 1f;

                            return;
                        }

                        if (NPC.ai[2] == 0)
                        {
                            NPC.ai[2] = NPC.Center.Y; //store arena height

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 7, NPC.whoAmI);
                        }

                        if (NPC.Center.Y > NPC.ai[2] + 1000) //now below arena, track player
                        {
                            targetPos = new Vector2(player.Center.X, NPC.ai[2] + 1100);
                            Movement(targetPos, 1.2f, 24f);

                            if (Math.Abs(player.Center.X - NPC.Center.X) < NPC.width / 2
                                && ++NPC.ai[1] > (NPC.localAI[2] == 1 ? 30 : 60)) //in position under player
                            {
                                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                                NPC.ai[3]++;
                                NPC.ai[1] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else //drop below arena
                        {
                            NPC.velocity.X *= 0.95f;
                            NPC.velocity.Y += 0.6f;
                        }
                    }
                    else
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = -36f;

                        if (++NPC.ai[1] > 1) //spawn pixies
                        {
                            NPC.ai[1] = 0;
                            NPC.localAI[0] = NPC.localAI[0] == 1 ? -1 : 1; //alternate sides

                            Vector2 velocity = 5f * Vector2.UnitX.RotatedBy(Math.PI * (Main.rand.NextDouble() - 0.5));
                            velocity.X *= NPC.localAI[0];

                            FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center,
                                ModContent.NPCType<LesserFairy>(), NPC.whoAmI,
                                target: NPC.target, velocity: velocity);
                        }

                        if (NPC.Center.Y < player.Center.Y - 600) //dash ended
                        {
                            NPC.velocity.Y *= -0.25f;
                            NPC.localAI[0] = 0f;

                            NPC.TargetClosest();
                            NPC.ai[0]++;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 3:
                    goto case 0;

                case 4: //beetle swarm
                    NPC.velocity *= 0.9f;

                    if (NPC.ai[3] == 0)
                        NPC.ai[3] = NPC.Center.X < player.Center.X ? -1 : 1;

                    if (++NPC.ai[2] > (NPC.localAI[2] == 1 ? 40 : 60))
                    {
                        NPC.ai[2] = 0;
                        SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                        if (NPC.localAI[0] > 0)
                            NPC.localAI[0] = -1;
                        else
                            NPC.localAI[0] = 1;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 projTarget = NPC.Center;
                            projTarget.X += 1200 * NPC.ai[3];
                            projTarget.Y += 1200 * -NPC.localAI[0];
                            int max = NPC.localAI[2] == 1 ? 30 : 20;
                            int increment = NPC.localAI[2] == 1 ? 180 : 250;
                            projTarget.Y += Main.rand.NextFloat(increment);
                            for (int i = 0; i < max; i++)
                            {
                                projTarget.Y += increment * NPC.localAI[0];
                                Vector2 speed = (projTarget - NPC.Center) / 40;
                                float ai0 = (NPC.localAI[2] == 1 ? 8 : 6) * -NPC.ai[3]; //x speed of beetles
                                float ai1 = 6 * -NPC.localAI[0]; //y speed of beetles
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<ChampionBeetle>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, ai0, ai1);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 440)
                    {
                        NPC.localAI[0] = 0;

                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 5:
                    goto case 0;

                case 6:
                    NPC.velocity *= 0.98f;

                    if (++NPC.ai[2] > (NPC.localAI[2] == 1 ? 45 : 60))
                    {
                        if (++NPC.ai[3] > (NPC.localAI[2] == 1 ? 4 : 7)) //spray fireballs that home down
                        {
                            NPC.ai[3] = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                //spawn anywhere above self
                                Vector2 target = new Vector2(Main.rand.NextFloat(1000), 0).RotatedBy(Main.rand.NextDouble() * -Math.PI);
                                Vector2 speed = 2 * target / 60;
                                float acceleration = -speed.Length() / 60;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<LifeFireball>(),
                                    FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 60f, acceleration);
                            }
                        }

                        if (NPC.ai[2] > (NPC.localAI[2] == 1 ? 120 : 100))
                        {
                            NPC.netUpdate = true;
                            NPC.ai[2] = 0;
                        }
                    }

                    if (++NPC.ai[1] > 480)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 7:
                    goto case 0;

                case 8:
                    goto case 2;

                case 9: //deathray spin
                    NPC.velocity *= 0.95f;

                    NPC.ai[3] += (float)Math.PI * 2 / (NPC.localAI[2] == 1 ? -300 : 360);

                    if (--NPC.ai[2] < 0)
                    {
                        NPC.ai[2] = 59;
                        if (NPC.ai[1] > 90) //longer telegraph on first attack
                            NPC.localAI[1] = NPC.localAI[1] == 0 ? 1 : 0;

                        if (NPC.ai[1] < 420 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = NPC.localAI[1] == 0 ? ModContent.ProjectileType<LifeDeathraySmall>() : ModContent.ProjectileType<LifeDeathray>();
                            int max = NPC.localAI[2] == 1 ? 6 : 4;
                            for (int i = 0; i < max; i++)
                            {
                                float offset = (float)Math.PI * 2 / max * i;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3] + offset),
                                    type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, offset, NPC.whoAmI);
                            }
                        }
                    }

                    if (++NPC.ai[1] > 450)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.localAI[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 10:
                    goto case 0;

                case 11: //cactus mines
                    NPC.velocity *= 0.98f;

                    if (++NPC.ai[2] > (NPC.localAI[2] == 1 ? 75 : 100))
                    {
                        if (++NPC.ai[3] > 5) //spray mines that home down
                        {
                            NPC.ai[3] = 0;

                            SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 target = player.Center - NPC.Center;
                                target.X += Main.rand.Next(-75, 76);
                                target.Y += Main.rand.Next(-75, 76);

                                Vector2 speed = 2 * target / 90;
                                float acceleration = -speed.Length() / 90;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<CactusMine>(),
                                    FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0f, acceleration);
                            }
                        }

                        if (NPC.ai[2] > 130)
                        {
                            NPC.netUpdate = true;
                            NPC.ai[2] = 0;
                        }
                    }

                    if (++NPC.ai[1] > 480)
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

            for (int i = 0; i < 3; i++)
            {
                Vector2 origin = NPC.Center - new Vector2(300, 200) * NPC.scale;
                int d = Dust.NewDust(origin, (int)(600 * NPC.scale), (int)(400 * NPC.scale), 87, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }

            //NPC.rotation += (float)Math.PI * 2 / 90;

            if (NPC.velocity.Length() > 1f && NPC.ai[0] != 2 && NPC.ai[0] != 8 && NPC.HasValidTarget)
                NPC.position.Y += player.velocity.Y / 3f;
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (NPC.lifeRegen < 0)
                NPC.lifeRegen /= 2;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.frame.Y = 0;
                }

                if (!NPC.IsABestiaryIconDummy)
                    NPC.rotation = MathHelper.WrapAngle(NPC.rotation + MathHelper.TwoPi / Main.npcFrameCount[NPC.type] / 2f);
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

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 10;
            if ((NPC.localAI[2] == 0 && NPC.life < NPC.lifeMax / 3)
                || (NPC.localAI[2] == 1 && NPC.life < NPC.lifeMax / 3 && FargoSoulsWorld.EternityMode))
            {
                damage = 1;
                crit = false;
                return false;
            }
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Purified>(), 300);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"LifeGore{i}").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.LifeChampion], -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new ChampionEnchDropRule(LifeForce.Enchants));
            
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<LifeChampionRelic>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Dyes.LifeDye>()));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.alpha != 0 && !NPC.IsABestiaryIconDummy) //proceed anyway for bestiary
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.IsABestiaryIconDummy ? Color.White : NPC.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None; /*NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }*/

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            int currentFrame = NPC.frame.Y / (texture2D13.Height / Main.npcFrameCount[NPC.type]);
            Texture2D wing = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/LifeChampion_Wings", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D wingGlow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/LifeChampion_WingsGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int wingHeight = wing.Height / Main.npcFrameCount[NPC.type];
            Rectangle wingRectangle = new Rectangle(0, currentFrame * wingHeight, wing.Width, wingHeight);
            Vector2 wingOrigin = wingRectangle.Size() / 2f;

            Color glowColor = Color.White;
            if (!NPC.IsABestiaryIconDummy)
                glowColor *= NPC.Opacity;
            float wingBackScale = 2 * NPC.scale * ((Main.mouseTextColor / 200f - 0.35f) * 0.1f + 0.95f);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                float num165 = 0; //NPC.oldRot[i];
                DrawData wingTrailGlow = new DrawData(wing, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(wingRectangle), glowColor * (0.5f / i), num165, wingOrigin, wingBackScale, effects, 0);
                GameShaders.Misc["LCWingShader"].UseColor(new Color(1f, 0.647f, 0.839f)).UseSecondaryColor(Color.CornflowerBlue);
                GameShaders.Misc["LCWingShader"].Apply(wingTrailGlow);
                wingTrailGlow.Draw(spriteBatch);
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            Main.EntitySpriteDraw(wing, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(wingRectangle), glowColor, 0, wingOrigin, NPC.scale * 2, effects, 0);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            DrawData wingGlowData = new DrawData(wingGlow, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(wingRectangle), glowColor * 0.5f, 0, wingOrigin, NPC.scale * 2, effects, 0);
            GameShaders.Misc["LCWingShader"].UseColor(new Color(1f, 0.647f, 0.839f)).UseSecondaryColor(Color.Goldenrod);
            GameShaders.Misc["LCWingShader"].Apply(wingGlowData);
            wingGlowData.Draw(spriteBatch);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[0] == 9 && NPC.ai[1] < 420)
                return;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            Texture2D star = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rect = new Rectangle(0, 0, star.Width, star.Height);
            float scale = NPC.localAI[3] == 0 ? NPC.ai[2] * Main.rand.NextFloat(1f, 2.5f) : (Main.cursorScale + 0.3f) * Main.rand.NextFloat(0.8f, 1.2f);
            Vector2 origin = new Vector2((star.Width / 2) + scale, (star.Height / 2) + scale);

            spriteBatch.Draw(star, NPC.Center - screenPos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
            DrawData starDraw = new DrawData(star, NPC.Center - screenPos, new Rectangle?(rect), Color.White, 0, origin, scale, SpriteEffects.None, 0);
            GameShaders.Misc["LCWingShader"].UseColor(Color.Goldenrod).UseSecondaryColor(Color.HotPink);
            GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
            starDraw.Draw(spriteBatch);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }
}
