using Fargowiltas.Items.Summons.Mutant;
using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class SkeletronHead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronHead);

        public int ReticleTarget;
        public int BabyGuardianTimer;
        public int DGSpeedRampup;
        
        public bool InPhase2;

        public bool DroppedSummon;
        public bool HasSaidEndure;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(ReticleTarget), IntStrategies.CompoundStrategy },
                { new Ref<object>(BabyGuardianTimer), IntStrategies.CompoundStrategy },
                { new Ref<object>(DGSpeedRampup), IntStrategies.CompoundStrategy },
                
                { new Ref<object>(InPhase2), BoolStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.skeleBoss = npc.whoAmI;

            if (FargoSoulsWorld.SwarmActive)
                return;

            if (npc.ai[1] == 0f)
            {
                if (npc.ai[2] == 800 - 90) //telegraph spin
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
                }
                if (npc.ai[2] < 800 - 5)
                {
                    ReticleTarget = npc.target;
                }
            }

            if (npc.ai[1] == 1f || npc.ai[1] == 2f) //spinning or DG mode
            {
                //force targeted player back to the one i telegraphed with reticle (otherwise, may target another player when spin starts)
                if (ReticleTarget > -1 && ReticleTarget < Main.maxPlayers)
                {
                    npc.target = ReticleTarget;
                    ReticleTarget = -1;

                    npc.netUpdate = true;
                    NetSync(npc);

                    if (!npc.HasValidTarget)
                        npc.TargetClosest(false);

                    if (npc.ai[1] == 1) //do cross guardian attack
                    {
                        if (!FargoSoulsWorld.MasochistModeReal)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++) //also clear leftover babies
                            {
                                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<SkeletronGuardian2>())
                                    Main.projectile[i].Kill();
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = -2; j <= 2; j++)
                                {
                                    Vector2 spawnPos = new Vector2(1200, 80 * j);
                                    Vector2 vel = -8 * Vector2.UnitX;
                                    spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * (i + 0.5));
                                    vel = vel.RotatedBy(Math.PI / 2 * (i + 0.5));
                                    int p = Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<Projectiles.Champions.ShadowGuardian>(),
                                        npc.damage / 4, 0f, Main.myPlayer);
                                    if (p != Main.maxProjectiles)
                                        Main.projectile[p].timeLeft = 1200 / 8 + 1;
                                }
                            }
                        }
                    }
                }

                npc.localAI[2]++;
                float ratio = (float)npc.life / npc.lifeMax;
                float threshold = 20f + 100f * ratio;
                if (npc.localAI[2] >= threshold) //spray bones
                {
                    npc.localAI[2] = 0f;
                    if (threshold > 0 && npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 6f;
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 vel = speed.RotatedBy(Math.PI * 2 / 8 * i);
                            vel += npc.velocity * (1f - ratio);
                            vel.Y -= Math.Abs(vel.X) * 0.2f;
                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<SkeletronBone>(), npc.defDamage / 9 * 2, 0f, Main.myPlayer);
                        }
                    }
                }

                if (BabyGuardianTimer > 180)
                    BabyGuardianTimer = 180;

                if (npc.life < npc.lifeMax * .75 && npc.ai[1] == 1f && --BabyGuardianTimer < 0)
                {
                    BabyGuardianTimer = 180;

                    Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                    if (Main.netMode != NetmodeID.MultiplayerClient) //spray of baby guardian missiles
                    {
                        const int max = 30;
                        float modifier = 1f - (float)npc.life / npc.lifeMax;
                        modifier *= 4f / 3f; //scaling maxes at 25% life
                        if (modifier > 1f)
                            modifier = 1f;
                        int actualNumberToSpawn = (int)(max * modifier);
                        for (int i = 0; i < actualNumberToSpawn; i++)
                        {
                            float speed = Main.rand.NextFloat(3f, 9f);
                            Vector2 velocity = speed * npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI * (Main.rand.NextDouble() - 0.5));
                            float ai1 = speed / (60f + Main.rand.NextFloat(actualNumberToSpawn * 2));
                            Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<SkeletronGuardian>(), npc.damage / 5, 0f, Main.myPlayer, 0f, ai1);
                        }
                    }
                }
            }
            else
            {
                //prevent skeletron from firing his stupid tick 1 no telegraph skull right after finishing spin
                if (npc.ai[2] == 0 && !FargoSoulsWorld.MasochistModeReal)
                    npc.ai[2] = 1;

                if (npc.life < npc.lifeMax * .75 && --BabyGuardianTimer < 0)
                {
                    BabyGuardianTimer = FargoSoulsWorld.MasochistModeReal ? 180 : 240;

                    Main.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                    if (Main.netMode != NetmodeID.MultiplayerClient) //area denial circle spray of baby guardians
                    {
                        for (int j = -1; j <= 1; j++) //to both sides
                        {
                            if (j == 0)
                                continue;

                            const int gap = 40;
                            const int max = 14;
                            float modifier = 1f - (float)npc.life / npc.lifeMax;
                            modifier *= 4f / 3f; //scaling maxes at 25% life
                            if (modifier > 1f || FargoSoulsWorld.MasochistModeReal)
                                modifier = 1f;
                            int actualNumberToSpawn = (int)(max * modifier);
                            Vector2 baseVel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.ToRadians(gap) * j);
                            for (int k = 0; k < actualNumberToSpawn; k++) //a fan of skulls
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float velModifier = 1f + 9f * k / max;
                                    Projectile.NewProjectile(npc.Center, velModifier * baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
                                        ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 5, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient) //one more shot straight behind skeletron
                        {
                            float velModifier = 10f;
                            Projectile.NewProjectile(npc.Center, velModifier * npc.DirectionFrom(Main.player[npc.target].Center),
                                ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 5, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            if (npc.ai[1] == 2f)
            {
                npc.defense = 9999;
                npc.damage = npc.defDamage * 15;

                if (!Main.dayTime && !FargoSoulsWorld.MasochistModeReal)
                {
                    if (++DGSpeedRampup < 120)
                    {
                        npc.position -= npc.velocity * (120 - DGSpeedRampup) / 120;
                    }
                }
            }

            EModeUtils.DropSummon(npc, ModContent.ItemType<SuspiciousSkull>(), NPC.downedBoss3, ref DroppedSummon);

            //FargoSoulsUtil.PrintAI(npc);
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.ai[1] != 2f && !FargoSoulsWorld.SwarmActive)
            {
                Main.PlaySound(SoundID.Roar, npc.Center, 0);

                npc.life = npc.lifeMax / 176;
                if (npc.life < 50)
                    npc.life = 50;

                npc.defense = 9999;
                npc.damage = npc.defDamage * 15;

                npc.ai[1] = 2f;
                npc.netUpdate = true;
                NetSync(npc);

                if (!HasSaidEndure)
                {
                    HasSaidEndure = true;
                    FargoSoulsUtil.PrintText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.SkeletronTransform"), new Color(175, 75, 255));
                }
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ItemID.DungeonFishingCrate, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<NecromanticBrew>());
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
            target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 19);
            LoadGoreRange(recolor, 54, 57);

            Main.boneArmTexture = LoadSprite(recolor, "Arm_Bone");
        }
    }

    public class SkeletronHand : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronHand);

        public int AttackTimer;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(AttackTimer), IntStrategies.CompoundStrategy },
            };

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (FargoSoulsWorld.SwarmActive)
                return;

            NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronHead);
            if (head != null && (head.ai[1] == 1f || head.ai[1] == 2f)) //spinning or DG mode
            {
                if (AttackTimer > 0) //for a short period after ending spin
                {
                    if (--AttackTimer < 65 && AttackTimer % 10 == 0 && npc.HasValidTarget) //periodic below 50%
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                }
            }
            else
            {
                if (AttackTimer != 65 + 150)
                {
                    AttackTimer = 65 + 150;

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget) //throw undead miner
                    {
                        float gravity = 0.4f; //shoot down
                        const float time = 60f;
                        Vector2 distance = Main.player[npc.target].Top - npc.Center;
                        distance.X = distance.X / time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.BoneThrowingSkeleton);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].velocity = distance;
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            target.AddBuff(BuffID.Dazed, 60);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
        }
    }
}
