using Fargowiltas.NPCs;
using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Pets;
using FargowiltasSouls.Content.Items.Placables.Relics;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Patreon.Phupperbat;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    [AutoloadBossHead]
    public class DeviBoss : ModNPC
    {
        #region Fields
        public bool playerInvulTriggered;
        private bool droppedSummon = false;

        public int[] attackQueue = new int[4];
        public int lastStrongAttack;
        public bool ignoreMoney;

        public int ringProj, spriteProj;

        public bool DrawRuneBorders;

        string TownNPCName;

        public ref float AttackIndex => ref NPC.localAI[2];
        public ref float Phase => ref NPC.localAI[3];
        public ref float State => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float SubTimer => ref NPC.ai[2];

        public enum DevianttAttackTypes
        {
            Die = -2,
            Phase2Transition = -1,
            SpawnEffects,
            PaladinHammers,
            HeartBarrages,
            WyvernOrbSpiral,
            Mimics,
            FrostballsNados,
            RuneWizard,
            MothDustCharges,
            WhileDashing,
            MageSkeletonAttacks,
            BabyGuardians,
            GeyserRain,
            CrossRayHearts,
            Butterflies,
            MedusaRay,
            SparklingLove,
            Pause,
            Bribery
        }
        #endregion
        #region Standard Methods
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    BuffID.Lovestruck,
                    ModContent.BuffType<LethargicBuff>(),
                    ModContent.BuffType<ClippedWingsBuff>()
            ]);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasSouls.Bestiary.DeviBoss")
            ]);
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            if (Main.getGoodWorld)
            {
                NPC.width = Player.defaultWidth;
                NPC.height = Player.defaultHeight;
            }
            NPC.damage = 64;
            NPC.defense = 10;
            NPC.lifeMax = 6000;
            if (WorldSavingSystem.EternityMode)
            {
                NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.5f);

                if (!Main.masterMode) //master mode is already long enough
                    NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.4f);
            }
            NPC.HitSound = SoundID.NPCHit9;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 50f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
            NPC.timeLeft = NPC.activeTime * 30;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, (musicMod.Version >= Version.Parse("0.1.4")) ? "Assets/Music/Strawberry_Sparkly_Sunrise" : "Assets/Music/LexusCyanixs") : MusicID.OtherworldlyHallow;
            SceneEffectPriority = SceneEffectPriority.BossMedium;

            NPC.value = Item.buyPrice(0, 5);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax /** 0.5f*/ * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(AttackIndex);
            writer.Write(Phase);
            writer.Write7BitEncodedInt(attackQueue[0]);
            writer.Write7BitEncodedInt(attackQueue[1]);
            writer.Write7BitEncodedInt(attackQueue[2]);
            writer.Write7BitEncodedInt(attackQueue[3]);
            writer.Write(ignoreMoney);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            AttackIndex = reader.ReadSingle();
            Phase = reader.ReadSingle();
            attackQueue[0] = reader.Read7BitEncodedInt();
            attackQueue[1] = reader.Read7BitEncodedInt();
            attackQueue[2] = reader.Read7BitEncodedInt();
            attackQueue[3] = reader.Read7BitEncodedInt();
            ignoreMoney = reader.ReadBoolean();
        }

        public override void OnSpawn(IEntitySource source)
        {
            int n = NPC.FindFirstNPC(ModContent.NPCType<Deviantt>());
            if (n != -1 && n != Main.maxNPCs)
            {
                NPC.Bottom = Main.npc[n].Bottom;
                TownNPCName = Main.npc[n].GivenName;

                Main.npc[n].life = 0;
                Main.npc[n].active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }
        }
        #endregion
        #region AI
        public override void AI()
        {
            EModeGlobalNPC.deviBoss = NPC.whoAmI;

            const int platinumToBribe = 10;

            if (Phase == 0)
            {
                NPC.TargetClosest();
                if (NPC.timeLeft < 30)
                    NPC.timeLeft = 30;

                if (NPC.Distance(Main.player[NPC.target].Center) < 2000)
                {
                    Phase = 1;
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    do
                    {
                        RefreshAttackQueue();
                    } while (attackQueue[0] == 3 || attackQueue[0] == 5 || attackQueue[0] == 9 || attackQueue[0] == 10);
                    //don't start with wyvern, mage spam, frostballs, baby guardian
                }
            }
            /*else if (Phase == 1)
            {
                Aura(2000f, ModContent.BuffType<GodEater>(), true, 86);
            }*/
            /*else if (Main.player[Main.myPlayer].active && NPC.Distance(Main.player[Main.myPlayer].Center) < 3000f)
            {
                if (WorldSavingSystem.MasochistMode)
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<DeviPresence>(), 2);
            }*/

            if (FargoSoulsUtil.HostCheck)
            {
                if (!ProjectileExists(ringProj, ModContent.ProjectileType<DeviRitual2>()))
                    ringProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual2>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                if (!ProjectileExists(spriteProj, ModContent.ProjectileType<DeviBossProjectile>()))
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        int number = 0;
                        for (int index = 999; index >= 0; --index)
                        {
                            if (!Main.projectile[index].active)
                            {
                                number = index;
                                break;
                            }
                        }
                        if (number >= 0)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Projectile projectile = Main.projectile[number];
                                projectile.SetDefaults(ModContent.ProjectileType<DeviBossProjectile>());
                                projectile.Center = NPC.Center;
                                projectile.owner = Main.myPlayer;
                                projectile.velocity.X = 0;
                                projectile.velocity.Y = 0;
                                projectile.damage = 0;
                                projectile.knockBack = 0f;
                                projectile.identity = number;
                                projectile.gfxOffY = 0f;
                                projectile.stepSpeed = 1f;
                                projectile.ai[1] = NPC.whoAmI;

                                spriteProj = number;
                            }
                        }
                    }
                    else //server
                    {
                        spriteProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviBossProjectile>(), 0, 0f, Main.myPlayer, 0, NPC.whoAmI);
                    }
                }
            }

            int projectileDamage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, Phase > 1 ? 1 : 0.8f);

            Player player = Main.player[NPC.target];
            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;
            Vector2 targetPos;

            void StrongAttackTeleport(Vector2 teleportTarget = default)
            {
                const float range = 450f;
                if (teleportTarget == default ? NPC.Distance(player.Center) < range : NPC.Distance(teleportTarget) < 80)
                    return;

                TeleportDust();
                if (FargoSoulsUtil.HostCheck)
                {
                    if (teleportTarget != default)
                        NPC.Center = teleportTarget;
                    else if (player.velocity == Vector2.Zero)
                        NPC.Center = player.Center + range * Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                    else
                        NPC.Center = player.Center + range * Vector2.Normalize(player.velocity);
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                TeleportDust();
                SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
            };

            // Set this to false, it will be set to true below if needed.
            DrawRuneBorders = false;

            switch ((DevianttAttackTypes)State)
            {
                case DevianttAttackTypes.Die: //ACTUALLY dead
                    if (!AliveCheck(player))
                        break;
                    Die();
                    break;

                case DevianttAttackTypes.Phase2Transition: //phase 2 transition
                    Phase2Transition();
                    break;

                case DevianttAttackTypes.SpawnEffects: //track player, decide which attacks to use
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    SpawnEffects();
                    break;

                case DevianttAttackTypes.PaladinHammers: //teleport marx hammers
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    PaladinHammers();
                    break;

                case DevianttAttackTypes.HeartBarrages: //heart barrages
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    HeartBarrages();
                    break;

                case DevianttAttackTypes.WyvernOrbSpiral: //slow while shooting wyvern orb spirals
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    WyvernOrbSpiral();
                    break;

                case DevianttAttackTypes.Mimics: //mimics
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    Mimics();
                    break;

                case DevianttAttackTypes.FrostballsNados: //frostballs and nados
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    FrostballsNados();
                    break;

                case DevianttAttackTypes.RuneWizard: //rune wizard
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    RuneWizard();
                    break;

                case DevianttAttackTypes.MothDustCharges: //moth dust charges
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    MothDustCharges();
                    break;

                case DevianttAttackTypes.WhileDashing: //while dashing
                    if (Phase2Check())
                        break;
                    WhileDashing();
                    break;

                case DevianttAttackTypes.MageSkeletonAttacks: //mage skeleton attacks
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    MageSkeletonAttacks();
                    break;

                case DevianttAttackTypes.BabyGuardians: //baby guardians
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    BabyGuardians();
                    break;

                case DevianttAttackTypes.GeyserRain: //noah/irisu geyser rain
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    GeyserRain();
                    break;

                case DevianttAttackTypes.CrossRayHearts: //lilith cross ray hearts
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    CrossRayHearts();
                    break;

                case DevianttAttackTypes.Butterflies: //that one boss that was a bunch of gems burst rain but with butterflies
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    Butterflies();
                    break;

                case DevianttAttackTypes.MedusaRay: //medusa ray
                    if (Timer < 420 && !AliveCheck(player) || Phase2Check())
                        break;
                    MedusaRay();
                    break;

                case DevianttAttackTypes.SparklingLove: //sparkling love
                    SparklingLove();
                    break;

                case DevianttAttackTypes.Pause: //pause between attacks
                    if (!AliveCheck(player) || Phase2Check())
                        break;
                    Pause();
                    break;

                case DevianttAttackTypes.Bribery: //i got money
                    Bribery();
                    break;

                default:
                    Main.NewText("UH OH, STINKY");
                    NPC.netUpdate = true;
                    State = 0;
                    goto case 0;
            }

            if (player.immune || player.hurtCooldowns[0] != 0 || player.hurtCooldowns[1] != 0)
                playerInvulTriggered = true;

            //drop summon
            if (WorldSavingSystem.EternityMode && !WorldSavingSystem.DownedDevi && FargoSoulsUtil.HostCheck && NPC.HasPlayerTarget && !droppedSummon)
            {
                Item.NewItem(NPC.GetSource_Loot(), player.Hitbox, ModContent.ItemType<DevisCurse>());
                droppedSummon = true;
            }

            #region STATES
            void Die()
            {

                NPC.velocity *= 0.9f;
                NPC.dontTakeDamage = true;
                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemAmethyst, 0f, 0f, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 12f;
                }
                if (++Timer > 180)
                {
                    int deviType = ModContent.NPCType<Deviantt>();
                    if (FargoSoulsUtil.HostCheck && !NPC.AnyNPCs(deviType))
                    {
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, deviType);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].homeless = true;
                            if (TownNPCName != default)
                                Main.npc[n].GivenName = TownNPCName;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    NPC.life = 0;
                    NPC.dontTakeDamage = false;
                    NPC.checkDead();
                }
            }
            void Phase2Transition()
            {
                NPC.velocity *= 0.9f;
                NPC.dontTakeDamage = true;
                if (NPC.buffType[0] != 0)
                    NPC.DelBuff(0);
                if (++Timer > 120)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemAmethyst, 0f, 0f, 0, default, 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 8f;
                    }
                    Phase = 2; //npc marks p2
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        int heal = (int)(NPC.lifeMax / 90 * Main.rand.NextFloat(1f, 1.5f));
                        NPC.life += heal;
                        if (NPC.life > NPC.lifeMax)
                            NPC.life = NPC.lifeMax;
                        CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);
                    }
                    if (Timer > 240)
                    {
                        RefreshAttackQueue();
                        attackQueue[3] = 15; //always do sparkling love
                        AttackIndex = Phase > 1 ? 1 : 0;
                        GetNextAttack();
                    }
                }
                else if (Timer == 120)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }
            }
            void SpawnEffects()
            {
                NPC.dontTakeDamage = false;

                targetPos = player.Center;
                targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, Phase > 0 ? 0.15f : 2f, Phase > 0 ? 12f : 1200f);

                if (Phase > 0) //in range, fight has begun, choose attacks
                {
                    NPC.netUpdate = true;
                    GetNextAttack();
                }
            }
            void PaladinHammers()
            {
                ref float TeleportCount = ref NPC.localAI[1]; // amount of teleports to do before each hammer
                ref float HammerCounter = ref NPC.localAI[0]; // amount of hammer attacks she's done

                if (TeleportCount == 0) //pick random number of teleports to do
                {
                    TeleportCount = Phase > 1 ? Main.rand.Next(3, 10) : Main.rand.Next(3, 6);
                    if (HammerCounter > 0)
                        TeleportCount = Main.rand.NextBool() ? 2 : 3;
                    NPC.netUpdate = true;
                }

                NPC.velocity = Vector2.Zero;
                if (++Timer > (Phase > 1 ? 10 : 20) && SubTimer < TeleportCount)
                {
                    //TeleportCount = 0;
                    Timer = 0;
                    SubTimer++;

                    TeleportDust();
                    if (FargoSoulsUtil.HostCheck)
                    {
                        bool wasOnLeft = NPC.Center.X < player.Center.X;
                        NPC.Center = player.Center + 200 * Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0, 2 * (float)Math.PI));
                        if (wasOnLeft ? NPC.Center.X < player.Center.X : NPC.Center.X > player.Center.X)
                        {
                            float x = player.Center.X - NPC.Center.X;
                            NPC.position.X += x * 2;
                        }
                        NPC.netUpdate = true;
                    }
                    TeleportDust();
                    SoundEngine.PlaySound(SoundID.Item84, NPC.Center);

                    if (SubTimer == TeleportCount)
                    {
                        if (FargoSoulsUtil.HostCheck) //hold out hammers for visual display
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    float ai1 = MathHelper.ToRadians(90 + 15) - MathHelper.ToRadians(30) * j;
                                    ai1 *= i;
                                    ai1 = ai1 / 60 * 2;
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<DeviHammerHeld>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI, ai1);
                                }
                            }
                        }
                    }
                }

                if (Timer == 60) //finished all the prior teleports, now attack
                {
                    NPC.netUpdate = true;

                    FargoSoulsUtil.DustRing(NPC.Center, 36, 246, 9f, default, 3f, true);

                    SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                    if (FargoSoulsUtil.HostCheck) //hammers
                    {
                        void SpawnHammers(float rad, float angleOffset)
                        {
                            const int time = 45;
                            float speed = 2 * (float)Math.PI * rad / time;
                            float acc = speed * speed / rad * NPC.direction;

                            for (int i = 0; i < 4; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i + angleOffset) * speed, ModContent.ProjectileType<DeviHammer>(), projectileDamage, 0f, Main.myPlayer, acc, time);
                        };

                        SpawnHammers(100, MathHelper.PiOver4);
                        SpawnHammers(150, 0);
                        if (WorldSavingSystem.EternityMode)
                            SpawnHammers(200, MathHelper.PiOver4);
                        if (WorldSavingSystem.MasochistModeReal)
                            SpawnHammers(300, 0);
                    }
                }
                else if (Timer > 90)
                {
                    NPC.netUpdate = true;
                    if (Phase > 1 && ++HammerCounter < 3)
                    {
                        SubTimer = 0; //reset tp counter and attack again
                        TeleportCount = 0;
                    }
                    else
                    {
                        GetNextAttack();
                    }
                }
            }
            void HeartBarrages()
            {
                ref float FirstFrameCheck = ref NPC.localAI[0];

                targetPos = player.Center;
                targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.2f);

                if (FirstFrameCheck == 0)
                {
                    FirstFrameCheck = 1;
                    Vector2 teleportTarget = new(player.Center.X, NPC.Center.Y);
                    teleportTarget.X += NPC.Center.X < teleportTarget.X ? -450 : 450;
                    StrongAttackTeleport(teleportTarget);
                }

                if (--Timer < 0)
                {
                    NPC.netUpdate = true;
                    Timer = 75;
                    if (++SubTimer > 3)
                    {
                        GetNextAttack();
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.Item43, NPC.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int damage = (int)(NPC.damage / 3.2); //comes out to 20 raw, fake hearts ignore the usual multipliers

                            Vector2 spawnVel = NPC.DirectionFrom(Main.player[NPC.target].Center) * 10f;

                            int boost = 0;
                            if (WorldSavingSystem.EternityMode)
                                boost += 1;
                            if (WorldSavingSystem.MasochistModeReal)
                                boost += 3;

                            int maxP1 = 3 + boost;
                            for (int i = -maxP1; i <= maxP1; i++)
                            {
                                float ai1 = 30;
                                if (WorldSavingSystem.MasochistModeReal)
                                    ai1 += 3 * Math.Abs(i);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, spawnVel.RotatedBy(Math.PI / 7 * i),
                                    ModContent.ProjectileType<FakeHeart2>(), damage, 0f, Main.myPlayer, 20, ai1);
                            }
                            if (Phase > 1)
                            {
                                int maxP2 = 5 + boost;
                                for (int i = -maxP2; i <= maxP2; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 1.5f * spawnVel.RotatedBy(Math.PI / 10 * i),
                                        ModContent.ProjectileType<FakeHeart2>(), damage, 0f, Main.myPlayer, 20, 40 + 5 * Math.Abs(i));
                                }
                            }
                        }
                    }
                }
            }
            void WyvernOrbSpiral()
            {
                ref float Direction = ref NPC.ai[3];

                if (Direction == 0)
                    Direction = Main.rand.NextBool() ? 1 : -1;

                targetPos = player.Center + (player.SafeDirectionTo(NPC.Center) * 375).RotatedBy(Direction * MathHelper.PiOver2 / 10f);
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.15f);

                if (--Timer < 0)
                {
                    NPC.netUpdate = true;
                    Timer = WorldSavingSystem.EternityMode ? 120 : 150;
                    Direction = Main.rand.NextBool() ? 1 : -1;

                    int repeats = 3;
                    if (++SubTimer > repeats)
                    {
                        GetNextAttack();
                    }
                    else
                    {
                        if (FargoSoulsUtil.HostCheck)
                        {
                            int max = Phase > 1 ? 8 : 12;
                            Vector2 vel = Vector2.Normalize(NPC.velocity);
                            if (WorldSavingSystem.MasochistModeReal)
                                vel *= 0.75f;
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviLightBall>(), projectileDamage, 0f, Main.myPlayer, 0f, .008f * NPC.direction);
                                if (Phase > 1)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviLightBall>(), projectileDamage, 0f, Main.myPlayer, 0f, .008f * -NPC.direction);
                            }
                        }
                    }
                }
            }
            void Mimics()
            {
                targetPos = player.Center;
                targetPos.X += 300 * (NPC.Center.X < targetPos.X ? -1 : 1);
                targetPos.Y -= 300;
                float speedMod = Timer < 120 ? 0.3f : 0.15f;
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, speedMod);

                if (++Timer < 120)
                {
                    if (++SubTimer > 20)
                    {
                        SubTimer = 0;

                        SoundEngine.PlaySound(SoundID.Item84, NPC.Center);

                        int delay = Phase > 1 ? 45 : 60;
                        Vector2 target = player.Center;
                        target.Y -= 400;
                        Vector2 speed = (target - NPC.Center) / delay;

                        for (int i = 0; i < 20; i++) //dust spray
                            Dust.NewDust(NPC.Center, 0, 0, Main.rand.NextBool() ? DustID.GoldFlame : DustID.SilverCoin, speed.X, speed.Y, 0, default, 2f);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int type = ModContent.ProjectileType<DeviMimic>();
                            float ai0 = player.position.Y - 16;

                            if (Phase > 1)
                            {
                                type = ModContent.ProjectileType<DeviBigMimic>();
                                ai0 = player.whoAmI;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, type, projectileDamage, 0f, Main.myPlayer, ai0, delay);
                        }
                    }
                }
                else if (Timer == 180) //big wave of mimics, aimed ahead of you
                {
                    SoundEngine.PlaySound(SoundID.Item84, NPC.Center);

                    int modifier = 150;
                    if (player.velocity.X != 0)
                        modifier *= Math.Sign(player.velocity.X);
                    else
                        modifier *= Math.Sign(player.Center.X - NPC.Center.X);

                    int start = 0;
                    if (WorldSavingSystem.EternityMode)
                        start = -3;
                    if (WorldSavingSystem.MasochistModeReal)
                        start = -6;

                    for (int j = start; j <= 6; j++)
                    {
                        Vector2 target = player.Center;
                        target.X += modifier * (j - 1);
                        target.Y -= 400;

                        int delay = Phase > 1 ? 45 : 60;
                        Vector2 speed = (target - NPC.Center) / delay;

                        for (int i = 0; i < 20; i++) //dust spray
                            Dust.NewDust(NPC.Center, 0, 0, Main.rand.NextBool() ? DustID.GoldFlame : DustID.SilverCoin, speed.X, speed.Y, 0, default, 2f);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int type = ModContent.ProjectileType<DeviMimic>();
                            float ai0 = player.position.Y - 16;

                            if (Phase > 1)
                            {
                                type = ModContent.ProjectileType<DeviBigMimic>();
                                ai0 = player.whoAmI;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, type, projectileDamage, 0f, Main.myPlayer, ai0, delay);
                        }
                    }
                }
                else if (Timer > 240)
                {
                    GetNextAttack();
                }
            }
            void FrostballsNados()
            {
                ref float NadoTimer = ref NPC.ai[3];

                if (WorldSavingSystem.EternityMode)
                {
                    targetPos = player.Center + 400 * player.SafeDirectionTo(NPC.Center).RotatedBy(MathHelper.ToRadians(10));
                    NPC.position += (player.position - player.oldPosition) / 2f;

                    if (WorldSavingSystem.MasochistModeReal)
                        Movement(targetPos, 0.8f, 24);
                    else
                        Movement(targetPos, 0.4f, 18);
                }
                else
                {
                    targetPos = player.Center + 350 * player.SafeDirectionTo(NPC.Center);
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.2f);
                }

                if (Timer == 0)
                {
                    StrongAttackTeleport(player.Center + 420 * Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi));
                }

                if (++Timer > 360)
                {
                    GetNextAttack();

                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active
                                && Main.projectile[i].hostile
                                && Main.projectile[i].type == ModContent.ProjectileType<FrostfireballHostile>())
                            {
                                int time = Main.rand.Next(90, 180);
                                if (Main.projectile[i].timeLeft > time)
                                    Main.projectile[i].timeLeft = time;
                            }
                        }
                    }
                }
                if (++SubTimer > (Phase > 1 ? 10 : 20))
                {
                    NPC.netUpdate = true;
                    SubTimer = 0;

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(4f, 0f).RotatedBy(Main.rand.NextDouble() * Math.PI * 2),
                            ModContent.ProjectileType<FrostfireballHostile>(), projectileDamage, 0f, Main.myPlayer, NPC.target, 15f);
                    }
                }
                if (Phase > 1 && --NadoTimer < 0) //spawn sandnado
                {
                    NPC.netUpdate = true;
                    NadoTimer = 110;

                    Vector2 target = player.Center;
                    target.X += player.velocity.X * 90;
                    target.Y -= 150;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), target, Vector2.Zero, ProjectileID.SandnadoHostileMark, 0, 0f, Main.myPlayer);

                    int length = (int)NPC.Distance(target) / 10;
                    Vector2 offset = NPC.SafeDirectionTo(target) * 10f;
                    for (int i = 0; i < length; i++) //dust warning line for sandnado
                    {
                        int d = Dust.NewDust(NPC.Center + offset * i, 0, 0, DustID.Sandnado, 0f, 0f, 0, new Color());
                        Main.dust[d].noLight = true;
                        Main.dust[d].scale = 1.25f;
                    }
                }
            }
            void RuneWizard()
            {


                int threshold = WorldSavingSystem.MasochistModeReal ? 400 : 450;
                EModeGlobalNPC.Aura(NPC, threshold, true, -1, Color.GreenYellow);//, ModContent.BuffType<HexedBuff>(), ModContent.BuffType<CrippledBuff>(), BuffID.Dazed, BuffID.OgreSpit);
                EModeGlobalNPC.Aura(NPC, WorldSavingSystem.MasochistModeReal ? 200 : 150, false, -1, default, ModContent.BuffType<HexedBuff>(), ModContent.BuffType<CrippledBuff>(), BuffID.Dazed, BuffID.OgreSpit);

                Player localPlayer = Main.LocalPlayer;
                float distance = localPlayer.Distance(NPC.Center);
                if (localPlayer.active && !localPlayer.dead && !localPlayer.ghost) //pull into arena
                {
                    if (distance > threshold && distance < threshold * 4f)
                    {
                        if (distance > threshold * 2f)
                        {
                            localPlayer.controlLeft = false;
                            localPlayer.controlRight = false;
                            localPlayer.controlUp = false;
                            localPlayer.controlDown = false;
                            localPlayer.controlUseItem = false;
                            localPlayer.controlUseTile = false;
                            localPlayer.controlJump = false;
                            localPlayer.controlHook = false;
                            if (localPlayer.grapCount > 0)
                                localPlayer.RemoveAllGrapplingHooks();
                            if (localPlayer.mount.Active)
                                localPlayer.mount.Dismount(localPlayer);
                            localPlayer.velocity.X = 0f;
                            localPlayer.velocity.Y = -0.4f;
                            localPlayer.FargoSouls().NoUsingItems = 2;
                        }

                        Vector2 movement = NPC.Center - localPlayer.Center;
                        float difference = movement.Length() - threshold;
                        movement.Normalize();
                        movement *= difference < 17f ? difference : 17f;
                        localPlayer.position += movement;

                        for (int i = 0; i < 10; i++)
                        {
                            int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                            int d = Dust.NewDust(localPlayer.position, localPlayer.width, localPlayer.height, DustType, 0f, 0f, 0, default, 1.25f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 5f;
                        }
                    }
                }


                // Indicate that the borders should be drawn.
                DrawRuneBorders = true;

                NPC.velocity = Vector2.Zero;

                if (WorldSavingSystem.MasochistModeReal && SubTimer < 1)
                    SubTimer = 1;

                int attackTime = WorldSavingSystem.EternityMode ? 40 : 50;
                if (++Timer == 1)
                {
                    TeleportDust();
                    if (FargoSoulsUtil.HostCheck)
                    {
                        bool wasOnLeft = NPC.Center.X < player.Center.X;
                        NPC.Center = player.Center + 300 * Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0, 2 * (float)Math.PI));
                        if (wasOnLeft ? NPC.Center.X < player.Center.X : NPC.Center.X > player.Center.X)
                        {
                            float x = player.Center.X - NPC.Center.X;
                            NPC.position.X += x * 2;
                        }
                        NPC.netUpdate = true;
                    }
                    TeleportDust();
                    SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
                }
                else if (Timer == attackTime)
                {
                    if (SubTimer > 0 && FargoSoulsUtil.HostCheck)
                    {
                        for (int i = -1; i <= 1; i++) //rune blast spread
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center,
                                12f * NPC.SafeDirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(5) * i),
                                ProjectileID.RuneBlast, projectileDamage, 0f, Main.myPlayer);

                        if (Phase > 1) //rune blast ring
                        {
                            Vector2 vel = NPC.DirectionFrom(Main.player[NPC.target].Center) * 8;
                            int max = WorldSavingSystem.MasochistModeReal ? 10 : 5;
                            for (int i = 0; i < max; i++)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.RotatedBy(2 * Math.PI / max * i),
                                    ProjectileID.RuneBlast, projectileDamage, 0f, Main.myPlayer, 1);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 300;
                            }
                        }
                    }
                }
                else if (Timer > attackTime * 2)
                {
                    if (++SubTimer > 3 + 1) //compensate for empty teleport at beginning
                    {
                        GetNextAttack();

                        if (FargoSoulsUtil.HostCheck)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ProjectileID.RuneBlast)
                                {
                                    int time = Main.rand.Next(90, 180);
                                    if (Main.projectile[i].timeLeft > time)
                                        Main.projectile[i].timeLeft = time;
                                }
                            }
                        }
                    }
                    else
                    {
                        NPC.netUpdate = true;
                        Timer = 0;
                    }
                }
                else if (SubTimer == 0) //faster on first empty tp
                {
                    Timer++;
                }
            }
            void MothDustCharges()
            {
                ref float FirstFrameCheck = ref NPC.localAI[0];
                ref float MothDustTimer = ref NPC.ai[3];

                NPC.velocity *= 0.9f;

                if (FirstFrameCheck == 0) //teleport behind you
                {
                    FirstFrameCheck = 1;
                    Timer = -45;

                    TeleportDust();
                    if (FargoSoulsUtil.HostCheck)
                    {
                        bool wasOnLeft = NPC.Center.X < player.Center.X;
                        NPC.Center = player.Center;
                        NPC.position.X += wasOnLeft ? 400 : -400;
                        NPC.netUpdate = true;
                    }
                    TeleportDust();

                    SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.Center.X < player.Center.X ? -1f : 1f, -1f),
                            ModContent.ProjectileType<DeviSparklingLoveSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI, 0.0001f * Math.Sign(player.Center.X - NPC.Center.X));
                    }
                }

                if (++MothDustTimer > 2)
                {
                    MothDustTimer = 0;

                    if (FargoSoulsUtil.HostCheck) //make moth dust trail
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Unit() * 2f, ModContent.ProjectileType<MothDust>(), projectileDamage, 0f, Main.myPlayer);
                }

                if (Timer == 0 && WorldSavingSystem.EternityMode && ((SubTimer % 2 == 1 && Phase > 1) || WorldSavingSystem.MasochistModeReal))
                {
                    int max = WorldSavingSystem.MasochistModeReal ? 8 : 3;
                    float spread = WorldSavingSystem.MasochistModeReal ? 64 : 48;
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 target = player.Center + player.velocity * 30f - NPC.Center;
                        target += Main.rand.NextVector2Circular(spread, spread);
                        if (WorldSavingSystem.MasochistModeReal)
                            target *= 2f;

                        Vector2 speed = 2 * target / 90;
                        float acceleration = -speed.Length() / 90;

                        int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);

                        float rotation = WorldSavingSystem.MasochistModeReal ? MathHelper.ToRadians(Main.rand.NextFloat(-10, 10)) : 0;

                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                damage, 0f, Main.myPlayer, rotation, acceleration);
                        }
                    }
                }

                if (++Timer > (Phase > 1 ? 45 : 60))
                {
                    NPC.netUpdate = true;
                    if (++SubTimer > 5)
                    {
                        GetNextAttack();
                    }
                    else
                    {
                        State++;
                        Timer = 0;
                        NPC.velocity = NPC.SafeDirectionTo(player.Center + player.velocity) * 20f;
                        if (FargoSoulsUtil.HostCheck)
                        {
                            float rotation = MathHelper.Pi * 1.5f * (SubTimer % 2 == 0 ? 1 : -1);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(-rotation / 2),
                                ModContent.ProjectileType<DeviSparklingLoveSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI, rotation / 60 * 2);
                        }
                    }
                }
            }
            void WhileDashing()
            {
                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                ref float MothDustTimer = ref NPC.ai[3];

                if (++MothDustTimer > 2)
                {
                    MothDustTimer = 0;

                    if (FargoSoulsUtil.HostCheck) //make moth dust trail
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2Unit() * 2f, ModContent.ProjectileType<MothDust>(), projectileDamage, 0f, Main.myPlayer);
                }

                if (++Timer > 30)
                {
                    NPC.netUpdate = true;
                    State--;
                    Timer = 0;
                }
            }
            void MageSkeletonAttacks()
            {
                ref float ShadowbeamTimer = ref NPC.ai[3];
                ref float StoredRotation = ref NPC.localAI[0];

                NPC.velocity = NPC.SafeDirectionTo(player.Center + NPC.DirectionFrom(player.Center) * 80) * 2f;

                if (++Timer == 1)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -1);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 10, NPC.whoAmI);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0f, Main.myPlayer, 10, NPC.whoAmI);
                    }
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }
                else if (Timer < 120) //spam shadowbeams after delay
                {
                    if (ShadowbeamTimer <= 0) //store rotation briefly before shooting
                        StoredRotation = NPC.SafeDirectionTo(player.Center).ToRotation();

                    if (++SubTimer > 90)
                    {
                        if (++ShadowbeamTimer > (Phase > 1 ? 5 : 8))
                        {
                            ShadowbeamTimer = 0;

                            if (FargoSoulsUtil.HostCheck) //shadowbeam
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 6f * Vector2.UnitX.RotatedBy(NPC.localAI[0]), ProjectileID.ShadowBeamHostile, projectileDamage, 0f, Main.myPlayer);
                            }

                            if (WorldSavingSystem.MasochistModeReal) //maso shotgun
                            {
                                for (int index = 0; index < 6; ++index)
                                {
                                    float num6 = player.Center.X - NPC.Center.X;
                                    float num10 = player.Center.Y - NPC.Center.Y;
                                    float num11 = 11f / (float)Math.Sqrt(num6 * num6 + num10 * num10);
                                    float num18 = num6 + Main.rand.Next(-40, 41);
                                    float num20 = num10 + Main.rand.Next(-40, 41);
                                    float SpeedX = num18 * num11;
                                    float SpeedY = num20 * num11;
                                    if (FargoSoulsUtil.HostCheck)
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, SpeedX, SpeedY, ProjectileID.MeteorShot, projectileDamage, 0f, Main.myPlayer);
                                }
                                SoundEngine.PlaySound(SoundID.Item38, NPC.Center);
                            }
                        }
                    }
                }
                else if (Timer < 240)
                {
                    ShadowbeamTimer = 0;
                    StoredRotation = 0;

                    if (++SubTimer > (Phase > 1 ? 20 : 40))
                    {
                        SubTimer = 0;

                        if (FargoSoulsUtil.HostCheck) //diabolist bolts
                        {
                            float speed = Phase > 1 ? 16 : 8;
                            Vector2 blastPos = NPC.Center + Main.rand.NextFloat(1, 2) * NPC.Distance(player.Center) * NPC.SafeDirectionTo(player.Center);
                            int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed * NPC.SafeDirectionTo(player.Center), ProjectileID.InfernoHostileBolt, projectileDamage, 0f, Main.myPlayer, blastPos.X, blastPos.Y);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = 300;
                        }

                        if (WorldSavingSystem.MasochistModeReal) //maso rockets
                        {
                            if (FargoSoulsUtil.HostCheck)
                            {
                                Vector2 speed = player.Center - NPC.Center;
                                speed.X += Main.rand.Next(-20, 21);
                                speed.Y += Main.rand.Next(-20, 21);
                                speed.Normalize();

                                int damage = projectileDamage;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 4f * speed, ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(10f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(-10f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                            }
                            SoundEngine.PlaySound(SoundID.Item11, NPC.Center);
                        }
                    }
                }
                else
                {
                    NPC.velocity /= 2;

                    if (Timer == 241 && WorldSavingSystem.EternityMode)
                    {
                        float tpDistance = WorldSavingSystem.MasochistModeReal ? 180 : 420;
                        StrongAttackTeleport(player.Center + tpDistance * NPC.SafeDirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4));
                    }

                    if (Timer == 315)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center); //eoc roar

                        if (FargoSoulsUtil.HostCheck)
                        {
                            int max = Phase > 1 ? 30 : 20;
                            for (int i = 0; i < max; i++) //spray ragged caster bolts
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextFloat(2f, 6f) * Vector2.UnitX.RotatedBy(Main.rand.NextFloat((float)Math.PI * 2)), ModContent.ProjectileType<DeviLostSoul>(), projectileDamage, 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 300;
                            }
                        }

                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            if (FargoSoulsUtil.HostCheck)
                            {
                                Vector2 speed = player.Center - NPC.Center;
                                speed.X += Main.rand.Next(-40, 41) * 0.2f;
                                speed.Y += Main.rand.Next(-40, 41) * 0.2f;
                                speed.Normalize();
                                speed *= 11f;

                                int damage = projectileDamage * 2;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<SniperBullet>(), damage, 0f, Main.myPlayer);
                            }
                            SoundEngine.PlaySound(SoundID.Item40, NPC.Center);
                        }
                    }

                    if (Timer > 360)
                    {
                        GetNextAttack();
                    }
                }
            }
            void BabyGuardians()
            {
                ref float WallDirection = ref NPC.ai[3];

                void BabyGuardianWall()
                {
                    ref float WallDirection = ref NPC.ai[3];

                    WallDirection *= -1;

                    bool flip = WallDirection > 0;

                    for (int i = -1; i <= 1; i++) //left and right sides
                    {
                        if (i == 0)
                            continue;

                        int min = 1;
                        int max = 1;
                        if (WorldSavingSystem.EternityMode)
                        {
                            int shortSide = WorldSavingSystem.MasochistModeReal ? 2 : 1;

                            min = flip ? shortSide : 12;
                            max = flip ? 12 : shortSide;
                        }

                        for (int j = -min; j <= max; j++)
                        {
                            Vector2 spawnPos = player.Center;
                            spawnPos.X += 1200 * i;
                            spawnPos.Y += 50 * j;
                            Vector2 vel = 10 * Vector2.UnitX * -i;

                            if (FargoSoulsUtil.HostCheck) //shoot guardians
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<DeviGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3), 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 1200 / 10 + 2;
                            }
                        }
                    }
                }

                targetPos = player.Center;
                targetPos.Y -= 400;
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.3f);

                if (Timer == 1) //tp above player
                {
                    TeleportDust();
                    if (FargoSoulsUtil.HostCheck)
                    {
                        NPC.Center = player.Center;
                        NPC.position.X += 500 * (Main.rand.NextBool() ? -1 : 1);
                        NPC.position.Y -= Main.rand.NextFloat(300, 500);
                        NPC.netUpdate = true;
                    }
                    TeleportDust();

                    SoundEngine.PlaySound(SoundID.Item84, NPC.Center);

                    WallDirection = Main.rand.NextBool() ? -1 : 1; //randomly decide initial wall direction
                }

                if (++Timer < 180)
                {
                    if (Timer % 5 == 0 && FargoSoulsUtil.HostCheck)
                    {
                        Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 / 3f) * 20;
                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<DeviGuardianHarmless>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3), 0f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 60;
                    }
                    /*
                    //warning dust
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 0, default, 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 12f;
                    }
                    */
                }
                else if (Timer == 180)
                {
                    NPC.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    BabyGuardianWall();
                }
                else
                {
                    if (++SubTimer > 3)
                    {
                        SubTimer = 0;
                        SoundEngine.PlaySound(SoundID.Item21, NPC.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            Vector2 spawnPos = player.Center;
                            spawnPos.X += Main.rand.Next(-200, 201);
                            spawnPos.Y += 700;
                            Vector2 vel = Main.rand.NextFloat(12, 16f) * Vector2.Normalize(player.Center - spawnPos);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<DeviGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3), 0f, Main.myPlayer);
                        }
                    }

                    if (Timer > 360)
                    {
                        if (Phase > 1 && WorldSavingSystem.MasochistModeReal) //another wave in maso
                        {
                            SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center); //eoc roar
                            BabyGuardianWall();
                        }

                        GetNextAttack();
                    }

                    if (Phase > 1 && Timer == 270) //surprise!
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center); //eoc roar

                        BabyGuardianWall();
                    }
                }
            }
            void GeyserRain()
            {
                ref float LockX = ref NPC.localAI[0];
                ref float LockY = ref NPC.localAI[1];
                ref float HeartTimer = ref NPC.ai[3];

                if (LockX == 0 && LockY == 0)
                {
                    StrongAttackTeleport(new Vector2(NPC.Center.X, player.Center.Y - 420));

                    if (WorldSavingSystem.EternityMode && FargoSoulsUtil.HostCheck) //spawn ritual for strong attacks
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);

                    LockX = NPC.Center.X;
                    LockY = NPC.Center.Y;
                    NPC.netUpdate = true;
                }

                //Main.NewText(NPC.localAI[0].ToString() + ", " + NPC.localAI[1].ToString());

                targetPos = player.Center;
                if (NPC.Center.Y > player.Center.Y)
                    targetPos.X += 300 * (NPC.Center.X < targetPos.X ? -1 : 1);
                targetPos.Y -= 350;
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.15f);

                if (WorldSavingSystem.EternityMode && Timer % 180 == 90)
                {
                    for (int i = -1; i <= 1; i += 2)
                    {
                        for (int j = -1; j <= 1; j += 2)
                        {
                            int max = WorldSavingSystem.MasochistModeReal ? 3 : 1;
                            for (int k = 0; k < max; k++)
                            {
                                Vector2 target = player.Center;
                                target.X += 16 * 24 * i;
                                target.Y += Player.defaultHeight / 2 * j;
                                if (WorldSavingSystem.MasochistModeReal)
                                    target += Main.rand.NextVector2Circular(16, 16);
                                target -= NPC.Center;

                                Vector2 speed = 2 * target / 90;
                                float acceleration = -speed.Length() / 90;

                                int damage = Phase > 1 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3) : FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                                float rotation = WorldSavingSystem.MasochistModeReal ? MathHelper.ToRadians(Main.rand.NextFloat(-20, 20)) : 0;

                                if (FargoSoulsUtil.HostCheck)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                      damage, 0f, Main.myPlayer, rotation, acceleration);
                                }
                            }
                        }
                    }
                }

                if (++Timer < 120)
                {
                    if (++SubTimer > 2)
                    {
                        SubTimer = 0;
                        SoundEngine.PlaySound(SoundID.Item44, NPC.Center);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -24 * Vector2.UnitY.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI / 2),
                                ModContent.ProjectileType<DeviVisualHeart>(), 0, 0f, Main.myPlayer);
                        }
                    }
                }
                else if (Timer < 420)
                {
                    if (--HeartTimer < 0)
                    {
                        NPC.netUpdate = true;
                        HeartTimer = WorldSavingSystem.MasochistModeReal ? 70 : 85;

                        SubTimer = SubTimer == 1 ? -1 : 1;

                        if (FargoSoulsUtil.HostCheck)
                        {
                            float angle = 10;
                            if (!WorldSavingSystem.MasochistModeReal) //nerf to have no x speed in p1, unless in maso
                                angle = Phase > 1 ? 5 : 0;
                            Vector2 speed = 24 * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(angle) * SubTimer);

                            int type = Phase > 1 ? ModContent.ProjectileType<DeviRainHeart2>() : ModContent.ProjectileType<DeviRainHeart>();
                            int damage = Phase > 1 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3) : FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                            int range = Phase > 1 ? 8 : 10;
                            float spacing = 1200f / range;
                            float offset = Main.rand.NextFloat(-spacing, spacing);

                            for (int i = -range; i <= range; i++)
                            {
                                Vector2 spawnPos = new(LockX, LockY);
                                spawnPos.X += spacing * i + offset;
                                spawnPos.Y -= 1200;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, speed, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }
                        }
                    }
                }
                else if (Timer > 510)
                {
                    GetNextAttack();
                }
            }
            void CrossRayHearts()
            {
                ref float FirstFrameCheck = ref NPC.localAI[0];
                ref float DirectionRandom = ref NPC.localAI[1];
                ref float HeartTimer = ref NPC.ai[3];

                targetPos = player.Center + player.SafeDirectionTo(NPC.Center) * 400;
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.3f);

                if (FirstFrameCheck == 0)
                {
                    StrongAttackTeleport();

                    if (WorldSavingSystem.EternityMode && FargoSoulsUtil.HostCheck) //spawn ritual for strong attacks
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);

                    FirstFrameCheck = 1;
                    NPC.netUpdate = true;
                }

                if (SubTimer == 0)
                {
                    DirectionRandom = Main.rand.NextBool() ? -1 : 1;
                    NPC.netUpdate = true;
                }

                if (++SubTimer > (Phase > 1 ? 75 : 100))
                {
                    if (++HeartTimer > (WorldSavingSystem.MasochistModeReal ? 3 : 5))
                    {
                        HeartTimer = 0;
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Vector2 target = player.Center - NPC.Center;
                            target.X += Main.rand.Next(-75, 76);
                            target.Y += Main.rand.Next(-75, 76);

                            Vector2 speed = 2 * target / 90;
                            float acceleration = -speed.Length() / 90;

                            int damage = Phase > 1 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3) : FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                            float rotation = WorldSavingSystem.MasochistModeReal ? MathHelper.ToRadians(Main.rand.NextFloat(-20, 20)) : 0;

                            if (WorldSavingSystem.EternityMode && DirectionRandom > 0)
                                rotation += MathHelper.PiOver4 * (Main.rand.NextBool() ? -1 : 1);

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                damage, 0f, Main.myPlayer, rotation, acceleration);
                        }
                    }

                    if (SubTimer > 130)
                    {
                        NPC.netUpdate = true;
                        SubTimer = 0;
                    }
                }

                if (++Timer > (Phase > 1 ? 450 : 480))
                {
                    GetNextAttack();
                }
            }
            void Butterflies()
            {
                NPC.velocity = Vector2.Zero;

                if (SubTimer == 0)
                {
                    StrongAttackTeleport();

                    SubTimer = 1;
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        float offset = Main.rand.NextFloat(600);
                        int damage = Phase > 1 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3) : FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                        int max = 8;
                        for (int i = 0; i < max; i++) //make butterflies
                        {
                            Vector2 speed = new(Main.rand.NextFloat(40f), Main.rand.NextFloat(-20f, 20f));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<DeviButterfly>(),
                               damage, 0f, Main.myPlayer, NPC.whoAmI, 300 / 4 * i + offset);
                        }
                    }

                    if (WorldSavingSystem.EternityMode && FargoSoulsUtil.HostCheck) //spawn ritual for strong attacks
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                }

                //rainbow spike rain, pulses on and off
                if (WorldSavingSystem.MasochistModeReal && Timer % 120 > 100 && Timer % 3 == 0)
                {
                    const int max = 3;
                    for (int i = -max; i <= max; i++)
                    {
                        float offset = i;
                        if (Timer % 240 > 120)
                            offset -= 0.5f * Math.Sign(i);

                        Vector2 target = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                        target.X += 1000f / (max + 1f) * offset;

                        const float gravity = 0.15f;
                        const float time = 180f;
                        Vector2 distance = target - NPC.Center;
                        distance.X /= time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance, ModContent.ProjectileType<RainbowSlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 1f);
                    }
                }

                if (++Timer > 480)
                {
                    GetNextAttack();
                }
            }
            void MedusaRay()
            {
                ref float FirstFrameCheck = ref NPC.localAI[0];
                ref float StoredRotation = ref NPC.localAI[1];
                ref float PulseCounter = ref NPC.ai[3];

                if (FirstFrameCheck == 0)
                {
                    StrongAttackTeleport();

                    FirstFrameCheck = 1;
                    NPC.velocity = Vector2.Zero;

                    if (WorldSavingSystem.EternityMode && FargoSoulsUtil.HostCheck) //spawn ritual for strong attacks
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                }

                if (PulseCounter < 4 && NPC.Distance(Main.LocalPlayer.Center) < 3000 && Collision.CanHitLine(NPC.Center, 0, 0, Main.LocalPlayer.Center, 0, 0)
                    && Math.Sign(Main.LocalPlayer.direction) == Math.Sign(NPC.Center.X - Main.LocalPlayer.Center.X)
                    && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                {
                    Vector2 target = Main.LocalPlayer.Center - Vector2.UnitY * 12;
                    Vector2 source = NPC.Center - Vector2.UnitY * 6;
                    Vector2 distance = target - source;

                    int length = (int)distance.Length() / 10;
                    Vector2 offset = Vector2.Normalize(distance) * 10f;
                    for (int i = 0; i <= length; i++) //dust indicator
                    {
                        int d = Dust.NewDust(source + offset * i, 0, 0, DustID.GoldFlame, 0f, 0f, 0, new Color());
                        Main.dust[d].noLight = true;
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = 1f;
                    }
                }

                if (PulseCounter < 7)
                {
                    if (WorldSavingSystem.EternityMode)
                    {
                        Timer += 0.4f;
                        SubTimer += 0.4f;
                    }

                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        Timer += 0.6f;
                        SubTimer += 0.6f;
                    }
                }

                if (++SubTimer > 60)
                {
                    SubTimer = 0;
                    //only make rings in p2 and before firing ray
                    if (Phase > 1 && PulseCounter < 7 && !Main.player[NPC.target].stoned)
                    {
                        if (FargoSoulsUtil.HostCheck)
                        {
                            const int max = 12;
                            int damage = Phase > 1 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3) : FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 6f * NPC.SafeDirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i),
                                    ModContent.ProjectileType<DeviHeart>(), damage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++PulseCounter < 4) //medusa warning
                    {
                        NPC.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center); //eoc roar

                        FargoSoulsUtil.DustRing(NPC.Center, 120, 228, 20f, default, 2f);

                        if (PulseCounter == 1 && FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, -1), ModContent.ProjectileType<DeviMedusa>(), 0, 0, Main.myPlayer);
                    }
                    else if (PulseCounter == 4) //petrify
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath17, NPC.Center);

                        if (NPC.Distance(Main.LocalPlayer.Center) < 3000 && Collision.CanHitLine(NPC.Center, 0, 0, Main.LocalPlayer.Center, 0, 0)
                            && Math.Sign(Main.LocalPlayer.direction) == Math.Sign(NPC.Center.X - Main.LocalPlayer.Center.X)
                            && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                        {
                            for (int i = 0; i < 40; i++) //petrify dust
                            {
                                int d = Dust.NewDust(Main.LocalPlayer.Center, 0, 0, DustID.Stone, 0f, 0f, 0, default, 2f);
                                Main.dust[d].velocity *= 3f;
                            }

                            Main.LocalPlayer.AddBuff(BuffID.Stoned, 300);
                            if (Main.LocalPlayer.HasBuff(BuffID.Stoned))
                                Main.LocalPlayer.AddBuff(BuffID.Featherfall, 300);

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), Main.LocalPlayer.Center, new Vector2(0, -1), ModContent.ProjectileType<DeviMedusa>(), 0, 0, Main.myPlayer);
                        }
                    }
                    else if (PulseCounter < 7) //ray warning
                    {
                        NPC.netUpdate = true;

                        FargoSoulsUtil.DustRing(NPC.Center, 160, 86, 40f, default, 2.5f);

                        StoredRotation = NPC.SafeDirectionTo(player.Center).ToRotation(); //store for aiming ray

                        if (PulseCounter == 6 && FargoSoulsUtil.HostCheck) //final warning
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(StoredRotation), ModContent.ProjectileType<DeviDeathraySmall>(),
                                0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                    }
                    else if (PulseCounter == 7) //fire deathray
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        NPC.velocity = -3f * Vector2.UnitX.RotatedBy(StoredRotation);

                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(StoredRotation), ModContent.ProjectileType<DeviBigDeathray>(),
                                FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }

                        const int ring = 160;
                        for (int i = 0; i < ring; ++i)
                        {
                            Vector2 vector2 = (-Vector2.UnitY.RotatedBy(i * 3.14159274101257 * 2 / ring) * new Vector2(8f, 16f)).RotatedBy(NPC.velocity.ToRotation());
                            int index2 = Dust.NewDust(NPC.Center, 0, 0, DustID.GemAmethyst, 0.0f, 0.0f, 0, new Color(), 1f);
                            Main.dust[index2].scale = 5f;
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].position = NPC.Center;
                            Main.dust[index2].velocity = vector2 * 3f;
                        }
                    }
                }

                if (PulseCounter < 7) //charge up dust
                {
                    float num1 = 0.99f;
                    if (PulseCounter >= 1f)
                        num1 = 0.79f;
                    if (PulseCounter >= 2f)
                        num1 = 0.58f;
                    if (PulseCounter >= 3f)
                        num1 = 0.43f;
                    if (PulseCounter >= 4f)
                        num1 = 0.33f;
                    for (int i = 0; i < 9; ++i)
                    {
                        if (Main.rand.NextFloat() >= num1)
                        {
                            float f = Main.rand.NextFloat() * 6.283185f;
                            float num2 = Main.rand.NextFloat();
                            Dust dust = Dust.NewDustPerfect(NPC.Center + f.ToRotationVector2() * (110 + 600 * num2), 86, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                            dust.scale = 0.9f;
                            dust.fadeIn = 1.15f + num2 * 0.3f;
                            //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                            dust.noGravity = true;
                            //dust.noLight = true;
                        }
                    }
                }

                if (StoredRotation != 0)
                    NPC.direction = NPC.spriteDirection = Math.Sign(StoredRotation.ToRotationVector2().X);

                if (++Timer > 600)//(Phase > 1 ? 540 : 600))
                {
                    GetNextAttack();
                }
            }
            void SparklingLove()
            {
                ref float FirstFrameCheck = ref NPC.localAI[0];
                ref float SwingRotation = ref NPC.ai[3];

                if (FirstFrameCheck == 0)
                {
                    StrongAttackTeleport(player.Center + new Vector2(300 * Math.Sign(NPC.Center.X - player.Center.X), -100));

                    FirstFrameCheck = 1;

                    if (WorldSavingSystem.EternityMode && FargoSoulsUtil.HostCheck) //spawn ritual for strong attacks
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DeviRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                    }
                }

                if (++Timer < 150)
                {
                    NPC.velocity = Vector2.Zero;

                    if (SubTimer == 0) //spawn weapon, teleport
                    {
                        double angle = NPC.position.X < player.position.X ? -Math.PI / 4 : Math.PI / 4;
                        SubTimer = (float)angle * -4f / 30;

                        //spawn axe
                        const int loveOffset = 90;
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + -Vector2.UnitY.RotatedBy(angle) * loveOffset, Vector2.Zero, ModContent.ProjectileType<DeviSparklingLove>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 2), 0f, Main.myPlayer, NPC.whoAmI, loveOffset);
                        }

                        //spawn hitboxes
                        const int spacing = 80;
                        Vector2 offset = -Vector2.UnitY.RotatedBy(angle) * spacing;
                        if (FargoSoulsUtil.HostCheck)
                        {
                            void SpawnAxeHitbox(Vector2 spawnPos)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<DeviAxe>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 2), 0f, Main.myPlayer, NPC.whoAmI, NPC.Distance(spawnPos));
                            }

                            for (int i = 0; i < 8; i++)
                                SpawnAxeHitbox(NPC.Center + offset * i);
                            for (int i = 1; i < 3; i++)
                            {
                                SpawnAxeHitbox(NPC.Center + offset * 5 + offset.RotatedBy(-angle * 2) * i);
                                SpawnAxeHitbox(NPC.Center + offset * 6 + offset.RotatedBy(-angle * 2) * i);
                            }
                        }

                        if (WorldSavingSystem.MasochistModeReal && FargoSoulsUtil.HostCheck)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 target = new Vector2(80f, 80f).RotatedBy(MathHelper.Pi / 2 * i);

                                Vector2 speed = 2 * target / 90;
                                float acceleration = -speed.Length() / 90;

                                int damage = Phase > 1 ? FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 4f / 3) : FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                    damage, 0f, Main.myPlayer, 0, acceleration);
                            }
                        }
                    }

                    //some slight rearing back before swing
                    float progress = Timer / 150f;
                    float modifier = 0.025f;
                    SwingRotation -= SubTimer * progress * modifier;

                    NPC.direction = NPC.spriteDirection = Math.Sign(SubTimer);
                }
                else if (Timer == 150) //start swinging
                {
                    targetPos = player.Center;
                    targetPos.X -= 360 * Math.Sign(SubTimer);
                    //targetPos.Y -= 200;
                    NPC.velocity = (targetPos - NPC.Center) / 30;
                    NPC.netUpdate = true;

                    NPC.direction = NPC.spriteDirection = Math.Sign(SubTimer);

                    if (!WorldSavingSystem.MasochistModeReal && Math.Sign(targetPos.X - NPC.Center.X) != Math.Sign(SubTimer))
                        NPC.velocity.X *= 0.5f; //worse movement if you're behind her
                }
                else if (Timer < 180)
                {
                    //acceleration logic from math on paper
                    const float startTime = 150;
                    const float totalTime = 30;
                    float progress = (Timer - startTime);
                    float maxSpeed = SubTimer * 2 / (totalTime);
                    SwingRotation += progress * maxSpeed;
                    //NPC.ai[3] += SubTimer;
                    NPC.direction = NPC.spriteDirection = Math.Sign(SubTimer);
                }
                else
                {
                    targetPos = player.Center + player.SafeDirectionTo(NPC.Center) * 400;
                    if (NPC.Distance(targetPos) > 50)
                        Movement(targetPos, 0.2f);

                    if (Timer > 300)
                    {
                        GetNextAttack();
                    }
                }
            }
            void Pause()
            {
                NPC.dontTakeDamage = false;

                targetPos = player.Center + player.SafeDirectionTo(NPC.Center) * 200;
                Movement(targetPos, 0.1f);
                if (NPC.Distance(player.Center) < 100)
                    Movement(targetPos, 0.5f);

                int delay = 180;
                if (WorldSavingSystem.MasochistModeReal)
                {
                    delay -= 30;
                    ignoreMoney = true;
                }
                if (WorldSavingSystem.EternityMode)
                    delay -= 60;
                if (Phase > 1)
                    delay -= 30;
                if (++Timer > delay)
                {
                    NPC.netUpdate = true;
                    State = 16; //placeholder
                    Timer = 0;
                    SubTimer = 0;
                    NPC.ai[3] = 0;
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;

                    if (!ignoreMoney && NPC.extraValue > Item.buyPrice(platinumToBribe))
                    {
                        State = 17;
                    }
                    else
                    {
                        State = attackQueue[(int)AttackIndex];

                        int threshold = attackQueue.Length; //only do super attacks in emode
                        if (!WorldSavingSystem.EternityMode)
                            threshold -= 1;
                        if (++AttackIndex >= threshold)
                        {
                            AttackIndex = Phase > 1 ? 1 : 0;
                            RefreshAttackQueue();
                        }
                    }
                }
            }
            void Bribery()
            {
                NPC.dontTakeDamage = true;
                NPC.velocity *= 0.95f;
                if (NPC.timeLeft < 600)
                    NPC.timeLeft = 600;

                if (NPC.buffType[0] != 0)
                    NPC.DelBuff(0);

                Rectangle displayPoint = new(NPC.Hitbox.Center.X, NPC.Hitbox.Center.Y - NPC.height / 4, 2, 2);

                if (Timer == 0)
                {
                    if (FargoSoulsUtil.HostCheck) //clear my arena
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DeviRitual>() && Main.projectile[i].ai[1] == NPC.whoAmI)
                            {
                                Main.projectile[i].Kill();
                            }
                        }
                    }
                }
                else if (Timer == 60)
                {
                    CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Line1"));
                }
                else if (Timer == 150)
                {
                    CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Line2"), true);
                }
                else if (Timer == 300)
                {
                    CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Line3"), true);
                }
                else if (Timer == 450)
                {
                    if (WorldSavingSystem.DownedDevi)
                    {
                        CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Accept1"));
                    }
                    else
                    {
                        CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Reject1"), true);
                    }
                }
                else if (Timer == 600)
                {
                    if (WorldSavingSystem.DownedDevi)
                    {
                        CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Accept2"), true);
                    }
                    else
                    {
                        CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Reject2"), true);

                        SoundEngine.PlaySound(SoundID.Item28, player.Center);
                        Vector2 spawnPos = NPC.Center + Vector2.UnitX * NPC.width * 2 * (player.Center.X < NPC.Center.X ? -1 : 1);
                        for (int i = 0; i < 30; i++)
                        {
                            int d = Dust.NewDust(spawnPos, 0, 0, DustID.RainbowTorch, 0, 0, 0, default, 1f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 6f;
                        }
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -21);

                            Item.NewItem(NPC.GetSource_Loot(), spawnPos, ItemID.PlatinumCoin, platinumToBribe);
                        }
                    }

                    NPC.extraValue -= Item.buyPrice(platinumToBribe);
                }
                else if (Timer == 900)
                {
                    if (WorldSavingSystem.DownedDevi)
                    {
                        CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Accept3"), true);
                    }
                    else
                    {
                        CombatText.NewText(displayPoint, Color.HotPink, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.DeviBoss.Bribe.Reject3"), true);
                    }
                }

                if (++Timer > 1050)
                {
                    ignoreMoney = true;
                    if (WorldSavingSystem.DownedDevi)
                    {
                        NPC.life = 0;
                        NPC.checkDead();
                    }
                    else
                    {
                        State = 16;
                        Timer = 0;
                        NPC.velocity = 20f * NPC.DirectionFrom(player.Center);
                    }
                    NPC.netUpdate = true;
                }
            }
            #endregion
        }
        #endregion
        #region Other Overrides

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<LovestruckBuff>(), 240);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemAmethyst, 0f, 0f, 0, default, 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
        }
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (State != 8)
                return false;

            CooldownSlot = 1;
            return NPC.Distance(FargoSoulsUtil.ClosestPointInHitbox(target, NPC.Center)) < Player.defaultHeight;
        }

        public override bool CanHitNPC(NPC target)
        {
            if (target.type == ModContent.NPCType<Deviantt>()
                || target.type == ModContent.NPCType<Abominationn>()
                || target.type == ModContent.NPCType<Deviantt>())
                return false;

            return base.CanHitNPC(target);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            //if (Item.melee && !ContentModLoaded) damage = (int)(damage * 1.25);

            ModifyHitByAnything(player, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //if ((projectile.melee || projectile.minion) && !ContentModLoaded) damage = (int)(damage * 1.25);

            ModifyHitByAnything(Main.player[projectile.owner], ref modifiers);
        }

        public void ModifyHitByAnything(Player player, ref NPC.HitModifiers hitModifiers)
        {
            if (player.loveStruck)
            {
                hitModifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
                {
                    /*npc.life += damage;
                    if (npc.life > npc.lifeMax)
                        npc.life = npc.lifeMax;
                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage);*/

                    Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                    float ai1 = 30 + Main.rand.Next(30);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, speed, ModContent.ProjectileType<HostileHealingHeart>(), hitInfo.Damage, 0f, Main.myPlayer, NPC.whoAmI, ai1);

                    hitInfo.Null();
                };
            }
        }

        public override bool CheckDead()
        {
            if (State == -2 && Timer >= 180)
                return true;

            NPC.life = 1;
            NPC.active = true;

            if (Phase < 2)
            {
                Phase = 2;
            }
            if (FargoSoulsUtil.HostCheck && State > -2)
            {
                State = -2;
                Timer = 0;
                SubTimer = 0;
                NPC.ai[3] = 0;
                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
            }
            return false;
        }

        public override void OnKill()
        {
            base.OnKill();

            if (!playerInvulTriggered && WorldSavingSystem.EternityMode)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ModContent.ItemType<BrokenBlade>());
                if (Main.bloodMoon)
                    Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ModContent.ItemType<VermillionTopHat>());
            }

            NPC.SetEventFlagCleared(ref WorldSavingSystem.downedDevi, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);

            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DeviatingEnergy>(), 1, 15, 30));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DeviBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeviTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<DeviRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<ChibiHat>(), 4));

            LeadingConditionRule emodeRule = new(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<SparklingAdoration>()));
            npcLoot.Add(emodeRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override void BossHeadSpriteEffects(ref SpriteEffects spriteEffects)
        {
            //spriteEffects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Vector2 position = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY);
            Rectangle rectangle = NPC.frame;

            Texture2D textureToDraw = texture2D13;
            if (Main.getGoodWorld && !NPC.IsABestiaryIconDummy)
            {
                textureToDraw = ModContent.Request<Texture2D>($"{Texture}_FTW", AssetRequestMode.ImmediateLoad).Value;
                int oldFrameHeight = texture2D13.Height / Main.npcFrameCount[NPC.type];
                int currentFrame = rectangle.Y / oldFrameHeight;
                int newFrameHeight = textureToDraw.Height / Main.npcFrameCount[NPC.type];
                rectangle = new Rectangle(0, currentFrame * newFrameHeight, textureToDraw.Width, newFrameHeight);
            }

            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(textureToDraw, position, new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);

            // Draw borders if needed.
            if (DrawRuneBorders)
                DrawBorders(spriteBatch, position);

            return false;

            static void DrawBorders(SpriteBatch spriteBatch, Vector2 position)
            {
                // Inner ring.
                Color innerColor = Color.Red;
                innerColor.A = 0;
                spriteBatch.Draw(FargosTextureRegistry.HardEdgeRing.Value, position, null, innerColor * 0.7f, 0f, FargosTextureRegistry.HardEdgeRing.Value.Size() * 0.5f, 0.65f, SpriteEffects.None, 0f);

                // Outer ring.
                Color outerColor = Color.Green;
                outerColor.A = 0;
                spriteBatch.Draw(FargosTextureRegistry.SoftEdgeRing.Value, position, null, outerColor * 0.7f, 0f, FargosTextureRegistry.SoftEdgeRing.Value.Size() * 0.5f, 2.05f, SpriteEffects.None, 0f);
            }
        }
        #endregion
        #region Help Methods
        private void GetNextAttack()
        {
            NPC.TargetClosest();
            NPC.netUpdate = true;
            State = 16;// attackQueue[(int)AttackIndex];
            Timer = 0;
            SubTimer = 0;
            NPC.ai[3] = 0;
            NPC.localAI[0] = 0;
            NPC.localAI[1] = 0;
        }
        private void RefreshAttackQueue()
        {
            NPC.netUpdate = true;

            int[] newQueue = new int[4];
            for (int i = 0; i < 3; i++)
            {
                newQueue[i] = Main.rand.Next(1, 11);

                bool repeat = false;
                if (newQueue[i] == 8) //npc is the middle of an attack pattern, dont pick it
                    repeat = true;
                for (int j = 0; j < 3; j++) //cant pick attack that's queued in the previous set
                    if (newQueue[i] == attackQueue[j])
                        repeat = true;
                for (int j = i; j >= 0; j--) //can't pick attack that's already queued in npc set
                    if (i != j && newQueue[i] == newQueue[j])
                        repeat = true;

                if (repeat) //retry npc one if needed
                    i--;
            }

            do
            {
                newQueue[3] = Main.rand.Next(11, 16);
            }
            while (newQueue[3] == attackQueue[3] || newQueue[3] == lastStrongAttack || newQueue[3] == 15 && Phase <= 1);
            //don't do sparkling love in p1

            lastStrongAttack = attackQueue[3]; //a strong attack can't be used again for the next 2 checks
            attackQueue = newQueue;

            /*Main.NewText("queue: "
                + attackQueue[0].ToString() + " "
                + attackQueue[1].ToString() + " "
                + attackQueue[2].ToString() + " "
                + attackQueue[3].ToString());*/
        }

        private bool AliveCheck(Player player)
        {
            if ((!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 5000f) && Phase > 0)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 5000f)
                {
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    NPC.velocity.Y -= 1f;
                    if (NPC.timeLeft == 1)
                    {
                        if (NPC.position.Y < 0)
                            NPC.position.Y = 0;
                        if (FargoSoulsUtil.HostCheck && ModContent.TryFind("Fargowiltas", "Deviantt", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                        {
                            FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, modNPC.Type);
                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].homeless = true;
                                if (TownNPCName != default)
                                    Main.npc[n].GivenName = TownNPCName;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                    }
                    return false;
                }
            }
            if (NPC.timeLeft < 600)
                NPC.timeLeft = 600;
            return true;
        }

        private bool Phase2Check()
        {
            if (Phase > 1)
                return false;

            if (NPC.life < NPC.lifeMax * (WorldSavingSystem.EternityMode && !WorldSavingSystem.MasochistModeReal ? 0.66 : 0.5) && Main.expertMode)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    State = -1;
                    Timer = 0;
                    SubTimer = 0;
                    NPC.ai[3] = 0;
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.netUpdate = true;

                    FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                }
                return true;
            }
            return false;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (Math.Abs(NPC.Center.X - targetPos.X) > 10)
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

        private void TeleportDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WitherLightning, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WitherLightning, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }
        }

        private static bool ProjectileExists(int id, int type)
        {
            return id > -1 && id < Main.maxProjectiles && Main.projectile[id].active && Main.projectile[id].type == type;
        }
        #endregion
    }
}