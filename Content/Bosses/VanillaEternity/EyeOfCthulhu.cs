using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
    public class EyeofCthulhu : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.EyeofCthulhu);

        public int AITimer;
        public int ScytheSpawnTimer;
        public int FinalPhaseDashCD;
        public int FinalPhaseDashStageDuration;
        public int FinalPhaseAttackCounter;

        public bool IsInFinalPhase;
        public bool FinalPhaseBerserkDashesComplete;
        public bool FinalPhaseDashHorizSpeedSet;

        public bool DroppedSummon;
        public bool ScytheRingIsOnCD;

        public int TeleportDirection = 0;

        Vector2 targetCenter = Vector2.Zero;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AITimer);
            binaryWriter.Write7BitEncodedInt(ScytheSpawnTimer);
            binaryWriter.Write7BitEncodedInt(FinalPhaseDashCD);
            binaryWriter.Write7BitEncodedInt(FinalPhaseDashStageDuration);
            binaryWriter.Write7BitEncodedInt(FinalPhaseAttackCounter);
            binaryWriter.Write7BitEncodedInt(TeleportDirection);

            bitWriter.WriteBit(IsInFinalPhase);
            bitWriter.WriteBit(FinalPhaseBerserkDashesComplete);
            bitWriter.WriteBit(FinalPhaseDashHorizSpeedSet);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AITimer = binaryReader.Read7BitEncodedInt();
            ScytheSpawnTimer = binaryReader.Read7BitEncodedInt();
            FinalPhaseDashCD = binaryReader.Read7BitEncodedInt();
            FinalPhaseDashStageDuration = binaryReader.Read7BitEncodedInt();
            FinalPhaseAttackCounter = binaryReader.Read7BitEncodedInt();
            TeleportDirection = binaryReader.Read7BitEncodedInt();

            IsInFinalPhase = bitReader.ReadBit();
            FinalPhaseBerserkDashesComplete = bitReader.ReadBit();
            FinalPhaseDashHorizSpeedSet = bitReader.ReadBit();


        }

        public override bool SafePreAI(NPC npc)
        {
            ref float ai_Phase = ref npc.ai[0];
            ref float ai_AttackState = ref npc.ai[1];
            ref float ai_Timer = ref npc.ai[2];
            EModeGlobalNPC.eyeBoss = npc.whoAmI;

            if (WorldSavingSystem.SwarmActive)
                return true;

            void SpawnServants()
            {
                if (npc.life <= npc.lifeMax * 0.65 && NPC.CountNPCS(NPCID.ServantofCthulhu) < 9 && FargoSoulsUtil.HostCheck)
                {
                    Vector2 vel = new(3, 3);
                    for (int i = 0; i < 4; i++)
                    {
                        int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.ServantofCthulhu);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].velocity = vel.RotatedBy(Math.PI / 2 * i);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }
            }

            npc.dontTakeDamage = npc.alpha > 50;
            if (npc.dontTakeDamage)
                Lighting.AddLight(npc.Center, 0.75f, 1.35f, 1.5f);

            if (ScytheSpawnTimer > 0)
            {
                if (ScytheSpawnTimer % (IsInFinalPhase ? 2 : 6) == 0 && FargoSoulsUtil.HostCheck)
                {
                    if (IsInFinalPhase && !WorldSavingSystem.MasochistModeReal)
                    {
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 1f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 75;
                    }
                    else
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 1f, Main.myPlayer);
                    }
                }
                ScytheSpawnTimer--;
            }

            if (ai_Phase == 0f) //p1
            {
                //Faster speed, even faster when far
                float modifier = 0.15f;
                if (npc.HasValidTarget)
                    modifier = MathHelper.Lerp(0.15f, 0.5f, Math.Clamp(npc.Distance(Main.player[npc.target].Center) / 1000f, 0, 1));
                npc.position += npc.velocity * modifier;

                //Faster consecutive dashes
                if (ai_AttackState == 2f)
                    npc.position += npc.ai[3] * 0.3f * npc.velocity;
            }
            if ((ai_Phase == 0f || ai_Phase == 3f) && ai_AttackState == 2f && !IsInFinalPhase) // Faster consecutive dashes in p1 and p2
            {
                float modifier = ai_Phase == 0 ? 0.25f : 0.5f; // more increase in p2
                npc.position += npc.ai[3] * modifier * npc.velocity;
            }


            if (ai_Phase == 0f && ai_AttackState == 2f && npc.HasValidTarget && WorldSavingSystem.MasochistModeReal) // Dashes curve in phase 1 done
            {
                float speed = npc.velocity.Length();
                float modifier = 0.25f;
                npc.velocity += npc.SafeDirectionTo(Main.player[npc.target].Center) * modifier;
                npc.velocity = Vector2.Normalize(npc.velocity) * speed;
            }

            if (ai_Phase == 0f && ai_AttackState == 2f && ai_Timer == 0f)
            {
                ScytheSpawnTimer = 30;

            }

            if (ai_AttackState == 3f && !IsInFinalPhase) //during dashes in phase 2
            {
                if (WorldSavingSystem.MasochistModeReal)
                {
                    ScytheSpawnTimer = 30;
                    SpawnServants();
                }

                if (!ScytheRingIsOnCD)
                {
                    ScytheRingIsOnCD = true;
                    if (FargoSoulsUtil.HostCheck)
                        FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                }
            }
            else
            {
                ScytheRingIsOnCD = false; //hacky fix for scythe spam during p2 transition
            }

            if (npc.life < npc.lifeMax / 2)
            {
                if (IsInFinalPhase) //final phase
                {
                    const float speedModifier = 0.3f;

                    if (npc.HasValidTarget && (!Main.dayTime || Main.zenithWorld || Main.remixWorld))
                    {
                        if (npc.timeLeft < 300)
                            npc.timeLeft = 300;
                    }
                    else //despawn and retarget
                    {
                        npc.TargetClosest(false);
                        npc.velocity.X *= 0.98f;
                        npc.velocity.Y -= npc.velocity.Y > 0 ? 1f : 0.25f;

                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;

                        AITimer = 90;
                        FinalPhaseDashCD = 0;
                        FinalPhaseBerserkDashesComplete = true;
                        FinalPhaseDashHorizSpeedSet = false;
                        FinalPhaseAttackCounter = 0;

                        npc.alpha = 0;

                        const float PI = (float)Math.PI;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;

                        float targetRotation = npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);
                    }

                    if (++AITimer == 1) //teleport to random position
                    {
                        if (FargoSoulsUtil.HostCheck)
                        {
                            npc.Center = Main.player[npc.target].Center;
                            npc.position.X += Main.rand.NextBool() ? -600 : 600;
                            npc.position.Y += Main.rand.NextBool() ? -400 : 400;

                            if (WorldSavingSystem.MasochistModeReal)
                                npc.position.X += Main.rand.Next(-100, 100); //1.6.1 change: random offset

                            npc.TargetClosest(false);
                            npc.netUpdate = true;
                            NetSync(npc);

                            AITimer = 40; //1.6.1 change: skip most of windup

                            if (npc.HasValidTarget) //1.6.1 change: telegraph with spectral EoC clone
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SpectralEoC>(), 0, 0, Main.myPlayer, AITimer + 20, npc.target);
                        }

                        if (npc.HasValidTarget)
                            targetCenter = Main.player[npc.target].Center;
                        else
                            targetCenter = npc.Center;
                    }
                    else if (AITimer < 90) //fade in
                    {

                        npc.alpha -= WorldSavingSystem.MasochistModeReal ? 30 : 25;
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                        }

                        const float PI = (float)Math.PI;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;

                        float targetRotation = npc.SafeDirectionTo(targetCenter).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.3f);

                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }

                        Vector2 target = targetCenter;
                        target.X += npc.Center.X < target.X ? -600 : 600;
                        target.Y += npc.Center.Y < target.Y ? -400 : 400;

                        /*
                        if (npc.Center.X < target.X)
                        {
                            npc.velocity.X += speedModifier;
                            if (npc.velocity.X < 0)
                                npc.velocity.X += speedModifier * 2;
                        }
                        else
                        {
                            npc.velocity.X -= speedModifier;
                            if (npc.velocity.X > 0)
                                npc.velocity.X -= speedModifier * 2;
                        }
                        if (npc.Center.Y < target.Y)
                        {
                            npc.velocity.Y += speedModifier;
                            if (npc.velocity.Y < 0)
                                npc.velocity.Y += speedModifier * 2;
                        }
                        else
                        {
                            npc.velocity.Y -= speedModifier;
                            if (npc.velocity.Y > 0)
                                npc.velocity.Y -= speedModifier * 2;
                        }
                        if (Math.Abs(npc.velocity.X) > 24)
                            npc.velocity.X = 24 * Math.Sign(npc.velocity.X);
                        if (Math.Abs(npc.velocity.Y) > 24)
                            npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);
                        */
                        npc.velocity = Vector2.Zero;
                    }
                    else if (!FinalPhaseBerserkDashesComplete) //berserk dashing phase
                    {
                        AITimer = 90;

                        const float xSpeed = 18f;
                        const float ySpeed = 40f;

                        if (++FinalPhaseDashCD == 1)
                        {
                            SoundEngine.PlaySound(SoundID.ForceRoarPitched, targetCenter);

                            if (!FinalPhaseDashHorizSpeedSet) //only set this on the first dash of each set
                            {
                                FinalPhaseDashHorizSpeedSet = true;
                                npc.velocity.X = npc.Center.X < targetCenter.X ? xSpeed : -xSpeed;
                            }

                            npc.velocity.Y = npc.Center.Y < targetCenter.Y ? ySpeed : -ySpeed; //alternate this every dash

                            ScytheSpawnTimer = 30;
                            //if (WorldSavingSystem.MasochistModeReal)
                            //    SpawnServants();
                            if (FargoSoulsUtil.HostCheck)
                                FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ModContent.ProjectileType<BloodScythe>(), 1f, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);

                            npc.netUpdate = true;
                        }
                        else if (FinalPhaseDashCD > 20)
                        {
                            FinalPhaseDashCD = 0;
                        }


                        if (++FinalPhaseDashStageDuration > 600 * 3 / xSpeed + 5) //proceed
                        {
                            ScytheSpawnTimer = 0;
                            FinalPhaseDashStageDuration = 0;
                            FinalPhaseBerserkDashesComplete = true;
                            if (!WorldSavingSystem.MasochistModeReal)
                                FinalPhaseAttackCounter++;
                            npc.velocity *= 0.75f;
                            npc.netUpdate = true;
                        }

                        const float PI = (float)Math.PI;
                        npc.rotation = npc.velocity.ToRotation() - PI / 2;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;
                    }
                    else
                    {
                        bool mustRest = FinalPhaseAttackCounter >= 3;

                        const int restingTime = 240;

                        int threshold = 180;
                        if (mustRest)
                            threshold += restingTime;

                        if (mustRest && AITimer < restingTime + 90)
                        {
                            if (AITimer == 91)
                                npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * npc.velocity.Length() * 0.75f;

                            npc.velocity.X *= 0.98f;
                            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 300)
                                npc.velocity.X *= 0.9f;

                            bool floatUp = Collision.SolidCollision(npc.position, npc.width, npc.height);
                            if (!floatUp && npc.Bottom.X > 0 && npc.Bottom.X < Main.maxTilesX * 16 && npc.Bottom.Y > 0 && npc.Bottom.Y < Main.maxTilesY * 16)
                            {
                                Tile tile = Framing.GetTileSafely(npc.Bottom);
                                if (tile != null && tile.HasUnactuatedTile)
                                    floatUp = Main.tileSolid[tile.TileType];
                            }

                            if (floatUp)
                            {
                                npc.velocity.X *= 0.95f;

                                npc.velocity.Y -= speedModifier;
                                if (npc.velocity.Y > 0)
                                    npc.velocity.Y = 0;
                                if (Math.Abs(npc.velocity.Y) > 24)
                                    npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);
                            }
                            else
                            {
                                npc.velocity.Y += speedModifier;
                                if (npc.velocity.Y < 0)
                                    npc.velocity.Y += speedModifier * 2;
                                if (npc.velocity.Y > 15)
                                    npc.velocity.Y = 15;
                            }
                        }
                        else
                        {
                            npc.alpha += WorldSavingSystem.MasochistModeReal ? 16 : 4;
                            if (npc.alpha > 255)
                            {
                                npc.alpha = 255;
                                if (WorldSavingSystem.MasochistModeReal && AITimer < threshold)
                                    AITimer = threshold;
                            }

                            if (mustRest)
                            {
                                npc.velocity.Y -= speedModifier * 0.5f;
                                if (npc.velocity.Y > 0)
                                    npc.velocity.Y = 0;
                                if (Math.Abs(npc.velocity.Y) > 24)
                                    npc.velocity.Y = 24 * Math.Sign(npc.velocity.Y);
                            }
                            else
                            {
                                npc.velocity *= 0.98f;
                            }
                        }

                        const float PI = (float)Math.PI;
                        float targetRotation = MathHelper.WrapAngle(npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2);
                        npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotation, 0.07f));

                        if (npc.alpha > 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].noLight = true;
                                Main.dust[d].velocity *= 4f;
                            }
                        }

                        if (AITimer > threshold) //reset
                        {
                            AITimer = 0;
                            FinalPhaseDashCD = 0;
                            FinalPhaseBerserkDashesComplete = false;
                            FinalPhaseDashHorizSpeedSet = false;
                            if (mustRest)
                                FinalPhaseAttackCounter = 0;
                            npc.velocity = Vector2.Zero;
                            npc.netUpdate = true;
                        }
                    }

                    if (npc.netUpdate)
                    {
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                            NetSync(npc);
                        }
                        npc.netUpdate = false;
                    }
                    return false;
                }
                else if (!IsInFinalPhase && npc.life <= npc.lifeMax * 0.1) //go into final phase
                {
                    npc.velocity *= 0.98f;
                    npc.alpha += 4;
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                        IsInFinalPhase = true;

                        SoundEngine.PlaySound(SoundID.Roar, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center);

                        if (FargoSoulsUtil.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                    }
                    return false;
                }
                else if (ai_Phase == 3 && (ai_AttackState == 0 || ai_AttackState == 5))
                {
                    if (ai_Timer < 2)
                    {
                        ai_Timer--;
                        npc.alpha += 4;
                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }
                        if (npc.alpha > 255)
                        {
                            npc.alpha = 255;
                            if (FargoSoulsUtil.HostCheck && npc.HasPlayerTarget)
                            {
                                ai_Timer = 60;
                                ai_AttackState = 5f;

                                Vector2 distance = Main.player[npc.target].Center - npc.Center;
                                if (distance.X == 0) //never zero side
                                    distance.X = 1;
                                const int Xmax = 1200; //1.6.1 note: was 1200 before
                                const int Xmin = 1100; //1.6.1 note: was 600 before
                                if (Math.Abs(distance.X) > Xmax)
                                    distance.X = Xmax * Math.Sign(distance.X);
                                else if (Math.Abs(distance.X) < Xmin)
                                    distance.X = Xmin * Math.Sign(distance.X);

                                if (TeleportDirection == 0)
                                    TeleportDirection = Math.Sign(distance.X); //first dash picks side towards player
                                else
                                    TeleportDirection *= -1; //switch side

                                distance.X = Math.Abs(distance.X) * TeleportDirection;

                                if (distance.Y > 0) //ensure to teleport above
                                    distance.Y *= -1;

                                const int Ymax = 300; // 1.6.1 note: was 450 before
                                const int Ymin = 150; // 1.6.1 note: was 150 before
                                if (Math.Abs(distance.Y) > Ymax)
                                    distance.Y = Ymax * Math.Sign(distance.Y);
                                if (Math.Abs(distance.Y) < Ymin)
                                    distance.Y = Ymin * Math.Sign(distance.Y);

                                distance.X += Main.rand.NextFloat(-50, 50);
                                distance.Y += Main.rand.NextFloat(-200, 200); //randomness otherwise pattern basically becomes static

                                npc.Center = Main.player[npc.target].Center + distance;

                                npc.netUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        const int aDif = 2;
                        npc.alpha -= aDif;
                        const int delay = 30;
                        if (Math.Abs(npc.alpha - (245 - delay)) <= aDif)
                        {
                            SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                        }
                        if (npc.alpha < 245 - delay && npc.alpha > 212 - delay) //latter value calibrates dash distance, basically
                        {
                            if (npc.HasValidTarget)
                                npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * 50;


                        }
                        if (npc.alpha < 245 - delay && npc.alpha > 120 - delay) //scythes
                        {
                            if (npc.alpha % (aDif * 10) <= aDif && FargoSoulsUtil.HostCheck)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 1f, Main.myPlayer);
                            }
                        }
                        if (npc.alpha < 245 - delay && npc.alpha > 30) //curve towards player
                        {
                            float speed = npc.velocity.Length();
                            float modifier = 1f;
                            npc.velocity += npc.SafeDirectionTo(Main.player[npc.target].Center) * modifier;
                            npc.velocity = Vector2.Normalize(npc.velocity) * speed;


                        }
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                        }
                        else
                        {
                            ai_Timer--;
                            npc.position -= npc.velocity / 2;
                            for (int i = 0; i < 3; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].noLight = true;
                                Main.dust[d].velocity *= 4f;
                            }
                        }
                    }
                }

                /*if (++Timer > 600)
                {
                    Timer = 0;
                    if (npc.HasValidTarget)
                    {
                        Player player = Main.player[npc.target];
                        SoundEngine.PlaySound(SoundID.Item9104, player.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Vector2 spawnPos = player.Center;
                            int direction;
                            if (player.velocity.X == 0f)
                                direction = player.direction;
                            else
                                direction = Math.Sign(player.velocity.X);
                            spawnPos.X += 600 * direction;
                            spawnPos.Y -= 600;
                            Vector2 speed = Vector2.UnitY;
                            for (int i = 0; i < 30; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, speed, ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 1f, Main.myPlayer);
                                spawnPos.X += 72 * direction;
                                speed.Y += 0.15f;
                            }
                        }
                    }
                }*/
            }
            else
            {
                npc.alpha = 0;
                npc.dontTakeDamage = false;
            }

            // Drop summon
            EModeUtils.DropSummon(npc, "SuspiciousEye", NPC.downedBoss1, ref DroppedSummon);

            return true;
        }


        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);
                target.AddBuff(BuffID.Bleeding, 600);
                target.AddBuff(BuffID.Obstructed, 15);
            }

            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 120);
            target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 300);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 0);
            LoadBossHeadSprite(recolor, 1);
            LoadGoreRange(recolor, 6, 10);
        }
    }
    /*
    public class Servants : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ServantofCthulhu);

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            npc.life = npc.lifeMax = 6;
        }
    }
    */
}
