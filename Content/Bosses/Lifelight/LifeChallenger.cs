using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Pets;
using FargowiltasSouls.Content.Items.Placables.Relics;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Golf;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.Bosses.Magmaw.Magmaw;
using static FargowiltasSouls.Core.Systems.DashManager;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    [AutoloadBossHead]
    public class LifeChallenger : ModNPC, IPixelatedPrimitiveRenderer
    {
        #region Variables
        public static readonly SoundStyle ScreechSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightScreech1") with { Volume = 1.5f};
        public static readonly SoundStyle DashSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightDash") with { Volume = 1.5f };
        public static readonly SoundStyle DashSound2 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightPixieDash") with { Volume = 1.5f};
        public static readonly SoundStyle RuneSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightRuneSound") with { Volume = 1.5f };
        public static readonly SoundStyle TelegraphSound1 = new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightShotPrep") with { Volume = 1.5f};

        const int DefaultHeight = 200;
        const int DefaultWidth = 200;

        private bool Charging = false;
        private bool Flying = false;
        private bool CustomRotation = false;

        public bool HitPlayer = false;
        private bool AttackF1;
        private int dustcounter;

        public Vector2 LockVector1 = new(0, 0);
        public Vector2 LockVector2 = new(0, 0);
        public Vector2 LockVector3 = new(0, 0);
        public Vector2 AuraCenter = new(0, 0);

        private double rotspeed = 0;
        public double rot = 0;

        private bool useDR;
        private bool phaseProtectionDR;
        private bool DoAura;

        public bool PhaseOne = true;

        int P2Threshold => Main.expertMode ? (int)(NPC.lifeMax * (WorldSavingSystem.EternityMode ? WorldSavingSystem.MasochistModeReal ? 0.75 : 0.66 : 0.5)) : 0;

        // Visual
        private List<Vector4> chunklist = [];
        public const float DefaultRuneDistance = 100;
        public float RuneDistance = DefaultRuneDistance;
        private bool DrawRunes = true;
        public const float DefaultChunkDistance = 65;
        public float ChunkDistance = 1000;


        public int RuneFormation;
        public int OldRuneFormation;
        public int RuneFormationTimer;

        public const int FormationTime = 60;
        public float GunRotation = 0;
        public Vector2 GunCircleCenter(float lerp) => NPC.Center + GunRotation.ToRotationVector2() * DefaultRuneDistance * (RuneFormation == Formations.GunCloser ? 0.15f : 0.9f) * lerp;
        public float FormationLerp
        {
            get
            {
                float lerp = Math.Clamp((float)RuneFormationTimer / FormationTime, 0, 1);
                return lerp;
            }
        }

        public struct Formations
        {
            public const int Circle = 0;
            public const int Shield = 1;
            public const int Gun = 2;
            public const int Scattered = 3;
            public const int Spear = 4;
            public const int GunCloser = 5;
        }

        public Vector2[] CustomRunePositions = new Vector2[RuneCount];

        public int PyramidPhase = 0;
        public int PyramidTimer = 0;
        public const int PyramidAnimationTime = 60;

        float BodyRotation = 0;
        public float RPS = 0.1f;
        private bool Draw = false;


        //NPC.AI
        public ref float AI_Timer => ref NPC.ai[1];

        //States
        public int State;
        public int AttackCount;
        // AttackCount increments each attack. pattern:
        // 1 - precision
        // 2 - charge
        // 3 - precision
        // 4 - plunge
        // 5 (phase 2 only): strong attack
        // entering phase 2 sets pattern to charge

        public enum States
        {
            // misc
            Opening,
            P1Transition,
            Deathray,

            // precision
            RuneExpand,
            RuneScatter,
            LifeBlades,
            ShotBarrage,

            // charges
            Charge,
            PixieCharge,

            // plunges
            Plunge,
            RuneSpear,

            // strongs
            CrystallineCongregation,

            // 
            Count
        }
        public List<States> PrecisionAttacks
        {
            get
            {
                List<States> attacks =
                    [
                    States.RuneExpand,
                    States.RuneScatter,
                    States.ShotBarrage
                    ];
                if (!PhaseOne)
                    attacks.Add(States.LifeBlades);
                return attacks;
            }
        }
        public List<States> ChargeAttacks =>
        [
            States.Charge,
            States.PixieCharge
        ];
        public List<States> PlungeAttacks
        {
            get
            {
                List<States> attacks =
                    [
                        States.RuneSpear,
                    ];
                if (!PhaseOne || WorldSavingSystem.MasochistModeReal)
                    attacks.Add(States.Plunge);
                return attacks;
            }
        }
        public List<States> StrongAttacks =>
        [
            States.CrystallineCongregation
        ];
        int[] LastAttack = new int[4];
        #endregion
        #region Standard
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lifelight");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 40;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = "FargowiltasSouls/Assets/Effects/LifeStar", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = new Vector2(0f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.Suffocation,
                    ModContent.BuffType<LethargicBuff>(),
                    ModContent.BuffType<ClippedWingsBuff>()
            ]);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            ]);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 55000;
            NPC.defense = 28;
            NPC.damage = 70;
            NPC.knockBackResist = 0f;
            NPC.width = 200;
            NPC.height = 200;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath7;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/LieflightNoCum") : MusicID.OtherworldlyBoss1;
            SceneEffectPriority = SceneEffectPriority.BossLow;

            NPC.value = Item.buyPrice(0, 15);

            NPC.dontTakeDamage = true; //until it Appears in Opening
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(State);
            writer.Write7BitEncodedInt(AttackCount);

            writer.Write(rotspeed);

            writer.Write(AttackF1);

            writer.WriteVector2(LockVector1);
            writer.WriteVector2(LockVector2);

            writer.Write7BitEncodedInt(PyramidPhase);
            writer.Write7BitEncodedInt(PyramidTimer);
            writer.Write7BitEncodedInt(RuneFormation);
            writer.Write7BitEncodedInt(RuneFormationTimer);

            for (int i = 0; i < LastAttack.Length; i++)
                writer.Write7BitEncodedInt(LastAttack[i]);

            for (int i = 0; i < CustomRunePositions.Length; i++)
                writer.WriteVector2(CustomRunePositions[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            State = reader.Read7BitEncodedInt();
            AttackCount = reader.Read7BitEncodedInt();

            rotspeed = reader.ReadDouble();

            AttackF1 = reader.ReadBoolean();

            LockVector1 = reader.ReadVector2();
            LockVector2 = reader.ReadVector2();

            PyramidPhase = reader.Read7BitEncodedInt();
            PyramidTimer = reader.Read7BitEncodedInt();
            RuneFormation = reader.Read7BitEncodedInt();
            RuneFormationTimer = reader.Read7BitEncodedInt();

            for (int i = 0; i < LastAttack.Length; i++)
                LastAttack[i] = reader.Read7BitEncodedInt();

            for (int i = 0; i < CustomRunePositions.Length; i++)
                CustomRunePositions[i] = reader.ReadVector2();

        }
        #endregion
        #region AI
        public override void AI()
        {

            //Defaults
            Player Player = Main.player[NPC.target];
            Main.time = 27000; //noon
            Main.dayTime = true;
            Main.LocalPlayer.ZoneHallow = true;
            Main.StopRain();
            Main.cloudAlpha = 0;

            NPC.defense = NPC.defDefense;
            NPC.chaseable = true;
            phaseProtectionDR = false;
            NPC.dontTakeDamage = false;
            Flying = false;
            CustomRotation = false;

            if (RuneFormationTimer >= FormationTime)
                OldRuneFormation = RuneFormation;
            if (RuneFormationTimer <= FormationTime)
                RuneFormationTimer++;

            useDR = false;

            if (PhaseOne && NPC.life < P2Threshold)
                phaseProtectionDR = true;


            if (PyramidPhase == 1)
            {
                if (PyramidTimer == PyramidAnimationTime)
                {
                    SoundEngine.PlaySound(SoundID.Item53, NPC.Center);
                    NPC.HitSound = SoundID.Item52;

                    NPC.defense = NPC.defDefense + 100;
                    NPC.netUpdate = true;
                }
                useDR = true;
                ChunkDistance = DefaultChunkDistance * (1 - Math.Min((float)PyramidTimer / PyramidAnimationTime, 1f));
            }
            else if (PyramidPhase == -1)
            {
                if (PyramidTimer == 5)
                {
                    SoundEngine.PlaySound(SoundID.Shatter with { Pitch = -0.5f }, NPC.Center);
                    NPC.HitSound = SoundID.NPCHit4;
                    NPC.defense = NPC.defDefense;
                    NPC.netUpdate = true;
                }
                ChunkDistance = DefaultChunkDistance * Math.Min((float)PyramidTimer / PyramidAnimationTime, 1f);
                if (ChunkDistance == DefaultChunkDistance)
                {
                    PyramidPhase = 0;
                    NPC.netUpdate = true;
                }
            }
            PyramidTimer++;
            //rotation
            BodyRotation += RPS * MathHelper.TwoPi / 60f; //first number is rotations/second
            if (RuneFormation == Formations.Gun || OldRuneFormation == Formations.Gun) // faster
            {
                BodyRotation += FormationLerp * 0.25f * MathHelper.TwoPi / 60f;
            }

            if (State != (int)States.Opening) //do not check during spawn anim
            {
                //Aura
                if (DoAura)
                {
                    if (dustcounter > 5 && (DoAura && State == (int)States.RuneExpand)) //do dust instead of runes during rune expand attack
                    {
                        for (int l = 0; l < 180; l++)
                        {
                            double rad2 = 2.0 * l * (MathHelper.Pi / 180.0);
                            double dustdist2 = 1200.0;
                            int DustX2 = (int)AuraCenter.X - (int)(Math.Cos(rad2) * dustdist2);
                            int DustY2 = (int)AuraCenter.Y - (int)(Math.Sin(rad2) * dustdist2);
                            int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                            int i = Dust.NewDust(new Vector2(DustX2, DustY2), 1, 1, DustType, Scale: 1.5f);
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
                                player.FargoSouls().NoUsingItems = 2;
                            }

                            Vector2 movement = AuraCenter - player.Center;
                            float difference = movement.Length() - threshold;
                            movement.Normalize();
                            movement *= difference < 17f ? difference : 17f;
                            player.position += movement;

                            for (int i = 0; i < 10; i++)
                            {
                                int DustType = Main.rand.NextFromList(DustID.YellowTorch, DustID.PinkTorch, DustID.UltraBrightTorch);
                                int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default, 1.25f);
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
            DoAura = WorldSavingSystem.MasochistModeReal; //reset aura status

            switch ((States)State)
            {
                case States.Opening:
                    Opening();
                    break;
                case States.P1Transition:
                    P1Transition();
                    break;
                case States.Deathray:
                    LaserSpin();
                    break;                
                case States.RuneExpand:
                    RuneExpand();
                    break;
                case States.RuneScatter:
                    RuneScatter();
                    break;
                case States.LifeBlades:
                    LifeBlades();
                    break;
                case States.ShotBarrage: 
                    ShotBarrage(); 
                    break;
                case States.Charge:
                    Charge();
                    break;
                case States.PixieCharge:
                    PixieCharge();
                    break;
                case States.Plunge:
                    Plunge();
                    break;
                case States.RuneSpear:
                    RuneSpear();
                    break;
                case States.CrystallineCongregation:
                    CrystallineCongregation();
                    break;
                default:
                    StateReset();
                    break;
            }


            if (!CustomRotation)
            {
                if (Charging) //Charging orientation
                {
                    if (NPC.velocity == Vector2.Zero)
                        NPC.rotation = 0f;
                    else
                        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi / 2;
                    GunRotation = NPC.rotation - MathHelper.PiOver2;
                }
                else if (!Flying) //standard upright orientation
                {
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.09f);
                }
            }

            AI_Timer += 1f;
        }
        #endregion
        #region States

        #region Misc
        public void Opening()
        {
            ref float P1AI_Timer = ref NPC.ai[2];

            if (!NPC.HasValidTarget)
                NPC.TargetClosest(false);

            Player Player = Main.player[NPC.target];
            NPC.position.X = Player.Center.X - NPC.width / 2;
            NPC.position.Y = Player.Center.Y - 490 - NPC.height / 2;
            NPC.alpha = (int)(255 - P1AI_Timer * 17);
            RPS = 0.1f;

            if (AI_Timer == 180)
            {

                if (!Main.dedServ)
                    ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 60);

                if (WorldSavingSystem.EternityMode && !WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight] && FargoSoulsUtil.HostCheck)
                    Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<FragilePixieLamp>());

                SoundEngine.PlaySound(ScreechSound1, NPC.Center);
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

            if (chunklist.Count < ChunkCount)
            {

                //generating an even sphere using cartesian coordinates
                float phi = MathHelper.Pi * (float)(Math.Sqrt(5) - 1); //golden angle in radians

                int i = chunklist.Count;
                //for (int i = 0; i < amount; i++)
                //{
                float y = 1 - (2 * ((float)i / (ChunkCount - 1)));
                float theta = phi * i;
                float radius = (float)Math.Sqrt(1 - y * y);
                float x = (float)Math.Cos(theta) * radius;
                float z = (float)Math.Sin(theta) * radius;
                chunklist.Add(new(x, y, z, Main.rand.Next(12) + 1));
                Vector2 pos = NPC.Center + (x * Vector2.UnitX + y * Vector2.UnitY) * ChunkDistance;
                SoundEngine.PlaySound(SoundID.Tink, pos);
                for (int d = 0; d < 3; d++)
                {
                    int dust = Dust.NewDust(pos, 5, 5, DustID.Gold, Scale: 1.5f);
                    Main.dust[dust].velocity = (Main.dust[dust].position - pos) / 10;
                }
                //}
            }
            if (AI_Timer < 180f)
            {
                ChunkDistance = 1000 - ((1000 - DefaultChunkDistance) * ((float)AI_Timer / 180f));
                NPC.dontTakeDamage = true;
            }

            if (AI_Timer >= 240 && chunklist.Count >= ChunkCount)
            {
                ChunkDistance = DefaultChunkDistance; // for good measure
                StateReset();
            }
        }
        public void P1Transition()
        {
            ref float SubAttack = ref NPC.localAI[1];
            Player player = Main.player[NPC.target];


            Charging = false;
            Flying = false;
            useDR = true;
            DoAura = true;
            NPC.velocity *= 0.95f;

            NPC.ai[3] = 0; //no periodic nuke

            void PhaseTransition()
            {
                if (RPS < 0.2f) //speed up rotation
                {
                    RPS += 0.1f / 100;
                }
                else
                {
                    RPS = 0.2f;
                }

                if (AI_Timer < 120)
                    FlyingState();
                else
                    NPC.velocity *= 0.96f;

                if (AI_Timer == 5)
                {
                    SoundEngine.PlaySound(ScreechSound1 with { Pitch = -0.5f }, NPC.Center);
                    RuneFormation = Formations.Circle;
                    RuneFormationTimer = 0;
                }
                if (AI_Timer < 60)
                {
                    //if (AI_Timer % 5 == 0)
                    //SoundEngine.PlaySound(SoundID.Tink, Main.LocalPlayer.Center);

                    Color color = Main.rand.NextFromList(Color.Goldenrod, Color.Pink, Color.Cyan);
                    Particle p = new SmallSparkle(
                        worldPosition: NPC.Center,
                        velocity: (Main.rand.NextFloat(5, 50) * Vector2.UnitX).RotatedByRandom(MathHelper.TwoPi),
                        drawColor: color,
                        scale: 1f,
                        lifetime: Main.rand.Next(20, 80),
                        rotation: 0,
                        rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                        );
                    p.Spawn();
                    p.Position -= p.Velocity * 4; //implosion
                }
                if (AI_Timer == 60f)
                {
                    SoundEngine.PlaySound(SoundID.Item82 with { Pitch = -0.2f }, Main.LocalPlayer.Center);
                    if (PyramidPhase == 0)
                    {
                        PyramidTimer = 0;
                    }
                    PyramidPhase = 1;
                    NPC.netUpdate = true;

                    for (int i = 0; i < 100; i++)
                    {
                        Color color = Main.rand.NextFromList(Color.Goldenrod, Color.Pink, Color.Cyan);
                        Particle p = new SmallSparkle(
                            worldPosition: NPC.Center,
                            velocity: (Main.rand.NextFloat(5, 50) * Vector2.UnitX).RotatedByRandom(MathHelper.TwoPi),
                            drawColor: color,
                            scale: 1f,
                            lifetime: Main.rand.Next(20, 80),
                            rotation: 0,
                            rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                            );
                        p.Spawn();
                        //p.Position -= p.Velocity * 4; //implosion
                    }
                }
                int mineAmount = WorldSavingSystem.EternityMode ? 45 : 0;
                if (AI_Timer <= 180 && AI_Timer > 180 - mineAmount)
                {
                    //mine explosion
                    int bombwidth = 22;
                    if (FargoSoulsUtil.HostCheck)
                    {
                        int bombType = ModContent.ProjectileType<LifeTransitionBomb>();
                        //for (int i = 0; i < MineAmount; i++)
                        //{
                        int i = (int)(AI_Timer - (180 - mineAmount));
                        Vector2 FindPos()
                        {
                            float rotation = ((float)i / mineAmount) * MathHelper.TwoPi;
                            float distFrac = Main.rand.NextFloat(1);
                            float modifier = (float)Math.Sin(MathHelper.TwoPi * i / 8f) * 0.3f + 0.9f;
                            distFrac = (float)Math.Pow(distFrac, modifier);
                            float min = NPC.width / 3f;
                            float max = 1200;
                            float distance = MathHelper.Lerp(min, max, distFrac);
                            return NPC.Center + (rotation.ToRotationVector2() * distance);
                        }
                        Vector2 pos = FindPos();
                        const int maxAttempts = 30;
                        for (int attempt = 0; attempt < maxAttempts; attempt++)
                        {
                            pos = FindPos();
                            if (!Main.projectile.Any(p => p.active && p.type == bombType && (Vector2.UnitX * p.ai[1] + Vector2.UnitY * p.ai[2]).Distance(pos) < bombwidth * 1.2f))
                            {
                                break;
                            }
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, bombType, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 0, pos.X, pos.Y);
                        //}
                    }
                }
                if (AI_Timer == 240 - 60 - FormationTime)
                {
                    RuneFormation = Formations.Gun;
                    RuneFormationTimer = 0;
                }
                if (AI_Timer == 240 - 60)
                {

                    SoundEngine.PlaySound(SoundID.Item92 with { Pitch = -0.5f }, NPC.Center);

                    if (!Main.dedServ)
                        ScreenShakeSystem.StartShake(10, shakeStrengthDissipationIncrement: 10f / 60);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        foreach (Projectile p in Main.projectile)
                        {
                            if (p.type == ModContent.ProjectileType<LifeBombExplosion>())
                            {
                                //make them fade
                                p.ai[0] = Math.Max(p.ai[0], LifeBombExplosion.MaxTime - 30);
                                p.netUpdate = true;
                            }
                        }
                    }
                }
                if (AI_Timer < 240)
                {
                    LockVector1 = NPC.DirectionTo(player.Center);
                    GunRotation = LockVector1.ToRotation();
                }
            }

            PhaseTransition();
            //ExpandRunes();
            if (AI_Timer >= 240f)
            {
                LaserSpin(true);
            }

        }
        public void LaserSpin(bool phaseTransition = false)
        {
            ref float RandomDistance = ref NPC.ai[0];
            ref float LaserTimer = ref NPC.localAI[2];
            ref float RotationDirection = ref NPC.localAI[0];

            Player player = Main.player[NPC.target];
            NPC.velocity *= 0.9f;
            HitPlayer = true;
            Flying = false;
            Charging = false;

            //for a starting time, make it fade in, then make it spin faster and faster up to a max speed
            const int fadeintime = 10;
            int endTime = 60 * 3;

            if (!phaseTransition)
                endTime = 110;

            float start = phaseTransition ? 240 : 220;

            if (AI_Timer < start - FormationTime)
                AI_Timer = start - FormationTime;

            if (AI_Timer < 280f)
            {
                if (AI_Timer < start)
                {
                    LockVector1 = NPC.DirectionTo(player.Center);
                    if (PyramidPhase == 0)
                    {
                        PyramidTimer = 0;
                        NPC.netUpdate = true;
                    }
                    PyramidPhase = 1;

                    if (RuneFormation != Formations.Gun)
                    {
                        RuneFormationTimer = 0;
                        NPC.netUpdate = true;
                    }
                    RuneFormation = Formations.Gun;

                }
                else if (AI_Timer == start)
                {
                    LockVector1 = NPC.DirectionTo(player.Center);
                    RotationDirection = Math.Sign(FargoSoulsUtil.RotationDifference(NPC.DirectionTo(player.Center + player.velocity), NPC.DirectionTo(player.Center)));
                    if (RotationDirection == 0)
                        RotationDirection = 1;
                    NPC.netUpdate = true;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, -1, NPC.whoAmI);
                }
                else
                {
                    Vector2 lockon = NPC.DirectionTo(player.Center).RotatedBy(-RotationDirection * MathF.PI * 0.15f);
                    LockVector1 = Vector2.Lerp(LockVector1, lockon, 0.1f);
                }
                GunRotation = LockVector1.RotatedBy(rot).ToRotation();
                return;
            }


            if (AttackF1)
            {
                AttackF1 = false;

                string extra = !phaseTransition ? "Short" : "";
                SoundEngine.PlaySound(new SoundStyle($"FargowiltasSouls/Assets/Sounds/LifelightDeathray{extra}") with { Volume = 3f}, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, LockVector1,
                                    ModContent.ProjectileType<LifeChalDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.2f), 3f, Main.myPlayer, 0, NPC.whoAmI, endTime);
                }
                NPC.velocity.X = 0;
                NPC.velocity.Y = 0;
                Flying = false;

                float pyramidRot = LockVector1.RotatedBy(rot).ToRotation();
                Vector2 PV = NPC.SafeDirectionTo(player.Center);
                Vector2 LV = pyramidRot.ToRotationVector2();
                float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
                RotationDirection = Math.Sign(anglediff);


                NPC.netUpdate = true;
                rotspeed = 0;
                rot = 0;

                if (!phaseTransition)
                {
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center).RotatedBy(MathF.PI * 0.75f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                    }
                }
            }
            GunRotation = LockVector1.RotatedBy(rot).ToRotation();
            if (RotationDirection == 0)
            {
                RotationDirection = 1;
                NPC.netUpdate = true;
            }
            if (LaserTimer >= fadeintime && LaserTimer < endTime)
            {
                float maxSpeed = 1.3f;
                if (rotspeed < maxSpeed)
                {
                    float increment = maxSpeed / 100f;
                    if (!phaseTransition)
                        increment = maxSpeed / 35f;
                    rotspeed += increment;
                }
                else
                {
                    rotspeed = maxSpeed;
                }
                rot += RotationDirection * MathHelper.Pi / 180 * rotspeed;

            }

            LaserTimer++;

            if (LaserTimer >= endTime && PyramidPhase == 1)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<LifeBombExplosion>())
                    {
                        //make them fade
                        p.ai[0] = Math.Max(p.ai[0], LifeBombExplosion.MaxTime - 30);
                        p.netUpdate = true;
                    }
                    //kill deathray, just to be sure
                    if (p.type == ModContent.ProjectileType<LifeChalDeathray>())
                    {
                        p.Kill();
                    }
                }

                PyramidPhase = -1;
                PyramidTimer = 0;
                NPC.netUpdate = true;

                RuneFormation = Formations.Circle;
                RuneFormationTimer = 0;
            }
            if (LaserTimer > endTime)
            {
                rotspeed = 0;
                rot = 0;
                if (phaseTransition)
                {
                    GoPhase2();
                }
                else
                {
                    HitPlayer = false;
                    StateReset();
                }
                return;
            }
        }
        public void GoPhase2()
        {
            PhaseOne = false;
            HitPlayer = false;
            NPC.netUpdate = true;
            NPC.TargetClosest(true);
            AttackF1 = true;
            AI_Timer = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.ai[0] = 0f;
            AttackCount = 1;
            StateReset();
        }

        public void FlyingState(float speedModifier = 1f, bool flyfast = false, Vector2? flyPosition = null, bool orient = true)
        {
            ref float speedVar = ref NPC.localAI[3];
            Flying = true;

            //basically, create a smooth transition when using different speedMod values
            float accel = 0.5f / 30f;
            if (speedVar < speedModifier)
            {
                speedVar += accel;
                if (speedVar > speedModifier)
                    speedVar = speedModifier;
            }
            if (speedVar > speedModifier)
            {
                speedVar -= accel;
                if (speedVar < speedModifier)
                    speedVar = speedModifier;
            }
            speedModifier = speedVar;

            Player Player = Main.player[NPC.target];
            //flight AI
            float flySpeed = 0f;
            float inertia = 10f;
            Vector2 AbovePlayer = new(Player.Center.X, Player.Center.Y - 300f);

            if (flyPosition.HasValue)
                AbovePlayer = flyPosition.Value;

            bool Close = Math.Abs(AbovePlayer.Y - NPC.Center.Y) < 32f && Math.Abs(AbovePlayer.X - NPC.Center.X) < 160f;
            if (!Close && NPC.Distance(AbovePlayer) < 500f)
            {
                flySpeed = 9f;
                if (!flyfast)
                {
                    Vector2 flyabovePlayer3 = NPC.SafeDirectionTo(AbovePlayer) * flySpeed;
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + flyabovePlayer3) / inertia;
                }
            }
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.velocity = NPC.SafeDirectionTo(AbovePlayer) * 1f;
            }
            if (NPC.Distance(AbovePlayer) > 360f)
            {
                flySpeed = NPC.Distance(AbovePlayer) / 35f;
                flyfast = true;
                Vector2 flyabovePlayer2 = NPC.SafeDirectionTo(AbovePlayer) * flySpeed;
                NPC.velocity = (NPC.velocity * (inertia - 1f) + flyabovePlayer2) / inertia;
            }
            if (flyfast && (NPC.Distance(AbovePlayer) < 100f || NPC.Distance(Player.Center) < 100f))
            {
                Vector2 flyabovePlayer = NPC.SafeDirectionTo(AbovePlayer) * flySpeed;
                NPC.velocity = flyabovePlayer;
            }

            //orientation
            if (orient)
            {
                if (NPC.velocity.ToRotation() > MathHelper.Pi)
                {
                    NPC.rotation = 0f - MathHelper.Pi * NPC.velocity.X * speedModifier / 100;
                }
                else
                {
                    NPC.rotation = 0f + MathHelper.Pi * NPC.velocity.X * speedModifier / 100;
                }
            }

            NPC.position -= NPC.velocity * (1f - speedModifier);
        }
        #endregion

        #region Attacks
        public void RuneExpand()
        {

            ref float ExtraShots = ref NPC.ai[3];
            ref float RandomAngle = ref NPC.ai[2];

            Player Player = Main.player[NPC.target];
            //let projectiles access
            NPC.localAI[0] = RuneDistance;
            NPC.localAI[1] = BodyRotation;
            NPC.localAI[2] = RuneCount;

            const int ExpandTime = 175;
            int AttackDuration = 5; //change this depending on phase

            if (NPC.Distance(Player.Center) > 2000)
                FlyingState(1.5f);
            else
                NPC.velocity *= 0.95f;

            if (RuneFormation != Formations.Circle)
            {
                RuneFormation = Formations.Circle;
                RuneFormationTimer = 0;
            }

            if (RuneFormationTimer < FormationTime)
            {
                AI_Timer--;
                return;
            }

            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(RuneSound1, NPC.Center);
                if (PyramidPhase == 0)
                {
                    PyramidTimer = 0;
                }
                PyramidPhase = 1;

                //invisible rune hitbox
                for (int i = 0; i < RuneCount; i++)
                {
                    float runeRot = (float)(BodyRotation + Math.PI * 2 / RuneCount * i);
                    Vector2 runePos = NPC.Center + runeRot.ToRotationVector2() * RuneDistance;
                    DrawRunes = false;
                    NPC.netUpdate = true;

                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), runePos, Vector2.Zero, ModContent.ProjectileType<LifeRuneHitbox>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, NPC.whoAmI, i);

                }

                if (WorldSavingSystem.MasochistModeReal)
                {
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Player.Center).RotatedBy(MathF.PI * 0.4f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                }
                    
                //decrease size to size of triangle
                NPC.position = NPC.Center;
                NPC.Size = new Vector2(DefaultChunkDistance * 2, DefaultChunkDistance * 2);
                NPC.Center = NPC.position;

                if (PhaseOne)
                    LockVector1 = -Vector2.UnitY;
                else
                    LockVector1 = NPC.DirectionTo(Player.Center);

            }


            if (AI_Timer < ExpandTime) //expand
            {
                if (AI_Timer < 30)
                {
                    if (NPC.Distance(Player.Center) > 350)
                        FlyingState(0.7f, false, Player.Center);

                }
                if (WorldSavingSystem.MasochistModeReal)
                {
                    RuneDistance = Math.Min((float)(100 + Math.Pow(AI_Timer / 5, 2)), 1200);
                }
                else
                {
                    RuneDistance = (float)(100 + Math.Pow(AI_Timer / 5, 2));
                }
                RPS += 0.0005f;
            }
            if (!PhaseOne) // p2 shots
            {
                LockVector1 = LockVector1.RotateTowards(NPC.DirectionTo(Player.Center).ToRotation(), 0.02f);

                Vector2 tipPos = NPC.Center + LockVector1.SafeNormalize(Vector2.UnitX) * 37;

                if (AI_Timer >= ExpandTime - 120)
                {
                    if (AI_Timer % 5 == 0)
                    {
                        float scale = 3f * (AI_Timer - (ExpandTime - 120f)) / 60f;
                        if (AI_Timer > ExpandTime + AttackDuration + 30)
                            scale = 3f * 1 - (AI_Timer - ExpandTime + AttackDuration + 30 / 60f);

                        scale = MathHelper.Clamp(scale, 0, 3);

                        Particle p = new ExpandingBloomParticle(tipPos, NPC.velocity, Color.Cyan, Vector2.One * scale, Vector2.One * scale, 10);
                        p.Spawn();
                    }
                }

                if (AI_Timer >= ExpandTime - 60 && AI_Timer <= ExpandTime + AttackDuration + 30)
                {
                    if (AI_Timer % 12 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            float ProjectileSpeed = 12f;
                            Vector2 vel = LockVector1.SafeNormalize(Vector2.UnitX) * ProjectileSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), tipPos, vel, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer);
                        }
                    }
                }
            }

            if (AI_Timer == ExpandTime + AttackDuration) //noise
            {
                SoundEngine.PlaySound(RuneSound1, NPC.Center);
            }
            if (AI_Timer >= ExpandTime + AttackDuration) //retract
            {
                HitPlayer = false; //stop dealing contact damage (anti-cheese)
                if (WorldSavingSystem.MasochistModeReal)
                {
                    RuneDistance = Math.Min((float)(DefaultRuneDistance + Math.Pow((ExpandTime - (AI_Timer - ExpandTime - AttackDuration)) / 5, 2)), 1200);
                }
                else
                {
                    RuneDistance = (float)(DefaultRuneDistance + Math.Pow((ExpandTime - (AI_Timer - ExpandTime - AttackDuration)) / 5, 2));
                }
                RPS -= 0.0005f;
            }
            if (AI_Timer >= ExpandTime + AttackDuration + ExpandTime)
            {
                RuneDistance = DefaultRuneDistance; //make sure

                //kill rune hitboxes
                if (FargoSoulsUtil.HostCheck)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.type == ModContent.ProjectileType<LifeRuneHitbox>())
                        {
                            p.Kill();
                        }
                    }
                }
                DrawRunes = true;
                NPC.netUpdate = true;

                PyramidPhase = -1;
                PyramidTimer = 0;

                //revert size
                NPC.position = NPC.Center;
                NPC.Size = new Vector2(DefaultWidth, DefaultHeight);
                NPC.Center = NPC.position;

                Flying = true;
                StateReset();
            }
        }
        public void RuneScatter()
        {
            Player Player = Main.player[NPC.target];

            int startup = FormationTime + (WorldSavingSystem.MasochistModeReal ? 40 : WorldSavingSystem.EternityMode ? 50 : 60);

            int explosionTime = 60;

            if (AttackF1)
            {
                AttackF1 = false;
                
                NPC.netUpdate = true;

                RuneFormation = Formations.Scattered;
                RuneFormationTimer = 0;

                for (int i = 0; i < CustomRunePositions.Length; i++)
                {
                    Vector2 offset = Vector2.Zero;
                    if (i == CustomRunePositions.Length - 1 && !CustomRunePositions.Any(p => p.Distance(Player.Center) < 100))
                    {
                        CustomRunePositions[i] = Player.Center + Vector2.UnitX.RotatedBy(MathF.Tau * i / RuneCount).RotatedByRandom(MathF.PI * 0.1f) * 70f;
                        break;
                    }
                    for (int attempts = 0; attempts < 20; attempts++)
                    {
                        bool valid = true;
                        offset = Vector2.UnitX.RotatedBy(MathF.Tau * i / RuneCount).RotatedByRandom(MathF.PI * 0.1f) * Main.rand.NextFloat(60f, 450f);
                        for (int j = 0; j < CustomRunePositions.Length; j++)
                        {
                            if (i != j && offset.Distance(CustomRunePositions[j] - Player.Center) < 100)
                            {
                                valid = false;
                                break;
                            }
                            
                        }
                        if (valid)
                        {
                            CustomRunePositions[i] = Player.Center + offset;
                            break;
                        }
                            
                    }
                    CustomRunePositions[i] = Player.Center + offset;
                }

                if (WorldSavingSystem.MasochistModeReal)
                {
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Player.Center).RotatedBy(MathF.PI * 0.4f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                }
            }

            FlyingState(0.2f);

            if (WorldSavingSystem.EternityMode && AI_Timer < startup)
            {
                for (int i = 0; i < CustomRunePositions.Length; i++)
                {
                    CustomRunePositions[i] += Player.velocity;
                }
            }
            if (AI_Timer == startup) // put hitboxes and eventual explosion
            {
                SoundEngine.PlaySound(RuneSound1, NPC.Center);

                NPC.netUpdate = true;
                for (int i = 0; i < RuneCount; i++)
                {
                    float runeRot = 0;
                    Vector2 runePos = CustomRunePositions[i];

                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), runePos, Vector2.Zero, ModContent.ProjectileType<LifeRuneExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, NPC.whoAmI, i, ai2: explosionTime);
                }
            }
            /*
            if (AI_Timer == startup + explosionTime) // explosion
            {

            }
            */
            if (AI_Timer >= startup + explosionTime)
                DrawRunes = true;
            if (AI_Timer > startup + explosionTime + 10)
            {
                if (!PhaseOne)
                {
                    GoToDeathray();
                    return;
                }
                if (RuneFormation != Formations.Circle)
                {
                    RuneFormation = Formations.Circle;
                    RuneFormationTimer = 0;
                }
                else if (RuneFormationTimer >= FormationTime)
                {
                    StateReset();
                }
            }
        }
        public void LifeBlades()
        {
            ref float TeleportX = ref NPC.localAI[0];
            ref float TeleportY = ref NPC.localAI[1];
            ref float RandomAngle = ref NPC.ai[3];

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;

                RuneFormation = Formations.Circle;
                RuneFormationTimer = 0;
            }

            /*
            Vector2 RouletteTpPos = Player.Center + 500 * RandomAngle.ToRotationVector2();
            TeleportX = RouletteTpPos.X; //exposing so proj can access
            TeleportY = RouletteTpPos.Y;

            if (AI_Timer == 1 && FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), RouletteTpPos, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -40, NPC.whoAmI);
            }

            if (AI_Timer == 40)
            {
                NPC.Center = RouletteTpPos;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center); //PLACEHOLDER
                LockVector1 = NPC.SafeDirectionTo(Player.Center);
                TeleportY = 0;
                NPC.netUpdate = true;
            }
            */

            /*
            if (AI_Timer > 40)
            {
                float angleDiff = MathHelper.WrapAngle(NPC.SafeDirectionTo(Player.Center).ToRotation() - LockVector1.ToRotation());
                if (Math.Abs(angleDiff) > MathHelper.Pi / 3f)
                {
                    LockVector1 = NPC.SafeDirectionTo(Player.Center);
                    NPC.netUpdate = true;
                }
            }

            if (AI_Timer < 420 + 120 && AI_Timer % 9 == 0 && AI_Timer > 60 && FargoSoulsUtil.HostCheck)
            {
                const float speed = 20f;
                Vector2 offset1 = LockVector1.RotatedBy(MathHelper.Pi / 3f) * speed;
                Vector2 offset2 = LockVector1.RotatedBy(-MathHelper.Pi / 3f) * speed;

                TeleportY++;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offset1, ModContent.ProjectileType<LifeProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer, 0, 3);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offset2, ModContent.ProjectileType<LifeProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer, 0, 4);
            }
            */

            FlyingState(0.4f);

            //new homing swords:

            if (AI_Timer < 50)
                AI_Timer = 50;

            if (AI_Timer >= 70 && AI_Timer % 70 == 0 && AI_Timer <= 420)
            {
                SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                int randSide = AI_Timer % 140 == 0 ? 1 : -1;
                float randRot = Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
                Vector2 offset1 = (NPC.SafeDirectionTo(Player.Center) * 14f).RotatedBy(MathHelper.PiOver2 * randSide + randRot);
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -offset1, ModContent.ProjectileType<JevilScar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 0f, NPC.whoAmI);
            }
            float extraTime = PhaseOne || !WorldSavingSystem.EternityMode ? 100 : -20;
            if (AI_Timer > 480 + extraTime)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.type == ModContent.ProjectileType<JevilScar>())
                    {
                        float extra = PhaseOne || !WorldSavingSystem.EternityMode ? 0 : 100;
                        p.ai[0] = 1200 - extra;
                    }
                }
                Flying = true;
                if (!PhaseOne)
                {
                    GoToDeathray();
                    return;
                }
                StateReset();
            }
        }
        public void ShotBarrage()
        {
            ref float ShotTimer = ref NPC.localAI[1];
            ref float P1AI_Timer = ref NPC.ai[2];

            P1AI_Timer++;

            FlyingState(0.2f);

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                AttackF1 = false;
                NPC.netUpdate = true;
                ShotTimer = 0;
                //Rampup = 1;
                RuneFormation = Formations.GunCloser;
                RuneFormationTimer = 0;
                SoundEngine.PlaySound(TelegraphSound1, NPC.Center);
                GunRotation = NPC.SafeDirectionTo(Player.Center).ToRotation();

                if (WorldSavingSystem.MasochistModeReal)
                {
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Player.Center).RotatedBy(MathF.PI * 0.4f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                }
            }
            GunRotation = GunRotation.ToRotationVector2().RotateTowards(NPC.SafeDirectionTo(Player.Center).ToRotation(), 0.1f).ToRotation();

            if (P1AI_Timer > FormationTime && AI_Timer < 220f)
            {
                int threshold = WorldSavingSystem.MasochistModeReal ? 5 : 6;
                if (++ShotTimer % threshold == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                    float finalSpreadOffset = MathHelper.Pi / (WorldSavingSystem.MasochistModeReal ? 6 : WorldSavingSystem.EternityMode ? 6 : 5);
                    float startOffset = (MathHelper.Pi - finalSpreadOffset) * 0.15f;
                    const int timeToFocus = 60;

                    float rampRatio = (float)Math.Min(1f, ShotTimer / timeToFocus);
                    float rotationToUse = finalSpreadOffset + startOffset * (float)Math.Cos(MathHelper.PiOver2 * rampRatio);

                    Vector2 vel = NPC.SafeDirectionTo(Player.Center);
                    float length = 8f + 6f * rampRatio;
                    vel *= length;

                    int projType = ModContent.ProjectileType<LifeHomingProjSmall>(); //length > 8f ? ModContent.ProjectileType<LifeHomingProjSmall>() : ModContent.ProjectileType<LifeProjSmall>();

                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 finalVel = vel.RotatedBy(rotationToUse * i);
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), GunCircleCenter(0) + finalVel * 2, finalVel, projType, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                    }
                }
            }
            float end = WorldSavingSystem.EternityMode ? 200f : 220f;

            if (AI_Timer >= end - 30)
            {
                if (!PhaseOne)
                {
                    GoToDeathray();
                    return;
                }
            }
            if (AI_Timer >= end)
            {

                StateReset();
            }
        }

        public void Charge()
        {
            ref float SwordSide = ref NPC.ai[3];
            ref float AttackCount = ref NPC.ai[2];

            ref float TeleportX = ref NPC.localAI[0];
            ref float TeleportY = ref NPC.localAI[1];

            Player Player = Main.player[NPC.target];
            int StartTime = (WorldSavingSystem.MasochistModeReal ? 60 : WorldSavingSystem.EternityMode ? 70 : 90);

            if (AttackF1)
            {
                LockVector3 = NPC.Center;
                AttackF1 = false;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(ScreechSound1, NPC.Center);

                RuneFormationTimer = 0;
                RuneFormation = Formations.Shield;

                NPC.rotation = NPC.DirectionTo(Player.Center).ToRotation() + MathHelper.PiOver2;
                SwordSide = Main.rand.NextBool() ? 1 : -1;
            }
            Flying = false;
            
            HitPlayer = true;
            AuraCenter = LockVector3; //lock arena in place during charges

            float chargecount = PhaseOne ? 3f : 4f;

            float chargeSpeed = 22f;
            Vector2 chargeatPlayer = NPC.SafeDirectionTo(Player.Center) * chargeSpeed;

            if (AI_Timer < StartTime * 0.5f && AttackCount == 0)
            {
                Vector2 desiredPos = Player.Center + Player.DirectionTo(NPC.Center) * 450;
                FlyingState(1f, false, desiredPos, orient: false);
                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Player.Center).ToRotation() + MathHelper.PiOver2, AI_Timer / StartTime);
            }


            if (AttackCount < chargecount)
            {
                if (AI_Timer == StartTime - 30 && AttackCount == 0 && WorldSavingSystem.EternityMode) // bomb
                {
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Player.Center).RotatedBy(MathF.PI * 0.2f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                }

                if (AI_Timer == StartTime + 1) // sfx
                    SoundEngine.PlaySound(DashSound1, NPC.Center);
                

                if (AI_Timer > StartTime * 0.5f) // movement
                {
                    Charging = true;
                    float speedmod = 1.2f;

                    float progress = (AI_Timer - StartTime) / StartTime;
                    if (AI_Timer < StartTime || progress > 0.4f)
                    {
                        NPC.velocity *= 0.95f;
                        speedmod /= 6f;
                    }
                    NPC.velocity += NPC.DirectionTo(Player.Center) * speedmod;
                    NPC.velocity.ClampLength(0, 35);
                }

                // p2 tp and double charge
                if (!PhaseOne && AttackCount == 2f)
                {
                    float tpTelegraph = 40;
                    float leeway = 5;
                    float timeOffset = tpTelegraph + leeway;

                    float endTime = StartTime * 2;

                    if (AI_Timer >= endTime - timeOffset && AI_Timer <= endTime - timeOffset + 20)
                    {
                        Vector2 TpPos = Player.Center + Player.velocity.SafeNormalize(Vector2.UnitX) * 500;

                        TeleportX = TpPos.X; //exposing so proj can access
                        TeleportY = TpPos.Y;
                        if (AI_Timer == endTime - timeOffset)
                        {

                            NPC.netUpdate = true;

                            SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                            if (FargoSoulsUtil.HostCheck)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), TpPos, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -tpTelegraph, NPC.whoAmI);
                                if (WorldSavingSystem.EternityMode)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(Player.Center).RotatedBy(MathF.PI * 0.2f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                            }
                                
                        }
                    }

                    if (AI_Timer == endTime - leeway)
                    {
                        NPC.Center = new(TeleportX, TeleportY);
                        for (int i = 0; i < NPC.oldPos.Length; i++)
                            NPC.oldPos[i] = NPC.Center;
                        NPC.velocity = NPC.DirectionTo(Player.Center) * 0.1f;
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                        NPC.netUpdate = true;

                        AI_Timer = StartTime * 0.5f;
                        AttackCount++;
                        return;
                    }
                }

                if (AI_Timer == StartTime * 2) // reset
                {
                    NPC.netUpdate = true;
                    AI_Timer = AttackCount == chargecount - 1 ? 0 : StartTime;
                    if (AttackCount == chargecount - 1)
                    {
                        if (RuneFormation != Formations.Circle)
                        {
                            RuneFormation = Formations.Circle;
                            RuneFormationTimer = 0;
                        }
                    }
                    AttackCount += 1f;
                }
            }
            GunRotation = NPC.rotation - MathHelper.PiOver2;
            
            if (AttackCount >= chargecount)
            {
                NPC.velocity *= 0.96f;
                Vector2 desiredPos = Player.Center + Player.DirectionTo(NPC.Center) * 250;
                FlyingState(1f, false, desiredPos, orient: true);

                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                HitPlayer = false;
                Flying = true;
                Charging = false;
                StateReset();
            }
        }
        public void PixieCharge()
        {
            ref float ChargeCounter = ref NPC.ai[2];
            ref float RandomOffset = ref NPC.ai[3];

            ref float TeleportX = ref NPC.localAI[0];
            ref float TeleportY = ref NPC.localAI[1];

            Player Player = Main.player[NPC.target];
            int StartTime = (WorldSavingSystem.MasochistModeReal ? 80 : WorldSavingSystem.EternityMode ? 90 : 100);

            if (AttackF1)
            {
                Flying = true;
                AttackF1 = false;
                NPC.netUpdate = true;
                RandomOffset = 0;
                SoundEngine.PlaySound(ScreechSound1, NPC.Center);
                NPC.rotation = NPC.DirectionTo(Player.Center).ToRotation() + MathHelper.PiOver2;
                LockVector3 = NPC.Center;

                RuneFormationTimer = 0;
                RuneFormation = Formations.Shield;
            }
            AuraCenter = LockVector3;
            Flying = false;

            if (AI_Timer < StartTime)
            {
                Vector2 desiredPos = Player.Center + Player.DirectionTo(NPC.Center) * 550;
                FlyingState(1f, false, desiredPos, orient: false);
                NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.DirectionTo(Player.Center).ToRotation() + MathHelper.PiOver2, AI_Timer / StartTime);
                Charging = false;
            }
            else
            {
                Charging = true;
            }
            if (AI_Timer == StartTime - 5)
            {
                LockVector1 = Player.Center;
                NPC.netUpdate = true;
            }
            const int ChargeCD = 60 + 8; // compensation for accel changes


            // p2 tp and double charge
            if (!PhaseOne && ChargeCounter == 0)
            {
                float tpTelegraph = 40;
                float leeway = 5;
                float timeOffset = tpTelegraph + leeway;

                if (AI_Timer >= StartTime + ChargeCD - timeOffset && AI_Timer <= StartTime + ChargeCD - timeOffset + 20)
                {
                    Vector2 TpPos = Player.Center + Player.velocity.SafeNormalize(Vector2.UnitX) * 500;

                    TeleportX = TpPos.X; //exposing so proj can access
                    TeleportY = TpPos.Y;
                    if (AI_Timer == StartTime + ChargeCD - timeOffset)
                    {
                        NPC.netUpdate = true;

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), TpPos, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -tpTelegraph, NPC.whoAmI);
                    }
                }

                if (AI_Timer == StartTime + ChargeCD - leeway)
                {
                    NPC.Center = new(TeleportX, TeleportY);

                    for (int i = 0; i < NPC.oldPos.Length; i++)
                        NPC.oldPos[i] = NPC.Center;
                    NPC.velocity = NPC.DirectionTo(Player.Center) * 0.1f;
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                    NPC.netUpdate = true;

                    AI_Timer = StartTime * 0.5f;
                    ChargeCounter++;
                    return;
                }
            }

            if (AI_Timer >= StartTime + ChargeCD)
            {
                NPC.velocity *= 0.97f;
                Vector2 desiredPos = Player.Center + Player.DirectionTo(NPC.Center) * 250;
                FlyingState(1f, false, desiredPos, orient: true);

                foreach (Projectile p in Main.projectile.Where(p => p.Alive()))
                {
                    if (p.type == ModContent.ProjectileType<LifeHomingProj>())
                        p.ai[2] = 1;
                }
                Flying = true;
                Charging = false;
                StateReset();
                return;
            }
            GunRotation = NPC.rotation - MathHelper.PiOver2;

            Vector2 atPlayer = NPC.SafeDirectionTo(Player.Center);

            if (AI_Timer == StartTime) //charge
            {
                SoundEngine.PlaySound(DashSound1, NPC.Center);

                float initialSpeed = 10f;
                NPC.velocity = atPlayer * initialSpeed;
                NPC.netUpdate = true;
            }
            if (AI_Timer > StartTime && AI_Timer < StartTime + 55)
            {
                float modifier = WorldSavingSystem.EternityMode ? WorldSavingSystem.MasochistModeReal ? 1.4f : 1.2f : 0.5f;
                NPC.velocity += atPlayer * modifier;
                NPC.velocity = NPC.velocity.ClampLength(0, 25f);
            }
            if (AI_Timer % 5 == 0 && AI_Timer > StartTime) //fire pixies during charges
            {
                SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    float knockBack10 = 3f;
                    Vector2 shootoffset4 = Vector2.Normalize(NPC.velocity).RotatedBy(RandomOffset) * 5f;
                    RandomOffset = (float)(Main.rand.Next(-30, 30) * (MathHelper.Pi / 180.0)); //change random offset after so game has time to sync
                    float ai0 = 0;
                    if (!WorldSavingSystem.MasochistModeReal)
                    {
                        ai0 = -30;
                        shootoffset4 /= 2;
                    }

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset4, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), knockBack10, Main.myPlayer, ai0, NPC.whoAmI);
                }
            }
            NPC.velocity *= 0.99f;
        }

        public void Plunge()
        {
            ref float TeleportX = ref NPC.localAI[0];
            ref float TeleportY = ref NPC.localAI[1];
            ref float PlungeCount = ref NPC.ai[2];

            Player Player = Main.player[NPC.target];
            int StartTime = (WorldSavingSystem.MasochistModeReal ? 60 : WorldSavingSystem.EternityMode ? 70 : 80);
            float tpTelegraph = 40;

            if (AttackF1)
            {
                LockVector3 = NPC.Center;
                AttackF1 = false;
                NPC.netUpdate = true;
                Flying = false;

                SoundEngine.PlaySound(ScreechSound1, NPC.Center);

                RuneFormation = Formations.Shield;
                RuneFormationTimer = 0;
            }
            AuraCenter = LockVector3;

            Vector2 TpPos = new(Player.Center.X, Player.Center.Y - 400f);

            TeleportX = TpPos.X; //exposing so proj can access
            TeleportY = TpPos.Y;

            if (AI_Timer == 1)
            {
                LockVector2 = new Vector2(Player.Center.X, Player.Center.Y - 400f);
            }
            if (AI_Timer == StartTime - 15 - tpTelegraph && FargoSoulsUtil.HostCheck)
            {

                Projectile.NewProjectile(NPC.GetSource_FromThis(), TpPos, Vector2.Zero, ModContent.ProjectileType<LifeTpTelegraph>(), 0, 0f, Main.myPlayer, -tpTelegraph, NPC.whoAmI);
                /*
                if (WorldSavingSystem.MasochistModeReal)
                {
                    //below wall telegraph
                    for (int i = 0; i < 60; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector2.X - 1500, LockVector2.Y + 400 + 500 + 60 * i), Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0, 0);
                    }
                }
                */
            }
            if (AI_Timer < StartTime - 15 && PlungeCount <= 0)
                NPC.velocity *= 0.95f;

            if (AI_Timer == StartTime - 15)
            {
                Flying = false;
                Charging = true;
                NPC.Center = TpPos;
                for (int i = 0; i < NPC.oldPos.Length; i++)
                    NPC.oldPos[i] = NPC.Center;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0.1f;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                NPC.netUpdate = true;
            }
            GunRotation = NPC.rotation - MathHelper.PiOver2;
            if (AI_Timer == StartTime)
            {
                SoundEngine.PlaySound(DashSound1, NPC.Center);
                HitPlayer = true;
                float chargeSpeed2 = 55f;
                NPC.velocity.Y = chargeSpeed2;
                NPC.netUpdate = true;
                //below wall
                /*
                if (WorldSavingSystem.MasochistModeReal)
                {
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = 0; i < 120; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector2.X - 1200, LockVector2.Y + 600 + 500 + 30 * i), new Vector2(60, 0), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer);
                        }
                        for (int i = 0; i < 120; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(LockVector2.X + 1200, LockVector2.Y + 600 + 500 + 30 * i), new Vector2(-60, 0), ModContent.ProjectileType<LifeProjLarge>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer);
                        }
                    }
                }
                */
            }
            if (AI_Timer >= StartTime)
            {
                HitPlayer = true;
                NPC.velocity = NPC.velocity * 0.96f;
            }
            if (AI_Timer == StartTime + 30 && FargoSoulsUtil.HostCheck)
            {
                float knockBack4 = 3f;
                Vector2 shootdown2 = new(0f, 10f);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                for (int k = 0; k <= 15; k++)
                {
                    double rotationrad3 = MathHelper.ToRadians(-90 + k * 12);
                    Vector2 shootoffset3 = shootdown2.RotatedBy(rotationrad3);
                    shootoffset3.X *= 2f;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset3, ModContent.ProjectileType<LifeNeggravProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), knockBack4, Main.myPlayer);
                }
            }
            if (AI_Timer == StartTime + 45 && FargoSoulsUtil.HostCheck)
            {
                float knockBack3 = 3f;
                Vector2 shootdown = new(0f, 10f);
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                for (int j = 0; j <= 15; j++)
                {
                    double rotationrad2 = MathHelper.ToRadians(-90 + j * 10);
                    Vector2 shootoffset2 = shootdown.RotatedBy(rotationrad2);
                    shootoffset2.X *= 2f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset2, ModContent.ProjectileType<LifeNeggravProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), knockBack3, Main.myPlayer);
                }
            }
            if (AI_Timer == StartTime + 50 && PlungeCount < 1f && WorldSavingSystem.MasochistModeReal && !PhaseOne)
            {
                AI_Timer = 0f;
                PlungeCount += 1f;
                NPC.netUpdate = true;
            }
            float neutralTime = 110;
            if (AI_Timer == StartTime + neutralTime)
            {
                RuneFormation = Formations.Circle;
                RuneFormationTimer = 0;
                NPC.netUpdate = true;
            }
            if (AI_Timer > StartTime + neutralTime) // fly back up
            {


                Vector2 desiredPos = Player.Center + Player.DirectionTo(NPC.Center) * 200;
                FlyingState(1.5f, false, desiredPos, orient: true);

                HitPlayer = false;
                Flying = true;
                Charging = false;
                
            }
            if (AI_Timer >= StartTime + neutralTime + FormationTime)
            {
                HitPlayer = false;
                StateReset();
            }
        }

        public void RuneSpear()
        {
            Player player = Main.player[NPC.target];

            int StartTime = WorldSavingSystem.MasochistModeReal ? 75 : 90;
            float stabTime = 60;
            float AttackTime = stabTime + 40;

            if (AttackF1)
            {
                LockVector3 = NPC.Center;
                AttackF1 = false;
                NPC.netUpdate = true;
                Flying = true;

                SoundEngine.PlaySound(ScreechSound1, NPC.Center);

                RuneFormation = Formations.Spear;
                RuneFormationTimer = 0;

                GunRotation = NPC.DirectionTo(player.Center).ToRotation();

                if (WorldSavingSystem.MasochistModeReal)
                {
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center).RotatedBy(MathF.PI * 0.4f * (Main.rand.NextBool() ? 1 : -1)) * 10f, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer, 32f, 0);
                }
            }
            AuraCenter = LockVector3;

            if (AI_Timer < StartTime) // prep, get in position
            {
                HitPlayer = false;
                float directionToPlayer = NPC.HorizontalDirectionTo(player.Center);

                Vector2 desiredPos = player.Center - Vector2.UnitY * 250 - Vector2.UnitX * directionToPlayer * 280;
                FlyingState(1.5f, false, desiredPos, orient: true);

                GunRotation = MathHelper.Lerp(GunRotation, NPC.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2, 0.4f);

                Vector2 forward = (GunRotation - MathHelper.PiOver2).ToRotationVector2();
                LockVector1 = forward.RotatedBy(-directionToPlayer * MathHelper.PiOver2) * ChunkDistance - forward * 100;

                //triangulation to find spear direction
                LockVector2 = (NPC.Center + LockVector1).DirectionTo(player.Center);

                if (AI_Timer == StartTime - 60)
                {
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LifeRunespearHitbox>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, NPC.whoAmI, AttackTime + 60);
                }
            }
            else // rest of attack
            {
                if (AI_Timer == StartTime)
                    SoundEngine.PlaySound(DashSound1, NPC.Center);

                NPC.velocity *= 0.965f;
                float rotationIncrement = WorldSavingSystem.EternityMode ? 0.005f : 0f;
                NPC.velocity = NPC.velocity.RotateTowards(NPC.DirectionTo(player.Center).ToRotation(), rotationIncrement);
                GunRotation = GunRotation.ToRotationVector2().RotateTowards(NPC.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2, rotationIncrement).ToRotation();

                float attackTimer = AI_Timer - StartTime;

                if (attackTimer < stabTime)
                {
                    HitPlayer = true;
                    float stabModifier = 1f - (attackTimer / stabTime);
                    stabModifier = MathF.Sqrt(stabModifier);

                    LockVector2 = LockVector2.RotateTowards(NPC.DirectionTo(player.Center).ToRotation(), rotationIncrement * 2f * stabModifier);

                    LockVector1 += LockVector2 * 2f * stabModifier;
                }
                else
                    HitPlayer = false;
            }   
                
            Flying = false;

            // little stab charge
            int lerpTime = 10;
            if (AI_Timer >= StartTime && AI_Timer <= StartTime + lerpTime)
            {
                float progress = (float)(AI_Timer - StartTime) / lerpTime;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center) * 22, progress);
            }

            
            // p2 explosion
            if (!PhaseOne)
            {
                float explosionTime = 55;
                if (AI_Timer == StartTime + AttackTime - explosionTime - 15)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + LockVector1 + LockVector2 * (LifeRunespearHitbox.Length + 30), Vector2.Zero, ModContent.ProjectileType<LifeRunespearExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, -explosionTime, NPC.whoAmI);
                }
                if (AI_Timer > StartTime + AttackTime - explosionTime - 5)
                    FlyingState(0.3f, false, player.Center - (LockVector1 + LockVector2 * (LifeRunespearHitbox.Length + 30)));
            }

            // reset
            if (AI_Timer >= StartTime + AttackTime)
            {
                HitPlayer = false;
                StateReset();
            }
        }
        
        public void CrystallineCongregation()
        {
            ref float SlurpTimer = ref NPC.ai[2];
            ref float BurpTimer = ref NPC.ai[3];

            Player Player = Main.player[NPC.target];
            if (AttackF1)
            {
                //only do attack when in range
                //this had bugs and is currently disabled, may be changed in the future
                //update: this has been replaced with not doing the attack in the first place if too far away (conditional attacks baybeee)

                //Vector2 targetPos = Player.Center;
                //targetPos.Y -= 16 * 15;
                //if (NPC.Distance(targetPos) < 18 * 10 || WorldSavingSystem.MasochistModeReal)

                AttackF1 = false;
                NPC.netUpdate = true;

                RuneFormation = Formations.Circle;
                RuneFormationTimer = 0;
            }

            if (NPC.Distance(Player.Center) > 2000)
            {
                FlyingState(1.5f);
            }

            NPC.velocity = Vector2.Zero;
            Flying = false;
            DoAura = true;

            float knockBack = 3f;
            double rad = AI_Timer * 5.721237 * (MathHelper.Pi / 180.0);

            double dustdist = 1200;
            if (!WorldSavingSystem.MasochistModeReal)
            {
                float distanceToPlayer = NPC.Distance(Player.Center);
                distanceToPlayer += 240;
                dustdist = Math.Max(dustdist, distanceToPlayer); //take higher of these values
                dustdist = Math.Min(dustdist, 2400); //capped at this value
            }

            int DustX = (int)NPC.Center.X - (int)(Math.Cos(rad) * dustdist);
            int DustY = (int)NPC.Center.Y - (int)(Math.Sin(rad) * dustdist);
            Vector2 DustV = new(DustX, DustY);
            if (SlurpTimer >= 2f && AI_Timer <= 300f)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), DustV, Vector2.Zero, ModContent.ProjectileType<LifeSlurp>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), knockBack, Main.myPlayer, 0, NPC.whoAmI);
                }

                SlurpTimer = 0f;
            }
            SlurpTimer += 1f;
            if (AI_Timer < 300f)
            {
                if (BurpTimer > 15f)
                {
                    SoundEngine.PlaySound(SoundID.Item101, DustV);

                    if (WorldSavingSystem.MasochistModeReal && AI_Timer % 2 == 0) //extra projectiles in maso
                    {
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                        float ProjectileSpeed = 10f;
                        float knockBack2 = 3f;
                        Vector2 shootatPlayer = NPC.SafeDirectionTo(Player.Center) * ProjectileSpeed;
                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootatPlayer, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), knockBack2, Main.myPlayer);

                        //for (int i = -2; i <= 2; i++)
                        //{
                        //    if (FargoSoulsUtil.HostCheck)
                        //        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 0.9f * NPC.SafeDirectionTo(Player.Center).RotatedBy(MathHelper.ToRadians(3) * i), ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, -60, 2f);
                        //}
                    }
                    BurpTimer = 0f;
                }
                BurpTimer += 1f;
            }
            int endTime = WorldSavingSystem.EternityMode ? 660 : 360;

            if (AI_Timer > 300f && AI_Timer < endTime - 60)
            {
                if (BurpTimer > 15f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
                    BurpTimer = 0f;
                }
                BurpTimer += 1f;
            }

            if (!WorldSavingSystem.MasochistModeReal && AI_Timer < 120)
            {
                SlurpTimer -= 0.5f;
                BurpTimer -= 0.5f;
            }

            if (AI_Timer >= endTime)
            {
                StateReset();
            }
        }
        #endregion
        #endregion
        #region Overrides
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (phaseProtectionDR)
                modifiers.FinalDamage /= 4f;
            else if (useDR)
                modifiers.FinalDamage /= 2.5f;
        }
        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ModContent.ProjectileType<DecrepitAirstrikeNuke>() || projectile.type == ModContent.ProjectileType<DecrepitAirstrikeNukeSplinter>())
            {
                modifiers.FinalDamage *= 0.75f;
            }
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (NPC.lifeRegen < 0)
            {
                NPC.lifeRegen /= 2;
            }
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
        public override bool CanHitNPC(NPC target)
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

            if (PyramidPhase == 1)
                ellipseDim /= 2;

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

            return x * x / (a * a) + y * y / (b * b) < 1; //point collision detection
        }
        #endregion
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 400; i++)
                {
                    Color color = Main.rand.NextFromList(Color.Goldenrod, Color.Pink, Color.Cyan);
                    Particle p = new SmallSparkle(
                        worldPosition: NPC.Center,
                        velocity: (Main.rand.NextFloat(5, 50) * Vector2.UnitX).RotatedByRandom(MathHelper.TwoPi),
                        drawColor: color,
                        scale: 1f,
                        lifetime: Main.rand.Next(20, 80),
                        rotation: 0,
                        rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                        );
                    p.Spawn();
                    p.Position -= p.Velocity * 4; //implosion
                }
                return;
            }
            else
            {
                if (NPC.GetLifePercent() < (float)chunklist.Count / (float)ChunkCount && State != (int)States.Opening && PyramidPhase == 0) //not during opening or pyramid attacks
                {
                    if (chunklist.Count <= 0)
                    {
                        return;
                    }
                    int i = Main.rand.Next(chunklist.Count);
                    Vector4 chunk = chunklist[i];
                    Vector2 pos = chunk.X * Vector2.UnitX * ChunkDistance + chunk.Y * Vector2.UnitY * ChunkDistance;// + chunk.Z * Vector3.UnitZ;
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"ShardGold{chunk.W}").Type, NPC.scale);
                    chunklist.RemoveAt(i);
                    SoundEngine.PlaySound(SoundID.Tink, NPC.Center);
                }
            }
        }
        public const int ChunkCount = 10 * 5;
        public const int RuneCount = 12;
        const int ChunkSpriteCount = 12;
        const string PartsPath = "FargowiltasSouls/Assets/ExtraTextures/LifelightParts/";
        internal struct Rune
        {
            public Rune(Vector3 center, int index, float scale, float rotation)
            {
                Center = center;
                Index = index;
                Scale = scale;
                Rotation = rotation;
            }
            public Vector3 Center;
            public int Index;
            public float Scale;
            public float Rotation;
        }
        List<Rune> PostdrawRunes = [];
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            const float ChunkRotationSpeed = MathHelper.TwoPi * (1f / 360);

            Vector2 drawCenter = NPC.Center - screenPos;

            for (int i = 0; i < chunklist.Count; i++)
            {
                chunklist[i] = RotateByMatrix(chunklist[i], ChunkRotationSpeed, Vector3.UnitX);
                chunklist[i] = RotateByMatrix(chunklist[i], -ChunkRotationSpeed / 2, Vector3.UnitZ);
                chunklist[i] = RotateByMatrix(chunklist[i], -ChunkRotationSpeed / 4, Vector3.UnitY);

            }
            chunklist.Sort(delegate (Vector4 x, Vector4 y)
            {
                return Math.Sign(x.Z - y.Z);
            });
            foreach (Vector4 chunk in chunklist.Where(pos => pos.Z <= 0))
            {
                DrawChunk(chunk, spriteBatch, drawColor);
            }
            List<Rune> PredrawRunes = [];
            PostdrawRunes = [];
            if (Draw || NPC.IsABestiaryIconDummy)
            {
                if (DrawRunes)
                {

                    for (int i = 0; i < RuneCount; i++)
                    {

                        float drawRot = (float)(BodyRotation + Math.PI * 2 / RuneCount * i);

                        Vector2 circlePos = drawCenter + drawRot.ToRotationVector2() * RuneDistance;


                        float runeRotation = drawRot + MathHelper.PiOver2;
                        Vector2 drawPos;
                        float runeScale = NPC.scale;
                        float runeLerp = (drawRot % MathF.Tau) / MathF.Tau;
                        float z = 0;

                        Vector2 GetFormationPosition(int formation)
                        {
                            switch (formation)
                            {
                                case Formations.Shield:
                                    {
                                        float rotation = GunRotation;

                                        float rotationForward = rotation;
                                        Vector2 spearPoint = drawCenter + rotationForward.ToRotationVector2() * DefaultRuneDistance * 1.2f;
                                        float spearRuneLerp = runeLerp - 0.5f;
                                        Vector2 spearOffset = Vector2.Zero;
                                        const float SpearAngle = MathF.PI * 0.13f;
                                        float spearLength = NPC.width * 1.2f;
                                        z = Math.Abs(spearRuneLerp);
                                        spearOffset = -(rotation - (SpearAngle * MathHelper.Lerp(1f, 3f, MathF.Pow(Math.Abs(spearRuneLerp), 1.5f)) * Math.Sign(spearRuneLerp))).ToRotationVector2() * spearLength * Math.Abs(spearRuneLerp);
                                        Vector2 spearPos = spearPoint + spearOffset;

                                        return spearPos;
                                    }
                                case Formations.Gun:
                                case Formations.GunCloser:
                                    {
                                        float gunRot = drawRot;
                                        Vector2 circleCenter = drawCenter + GunCircleCenter(1) - NPC.Center; // remove duplicate NPC.Center
                                        float circleRadius = 90;
                                        if (formation == Formations.GunCloser)
                                            circleRadius += 40;
                                        float deformMult = MathF.Pow(Math.Abs(gunRot.ToRotationVector2().X), 1);
                                        circleRadius *= MathHelper.Lerp(1, 0.6f, deformMult); // circle deformation
                                        Vector2 runeCircleOffset = (GunRotation + gunRot).ToRotationVector2() * circleRadius;
                                        Vector2 gunPos = circleCenter + runeCircleOffset;

                                        float rot = (gunRot + MathF.PI / 2) % MathF.Tau;
                                        float scaleMult = (rot.ToRotationVector2().Y + 1) / 2;
                                        runeScale *= MathHelper.Lerp(1.3f, 0.7f, scaleMult);
                                        z = scaleMult;

                                        return gunPos;
                                    }
                                case Formations.Scattered:
                                    {
                                        runeRotation = MathHelper.SmoothStep(runeRotation, 0, FormationLerp);
                                        return CustomRunePositions[i] - screenPos;
                                    }
                                case Formations.Spear:
                                    {
                                        Vector2 spearHilt = drawCenter + LockVector1;
                                        Vector2 spearDirection = LockVector2;

                                        float totalLength = LifeRunespearHitbox.Length;
                                        float spearRunes = RuneCount;// - 2;
                                        Vector2 spearPos = spearHilt + spearDirection * (totalLength / spearRunes) * i;
                                        /*
                                        if (i < 2)
                                        {
                                            float dir = i == 0 ? 1 : -1;
                                            spearPos = spearHilt + spearDirection * (totalLength / spearRunes) * (spearRunes + 0.2f) + spearDirection.RotatedBy(MathHelper.PiOver2 * dir) * 14;
                                        }
                                        */

                                        return spearPos;
                                    }
                                default:
                                    {
                                        return circlePos;
                                    }
                            };
                        }
                        Vector2 previousPos = GetFormationPosition(OldRuneFormation);
                        Vector2 newPos = GetFormationPosition(RuneFormation);
                        drawPos = Vector2.SmoothStep(previousPos, newPos, FormationLerp);

                        if (z <= 0)
                        {
                            PredrawRunes.Add(new(new Vector3(drawPos.X, drawPos.Y, z), i, runeScale, runeRotation));
                        }
                        else
                        {
                            PostdrawRunes.Add(new(new Vector3(drawPos.X, drawPos.Y, z), i, runeScale, runeRotation));
                        }
                    }
                }
                if (PredrawRunes.Count != 0)
                {
                    PredrawRunes.Sort(delegate (Rune x, Rune y)
                    {
                        return Math.Sign(x.Center.Z - y.Center.Z);
                    });
                    foreach (Rune rune in PredrawRunes)
                    {
                        DrawRune(rune, spriteBatch, drawColor);
                    }
                }

                //draw arena runes
                if (DoAura)
                {
                    const int AuraRuneCount = 48;
                    for (int i = 0; i < AuraRuneCount; i++)
                    {
                        float rotation = (RuneFormation == Formations.Gun || OldRuneFormation == Formations.Gun) ? BodyRotation / MathHelper.Lerp(1, 3, FormationLerp) : BodyRotation;
                        float drawRot = (float)(rotation + Math.PI * 2 / AuraRuneCount * i);
                        Texture2D RuneTexture = ModContent.Request<Texture2D>(PartsPath + $"Rune{(i % RuneCount) + 1}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        Vector2 drawPos = AuraCenter + drawRot.ToRotationVector2() * (1100 + RuneDistance) - screenPos;
                        float RuneRotation = drawRot + MathHelper.PiOver2;

                        //rune glow
                        for (int j = 0; j < AuraRuneCount; j++)
                        {
                            Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                            Color glowColor;

                            if (i % 3 == 0) //cyan
                                glowColor = new Color(0f, 1f, 1f, 0f) * 0.7f;
                            else if (i % 3 == 1) //yellow
                                glowColor = new Color(1f, 1f, 0f, 0f) * 0.7f;
                            else //pink
                                glowColor = new Color(1, 192 / 255f, 203 / 255f, 0f) * 0.7f;

                            Main.spriteBatch.Draw(RuneTexture, drawPos + afterimageOffset, null, glowColor, RuneRotation, RuneTexture.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
                        }
                        spriteBatch.Draw(origin: new Vector2(RuneTexture.Width / 2, RuneTexture.Height / 2), texture: RuneTexture, position: drawPos, sourceRectangle: null, color: Color.White, rotation: RuneRotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                    }
                }

                //draw wings
                //draws 4 things: 2 upper wings, 2 lower wings
                if (ChunkDistance > 3)
                {
                    Texture2D wingUtexture = ModContent.Request<Texture2D>(PartsPath + "Lifelight_WingUpper", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Texture2D wingLtexture = ModContent.Request<Texture2D>(PartsPath + "Lifelight_WingLower", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Vector2 wingdrawPos = NPC.Center - screenPos;
                    int currentFrame = NPC.frame.Y;
                    int wingUHeight = wingUtexture.Height / Main.npcFrameCount[NPC.type];
                    Rectangle wingURectangle = new(0, currentFrame * wingUHeight, wingUtexture.Width, wingUHeight);
                    int wingLHeight = wingLtexture.Height / Main.npcFrameCount[NPC.type];
                    Rectangle wingLRectangle = new(0, currentFrame * wingLHeight, wingLtexture.Width, wingLHeight);
                    Vector2 wingUOrigin = new(wingUtexture.Width / 2, wingUtexture.Height / 2 / Main.npcFrameCount[NPC.type]);
                    Vector2 wingLOrigin = new(wingLtexture.Width / 2, wingLtexture.Height / 2 / Main.npcFrameCount[NPC.type]);

                    float distance = ChunkDistance;
                    if (RuneFormation == Formations.Shield || OldRuneFormation == Formations.Shield)
                        distance /= MathHelper.Lerp(1, 1.5f, MathHelper.Clamp((float)RuneFormationTimer / FormationTime, 0, 1));

                    for (int i = -1; i < 2; i += 2)
                    {
                        float wingLRotation = NPC.rotation - MathHelper.PiOver2 + MathHelper.ToRadians(110 * i);
                        float wingURotation = NPC.rotation - MathHelper.PiOver2 + MathHelper.ToRadians(70 * i);
                        SpriteEffects flip = i == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                        spriteBatch.Draw(origin: wingUOrigin, texture: wingUtexture, position: wingdrawPos + wingURotation.ToRotationVector2() * (distance + 30), sourceRectangle: wingURectangle, color: drawColor, rotation: wingURotation, scale: NPC.scale, effects: flip, layerDepth: 0f);
                        spriteBatch.Draw(origin: wingLOrigin, texture: wingLtexture, position: wingdrawPos + wingLRotation.ToRotationVector2() * (distance + 30), sourceRectangle: wingLRectangle, color: drawColor, rotation: wingLRotation, scale: NPC.scale, effects: flip, layerDepth: 0f);
                    }
                }


            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) //DRAW STAR
        {

            Vector2 drawCenter = NPC.Center - screenPos;
            if (Main.LocalPlayer.gravDir < 0)
            {
                drawCenter.Y = Main.screenHeight - drawCenter.Y;
            }
            //if ((SpritePhase > 1 || !Draw) && !NPC.IsABestiaryIconDummy) //star
            if (ChunkDistance > 20)
            {

                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);


                Texture2D star = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle rect = new(0, 0, star.Width, star.Height);
                float scale = 0.45f * Main.rand.NextFloat(1f, 2.5f);
                Vector2 origin = new(star.Width / 2 + scale, star.Height / 2 + scale);

                Vector2 pos = drawCenter;
                if (NPC.IsABestiaryIconDummy)
                {
                    pos += Vector2.UnitX * 85 + Vector2.UnitY * 48;
                }
                spriteBatch.Draw(star, pos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                DrawData starDraw = new(star, pos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.HotPink).UseSecondaryColor(Color.HotPink);
                GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
                starDraw.Draw(spriteBatch);


                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            foreach (Vector4 chunk in chunklist.Where(pos => pos.Z > 0))
            {
                DrawChunk(chunk, spriteBatch, drawColor);
            }
            if (PyramidPhase != 0) //draw pyramid
            {
                float pyramidRot = LockVector1.RotatedBy(rot + MathHelper.PiOver2).ToRotation();
                if (PyramidPhase == 1 && PyramidTimer > PyramidAnimationTime)
                {
                    Texture2D pyramid = ModContent.Request<Texture2D>(PartsPath + "PyramidFull", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Rectangle pyramidRect = new(0, 0, pyramid.Width, pyramid.Height);
                    Vector2 pyramidOrigin = pyramidRect.Size() / 2;
                    spriteBatch.Draw(origin: pyramidOrigin, texture: pyramid, position: NPC.Center - screenPos, sourceRectangle: pyramidRect, color: drawColor, rotation: pyramidRot, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                }
                else
                {
                    Texture2D[] pyramidp = new Texture2D[4];
                    Rectangle[] rects = new Rectangle[4];
                    Vector2[] origins = new Vector2[4];
                    Vector2[] offsets = new Vector2[4];

                    pyramidp[0] = ModContent.Request<Texture2D>(PartsPath + "Pyramid_U", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    pyramidp[1] = ModContent.Request<Texture2D>(PartsPath + "Pyramid_L", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    pyramidp[2] = ModContent.Request<Texture2D>(PartsPath + "Pyramid_R", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    pyramidp[3] = ModContent.Request<Texture2D>(PartsPath + "Pyramid_D", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    float progress = 0;
                    if (PyramidPhase > 0)
                    {
                        progress = 1 - Math.Min((float)PyramidTimer / PyramidAnimationTime, 1f);
                    }
                    else if (PyramidPhase < 0)
                    {
                        progress = Math.Min((float)PyramidTimer * 4 / PyramidAnimationTime, 1f);
                    }
                    float expansion = progress;
                    byte alpha = (byte)(255 * (1 - progress));
                    offsets[0] = new Vector2(0, -15) * expansion + new Vector2(0, -30); //top
                    offsets[1] = new Vector2(-12.5f, 3) * expansion + new Vector2(-25, 10); //left
                    offsets[2] = new Vector2(12.5f, 3) * expansion + new Vector2(25, 10); //right
                    offsets[3] = new Vector2(0, 10) * expansion + new Vector2(0, 20);  //bottom

                    Color color = drawColor;
                    color.A = alpha;


                    for (int i = 0; i < 4; i++)
                    {
                        rects[i] = new Rectangle(0, 0, pyramidp[i].Width, pyramidp[i].Height);
                        origins[i] = pyramidp[i].Size() / 2;
                        offsets[i] = offsets[i].RotatedBy(pyramidRot);


                        spriteBatch.Draw(origin: origins[i], texture: pyramidp[i], position: NPC.Center + offsets[i] - screenPos, sourceRectangle: rects[i], color: color, rotation: pyramidRot, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
                    }
                }
            }
            if (PostdrawRunes.Any())
            {
                PostdrawRunes.Sort(delegate (Rune x, Rune y)
                {
                    return Math.Sign(x.Center.Z - y.Center.Z);
                });
                foreach (Rune rune in PostdrawRunes)
                {
                    DrawRune(rune, spriteBatch, drawColor);
                }
            }
        }
        private void DrawRune(Rune rune, SpriteBatch spriteBatch, Color drawColor)
        {
            int i = rune.Index;
            Texture2D RuneTexture = ModContent.Request<Texture2D>(PartsPath + $"Rune{i + 1}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawPos = new(rune.Center.X, rune.Center.Y);
            //rune glow
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f * rune.Scale;
                Color glowColor;

                if (i % 3 == 0) //cyan
                    glowColor = new Color(0f, 1f, 1f, 0f) * 0.7f;
                else if (i % 3 == 1) //yellow
                    glowColor = new Color(1f, 1f, 0f, 0f) * 0.7f;
                else //pink
                    glowColor = new Color(1, 192 / 255f, 203 / 255f, 0f) * 0.7f;

                Main.spriteBatch.Draw(RuneTexture, drawPos + afterimageOffset, null, glowColor, rune.Rotation, RuneTexture.Size() * 0.5f, rune.Scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(origin: new Vector2(RuneTexture.Width / 2, RuneTexture.Height / 2), texture: RuneTexture, position: drawPos, sourceRectangle: null, color: Color.White, rotation: rune.Rotation, scale: rune.Scale, effects: SpriteEffects.None, layerDepth: 0f);
        }
        private void DrawChunk(Vector4 chunk, SpriteBatch spriteBatch, Color drawColor)
        {
            if (ChunkDistance <= 20)
            {
                return;
            }
            Vector3 pos = chunk.X * Vector3.UnitX + chunk.Y * Vector3.UnitY + chunk.Z * Vector3.UnitZ;
            string textureString = $"ShardGold{chunk.W}";
            float scale = 0.3f * pos.Z;

            byte alpha = (byte)(150 + (100f * pos.Z));


            Color color = drawColor;
            color.A = alpha;
            Vector2 drawCenter = NPC.Center - Main.screenPosition;
            float distance = ChunkDistance;
            if (RuneFormation == Formations.Shield || OldRuneFormation == Formations.Shield)
                distance /= MathHelper.Lerp(1, 1.5f, MathHelper.Clamp((float)RuneFormationTimer / FormationTime, 0, 1));
            Vector2 chunkOffset = pos.X * Vector2.UnitX * distance + pos.Y * Vector2.UnitY * distance;
            Vector2 drawPos = drawCenter + chunkOffset;
            Texture2D ChunkTexture = ModContent.Request<Texture2D>(PartsPath + textureString, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(origin: new Vector2(ChunkTexture.Width / 2, ChunkTexture.Height / 2), texture: ChunkTexture, position: drawPos, sourceRectangle: null, color: color, rotation: 0, scale: NPC.scale + scale, effects: SpriteEffects.None, layerDepth: 0f);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = NPC.scale * 20;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            const float LerpTime = 60;
            float timer = (Main.GameUpdateCount % (LerpTime * 3)) / LerpTime;
            timer += completionRatio * 2;
            Color color = timer switch
            {
                _ when timer <= 1 => Color.Lerp(Color.Cyan, Color.Goldenrod, timer),
                _ when timer > 1 && timer <= 2 => Color.Lerp(Color.Goldenrod, Color.DeepPink, timer - 1),
                _ => Color.Lerp(Color.DeepPink, Color.Cyan, timer - 2)
            };
            return Color.Lerp(color, Color.Transparent, completionRatio * FormationLerp) * 0.7f;
        }
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.BeforeNPCs;
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            if (RuneFormation != Formations.Shield)
                return;
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(NPC.oldPos, new(WidthFunction, ColorFunction, _ => NPC.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }

        public override void FindFrame(int frameHeight)
        {
            double fpf = 60 / 10; //  60/fps
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter += 1;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type] * fpf;
            NPC.frame.Y = (int)(NPC.frameCounter / fpf);
        }
        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight], -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<LifelightBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LifelightTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<LifelightRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<LifelightMasterPet>(), 4));

            LeadingConditionRule rule = new(new Conditions.NotExpert());
            rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<EnchantedLifeblade>(), ModContent.ItemType<Lightslinger>(), ModContent.ItemType<CrystallineCongregation>(), ModContent.ItemType<KamikazePixieStaff>()));
            rule.OnSuccess(ItemDropRule.Common(ItemID.HallowedFishingCrateHard, 1, 1, 5)); //divine crate
            rule.OnSuccess(ItemDropRule.Common(ItemID.SoulofLight, 1, 1, 3));
            rule.OnSuccess(ItemDropRule.Common(ItemID.PixieDust, 1, 15, 25));

            npcLoot.Add(rule);
        }
        #endregion
        #region Help Methods

        public void ResetVariables()
        {
            AI_Timer = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.ai[0] = 0f;
            NPC.localAI[0] = 0;
            NPC.localAI[1] = 0;
            NPC.localAI[2] = 0;
            AttackF1 = true;
            NPC.netUpdate = true;
        }
        public void StateReset()
        {
            NPC.TargetClosest(true);
            
            ChooseState();
            ResetVariables();

        }
        public void GoToDeathray()
        {
            if (!WorldSavingSystem.EternityMode)
            {
                StateReset();
                return;
            }
            NPC.TargetClosest(true);
            ResetVariables();
            State = (int)States.Deathray;
        }


        public void ChooseState() //it's done this way so it cycles between attacks in a random order: for increased variety
        {
            NPC.netUpdate = true;

            AttackCount++;
            int max = (!PhaseOne && WorldSavingSystem.EternityMode) ? 5 : 4;
            if (AttackCount > max)
                AttackCount = 1;

            var doableStates = new List<States>(AttackCount switch
            {
                2 => ChargeAttacks,
                4 => PlungeAttacks,
                5 => StrongAttacks,
                _ => PrecisionAttacks
            });

            if (PhaseOne && NPC.life < P2Threshold)
                State = (int)States.P1Transition;
            else
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    int i = AttackCount switch
                    {
                        2 => 0,
                        4 => 1,
                        5 => 2,
                        _ => 3
                    };
                    if (LastAttack[i] >= 0 && doableStates.Count > 1)
                        doableStates.Remove((States)LastAttack[i]);
                    State = (int)Main.rand.NextFromCollection(doableStates);
                    LastAttack[i] = State;
                }
            }
        }

        private static Vector4 RotateByMatrix(Vector4 obj, float radians, Vector3 axis)
        {
            Vector3 vector = obj.X * Vector3.UnitX + obj.Y * Vector3.UnitY + obj.Z * Vector3.UnitZ;
            Matrix rotationMatrix;
            if (axis == Vector3.UnitX)
            {
                rotationMatrix = Matrix.CreateRotationX(radians);
            }
            else if (axis == Vector3.UnitY)
            {
                rotationMatrix = Matrix.CreateRotationY(radians);
            }
            else
            {
                rotationMatrix = Matrix.CreateRotationZ(radians);
            }
            vector = Vector3.Transform(vector, rotationMatrix);
            obj.X = vector.X;
            obj.Y = vector.Y;
            obj.Z = vector.Z;
            return obj;
        }
        #endregion
    }
}
