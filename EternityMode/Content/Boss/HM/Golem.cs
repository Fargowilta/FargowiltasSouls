using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Deathrays;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.HM
{
    public abstract class GolemPart : EModeNPCBehaviour
    {
        public int HealPerSecond;
        public int HealCounter;

        protected GolemPart(int heal)
        {
            HealPerSecond = heal;
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;

            npc.damage = (int)Math.Round(npc.damage * 1.1);
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!FargoSoulsWorld.SwarmActive && !npc.dontTakeDamage && HealPerSecond != 0)
            {
                npc.life += HealPerSecond / 60; //healing stuff
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;

                if (++HealCounter >= 75)
                {
                    HealCounter = Main.rand.Next(30);
                    CombatText.NewText(npc.Hitbox, CombatText.HealLife, HealPerSecond);
                }
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.BrokenArmor, 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }

    public class Golem : GolemPart
    {
        public Golem() : base(180) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Golem);

        public int StompAttackCounter;
        public int SpikyBallTimer;
        //public int AntiAirTimer;

        public bool DoStompBehaviour;
        public bool HaveBoostedJumpHeight;
        public bool IsInTemple;

        public bool DroppedSummon;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(StompAttackCounter);
            binaryWriter.Write7BitEncodedInt(SpikyBallTimer);
            //binaryWriter.Write7BitEncodedInt(AntiAirTimer);
            bitWriter.WriteBit(DoStompBehaviour);
            bitWriter.WriteBit(HaveBoostedJumpHeight);
            bitWriter.WriteBit(IsInTemple);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            StompAttackCounter = binaryReader.Read7BitEncodedInt();
            SpikyBallTimer = binaryReader.Read7BitEncodedInt();
            //AntiAirTimer = binaryReader.Read7BitEncodedInt();
            DoStompBehaviour = bitReader.ReadBit();
            HaveBoostedJumpHeight = bitReader.ReadBit();
            IsInTemple = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 5;
            npc.damage = (int)(npc.damage * 1.2);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            NPC.golemBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return result;

            /*if (npc.ai[0] == 0f && npc.velocity.Y == 0f) //manipulating golem jump ai
            {
                if (npc.ai[1] > 0f)
                {
                    npc.ai[1] += 5f; //count up to initiate jump faster
                }
                else
                {
                    float threshold = -2f - (float)Math.Round(18f * npc.life / npc.lifeMax);

                    if (npc.ai[1] < threshold) //jump activates at npc.ai[1] == -1
                        npc.ai[1] = threshold;
                }
            }*/

            foreach (Player p in Main.player)
            {
                if (p.active && p.Distance(npc.Center) < 2000)
                    p.AddBuff(ModContent.BuffType<LowGround>(), 2);
            }

            HealPerSecond = FargoSoulsWorld.MasochistModeReal ? 240 : 180;
            if (!IsInTemple) //temple enrage, more horiz move and fast jumps
            {
                HealPerSecond *= 2;
                npc.position.X += npc.velocity.X / 2f;
                if (npc.velocity.Y < 0)
                {
                    npc.position.Y += npc.velocity.Y * 0.5f;
                    if (npc.velocity.Y > -2)
                        npc.velocity.Y = 20;
                }
            }

            if (npc.velocity.Y < 0) //jumping up
            {
                if (!HaveBoostedJumpHeight)
                {
                    HaveBoostedJumpHeight = true;
                    npc.velocity.Y *= 1.25f;

                    if (!IsInTemple) //temple enrage
                    {
                        if (Main.player[npc.target].Center.Y < npc.Center.Y - 16 * 30)
                            npc.velocity.Y *= 1.5f;
                    }
                }
            }
            else
            {
                HaveBoostedJumpHeight = false;
            }

            if (DoStompBehaviour)
            {
                if (npc.velocity.Y == 0f) //landing attacks
                {
                    DoStompBehaviour = false;
                    IsInTemple = Framing.GetTileSafely(npc.Center).WallType == WallID.LihzahrdBrickUnsafe;

                    if (IsInTemple) //in temple
                    {
                        StompAttackCounter++;
                        if (StompAttackCounter == 1) //plant geysers
                        {
                            if (FargoSoulsWorld.MasochistModeReal)
                                StompAttackCounter++;

                            Vector2 spawnPos = new Vector2(npc.position.X, npc.Center.Y); //floor geysers
                            spawnPos.X -= npc.width * 7;
                            for (int i = 0; i < 6; i++)
                            {
                                int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<GolemGeyser2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI);
                            }

                            spawnPos = npc.Center;
                            for (int i = -3; i <= 3; i++) //ceiling geysers
                            {
                                int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                                int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, ModContent.ProjectileType<GolemGeyser>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI);
                            }
                        }
                        else if (StompAttackCounter == 2) //empty jump
                        {

                        }
                        else if (StompAttackCounter == 3) //rocks fall
                        {
                            if (FargoSoulsWorld.MasochistModeReal)
                                StompAttackCounter = 0;

                            if (npc.HasPlayerTarget)
                            {
                                if (!Main.dedServ)
                                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 20;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileID.DD2OgreSmash, 0, 0, Main.myPlayer);

                                for (int i = -2; i <= 2; i++)
                                {
                                    int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                                    int tilePosY = (int)Main.player[npc.target].Center.Y / 16;// + 1;
                                    tilePosX += 4 * i;

                                    //first move up through solid tiles
                                    for (int j = 0; j < 100; j++)
                                    {
                                        if (Framing.GetTileSafely(tilePosX, tilePosY).HasUnactuatedTile && Main.tileSolid[Framing.GetTileSafely(tilePosX, tilePosY).TileType])
                                            tilePosY--;
                                        else
                                            break;
                                    }
                                    //then move up through air until next ceiling reached
                                    for (int j = 0; j < 100; j++)
                                    {
                                        if (Framing.GetTileSafely(tilePosX, tilePosY).HasUnactuatedTile && Main.tileSolid[Framing.GetTileSafely(tilePosX, tilePosY).TileType])
                                            break;

                                        tilePosY--;
                                    }

                                    Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<GolemBoulder>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                }
                            }
                        }
                        else //empty jump
                        {
                            StompAttackCounter = 0;
                        }
                    }
                    else //outside temple
                    {
                        Vector2 spawnPos = new Vector2(npc.position.X, npc.Center.Y);
                        spawnPos.X -= npc.width * 7;
                        for (int i = 0; i < 6; i++)
                        {
                            int tilePosX = (int)spawnPos.X / 16 + npc.width * i * 3 / 16;
                            int tilePosY = (int)spawnPos.Y / 16;// + 1;

                            for (int j = 0; j < 100; j++)
                            {
                                if (Framing.GetTileSafely(tilePosX, tilePosY).HasUnactuatedTile && Main.tileSolid[Framing.GetTileSafely(tilePosX, tilePosY).TileType])
                                    break;

                                tilePosY++;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npc.HasPlayerTarget && Main.player[npc.target].position.Y > tilePosY * 16)
                                {
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, 6.3f, 6.3f,
                                        ProjectileID.FlamesTrap, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, -6.3f, 6.3f,
                                        ProjectileID.FlamesTrap, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                }

                                Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, -8f, ProjectileID.GeyserTrap, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);

                                Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8 - 640, 0f, -8f, ProjectileID.GeyserTrap, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8 - 640, 0f, 8f, ProjectileID.GeyserTrap, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                            }
                        }
                        if (npc.HasPlayerTarget)
                        {
                            for (int i = -3; i <= 3; i++)
                            {
                                int tilePosX = (int)Main.player[npc.target].Center.X / 16;
                                int tilePosY = (int)Main.player[npc.target].Center.Y / 16;// + 1;
                                tilePosX += 10 * i;

                                for (int j = 0; j < 30; j++)
                                {
                                    if (Framing.GetTileSafely(tilePosX, tilePosY).HasUnactuatedTile && Main.tileSolid[Framing.GetTileSafely(tilePosX, tilePosY).TileType])
                                        break;
                                    tilePosY--;
                                }

                                Vector2 spawn = new Vector2(tilePosX * 16 + 8, tilePosY * 16 + 8);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), spawn, Vector2.Zero, ModContent.ProjectileType<GolemBoulder>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                DoStompBehaviour = true;
            }

            //spray spiky balls
            if (FargoSoulsWorld.MasochistModeReal && ++SpikyBallTimer >= 900)
            {
                if (Framing.GetTileSafely(npc.Center).WallType == WallID.LihzahrdBrickUnsafe)
                {
                    if (npc.velocity.Y > 0) //only when falling, implicitly assume at peak of a jump
                    {
                        SpikyBallTimer = FargoSoulsWorld.MasochistModeReal ? 600 : 0;
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                                  Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-10, -6), ModContent.ProjectileType<GolemSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        }
                    }
                }
                else //outside temple
                {
                    SpikyBallTimer = 600; //do it more often
                    for (int i = 0; i < 16; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height),
                              Main.rand.NextFloat(-1f, 1f), Main.rand.Next(-20, -9), ModContent.ProjectileType<GolemSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                    }
                }
            }

            //golem's anti-air fireball spray (when player is above)
            //if (FargoSoulsWorld.MasochistModeReal && ++AntiAirTimer > 240 && npc.velocity.Y == 0)
            //{
            //    AntiAirTimer = 0;
            //    if (npc.HasPlayerTarget && Main.player[npc.target].Center.Y < npc.Bottom.Y
            //        && Main.netMode != NetmodeID.MultiplayerClient) //shoutouts to arterius
            //    {
            //        bool inTemple = Framing.GetTileSafely(npc.Center).WallType == WallID.LihzahrdBrickUnsafe;

            //        float gravity = -0.2f; //normally floats up
            //        if (Main.player[npc.target].Center.Y > npc.Bottom.Y)
            //            gravity *= -1f; //aim down if player below golem

            //        const float time = 60f;
            //        Vector2 distance = Main.player[npc.target].Center - npc.Center;
            //        distance += Main.player[npc.target].velocity * 45f;
            //        distance.X = distance.X / time;
            //        distance.Y = distance.Y / time - 0.5f * gravity * time;

            //        if (Math.Sign(distance.Y) != Math.Sign(gravity))
            //            distance.Y = 0f; //cannot arc shots to hit someone on the same elevation

            //        int max = inTemple ? 2 : 4;
            //        for (int i = -max; i <= max; i++)
            //        {
            //            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.Center.Y, distance.X + i, distance.Y,
            //                ModContent.ProjectileType<GolemFireball>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, gravity, 0);
            //        }
            //    }
            //}

            EModeUtils.DropSummon(npc, "LihzahrdPowerCell2", NPC.downedGolemBoss, ref DroppedSummon, NPC.downedPlantBoss);

            return result;
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0.9;

            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<LihzahrdTreasureBox>()));
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.GoldenCrateHard, 5));
            npcLoot.Add(emodeRule);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            for (int i = 1; i <= 3; i++)
                LoadGolem(recolor, i);
        }
    }

    public class GolemFist : GolemPart
    {
        public GolemFist() : base(9999) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.GolemFistLeft, NPCID.GolemFistRight);

        public int FistAttackRateSlowdownTimer;

        public bool DoAttackOnFistImpact;


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(FistAttackRateSlowdownTimer);
            bitWriter.WriteBit(DoAttackOnFistImpact);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            FistAttackRateSlowdownTimer = binaryReader.Read7BitEncodedInt();
            DoAttackOnFistImpact = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lifeMax *= 2;
            npc.damage = (int)(npc.damage * 1.3);

            npc.scale += 0.5f;
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (ProjectileID.Sets.IsAWhip[projectile.type])
                return false;

            return base.CanBeHitByProjectile(npc, projectile);
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            if (npc.HasValidTarget && Framing.GetTileSafely(Main.player[npc.target].Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                if (npc.ai[0] == 1) //on the tick it shoots out, reset counter
                {
                    FistAttackRateSlowdownTimer = 0;
                }
                else
                {
                    if (++FistAttackRateSlowdownTimer < 90) //this basically tracks total time since punch started
                    {
                        npc.ai[1] = 0; //don't allow attacking until counter finishes counting up
                    }
                }

                if (npc.velocity.Length() > 10)
                    npc.position -= Vector2.Normalize(npc.velocity) * (npc.velocity.Length() - 10);
            }

            if (npc.ai[0] == 0f && DoAttackOnFistImpact)
            {
                DoAttackOnFistImpact = false;
                if (Framing.GetTileSafely(Main.player[npc.target].Center).WallType != WallID.LihzahrdBrickUnsafe || FargoSoulsWorld.MasochistModeReal)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                }
            }
            DoAttackOnFistImpact = npc.ai[0] != 0f;

            NPC golem = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
            if (golem != null)
            {
                if (npc.ai[0] == 0 && npc.Distance(golem.Center) < golem.width * 1.5f) //when attached to body
                    npc.position += golem.velocity; //stick to body better, dont get left behind during jumps
            }

            return result;
        }

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void SafeOnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.SafeOnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (projectile.maxPenetrate != 1 && FargoSoulsUtil.CanDeleteProjectile(projectile))
                projectile.timeLeft = 0;
        }
    }

    public class GolemHead : GolemPart
    {
        public GolemHead() : base(180) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.GolemHead, NPCID.GolemHeadFree);

        public int AttackTimer;
        public int DeathraySweepTargetHeight;

        public float SuppressedAi1;
        public float SuppressedAi2;

        public bool DoAttack;
        public bool DoDeathray;
        public bool SweepToLeft;
        public bool IsInTemple;



        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(AttackTimer);
            binaryWriter.Write7BitEncodedInt(DeathraySweepTargetHeight);
            binaryWriter.Write(SuppressedAi1);
            binaryWriter.Write(SuppressedAi2);
            bitWriter.WriteBit(DoAttack);
            bitWriter.WriteBit(DoDeathray);
            bitWriter.WriteBit(SweepToLeft);
            bitWriter.WriteBit(IsInTemple);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            AttackTimer = binaryReader.Read7BitEncodedInt();
            DeathraySweepTargetHeight = binaryReader.Read7BitEncodedInt();
            SuppressedAi1 = binaryReader.ReadSingle();
            SuppressedAi2 = binaryReader.ReadSingle();
            DoAttack = bitReader.ReadBit();
            DoDeathray = bitReader.ReadBit();
            SweepToLeft = bitReader.ReadBit();
            IsInTemple = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            AttackTimer = 540;
            DoDeathray = true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
        {
            return base.CanHitPlayer(npc, target, ref CooldownSlot) && npc.type != NPCID.GolemHeadFree;
        }

        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return result;

            NPC golem = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);

            if (npc.type == NPCID.GolemHead)
            {
                if (golem != null)
                    npc.position += golem.velocity;
            }
            else //detatched head
            {
                const int attackThreshold = 540;

                if (!DoAttack) //default mode
                {
                    npc.position += npc.velocity * 0.25f;
                    npc.position.Y += npc.velocity.Y * 0.25f;

                    if (!npc.noTileCollide && npc.HasValidTarget && Collision.SolidCollision(npc.position, npc.width, npc.height)) //unstick from walls
                        npc.position += npc.DirectionTo(Main.player[npc.target].Center) * 4;

                    //disable attacks when nearby
                    if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 350 && !FargoSoulsWorld.MasochistModeReal)
                    {
                        if (SuppressedAi1 < npc.ai[1])
                            SuppressedAi1 = npc.ai[1];
                        npc.ai[1] = 0f;

                        if (SuppressedAi2 < npc.ai[2])
                            SuppressedAi2 = npc.ai[2];
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        if (npc.ai[1] < SuppressedAi1)
                            npc.ai[1] = SuppressedAi1;
                        SuppressedAi1 = 0;

                        if (npc.ai[2] < SuppressedAi2)
                            npc.ai[2] = SuppressedAi2;
                        SuppressedAi2 = 0;

                        if (!DoDeathray && AttackTimer % 120 > 90)
                        {
                            npc.ai[1] += 90;
                            npc.ai[2] += 90;
                        }
                    }

                    if (++AttackTimer > attackThreshold)
                    {
                        AttackTimer = 0;

                        DeathraySweepTargetHeight = 0;
                        DoAttack = true;
                        IsInTemple = Framing.GetTileSafely(npc.Center).WallType == WallID.LihzahrdBrickUnsafe;

                        npc.netUpdate = true;
                        NetSync(npc);
                    }
                }
                else //deathray time
                {
                    if (golem == null) //die if golem is dead
                    {
                        npc.life = 0;
                        npc.HitEffect();
                        npc.checkDead();
                        return false;
                    }

                    npc.noTileCollide = true;

                    const int fireTime = 120;

                    npc.localAI[0] = AttackTimer > fireTime ? 1f : 0f; //mouth animations

                    bool doSpikeBalls = !DoDeathray;
                    if (FargoSoulsWorld.MasochistModeReal || !IsInTemple)
                    {
                        DoDeathray = true;
                        doSpikeBalls = true;
                    }

                    if (++AttackTimer < fireTime) //move to above golem
                    {
                        if (AttackTimer == 1)
                        {
                            SoundEngine.PlaySound(SoundID.Roar, npc.Center);

                            //telegraph
                            if (DoDeathray && Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, NPCID.QueenBee);
                        }

                        Vector2 target = golem.Center;
                        target.Y -= 250;
                        if (target.Y > DeathraySweepTargetHeight) //stores lowest remembered golem position
                            DeathraySweepTargetHeight = (int)target.Y;
                        target.Y = DeathraySweepTargetHeight;
                        if (npc.HasPlayerTarget && Main.player[npc.target].position.Y < target.Y)
                            target.Y = Main.player[npc.target].position.Y;

                        npc.velocity = (target - npc.Center) / 30;
                    }
                    else if (AttackTimer == fireTime) //attack
                    {
                        npc.velocity = Vector2.Zero;
                        if (npc.HasPlayerTarget) //stores if player is on head's left at this moment
                            SweepToLeft = Main.player[npc.target].Center.X < npc.Center.X;
                        npc.netUpdate = true;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (DoDeathray)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.UnitY, ModContent.ProjectileType<GolemBeam>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 1.5f), 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }

                            if (doSpikeBalls)
                            {
                                SoundEngine.PlaySound(SoundID.Item92, npc.Center);

                                const int max = 3;
                                for (int i = -max; i <= max; i++)
                                {
                                    Vector2 vel = 6f * -Vector2.UnitY.RotatedBy(MathHelper.PiOver2 / max * (i + Main.rand.NextFloat(0.25f, 0.75f) * (Main.rand.NextBool() ? -1 : 1)));
                                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<GolemSpikeBallBig>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft -= Main.rand.Next(60);
                                }
                            }
                        }
                    }
                    else if (AttackTimer < fireTime + 20)
                    {
                        //do nothing
                    }
                    else if (AttackTimer < fireTime + 150 && DoDeathray)
                    {
                        npc.velocity.X += SweepToLeft ? -.15f : .15f;

                        Tile tile = Framing.GetTileSafely(npc.Center); //stop if reached a wall, but only 1sec after started firing
                        if (AttackTimer > fireTime + 60 && (tile.HasUnactuatedTile && tile.TileType == TileID.LihzahrdBrick && tile.WallType == WallID.LihzahrdBrickUnsafe)
                            || (IsInTemple && tile.WallType != WallID.LihzahrdBrickUnsafe)) //i.e. started in temple but has left temple, then stop
                        {
                            npc.velocity = Vector2.Zero;
                            npc.netUpdate = true;

                            AttackTimer = 0;
                            DeathraySweepTargetHeight = 0;
                            DoAttack = false;
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;
                        AttackTimer = 0;
                        DeathraySweepTargetHeight = 0;
                        DoAttack = false;
                    }

                    if (!FargoSoulsWorld.MasochistModeReal)
                    {
                        const float geyserTiming = 100;
                        if (AttackTimer % geyserTiming == geyserTiming - 5)
                        {
                            Vector2 spawnPos = golem.Center;
                            float offset = AttackTimer % (geyserTiming * 2) == geyserTiming - 5 ? 0 : 0.5f;
                            for (int i = -3; i <= 3; i++) //ceiling geysers
                            {
                                int tilePosX = (int)(spawnPos.X / 16 + golem.width * (i + offset) * 3 / 16);
                                int tilePosY = (int)spawnPos.Y / 16;// + 1;

                                int type = IsInTemple ? ModContent.ProjectileType<GolemGeyser>() : ModContent.ProjectileType<GolemGeyser2>();
                                Projectile.NewProjectile(npc.GetSource_FromThis(), tilePosX * 16 + 8, tilePosY * 16 + 8, 0f, 0f, type, FargoSoulsUtil.ScaledProjectileDamage(golem.damage), 0f, Main.myPlayer, golem.whoAmI);
                            }
                        }

                        if (IsInTemple) //nerf golem movement during deathray dash, provided we're in temple
                        {
                            if (golem.HasValidTarget)
                            {
                                //golem.velocity.X = 0f;

                                if (golem.ai[0] == 0f && golem.velocity.Y == 0f && golem.ai[1] > 1f) //if golem is standing on ground and preparing to jump, stall it
                                    golem.ai[1] = 1f;

                                golem.GetGlobalNPC<Golem>().DoStompBehaviour = false; //disable stomp attacks
                            }
                        }
                    }

                    if (!DoAttack) //spray lasers after dash
                    {
                        DoDeathray = !DoDeathray;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = IsInTemple ? 6 : 10;
                            int speed = IsInTemple ? 6 : -12; //down in temple, up outside it
                            for (int i = -max; i <= max; i++)
                            {
                                int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed * Vector2.UnitY.RotatedBy(Math.PI / 2 / max * i),
                                    ModContent.ProjectileType<EyeBeam2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 1200;
                            }
                        }
                    }

                    if (npc.netUpdate)
                    {
                        npc.netUpdate = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        NetSync(npc);
                    }
                    return false;
                }
            }

            return result;
        }
    }
}
