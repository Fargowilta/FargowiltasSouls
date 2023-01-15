using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
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


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AITimer);
            binaryWriter.Write7BitEncodedInt(ScytheSpawnTimer);
            binaryWriter.Write7BitEncodedInt(FinalPhaseDashCD);
            binaryWriter.Write7BitEncodedInt(FinalPhaseDashStageDuration);
            binaryWriter.Write7BitEncodedInt(FinalPhaseAttackCounter);
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
            IsInFinalPhase = bitReader.ReadBit();
            FinalPhaseBerserkDashesComplete = bitReader.ReadBit();
            FinalPhaseDashHorizSpeedSet = bitReader.ReadBit();
        }

        public override bool SafePreAI(NPC npc)
        {
            EModeGlobalNPC.eyeBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return true;

            void SpawnServants()
            {
                if (npc.life <= npc.lifeMax * 0.65 && NPC.CountNPCS(NPCID.ServantofCthulhu) < 9 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = new Vector2(3, 3);
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
                if (ScytheSpawnTimer % (IsInFinalPhase ? 2 : 6) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (IsInFinalPhase && !FargoSoulsWorld.MasochistModeReal)
                    {
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f, Main.myPlayer);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 75;
                    }
                    else
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f, Main.myPlayer);
                    }
                }
                ScytheSpawnTimer--;
            }

            if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] == 0f)
            {
                ScytheSpawnTimer = 30;
            }

            if (npc.ai[1] == 3f && !IsInFinalPhase) //during dashes in phase 2
            {
                if (FargoSoulsWorld.MasochistModeReal)
                {
                    ScytheSpawnTimer = 30;
                    SpawnServants();
                }

                if (!ScytheRingIsOnCD)
                {
                    ScytheRingIsOnCD = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
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

                    if (npc.HasValidTarget && !Main.dayTime)
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

                        float targetRotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);
                    }

                    if (++AITimer == 1) //teleport to random position
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = Main.player[npc.target].Center;
                            npc.position.X += Main.rand.NextBool() ? -600 : 600;
                            npc.position.Y += Main.rand.NextBool() ? -400 : 400;
                            npc.TargetClosest(false);
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                    }
                    else if (AITimer < 90) //fade in, moving into position
                    {
                        npc.alpha -= FargoSoulsWorld.MasochistModeReal ? 5 : 4;
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                            if (FargoSoulsWorld.MasochistModeReal && AITimer < 90)
                                AITimer = 90;
                        }

                        const float PI = (float)Math.PI;
                        if (npc.rotation > PI)
                            npc.rotation -= 2 * PI;
                        if (npc.rotation < -PI)
                            npc.rotation += 2 * PI;

                        float targetRotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2;
                        if (targetRotation > PI)
                            targetRotation -= 2 * PI;
                        if (targetRotation < -PI)
                            targetRotation += 2 * PI;
                        npc.rotation = MathHelper.Lerp(npc.rotation, targetRotation, 0.07f);

                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }

                        Vector2 target = Main.player[npc.target].Center;
                        target.X += npc.Center.X < target.X ? -600 : 600;
                        target.Y += npc.Center.Y < target.Y ? -400 : 400;

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
                    }
                    else if (!FinalPhaseBerserkDashesComplete) //berserk dashing phase
                    {
                        AITimer = 90;

                        const float xSpeed = 18f;
                        const float ySpeed = 40f;

                        if (++FinalPhaseDashCD == 1)
                        {
                            SoundEngine.PlaySound(SoundID.ForceRoarPitched, Main.player[npc.target].Center);

                            if (!FinalPhaseDashHorizSpeedSet) //only set this on the first dash of each set
                            {
                                FinalPhaseDashHorizSpeedSet = true;
                                npc.velocity.X = npc.Center.X < Main.player[npc.target].Center.X ? xSpeed : -xSpeed;
                            }

                            npc.velocity.Y = npc.Center.Y < Main.player[npc.target].Center.Y ? ySpeed : -ySpeed; //alternate this every dash

                            ScytheSpawnTimer = 30;
                            //if (FargoSoulsWorld.MasochistModeReal)
                            //    SpawnServants();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ModContent.ProjectileType<BloodScythe>(), 1f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);

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
                            if (!FargoSoulsWorld.MasochistModeReal)
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
                                npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * npc.velocity.Length() * 0.75f;

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
                            npc.alpha += FargoSoulsWorld.MasochistModeReal ? 16 : 4;
                            if (npc.alpha > 255)
                            {
                                npc.alpha = 255;
                                if (FargoSoulsWorld.MasochistModeReal && AITimer < threshold)
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
                        float targetRotation = MathHelper.WrapAngle(npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - PI / 2);
                        npc.rotation = MathHelper.WrapAngle(MathHelper.Lerp(npc.rotation, targetRotation, 0.07f));

                        if (npc.alpha > 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
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
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                        IsInFinalPhase = true;

                        SoundEngine.PlaySound(SoundID.Roar, npc.HasValidTarget ? Main.player[npc.target].Center : npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                    }
                    return false;
                }
                else if (npc.ai[0] == 3 && (npc.ai[1] == 0 || npc.ai[1] == 5))
                {
                    if (npc.ai[2] < 2)
                    {
                        npc.ai[2]--;
                        npc.alpha += 4;
                        for (int i = 0; i < 3; i++)
                        {
                            int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].noLight = true;
                            Main.dust[d].velocity *= 4f;
                        }
                        if (npc.alpha > 255)
                        {
                            npc.alpha = 255;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                            {
                                npc.ai[2] = 60;
                                npc.ai[1] = 5f;

                                Vector2 distance = Main.player[npc.target].Center - npc.Center;
                                if (Math.Abs(distance.X) > 1200)
                                    distance.X = 1200 * Math.Sign(distance.X);
                                else if (Math.Abs(distance.X) < 600)
                                    distance.X = 600 * Math.Sign(distance.X);
                                if (distance.Y > 0) //always ensure eoc teleports above player
                                    distance.Y *= -1;
                                if (Math.Abs(distance.Y) > 450)
                                    distance.Y = 450 * Math.Sign(distance.Y);
                                if (Math.Abs(distance.Y) < 150)
                                    distance.Y = 150 * Math.Sign(distance.Y);
                                npc.Center = Main.player[npc.target].Center + distance;

                                npc.netUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        npc.alpha -= 4;
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                        }
                        else
                        {
                            npc.ai[2]--;
                            npc.position -= npc.velocity / 2;
                            for (int i = 0; i < 3; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default(Color), 1.5f);
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
                        if (Main.netMode != NetmodeID.MultiplayerClient)
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
                                Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, speed, ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1f, Main.myPlayer);
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

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<AgitatingLens>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrate, 5));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.FallenStar, 5));
            npcLoot.Add(emodeRule);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (FargoSoulsWorld.MasochistModeReal)
            {
                target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                target.AddBuff(BuffID.Bleeding, 600);
                target.AddBuff(BuffID.Obstructed, 15);
            }

            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 120);
            target.AddBuff(ModContent.BuffType<Berserked>(), 300);
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
}
