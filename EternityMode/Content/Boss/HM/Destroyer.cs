using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public class Destroyer : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.TheDestroyer);

        public int AttackModeTimer;
        public int CoilRadius;
        public int LaserTimer;
        public int SecondaryAttackTimer;
        public int RotationDirection = 1;

        public bool InPhase2;
        public bool IsCoiling;
        public bool PrepareToCoil;

        public bool DroppedSummon;

        public const int P2_ATTACK_SPACING = 480;
        public const int P2_COIL_BEGIN_TIME = P2_ATTACK_SPACING * 4;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackModeTimer);
            binaryWriter.Write7BitEncodedInt(CoilRadius);
            binaryWriter.Write7BitEncodedInt(LaserTimer);
            binaryWriter.Write7BitEncodedInt(SecondaryAttackTimer);
            binaryWriter.Write7BitEncodedInt(RotationDirection);
            bitWriter.WriteBit(InPhase2);
            bitWriter.WriteBit(IsCoiling);
            bitWriter.WriteBit(PrepareToCoil);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackModeTimer = binaryReader.Read7BitEncodedInt();
            CoilRadius = binaryReader.Read7BitEncodedInt();
            LaserTimer = binaryReader.Read7BitEncodedInt();
            SecondaryAttackTimer = binaryReader.Read7BitEncodedInt();
            RotationDirection = binaryReader.Read7BitEncodedInt();
            InPhase2 = bitReader.ReadBit();
            IsCoiling = bitReader.ReadBit();
            PrepareToCoil = bitReader.ReadBit();
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.Chilled] = false;
            npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.destroyBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            if (!InPhase2)
            {
                if (npc.life < (int)(npc.lifeMax * (FargoSoulsWorld.MasochistModeReal ? 0.95 : .75)))
                {
                    InPhase2 = true;
                    AttackModeTimer = P2_COIL_BEGIN_TIME;
                    npc.netUpdate = true;
                    if (npc.HasPlayerTarget)
                        SoundEngine.PlaySound(SoundID.Roar, Main.player[npc.target].Center);
                }
            }
            else
            {
                int projDamage = FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 9);

                if (npc.HasValidTarget && !Main.dayTime)
                {
                    npc.timeLeft = 600; //don't despawn

                    if (IsCoiling) //spinning
                    {
                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = true;

                        npc.netUpdate = true;
                        npc.velocity = Vector2.Normalize(npc.velocity) * 20f;
                        npc.velocity += npc.velocity.RotatedBy(MathHelper.PiOver2 * RotationDirection) * npc.velocity.Length() / CoilRadius;
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

                        if (AttackModeTimer == 0)
                            LaserTimer = 0;

                        if (npc.life < npc.lifeMax / 10) //permanent coil phase 3
                        {
                            if (npc.localAI[2] >= 0)
                            {
                                npc.localAI[2] = FargoSoulsWorld.MasochistModeReal ? -60 : 0;
                                AttackModeTimer = 0; //for edge case where destroyer coils, then goes below 10% while coiling, make sure DR behaves right
                            }

                            if (--npc.localAI[2] < -120)
                            {
                                npc.localAI[2] += FargoSoulsWorld.MasochistModeReal ? 3 : 6;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 distance = npc.DirectionTo(Main.player[npc.target].Center) * 14f;
                                    int type = ModContent.ProjectileType<DarkStarHoming>();
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance, type, projDamage, 0f, Main.myPlayer, npc.target);
                                }
                            }

                            if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
                            {
                                npc.TargetClosest(false);
                                if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
                                {
                                    AttackModeTimer = 0;
                                    CoilRadius = 0;
                                    IsCoiling = false;
                                    PrepareToCoil = false;

                                    NetSync(npc);
                                }
                            }

                            AttackModeTimer++;
                        }
                        else
                        {
                            if (--npc.localAI[2] < 0) //shoot star spreads into the circle
                            {
                                npc.localAI[2] = Main.player[npc.target].HasBuff(ModContent.BuffType<LightningRod>()) ? 110 : 65;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 distance = Main.player[npc.target].Center - npc.Center;
                                    double angleModifier = MathHelper.ToRadians(30);
                                    distance.Normalize();
                                    distance *= 14f;
                                    int type = ModContent.ProjectileType<DarkStarHoming>();
                                    int delay = -5;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance.RotatedBy(-angleModifier), type, projDamage, 0f, Main.myPlayer, npc.target, delay);
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance, type, projDamage, 0f, Main.myPlayer, npc.target, delay);
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance.RotatedBy(angleModifier), type, projDamage, 0f, Main.myPlayer, npc.target, delay);
                                }
                            }

                            if (++AttackModeTimer > 300) //go back to normal AI
                            {
                                AttackModeTimer = 0;
                                CoilRadius = 0;
                                IsCoiling = false;
                                PrepareToCoil = false;

                                NetSync(npc);
                            }
                        }

                        Vector2 pivot = npc.Center;
                        pivot += Vector2.Normalize(npc.velocity.RotatedBy(MathHelper.PiOver2 * RotationDirection)) * 600;

                        if (++LaserTimer > 95)
                        {
                            LaserTimer = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float ratio = (float)npc.life / npc.lifeMax;
                                if (FargoSoulsWorld.MasochistModeReal)
                                    ratio = 0;

                                int max = (int)(14f - 10f * ratio);
                                if (max % 2 != 0) //always shoot even number
                                    max++;

                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = npc.DirectionTo(pivot).RotatedBy(2 * Math.PI / max * i);
                                    Vector2 spawnPos = pivot - speed * 600;
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, 0.2f * speed, ModContent.ProjectileType<DestroyerLaser>(), projDamage, 0f, Main.myPlayer, 1f);
                                }
                            }
                        }

                        for (int i = 0; i < 20; i++) //arena dust
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 600);
                            offset.Y += (float)(Math.Cos(angle) * 600);
                            Dust dust = Main.dust[Dust.NewDust(pivot + offset - new Vector2(4, 4), 0, 0, 112, 0, 0, 100, Color.White, 1f)];
                            dust.velocity = Vector2.Zero;
                            if (Main.rand.NextBool(3))
                                dust.velocity += Vector2.Normalize(offset) * 5f;
                            dust.noGravity = true;
                        }

                        Player target = Main.player[npc.target];
                        if (target.active && !target.dead) //arena effect
                        {
                            float distance = target.Distance(pivot);
                            if (distance > 600 && distance < 3000)
                            {
                                Vector2 movement = pivot - target.Center;
                                float difference = movement.Length() - 600;
                                movement.Normalize();
                                movement *= difference < 34f ? difference : 34f;
                                target.position += movement;

                                for (int i = 0; i < 20; i++)
                                {
                                    int d = Dust.NewDust(target.position, target.width, target.height, 112, 0f, 0f, 0, default(Color), 2f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 5f;
                                }
                            }
                        }
                    }
                    else
                    {
                        npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;

                        npc.localAI[2] = 0;

                        float maxSpeed = 16f;    //max speed?
                        float num15 = 0.1f;   //turn speed?
                        float num16 = 0.15f;   //acceleration?

                        bool fastStart = AttackModeTimer < 120;

                        float flySpeedModifierRatio = (float)npc.life / npc.lifeMax;
                        if (flySpeedModifierRatio > 0.5f) //prevent it from subtracting speed
                            flySpeedModifierRatio = 0.5f;
                        if (fastStart) //if just entered this stage, max out ratio
                            flySpeedModifierRatio = 0;

                        if (npc.HasValidTarget)
                        {
                            if (!fastStart) //after fast start to uncoil
                            {
                                float distance = npc.Distance(Main.player[npc.target].Center);
                                if (distance < 600) //slower nearby
                                {
                                    maxSpeed *= 0.5f;
                                }
                                else if (distance > 900) //come at you really hard when out of range
                                {
                                    num15 *= 2f;
                                    num16 *= 2f;
                                }
                            }

                            float comparisonSpeed = Main.player[npc.target].velocity.Length() * 1.5f;
                            float rotationDifference = MathHelper.WrapAngle(npc.velocity.ToRotation() - npc.DirectionTo(Main.player[npc.target].Center).ToRotation());
                            bool inFrontOfMe = Math.Abs(rotationDifference) < MathHelper.ToRadians(90 / 2);
                            if (maxSpeed < comparisonSpeed && inFrontOfMe) //player is moving faster than my top speed
                            {
                                maxSpeed = comparisonSpeed; //outspeed them
                            }
                        }

                        Vector2 target = Main.player[npc.target].Center;
                        if (PrepareToCoil) //move MUCH faster, approach a position nearby
                        {
                            num15 = 0.4f;
                            num16 = 0.5f;

                            target += Main.player[npc.target].DirectionTo(npc.Center) * 600;

                            if (++AttackModeTimer > 120) //move way faster if still not in range
                                maxSpeed *= 2f;

                            if (npc.Distance(target) < 50)
                            {
                                AttackModeTimer = 0;
                                CoilRadius = (int)npc.Distance(Main.player[npc.target].Center);
                                IsCoiling = true;

                                //angle difference from npc velocity, to angle towards player
                                float rotationDiff = MathHelper.WrapAngle(npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - npc.velocity.ToRotation());
                                RotationDirection = Math.Sign(rotationDiff);

                                npc.velocity = 20 * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(-MathHelper.PiOver2 * RotationDirection);

                                npc.netUpdate = true;
                                NetSync(npc);

                                SoundEngine.PlaySound(SoundID.Roar, Main.player[npc.target].Center);
                                if (npc.life < npc.lifeMax / 10)
                                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, Main.player[npc.target].Center); //eoc roar

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < Main.maxProjectiles; i++)
                                    {
                                        if (Main.projectile[i].active && (
                                            Main.projectile[i].type == ModContent.ProjectileType<DarkStarHoming>() ||
                                            Main.projectile[i].type == ModContent.ProjectileType<DarkStarDestroyer>() ||
                                            Main.projectile[i].type == ModContent.ProjectileType<DestroyerLaser>() ||
                                            Main.projectile[i].type == ProjectileID.DeathLaser))
                                        {
                                            Main.projectile[i].Kill();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (npc.life < npc.lifeMax / 10)
                            {
                                if (AttackModeTimer < P2_COIL_BEGIN_TIME) //force begin desperation
                                {
                                    AttackModeTimer = P2_COIL_BEGIN_TIME;
                                    NetSync(npc);
                                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, Main.player[npc.target].Center); //eoc roar
                                }
                            }
                            else
                            {
                                int darkStarThreshold = P2_ATTACK_SPACING * 3;
                                int laserThreshold = P2_ATTACK_SPACING * 2;

                                if (Main.netMode != NetmodeID.MultiplayerClient && AttackModeTimer > P2_ATTACK_SPACING - 120 && AttackModeTimer < P2_ATTACK_SPACING * 2 - 60)
                                {
                                    int interval = FargoSoulsWorld.MasochistModeReal ? 40 : 120;
                                    if (AttackModeTimer % interval == 12) //make a probe shoot
                                    {
                                        List<NPC> probes = Main.npc.Where(n => n.active && n.type == NPCID.Probe).ToList();

                                        int max = probes.Count;
                                        int attempt = Main.rand.Next(max);
                                        int probesActivated = 0;
                                        for (int i = 0; i < max; i++)
                                        {
                                            NPC probe = probes[attempt];
                                            if (!probe.GetGlobalNPC<Probe>().ShootLaser)
                                            {
                                                probe.GetGlobalNPC<Probe>().ShootLaser = true;
                                                probe.GetGlobalNPC<Probe>().AttackTimer = 0;

                                                if (++probesActivated >= 2)
                                                    break;

                                                attempt += Main.rand.Next(max);
                                            }

                                            attempt = (attempt + 1) % max; //start from a random point in the probe collection and try to make one shoot a laser, looping around at end of list
                                        }
                                    }
                                }

                                int maxDarkStarIntervals = 4;
                                if (npc.life < npc.lifeMax * 0.75)
                                    maxDarkStarIntervals = 5;
                                if (npc.life < npc.lifeMax * 0.5)
                                    maxDarkStarIntervals = 6;
                                if (npc.life < npc.lifeMax * 0.25)
                                    maxDarkStarIntervals = 7;

                                const int darkStarPause = 50;
                                int upperDarkStarTime = darkStarThreshold + maxDarkStarIntervals * darkStarPause;
                                if (AttackModeTimer == darkStarThreshold)
                                    SecondaryAttackTimer = 0;
                                if (AttackModeTimer >= darkStarThreshold && AttackModeTimer <= upperDarkStarTime + 90) //spaced star spread attack
                                {
                                    if (FargoSoulsWorld.MasochistModeReal)
                                    {
                                        num15 *= 0.75f;
                                        num16 *= 0.75f;
                                    }
                                    else
                                    {
                                        if (npc.Distance(target) < 600) //get away from player at high speed
                                        {
                                            target += npc.DirectionFrom(target) * 1000;

                                            num15 = 0.4f;
                                            num16 = 0.5f;
                                        }
                                        else
                                        {
                                            target += npc.DirectionTo(target).RotatedBy(MathHelper.PiOver2) * 1200;

                                            if (npc.Distance(target) < 1200)
                                            {
                                                maxSpeed *= 0.5f;
                                            }
                                            else //stop running
                                            {
                                                num15 *= 2f;
                                                num16 *= 2f;
                                            }
                                        }
                                    }

                                    if (AttackModeTimer < upperDarkStarTime && AttackModeTimer % darkStarPause == 0)
                                    {
                                        List<int> segments = new List<int>(npc.whoAmI);
                                        foreach (NPC n in Main.npc.Where(n => n.active && n.realLife == npc.whoAmI))
                                            segments.Add(n.whoAmI);

                                        NPC segment = Main.npc[Main.rand.Next(segments)]; //use a random segment for each star spray

                                        Vector2 targetPos = Main.player[npc.target].Center;
                                        targetPos += segment.DirectionFrom(targetPos) * Math.Min(300, segment.Distance(targetPos)); //slightly between player and npc

                                        float accelerationAngle = segment.DirectionTo(targetPos).ToRotation();

                                        double maxStarModifier = 0.5 + 0.5 * Math.Sin(MathHelper.Pi / (maxDarkStarIntervals - 1) * SecondaryAttackTimer++);
                                        int maxStarsInOneWave = (int)(maxStarModifier * (8.0 - 7.0 * npc.life / npc.lifeMax));
                                        if (maxStarsInOneWave > 6)
                                            maxStarsInOneWave = 6;
                                        //Main.NewText($"{Counter3} {maxStarModifier} {maxStarsInOneWave} {maxDarkStarIntervals}");
                                        for (int i = -maxStarsInOneWave; i <= maxStarsInOneWave; i++)
                                        {
                                            Vector2 offset = segment.DirectionTo(targetPos).RotatedBy(MathHelper.PiOver2);
                                            float offsetLength = 1000 / Math.Max(maxStarsInOneWave, 1) * i;
                                            int travelTime = 30 + Math.Abs(i) * 5;
                                            Vector2 vel = (targetPos + offset * offsetLength - segment.Center) / travelTime;
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                                Projectile.NewProjectile(npc.GetSource_FromThis(), segment.Center, vel * 2, ModContent.ProjectileType<DarkStarDestroyer>(), projDamage, 0f, Main.myPlayer, accelerationAngle, -travelTime);
                                        }
                                    }
                                }

                                if (AttackModeTimer == laserThreshold - 120) //tell for hyper dash for light show
                                {
                                    SecondaryAttackTimer = 0;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 9, npc.whoAmI);
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 9, npc.whoAmI);
                                    }
                                }
                                if (AttackModeTimer > laserThreshold && AttackModeTimer < laserThreshold + 420)
                                {
                                    flySpeedModifierRatio /= 2;

                                    if (SecondaryAttackTimer == 0) //fly at player
                                    {
                                        if (maxSpeed < 16)
                                            maxSpeed = 16;
                                        maxSpeed *= 1.5f;

                                        num15 *= 10;
                                        num16 *= 10;

                                        if (npc.Distance(target) < 400)
                                        {
                                            SecondaryAttackTimer = 1;
                                            npc.velocity = 20f * npc.DirectionTo(target);//.RotatedBy(MathHelper.ToRadians(30) * (Main.rand.NextBool() ? -1 : 1));

                                            if (!FargoSoulsWorld.MasochistModeReal) //deflect away at the last second
                                            {
                                                float targetSpeedDirection = MathHelper.WrapAngle(Main.player[npc.target].velocity.ToRotation() - npc.velocity.ToRotation());
                                                npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(30) * -Math.Sign(targetSpeedDirection));
                                            }

                                            npc.netUpdate = true;
                                            NetSync(npc);
                                        }
                                    }
                                    else
                                    {
                                        double angle = npc.DirectionTo(target).ToRotation() - npc.velocity.ToRotation();
                                        while (angle > Math.PI)
                                            angle -= 2.0 * Math.PI;
                                        while (angle < -Math.PI)
                                            angle += 2.0 * Math.PI;
                                        int rotationTowardsPlayer = Math.Sign(angle);

                                        bool playerIsInFront = Math.Abs(angle) < MathHelper.ToRadians(45);
                                        if (!playerIsInFront)
                                        {
                                            if (FargoSoulsWorld.MasochistModeReal)
                                                maxSpeed /= 2;
                                            else if (maxSpeed > 4)
                                                maxSpeed = 4;

                                            if (npc.velocity.Length() > maxSpeed)
                                                npc.velocity *= 0.986f;

                                            float turnModifier = 15f;
                                            num15 /= turnModifier; //garbage turning
                                            num16 /= turnModifier;
                                        }

                                        //curve very slightly towards player
                                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(0.3f) * rotationTowardsPlayer);

                                        if (AttackModeTimer < laserThreshold + 300 && ++SecondaryAttackTimer % 90 == 20)
                                        {
                                            bool flip = Main.rand.NextBool();
                                            bool spawn = true;
                                            foreach (NPC n in Main.npc.Where(n => n.active && n.realLife == npc.whoAmI))
                                            {
                                                spawn = !spawn;
                                                if (!spawn)
                                                    continue;

                                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                                {
                                                    if (Main.rand.NextFloat() > npc.life / npc.lifeMax)
                                                    {
                                                        float range = MathHelper.ToRadians(10);
                                                        float ai1 = n.rotation + (flip ? 0 : MathHelper.Pi) + Main.rand.NextFloat(-range, range);
                                                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), projDamage, 0f, Main.myPlayer, 11, n.whoAmI);
                                                        if (p != Main.maxProjectiles)
                                                        {
                                                            Main.projectile[p].localAI[1] = ai1;
                                                            if (Main.netMode == NetmodeID.Server)
                                                                NetMessage.SendData(MessageID.SyncProjectile, number: p);
                                                        }
                                                    }
                                                }

                                                flip = !flip;
                                                if (Main.rand.NextBool(5))
                                                    flip = !flip;
                                            }
                                        }
                                    }
                                }
                            }

                            if (++AttackModeTimer > P2_COIL_BEGIN_TIME) //change state
                            {
                                AttackModeTimer = 0;
                                PrepareToCoil = true;

                                npc.netUpdate = true;
                                NetSync(npc);
                            }
                            else if (AttackModeTimer == P2_COIL_BEGIN_TIME - 120) //telegraph with roar
                            {
                                SoundEngine.PlaySound(SoundID.Roar, Main.player[npc.target].Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 6, npc.whoAmI);
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 6, npc.whoAmI);
                                }
                            }
                        }
                        float num17 = target.X;
                        float num18 = target.Y;

                        float num21 = num17 - npc.Center.X;
                        float num22 = num18 - npc.Center.Y;
                        float num23 = (float)Math.Sqrt((double)num21 * (double)num21 + (double)num22 * (double)num22);

                        //ground movement code but it always runs
                        float num2 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                        float num3 = Math.Abs(num21);
                        float num4 = Math.Abs(num22);
                        float num5 = maxSpeed / num2;
                        float num6 = num21 * num5;
                        float num7 = num22 * num5;
                        if ((npc.velocity.X > 0f && num6 > 0f || npc.velocity.X < 0f && num6 < 0f) && (npc.velocity.Y > 0f && num7 > 0f || npc.velocity.Y < 0f && num7 < 0f))
                        {
                            if (npc.velocity.X < num6)
                                npc.velocity.X += num16;
                            else if (npc.velocity.X > num6)
                                npc.velocity.X -= num16;
                            if (npc.velocity.Y < num7)
                                npc.velocity.Y += num16;
                            else if (npc.velocity.Y > num7)
                                npc.velocity.Y -= num16;
                        }
                        if (npc.velocity.X > 0f && num6 > 0f || npc.velocity.X < 0f && num6 < 0f || npc.velocity.Y > 0f && num7 > 0f || npc.velocity.Y < 0f && num7 < 0f)
                        {
                            if (npc.velocity.X < num6)
                                npc.velocity.X += num15;
                            else if (npc.velocity.X > num6)
                                npc.velocity.X -= num15;
                            if (npc.velocity.Y < num7)
                                npc.velocity.Y += num15;
                            else if (npc.velocity.Y > num7)
                                npc.velocity.Y -= num15;

                            if (Math.Abs(num7) < maxSpeed * 0.2f && (npc.velocity.X > 0f && num6 < 0f || npc.velocity.X < 0f && num6 > 0f))
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += num15 * 2f;
                                else
                                    npc.velocity.Y -= num15 * 2f;
                            }
                            if (Math.Abs(num6) < maxSpeed * 0.2f && (npc.velocity.Y > 0f && num7 < 0f || npc.velocity.Y < 0f && num7 > 0f))
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += num15 * 2f;
                                else
                                    npc.velocity.X -= num15 * 2f;
                            }
                        }
                        else if (num3 > num4)
                        {
                            if (npc.velocity.X < num6)
                                npc.velocity.X += num15 * 1.1f;
                            else if (npc.velocity.X > num6)
                                npc.velocity.X -= num15 * 1.1f;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5f)
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += num15;
                                else
                                    npc.velocity.Y -= num15;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num7)
                                npc.velocity.Y += num15 * 1.1f;
                            else if (npc.velocity.Y > num7)
                                npc.velocity.Y -= num15 * 1.1f;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5f)
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += num15;
                                else
                                    npc.velocity.X -= num15;
                            }
                        }
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
                        npc.netUpdate = true;
                        npc.localAI[0] = 1f;

                        npc.position += npc.velocity * (.5f - flySpeedModifierRatio);
                    }

                    if (Main.netMode == NetmodeID.Server && npc.netUpdate && --npc.netSpam < 0) //manual mp sync control
                    {
                        npc.netUpdate = false;
                        npc.netSpam = 5;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    }
                    return false;
                }
                else
                {
                    npc.velocity.Y++;
                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;
                    return true;
                }
            }

            EModeUtils.DropSummon(npc, "MechWorm", NPC.downedMechBoss1, ref DroppedSummon, Main.hardMode);

            return true;
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            if (IsCoiling)
            {
                if (npc.life < npc.lifeMax / 10)
                {
                    float modifier = Math.Min(1f, AttackModeTimer / 480f);
                    damage = (int)(damage * modifier);
                }
                else
                {
                    damage = (int)(damage * 0.1);
                }
            }
            else if (PrepareToCoil || AttackModeTimer >= P2_COIL_BEGIN_TIME - 120 || npc.life < npc.lifeMax / 10)
            {
                damage = (int)(damage * 0.1);
            }
        }

        public override void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.SafeModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);

            if (projectile.numHits > 0 && !FargoSoulsUtil.IsSummonDamage(projectile))
                damage = (int)(damage * (2.0 / 3.0 + 1.0 / 3.0 * 1 / projectile.numHits));
            if (projectile.type == ProjectileID.RainFriendly)
                damage /= 2;
            if (projectile.type == ProjectileID.SoulDrain)
                damage = (int)(damage * 0.75);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Electrified, 60);
            target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GroundStick>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 25);
            LoadGore(recolor, 156);
        }
    }

    public class DestroyerSegment : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.TheDestroyerBody, NPCID.TheDestroyerTail);

        public int ProjectileCooldownTimer;
        public int AttackTimer;
        public int ProbeReleaseTimer;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(ProjectileCooldownTimer);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(ProbeReleaseTimer);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            ProjectileCooldownTimer = binaryReader.Read7BitEncodedInt();
            AttackTimer = binaryReader.Read7BitEncodedInt();
            ProbeReleaseTimer = binaryReader.Read7BitEncodedInt();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            ProbeReleaseTimer = -Main.rand.Next(360);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.Chilled] = false;
            npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = false;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            NPC destroyer = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.TheDestroyer);

            if (destroyer == null || npc.life <= 0 || !destroyer.active || destroyer.life <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.life = 0;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                    npc.active = false;
                }
                return result;
            }

            Destroyer destroyerEmode = destroyer.GetGlobalNPC<Destroyer>();

            npc.defense = npc.defDefense;
            npc.localAI[0] = 0f; //disable vanilla lasers
            npc.buffImmune[ModContent.BuffType<TimeFrozen>()] = destroyer.buffImmune[ModContent.BuffType<TimeFrozen>()];

            if (destroyerEmode.IsCoiling) //spinning
            {
                npc.defense = 0;

                ProjectileCooldownTimer = 180;
                if (AttackTimer < 0)
                    AttackTimer = 0; //cancel startup on any imminent projectiles
                Vector2 pivot = Main.npc[npc.realLife].Center;
                pivot += Vector2.Normalize(Main.npc[npc.realLife].velocity.RotatedBy(MathHelper.PiOver2 * destroyerEmode.RotationDirection)) * 600;
                if (npc.Distance(pivot) < 600) //make sure body doesnt coil into the circling zone
                    npc.Center = pivot + npc.DirectionFrom(pivot) * 600;
            }

            if (destroyerEmode.InPhase2)
                AttackTimer = 0; //just shut it off, fuck it

            if (ProjectileCooldownTimer > 0) //no lasers or stars while or shortly after spinning
            {
                ProjectileCooldownTimer--;
                if (AttackTimer > 1000)
                    AttackTimer = 1000;
            }

            if (npc.ai[2] == 0) //shoot lasers
            {
                if (++ProbeReleaseTimer > 60)
                {
                    ProbeReleaseTimer = -Main.rand.Next(360);
                    float lifeThreshold = FargoSoulsWorld.MasochistModeReal ? 0.8f : 0.4f;
                    if (Main.npc[npc.realLife].life < Main.npc[npc.realLife].lifeMax * lifeThreshold && NPC.CountNPCS(NPCID.Probe) < 10 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Main.rand.NextBool(FargoSoulsWorld.MasochistModeReal ? 5 : 10)) //higher chance in maso
                        {
                            npc.ai[2] = 1;
                            npc.HitEffect();
                            npc.netUpdate = true;

                            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.Probe);
                        }
                    }
                }

                AttackTimer += Main.rand.Next(6);
                if (AttackTimer > Main.rand.Next(1200, 22000)) //replacement for vanilla lasers
                {
                    AttackTimer = 0;
                    npc.TargetClosest();
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speed = 2f + npc.Distance(Main.player[npc.target].Center) / 360;
                        if (speed > 16f)
                            speed = 16f;
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed * npc.DirectionTo(Main.player[npc.target].Center), ProjectileID.DeathLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = Math.Min((int)(npc.Distance(Main.player[npc.target].Center) / speed) + 180, 600);
                    }
                    npc.netUpdate = true;
                }
            }
            else //light is out, shoot dark star
            {
                int cap = Main.npc[npc.realLife].lifeMax / Main.npc[npc.realLife].life;
                if (cap > 20) //prevent meme scaling at super low life
                    cap = 20;
                AttackTimer += Main.rand.Next(2 + cap) + 1;
                if (AttackTimer >= Main.rand.Next(3600, 36000))
                {
                    AttackTimer = 0;
                    npc.TargetClosest();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 distance = Main.player[npc.target].Center - npc.Center;

                        float modifier = 28f * (1f - (float)Main.npc[npc.realLife].life / Main.npc[npc.realLife].lifeMax);
                        if (modifier < 12)
                            modifier = 12;

                        int delay = (int)(distance.Length() / modifier) / 2;
                        if (delay < 0)
                            delay = 0;

                        int type = ModContent.ProjectileType<DarkStarHoming>();
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(distance) * modifier, type, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.target, -delay);
                    }
                }
            }

            if (npc.buffType[0] != 0 && npc.buffType[0] != BuffID.Chilled && npc.buffType[0] != ModContent.BuffType<TimeFrozen>())
                npc.DelBuff(0);

            return result;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            NPC destroyer = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.TheDestroyer);

            if (destroyer == null)
                return base.CanHitPlayer(npc, target, ref CooldownSlot);

            Destroyer destroyerEmode = destroyer.GetGlobalNPC<Destroyer>(); //basically, don't hit player right around when a coil begins, segments inside radius may move eratically
            if (destroyerEmode.IsCoiling)
            {
                if (destroyerEmode.AttackModeTimer < 15)
                    return false;
            }
            else if (destroyerEmode.PrepareToCoil)
            {
                return false;
            }
            
            return true;
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            NPC destroyer = FargoSoulsUtil.NPCExists(npc.realLife, NPCID.TheDestroyer);

            if (destroyer == null)
                return;

            Destroyer destroyerEmode = destroyer.GetGlobalNPC<Destroyer>();

            if (destroyerEmode.IsCoiling)
            {
                if (npc.life < npc.lifeMax / 10)
                {
                    float modifier = Math.Min(1f, destroyerEmode.AttackModeTimer / 480f);
                    damage = (int)(damage * modifier);
                }
                else
                {
                    damage = (int)(damage * 0.1);
                }
            }
            else if (destroyerEmode.PrepareToCoil || destroyerEmode.AttackModeTimer >= Destroyer.P2_COIL_BEGIN_TIME - 120 || destroyer.life < destroyer.lifeMax / 10)
            {
                damage = (int)(damage * 0.1);
            }
        }

        public override void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.SafeModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);

            if (projectile.numHits > 0 && !FargoSoulsUtil.IsSummonDamage(projectile))
                damage = (int)(damage * (2.0 / 3.0 + 1.0 / 3.0 * 1 / projectile.numHits));
            if (projectile.type == ProjectileID.RainFriendly)
                damage /= 2;
            if (projectile.type == ProjectileID.SoulDrain)
                damage = (int)(damage * 0.75);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Electrified, 60);
            target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Probe : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Probe);

        public int OrbitChangeTimer;
        public int OrbitDirection;
        public int AttackTimer;

        public float TargetOrbitRotation;

        public bool ShootLaser;



        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(OrbitChangeTimer);
            binaryWriter.Write7BitEncodedInt(OrbitDirection);
            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write(TargetOrbitRotation);
            bitWriter.WriteBit(ShootLaser);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            OrbitChangeTimer = binaryReader.Read7BitEncodedInt();
            OrbitDirection = binaryReader.Read7BitEncodedInt();
            AttackTimer = binaryReader.Read7BitEncodedInt();
            TargetOrbitRotation = binaryReader.ReadSingle();
            ShootLaser = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
                npc.lifeMax = (int)(npc.lifeMax * 1.5);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return FargoSoulsWorld.SwarmActive
                || !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer)
                || FargoSoulsWorld.MasochistModeReal;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Suffocation] = true;

            if (Main.rand.NextBool(4) && !FargoSoulsUtil.AnyBossAlive())
                EModeGlobalNPC.Horde(npc, 8);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive || !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
                return result;

            //bool isCoiling = Main.npc[EModeGlobalNPC.destroyBoss].GetGlobalNPC<Destroyer>().IsCoiling;

            if (FargoSoulsWorld.MasochistModeReal)
            {
                //if (isCoiling && npc.localAI[0] > 30) //disable vanilla lasers during coil
                if (npc.localAI[0] > 30)
                    npc.localAI[0] -= 30;

                if (!ShootLaser)
                    return result;
            }

            if (npc.localAI[0] > 30)
                npc.localAI[0] = -30;

            if (++OrbitChangeTimer > 120) //choose a direction to orbit in
            {
                OrbitChangeTimer = 0;
                OrbitDirection = Main.rand.NextBool() ? 1 : -1;

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (Main.npc[EModeGlobalNPC.destroyBoss].GetGlobalNPC<Destroyer>().IsCoiling)
                ShootLaser = false;

            if (npc.HasValidTarget)
            {
                if (ShootLaser)
                {
                    if (AttackTimer == 0)
                    {
                        TargetOrbitRotation = Main.player[npc.target].DirectionTo(npc.Center).ToRotation(); //when shooting laser, stop orbiting

                        npc.netUpdate = true;
                        NetSync(npc);
                    }

                    const int attackTime = 110;

                    Vector2 towardsPlayer = 6f * npc.DirectionTo(Main.player[npc.target].Center);

                    float dustScale = 0.5f + 2.5f * AttackTimer / attackTime;
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 90, 2f * towardsPlayer.X, 2f * towardsPlayer.Y, 0, default(Color), dustScale);
                    Main.dust[d].noGravity = true;

                    if (++AttackTimer > attackTime)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, towardsPlayer, ProjectileID.DeathLaser, (int)(1.1 * FargoSoulsUtil.ScaledProjectileDamage(npc.damage)), 0f, Main.myPlayer);

                        AttackTimer = 0;
                        ShootLaser = false;

                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }

                float orbitDistance = ShootLaser ? 440 : 220; //LAUGH
                Vector2 vel = Main.player[npc.target].Center - npc.Center;
                vel += orbitDistance * (ShootLaser ? Vector2.UnitX.RotatedBy(TargetOrbitRotation) : Main.player[npc.target].DirectionTo(npc.Center).RotatedBy(MathHelper.ToRadians(22) * OrbitDirection));

                npc.position += (Main.player[npc.target].position - Main.player[npc.target].oldPosition) / 3;

                if (npc.Distance(Main.player[npc.target].Center) < 150) //snap away really fast if too close
                {
                    npc.velocity = vel / 20;
                }
                else //regular movement
                {
                    vel.Normalize();
                    vel *= 12f;

                    float moveSpeed = 0.7f;

                    if (ShootLaser)
                    {
                        vel *= 1.5f;
                        moveSpeed *= 1.5f;
                    }

                    if (npc.velocity.X < vel.X)
                    {
                        npc.velocity.X += moveSpeed;
                        if (npc.velocity.X < 0 && vel.X > 0)
                            npc.velocity.X += moveSpeed;
                    }
                    else if (npc.velocity.X > vel.X)
                    {
                        npc.velocity.X -= moveSpeed;
                        if (npc.velocity.X > 0 && vel.X < 0)
                            npc.velocity.X -= moveSpeed;
                    }
                    if (npc.velocity.Y < vel.Y)
                    {
                        npc.velocity.Y += moveSpeed;
                        if (npc.velocity.Y < 0 && vel.Y > 0)
                            npc.velocity.Y += moveSpeed;
                    }
                    else if (npc.velocity.Y > vel.Y)
                    {
                        npc.velocity.Y -= moveSpeed;
                        if (npc.velocity.Y > 0 && vel.Y < 0)
                            npc.velocity.Y -= moveSpeed;
                    }
                }
            }

            return result;
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
