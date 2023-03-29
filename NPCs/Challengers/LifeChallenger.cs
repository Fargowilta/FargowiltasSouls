using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.DataStructures;
using FargowiltasSouls.Projectiles.Challengers;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Items.BossBags;
using FargowiltasSouls.Items.Weapons.Challengers;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.Items.Placeables.Trophies;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Buffs.Masomode;
using Terraria.GameContent.Bestiary;
using FargowiltasSouls.Items.Summons;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using Terraria.Localization;
using FargowiltasSouls.EternityMode.Content.Enemy.FrostMoon;
using static System.Formats.Asn1.AsnWriter;
using FargowiltasSouls.Patreon.Sasha;
using FargowiltasSouls.Items.Placeables.Relics;

namespace FargowiltasSouls.NPCs.Challengers
{

    [AutoloadBossHead]
    public class LifeChallenger : ModNPC
    {
        #region Variables
        const int DefaultHeight = 200;
        const int DefaultWidth = 200;

        public double Phase;

        private bool first = true;

        private bool flyfast;

        private bool Flying = true;

        private bool Charging = false;

        private bool AttackF1;

        private int Attacking = -1;

        public bool PhaseOne = true;

        public bool PhaseThree;

        private int dustcounter;

        public int state;

        private int oldstate = 999;

        private int statecount = 8;

        private bool shoot = false;

        private List<int> availablestates = new List<int>(0);

        private List<int> choicelist = new List<int>(0);

        public Vector2 LockVector1 = new Vector2(0, 0);

        private Vector2 LockVector2 = new Vector2(0, 0);

        private Vector2 LockVector3 = new Vector2(0, 0);

        private Vector2 AuraCenter = new Vector2(0, 0);

        public float choice;

        private int oldchoice = 999;

        private int index;

        private double rotspeed = 0;

        private double rot = 0;

        private bool HitPlayer = false;

        int interval = 0;

        int index2;

        int firstblaster = 2;

        private bool UseTrueOriginAI;

        float BodyRotation = 0;

        public float RPS = 0.1f;

        private int P1state = -2;

        //private int LifeWaveCount;

        private int oldP1state;

        private int P1statecount = 5;

        private bool Draw = false;

        bool useDR;
        bool phaseProtectionDR;

        int flyTimer = 9000;

        private List<int> intervalist = new List<int>(0);

        int P2Threshold => Main.expertMode ? (int)(NPC.lifeMax * 0.66) : 0;
        int P3Threshold => FargoSoulsWorld.EternityMode ? NPC.lifeMax / (FargoSoulsWorld.MasochistModeReal ? 2 : 3) : 0;
        int SansThreshold => FargoSoulsWorld.MasochistModeReal && UseTrueOriginAI ? NPC.lifeMax / 10 : 0;

        private List<int> chunklist = new List<int>(0);
        private List<float> chunkrotlist = new List<float>(0);

        float ChunkTriangleInnerRotation = 0;
        float ChunkTriangleOuterRotation = 0;

        public float RuneDistance = 100;

        public float ChunkDistance = 10;

        public const float ChunkDistanceMax = 80;

        public float SpritePhase = 1;

        public int DrawTime = 0;

        public float PyramidWobble = 5;

        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lieflight");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 18;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>()
                }
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
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 45000;
            NPC.defense = 0;
            NPC.damage = 45;
            NPC.knockBackResist = 0f;
            NPC.width = 200;
            NPC.height = 200;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath7;

            Music = MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.value = Item.buyPrice(0, 15);

            NPC.dontTakeDamage = true; //until it Appears in Opening
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override void OnSpawn(IEntitySource source)
        {
            //only enable this in maso
            if (FargoSoulsWorld.MasochistModeReal)// && Main.player.Any(p => p.active && p.name.ToLower().Contains("cum")))
                UseTrueOriginAI = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(state);
            writer.Write7BitEncodedInt(oldstate);
            writer.Write(choice);
            writer.Write7BitEncodedInt(index);
            writer.Write7BitEncodedInt(index2);
            writer.Write7BitEncodedInt(P1state);
            writer.Write7BitEncodedInt(oldP1state);
            //writer.Write7BitEncodedInt(LifeWaveCount);
            writer.Write(UseTrueOriginAI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            state = reader.Read7BitEncodedInt();
            oldstate = reader.Read7BitEncodedInt();
            choice = reader.ReadSingle();
            index = reader.Read7BitEncodedInt();
            index2 = reader.Read7BitEncodedInt();
            P1state = reader.Read7BitEncodedInt();
            oldP1state = reader.Read7BitEncodedInt();
            //LifeWaveCount = reader.Read7BitEncodedInt();
            UseTrueOriginAI = reader.ReadBoolean();
        }
        #endregion
        #region AI
        public override void AI()
        {
            //Defaults
            Player Player = Main.player[NPC.target];
            Main.time = 27000; //noon
            Main.dayTime = true;
            NPC.defense = NPC.defDefense;
            NPC.chaseable = true;
            useDR = false;
            phaseProtectionDR = false;

            if (PhaseOne && NPC.life < P2Threshold)
                phaseProtectionDR = true;
            if (!PhaseThree && NPC.life < P3Threshold)
                phaseProtectionDR = true;
            if (UseTrueOriginAI && NPC.life < SansThreshold)
                phaseProtectionDR = true;

            //permanent regen for sans phase
            //deliberately done this way so that you can still eventually muscle past with endgame gear (this is ok)
            if (UseTrueOriginAI && NPC.life < SansThreshold * 0.5) //lowered so that sans phase check goes through properly
            {
                int healPerSecond = NPC.lifeMax / 10;
                NPC.life += healPerSecond / 60;
                CombatText.NewText(NPC.Hitbox, CombatText.HealLife, healPerSecond);
            }

            //rotation
            BodyRotation += RPS * MathHelper.TwoPi / 60f; //first number is rotations/second
            ChunkTriangleOuterRotation -= 0.2f * MathHelper.TwoPi / 60f; //first number is rotations/second
            ChunkTriangleInnerRotation += 0.1f * MathHelper.TwoPi / 60f; //first number is rotations/second
            ChunkTriangleInnerRotation %= MathHelper.TwoPi;
            ChunkTriangleOuterRotation %= MathHelper.TwoPi;

            if (P1state != -2) //do not check during spawn anim
            {
                //Aura
                if (FargoSoulsWorld.MasochistModeReal)
                {
                    if (dustcounter > 5)
                    {
                        for (int l = 0; l < 180; l++)
                        {
                            double rad2 = 2.0 * (double)l * (MathHelper.Pi / 180.0);
                            double dustdist2 = 1200.0;
                            int DustX2 = (int)AuraCenter.X - (int)(Math.Cos(rad2) * dustdist2);
                            int DustY2 = (int)AuraCenter.Y - (int)(Math.Sin(rad2) * dustdist2);
                            int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                            int i = Dust.NewDust(new Vector2(DustX2, DustY2), 1, 1, DustType, Scale:1.5f);
                            Main.dust[i].noGravity = true;
                        } 
                        dustcounter = 0;
                    }
                    dustcounter++;

                    float distance = AuraCenter.Distance(Main.LocalPlayer.Center);
                    float threshold = 1200f;
                    Player player = Main.LocalPlayer;
                    if (player.active && !player.dead && !player.ghost) //pull into arena
                    {
                        if (distance > threshold && distance < threshold * 4f)
                        {
                            if (distance > threshold * 2f)
                            {
                                player.controlLeft = false;
                                player.controlRight = false;
                                player.controlUp = false;
                                player.controlDown = false;
                                player.controlUseItem = false;
                                player.controlUseTile = false;
                                player.controlJump = false;
                                player.controlHook = false;
                                if (player.grapCount > 0)
                                    player.RemoveAllGrapplingHooks();
                                if (player.mount.Active)
                                    player.mount.Dismount(player);
                                player.velocity.X = 0f;
                                player.velocity.Y = -0.4f;
                                player.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = 2;
                            }

                            Vector2 movement = AuraCenter - player.Center;
                            float difference = movement.Length() - threshold;
                            movement.Normalize();
                            movement *= difference < 17f ? difference : 17f;
                            player.position += movement;

                            for (int i = 0; i < 10; i++)
                            {
                                int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                                int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default(Color), 1.25f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity *= 5f;
                            }
                        }
                    }
                }
                AuraCenter = NPC.Center;

                //Targeting
                if (!Player.active || Player.dead || Player.ghost || NPC.Distance(Player.Center) > 2400)
                {
                    NPC.TargetClosest(false);
                    Player = Main.player[NPC.target];
                    if (!Player.active || Player.dead || Player.ghost || NPC.Distance(Player.Center) > 2400)
                    {
                        if (NPC.timeLeft > 60)
                            NPC.timeLeft = 60;
                        NPC.velocity.Y -= 0.4f;
                        return;
                    }
                }
                NPC.timeLeft = 60;
            }

            if (PhaseOne) //p1 just skip the rest of the ai and do its own ai lolll
            {
                P1AI();
                return;
            }

            if (Phase < 4.0)
            {
                NPC.dontTakeDamage = true;
            }
            else if (Phase >= 4.0)
            {
                NPC.dontTakeDamage = false;
                Attacking = 1;
            }

            
            if (Phase == 0.0)
            {
                NPC.TargetClosest(true);
                Phase = 0.5;
            }
            if (Phase < 4) //Initial Attack
            {
                Phase = 4.0; //REWORK: REMOVED P2 OPENING ATTACK
                NPC.netUpdate = true;
                //AttackP2Start();
            }

            //Normal looping attack AI
            if (Phase >= 4.0)
            {
                if (Flying) //Flying AI
                {
                    FlyingState();
                }

                if (Charging) //Charging AI (orientation)
                {
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                    if (NPC.velocity == Vector2.Zero)
                    {
                        NPC.rotation = 0f;
                    }
                }
                if (!Charging && !Flying) //standard upright orientation
                {
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.09f);
                }
                if (Attacking == 1) //Phases and random attack choosing
                {
                    if (Phase == 4.0)
                    {
                        NPC.ai[1] = 0f;
                        Phase = 5.0;
                        StateReset();
                    }
                    if (state == oldstate) //ensure you never get the same attack twice (might happen when the possible state list is refilled)
                    {
                        RandomizeState();
                        
                        bool resetFly = true;
                        
                        if (!PhaseThree && NPC.life < P3Threshold)
                        {
                            state = 100;
                            resetFly = false;
                        }

                        if (PhaseThree && NPC.life < SansThreshold)
                        {
                            state = 101;
                            oldstate = 0;
                            resetFly = false;
                        }

                        if (resetFly)
                            flyTimer = 0;
                    }

                    if (FlightCheck())
                    {
                        NPC.ai[1] -= 1; //negate increment below
                    }
                    else if (state != oldstate)
                    {
                        switch (state) //Attack Choices
                        {
                            case 0: //slurp n burp attack
                                AttackSlurpBurp();
                                break;
                            case 1: //rune expand attack
                                AttackRuneExpand();
                                break;
                            case 2: //charge attack
                                AttackCharge();
                                break;
                            case 3: //above tp and down charge -> antigrav cum attack
                                AttackPlunge();
                                break;
                            case 4: //homing pixie attack
                                AttackPixies();
                                break;
                            case 5: // attack where he cuts you off (fires shots at angles from you) then fires a random assortment of projectiles in your direction (including nukes)
                                AttackRoulette();
                                break;
                            case 6: //charged reaction crosshair shotgun
                                AttackReactionShotgun();
                                break;
                            case 7: //running minigun
                                AttackRunningMinigun();
                                break;
                            case 8: //p3 shotgun run
                                AttackShotgun();
                                break;
                            case 9: //p3 teleport on you -> shit nukes
                                AttackTeleportNukes();
                                break;
                            case 99: // (big spinny ray with real damaging lingering spinning scar projectiles and invisible damaging projectiles under it covering the entirety)
                                AttackP3Start();
                                break;
                            case 100: //phase 3 transition
                                {
                                    P3Transition();
                                    break;
                                }
                            case 101: // Life is a cage, and death is the key.
                                {
                                    AttackFinal();
                                    break;
                                }
                            default:
                                StateReset();
                                break;
                        }
                    }
                }
            }
            NPC.ai[1] += 1f;
        }
        public void P1AI()
        {
            
            if (NPC.ai[0] == 0f)
            {
                if (NPC.ai[1] == 30f || P1state == -2)
                {
                    NPC.TargetClosest(true);
                }
                if (NPC.ai[1] >= 60f || P1state == -2)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[0] = 1f;
                }
            }
            if (NPC.ai[0] == 1f)
            {
                if (P1state == oldP1state && P1state != -2) //ensure you never get the same attack twice
                {
                    flyTimer = 0;
                    RandomizeP1state();
                }

                if (FlightCheck()) //negate increment below
                {
                    NPC.ai[1] -= 1f;
                    NPC.ai[2] -= 1f;
                }
                else if (P1state != oldP1state)
                {
                    switch (P1state)
                    {
                        case -2:
                            Opening();
                            break;
                        case -1:
                            P1Transition();
                            break;
                        case 0:
                            P1ShotSpam();
                            break;
                        case 1:
                            P1Nuke();
                            break;
                        case 2:
                            P1Mines();
                            break;
                        case 3:
                            P1Pixies();
                            break;
                        case 4:
                            AttackRuneExpand();
                            break;
                        default:
                            RandomizeP1state();
                            flyTimer = 9000;
                            break;
                    }

                    if (!Flying)
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.09f);
                }
            }
            P1PeriodicNuke();

            NPC.ai[1] += 1f;
            NPC.ai[2] += 1f;
        }
        #endregion
        #region States
        #region P1
        public void Opening()
        {
            if (!NPC.HasValidTarget)
                NPC.TargetClosest(false);

            Player Player = Main.player[NPC.target];
            NPC.position.X = Player.Center.X - (NPC.width / 2);
            NPC.position.Y = Player.Center.Y - 490 - (NPC.height / 2);
            NPC.alpha = (int)(255 - (NPC.ai[2] * 17));
            RPS = 0.1f;

            if (!Main.dedServ && UseTrueOriginAI && ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                && musicMod.Version >= Version.Parse("0.1.1.5"))
            {
                Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Lieflight");
            }
            if (NPC.ai[1] == 120)
            {
                if (UseTrueOriginAI)
                {
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.FatherOfLies");
                    Color color = Color.Goldenrod;
                    FargoSoulsUtil.PrintText(text, color);
                    CombatText.NewText(Player.Hitbox, color, text, true);
                }

                if (!Main.dedServ)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 60;

                if (FargoSoulsWorld.EternityMode && !FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.LifeChallenger] && Main.netMode != NetmodeID.MultiplayerClient)
                    Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<FragilePixieLamp>());

                SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);

                for (int i = 0; i < 150; i++)
                {
                    Vector2 vel = new Vector2(1, 0).RotatedByRandom(MathHelper.Pi * 2) * Main.rand.Next(20);
                    int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                    Dust.NewDust(NPC.Center, 0, 0, DustType, vel.X, vel.Y, 100, new Color(), 1f);
                }
                Draw = true;
                NPC.dontTakeDamage = false;
            }

            if (NPC.ai[1] == 180)
            {
                P1state = -3;
                oldP1state = P1state;
                P1stateReset();
            }
        }
        public void P1ShotSpam()
        {
            if (FargoSoulsWorld.MasochistModeReal)
                NPC.velocity *= 0.95f;
            else
                FlyingState(0.15f);

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
                NPC.localAI[1] = 0;
                //Rampup = 1;
            }

            //if (NPC.ai[2] >= (60 - (11 * Rampup)))
            //{
            //    SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

            //    if (Main.netMode != NetmodeID.MultiplayerClient)
            //    {
            //        Vector2 shootatPlayer2 = NPC.DirectionTo(Player.Center) * Rampup * 3.2f;
            //        for (int i = -1; i < 2; i++)
            //        {
            //            int type = Rampup == 5 ? ModContent.ProjectileType<LifeSplittingProjSmall>() : ModContent.ProjectileType<LifeProjSmall>();
            //            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootatPlayer2.RotatedBy(i * MathHelper.Pi / 5), type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
            //        }
            //    }
            //    if (Rampup < 5)
            //    {
            //        Rampup++;
            //    }
            //    NPC.ai[2] = 0f;
            //}
            //NPC.ai[2]++;

            if (NPC.ai[2] > 60)
            {
                int threshold = FargoSoulsWorld.MasochistModeReal ? 5 : 6;
                if (++NPC.localAI[1] % threshold == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                    float finalSpreadOffset = MathHelper.Pi / (FargoSoulsWorld.MasochistModeReal ? 8 : 5);
                    float startOffset = (MathHelper.Pi - finalSpreadOffset) * 0.9f;
                    const int timeToFocus = 60;

                    float rampRatio = (float)Math.Min(1f, NPC.localAI[1] / timeToFocus);
                    float rotationToUse = finalSpreadOffset + startOffset * (float)Math.Cos(MathHelper.PiOver2 * rampRatio);

                    Vector2 vel = NPC.DirectionTo(Player.Center);
                    vel *= 3f + 12f * rampRatio;

                    int projType = NPC.localAI[1] > timeToFocus ? ModContent.ProjectileType<LifeSplittingProjSmall>() : ModContent.ProjectileType<LifeProjSmall>();

                    for (int i = -1; i <= 1; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel.RotatedBy(rotationToUse * i), projType, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }
                }
            }

            if (NPC.ai[1] >= 300f)
            {
                oldP1state = P1state;
                P1stateReset();
            }
        }
        public void P1Nuke()
        {
            if (FargoSoulsWorld.MasochistModeReal)
                NPC.velocity *= 0.95f;
            else
                FlyingState(0.5f);

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] == 70f)
            {
                SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float ProjectileSpeed3 = 12f;
                    Vector2 shootatPlayer3 = NPC.DirectionTo(Player.Center) * ProjectileSpeed3;
                    float ai1 = FargoSoulsWorld.EternityMode ? 1 : 0;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootatPlayer3, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.5f), 300f, Main.myPlayer, 32f, ai1);
                }
            }
            if (NPC.ai[1] >= 145f)
            {
                oldP1state = P1state;
                P1stateReset();
            }
        }
        public void P1Pixies()
        {
            if (FargoSoulsWorld.MasochistModeReal)
                NPC.velocity *= 0.95f;
            else
                FlyingState(0.2f);

            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] % 10 == 9 && NPC.ai[1] < 130f)
            {
                SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -1; i <= 1; i += 2) //-1 and 1
                    {
                        float SoulX = NPC.Center.X - (200 - (NPC.ai[2]/10) * 50);
                        float SoulY = NPC.Center.Y + (i * 50);
                        Vector2 SoulPos = new Vector2(SoulX, SoulY);
                        Vector2 soulVel = new Vector2(0f, i * 5f);
                        float ai0 = 0;
                        if (!FargoSoulsWorld.MasochistModeReal)
                        {
                            ai0 = -30;
                            soulVel /= 2;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), SoulPos, soulVel, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, ai0, NPC.whoAmI);
                    }
                }
                NPC.netUpdate = true;
            }
            int endtime = FargoSoulsWorld.MasochistModeReal ? 200 : 300;
            if (NPC.ai[1] >= endtime)
            {
                oldP1state = P1state;
                P1stateReset();
            }
        }
        //UNUSED
        /*public void P1Wave()
        {
            Player Player = Main.player[NPC.target];
            if (LifeWaveCount >= 3) //only 3 of these per fight
            {
                RandomizeP1state();
                return;
            }

            if (AttackF1)
            {
                //only do attack when in range
                Vector2 targetPos = Player.Center;
                targetPos.Y -= 16 * 15;
                if (NPC.Distance(targetPos) < 16 * 10 || FargoSoulsWorld.MasochistModeReal)
                {
                    AttackF1 = false;
                    NPC.netUpdate = true;
                }
                else
                {
                    FlyingState(1.5f);
                    NPC.velocity = NPC.DirectionTo(targetPos) * NPC.velocity.Length();
                    NPC.ai[1] -= 1; //negate the usual increment
                    NPC.ai[2] -= 1;
                    return;
                }
            }

            NPC.velocity *= 0.9f;

            if (NPC.ai[2] >= 45f)
            {
                SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int k = 0; k < 45; k++)
                    {
                        float knockBack6 = 3f;
                        double rotationrad2 = MathHelper.ToRadians(4 * k - 90);
                        Vector2 shootrandom2 = (NPC.DirectionTo(Player.Center) * 0.8f).RotatedBy(rotationrad2);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootrandom2, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack6, Main.myPlayer);
                    }
                    NPC.ai[2] = 0f;
                }
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] >= 170f)
            {
                NPC.netUpdate = true;
                LifeWaveCount++;
                oldP1state = P1state;
                P1stateReset();
            }
        }*/
        public void P1Mines()
        {
            if (FargoSoulsWorld.MasochistModeReal)
                NPC.velocity *= 0.95f;
            else
                FlyingState(0.5f);

            Player Player = Main.player[NPC.target];
            
            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.type == ModContent.ProjectileType<LifeBombExplosion>())
                        {
                            //make them fade
                            p.ai[0] = Math.Max(p.ai[0], 2400 - 30);
                            p.netUpdate = true;
                        }
                    }
                }
            }

            if (NPC.ai[1] > 0 && NPC.ai[1] % 70f == 0)
            {
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                int max = 14;// Main.expertMode ? 14 : 10;
                for (int i = 0; i < max; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float bigSpeed = Main.rand.NextFloat(25, 172); //172 goes to edge of arena
                        int maxDegreeRand = 40;// Main.expertMode ? 60 : 40;
                        double rotationrad = MathHelper.ToRadians(Main.rand.NextFloat(-maxDegreeRand, maxDegreeRand));
                        Vector2 shootrandom = (NPC.DirectionTo(Player.Center) * (bigSpeed / 6f)).RotatedBy(rotationrad);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootrandom, ModContent.ProjectileType<LifeBomb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                    }
                }
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] >= 70f * 3.5f)
            {
                oldP1state = P1state;
                P1stateReset();
            }
        }
        public void P1PeriodicNuke()
        {
            Player Player = Main.player[NPC.target];
            if (NPC.ai[3] > 600f)
            {
                SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float ProjectileSpeed = 8f;
                    float knockBack = 300f;
                    Vector2 shootatPlayer = NPC.DirectionTo(Player.Center) * ProjectileSpeed;
                    float ai1 = FargoSoulsWorld.EternityMode ? 1 : 0;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero - shootatPlayer, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.5f), knockBack, Main.myPlayer, 32f, ai1);
                    NPC.ai[3] = 0f;
                }
                NPC.netUpdate = true;
            }
            NPC.ai[3] += 1f;
        }
        public void P1Transition()
        {
            Charging = false;
            Flying = false;

            useDR = true;

            NPC.velocity *= 0.95f;

            if (RPS < 0.2f) //speed up rotation up
            {
                RPS += (0.1f / 100);
            }
            else
            {
                RPS = 0.2f;
            }

            //if (FargoSoulsWorld.MasochistModeReal)
            //{
            //    int heal = (int)(NPC.lifeMax / 100f * Main.rand.NextFloat(1f, 1.5f));
            //    NPC.life += heal;
            //    if (NPC.life > NPC.lifeMax)
            //        NPC.life = NPC.lifeMax;
            //    CombatText.NewText(NPC.Hitbox, CombatText.HealLife, heal);
            //}
            if (NPC.ai[1] < 60)
            {
                if (NPC.ai[1] % 5 == 0)
                    SoundEngine.PlaySound(SoundID.Tink, Main.LocalPlayer.Center);

                if (PyramidWobble > 0)
                    PyramidWobble -= 0.2f;
                
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Gold, Scale: 1.5f);
                Main.dust[dust].velocity = (Main.dust[dust].position - NPC.Center) / 10;
            }
            if (NPC.ai[1] == 60f)
            {
                SoundEngine.PlaySound(SoundID.Item62, Main.LocalPlayer.Center);
                for (int i = 0; i < 60; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Gold, Scale:1.5f);
                    Main.dust[dust].velocity = (Main.dust[dust].position - NPC.Center) / 5;
                }
            }
            if (NPC.ai[1] >= 60f)
            {
                SpritePhase = 2;
                if (ChunkDistance < ChunkDistanceMax)
                    ChunkDistance ++;
                
            }

            if (NPC.ai[1] == 180f)
            {
                SoundEngine.PlaySound(SoundID.Item82, Main.LocalPlayer.Center);

                if (!Main.dedServ)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 60;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.type == ModContent.ProjectileType<LifeBombExplosion>())
                        {
                            //make them fade
                            p.ai[0] = Math.Max(p.ai[0], 2400 - 30);
                            p.netUpdate = true;
                        }
                    }
                }

                PhaseOne = false;
                NPC.netUpdate = true;
                NPC.TargetClosest(true);
                NPC.netUpdate = true;
                AttackF1 = true;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.ai[0] = 0f;
                state = 0;
            }
        }

        #endregion
        #region P2-3
        public void FlyingState(float speedModifier = 1f)
        {
            Flying = true;

            //basically, create a smooth transition when using different speedMod values
            float accel = 0.5f / 30f;
            if (NPC.localAI[3] < speedModifier)
            {
                NPC.localAI[3] += accel;
                if (NPC.localAI[3] > speedModifier)
                    NPC.localAI[3] = speedModifier;
            }
            if (NPC.localAI[3] > speedModifier)
            {
                NPC.localAI[3] -= accel;
                if (NPC.localAI[3] < speedModifier)
                    NPC.localAI[3] = speedModifier;
            }
            speedModifier = NPC.localAI[3];

            Player Player = Main.player[NPC.target];
            //flight AI
            float flySpeed = 0f;
            float inertia = 10f;
            Vector2 AbovePlayer = new Vector2(Player.Center.X, Player.Center.Y - 300f);
            if (state == 8)
            {
                AbovePlayer.Y = Player.Center.Y - 700f;
            }
            bool Close = ((Math.Abs(AbovePlayer.Y - NPC.Center.Y) < 32f && Math.Abs(AbovePlayer.X - NPC.Center.X) < 160f) ? true : false);
            if (!Close && NPC.Distance(AbovePlayer) < 500f)
            {
                flySpeed = 9f;
                if (!flyfast)
                {
                    Vector2 flyabovePlayer3 = NPC.DirectionTo(AbovePlayer) * flySpeed;
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + flyabovePlayer3) / inertia;
                }
            }
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.velocity = NPC.DirectionTo(AbovePlayer) * 1f;
            }
            if (NPC.Distance(AbovePlayer) > 360f)
            {
                flySpeed = NPC.Distance(AbovePlayer) / 35f;
                flyfast = true;
                Vector2 flyabovePlayer2 = NPC.DirectionTo(AbovePlayer) * flySpeed;
                NPC.velocity = (NPC.velocity * (inertia - 1f) + flyabovePlayer2) / inertia;
            }
            if (flyfast && (NPC.Distance(AbovePlayer) < 100f || NPC.Distance(Player.Center) < 100f))
            {
                flyfast = false;
                Vector2 flyabovePlayer = NPC.DirectionTo(AbovePlayer) * flySpeed;
                NPC.velocity = flyabovePlayer;
            }

            //orientation
            if (NPC.velocity.ToRotation() > MathHelper.Pi)
            {
                NPC.rotation = 0f - MathHelper.Pi * NPC.velocity.X * speedModifier / 100;
            }
            else
            {
                NPC.rotation = 0f + MathHelper.Pi * NPC.velocity.X * speedModifier / 100;
            }

            NPC.position -= NPC.velocity * (1f - speedModifier);
        }
        //unused
        /*
        public void AttackP2Start()
        {
            Player Player = Main.player[NPC.target];
            NPC.velocity.X = 0f;
            NPC.velocity.Y = 0f;
            Charging = false;
            Flying = false;
            float ProjectileSpeed = 16f;
            if (AttackF1)
            {
                NPC.ai[0] = Main.rand.NextBool(2) ? 55 : -55;
                NPC.netUpdate = true;
                rotspeed = 0;
                AttackF1 = false;
            }
            if (NPC.ai[1] == 60f)
            {
                LockVector1 = (NPC.DirectionTo(Player.Center) * ProjectileSpeed).RotatedBy(MathHelper.ToRadians(NPC.ai[0]));
                NPC.netUpdate = true;
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Zombie_104") with { Volume = 0.5f }, NPC.Center);
            }
            if (NPC.ai[1] > 60f)
            {
                //this is unnecessarily complicated but works and i personally advise you shouldn't touch it
                Vector2 PV = NPC.DirectionTo(Player.Center);
                Vector2 LV = LockVector1;
                float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
                float RotAccel = 0.008f * anglediff;
                float rotMinSpeed = 0.15f; //very important
                float rotMaxSpeed = 0.8f * anglediff + rotMinSpeed * Math.Sign(anglediff);
                //change rotation towards player
                LockVector1 = LockVector1.RotatedBy(rotspeed * MathHelper.Pi / 180);
                if (rotspeed > Math.Abs(rotMaxSpeed) || rotspeed < -Math.Abs(rotMaxSpeed))
                {
                    rotspeed = rotMaxSpeed;
                }
                else
                {
                    rotspeed += RotAccel + (rotMinSpeed * Math.Sign(RotAccel));
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(LockVector1),
                                ModContent.ProjectileType<LifeChalDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0, NPC.whoAmI);
                }
            }
            if (NPC.ai[3] > 55f)
            {
                SoundEngine.PlaySound(SoundID.Item25, NPC.Center);

                NPC.netUpdate = true;
                int amount = 6;
                for (int m = 0; m <= amount; m++)
                {
                    float knockBack9 = 3f;
                    double rad5 = (double)m * (360 / amount) * (MathHelper.Pi / 180.0);
                    Vector2 shootoffset5 = new Vector2(0f, 3f).RotatedBy(rad5);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset5, ModContent.ProjectileType<LifeBee>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack9, Main.myPlayer, 0, NPC.ai[1]);
                    }
                }
                NPC.ai[3] = 0f;

            }
            NPC.ai[3] += 1f;
            if (NPC.ai[1] > 775f)
            {
                Phase = 4.0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                NPC.dontTakeDamage = false;
                NPC.netUpdate = true;
            }
        }*/
        public void P3Transition()
        {
            Flying = true;
            useDR = true;
            PhaseThree = true;
            statecount = 10;
            availablestates.Clear();

            if (NPC.ai[1] == 120f)
            {
                SoundEngine.PlaySound(SoundID.Item82, Main.LocalPlayer.Center);
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 60;
                SpritePhase = 3;
            }
            if (NPC.ai[1] >= 180f) //REWORK: AXED HEAL ON P3 TRANSITION AND PUT ON HALF HEALTH
            {
                NPC.netUpdate = true;
                state = 99;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.ai[0] = 0f;
                AttackF1 = true;
                NPC.dontTakeDamage = true;
                NPC.TargetClosest(true);
            }
        }
        public void AttackP3Start()
        {
            useDR = true;

            if (AttackF1)
            {
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Zombie_104") with { Volume = 0.5f }, NPC.Center);
                AttackF1 = false;
                NPC.velocity.X = 0;
                NPC.velocity.Y = 0;
                Flying = false;
                if (FargoSoulsWorld.MasochistModeReal)
                    NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                rotspeed = 0;
                rot = 0;
            }

            Player Player = Main.player[NPC.target];
            if (NPC.Distance(Player.Center) > 2000)
            {
                FlyingState(1.5f);
            }
            else
            {
                Flying = false;
                NPC.velocity *= 0.9f;
            }

            //for a starting time, make it fade in, then make it spin faster and faster up to a max speed
            int fadeintime = 10;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] < fadeintime)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0f, -1f),
                                ModContent.ProjectileType<LifeChalDeathray>(), 0, 0f, Main.myPlayer, 0, NPC.whoAmI);
            }

            int endTime = 1650;
            if (!FargoSoulsWorld.MasochistModeReal)
                endTime = 1237;

            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] >= fadeintime)
            {
                if (rotspeed < 0.88f)
                {
                    rotspeed += (double)((2f / 60) / 4);
                }
                rot += ((MathHelper.Pi / 180) * rotspeed);
                Vector2 rotV = new Vector2(0f, -1f).RotatedBy(rot);
                int rayDamage = FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, FargoSoulsWorld.MasochistModeReal ? 4f : 1.5f);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, rotV,
                    ModContent.ProjectileType<LifeChalDeathray>(), rayDamage, 0f, Main.myPlayer, 0, NPC.whoAmI);
                //randomly make Scar obstacles at specific points, obstacles have Projectile.ai[1] = NPC.ai[1]
                if (NPC.ai[1] % 8 == 0 && Main.netMode != NetmodeID.MultiplayerClient && rotspeed > 0.82f)
                {
                    if (intervalist.Count < 1)
                    {
                        intervalist.Clear();
                        for (int i = 0; i < 6; i++)
                        {
                            intervalist.Add(i);
                        }
                    }
                    index2 = Main.rand.Next(intervalist.Count);
                    NPC.netUpdate = true;
                    interval = intervalist[index2];
                    intervalist.RemoveAt(index2);

                    NPC.ai[0] = Main.rand.Next(200);
                    int dist = (200 * interval) + (int)NPC.ai[0];
                    Vector2 distV = NPC.Center - new Vector2(0f, dist).RotatedBy(rot);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), distV, Vector2.Zero, ModContent.ProjectileType<LifeScar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.ai[1], endTime);
                    NPC.netUpdate = true;
                }
            }

            if (NPC.ai[1] > endTime)
            {
                NPC.dontTakeDamage = false;
                Flying = true;
                oldstate = state;
                StateReset();
                rotspeed = 0;
                rot = 0;
            }
        }
        public void AttackFinal()
        {
            Player Player = Main.player[NPC.target];

            if (UseTrueOriginAI) //disable items
            {
                NPC.chaseable = false;

                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && NPC.Distance(Main.LocalPlayer.Center) < 3000)
                {
                    if (Main.LocalPlayer.grapCount > 0)
                        Main.LocalPlayer.RemoveAllGrapplingHooks();

                    //Main.LocalPlayer.controlUseItem = false;
                    //Main.LocalPlayer.controlUseTile = false;
                    //Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = true;
                }
            }

            if (AttackF1) 
            {
                AttackF1 = false;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                Flying = true;
            }

            //for (int i = 0; i < Main.musicFade.Length; i++) //shut up music
            //    if (Main.musicFade[i] > 0f)
            //        Main.musicFade[i] -= 1f / 60;
            const int InitTime = 120;

            if (NPC.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient) // cage size is 600x600, 300 from center, 25 projectiles per side, 24x24 each
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<LifeCageTelegraph>(), 0, 0f, Main.myPlayer, ai1: Player.whoAmI);
            }
            if (NPC.ai[1] == InitTime)
            {
                SoundEngine.PlaySound(SoundID.DD2_DefenseTowerSpawn, Player.Center);
                for (int i = 0; i < 26; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(Player.Center.X - 300 + (600 * j), Player.Center.Y - 300 + (24 * i)), Vector2.Zero, ModContent.ProjectileType<LifeCageProjectile>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, j);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(Player.Center.X - 300 + (24 * i), Player.Center.Y - 300 + (600 * j)), Vector2.Zero, ModContent.ProjectileType<LifeCageProjectile>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, 2 + j);
                        }
                    }
                }
                /*if (Main.netMode != NetmodeID.MultiplayerClient) //bars
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<LifeCageBars>(), 0, 0, Main.myPlayer);
                }*/
                LockVector1 = Player.Center;
                NPC.netUpdate = true;
            }

            if (NPC.ai[1] > InitTime) //make sure to teleport any player outside the cage inside
            {
                if ((Main.LocalPlayer.active && ((Math.Abs(Main.LocalPlayer.Center.X - LockVector1.X) > 320) || (Math.Abs(Main.LocalPlayer.Center.Y - LockVector1.Y) > 320))) && (Main.LocalPlayer.active && ((Math.Abs(Main.LocalPlayer.Center.X - LockVector1.X) < 1500) || (Math.Abs(Main.LocalPlayer.Center.Y - LockVector1.Y) < 1500))))
                {
                    Main.LocalPlayer.position = LockVector1;
                }
            }
            //attack 1: arena is divided in 9 squares, only 1 is safe
            #region GridShots (removed)
            //comments within consts is to remove attack time
            const int Attack1Time = 80;
            const int Attack1Start = InitTime + 40;
            const int Attack1End = Attack1Start /*+ (Attack1Time * 5)*/;
            const int telegdist = 175;

            /*
            int time1 = (int)NPC.ai[1] - Attack1Start;

            if (NPC.ai[1] > Attack1Start && time1 % Attack1Time == 0 && NPC.ai[1] < Attack1End) // get random choice
            {
                SoundEngine.PlaySound(SoundID.Unlock, Player.Center);
                NPC.ai[0] = Main.rand.Next(3);
                NPC.ai[2] = Main.rand.Next(3);
                NPC.netUpdate = true;

                for (int i = 0; i < 3; i++) // chosen telegraphs (all but the Chosen One)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && (i != NPC.ai[0] || j != NPC.ai[2]))
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector1.X + (telegdist * (i - 1)), LockVector1.Y + (telegdist * (j - 1))), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, -Attack1Time, 3);
                        }
                    }
                }
            }
            */
            //end of square attack
            #endregion
            #region BulletHell
            //start of shooting attack: cum god fires a nuke or two straight up while he shoots slow shots straight down from him
            const int Attack2Time = 25;
            const int Attack2Start = Attack1End + 60;
            const int Attack2End = Attack2Start + (60 * 8);
            int time2 = (int)NPC.ai[1] - Attack2Start;

            if (NPC.ai[1] > Attack2Start && time2 % (Attack2Time * 3) + 1 == 1 && NPC.ai[1] < Attack2End) //cum nuke up
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    for (int i = 0; i < 2; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-4 + (8 * i), -2f), ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.5f), 3f, Main.myPlayer, 24f);
            }
            if (NPC.ai[1] > Attack2Start && time2 % (Attack2Time * 2) + 1 == 1 && NPC.ai[1] < Attack2End) //fire shots down
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, 2.5f), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
            }
            #endregion
            #region Jevilsknife (removed)
            //comments within consts is to remove attack duration
            const int Attack3Start = Attack2End /*+ 300*/;
            const int Attack3End = Attack3Start /*+ (60 * 12)*/;
            /*
            int time3 = (int)NPC.ai[1] - Attack3Start;
            if (NPC.ai[1] >= Attack3Start && NPC.ai[1] < Attack3End)
                NPC.position = LockVector1 + new Vector2(-(NPC.width / 2), -600 - (NPC.height / 2)); //position self above cage

            if (NPC.ai[1] == Attack3Start) // get random
            {
                NPC.ai[0] = Main.rand.Next(90);
                NPC.ai[2] = Main.rand.Next(2); //random which 2 spots the aimed shots start at
                //position of knife: start pos + spin speed (1.75) * time since start, offset by 45 deg to get space in between them
                choice = NPC.ai[0] + 45;
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] >= Attack3Start && time3 == 10) // spawn circle
            {
                SoundEngine.PlaySound(SoundID.Item71, LockVector1);
                Vector2 pos = LockVector1 - new Vector2(500, 0);
                Vector2 ScopeToPos = pos - LockVector1;
                for (int i = 0; i < 4; i++)
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + ScopeToPos.RotatedBy((i * MathHelper.Pi / 2) + MathHelper.ToRadians(NPC.ai[0])),
            Vector2.Zero, ModContent.ProjectileType<JevilScar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.whoAmI, i * 90 + NPC.ai[0]);
            }
            float spin = choice * MathHelper.Pi / 180f;
            if (time3 >= 10 && NPC.ai[1] <= Attack3End - 20 && (time3 - 10) % 100 == 1) //telegraphs (spinning with knife)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 1; j < 3; j++)
                        {
                            Vector2 offset = (250 / j * spin.ToRotationVector2()).RotatedBy(i * MathHelper.Pi);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + offset, offset, ModContent.ProjectileType<LifeCrosshair>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, -90, 1);
                        }
                    }
                }
            }
            if (time3 > 10 && (time3 - 10) % 100 == 0 && NPC.ai[1] <= Attack3End) //shoot when knives are at peak length
            {
                SoundEngine.PlaySound(SoundID.Item41, LockVector1);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 1; j < 3; j++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + ((250 / j) * (choice * MathHelper.Pi / 180f).ToRotationVector2() / j).RotatedBy(i * MathHelper.Pi), Vector2.Zero, ModContent.ProjectileType<LifeCageExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                        }
                    }
                }
            }
            if (time3 >= 60)
            {
                // degrees/bounce rotation
                // check this matches in LifeCrosshair
                choice += 1.75f;
            }
            if (NPC.ai[1] >= Attack3Start && time3 % 100 == 0 && NPC.ai[1] < Attack3End) // periodic sound
            {
                SoundEngine.PlaySound(SoundID.Item71, LockVector1);
            }
            // square walls from bottom and right
            */
            #endregion
            #region Excel (removed)

            //comments within consts is to remove duration
            const int Attack4Time = 75;
            const int Attack4Start = Attack3End /*- 120*/; //start earlier so they overlap
            const int Attack4End = Attack4Start /*+ (Attack4Time * 8) + 240*/;
            /*
            int time4 = (int)NPC.ai[1] - Attack4Start;
            if (NPC.ai[1] >= Attack4Start && time4 % Attack4Time + 1 == 1 && NPC.ai[1] < Attack4End - 240) // get random
            {
                NPC.ai[0] = Main.rand.Next(-60, 60);
                NPC.ai[2] = Main.rand.Next(-60, 60);
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] >= Attack4Start && time4 % Attack4Time + 1 == Attack4Time && NPC.ai[1] < Attack4End - 240) // spawn walls
            {
                SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                NPC.localAI[1]++;
                for (int i = -10; i <= 10; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + new Vector2(1000, i * 120 + NPC.ai[0]), new Vector2(-3, 0), ModContent.ProjectileType<LifeProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.localAI[1] + (1000 * i) + (100000), 1);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + new Vector2(i * 120 + NPC.ai[2], 1000), new Vector2(0, -3), ModContent.ProjectileType<LifeProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.localAI[1] + (1000 * i) + (200000), 1);
                    }
                }
            }
            if (time4 == 200) //respawn cage because projectile limit
            {
                for (int i = 0; i < 26; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector1.X - 300 + (600 * j), LockVector1.Y - 300 + (24 * i)), Vector2.Zero, ModContent.ProjectileType<LifeCageProjectile>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, j);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector1.X - 300 + (24 * i), LockVector1.Y - 300 + (600 * j)), Vector2.Zero, ModContent.ProjectileType<LifeCageProjectile>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, 2 + j);
                        }
                    }
                }
            }
            */
            #endregion
            
            #region Blaster1
            //GASTER BLASTER 1
            const int Attack5Time = 90;
            const int Attack5Start = Attack4End + 60;
            const int Attack5End = Attack5Start + (Attack5Time * 8);
            int time5 = (int)NPC.ai[1] - Attack5Start;
            if (NPC.ai[1] >= Attack5Start && time5 % Attack5Time + 1 == 1 && NPC.ai[1] < Attack5End) // get random angle
            {
                NPC.ai[0] = Main.rand.Next(-90, 90);
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] >= Attack5Start && time5 % Attack5Time + 1 == Attack5Time && NPC.ai[1] < Attack5End) // spawn blasters
            {
                Vector2 aim = new Vector2(0, 450);
                if (firstblaster < 1 || firstblaster > 1)
                    SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                for (int i = 0; i <= 12; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && (firstblaster < 1 || firstblaster > 1))
                    {
                        Vector2 vel = -Vector2.Normalize(aim).RotatedBy((i * MathHelper.Pi / 6) + MathHelper.ToRadians(NPC.ai[0]));
                        float ai0 = vel.ToRotation();
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + aim.RotatedBy((i * MathHelper.Pi / 6) + MathHelper.ToRadians(NPC.ai[0])), Vector2.Zero, ModContent.ProjectileType<LifeBlaster>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, ai0, firstblaster);
                    }
                }
                if (firstblaster > 0)
                    firstblaster -= 1;
                NPC.netUpdate = true;

            }
            #endregion
            #region Blaster2
            //GASTER BLASTER 2 FINAL BIG SPIN FINAL CUM GOD DONE DUN DEAL
            const int Attack6Time = 4;
            const int Attack6Start = Attack5End + 90;
            const int Attack6End = Attack6Start + (180 * 5); //2 seconds per rotation
            int time6 = (int)NPC.ai[1] - Attack6Start;
            if (NPC.ai[1] >= Attack6Start && time6 == 0) // reset NPC.ai[0]
            {
                NPC.ai[0] = 0;
                NPC.netUpdate = true;
                LockVector2 = Player.Center;
            }

            if (NPC.ai[1] > Attack6Start && time5 % Attack6Time == Attack6Time - 1 && NPC.ai[1] < Attack6End) // spawn blasters. 1 every 4th frame, 2 seconds per rotation, 45 total
            {
                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                Vector2 aim = (Vector2.Normalize(LockVector2 - LockVector1) * 550).RotatedBy(MathHelper.PiOver2);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = -Vector2.Normalize(aim).RotatedBy(NPC.ai[0] * MathHelper.Pi / 18);
                    float ai0 = vel.ToRotation();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1 + aim.RotatedBy(NPC.ai[0] * MathHelper.Pi / 18), Vector2.Zero, ModContent.ProjectileType<LifeBlaster>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, ai0);
                }
                NPC.netUpdate = true;
                NPC.ai[0] += 1;
            }
            #endregion
            #region End
            int end = Attack6End + 120;
            if (NPC.ai[1] >= end)
            {

                UseTrueOriginAI = false;
                NPC.dontTakeDamage = false;
                SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center);
            }
            if (NPC.ai[1] >= end && NPC.ai[1] % 10 == 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType);
                }

                NPC.position = NPC.position + new Vector2(Main.rand.Next(-60, 60), Main.rand.Next(-60, 60));
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
            }
            if (NPC.ai[1] == end + 90)
            {
                NPC.life = 0;
                NPC.checkDead();
                //there was dialogue here before
            }
            #endregion
        }
        public void AttackSlurpBurp()
        {
            useDR = true;

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                //only do attack when in range
                Vector2 targetPos = Player.Center;
                targetPos.Y -= 16 * 15;
                if (NPC.Distance(targetPos) < 16 * 10 || FargoSoulsWorld.MasochistModeReal)
                {
                    AttackF1 = false;
                    NPC.netUpdate = true;
                }
                else
                {
                    FlyingState();
                    NPC.velocity = NPC.DirectionTo(targetPos) * NPC.velocity.Length();
                    NPC.ai[1] = 0; //negate the usual increment
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    return;
                }
            }

            if (NPC.Distance(Player.Center) > 2000)
            {
                FlyingState(1.5f);
            }

            NPC.velocity = Vector2.Zero;
            Flying = false;

            float knockBack = 3f;
            double rad = (double)NPC.ai[1] * 5.721237 * (MathHelper.Pi / 180.0);
            
            double dustdist = 1200;
            if (!FargoSoulsWorld.MasochistModeReal)
            {
                float distanceToPlayer = NPC.Distance(Player.Center);
                distanceToPlayer += 240;
                dustdist = Math.Max(dustdist, distanceToPlayer); //take higher of these values
                dustdist = Math.Min(dustdist, 2400); //capped at this value
            }

            int DustX = (int)NPC.Center.X - (int)(Math.Cos(rad) * dustdist);
            int DustY = (int)NPC.Center.Y - (int)(Math.Sin(rad) * dustdist);
            Vector2 DustV = new Vector2(DustX, DustY);
            if (NPC.ai[2] >= 2f && NPC.ai[1] <= 300f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), DustV, Vector2.Zero, ModContent.ProjectileType<LifeSlurp>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack, Main.myPlayer, 0, NPC.whoAmI);
                }
                
                NPC.ai[2] = 0f;
            }
            NPC.ai[2] += 1f;
            if (NPC.ai[1] < 300f)
            {
                if (NPC.ai[3] > 15f)
                {
                    SoundEngine.PlaySound(SoundID.Item101, DustV);

                    if (PhaseThree && shoot != false) //extra projectiles during p3
                    {
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        
                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            float ProjectileSpeed = 10f;
                            float knockBack2 = 3f;
                            Vector2 shootatPlayer = NPC.DirectionTo(Player.Center) * ProjectileSpeed;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootatPlayer, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
                        }

                        //for (int i = -2; i <= 2; i++)
                        //{
                        //    if (Main.netMode != NetmodeID.MultiplayerClient)
                        //        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 0.9f * NPC.DirectionTo(Player.Center).RotatedBy(MathHelper.ToRadians(3) * i), ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, -60, 2f);
                        //}

                        shoot = false;
                    }
                    else
                    {
                        shoot = true;
                    }
                    NPC.ai[3] = 0f;
                }
                NPC.ai[3] += 1f;
            }

            if (NPC.ai[1] > 300f && NPC.ai[1] < 600f)
            {
                if (NPC.ai[3] > 15f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                    NPC.ai[3] = 0f;
                }
                NPC.ai[3] += 1f;
            }

            if (!FargoSoulsWorld.MasochistModeReal && !PhaseThree && NPC.ai[1] < 120)
            {
                NPC.ai[2] -= 0.5f;
                NPC.ai[3] -= 0.5f;
            }

            if (NPC.ai[1] >= 660f)
            {
                oldstate = state;
                Flying = true;
                StateReset();
            }
        }

        public void AttackShotgun()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
                Flying = true;
                if (PhaseThree)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }
            }
            if (PhaseThree) //p3 variant
            {
                Flying = false;
                float flySpeed2 = 7f;
                float inertia2 = 7f;
                Vector2 flyonPlayer = NPC.DirectionTo(Player.Center) * flySpeed2;
                NPC.velocity = (NPC.velocity * (inertia2 - 1f) + flyonPlayer) / inertia2;

                //rotation
                if (NPC.velocity.ToRotation() > MathHelper.Pi)
                {
                    NPC.rotation = 0f - MathHelper.Pi * NPC.velocity.X / 100;
                }
                else
                {
                    NPC.rotation = 0f + MathHelper.Pi * NPC.velocity.X / 100;
                }
                if (NPC.ai[3] < 3f)
                {
                    NPC.ai[3] = 3f;
                }
                if (NPC.ai[2] >= 120f)
                {
                    SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                    float ProjectileSpeed = 10f;
                    float knockBack2 = 3f;
                    Vector2 shootatPlayer = NPC.DirectionTo(Player.Center) * ProjectileSpeed;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spread = 10;
                        for (int i = 0; (float)i <= NPC.ai[3]; i++)
                        {
                            double rotationrad = MathHelper.ToRadians(0f - NPC.ai[3] * spread / 2 + (float)(i * spread));
                            Vector2 shootoffset = shootatPlayer.RotatedBy(rotationrad);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
                        }
                    }
                    NPC.ai[3] += 1f;
                    NPC.ai[2] = 80f;
                }
                
            }
            else //p2 variant, unused rn
            {
                if (NPC.ai[3] < 3f)
                {
                    NPC.ai[3] = 3f;
                }
                float ProjectileSpeed = 30f;
                float knockBack2 = 3f;
                int spread = 18 - (int)(NPC.ai[3] - 3); //gets tighter after each shot
                if (NPC.ai[2] == 41f)
                {
                    LockVector1 = NPC.Center;
                    LockVector2 = (NPC.DirectionTo(Player.Center) * ProjectileSpeed).RotatedBy(MathHelper.Pi / 80 * (Main.rand.NextFloat() - 0.5f));
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; (float)i <= NPC.ai[3]; i++)
                        {
                            double rotationrad = MathHelper.ToRadians(0f - NPC.ai[3] * spread / 2 + (float)(i * spread));
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0, LockVector2.RotatedBy(rotationrad).ToRotation());
                        }
                    }
                    NPC.netUpdate = true;
                }
                if (NPC.ai[2] >= 80f)
                {
                    SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; (float)i <= NPC.ai[3]; i++)
                        {
                            double rotationrad = MathHelper.ToRadians(0f - NPC.ai[3] * spread / 2 + (float)(i * spread));
                            Vector2 shootoffset = LockVector2.RotatedBy(rotationrad);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1, shootoffset, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
                        }
                    }
                    NPC.ai[3] += 1f;
                    NPC.ai[2] = 40f;
                }
            }
            NPC.ai[2] += 1f;
            if (NPC.ai[3] >= 12f)
            {
                Flying = true;
                oldstate = state;
                StateReset();
            }
        }
        public void AttackCharge()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                LockVector3 = NPC.Center;
                AttackF1 = false;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center);
            }
            Flying = false;
            Charging = true;
            HitPlayer = true;
            AuraCenter = LockVector3; //lock arena in place during charges
            if (PhaseThree) //tp
            {
                if (NPC.ai[1] == 0f)
                {
                    NPC.ai[2] = Main.rand.Next(360);
                    if (NPC.ai[3] >= 6f)
                    {
                        NPC.ai[2] = 90f;
                    }
                }
                double rad3 = (double)NPC.ai[2] * (MathHelper.Pi / 180.0);
                double tpdist = 350.0;
                int TpX = (int)Player.Center.X - (int)(Math.Cos(rad3) * tpdist);
                int TpY = (int)Player.Center.Y - (int)(Math.Sin(rad3) * tpdist);
                Vector2 TpPos = new Vector2(TpX, TpY);

                NPC.localAI[0] = TpPos.X; //exposing these so proj can access them
                NPC.localAI[1] = TpPos.Y;

                if (NPC.ai[1] == 1f && Main.netMode != NetmodeID.MultiplayerClient) //telegraph
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(TpPos.X + (float)(NPC.width / 2), TpPos.Y + (float)(NPC.height / 2)), Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -70);
                }
                if (NPC.ai[1] == 75f) //tp
                {
                    NPC.Center = new Vector2(TpX, TpY);
                    NPC.velocity.X = 0f;
                    NPC.velocity.Y = 0f;
                    NPC.rotation = NPC.DirectionTo(Player.Center).ToRotation();
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center); //PLACEHOLDER
                    NPC.netUpdate = true;
                }
            }
            if (((NPC.ai[1] == 60f && Main.netMode != NetmodeID.MultiplayerClient && !PhaseThree) || (NPC.ai[1] == 80f && Main.netMode != NetmodeID.MultiplayerClient && PhaseThree)) && NPC.ai[3] < 6f)
            {
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                //circle of cum before charge
                float ProjectileSpeed = 10f;
                Vector2 shootatPlayer = NPC.DirectionTo(Player.Center) * ProjectileSpeed;
                int amount = 14;
                for (int i = 0; i < amount; i++)
                {
                    Vector2 shootoffset = shootatPlayer.RotatedBy(i * (MathHelper.Pi / (amount / 2)));
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset, ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                }
                //charge
                float chargeSpeed = 22f;
                Vector2 chargeatPlayer = NPC.DirectionTo(Player.Center) * chargeSpeed;
                NPC.velocity = chargeatPlayer;
                NPC.ai[2] = Main.rand.Next(360);
                NPC.netUpdate = true;
                if (NPC.ai[3] >= 6f)
                {
                    NPC.ai[2] = 90f;
                }
                NPC.ai[1] = 0f;
                NPC.ai[3] += 1f;
            }
            if (!PhaseThree)
            {
                NPC.velocity = NPC.velocity * 0.99f;
            }
            if ((NPC.ai[3] >= 6f && NPC.ai[1] >= 75f && !PhaseThree) || (NPC.ai[3] >= 6f && NPC.ai[1] >= 105f && PhaseThree))
            {
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                HitPlayer = false;
                Flying = true;
                Charging = false;
                oldstate = state;
                StateReset();
            }
        }
        public void AttackPlunge()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                LockVector3 = NPC.Center;
                AttackF1 = false;
                NPC.netUpdate = true;
                Flying = true;
            }
            AuraCenter = LockVector3;

            Vector2 TpPos = new Vector2(Player.Center.X, Player.Center.Y - 400f);

            NPC.localAI[0] = TpPos.X; //exposing so proj can access
            NPC.localAI[1] = TpPos.Y;
            if (NPC.ai[1] == 1)
            {
                LockVector2 = new Vector2(Player.Center.X, Player.Center.Y - 400f);
            }
            if (NPC.ai[1] == 5 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                
                Projectile.NewProjectile(NPC.GetSource_FromThis(), TpPos, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -40);
                //below wall telegraph
                for (int i = 0; i < 60; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector2.X - 1500, LockVector2.Y + 400 + 500 + (60 * i)), Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0, 0);
                }
            }
            if (NPC.ai[1] == 45f)
            {
                Flying = false;
                Charging = true;
                NPC.Center = TpPos;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                NPC.rotation = MathHelper.Pi;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] == 60)
            {
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                HitPlayer = true;
                float chargeSpeed2 = 55f;
                NPC.velocity.Y = chargeSpeed2;
                NPC.netUpdate = true;
                //below wall
                if (FargoSoulsWorld.MasochistModeReal)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 120; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector2.X - 1200, LockVector2.Y + 600 + 500 + (30 * i)), new Vector2(60, 0), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                        }
                        for (int i = 0; i < 120; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector2.X + 1200, LockVector2.Y + 600 + 500 + (30 * i)), new Vector2(-60, 0), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                        }
                    }
                }
            }
            if (NPC.ai[1] >= 60)
            {
                HitPlayer = true;
                NPC.velocity = NPC.velocity * 0.96f;
            }
            if (NPC.ai[1] == 90 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float knockBack4 = 3f;
                Vector2 shootdown2 = new Vector2(0f, 10f);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                for (int k = 0; k <= 15; k++)
                {
                    double rotationrad3 = MathHelper.ToRadians(-90 + k * 12);
                    Vector2 shootoffset3 = shootdown2.RotatedBy(rotationrad3);
                    if (!FargoSoulsWorld.MasochistModeReal)
                        shootoffset3.X *= 2f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset3, ModContent.ProjectileType<LifeNeggravProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack4, Main.myPlayer);
                }
            }
            if (NPC.ai[1] == 105 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float knockBack3 = 3f;
                Vector2 shootdown = new Vector2(0f, 10f);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                for (int j = 0; j <= 15; j++)
                {
                    double rotationrad2 = MathHelper.ToRadians(-90 + j * 10);
                    Vector2 shootoffset2 = shootdown.RotatedBy(rotationrad2);
                    if (!FargoSoulsWorld.MasochistModeReal)
                        shootoffset2.X *= 2f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset2, ModContent.ProjectileType<LifeNeggravProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack3, Main.myPlayer);
                }
            }
            if (NPC.ai[1] == 110 && PhaseThree && NPC.ai[2] < 5f)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 10f;
            }
            if (NPC.ai[1] == 240)
            { //teleport back up
                NPC.position.X = Player.Center.X - (NPC.width / 2);
                NPC.position.Y = Player.Center.Y - ((NPC.height / 2) + 450f);
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                HitPlayer = false;
                Flying = true;
                Charging = false;
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] >= 300)
            {
                HitPlayer = false;
                oldstate = state;
                StateReset();
            }
        }
        public void AttackPixies()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                Flying = true;
                AttackF1 = false;
                NPC.netUpdate = true;
                NPC.ai[3] = 0;
                if (PhaseThree)
                {
                    SoundEngine.PlaySound(SoundID.ScaryScream, NPC.Center);
                    LockVector3 = NPC.Center;
                }
            }
            if (!PhaseThree) //p2 version
            {
                if (NPC.ai[2] > 60f && (NPC.ai[2] % 5) == 0 && NPC.ai[1] < 280)
                {
                    SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float knockBack10 = 3f;
                        Vector2 shootoffset4 = new Vector2(0f, -5f).RotatedBy(NPC.ai[3]);
                        NPC.ai[3] = (float)(Main.rand.Next(-30, 30) * (MathHelper.Pi / 180.0)); //change random offset after so game has time to sync
                        float ai0 = 0;
                        if (!FargoSoulsWorld.MasochistModeReal)
                        {
                            ai0 = -30;
                            shootoffset4 /= 2;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset4, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack10, Main.myPlayer, ai0, NPC.whoAmI);
                    }
                }
                NPC.ai[2] += 1f;
                int endtime = FargoSoulsWorld.MasochistModeReal ? 280 : 390;
                if (NPC.ai[1] > endtime)
                {
                    oldstate = state;
                    StateReset();
                }
            }
            if (PhaseThree) //p3 version
            {
                AuraCenter = LockVector3;
                Flying = false;
                Charging = true;
                if (PhaseThree && NPC.ai[1] == 60)
                {
                    LockVector1 = Player.Center;
                    NPC.netUpdate = true;
                }
                const int ChargeCD = 60;
                if (NPC.ai[2] == ChargeCD) //charge
                {
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                    float chargeSpeed = 22f;
                    Vector2 chargeatPlayer = NPC.DirectionTo(Player.Center) * chargeSpeed;
                    NPC.velocity = chargeatPlayer;
                    NPC.netUpdate = true;
                }
                if ((NPC.ai[1] % 5) == 0 && NPC.ai[2] > ChargeCD && NPC.ai[2] < ChargeCD + 40) //fire pixies during charges
                {
                    SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float knockBack10 = 3f;
                        Vector2 shootoffset4 = Vector2.Normalize(NPC.velocity).RotatedBy(NPC.ai[3]) * 5f;
                        NPC.ai[3] = (float)(Main.rand.Next(-30, 30) * (MathHelper.Pi / 180.0)); //change random offset after so game has time to sync
                        float ai0 = 0;
                        if (!FargoSoulsWorld.MasochistModeReal)
                        {
                            ai0 = -30;
                            shootoffset4 /= 2;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset4, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack10, Main.myPlayer, ai0, NPC.whoAmI);
                    }
                }
                if (NPC.ai[2] >= ChargeCD + 60)
                {
                    NPC.ai[2] = ChargeCD - 1;
                }
                NPC.ai[2]++;
                NPC.velocity *= 0.99f;
                if (NPC.ai[1] >= ChargeCD * 5)
                {
                    Flying = true;
                    Charging = false;
                    oldstate = state;
                    StateReset();
                }
            }


        }
        public void AttackRoulette()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                Flying = false;
                NPC.velocity = Vector2.Zero;
                NPC.ai[3] = Main.rand.NextFloat(MathHelper.ToRadians(45)) * (Main.rand.NextBool() ? 1 : -1);
                if (Player.Center.X < NPC.Center.X)
                    NPC.ai[3] += MathHelper.Pi;
                NPC.netUpdate = true;
            }
            
            Vector2 RouletteTpPos = Player.Center + 500 * NPC.ai[3].ToRotationVector2();
            NPC.localAI[0] = RouletteTpPos.X; //exposing so proj can access
            NPC.localAI[1] = RouletteTpPos.Y;

            if (NPC.ai[1] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), RouletteTpPos, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -40);
            }

            if (NPC.ai[1] == 40)
            {
                NPC.Center = RouletteTpPos;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center); //PLACEHOLDER
                LockVector1 = NPC.DirectionTo(Player.Center);
                NPC.localAI[1] = 0;
                NPC.netUpdate = true;
            }

            if (NPC.ai[1] > 40)
            {
                float angleDiff = MathHelper.WrapAngle(NPC.DirectionTo(Player.Center).ToRotation() - LockVector1.ToRotation());
                if (Math.Abs(angleDiff) > MathHelper.Pi / 3f)
                {
                    LockVector1 = NPC.DirectionTo(Player.Center);
                    NPC.netUpdate = true;
                }
            }

            if (NPC.ai[1] < 420 + 120/*rework*/ && NPC.ai[1] % 4 == 0 && NPC.ai[1] > 60 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                const float speed = 20f;
                Vector2 offset1 = LockVector1.RotatedBy(MathHelper.Pi / 3f) * speed;
                Vector2 offset2 = LockVector1.RotatedBy(-MathHelper.Pi / 3f) * speed;
                //in p3, rotate offsets by +-5 degrees determined by sine curve, one loop is 4 seconds
                if (PhaseThree)
                {
                    float waveModifier = FargoSoulsWorld.MasochistModeReal ? 6.5f : 8f;
                    offset1 = offset1.RotatedBy((MathHelper.Pi / waveModifier) * Math.Sin(MathHelper.ToRadians(1.5f * NPC.ai[1])));
                    offset2 = offset2.RotatedBy((MathHelper.Pi / waveModifier) * -Math.Sin(MathHelper.ToRadians(1.5f * NPC.ai[1])));
                }

                //const int timeleft = 180;
                NPC.localAI[1]++;
                //int p =
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offset1, ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, 0, 2);
                //if (p != Main.maxProjectiles)
                //    Main.projectile[p].timeLeft = timeleft;
                //p = 
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offset2, ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, 0, 2);
                //if (p != Main.maxProjectiles)
                //    Main.projectile[p].timeLeft = timeleft;
            }

            //old random projectiles:
            /*
            if (NPC.ai[1] < 420 && NPC.ai[1] % 60 == 10 && NPC.ai[1] > 60) // get Choice half a second before because terraria netcode fucking sucks
            {
                if (choicelist.Count < 1)
                {
                    choicelist.Clear();
                    for (int j = 0; j < 4; j++)
                    {
                        choicelist.Add(j);
                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    index = Main.rand.Next(choicelist.Count);
                    if (oldchoice != choicelist[index]) //can't get same attack twice in a row
                    {
                        choice = choicelist[index];
                        oldchoice = (int)choice;
                        choicelist.RemoveAt(index);
                    }
                    else
                    {
                        int index2 = index;
                        choicelist.RemoveAt(index);
                        index = Main.rand.Next(choicelist.Count);
                        choice = choicelist[index];
                        choicelist.Add(index2);
                        oldchoice = (int)choice;
                        choicelist.RemoveAt(index);
                    }

                }
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] < 420 && NPC.ai[1] % 70 == 55 && NPC.ai[1] > 70) //fire a random assortment of things
            {
                switch (choice)
                {
                    case 0: //nuke

                        SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                        float knockBack10 = 300f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float ai0 = PhaseThree || FargoSoulsWorld.MasochistModeReal ? 32 : 24;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Player.Center) * 12f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.5f), knockBack10, Main.myPlayer, ai0);
                        }
                        break;
                    case 1: //small random spread
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        for (int i = 0; i < 10; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.ai[0] = Main.rand.Next(8, 13);
                                NPC.ai[2] = Main.rand.Next(-40, 40);
                                Vector2 offsetRAND = (NPC.DirectionTo(Player.Center) * NPC.ai[0]).RotatedBy((MathHelper.Pi / 180) * NPC.ai[2]);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offsetRAND, ModContent.ProjectileType<LifeProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, i * 1000);
                            }
                        }
                        break;
                    case 2: //consistent spread of big shots
                        SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 offset1 = (NPC.DirectionTo(Player.Center) * 9f).RotatedBy((i - 3) * MathHelper.Pi / 8f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offset1, ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                        }
                        break;
                    case 3: //dark souls
                        SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 offset1 = (NPC.DirectionTo(Player.Center) * 4f).RotatedBy((i - 2) * MathHelper.Pi / 12f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -offset1, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                        break;
                }
            }
            */

            //new homing swords:

            if (NPC.ai[1] >= 70 && NPC.ai[1] % 70 == 0 && NPC.ai[1] <= 420)
            {
                SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                int randSide = Main.rand.NextBool(2) ? 1 : -1;
                float randRot = Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8);
                Vector2 offset1 = (NPC.DirectionTo(Player.Center) * 8f).RotatedBy((MathHelper.PiOver2 * randSide) + randRot);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -offset1, ModContent.ProjectileType<JevilScar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, 0f, NPC.whoAmI);
            }
            if (NPC.ai[1] > 480 + 100/*rework*/)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<JevilScar>())
                    {
                        p.ai[0] = 1200;
                    }
                }
                Flying = true;
                oldstate = state;
                StateReset();
            }
        }
        public void AttackReactionShotgun()
        {
            useDR = true;

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                HitPlayer = true;
                NPC.netUpdate = true;
            }

            if (NPC.Distance(Player.Center) > 1200)
            {
                FlyingState(1.5f);
            }
            else
            {
                Flying = false;
                NPC.velocity *= 0.9f;
            }

            if (NPC.ai[1] == 1)
            {
                NPC.ai[2] = Main.rand.Next(140, 220);
                SoundEngine.PlaySound(SoundID.Unlock, Player.Center);
                NPC.ai[3] = (Main.rand.Next(2));
                NPC.netUpdate = true;
            }


            if (NPC.ai[1] < NPC.ai[2])
            { //wait for blast
                Flying = false;
                float flySpeed2 = 0.5f;
                //float inertia2 = 1f;
                Vector2 OnPlayer = new Vector2(Player.Center.X, Player.Center.Y);
                Vector2 flyonPlayer = NPC.DirectionTo(OnPlayer) * flySpeed2;
                // NPC.velocity = (NPC.velocity * (inertia2 - 1f) + flyonPlayer) / inertia2;
                if (NPC.ai[1] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float ai0 = -(NPC.ai[2] - 30);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy(-MathHelper.Pi / 12)), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, ai0, 2);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy(MathHelper.Pi / 12)), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, ai0, 2);
                    if (PhaseThree)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy(MathHelper.Pi / 4)), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, ai0, 2);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy(-MathHelper.Pi / 4)), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, ai0, 2);
                    }
                }
            }
            if (NPC.ai[1] == (NPC.ai[2] - 30))
            {
                SoundEngine.PlaySound(SoundID.Unlock, Player.Center);
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] == NPC.ai[2] - 20 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy((-MathHelper.Pi / 12) + (NPC.ai[3] * MathHelper.Pi / 6))), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, -20, 2);
                if (PhaseThree)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy(MathHelper.Pi / 4)), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, -20, 2);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - ((NPC.Center - Player.Center).RotatedBy(-MathHelper.Pi / 4)), Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, -20, 2);
                }
            }
            else if (NPC.ai[1] == NPC.ai[2])
            {
                float shootSpeed = FargoSoulsWorld.MasochistModeReal || PhaseThree ? 27f : 22f;
                LockVector2 = NPC.DirectionTo(Player.Center) * shootSpeed;
                NPC.netUpdate = true;
            }
            else if ((NPC.ai[1] - NPC.ai[2]) % 10 == 0 && NPC.ai[1] > NPC.ai[2] && Main.netMode != NetmodeID.MultiplayerClient && (((NPC.ai[1] < (NPC.ai[2] + 90) && !PhaseThree)) || ((NPC.ai[1] < (NPC.ai[2] + 270) && PhaseThree)))) //blast
            {
                SoundEngine.PlaySound(SoundID.Item12, Player.Center);
                float knockBack10 = 3f;
                for (int i = -3; i < 17; i++)
                {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (LockVector2).RotatedBy((i * -MathHelper.Pi / 48) + (i * NPC.ai[3] * MathHelper.Pi / 24)), ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack10, Main.myPlayer);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].timeLeft = 120;
                    
                    if (PhaseThree)
                    {
                        p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (LockVector2).RotatedBy(((MathHelper.Pi / 4) + ((i + 4) * MathHelper.Pi / 48) - ((NPC.ai[3] * MathHelper.Pi / 2) + ((i + 4) * NPC.ai[3] * MathHelper.Pi / 24)))), ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack10, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 120;
                    }
                }
            }

            //in p3, shoot volleys in closed area
            if (/*FargoSoulsWorld.MasochistModeReal && */PhaseThree && NPC.ai[1] >= NPC.ai[2] && NPC.ai[1] < NPC.ai[2] + 244)
            {
                if ((NPC.ai[1] - NPC.ai[2]) % 61 == 0) //choose spot
                {
                    NPC.ai[0] = MathHelper.ToRadians(Main.rand.Next(-15, 15));
                    LockVector1 = (Vector2.Normalize(LockVector2)).RotatedBy(MathHelper.ToRadians(25 - (50 * NPC.ai[3])) - NPC.ai[0]);
                    NPC.netUpdate = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient) //telegraph
                    {
                        int x = FargoSoulsWorld.MasochistModeReal ? 1 : 0; //1 shot below maso, 3 shots in maso
                        for (int i = -x; i <= x; i++)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, LockVector1.RotatedBy(MathHelper.Pi / 32 * i).ToRotation());
                        //Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + LockVector1 * 600f, Vector2.Zero, ModContent.ProjectileType<LifeCrosshair>(), 0, 0f, Main.myPlayer, -55);
                    }
                }
                if ((NPC.ai[1] - NPC.ai[2]) % 61 > 55 && (NPC.ai[1] - NPC.ai[2]) % 2 == 0) //fire
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Player.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int x = FargoSoulsWorld.MasochistModeReal ? 1 : 0; //1 shot below maso, 3 shots in maso
                        for (int i = -x; i <= x; i++)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 30f * LockVector1.RotatedBy(MathHelper.Pi / 32 * i), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                    }
                }

            }

            if ((NPC.ai[1] == NPC.ai[2] + 90 && !PhaseThree) || (NPC.ai[1] == NPC.ai[2] + 300 && PhaseThree))
            {
                NPC.position.X = Player.position.X - (NPC.width / 2);
                NPC.position.Y = Player.position.Y - 450f - (NPC.height / 2);
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center); //PLACEHOLDER
                HitPlayer = false;
                Flying = true;
                NPC.netUpdate = true;
            }

            int endtime = PhaseThree ? (FargoSoulsWorld.MasochistModeReal ? 340 : 240) : 110;
            if (NPC.ai[1] > NPC.ai[2] + endtime)
            {
                HitPlayer = false;
                oldstate = state;
                StateReset();
            }
        }
        public void AttackRunningMinigun()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                SoundEngine.PlaySound(SoundID.Zombie100, NPC.Center);
                NPC.netUpdate = true;
                rot = 0;

                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, NPC.DirectionTo(Player.Center).ToRotation());
            }
            Flying = false;
            float flySpeed3 = 5f;
            float inertia3 = 5f;
            Vector2 flyonPlayer2 = NPC.DirectionTo(Player.Center) * flySpeed3;
            NPC.velocity = (NPC.velocity * (inertia3 - 1f) + flyonPlayer2) / inertia3;

            const int startup = 40;
            int endtime = 360 + startup;
            if (!PhaseThree)
            {
                endtime = (endtime - startup) / 2 + startup;
            }
            if (PhaseThree)
            {
                float rampRatio = NPC.ai[1] / endtime;
                rampRatio *= 0.2f;
                NPC.position += NPC.velocity * rampRatio;
            }

            //rotation
            if (NPC.velocity.ToRotation() > MathHelper.Pi)
            {
                NPC.rotation = 0f - MathHelper.Pi * NPC.velocity.X / 100;
            }
            else
            {
                NPC.rotation = 0f + MathHelper.Pi * NPC.velocity.X / 100;
            }
            const float timescale = 1.5f; // frames per shot = 10 * timescale
            if (NPC.ai[1] >= startup && NPC.ai[1] % (10 * timescale) == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                for (int i = -1; i < 2; i+= 2)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 ShootPlayer = (NPC.DirectionTo(Player.Center) * 12f).RotatedBy(i * rot * MathHelper.Pi / 180);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, ShootPlayer, ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                    }
                }
                if (rot >= 0)
                    rot += (NPC.ai[2] < 8 / timescale || (NPC.ai[2] >= 16 / timescale && NPC.ai[2] < 24 / timescale)) ? 5 * timescale : -5 * timescale;
                else
                    rot = 0;
                NPC.ai[2]++;
            }
            if (NPC.ai[1] == endtime || (PhaseThree && NPC.ai[1] == (endtime + startup) / 2)) //final shot towards player to prevent dodging by just standing still
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 ShootPlayer = (NPC.DirectionTo(Player.Center) * 12f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, ShootPlayer, ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                }
            }
            /*
            //firing machinegun
            if (NPC.ai[1] > 15 && NPC.ai[2] >= (72 - (11 * Rampup)) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                for (int i = -1; i < 2; i++)
                {
                    Vector2 ShootPlayer = (NPC.DirectionTo(Player.Center) * 5f*Rampup).RotatedBy(i * MathHelper.Pi / 7f);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, ShootPlayer, ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                }
                if (Rampup < 6)
                    Rampup++;
                NPC.ai[2] = 0;
            }
            NPC.ai[2]++;

            //firing circle in p3
            if (FargoSoulsWorld.MasochistModeReal && NPC.ai[1] > 90 && NPC.ai[1] % 45 == 0 && Main.netMode != NetmodeID.MultiplayerClient && PhaseThree)
            {
                float ProjectileSpeed = 10f;
                Vector2 shootatPlayer3 = NPC.DirectionTo(Player.Center) * ProjectileSpeed;
                int amount2 = 14;
                for (int i = 0; i < amount2; i++)
                {
                    Vector2 shootoffset = shootatPlayer3.RotatedBy(i * (MathHelper.Pi / (amount2 / 2)));
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                }
            }
            */
            if (NPC.ai[1] >= endtime)
            {
                Flying = true;
                oldstate = state;
                StateReset();
            }
        }
        //unused
        /*
        public void AttackRain()
        {
            Player Player = Main.player[NPC.target];

            NPC.localAI[0] = Player.Center.X; //expose for proj to access
            NPC.localAI[1] = Player.Center.Y - 300;

            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(NPC.localAI[0], NPC.localAI[1]), Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -60);
            }

            if (NPC.ai[1] > 60f && NPC.ai[1] < 360f)
            {
                Flying = true;
                NPC.Center = new Vector2(NPC.localAI[0], NPC.localAI[1]);

                if (NPC.ai[2] > 1)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[0]++;
                        float knockBack6 = 3f;
                        NPC.ai[3] = Main.rand.Next(-750, 750);
                        Vector2 spawnPos = new Vector2(Player.Center.X - NPC.ai[3], Player.Center.Y - 750f);
                        if (!FargoSoulsWorld.MasochistModeReal)
                            spawnPos.X += Player.velocity.X * 30; //malice
                        float ai1 = -1;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, new Vector2(0f, 7f), ModContent.ProjectileType<LifeProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack6, Main.myPlayer, NPC.ai[0] * 1000, ai1);
                    }
                    NPC.ai[2] = 0f;
                }
                NPC.ai[2] += 1f;
            }
            if (NPC.ai[1] > 420f)
            {
                Flying = true;
                oldstate = state;
                StateReset();
            }
        }
        */
        public void AttackTeleportNukes()
        {
            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                
                LockVector1 = Player.Center;

                Flying = false;
                NPC.velocity = Vector2.Zero;
                NPC.netUpdate = true;
            }

            NPC.localAI[0] = LockVector1.X; //exposing so proj can access
            NPC.localAI[1] = LockVector1.Y;

            if (NPC.ai[1] == 1 && Main.netMode != NetmodeID.MultiplayerClient) //telegraph teleport and first shots
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -60);
                for (int i = 0; i < 16; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, (MathHelper.Pi / 8) * i);
                }
            }

            if (NPC.ai[1] == 60) //teleport and first shots
            {
                NPC.Center = LockVector1;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int i = 0; i < 16; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(24f, 0f).RotatedBy((MathHelper.Pi / 8) * i), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                }
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 90;

                //telegraph nukes
                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, (MathHelper.Pi / 3) * i);
            }

            if (NPC.ai[1] >= 120 && (NPC.ai[1] - 120) % 3 == 0 && NPC.ai[1] < 137) //nukes
            {
                SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float ai0 = FargoSoulsWorld.MasochistModeReal ? 32 : 24;
                    float ai1 = 0;
                    float speed = 16;
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(speed, 0f).RotatedBy((MathHelper.Pi / 3) * NPC.ai[2]), ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.5f), 300f, Main.myPlayer, ai0, ai1);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].timeLeft = 60;
                }
                NPC.ai[2]++;
            }
            if (NPC.ai[1] > 420f)
            {
                Flying = true;
                oldstate = state;
                StateReset();
            }
        }
        public void AttackRuneExpand()
        {
            Player Player = Main.player[NPC.target];
            //let projectiles access
            NPC.localAI[0] = RuneDistance;
            NPC.localAI[1] = BodyRotation;
            NPC.localAI[2] = RuneCount;

            const int ExpandTime = 175;
            int AttackDuration = PhaseOne ? 5 : 439 + 80; //change this depending on phase

            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item84, NPC.Center);

                //invisible rune hitbox
                for (int i = 0; i < RuneCount; i++)
                {
                    float runeRot = (float)(BodyRotation + (Math.PI * 2 / RuneCount * i));
                    Vector2 runePos = NPC.Center + (runeRot.ToRotationVector2() * RuneDistance);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), runePos, Vector2.Zero, ModContent.ProjectileType<LifeRuneHitbox>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.whoAmI, i);
                }
                if (!PhaseOne)
                {
                    Flying = false;
                    NPC.velocity = Vector2.Zero;
                }
                else //PhaseOne
                {
                    //decrease size to size of pyramid
                    NPC.position = NPC.Center;
                    NPC.Size = new Vector2(90, 90);
                    NPC.Center = NPC.position;
                }

            }

            if (NPC.Distance(Player.Center) > 2000)
                FlyingState(1.5f);
            else
                NPC.velocity *= 0.95f;

            if (NPC.ai[1] < ExpandTime) //expand
            {
                RuneDistance = (float)(100 + Math.Pow((NPC.ai[1] / 5), 2));
                RPS += 0.0005f;
            }

            if (NPC.ai[1] <= ExpandTime + AttackDuration - 80 && NPC.ai[1] >= ExpandTime && !PhaseOne) //p2-3 shots during expansion
            {
                    float startShots = PhaseThree ? 5f : 3f;
                    if (NPC.ai[3] < startShots)
                    {
                        NPC.ai[3] = startShots;
                    }
                    float ProjectileSpeed = 30f;
                    float knockBack2 = 3f;
                    int spread = 18 - (int)(NPC.ai[3] - startShots); //gets tighter after each shot
                    if ((NPC.ai[1] - ExpandTime) % 40 == 0f && NPC.ai[3] < 9 + startShots)
                    {
                        LockVector1 = NPC.Center;
                        LockVector2 = (NPC.DirectionTo(Player.Center) * ProjectileSpeed).RotatedBy(MathHelper.Pi / 80 * (Main.rand.NextFloat() - 0.5f));
                        NPC.ai[2] = Main.rand.Next(-spread / 2, spread / 2);
                        if (Main.netMode != NetmodeID.MultiplayerClient) //telegraph
                        {
                            for (int i = 0; (float)i < NPC.ai[3]; i++)
                            {
                                double rotationrad = MathHelper.ToRadians(0f - NPC.ai[3] * spread / 2 + (float)(i * spread) + NPC.ai[2]);
                                for (int j = 0; j < 2; j++) //shoot behind aswell to prevent cheese by going through boss
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0, LockVector2.RotatedBy(rotationrad + Math.PI*j).ToRotation());
                            }
                        }
                        NPC.netUpdate = true;
                    }
                    if ((NPC.ai[1] - ExpandTime) % 40 == 39 && NPC.ai[3] < 9 + startShots)
                    {
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient) //shoot
                        {
                            for (int i = 0; (float)i < NPC.ai[3]; i++)
                            {
                                double rotationrad = MathHelper.ToRadians(0f - NPC.ai[3] * spread / 2 + (float)(i * spread) + NPC.ai[2]);
                                Vector2 shootoffset = LockVector2.RotatedBy(rotationrad);
                                for (int j = 0; j < 2; j++) //shoot behind aswell to prevent cheese by going through boss
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), LockVector1, shootoffset.RotatedBy(Math.PI*j), ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
                            }
                        }
                        NPC.ai[3] += 1f;
                    }

            }
            
            if (NPC.ai[1] == ExpandTime + AttackDuration) //noise
            {
                SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
            }
            if (NPC.ai[1] >= ExpandTime + AttackDuration) //retract
            {
                RuneDistance = (float)(100 + Math.Pow((ExpandTime-(NPC.ai[1] - ExpandTime - AttackDuration)) / 5, 2));
                RPS -= 0.0005f;
            }
            if (NPC.ai[1] >= ExpandTime + AttackDuration + ExpandTime)
            {
                RuneDistance = 100; //make sure

                //kill rune hitboxes
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.type == ModContent.ProjectileType<LifeRuneHitbox>())
                        {
                            p.Kill();
                        }
                    }
                }
                if (PhaseOne)
                {
                    //revert size
                    NPC.position = NPC.Center; 
                    NPC.Size = new Vector2(DefaultWidth, DefaultHeight);
                    NPC.Center = NPC.position;

                    oldP1state = P1state;
                    P1stateReset();
                }
                else
                {
                    Flying = true;
                    oldstate = state;
                    StateReset();
                }
            }
        }
        #endregion
        #endregion
        #region Overrides
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 2;

            if (useDR)
                damage /= 4;

            if (phaseProtectionDR)
                damage /= 4;

            return true;
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if ((useDR || phaseProtectionDR) && NPC.lifeRegen < 0)
                NPC.lifeRegen /= 2;
        }

        #region Hitbox
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (HitPlayer)
            {
                Vector2 boxPos = target.position;
                Vector2 boxDim = target.Size;
                return Collides(boxPos, boxDim);
            }
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (HitPlayer)
            {
                Vector2 boxPos = target.position;
                Vector2 boxDim = target.Size;
                return Collides(boxPos, boxDim);
            }
            return false;
        }
        public bool Collides(Vector2 boxPos, Vector2 boxDim)
        {
            //circular hitbox-inator
            Vector2 ellipseDim = NPC.Size;
            Vector2 ellipseCenter = NPC.position + 0.5f * new Vector2(NPC.width, NPC.height);
            
            float x = 0f; //ellipse center
            float y = 0f; //ellipse center
            if (boxPos.X > ellipseCenter.X)
            {
                x = boxPos.X - ellipseCenter.X; //left corner
            }
            else if (boxPos.X + boxDim.X < ellipseCenter.X)
            {
                x = boxPos.X + boxDim.X - ellipseCenter.X; //right corner
            }
            if (boxPos.Y > ellipseCenter.Y)
            {
                y = boxPos.Y - ellipseCenter.Y; //top corner
            }
            else if (boxPos.Y + boxDim.Y < ellipseCenter.Y)
            {
                y = boxPos.Y + boxDim.Y - ellipseCenter.Y; //bottom corner
            }
            float a = ellipseDim.X / 2f;
            float b = ellipseDim.Y / 2f;
            
            return (x * x) / (a * a) + (y * y) / (b * b) < 1; //point collision detection
        }
        #endregion
        public override void HitEffect(int hitDirection, double HitDamage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType, 0, 0, 100, new Color(), 1f);
                }
                for (int i = 1; i <= 12; i++)
                {
                    Vector2 rand = new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    int j = Main.rand.Next(ChunkSpriteCount) + 1;
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + rand, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"ShardGold{j}").Type, NPC.scale);
                }
                for (int i = 1; i <= 4; i++)
                {
                    Vector2 rand = new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + rand, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"Pyramid{i}").Type, NPC.scale);
                }
                return;
            }
        }

        public override bool CheckDead()
        {
            //if (!resigned) //no dying before final phase
            //{
            //    NPC.life = 1;
            //    return false;
            //}
            return base.CheckDead();
        }

        public const int ChunkCount = 12;
        public const int RuneCount = 12;
        const int ChunkSpriteCount = 12;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) //DRAW BODY AND WINGS
		{
            /*if (Draw)
            {
			    Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
                Texture2D wingtexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallenger_Wings", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Vector2 drawPos = NPC.Center - screenPos;
                int currentFrame = NPC.frame.Y / (bodytexture.Height / Main.npcFrameCount[NPC.type]);
                int wingHeight = wingtexture.Height / Main.npcFrameCount[NPC.type];
                Rectangle wingRectangle = new Rectangle(0, currentFrame * wingHeight, wingtexture.Width, wingHeight);
                Vector2 wingOrigin = new Vector2(wingtexture.Width / 2, wingtexture.Height / 2 / Main.npcFrameCount[NPC.type]);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 value4 = NPC.oldPos[i];
                    double fpf = (int)(60 / Main.npcFrameCount[NPC.type] * RPS); //multiply by sec/rotation)
                    int oldFrame = (int)((NPC.frameCounter - i) / fpf);
                    Rectangle oldWingRectangle = new Rectangle(0, oldFrame * wingHeight, wingtexture.Width, wingHeight);
                    DrawData wingTrailGlow = new DrawData(wingtexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldWingRectangle), drawColor * (0.5f / i), NPC.rotation, wingOrigin, NPC.scale, SpriteEffects.None, 0);
                    GameShaders.Misc["LCWingShader"].UseColor(Color.HotPink).UseSecondaryColor(Color.HotPink);
                    GameShaders.Misc["LCWingShader"].Apply(wingTrailGlow);
                    wingTrailGlow.Draw(spriteBatch);
                }

                spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: drawColor, rotation: BodyRotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                spriteBatch.Draw(origin: wingOrigin, texture: wingtexture, position: drawPos, sourceRectangle: wingRectangle, color: drawColor, rotation: NPC.rotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
            }*/

            if (Draw)
            {
                //if chunk list is empty, fill list
                //for list, for loop, rotate by rotation + 360 * current item / max item
                //for chunk list, place using triangle formula
                //also rotate each chunk individually, random start rotation

                
                //const int RuneSpriteCount = 12;
                
                const float ChunkRotationSpeed = MathHelper.TwoPi / (8 * 60);

                if (chunklist.Count < ChunkCount)
                {
                    chunklist = InitializeSpriteList(ChunkSpriteCount, ChunkCount);
                }

                if (chunkrotlist.Count < ChunkCount)
                {
                    for (int i = 0; i < ChunkCount; i++)
                    {
                        chunkrotlist.Add(MathHelper.ToRadians(Main.rand.Next(360)));
                    }
                }

                
                if (SpritePhase > 1)
                {
                    for (int i = 0; i < ChunkCount; i++)
                    {
                        float drawRot = (float)(-BodyRotation - (Math.PI * 2 / ChunkCount * i));
                        Vector2 drawPos = NPC.Center + (drawRot.ToRotationVector2() * ChunkDistance) - screenPos;
                        //Vector2 drawPos = Trianglinator(i, screenPos);
                    
                        Texture2D ChunkTexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Challengers/LifeChallengerParts/ShardGold{chunklist[i]}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        float ChunkRotation = chunkrotlist[i];
                        chunkrotlist[i] += ChunkRotationSpeed;

                        spriteBatch.Draw(origin: new Vector2(ChunkTexture.Width / 2, ChunkTexture.Height / 2), texture: ChunkTexture, position: drawPos, sourceRectangle: null, color: drawColor, rotation: ChunkRotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                    }
                }
                for (int i = 0; i < RuneCount; i++)
                {
                    float drawRot = (float)(BodyRotation + (Math.PI * 2 / RuneCount * i));
                    Texture2D RuneTexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Challengers/LifeChallengerParts/Rune{i+1}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Vector2 drawPos = NPC.Center + (drawRot.ToRotationVector2() * RuneDistance) - screenPos;
                    float RuneRotation = drawRot + MathHelper.PiOver2;

                    //rune glow
                    for (int j = 0; j < 12; j++)
                    {
                        Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                        Color glowColor;
                        if (i % 3 == 0) //cyan
                        {
                            glowColor = new Color(0f, 1f, 1f, 0f) * 0.7f;
                        }
                        else if (i % 3 == 1) //yellow
                        {
                            glowColor = new Color(1f, 1f, 0f, 0f) * 0.7f;
                        }
                        else //pink
                        {
                            glowColor = new Color(1, 192/255f, 203/255f, 0f) * 0.7f;
                        }
                        
                        Main.spriteBatch.Draw(RuneTexture, drawPos + afterimageOffset, null, NPC.GetAlpha(glowColor), RuneRotation, RuneTexture.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
                    }
                    spriteBatch.Draw(origin: new Vector2(RuneTexture.Width / 2, RuneTexture.Height / 2), texture: RuneTexture, position: drawPos, sourceRectangle: null, color: drawColor, rotation: RuneRotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                }


                //draw wings
                //draws 4 things: 2 upper wings, 2 lower wings

                Texture2D wingUtexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/LifeChallenger_WingUpper", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D wingLtexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/LifeChallenger_WingLower", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Vector2 wingdrawPos = NPC.Center - screenPos;
                int currentFrame = NPC.frame.Y / (wingUtexture.Height / Main.npcFrameCount[NPC.type]);
                int wingUHeight = wingUtexture.Height / Main.npcFrameCount[NPC.type];
                Rectangle wingURectangle = new Rectangle(0, currentFrame * wingUHeight, wingUtexture.Width, wingUHeight);
                int wingLHeight = wingLtexture.Height / Main.npcFrameCount[NPC.type];
                Rectangle wingLRectangle = new Rectangle(0, currentFrame * wingLHeight, wingLtexture.Width, wingLHeight);
                Vector2 wingUOrigin = new Vector2(wingUtexture.Width / 2, wingUtexture.Height / 2 / Main.npcFrameCount[NPC.type]);
                Vector2 wingLOrigin = new Vector2(wingLtexture.Width / 2, wingLtexture.Height / 2 / Main.npcFrameCount[NPC.type]);

                for (int i = -1; i < 2; i += 2)
                {
                    float wingLRotation = NPC.rotation - MathHelper.PiOver2 + MathHelper.ToRadians(110*i);
                    float wingURotation = NPC.rotation - MathHelper.PiOver2 + MathHelper.ToRadians(70*i);
                    SpriteEffects flip = (i == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
                    spriteBatch.Draw(origin: wingUOrigin, texture: wingUtexture, position: wingdrawPos + (wingURotation.ToRotationVector2() * ((DefaultWidth / 2) + 30)), sourceRectangle: wingURectangle, color: drawColor, rotation: wingURotation, scale: NPC.scale, effects: flip, layerDepth: 0f);
                    spriteBatch.Draw(origin: wingLOrigin, texture: wingLtexture, position: wingdrawPos + (wingLRotation.ToRotationVector2() * ((DefaultWidth / 2) + 30)), sourceRectangle: wingLRectangle, color: drawColor, rotation: wingLRotation, scale: NPC.scale, effects: flip, layerDepth: 0f);
                }

                
            }
            return false;
		}
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) //DRAW STAR
        {
            if (SpritePhase > 1 || !Draw) //star
            {
                if (!NPC.IsABestiaryIconDummy)
                {
                    spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                }


                Texture2D star = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle rect = new Rectangle(0, 0, star.Width, star.Height);
                float scale = 0.45f * Main.rand.NextFloat(1f, 2.5f);
                Vector2 origin = new Vector2((star.Width / 2) + scale, (star.Height / 2) + scale);

                spriteBatch.Draw(star, NPC.Center - screenPos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                DrawData starDraw = new DrawData(star, NPC.Center - screenPos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.HotPink).UseSecondaryColor(Color.HotPink);
                GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
                starDraw.Draw(spriteBatch);

                if (!NPC.IsABestiaryIconDummy)
                {
                    spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
                }
            }
            float PyramidRot = 0;
            if (NPC.velocity.ToRotation() > MathHelper.Pi)
            {
                PyramidRot = 0f - MathHelper.Pi * NPC.velocity.X / 300;
            }
            else
            {
                PyramidRot = 0f + MathHelper.Pi * NPC.velocity.X / 300;
            }
            if (SpritePhase == 1 && Draw) //whole pyramid
            {
                Texture2D pyramid = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/Phase1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle rect = new Rectangle(0, 0, pyramid.Width, pyramid.Height);
                Vector2 origin = pyramid.Size() / 2;
                Vector2 wobble = new Vector2((float)Math.Sin(MathHelper.ToRadians(5.632167f /*purposefully random weird number*/ * DrawTime)), (float)Math.Sin(MathHelper.ToRadians(3*DrawTime))) * PyramidWobble;
                spriteBatch.Draw(origin: origin, texture: pyramid, position: NPC.Center + wobble - screenPos, sourceRectangle: rect, color: drawColor, rotation: PyramidRot, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
            }
            else if (Draw) //broken pyramid
            {
                Texture2D[] pyramidp = new Texture2D[4];
                Rectangle[] rects = new Rectangle[4];
                Vector2[] origins = new Vector2[4];
                Vector2[] offsets = new Vector2[4];

                pyramidp[0] = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/Phase2U", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                pyramidp[1] = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/Phase2L", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                pyramidp[2] = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/Phase2R", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                pyramidp[3] = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/Phase2D", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                float expansion = ChunkDistance / ChunkDistanceMax;
                float P = (float)Math.Sqrt(SpritePhase - 1); //1 in p2, sqrt2 in p3, this doesn't draw in p1
                offsets[0] = new Vector2(0, -15) * (float)Math.Abs(Math.Sin(MathHelper.ToRadians(DrawTime * P))) * expansion + new Vector2(0, -30); //top
                offsets[1] = new Vector2(-12.5f, 3) * (float)Math.Abs(Math.Sin(MathHelper.ToRadians(DrawTime * P))) * expansion + new Vector2(-25, 10) * P; //left
                offsets[2] = new Vector2(12.5f, 3) * (float)Math.Abs(Math.Sin(MathHelper.ToRadians(DrawTime * P))) * expansion + new Vector2(25, 10) * P; //right
                offsets[3] = new Vector2(0, 10) * (float)Math.Abs(Math.Sin(MathHelper.ToRadians(DrawTime * P))) * expansion + new Vector2(0, 20) * P;  //bottom

                for (int i = 0; i < 4; i++)
                {
                    rects[i] = new Rectangle(0, 0, pyramidp[i].Width, pyramidp[i].Height);
                    origins[i] = pyramidp[i].Size() / 2;
                    offsets[i] = offsets[i].RotatedBy(PyramidRot);
                    
                    spriteBatch.Draw(origin: origins[i], texture: pyramidp[i], position: NPC.Center + offsets[i] - screenPos, sourceRectangle: rects[i], color: drawColor, rotation: PyramidRot, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                }
                
                
            }

            DrawTime++;
        }

        public override void FindFrame(int frameHeight)
        {
            Texture2D wingUtexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallengerParts/LifeChallenger_WingUpper", ReLogic.Content.AssetRequestMode.DoNotLoad).Value;
            double fpf = 60/10; //  60/fps
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter += 1;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type] * fpf;
            NPC.frame.Y = wingUtexture.Height / (Main.npcFrameCount[NPC.type]) * (int)(NPC.frameCounter / fpf);
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.LifeChallenger], -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<LifeChallengerBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifeChallengerTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<LifeChallengerRelic>()));

            LeadingConditionRule rule = new LeadingConditionRule(new Conditions.NotExpert());
            rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<EnchantedLifeblade>(), ModContent.ItemType<Lightslinger>(), ModContent.ItemType<CrystallineCongregation>(), ModContent.ItemType<KamikazePixieStaff>()));
            rule.OnSuccess(ItemDropRule.Common(ItemID.HallowedFishingCrateHard, 1, 1, 5)); //hallowed crate
            rule.OnSuccess(ItemDropRule.Common(ItemID.SoulofLight, 1, 1, 3));
            rule.OnSuccess(ItemDropRule.Common(ItemID.PixieDust, 1, 15, 25));

            npcLoot.Add(rule);
        }
        #endregion
        #region Help Methods
        public bool FlightCheck()
        {
            if (FargoSoulsWorld.MasochistModeReal)
                return false;

            if (++flyTimer < (FargoSoulsWorld.EternityMode ? 120 : 180))
            {
                float speed = FargoSoulsWorld.EternityMode ? 1.2f : 0.8f;
                FlyingState(speed);
                return true;
            }
            return false;
        }
        
        public void P1stateReset()
        {
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            AttackF1 = true;
            NPC.netUpdate = true;
        }

        public void RandomizeP1state()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                P1state = Main.rand.Next(P1statecount);
                if (P1state == oldP1state)
                    P1state = (P1state + 1) % P1statecount;
            }

            if (NPC.life < P2Threshold) //phase 2 switch
            {
                P1state = -1;
                flyTimer = 9000;
            }

            NPC.netUpdate = true;
        }
        public void StateReset()
		{
            NPC.TargetClosest(true);
			NPC.netUpdate = true;
			RandomizeState();
			NPC.ai[1] = 0f;
			NPC.ai[2] = 0f;
			NPC.ai[3] = 0f;
			NPC.ai[0] = 0f;
            NPC.localAI[1] = 0;
			AttackF1 = true;
		}

		public void RandomizeState() //it's done this way so it cycles between attacks in a random order: for increased variety
		{
            if (availablestates.Count < 1)
			{
				availablestates.Clear();
				for (int j = 0; j < statecount; j++)
				{
					availablestates.Add(j);
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				index = Main.rand.Next(availablestates.Count);
				state = availablestates[index];
				availablestates.RemoveAt(index);

			}

            if (!PhaseThree && NPC.life < P3Threshold)
			{
				state = 100;
			}

			if (PhaseThree && NPC.life < SansThreshold)
			{
				state = 101;
				oldstate = -665;
			}

            if (first)
            {
                state = 0;
                availablestates.Remove(0);
                first = false;
                oldstate = -666;
            }
			NPC.netUpdate = true;
		}

        private List<int> InitializeSpriteList(int countSprites, int count)
        {
            List<int> lst = new List<int>();

            for (int j = 1; j <= countSprites; j++)
            {
                lst.Add(j);
            }

            //shuffle
            int n = lst.Count;
            while (n > 1)
            {
                n--;
                int k = Main.rand.Next(n + 1);
                (lst[n], lst[k]) = (lst[k], lst[n]);
            }

            //delete so we have correct amount
            if (countSprites - count > 0)
            {
                for (int k = 0; k < countSprites - count; k++)
                {
                    lst.RemoveAt(k);
                }
            }
            return lst;
        }

        /*
        private Vector2 Trianglinator(int chunk, Vector2 screenPos)
        {
            //determine positions for each chunk, depending on their rotation (evenly spread), to form a triangle
            //orth are lines with angles orthogonal to triangle sides, used with sec (1/cos) to find length from center to place on triangle side
            float OrthLineLength = (NPC.width / 2) * 0.9f * 0.5f; //triangle vertex distance from center * cos(60deg)
            float drawRot = (float)(ChunkTriangleInnerRotation + (Math.PI * 2 / ChunkCount * chunk));
            float RotDif;
            float RotDeg = MathHelper.ToDegrees(drawRot);

            float Orth1 = 60;
            float Orth2 = 180;
            //orth3 is the "else"

            if (RotDeg % 360 >= Orth1 - 60 && RotDeg % 360 < Orth1 + 60)
            {
                RotDif = MathHelper.ToRadians(RotDeg - 60);
            }
            else if (RotDeg % 360 >= Orth2 - 60 && RotDeg % 360 < Orth2 + 60)
            {
                RotDif = MathHelper.ToRadians(RotDeg - 180);
            }
            else
            {
                RotDif = MathHelper.ToRadians(RotDeg - 300);
            }
            //pos = center + rot * (r / 2) * sec(rotdif)
            return NPC.Center + (drawRot + ChunkTriangleOuterRotation).ToRotationVector2() * (float)(OrthLineLength / Math.Cos(RotDif)) - screenPos; //the trianglinator
        }
        */
        #endregion
    }
}
