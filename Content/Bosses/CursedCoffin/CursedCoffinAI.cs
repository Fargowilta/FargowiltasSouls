using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.StateMachines;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
    public partial class CursedCoffin : ModNPC
    {
        #region Variables
        public const int WavyShotFlightPrepTime = 60;
        public const int WavyShotFlightCirclingTime = 280;
        public const int WavyShotFlightEndTime = 0;
        public const int RandomStuffOpenTime = 60;


        public static readonly SoundStyle PhaseTransitionSFX = new("FargowiltasSouls/Assets/Sounds/CoffinPhaseTransition");
        public static readonly SoundStyle SlamSFX = new("FargowiltasSouls/Assets/Sounds/CoffinSlam") { PitchVariance = 0.3f };
        public static readonly SoundStyle SpiritDroneSFX = new("FargowiltasSouls/Assets/Sounds/CoffinSpiritDrone") { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, Volume = 0.2f };
        public static readonly SoundStyle BigShotSFX = new("FargowiltasSouls/Assets/Sounds/CoffinBigShot") { Volume = 0.6f, PitchVariance = 0.3f };
        public static readonly SoundStyle ShotSFX = new("FargowiltasSouls/Assets/Sounds/CoffinShot") { Volume = 0.3f, PitchVariance = 0.3f };
        public static readonly SoundStyle SoulShotSFX = new("FargowiltasSouls/Assets/Sounds/CoffinSoulShot") { Volume = 0.3f, PitchVariance = 0.3f };
        public static readonly SoundStyle HandChargeSFX = new("FargowiltasSouls/Assets/Sounds/CoffinHandCharge");

        public enum BehaviorStates
        {
            Opening,
            PhaseTransition,
            StunPunish,
            SpiritGrabPunish,
            HoveringForSlam,
            SlamWShockwave,
            WavyShotCircle,
            WavyShotFlight,
            GrabbyHands,
            RandomStuff,

            // For the state machine.
            RefillAttacks,
            Count
        }

        private readonly List<BehaviorStates> P1Attacks = new List<BehaviorStates>
        {
            BehaviorStates.HoveringForSlam,
            BehaviorStates.WavyShotCircle,
            BehaviorStates.WavyShotFlight,
            BehaviorStates.GrabbyHands
        };
        private readonly List<BehaviorStates> P2Attacks = new List<BehaviorStates>
        {
            BehaviorStates.HoveringForSlam,
            BehaviorStates.WavyShotCircle,
            BehaviorStates.WavyShotFlight,
            BehaviorStates.GrabbyHands,
            BehaviorStates.RandomStuff
        };

        public Player Player => Main.player[NPC.target];

        #endregion
        #region AI
        public override void OnSpawn(IEntitySource source)
        {
            Targeting();
            Player player = Main.player[NPC.target];
            if (player.Alive())
            {
                NPC.position = player.Center + new Vector2(0, -700) - NPC.Size / 2;
                LockVector1 = player.Top - Vector2.UnitY * 50;
                NPC.velocity = new Vector2(0, 0.25f);
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.noTileCollide || (Player.Top.Y > NPC.Bottom.Y + 30) ? true : null;
        public override void AI()
        {
            //Defaults
            NPC.defense = NPC.defDefense;
            if (PhaseTwo)
                NPC.defense += 15;
            NPC.rotation = 0;
            NPC.noTileCollide = true;

            if (!Targeting())
                return;
            NPC.timeLeft = 60;

            // Refill the attacks if empty.
            if ((StateMachine?.StateStack?.Count ?? 1) <= 0)
                StateMachine.StateStack.Push(StateMachine.StateRegistry[BehaviorStates.RefillAttacks]);

            // Update the state machine.
            StateMachine.PerformBehaviors();
            StateMachine.PerformStateTransitionCheck();

            // Ensure that there is a valid state timer to get.
            if (StateMachine.StateStack.Count > 0)
                Timer++;
        }
        #endregion

        #region States
        // These might have 0 references, but it is automatically called due to having the attribute, do not remove!
        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.Opening)]
        public void Opening()
        {
            if (Timer >= 0)
            {
                ExtraTrail = true;
                if (Math.Abs(NPC.velocity.Y) < 22)
                    NPC.velocity.Y *= 1.04f;
                if (NPC.Center.Y >= LockVector1.Y || Timer > 60 * 2)
                {
                    NPC.noTileCollide = false;
                    if (NPC.velocity.Y <= 1) //when you hit tile
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                        SoundEngine.PlaySound(SlamSFX, NPC.Center);
                        //dust explosion
                        ExtraTrail = false;
                        Timer = -60;
                        //shockwaves
                        if (FargoSoulsUtil.HostCheck)
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Vector2 vel = Vector2.UnitX * i * 3;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom - Vector2.UnitY * 50, vel, ModContent.ProjectileType<CoffinSlamShockwave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer);
                            }
                        }

                        // drop summon
                        if (WorldSavingSystem.EternityMode && !WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CursedCoffin] && FargoSoulsUtil.HostCheck)
                            Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<CoffinSummon>());
                    }
                }
                if (NPC.Center.Y >= LockVector1.Y + 800) //only go so far
                {
                    NPC.velocity = Vector2.Zero;
                }
            }
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.PhaseTransition)]
        public void PhaseTransition()
        {
            HoverSound();

            const int TransTime = 60;
            if (Timer > TransTime - 5)
                PhaseTwo = true;
            NPC.velocity = -Vector2.UnitY * 5 * (1 - (Timer / (TransTime * 1.5f)));
            NPC.rotation = Main.rand.NextFloat(MathF.Tau * 0.06f * (Timer / TransTime));
            SoundEngine.PlaySound(SpiritDroneSFX, NPC.Center);
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.StunPunish)]
        public void StunPunish()
        {
            NPC.velocity *= 0.95f;
            if (Timer < 20)
            {
                if (++NPC.frameCounter % 4 == 3)
                    if (Frame < Main.npcFrameCount[Type] - 1)
                        Frame++;
            }
            else if (Timer == 20)
            {
                SoundEngine.PlaySound(ShotSFX, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 dir = NPC.rotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer, NPC.whoAmI, 22);
                }
            }
            else
            {
                if (++NPC.frameCounter % 60 == 59)
                    Frame--;
            }
        }
        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.SpiritGrabPunish)]
        public void SpiritGrabPunish()
        {
            ref float initialDir = ref AI2;
            ref float initialDist = ref AI3;
            HoverSound();
            const int PrepTime = 60;

            if (++NPC.frameCounter % 10 == 9 && Frame > 0)
                Frame--;

            if (Timer <= 1)
            {
                initialDir = Player.SafeDirectionTo(NPC.Center).ToRotation();
                initialDist = NPC.Distance(Player.Center);
            }
            if (Timer <= PrepTime)
            {
                float progress = Timer / PrepTime;
                float distance = MathHelper.Lerp(initialDist, 350, progress);
                Vector2 direction = Vector2.Lerp(initialDir.ToRotationVector2(), -Vector2.UnitY, progress);
                Vector2 desiredPos = Player.Center + distance * direction;
                NPC.velocity = (desiredPos - NPC.Center);
            }
        }
        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.HoveringForSlam)]
        public void HoveringForSlam()
        {
            const float WaveAmpX = 200;
            const float WaveAmpY = 30;
            const float XHalfPeriod = 60 * 1.5f;
            const float YHalfPeriod = 60 * 0.75f;
            ref float XThetaOffset = ref AI2;
            ref float RandomTimer = ref AI3;

            HoverSound();

            if (Timer == 1)
            {
                float xOffset = Utils.Clamp(NPC.Center.X - Player.Center.X, -WaveAmpX, WaveAmpX);
                XThetaOffset = MathF.Asin(xOffset / WaveAmpX);
                RandomTimer = Main.rand.Next(160, 220);
                if (!PhaseTwo)
                    RandomTimer -= 55;
            }

            if (Timer < RandomTimer && Timer >= 0)
            {
                NPC.noTileCollide = true;
                float desiredX = WaveAmpX * MathF.Sin(XThetaOffset + MathF.PI * (Timer / XHalfPeriod));
                float desiredY = -350 + WaveAmpY * MathF.Sin(MathF.PI * (Timer / YHalfPeriod));
                Vector2 desiredPos = Player.Center + desiredX * Vector2.UnitX + desiredY * Vector2.UnitY;
                Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);
            }
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.SlamWShockwave)]
        public void SlamWShockwave()
        {
            ref float Counter = ref AI2;

            NPC.noTileCollide = (NPC.Bottom.Y + NPC.velocity.Y < Player.Bottom.Y - 16);

            if (Timer >= 0)
            {
                //if (Timer < 15) // no funny double hits from weird terrain
                //  NPC.noTileCollide = true;

                NPC.velocity.X *= 0.97f;
                float speedUp = Counter == 2 ? 0.35f : 0.2f;
                if (WorldSavingSystem.EternityMode)
                    NPC.velocity.X += Math.Sign(Player.Center.X - NPC.Center.X) * speedUp;
                if (NPC.velocity.Y >= 0 && Counter == 0)
                {
                    Counter = 1;
                }
                if (NPC.velocity.Y == 0 && Counter > 0 && !NPC.noTileCollide) //when you hit tile
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    SoundEngine.PlaySound(SlamSFX, NPC.Center);
                    ExtraTrail = false;

                    //shockwaves
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 vel = Vector2.UnitX * i * 3;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom - Vector2.UnitY * 50, vel, ModContent.ProjectileType<CoffinSlamShockwave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer);
                        }
                    }
                    if (WorldSavingSystem.EternityMode && Counter < 2)
                    {
                        Counter = 2;
                        Timer = 0;
                        NPC.velocity.Y = -10;
                    }
                    else
                    {
                        int endlag = WorldSavingSystem.MasochistModeReal ? 80 : WorldSavingSystem.EternityMode ? 100 : 120;
                        Timer = -endlag;
                        NPC.velocity.X = 0;
                    }
                    return;
                }
                NPC.velocity.Y += 0.175f;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y += 0.32f;
                if (NPC.velocity.Y > 15)
                    NPC.velocity.Y = 15;
                ExtraTrail = true;

                //NPC.noTileCollide = false;

                if (NPC.Center.Y >= LockVector1.Y + 1000) //only go so far
                {
                    NPC.velocity = Vector2.Zero;
                }
            }
            else
            {
                //NPC.noTileCollide = false;
            }
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.WavyShotCircle)]
        public void WavyShotCircle()
        {
            int TelegraphTime = WorldSavingSystem.MasochistModeReal ? 60 : 70;
            float progress = 1 - (Timer / TelegraphTime);
            Vector2 maskCenter = MaskCenter();


            if (Timer < TelegraphTime)
            {
                Vector2 sparkDir = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                float sparkDistance = (120 * progress) * Main.rand.NextFloat(0.6f, 1.3f);
                Vector2 sparkCenter = maskCenter + sparkDir * sparkDistance * 2;
                float sparkTime = 15;
                Vector2 sparkVel = (maskCenter - sparkCenter) / sparkTime;
                float sparkScale = 2f - progress * 1.2f;
                Particle spark = new SparkParticle(sparkCenter, sparkVel, GlowColor, sparkScale, (int)sparkTime);
                spark.Spawn();
            }
            else if (Timer == TelegraphTime)
            {
                SoundEngine.PlaySound(BigShotSFX, maskCenter);
                int shots = Main.expertMode ? WorldSavingSystem.EternityMode ? WorldSavingSystem.MasochistModeReal ? 12 : 10 : 8 : 6;
                if (FargoSoulsUtil.HostCheck)
                {
                    float baseRot = Main.rand.NextFloat(MathF.Tau);
                    for (int i = 0; i < shots; i++)
                    {
                        float rot = baseRot + MathF.Tau * ((float)i / shots);
                        Vector2 vel = rot.ToRotationVector2() * 4;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), maskCenter, vel, ModContent.ProjectileType<CoffinWaveShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer);
                    }
                }
            }
            else if (Timer > TelegraphTime + (WorldSavingSystem.MasochistModeReal || AI3 < 1 ? 20 : 50)) // + endlag
            {
                if (PhaseTwo && AI3 < 1 && WorldSavingSystem.EternityMode)
                {
                    AI3 = 1;
                    Timer = 0;
                    return;
                }
            }
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.WavyShotFlight)]
        public void WavyShotFlight()
        {
            ref float totalRotate = ref AI2;
            ref float circleStart = ref AI3;

            NPC.noTileCollide = true;
            HoverSound();
            const int Distance = 350;

            static float MomentumProgress(float x) => (x * x * 3) - (x * x * x * 2);

            if (Timer <= WavyShotFlightPrepTime)
            {
                Vector2 currentDir = Player.SafeDirectionTo(NPC.Center);
                circleStart = currentDir.ToRotation();

                float rot = FargoSoulsUtil.RotationDifference(currentDir, -Vector2.UnitY);
                float rotDir = Math.Sign(rot);
                Vector2 desiredPos = Player.Center + currentDir * Distance;
                Movement(desiredPos, 0.08f, 30, 5, 0.06f, 50);

                totalRotate = (MathF.Tau - Math.Abs(rot)) * -rotDir;
            }
            else if (Timer <= WavyShotFlightPrepTime + WavyShotFlightCirclingTime)
            {
                float progress = (Timer - WavyShotFlightPrepTime) / WavyShotFlightCirclingTime;
                float circleProgress = MomentumProgress(progress);
                Vector2 desiredPos = Player.Center + (circleStart + (totalRotate + MathF.Tau * Math.Sign(totalRotate)) * circleProgress).ToRotationVector2() * Distance;

                float modifier = Utils.Clamp(progress / 0.2f, 0, 1);
                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredPos - NPC.Center, modifier);

                const float TimePadding = 0.2f;
                const int ShotTime = 15;
                if (Timer % ShotTime == 0 && progress >= TimePadding && progress <= 1 - TimePadding)
                {
                    SoundEngine.PlaySound(ShotSFX, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        Vector2 maskCenter = MaskCenter();
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), maskCenter, maskCenter.SafeDirectionTo(Player.Center).RotatedBy(-MathHelper.Pi / 10) * 4, ModContent.ProjectileType<CoffinWaveShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer, 1);
                    }
                }
            }
            else if (Timer < WavyShotFlightPrepTime + WavyShotFlightCirclingTime + WavyShotFlightEndTime)
                NPC.velocity *= 0.96f;
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.GrabbyHands)]
        public void GrabbyHands()
        {
            NPC.noTileCollide = true;
            HoverSound();
            if (Timer < 40)
            {
                Vector2 offset = -Vector2.UnitY * 300 + Vector2.UnitX * Math.Sign(NPC.Center.X - Player.Center.X) * 200;
                Vector2 desiredPos = Player.Center + offset;
                Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);
            }
            else
                NPC.velocity *= 0.97f;

            if (Timer == 2)
            {
                AI3 = Main.rand.Next(90, 103); // time for hands to grab
                NPC.netUpdate = true;
            }
            if (Timer > 2 && Timer == AI3)
            {

                foreach (Projectile hand in Main.projectile.Where(p => p.TypeAlive<CoffinHand>() && p.ai[0] == NPC.whoAmI && p.ai[1] == 1))
                {
                    SoundEngine.PlaySound(HandChargeSFX, hand.Center);
                    hand.ai[1] = 2;
                    hand.netUpdate = true;
                }
            }
            if (Timer < 40)
            {
                if (++NPC.frameCounter % 4 == 3)
                    if (Frame < Main.npcFrameCount[Type] - 1)
                        Frame++;
            }
            else if (Timer == 40)
            {
                SoundEngine.PlaySound(ShotSFX, NPC.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 dir = NPC.rotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer, NPC.whoAmI, 1, 1);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, dir * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.1f), 1f, Main.myPlayer, NPC.whoAmI, 1, -1);
                    if (WorldSavingSystem.EternityMode)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 4, ModContent.ProjectileType<CoffinHand>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.5f), 1f, Main.myPlayer, NPC.whoAmI, 1, Main.rand.NextBool() ? 1.5f : -1.5f);
                }
            }
            else
            {
                if (++NPC.frameCounter % 60 == 59 && Frame > 0)
                    Frame--;
            }
        }

        [AutoloadAsBehavior<EntityAIState<BehaviorStates>, BehaviorStates>(BehaviorStates.RandomStuff)]
        public void RandomStuff()
        {
            ref float RandomProj = ref AI3;
            NPC.noTileCollide = true;

            Vector2 CalculateAngle()
            {
                ref float RandomProj = ref AI3;
                float gravity = CoffinRandomStuff.Gravity(AI3);

                float xDif = Player.Center.X - NPC.Center.X;
                float yDif = Player.Center.Y - NPC.Center.Y;


                float velY = -10; // initial y vel
                if (yDif < 0) // if player is above
                {
                    float arcTop = yDif - 300;
                    // calculate initial y vel that results in good arc above
                    float newVelY = -MathF.Sqrt(-arcTop * gravity) / 1.5f;
                    if (newVelY < velY)
                        velY = newVelY;
                }

                float t = -velY / gravity + MathF.Sqrt(MathF.Pow(velY / gravity, 2) + (2 * yDif / gravity));
                float velX = xDif / t;
                return velX * Vector2.UnitX + velY * Vector2.UnitY;
            }


            Vector2 dir = CalculateAngle();
            NPC.rotation = Vector2.Lerp(NPC.rotation.ToRotationVector2(), dir, Timer / 35).ToRotation();

            HoverSound();
            Vector2 offset = Vector2.UnitX * Math.Sign(NPC.Center.X - Player.Center.X) * 500;
            Vector2 desiredPos = Player.Center + offset;
            Movement(desiredPos, 0.1f, 10, 5, 0.08f, 20);

            int frameTime = (int)MathF.Floor(RandomStuffOpenTime / Main.npcFrameCount[Type]);
            if (Timer < RandomStuffOpenTime)
            {
                if (++NPC.frameCounter % frameTime == frameTime - 1)
                    if (Frame < Main.npcFrameCount[Type] - 1)
                        Frame++;
            }
            else if (Timer < RandomStuffOpenTime + 310 && Timer >= RandomStuffOpenTime)
            {
                NPC.velocity.X *= 0.7f; // moves slower horizontally 
                int shotTime = WorldSavingSystem.MasochistModeReal ? 20 : 25;
                if (Timer % shotTime == 0)
                {
                    RandomProj = Main.rand.Next(3) switch
                    {
                        1 => 5,
                        2 => 6,
                        _ => Main.rand.Next(5)
                    };
                    NPC.netUpdate = true;
                }
                if (Timer % shotTime == shotTime - 1)
                {
                    SoundStyle sound = RandomProj switch
                    {
                        5 => SoundID.Item106,
                        6 => SoundID.NPCHit2,
                        _ => SoundID.Item101
                    };
                    SoundEngine.PlaySound(sound, NPC.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {

                        Vector2 vel = dir;
                        vel *= Main.rand.NextFloat(0.9f, 1.3f);

                        Vector2 offsetDir = Vector2.Normalize(dir);
                        Vector2 posOffset = offsetDir.RotatedBy(MathF.PI / 2) * Main.rand.NextFloat(-NPC.height / 3, NPC.height / 3);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + posOffset, vel, ModContent.ProjectileType<CoffinRandomStuff>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, Main.myPlayer, RandomProj);
                    }
                }
            }
            else
            {
                NPC.velocity *= 0.96f;
                if (++NPC.frameCounter % 30 == 29 && Frame > 0)
                    Frame--;
                if (Frame > 0)
                    NPC.rotation *= 0.9f;
            }
        }
        #endregion

        #region Help Methods
        public void HoverSound() => SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, Pitch = -0.5f, Volume = 10f }, NPC.Center);

        public void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
        {
            if (NPC.Distance(pos) > slowdown)
                NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
            else
                NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
        }
        public bool Targeting()
        {
            Player player = Main.player[NPC.target];
            //Targeting
            if (!player.active || player.dead || player.ghost || NPC.Distance(player.Center) > 2400)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || player.ghost || NPC.Distance(player.Center) > 2400)
                {
                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;
                    NPC.velocity.Y -= 0.4f;
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}