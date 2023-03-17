using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.BossBags;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Pets;
using FargowiltasSouls.Items.Placeables.Relics;
using FargowiltasSouls.Items.Placeables.Trophies;
using FargowiltasSouls.Items.Summons;
using FargowiltasSouls.Projectiles.MutantBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.MutantBoss
{
    [AutoloadBossHead]
    public class MutantBoss : ModNPC
    {
        Player player => Main.player[NPC.target];

        public bool playerInvulTriggered;
        public int ritualProj, spriteProj, ringProj;
        private bool droppedSummon = false;

        public Queue<float> attackHistory = new Queue<float>();
        public int attackCount;

        public float endTimeVariance;

        public bool ShouldDrawAura;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Godkiller Yharim");

            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>(),
                    ModContent.BuffType<MutantNibble>(),
                    ModContent.BuffType<OceanicMaul>(),
                    ModContent.BuffType<LightningRod>(),
                    ModContent.BuffType<Sadism>(),
                    ModContent.BuffType<GodEater>(),
                    ModContent.BuffType<TimeFrozen>(),
                    ModContent.BuffType<LeadPoison>()
                }
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 120;//34;
            NPC.height = 120;//50;
            NPC.damage = 444;
            NPC.defense = 255;
            NPC.value = Item.buyPrice(7);
            NPC.lifeMax = Main.expertMode ? 7700000 : 3500000;
            NPC.HitSound = SoundID.NPCHit57;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 50f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
            NPC.timeLeft = NPC.activeTime * 30;
            if (FargoSoulsWorld.AngryMutant)
            {
                NPC.damage *= 17;
                NPC.defense *= 10;
            }

            if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                Music = MusicLoader.GetMusicSlot(musicMod,
                    FargoSoulsWorld.MasochistModeReal ? "Assets/Music/rePrologue" : "Assets/Music/SteelRed");
            }
            else
            {
                Music = MusicID.OtherworldlyTowers;
            }
            SceneEffectPriority = SceneEffectPriority.BossHigh;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = (int)Math.Round(NPC.damage * 0.5);
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 0.5 * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            if (!FargoSoulsWorld.MasochistModeReal)
                return false;
            return NPC.Distance(FargoSoulsUtil.ClosestPointInHitbox(target, NPC.Center)) < Player.defaultHeight && NPC.ai[0] > -1;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.type == ModContent.Find<ModNPC>("Fargowiltas", "Deviantt").Type
                || target.type == ModContent.Find<ModNPC>("Fargowiltas", "Abominationn").Type
                || target.type == ModContent.Find<ModNPC>("Fargowiltas", "Mutant").Type)
                return false;

            return base.CanHitNPC(target);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(endTimeVariance);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            endTimeVariance = reader.ReadSingle();
        }

        public override void AI()
        {
            EModeGlobalNPC.mutantBoss = NPC.whoAmI;

            NPC.dontTakeDamage = NPC.ai[0] < 0; //invul in p3

            // Set this to false by default.
            ShouldDrawAura = false;

            ManageAurasAndPreSpawn();
            ManageNeededProjectiles();

            NPC.direction = NPC.spriteDirection = NPC.Center.X < player.Center.X ? 1 : -1;

            bool drainLifeInP3 = true;

            switch ((int)NPC.ai[0])
            {
                #region phase 1

                case 0: SpearTossDirectP1AndChecks(); break;

                case 1: OkuuSpheresP1(); break;

                case 2: PrepareTrueEyeDiveP1(); break;
                case 3: TrueEyeDive(); break;

                case 4: PrepareSpearDashDirectP1(); break;
                case 5: SpearDashDirectP1(); break;
                case 6: WhileDashingP1(); break;

                case 7: ApproachForNextAttackP1(); break;
                case 8: VoidRaysP1(); break;

                case 9: BoundaryBulletHellAndSwordP1(); break;

                #endregion

                #region phase 2

                case 10: Phase2Transition(); break;

                case 11: ApproachForNextAttackP2(); break;
                case 12: VoidRaysP2(); break;

                case 13: PrepareSpearDashPredictiveP2(); break;
                case 14: SpearDashPredictiveP2(); break;
                case 15: WhileDashingP2(); break;

                case 16: goto case 11; //approach for bullet hell
                case 17: BoundaryBulletHellP2(); break;

                case 18: NPC.ai[0]++; break; //new attack can be put here

                case 19: PillarDunk(); break;

                case 20: EOCStarSickles(); break;

                case 21: PrepareSpearDashDirectP2(); break;
                case 22: SpearDashDirectP2(); break;
                case 23: //while dashing
                    if (NPC.ai[1] % 3 == 0)
                        NPC.ai[1]++;
                    goto case 15;

                case 24: SpawnDestroyersForPredictiveThrow(); break;
                case 25: SpearTossPredictiveP2(); break;

                case 26: PrepareMechRayFan(); break;
                case 27: MechRayFan(); break;

                case 28: NPC.ai[0]++; break; //free slot for new attack

                case 29: PrepareFishron1(); break;
                case 30: SpawnFishrons(); break;

                case 31: PrepareTrueEyeDiveP2(); break;
                case 32: goto case 3; //spawn eyes

                case 33: PrepareNuke(); break;
                case 34: Nuke(); break;

                case 35: PrepareSlimeRain(); break;
                case 36: SlimeRain(); break;

                case 37: PrepareFishron2(); break;
                case 38: goto case 30; //spawn fishrons

                case 39: PrepareOkuuSpheresP2(); break;
                case 40: OkuuSpheresP2(); break;

                case 41: SpearTossDirectP2(); break;

                case 42: PrepareTwinRangsAndCrystals(); break;
                case 43: TwinRangsAndCrystals(); break;

                case 44: EmpressSwordWave(); break;

                case 45: PrepareMutantSword(); break;
                case 46: MutantSword(); break;

                //gap in the numbers here so the ai loops right
                //when adding a new attack, remember to make ChooseNextAttack() point to the right case!

                case 48: P2NextAttackPause(); break;

                #endregion

                #region phase 3

                case -1: drainLifeInP3 = Phase3Transition(); break;

                case -2: VoidRaysP3(); break;

                case -3: OkuuSpheresP3(); break;

                case -4: BoundaryBulletHellP3(); break;

                case -5: FinalSpark(); break;

                case -6: DyingDramaticPause(); break;
                case -7: DyingAnimationAndHandling(); break;

                #endregion

                default: NPC.ai[0] = 11; goto case 11; //return to first phase 2 attack
            }

            //in emode p2
            if (FargoSoulsWorld.EternityMode && (NPC.ai[0] < 0 || NPC.ai[0] > 10 || (NPC.ai[0] == 10 && NPC.ai[1] > 150)))
            {
                Main.dayTime = false;
                Main.time = 16200; //midnight, for empress visuals

                Main.raining = false; //disable rain
                Main.rainTime = 0;
                Main.maxRaining = 0;

                Main.bloodMoon = false; //disable blood moon
            }

            if (NPC.ai[0] < 0 && NPC.life > 1 && drainLifeInP3) //in desperation
            {
                int time = FargoSoulsWorld.MasochistModeReal ? 4350 : 480 + 240 + 420 + 480 + 1020 - 60;
                NPC.life -= NPC.lifeMax / time;
                if (NPC.life < 1)
                    NPC.life = 1;
            }

            if (player.immune || player.hurtCooldowns[0] != 0 || player.hurtCooldowns[1] != 0)
                playerInvulTriggered = true;

            //drop summon
            if (FargoSoulsWorld.EternityMode && FargoSoulsWorld.downedAbom && !FargoSoulsWorld.downedMutant && Main.netMode != NetmodeID.MultiplayerClient && NPC.HasPlayerTarget && !droppedSummon)
            {
                Item.NewItem(NPC.GetSource_Loot(), player.Hitbox, ModContent.ItemType<MutantsCurse>());
                droppedSummon = true;
            }
        }

        #region helper functions

        bool spawned;
        void ManageAurasAndPreSpawn()
        {
            if (!spawned)
            {
                spawned = true;

                int prevLifeMax = NPC.lifeMax;
                if (FargoSoulsWorld.AngryMutant) //doing it here to avoid overflow i think
                {
                    NPC.lifeMax *= 100;
                    if (NPC.lifeMax < prevLifeMax)
                        NPC.lifeMax = int.MaxValue;
                }
                NPC.life = NPC.lifeMax;
            }

            if (FargoSoulsWorld.MasochistModeReal && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresence>(), 2);

            if (NPC.localAI[3] == 0)
            {
                NPC.TargetClosest();
                if (NPC.timeLeft < 30)
                    NPC.timeLeft = 30;
                if (NPC.Distance(Main.player[NPC.target].Center) < 1500)
                {
                    NPC.localAI[3] = 1;
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    //EdgyBossText("I hope you're ready to embrace suffering.");
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //if (FargowiltasSouls.Instance.MasomodeEXLoaded) Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModLoader.GetMod("MasomodeEX").ProjectileType("MutantText"), 0, 0f, Main.myPlayer, NPC.whoAmI);

                        if (FargoSoulsWorld.AngryMutant && FargoSoulsWorld.MasochistModeReal)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BossRush>(), 0, 0f, Main.myPlayer, NPC.whoAmI);
                    }
                }
            }
            else if (NPC.localAI[3] == 1)
            {
                ShouldDrawAura = true;
                // -1 means no dust is drawn, as it looks ugly.
                EModeGlobalNPC.Aura(NPC, 2000f, true, -1, default, ModContent.BuffType<GodEater>(), ModContent.BuffType<MutantFang>());
            }
            else
            {
                if (Main.LocalPlayer.active && NPC.Distance(Main.LocalPlayer.Center) < 3000f)
                {                 
                    if (Main.expertMode)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<MutantPresence>(), 2);
                    }

                    if (FargoSoulsWorld.EternityMode && NPC.ai[0] < 0 && NPC.ai[0] > -6)
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<GoldenStasisCD>(), 2);
                        if (FargoSoulsWorld.MasochistModeReal)
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<TimeStopCD>(), 2);
                    }
                    //if (FargowiltasSouls.Instance.CalamityLoaded)
                    //{
                    //    Main.LocalPlayer.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("RageMode")] = true;
                    //    Main.LocalPlayer.buffImmune[ModLoader.GetMod("CalamityMod").BuffType("AdrenalineMode")] = true;
                    //}
                }
            }
        }

        void ManageNeededProjectiles()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) //checks for needed projs
            {
                if (FargoSoulsWorld.EternityMode && NPC.ai[0] != -7 && (NPC.ai[0] < 0 || NPC.ai[0] > 10) && FargoSoulsUtil.ProjectileExists(ritualProj, ModContent.ProjectileType<MutantRitual>()) == null)
                    ritualProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0f, NPC.whoAmI);

                if (FargoSoulsUtil.ProjectileExists(ringProj, ModContent.ProjectileType<MutantRitual5>()) == null)
                    ringProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual5>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                if (FargoSoulsUtil.ProjectileExists(spriteProj, ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>()) == null)
                {
                    /*if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("wheres my sprite"), Color.LimeGreen);
                    else
                        Main.NewText("wheres my sprite");*/
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
                            Projectile projectile = Main.projectile[number];
                            projectile.SetDefaults(ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>());
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
                    else //server
                    {
                        spriteProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.MutantBoss.MutantBoss>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        /*if (Main.netMode == NetmodeID.Server)
                            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"got sprite {spriteProj}"), Color.LimeGreen);
                        else
                            Main.NewText($"got sprite {spriteProj}");*/
                    }
                }
            }
        }

        void ChooseNextAttack(params int[] args)
        {
            float buffer = NPC.ai[0] + 1;
            NPC.ai[0] = 48;
            NPC.ai[1] = 0;
            NPC.ai[2] = buffer;
            NPC.ai[3] = 0;
            NPC.localAI[0] = 0;
            NPC.localAI[1] = 0;
            NPC.localAI[2] = 0;
            //NPC.TargetClosest();
            NPC.netUpdate = true;

            /*string text = "-------------------------------------------------";
            Main.NewText(text);

            text = "";
            foreach (float f in attackHistory)
                text += f.ToString() + " ";
            Main.NewText($"history: {text}");*/

            if (FargoSoulsWorld.EternityMode)
            {
                //become more likely to use randoms as life decreases
                bool useRandomizer = NPC.localAI[3] >= 3 && (FargoSoulsWorld.MasochistModeReal || Main.rand.NextFloat(0.8f) + 0.2f > (float)Math.Pow((float)NPC.life / NPC.lifeMax, 2));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Queue<float> recentAttacks = new Queue<float>(attackHistory); //copy of attack history that i can remove elements from freely

                    //if randomizer, start with a random attack, else use the previous state + 1 as starting attempt BUT DO SOMETHING ELSE IF IT'S ALREADY USED
                    if (useRandomizer)
                        NPC.ai[2] = Main.rand.Next(args);

                    //Main.NewText(useRandomizer ? "(Starting with random)" : "(Starting with regular next attack)");

                    while (recentAttacks.Count > 0)
                    {
                        bool foundAttackToUse = false;

                        for (int i = 0; i < 5; i++) //try to get next attack that isnt in this queue
                        {
                            if (!recentAttacks.Contains(NPC.ai[2]))
                            {
                                foundAttackToUse = true;
                                break;
                            }
                            NPC.ai[2] = Main.rand.Next(args);
                        }

                        if (foundAttackToUse)
                            break;

                        //couldn't find an attack to use after those attempts, forget 1 attack and repeat
                        recentAttacks.Dequeue();

                        //Main.NewText("REDUCE");
                    }

                    /*text = "";
                    foreach (float f in recentAttacks)
                        text += f.ToString() + " ";
                    Main.NewText($"recent: {text}");*/
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int maxMemory = FargoSoulsWorld.MasochistModeReal ? 10 : 16;

                if (attackCount++ > maxMemory * 1.25) //after doing this many attacks, shorten queue so i can be more random again
                {
                    attackCount = 0;
                    maxMemory /= 4;
                }

                attackHistory.Enqueue(NPC.ai[2]);
                while (attackHistory.Count > maxMemory)
                    attackHistory.Dequeue();
            }

            endTimeVariance = FargoSoulsWorld.MasochistModeReal ? Main.rand.NextFloat() : 0;

            /*text = "";
            foreach (float f in attackHistory)
                text += f.ToString() + " ";
            Main.NewText($"after: {text}");*/
        }

        void P1NextAttackOrMasoOptions(float sourceAI)
        {
            if (FargoSoulsWorld.MasochistModeReal && Main.rand.NextBool(3))
            {
                int[] options = new int[] { 0, 1, 2, 4, 7, 9, 9 };
                NPC.ai[0] = Main.rand.Next(options);
                if (NPC.ai[0] == sourceAI) //dont repeat attacks consecutively
                    NPC.ai[0] = sourceAI == 9 ? 0 : 9;

                bool badCombo = false;
                //dont go into boundary/sword from spheres, true eye dive, void rays
                if (NPC.ai[0] == 9 && (sourceAI == 1 || sourceAI == 2 || sourceAI == 7))
                    badCombo = true;
                //dont go into destroyer-toss or void rays from true eye dive
                if ((NPC.ai[0] == 0 || NPC.ai[0] == 7) && sourceAI == 2)
                    badCombo = true;

                if (badCombo)
                    NPC.ai[0] = 4; //default to dashes
                else if (NPC.ai[0] == 9 && Main.rand.NextBool())
                    NPC.localAI[2] = 1f; //force sword attack instead of boundary
                else
                    NPC.localAI[2] = 0f;
            }
            else
            {
                if (NPC.ai[0] == 9 && NPC.localAI[2] == 0)
                {
                    NPC.localAI[2] = 1;
                }
                else
                {
                    NPC.ai[0]++;
                    NPC.localAI[2] = 0f;
                }
            }

            if (NPC.ai[0] >= 10) //dont accidentally go into p2
                NPC.ai[0] = 0;

            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            NPC.localAI[0] = 0;
            NPC.localAI[1] = 0;
            //NPC.localAI[2] = 0; //excluded because boundary-sword logic
            NPC.netUpdate = true;
        }

        void SpawnSphereRing(int max, float speed, int damage, float rotationModifier, float offset = 0)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            float rotation = 2f * (float)Math.PI / max;
            int type = ModContent.ProjectileType<MutantSphereRing>();
            for (int i = 0; i < max; i++)
            {
                Vector2 vel = speed * Vector2.UnitY.RotatedBy(rotation * i + offset);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, type, damage, 0f, Main.myPlayer, rotationModifier * NPC.spriteDirection, speed);
            }
            SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
        }

        bool AliveCheck(Player p, bool forceDespawn = false)
        {
            if (FargoSoulsWorld.SwarmActive || forceDespawn || ((!p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f) && NPC.localAI[3] > 0))
            {
                NPC.TargetClosest();
                p = Main.player[NPC.target];
                if (FargoSoulsWorld.SwarmActive || forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
                {
                    if (NPC.timeLeft > 30)
                        NPC.timeLeft = 30;
                    NPC.velocity.Y -= 1f;
                    if (NPC.timeLeft == 1)
                    {
                        if (NPC.position.Y < 0)
                            NPC.position.Y = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                        {
                            FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, modNPC.Type);
                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].homeless = true;
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                    }
                    return false;
                }
            }

            if (NPC.timeLeft < 3600)
                NPC.timeLeft = 3600;

            if (player.Center.Y / 16f > Main.worldSurface)
            {
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y -= 1f;
                if (NPC.velocity.Y < -32f)
                    NPC.velocity.Y = -32f;
                return false;
            }

            return true;
        }

        bool Phase2Check()
        {
            if (Main.expertMode && NPC.life < NPC.lifeMax / 2)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = 10;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.netUpdate = true;
                    FargoSoulsUtil.ClearHostileProjectiles(1, NPC.whoAmI);
                    //EdgyBossText("Time to stop playing around.");
                }
                return true;
            }
            return false;
        }

        void Movement(Vector2 target, float speed, bool fastX = true, bool obeySpeedCap = true)
        {
            float turnaroundModifier = 1f;
            float maxSpeed = 24;

            if (FargoSoulsWorld.MasochistModeReal)
            {
                speed *= 2;
                turnaroundModifier *= 2f;
                maxSpeed *= 1.5f;
            }

            if (Math.Abs(NPC.Center.X - target.X) > 10)
            {
                if (NPC.Center.X < target.X)
                {
                    NPC.velocity.X += speed;
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += speed * (fastX ? 2 : 1) * turnaroundModifier;
                }
                else
                {
                    NPC.velocity.X -= speed;
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X -= speed * (fastX ? 2 : 1) * turnaroundModifier;
                }
            }
            if (NPC.Center.Y < target.Y)
            {
                NPC.velocity.Y += speed;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speed * 2 * turnaroundModifier;
            }
            else
            {
                NPC.velocity.Y -= speed;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speed * 2 * turnaroundModifier;
            }

            if (obeySpeedCap)
            {
                if (Math.Abs(NPC.velocity.X) > maxSpeed)
                    NPC.velocity.X = maxSpeed * Math.Sign(NPC.velocity.X);
                if (Math.Abs(NPC.velocity.Y) > maxSpeed)
                    NPC.velocity.Y = maxSpeed * Math.Sign(NPC.velocity.Y);
            }
        }

        void DramaticTransition(bool fightIsOver, bool normalAnimation = true)
        {
            NPC.velocity = Vector2.Zero;

            if (fightIsOver)
            {
                Main.player[NPC.target].ClearBuff(ModContent.BuffType<MutantFang>());
                Main.player[NPC.target].ClearBuff(ModContent.BuffType<AbomRebirth>());
            }

            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 1.5f }, NPC.Center);

            if (normalAnimation)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantBomb>(), 0, 0f, Main.myPlayer);
            }

            const int max = 40;
            float totalAmountToHeal = fightIsOver
                ? Main.player[NPC.target].statLifeMax2 / 4f
                : (NPC.lifeMax - NPC.life) + NPC.lifeMax * 0.1f;
            for (int i = 0; i < max; i++)
            {
                int heal = (int)(Main.rand.NextFloat(0.9f, 1.1f) * totalAmountToHeal / max);
                Vector2 vel = normalAnimation
                    ? Main.rand.NextFloat(2f, 18f) * -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) //looks messier normally
                    : 0.1f * -Vector2.UnitY.RotatedBy(MathHelper.TwoPi / max * i); //looks controlled during mutant p1 skip
                float ai0 = fightIsOver ? -Main.player[NPC.target].whoAmI - 1 : NPC.whoAmI; //player -1 necessary for edge case of player 0
                float ai1 = vel.Length() / Main.rand.Next(fightIsOver ? 90 : 150, 180); //window in which they begin homing in
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantHeal>(), heal, 0f, Main.myPlayer, ai0, ai1);
            }
        }

        void EModeSpecialEffects()
        {
            if (FargoSoulsWorld.EternityMode)
            {
                //because this breaks the background???
                if (Main.GameModeInfo.IsJourneyMode && CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)
                    CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().SetPowerInfo(false);

                if (!SkyManager.Instance["FargowiltasSouls:MutantBoss"].IsActive())
                    SkyManager.Instance.Activate("FargowiltasSouls:MutantBoss");

                if (ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
                {
                    if (FargoSoulsWorld.MasochistModeReal && musicMod.Version >= Version.Parse("0.1.1"))
                        Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Storia");
                    else
                        Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/rePrologue");
                }
            }
        }

        void TryMasoP3Theme()
        {
            if (FargoSoulsWorld.MasochistModeReal
                && ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                && musicMod.Version >= Version.Parse("0.1.1.3"))
            {
                Music = MusicLoader.GetMusicSlot(musicMod, "Assets/Music/StoriaShort");
            }
        }

        void FancyFireballs(int repeats)
        {
            float modifier = 0;
            for (int i = 0; i < repeats; i++)
                modifier = MathHelper.Lerp(modifier, 1f, 0.08f);

            float distance = 1600 * (1f - modifier);
            float rotation = MathHelper.TwoPi * modifier;
            const int max = 6;
            for (int i = 0; i < max; i++)
            {
                int d = Dust.NewDust(NPC.Center + distance * Vector2.UnitX.RotatedBy(rotation + MathHelper.TwoPi / max * i), 0, 0, DustID.Vortex, NPC.velocity.X * 0.3f, NPC.velocity.Y * 0.3f, newColor: Color.White);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = 6f - 4f * modifier;
            }
        }

        /*private void EdgyBossText(string text)
        {
            if (Fargowiltas.Instance.CalamityLoaded) //edgy boss text
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(text, Color.LimeGreen);
                else if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen);
            }
        }*/

        #endregion

        #region p1

        void SpearTossDirectP1AndChecks()
        {
            if (!AliveCheck(player))
                return;
            if (Phase2Check())
                return;
            NPC.localAI[2] = 0;
            Vector2 targetPos = player.Center;
            targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
            if (NPC.Distance(targetPos) > 50)
            {
                Movement(targetPos, NPC.localAI[3] > 0 ? 0.5f : 2f, true, NPC.localAI[3] > 0);
            }

            if (NPC.ai[3] == 0)
            {
                NPC.ai[3] = FargoSoulsWorld.MasochistModeReal ? Main.rand.Next(2, 8) : 5;
                NPC.netUpdate = true;
            }

            if (NPC.localAI[3] > 0) //dont begin proper ai timer until in range to begin fight
                NPC.ai[1]++;

            if (NPC.ai[1] < 145) //track player up until just before attack
            {
                NPC.localAI[0] = NPC.DirectionTo(player.Center + player.velocity * 30f).ToRotation();
            }

            if (NPC.ai[1] > 150) //120)
            {
                NPC.netUpdate = true;
                //NPC.TargetClosest();
                NPC.ai[1] = 60;
                if (++NPC.ai[2] > NPC.ai[3])
                {
                    P1NextAttackOrMasoOptions(NPC.ai[0]);
                    NPC.velocity = NPC.DirectionTo(player.Center) * 2f;
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.localAI[0].ToRotationVector2() * 25f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target);
                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }
                }
                NPC.localAI[0] = 0;
            }
            else if (NPC.ai[1] == 61 && NPC.ai[2] < NPC.ai[3] && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (FargoSoulsWorld.EternityMode && FargoSoulsWorld.skipMutantP1 >= 10 && !FargoSoulsWorld.MasochistModeReal)
                {
                    NPC.ai[0] = 10; //skip to phase 2
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.localAI[0] = 0;
                    NPC.netUpdate = true;

                    if (FargoSoulsWorld.skipMutantP1 == 10)
                        FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.MutantSkipP1", Color.LimeGreen);

                    if (FargoSoulsWorld.skipMutantP1 >= 10)
                        NPC.ai[2] = 1; //flag for different p2 transition animation

                    return;
                }

                if (FargoSoulsWorld.MasochistModeReal && NPC.ai[2] == 0) //first time only
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient) //spawn worm
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Vector2 vel = NPC.DirectionFrom(player.Center).RotatedByRandom(MathHelper.ToRadians(120)) * 10f;
                            float ai1 = 0.8f + 0.4f * j / 5f;
                            int current = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerHead>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target, ai1);
                            //timeleft: remaining duration of this case + extra delay after + successive death
                            Main.projectile[current].timeLeft = 90 * ((int)NPC.ai[3] + 1) + 30 + j * 6;
                            int max = Main.rand.Next(8, 19);
                            for (int i = 0; i < max; i++)
                                current = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerBody>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, Main.projectile[current].identity);
                            int previous = current;
                            current = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerTail>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, Main.projectile[current].identity);
                            Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                            Main.projectile[previous].netUpdate = true;
                        }
                    }
                }

                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 85f, NPC.whoAmI);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 3);

                //Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
            }
        }

        void OkuuSpheresP1()
        {
            if (Phase2Check())
                return;

            if (FargoSoulsWorld.MasochistModeReal)
                NPC.velocity = Vector2.Zero;

            if (--NPC.ai[1] < 0)
            {
                NPC.netUpdate = true;
                float modifier = FargoSoulsWorld.MasochistModeReal ? 3 : 1;
                NPC.ai[1] = 90 / modifier;
                if (++NPC.ai[2] > 4 * modifier)
                {
                    P1NextAttackOrMasoOptions(NPC.ai[0]);
                }
                else
                {
                    int max = FargoSoulsWorld.MasochistModeReal ? 9 : 6;
                    float speed = FargoSoulsWorld.MasochistModeReal ? 12 : 9;
                    int sign = FargoSoulsWorld.MasochistModeReal ? (NPC.ai[2] % 2 == 0 ? 1 : -1) : 1;
                    SpawnSphereRing(max, speed, (int)(0.8 * FargoSoulsUtil.ScaledProjectileDamage(NPC.damage)), 1f * sign);
                    SpawnSphereRing(max, speed, (int)(0.8 * FargoSoulsUtil.ScaledProjectileDamage(NPC.damage)), -0.5f * sign);
                }
            }
        }

        void PrepareTrueEyeDiveP1()
        {
            if (!AliveCheck(player))
                return;
            if (Phase2Check())
                return;
            Vector2 targetPos = player.Center;
            targetPos.X += 700 * (NPC.Center.X < targetPos.X ? -1 : 1);
            targetPos.Y -= 400;
            Movement(targetPos, 0.6f);
            if (NPC.Distance(targetPos) < 50 || ++NPC.ai[1] > 180) //dive here
            {
                NPC.velocity.X = 35f * (NPC.position.X < player.position.X ? 1 : -1);
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y *= -1;
                NPC.velocity.Y *= 0.3f;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }
        }

        void TrueEyeDive()
        {
            if (NPC.ai[3] == 0)
                NPC.ai[3] = Math.Sign(NPC.Center.X - player.Center.X);

            if (NPC.ai[2] > 3)
            {
                Vector2 targetPos = player.Center;
                targetPos.X += NPC.Center.X < player.Center.X ? -500 : 500;
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.3f);
            }
            else
            {
                NPC.velocity *= 0.99f;
            }

            if (--NPC.ai[1] < 0)
            {
                NPC.ai[1] = 15;
                int maxEyeThreshold = FargoSoulsWorld.MasochistModeReal ? 6 : 3;
                int endlag = FargoSoulsWorld.MasochistModeReal ? 3 : 5;
                if (++NPC.ai[2] > maxEyeThreshold + endlag)
                {
                    if (NPC.ai[0] == 3)
                        P1NextAttackOrMasoOptions(2);
                    else
                        ChooseNextAttack(13, 19, 21, 24, 33, 33, 33, 39, 41, 44);
                }
                else if (NPC.ai[2] <= maxEyeThreshold)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type;
                        float ratio = NPC.ai[2] / maxEyeThreshold * 3;
                        if (ratio <= 1f)
                            type = ModContent.ProjectileType<MutantTrueEyeL>();
                        else if (ratio <= 2f)
                            type = ModContent.ProjectileType<MutantTrueEyeS>();
                        else
                            type = ModContent.ProjectileType<MutantTrueEyeR>();

                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer, NPC.target);
                        if (p != Main.maxProjectiles) //inform them which side attack began on
                        {
                            Main.projectile[p].localAI[1] = NPC.ai[3]; //this is ok, they sync this
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                    SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 135, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 12f;
                    }
                }
            }
        }

        void PrepareSpearDashDirectP1()
        {
            if (Phase2Check())
                return;
            if (NPC.ai[3] == 0)
            {
                if (!AliveCheck(player))
                    return;
                NPC.ai[3] = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 240); // 250);
            }

            if (++NPC.ai[1] > 240)
            {
                if (!AliveCheck(player))
                    return;
                NPC.ai[0]++;
                NPC.ai[3] = 0;
                NPC.netUpdate = true;
            }

            Vector2 targetPos = player.Center;
            if (NPC.Top.Y < player.Bottom.Y)
                targetPos.X += 600f * Math.Sign(NPC.Center.X - player.Center.X);
            targetPos.Y += 400f;
            Movement(targetPos, 0.7f, false);
        }

        void SpearDashDirectP1()
        {
            if (Phase2Check())
                return;
            NPC.velocity *= 0.9f;

            if (NPC.ai[3] == 0)
                NPC.ai[3] = FargoSoulsWorld.MasochistModeReal ? Main.rand.Next(3, 15) : 10;

            if (++NPC.ai[1] > NPC.ai[3])
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                if (++NPC.ai[2] > 5)
                {
                    P1NextAttackOrMasoOptions(4); //go to next attack after dashes
                }
                else
                {
                    float speed = FargoSoulsWorld.MasochistModeReal ? 45f : 30f;
                    NPC.velocity = speed * NPC.DirectionTo(player.Center + player.velocity);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);

                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }
                }
            }
        }

        void WhileDashingP1()
        {
            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
            if (++NPC.ai[1] > 30)
            {
                if (!AliveCheck(player))
                    return;
                NPC.netUpdate = true;
                NPC.ai[0]--;
                NPC.ai[1] = 0;
            }
        }

        void ApproachForNextAttackP1()
        {
            if (!AliveCheck(player))
                return;
            if (Phase2Check())
                return;
            Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 250;
            if (NPC.Distance(targetPos) > 50 && ++NPC.ai[2] < 180)
            {
                Movement(targetPos, 0.5f);
            }
            else
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = player.DirectionTo(NPC.Center).ToRotation();
                NPC.ai[3] = (float)Math.PI / 10f;
                if (player.Center.X < NPC.Center.X)
                    NPC.ai[3] *= -1;
            }
        }

        void VoidRaysP1()
        {
            if (Phase2Check())
                return;
            NPC.velocity = Vector2.Zero;
            if (--NPC.ai[1] < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(2, 0).RotatedBy(NPC.ai[2]), ModContent.ProjectileType<MutantMark1>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                NPC.ai[1] = FargoSoulsWorld.MasochistModeReal ? 3 : 5; //delay between projs
                NPC.ai[2] += NPC.ai[3];
                if (NPC.localAI[0]++ == 20 || NPC.localAI[0] == 40)
                {
                    NPC.netUpdate = true;
                    NPC.ai[2] -= NPC.ai[3] / (FargoSoulsWorld.MasochistModeReal ? 3 : 2);
                }
                else if (NPC.localAI[0] >= (FargoSoulsWorld.MasochistModeReal ? 60 : 40))
                {
                    P1NextAttackOrMasoOptions(7);
                }
            }
        }

        const int MUTANT_SWORD_SPACING = 80;
        const int MUTANT_SWORD_MAX = 12;

        void BoundaryBulletHellAndSwordP1()
        {
            switch ((int)NPC.localAI[2])
            {
                case 0: //boundary lite
                    if (NPC.ai[3] == 0)
                    {
                        if (AliveCheck(player))
                        {
                            NPC.ai[3] = 1;
                            NPC.localAI[0] = Math.Sign(NPC.Center.X - player.Center.X);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (Phase2Check())
                        return;

                    NPC.velocity = Vector2.Zero;

                    //if (FargoSoulsWorld.MasochistModeReal && NPC.ai[3] >= 300) //spear barrage
                    //{
                    //    if (NPC.ai[3] == 0)
                    //    {
                    //        NPC.ai[1] = 0;
                    //        NPC.ai[2] = Main.rand.NextFloat(MathHelper.TwoPi);
                    //        NPC.localAI[0] = player.Center.X;
                    //        NPC.localAI[1] = player.Center.Y;
                    //    }

                    //    const int spearsToMake = 18;
                    //    const int timeToFullSpears = 180;
                    //    const int timeGap = timeToFullSpears / spearsToMake;
                    //    if (NPC.ai[3] % timeGap == 0 && NPC.ai[3] < 300 + timeToFullSpears)
                    //    {
                    //        NPC.ai[2] += MathHelper.TwoPi / spearsToMake * ++NPC.ai[1];

                    //        Vector2 target = new Vector2(NPC.localAI[0], NPC.localAI[1]);
                    //        Vector2 spawnpos = target + 600 * NPC.ai[2].ToRotationVector2();

                    //        if (Main.netMode != NetmodeID.MultiplayerClient)
                    //        {

                    //        }
                    //    }
                    //}
                    //else
                    if (++NPC.ai[1] > 2) //boundary
                    {
                        SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                        NPC.ai[1] = 0;
                        //ai3 - 300 so that when attack ends, the projs will behave like at start of attack normally (straight streams)
                        NPC.ai[2] += FargoSoulsWorld.MasochistModeReal //maso uses true boundary
                                ? (float)Math.PI / 8 / 480 * (NPC.ai[3] - 300) * NPC.localAI[0]
                                : MathHelper.Pi / 77f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int max = FargoSoulsWorld.MasochistModeReal ? 5 : 4;
                            for (int i = 0; i < max; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0f, -7f).RotatedBy(NPC.ai[2] + MathHelper.TwoPi / max * i),
                                    ModContent.ProjectileType<MutantEye>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++NPC.ai[3] > (FargoSoulsWorld.MasochistModeReal ? 360 : 240))
                    {
                        P1NextAttackOrMasoOptions(NPC.ai[0]);
                    }
                    break;

                case 1:
                    PrepareMutantSword();
                    break;

                case 2:
                    MutantSword();
                    break;

                default:
                    break;
            }
        }

        void PrepareMutantSword()
        {
            if (NPC.ai[0] == 9 && Main.LocalPlayer.active && NPC.Distance(Main.LocalPlayer.Center) < 3000f && Main.expertMode)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<Purged>(), 2);

            //can alternate directions
            int sign = NPC.ai[0] != 9 && NPC.localAI[2] % 2 == 1 ? -1 : 1;

            if (NPC.ai[2] == 0) //move onscreen so player can see
            {
                if (!AliveCheck(player))
                    return;

                Vector2 targetPos = player.Center;
                targetPos.X += 420 * Math.Sign(NPC.Center.X - player.Center.X);
                targetPos.Y -= 210 * sign;
                Movement(targetPos, 1.2f);

                if ((++NPC.localAI[0] > 30 || FargoSoulsWorld.MasochistModeReal) && NPC.Distance(targetPos) < 64)
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    NPC.localAI[1] = Math.Sign(player.Center.X - NPC.Center.X);
                    float startAngle = MathHelper.PiOver4 * -NPC.localAI[1];
                    NPC.ai[2] = startAngle * -4f / 20 * sign; //travel the full arc over number of ticks
                    if (sign < 0)
                        startAngle += MathHelper.PiOver2 * -NPC.localAI[1];

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 offset = Vector2.UnitY.RotatedBy(startAngle) * -MUTANT_SWORD_SPACING;

                        void MakeSword(Vector2 pos, float spacing, float rotation = 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + pos, Vector2.Zero, ModContent.ProjectileType<MutantSword>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0f, Main.myPlayer, NPC.whoAmI, spacing);
                        }

                        for (int i = 0; i < MUTANT_SWORD_MAX; i++)
                        {
                            MakeSword(offset * i, MUTANT_SWORD_SPACING * i);
                        }

                        for (int i = -1; i <= 1; i += 2)
                        {
                            MakeSword(offset.RotatedBy(MathHelper.ToRadians(26.5f * i)), 60 * 3);
                            MakeSword(offset.RotatedBy(MathHelper.ToRadians(40 * i)), 60 * 4f);
                        }
                    }
                }
            }
            else
            {
                NPC.velocity = Vector2.Zero;

                FancyFireballs((int)(NPC.ai[1] / 90f * 60f));

                if (++NPC.ai[1] > 90)
                {
                    if (NPC.ai[0] != 9)
                        NPC.ai[0]++;

                    NPC.localAI[2]++; //progresses state in p1, counts swings in p2

                    Vector2 targetPos = player.Center;
                    targetPos.X -= 300 * NPC.ai[2];
                    NPC.velocity = (targetPos - NPC.Center) / 20;
                    NPC.ai[1] = 0;
                    NPC.netUpdate = true;
                }

                NPC.direction = NPC.spriteDirection = Math.Sign(NPC.localAI[1]);
            }
        }

        void MutantSword()
        {
            if (NPC.ai[0] == 9 && Main.LocalPlayer.active && NPC.Distance(Main.LocalPlayer.Center) < 3000f && Main.expertMode)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<Purged>(), 2);

            NPC.ai[3] += NPC.ai[2];
            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.localAI[1]);

            if (NPC.ai[1] == 20)
            {
                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                //moon chain explosions
                if ((FargoSoulsWorld.EternityMode && NPC.ai[0] != 9) || FargoSoulsWorld.MasochistModeReal)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Thunder") { Pitch = -0.5f }, NPC.Center);

                    float lookSign = Math.Sign(NPC.localAI[1]);
                    float arcSign = Math.Sign(NPC.ai[2]);
                    Vector2 offset = lookSign * Vector2.UnitX.RotatedBy(MathHelper.PiOver4 * arcSign);

                    const float length = MUTANT_SWORD_SPACING * MUTANT_SWORD_MAX / 2f;
                    Vector2 spawnPos = NPC.Center + length * offset;
                    Vector2 baseDirection = player.DirectionFrom(spawnPos);

                    const int max = 8; //spread
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 angle = baseDirection.RotatedBy(MathHelper.TwoPi / max * i);
                        float ai1 = i <= 2 || i == max - 2 ? 48 : 24;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), Vector2.Zero, ModContent.ProjectileType<Projectiles.Masomode.MoonLordMoonBlast>(),
                                FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0f, Main.myPlayer, MathHelper.WrapAngle(angle.ToRotation()), ai1);
                        }
                    }
                }
            }

            if (++NPC.ai[1] > 25)
            {
                if (NPC.ai[0] == 9)
                {
                    P1NextAttackOrMasoOptions(NPC.ai[0]);
                }
                else if (FargoSoulsWorld.MasochistModeReal && NPC.localAI[2] < 5 * endTimeVariance)
                {
                    NPC.ai[0]--;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.localAI[1] = 0;
                    NPC.netUpdate = true;
                }
                else
                {
                    ChooseNextAttack(13, 21, 24, 29, 31, 33, 37, 41, 42, 44);
                }
            }
        }

        #endregion

        #region p2

        void Phase2Transition()
        {
            NPC.velocity *= 0.9f;
            NPC.dontTakeDamage = true;

            if (NPC.buffType[0] != 0)
                NPC.DelBuff(0);

            EModeSpecialEffects();

            if (NPC.ai[2] == 0)
            {
                if (NPC.ai[1] < 60 && !Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;
            }
            else
            {
                NPC.velocity = Vector2.Zero;
            }

            if (NPC.ai[1] < 240)
            {
                //make you stop attacking
                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && NPC.Distance(Main.LocalPlayer.Center) < 3000)
                {
                    Main.LocalPlayer.controlUseItem = false;
                    Main.LocalPlayer.controlUseTile = false;
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = true;
                }
            }

            if (NPC.ai[1] == 0)
            {
                FargoSoulsUtil.ClearAllProjectiles(2, NPC.whoAmI);

                if (FargoSoulsWorld.EternityMode)
                {
                    DramaticTransition(false, NPC.ai[2] == 0);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        ritualProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0f, NPC.whoAmI);

                        if (FargoSoulsWorld.MasochistModeReal)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual2>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual3>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantRitual4>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                    }
                }
            }
            else if (NPC.ai[1] == 150)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), 0, 0f, Main.myPlayer, 5);
                    //Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -22);
                }

                if (FargoSoulsWorld.EternityMode && FargoSoulsWorld.skipMutantP1 <= 10)
                {
                    FargoSoulsWorld.skipMutantP1++;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }

                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 9f;
                }
                //if (player.GetModPlayer<FargoSoulsPlayer>().TerrariaSoul) EdgyBossText("Hand it over. That thing, your soul toggles.");
            }
            else if (NPC.ai[1] > 150)
            {
                NPC.localAI[3] = 3;
            }

            if (++NPC.ai[1] > 270)
            {
                if (FargoSoulsWorld.EternityMode)
                {
                    NPC.life = NPC.lifeMax;
                    NPC.ai[0] = Main.rand.Next(new int[] { 11, 13, 16, 19, 20, 21, 24, 26, 29, 35, 37, 39, 42 }); //force a random choice
                }
                else
                {
                    NPC.ai[0]++;
                }
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                //NPC.TargetClosest();
                NPC.netUpdate = true;

                attackHistory.Enqueue(NPC.ai[0]);
            }
        }

        void ApproachForNextAttackP2()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 300;
            if (NPC.Distance(targetPos) > 50 && ++NPC.ai[2] < 180)
            {
                Movement(targetPos, 0.8f);
            }
            else
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = player.DirectionTo(NPC.Center).ToRotation();
                NPC.ai[3] = (float)Math.PI / 10f;
                NPC.localAI[0] = 0;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                if (player.Center.X < NPC.Center.X)
                    NPC.ai[3] *= -1;
            }
        }

        void VoidRaysP2()
        {
            NPC.velocity = Vector2.Zero;
            if (--NPC.ai[1] < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(2, 0).RotatedBy(NPC.ai[2]), ModContent.ProjectileType<MutantMark1>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                NPC.ai[1] = 3;
                NPC.ai[2] += NPC.ai[3];

                if (NPC.localAI[0]++ == 20 || NPC.localAI[0] == 40)
                {
                    NPC.netUpdate = true;
                    NPC.ai[2] -= NPC.ai[3] / (FargoSoulsWorld.MasochistModeReal ? 3 : 2);

                    if ((NPC.localAI[0] == 21 && endTimeVariance > 0.75f) //sometimes skip to end
                    || (NPC.localAI[0] == 41 && endTimeVariance < 0.25f))
                        NPC.localAI[0] = 60;
                }
                else if (NPC.localAI[0] >= 60)
                {
                    ChooseNextAttack(13, 19, 21, 24, 31, 39, 41, 42);
                }
            }
        }

        void PrepareSpearDashPredictiveP2()
        {
            if (NPC.ai[3] == 0)
            {
                if (!AliveCheck(player))
                    return;
                NPC.ai[3] = 1;
                //NPC.velocity = NPC.DirectionFrom(player.Center) * NPC.velocity.Length();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 180); // + 60);
            }

            if (++NPC.ai[1] > 180)
            {
                if (!AliveCheck(player))
                    return;
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[3] = 0;
                //NPC.TargetClosest();
            }

            Vector2 targetPos = player.Center;
            targetPos.Y += 400f * Math.Sign(NPC.Center.Y - player.Center.Y); //can be above or below
            Movement(targetPos, 0.7f, false);
            if (NPC.Distance(player.Center) < 200)
                Movement(NPC.Center + NPC.DirectionFrom(player.Center), 1.4f);
        }

        void SpearDashPredictiveP2()
        {
            if (NPC.localAI[1] == 0) //max number of attacks
            {
                if (FargoSoulsWorld.EternityMode)
                    NPC.localAI[1] = Main.rand.Next(FargoSoulsWorld.MasochistModeReal ? 3 : 5, 9);
                else
                    NPC.localAI[1] = 5;
            }

            if (NPC.ai[1] == 0) //telegraph
            {
                if (!AliveCheck(player))
                    return;

                if (NPC.ai[2] == NPC.localAI[1] - 1)
                {
                    if (NPC.Distance(player.Center) > 450) //get closer for last dash
                    {
                        Movement(player.Center, 0.6f);
                        return;
                    }

                    NPC.velocity *= 0.75f; //try not to bump into player
                }

                if (NPC.ai[2] < NPC.localAI[1])
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 55, NPC.whoAmI);

                    if (NPC.ai[2] == NPC.localAI[1] - 1)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 4);
                    }
                }
            }

            NPC.velocity *= 0.9f;

            if (NPC.ai[1] < 55) //track player up until just before dash
            {
                NPC.localAI[0] = NPC.DirectionTo(player.Center + player.velocity * 30f).ToRotation();
            }

            int endTime = 60;
            if (NPC.ai[2] == NPC.localAI[1] - 1)
                endTime = 80;
            if (FargoSoulsWorld.MasochistModeReal && (NPC.ai[2] == 0 || NPC.ai[2] >= NPC.localAI[1]))
                endTime = 0;
            if (++NPC.ai[1] > endTime)
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[3] = 0;
                if (++NPC.ai[2] > NPC.localAI[1])
                {
                    ChooseNextAttack(16, 19, 20, 26, 29, 31, 33, 39, 42, 44, 45);
                }
                else
                {
                    NPC.velocity = NPC.localAI[0].ToRotationVector2() * 45f;
                    float spearAi = 0f;
                    if (NPC.ai[2] == NPC.localAI[1])
                        spearAi = -2f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, spearAi);
                    }
                }
                NPC.localAI[0] = 0;
            }
        }

        void WhileDashingP2()
        {
            NPC.direction = NPC.spriteDirection = Math.Sign(NPC.velocity.X);
            if (++NPC.ai[1] > 30)
            {
                if (!AliveCheck(player))
                    return;
                NPC.netUpdate = true;
                NPC.ai[0]--;
                NPC.ai[1] = 0;

                //quickly bounce back towards player
                if (NPC.ai[0] == 14 && NPC.ai[2] == NPC.localAI[1] - 1 && NPC.Distance(player.Center) > 450)
                    NPC.velocity = NPC.DirectionTo(player.Center) * 16f;
            }
        }

        void BoundaryBulletHellP2()
        {
            NPC.velocity = Vector2.Zero;
            if (NPC.localAI[0] == 0)
            {
                NPC.localAI[0] = Math.Sign(NPC.Center.X - player.Center.X);
                //if (FargoSoulsWorld.MasochistMode) NPC.ai[2] = NPC.DirectionTo(player.Center).ToRotation(); //starting rotation offset to avoid hitting at close range
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -2);
            }
            if (NPC.ai[3] > 60 && ++NPC.ai[1] > 2)
            {
                SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                NPC.ai[1] = 0;
                NPC.ai[2] += (float)Math.PI / 8 / 480 * NPC.ai[3] * NPC.localAI[0];
                if (NPC.ai[2] > (float)Math.PI)
                    NPC.ai[2] -= (float)Math.PI * 2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int max = 4;
                    if (FargoSoulsWorld.EternityMode)
                        max += 1;
                    if (FargoSoulsWorld.MasochistModeReal)
                        max += 1;
                    for (int i = 0; i < max; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0f, -6f).RotatedBy(NPC.ai[2] + Math.PI * 2 / max * i),
                            ModContent.ProjectileType<MutantEye>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }
                }
            }

            int endTime = 360 + 60 + (int)(240 * 2 * (endTimeVariance - 0.33f));
            if (++NPC.ai[3] > endTime)
            {
                ChooseNextAttack(11, 13, 19, 20, 21, 24, FargoSoulsWorld.MasochistModeReal ? 31 : 26, 33, 41, 44);
            }
        }

        void PillarDunk()
        {
            if (!AliveCheck(player))
                return;

            int pillarAttackDelay = 60;

            if (NPC.ai[2] == 0 && NPC.ai[3] == 0) //target one corner of arena
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient) //spawn cultists
                {
                    void Clone(float ai1, float ai2, float ai3) => FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<MutantIllusion>(), NPC.whoAmI, NPC.whoAmI, ai1, ai2, ai3);
                    Clone(-1, 1, pillarAttackDelay * 4);
                    Clone(1, -1, pillarAttackDelay * 2);
                    Clone(1, 1, pillarAttackDelay * 3);
                    if (FargoSoulsWorld.MasochistModeReal)
                        Clone(1, 1, pillarAttackDelay * 6);
                }

                NPC.netUpdate = true;
                NPC.ai[2] = NPC.Center.X;
                NPC.ai[3] = NPC.Center.Y;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<MutantRitual>() && Main.projectile[i].ai[1] == NPC.whoAmI)
                    {
                        NPC.ai[2] = Main.projectile[i].Center.X;
                        NPC.ai[3] = Main.projectile[i].Center.Y;
                        break;
                    }
                }

                Vector2 offset = 1000f * Vector2.UnitX.RotatedBy(MathHelper.ToRadians(45));
                if (Main.rand.NextBool()) //always go to a side player isn't in but pick a way to do it randomly
                {
                    if (player.Center.X > NPC.ai[2])
                        offset.X *= -1;
                    if (Main.rand.NextBool())
                        offset.Y *= -1;
                }
                else
                {
                    if (Main.rand.NextBool())
                        offset.X *= -1;
                    if (player.Center.Y > NPC.ai[3])
                        offset.Y *= -1;
                }

                NPC.localAI[1] = NPC.ai[2]; //for illusions
                NPC.localAI[2] = NPC.ai[3];

                NPC.ai[2] = offset.Length();
                NPC.ai[3] = offset.ToRotation();
            }

            Vector2 targetPos = player.Center;
            targetPos.X += NPC.Center.X < player.Center.X ? -700 : 700;
            targetPos.Y += NPC.ai[1] < 240 ? 400 : 150;
            if (NPC.Distance(targetPos) > 50)
                Movement(targetPos, 1f);

            int endTime = 240 + pillarAttackDelay * 4 + 60;
            if (FargoSoulsWorld.MasochistModeReal)
                endTime += pillarAttackDelay * 2;

            NPC.localAI[0] = endTime - NPC.ai[1]; //for pillars to know remaining duration
            NPC.localAI[0] += 60f + 60f * (1f - NPC.ai[1] / endTime); //staggered despawn

            if (++NPC.ai[1] > endTime)
            {
                ChooseNextAttack(11, 13, 20, 21, 26, 33, 41, 44);
            }
            else if (NPC.ai[1] == pillarAttackDelay)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -5,
                        ModContent.ProjectileType<MutantPillar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0, Main.myPlayer, 3, NPC.whoAmI);
                }
            }
            else if (FargoSoulsWorld.MasochistModeReal && NPC.ai[1] == pillarAttackDelay * 5)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -5,
                        ModContent.ProjectileType<MutantPillar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0, Main.myPlayer, 1, NPC.whoAmI);
                }
            }
        }

        void EOCStarSickles()
        {
            if (!AliveCheck(player))
                return;

            if (NPC.ai[1] == 0)
            {
                float ai1 = 0;

                if (FargoSoulsWorld.MasochistModeReal) //begin attack much faster
                {
                    ai1 = 30;
                    NPC.ai[1] = 30;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<MutantEyeOfCthulhu>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target, ai1);
                    if (FargoSoulsWorld.MasochistModeReal && p != Main.maxProjectiles)
                        Main.projectile[p].timeLeft -= 30;
                }
            }

            if (NPC.ai[1] < 120) //stop tracking when eoc begins attacking, this locks arena in place
            {
                NPC.ai[2] = player.Center.X;
                NPC.ai[3] = player.Center.Y;
            }

            /*if (NPC.Distance(player.Center) < 200)
            {
                Movement(NPC.Center + 200 * NPC.DirectionFrom(player.Center), 0.9f);
            }
            else
            {*/
            Vector2 targetPos = new Vector2(NPC.ai[2], NPC.ai[3]);
            targetPos += NPC.DirectionFrom(targetPos).RotatedBy(MathHelper.ToRadians(-5)) * 450f;
            if (NPC.Distance(targetPos) > 50)
                Movement(targetPos, 0.25f);
            //}

            if (++NPC.ai[1] > 450)
            {
                ChooseNextAttack(11, 13, 16, 21, 26, 29, 31, 33, 35, 37, 41, 44, 45);
            }

            /*if (Math.Abs(targetPos.X - player.Center.X) < 150) //avoid crossing up player
            {
                targetPos.X = player.Center.X + 150 * Math.Sign(targetPos.X - player.Center.X);
                Movement(targetPos, 0.3f);
            }
            if (NPC.Distance(targetPos) > 50)
            {
                Movement(targetPos, 0.5f);
            }

            if (--NPC.ai[1] < 0)
            {
                NPC.ai[1] = 60;
                if (++NPC.ai[2] > (FargoSoulsWorld.MasochistMode ? 3 : 1))
                {
                    //float[] options = { 13, 19, 21, 24, 26, 31, 33, 40 }; NPC.ai[0] = options[Main.rand.Next(options.Length)];
                    NPC.ai[0]++;
                    NPC.ai[2] = 0;
                    NPC.TargetClosest();
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        for (int i = 0; i < 8; i++)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 4 * i) * 10f, ModContent.ProjectileType<MutantScythe1>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer, NPC.whoAmI);
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                }
                NPC.netUpdate = true;
                break;
            }*/
        }

        void PrepareSpearDashDirectP2()
        {
            if (NPC.ai[3] == 0)
            {
                if (!AliveCheck(player))
                    return;
                NPC.ai[3] = 1;
                //NPC.velocity = NPC.DirectionFrom(player.Center) * NPC.velocity.Length();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearSpin>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 180);// + (FargoSoulsWorld.MasochistMode ? 10 : 20));
            }

            if (++NPC.ai[1] > 180)
            {
                if (!AliveCheck(player))
                    return;
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[3] = 0;
                //NPC.TargetClosest();
            }

            Vector2 targetPos = player.Center;
            targetPos.Y += 450f * Math.Sign(NPC.Center.Y - player.Center.Y); //can be above or below
            Movement(targetPos, 0.7f, false);
            if (NPC.Distance(player.Center) < 200)
                Movement(NPC.Center + NPC.DirectionFrom(player.Center), 1.4f);
        }

        void SpearDashDirectP2()
        {
            NPC.velocity *= 0.9f;

            if (NPC.localAI[1] == 0) //max number of attacks
            {
                if (FargoSoulsWorld.EternityMode)
                    NPC.localAI[1] = Main.rand.Next(FargoSoulsWorld.MasochistModeReal ? 3 : 5, 9);
                else
                    NPC.localAI[1] = 5;
            }

            if (++NPC.ai[1] > (FargoSoulsWorld.EternityMode ? 5 : 20))
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                if (++NPC.ai[2] > NPC.localAI[1])
                {
                    if (FargoSoulsWorld.MasochistModeReal)
                        ChooseNextAttack(11, 13, 16, 19, 20, 31, 33, 35, 39, 42, 44);
                    else
                        ChooseNextAttack(11, 16, 26, 29, 31, 35, 37, 39, 42, 44);
                }
                else
                {
                    NPC.velocity = NPC.DirectionTo(player.Center) * (FargoSoulsWorld.MasochistModeReal ? 60f : 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.Normalize(NPC.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearDash>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
                    }
                }
            }
        }

        void SpawnDestroyersForPredictiveThrow()
        {
            if (!AliveCheck(player))
                return;

            if (FargoSoulsWorld.EternityMode)
            {
                Vector2 targetPos = player.Center + NPC.DirectionFrom(player.Center) * 500;
                if (Math.Abs(targetPos.X - player.Center.X) < 150) //avoid crossing up player
                {
                    targetPos.X = player.Center.X + 150 * Math.Sign(targetPos.X - player.Center.X);
                    Movement(targetPos, 0.3f);
                }
                if (NPC.Distance(targetPos) > 50)
                {
                    Movement(targetPos, 0.9f);
                }
            }
            else
            {
                Vector2 targetPos = player.Center;
                targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
                if (NPC.Distance(targetPos) > 50)
                {
                    Movement(targetPos, 0.4f);
                }
            }

            if (NPC.localAI[1] == 0) //max number of attacks
            {
                if (FargoSoulsWorld.EternityMode)
                    NPC.localAI[1] = Main.rand.Next(FargoSoulsWorld.MasochistModeReal ? 3 : 5, 9);
                else
                    NPC.localAI[1] = 5;
            }

            if (++NPC.ai[1] > 60)
            {
                NPC.netUpdate = true;
                NPC.ai[1] = 30;
                int cap = 3;
                if (FargoSoulsWorld.EternityMode)
                {
                    cap += 2;
                }
                if (FargoSoulsWorld.MasochistModeReal)
                {
                    cap += 2;
                    NPC.ai[1] += 15; //faster
                }

                if (++NPC.ai[2] > cap)
                {
                    //NPC.TargetClosest();
                    NPC.ai[0]++;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient) //spawn worm
                    {
                        Vector2 vel = NPC.DirectionFrom(player.Center).RotatedByRandom(MathHelper.ToRadians(120)) * 10f;
                        float ai1 = 0.8f + 0.4f * NPC.ai[2] / 5f;
                        if (FargoSoulsWorld.MasochistModeReal)
                            ai1 += 0.4f;
                        int current = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerHead>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target, ai1);
                        //timeleft: remaining duration of this case + duration of next case + extra delay after + successive death
                        Main.projectile[current].timeLeft = 30 * (cap - (int)NPC.ai[2]) + 60 * (int)NPC.localAI[1] + 30 + (int)NPC.ai[2] * 6;
                        int max = Main.rand.Next(8, 19);
                        for (int i = 0; i < max; i++)
                            current = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerBody>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, Main.projectile[current].identity);
                        int previous = current;
                        current = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantDestroyerTail>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, Main.projectile[current].identity);
                        Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                        Main.projectile[previous].netUpdate = true;
                    }
                }
            }
        }

        void SpearTossPredictiveP2()
        {
            if (!AliveCheck(player))
                return;

            Vector2 targetPos = player.Center;
            targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
            if (NPC.Distance(targetPos) > 25)
                Movement(targetPos, 0.8f);

            if (++NPC.ai[1] > 60)
            {
                NPC.netUpdate = true;
                NPC.ai[1] = 0;
                bool shouldAttack = true;
                if (++NPC.ai[2] > NPC.localAI[1])
                {
                    shouldAttack = false;
                    if (FargoSoulsWorld.MasochistModeReal)
                        ChooseNextAttack(11, 19, 20, 29, 31, 33, 35, 37, 39, 42, 44, 45);
                    else
                        ChooseNextAttack(11, 19, 20, 26, 26, 26, 29, 31, 33, 35, 37, 39, 42, 44);
                }

                if ((shouldAttack || FargoSoulsWorld.MasochistModeReal) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.DirectionTo(player.Center + player.velocity * 30f) * 30f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target, 1f);
                }
            }
            else if (NPC.ai[1] == 1 && (NPC.ai[2] < NPC.localAI[1] || FargoSoulsWorld.MasochistModeReal) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(player.Center + player.velocity * 30f), ModContent.ProjectileType<MutantDeathrayAim>(), 0, 0f, Main.myPlayer, 60f, NPC.whoAmI);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 2);
            }
        }

        void PrepareMechRayFan()
        {
            if (NPC.ai[1] == 0)
            {
                if (!AliveCheck(player))
                    return;

                if (FargoSoulsWorld.MasochistModeReal)
                    NPC.ai[1] = 31; //skip the pause, skip the telegraph
            }

            if (NPC.ai[1] == 30)
            {
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center); //eoc roar
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, NPCID.Retinazer);
            }

            Vector2 targetPos;
            if (NPC.ai[1] < 30)
            {
                targetPos = player.Center + NPC.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(15)) * 500f;
                if (NPC.Distance(targetPos) > 50)
                    Movement(targetPos, 0.3f);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 12f;
                }

                targetPos = player.Center;
                targetPos.X += 600 * (NPC.Center.X < targetPos.X ? -1 : 1);
                Movement(targetPos, 1.2f, false);
            }

            if (++NPC.ai[1] > 150 || (FargoSoulsWorld.MasochistModeReal && NPC.Distance(targetPos) < 64))
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                //NPC.TargetClosest();
            }
        }

        void MechRayFan()
        {
            NPC.velocity = Vector2.Zero;

            if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = Main.rand.NextBool() ? -1 : 1; //randomly aim either up or down
            }

            if (NPC.ai[3] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int max = 7;
                for (int i = 0; i <= max; i++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy(NPC.ai[2] * i * MathHelper.Pi / max) * 6; //rotate initial velocity of telegraphs by 180 degrees depending on velocity of lasers
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + dir, Vector2.Zero, ModContent.ProjectileType<MutantGlowything>(), 0, 0f, Main.myPlayer, dir.ToRotation(), NPC.whoAmI);
                }
            }

            int endTime = 60 + 180 + 150;

            if (NPC.ai[3] > (FargoSoulsWorld.MasochistModeReal ? 45 : 60) && NPC.ai[3] < 60 + 180 && ++NPC.ai[1] > 10)
            {
                NPC.ai[1] = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float rotation = MathHelper.ToRadians(245) * NPC.ai[2] / 80f;
                    int timeBeforeAttackEnds = endTime - (int)NPC.ai[3];

                    void SpawnRay(Vector2 pos, float angleInDegrees, float turnRotation)
                    {
                        int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, MathHelper.ToRadians(angleInDegrees).ToRotationVector2(),
                            ModContent.ProjectileType<MutantDeathray3>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, turnRotation, NPC.whoAmI);
                        if (p != Main.maxProjectiles && Main.projectile[p].timeLeft > timeBeforeAttackEnds)
                            Main.projectile[p].timeLeft = timeBeforeAttackEnds;
                    };

                    SpawnRay(NPC.Center, 8 * NPC.ai[2], rotation);
                    SpawnRay(NPC.Center, -8 * NPC.ai[2] + 180, -rotation);

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        Vector2 spawnPos = NPC.Center + NPC.ai[2] * -1200 * Vector2.UnitY;
                        SpawnRay(spawnPos, 8 * NPC.ai[2] + 180, rotation);
                        SpawnRay(spawnPos, -8 * NPC.ai[2], -rotation);
                    }
                }
            }

            void SpawnPrime(float varianceInDegrees, float rotationInDegrees)
            {
                SoundEngine.PlaySound(SoundID.Item21, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float spawnOffset = (Main.rand.NextBool() ? -1 : 1) * Main.rand.NextFloat(1400, 1800);
                    float maxVariance = MathHelper.ToRadians(varianceInDegrees);
                    Vector2 aimPoint = NPC.Center - Vector2.UnitY * NPC.ai[2] * 600;
                    Vector2 spawnPos = aimPoint + spawnOffset * Vector2.UnitY.RotatedByRandom(maxVariance).RotatedBy(MathHelper.ToRadians(rotationInDegrees));
                    Vector2 vel = 32f * Vector2.Normalize(aimPoint - spawnPos);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<MutantGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0f, Main.myPlayer);
                }
            }

            if (NPC.ai[3] < 180 && ++NPC.localAI[0] > 1)
            {
                NPC.localAI[0] = 0;
                SpawnPrime(15, 0);
            }

            //if (FargoSoulsWorld.MasochistModeReal && NPC.ai[3] == endTime - 40)
            //{
            //    Vector2 aimPoint = NPC.Center - Vector2.UnitY * NPC.ai[2] * 600;
            //    for (int i = -3; i <= 3; i++)
            //    {
            //        Vector2 spawnPos = aimPoint + 200 * i * Vector2.UnitX;
            //        if (Main.netMode != NetmodeID.MultiplayerClient)
            //            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantReticle2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
            //    }
            //}

            if (++NPC.ai[3] > endTime)
            {
                //if (FargoSoulsWorld.MasochistModeReal) //maso prime jumpscare after rays
                //{
                //    for (int i = 0; i < 60; i++)
                //        SpawnPrime(45, 90);
                //}

                if (FargoSoulsWorld.EternityMode) //use full moveset
                {
                    ChooseNextAttack(11, 13, 16, 19, 21, 24, 29, 31, 33, 35, 37, 39, 41, 42, 45);
                }
                else
                {
                    NPC.ai[0] = 11;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                }
                NPC.netUpdate = true;
            }
        }

        void PrepareFishron1()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = new Vector2(player.Center.X, player.Center.Y + 600 * Math.Sign(NPC.Center.Y - player.Center.Y));
            Movement(targetPos, 1.4f, false);

            if (NPC.ai[1] == 0) //always dash towards same side i started on
                NPC.ai[2] = Math.Sign(NPC.Center.X - player.Center.X);

            if (++NPC.ai[1] > 60 || NPC.Distance(targetPos) < 64) //dive here
            {
                NPC.velocity.X = 30f * NPC.ai[2];
                NPC.velocity.Y = 0f;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        void SpawnFishrons()
        {
            NPC.velocity *= 0.97f;
            if (NPC.ai[1] == 0)
            {
                NPC.ai[2] = Main.rand.NextBool() ? 1 : 0;
            }
            const int fishronDelay = 3;
            int maxFishronSets = FargoSoulsWorld.MasochistModeReal ? 3 : 2;
            if (NPC.ai[1] % fishronDelay == 0 && NPC.ai[1] <= fishronDelay * maxFishronSets)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int j = -1; j <= 1; j += 2) //to both sides of player
                    {
                        int max = (int)NPC.ai[1] / fishronDelay;
                        for (int i = -max; i <= max; i++) //fan of fishron
                        {
                            if (Math.Abs(i) != max) //only spawn the outmost ones
                                continue;
                            float spread = MathHelper.Pi / 3 / (maxFishronSets + 1);
                            Vector2 offset = NPC.ai[2] == 0 ? Vector2.UnitY.RotatedBy(spread * i) * -450f * j : Vector2.UnitX.RotatedBy(spread * i) * 475f * j;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantFishron>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, offset.X, offset.Y);
                        }
                    }
                }
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 135, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 12f;
                }
            }

            if (++NPC.ai[1] > (FargoSoulsWorld.MasochistModeReal ? 60 : 120))
            {
                ChooseNextAttack(13, 19, 20, 21, FargoSoulsWorld.MasochistModeReal ? 44 : 26, 31, 31, 31, 33, 35, 39, 41, 42, 44);
            }
        }

        void PrepareTrueEyeDiveP2()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = player.Center;
            targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
            targetPos.Y += 400;
            Movement(targetPos, 1.2f);

            //dive here
            if (++NPC.ai[1] > 60)
            {
                NPC.velocity.X = 30f * (NPC.position.X < player.position.X ? 1 : -1);
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y *= -1;
                NPC.velocity.Y *= 0.3f;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
            }
        }

        void PrepareNuke()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = player.Center;
            targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
            targetPos.Y -= 400;
            Movement(targetPos, 1.2f, false);
            if (++NPC.ai[1] > 60)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float gravity = 0.2f;
                    float time = FargoSoulsWorld.MasochistModeReal ? 120f : 180f;
                    Vector2 distance = player.Center - NPC.Center;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance, ModContent.ProjectileType<MutantNuke>(), FargoSoulsWorld.MasochistModeReal ? FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f) : 0, 0f, Main.myPlayer, gravity);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantFishronRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0f, Main.myPlayer, NPC.whoAmI);
                }
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                
                if (Math.Sign(player.Center.X - NPC.Center.X) == Math.Sign(NPC.velocity.X))
                    NPC.velocity.X *= -1f;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y *= -1f;
                NPC.velocity.Normalize();
                NPC.velocity *= 3f;

                NPC.netUpdate = true;
                //NPC.TargetClosest();
            }
        }

        void Nuke()
        {
            if (!AliveCheck(player))
                return;

            if (FargoSoulsWorld.MasochistModeReal)
            {
                Vector2 target = NPC.Bottom.Y < player.Top.Y
                    ? player.Center + 300f * Vector2.UnitX * Math.Sign(NPC.Center.X - player.Center.X)
                    : NPC.Center + 30 * NPC.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(60) * Math.Sign(player.Center.X - NPC.Center.X));
                Movement(target, 0.1f);
                if (NPC.velocity.Length() > 2f)
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * 2f;
            }

            if (NPC.ai[1] > (FargoSoulsWorld.MasochistModeReal ? 120 : 180))
            {
                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 safeZone = NPC.Center;
                    safeZone.Y -= 100;
                    const float safeRange = 150 + 200;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(1200, 1200);
                        if (Vector2.Distance(safeZone, spawnPos) < safeRange)
                        {
                            Vector2 directionOut = spawnPos - safeZone;
                            directionOut.Normalize();
                            spawnPos = safeZone + directionOut * Main.rand.NextFloat(safeRange, 1200);
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantBomb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3f), 0f, Main.myPlayer);
                    }
                }
            }

            if (++NPC.ai[1] > 360 + 360 * endTimeVariance)
            {
                ChooseNextAttack(11, 13, 16, 19, 24, FargoSoulsWorld.MasochistModeReal ? 26 : 29, 31, 35, 37, 39, 41, 42);
            }

            if (NPC.ai[1] > 45)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    offset.Y -= 100;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * 150);
                    offset.Y += (float)(Math.Cos(angle) * 150);
                    Dust dust = Main.dust[Dust.NewDust(NPC.Center + offset - new Vector2(4, 4), 0, 0, 229, 0, 0, 100, Color.White, 1.5f)];
                    dust.velocity = NPC.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                }
            }
        }

        void PrepareSlimeRain()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = player.Center;
            targetPos.X += 700 * (NPC.Center.X < targetPos.X ? -1 : 1);
            targetPos.Y += 200;
            Movement(targetPos, 2f);

            if (++NPC.ai[2] > 30 || (FargoSoulsWorld.MasochistModeReal && NPC.Distance(targetPos) < 64))
            {
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                NPC.netUpdate = true;
                //NPC.TargetClosest();
            }
        }

        void SlimeRain()
        {
            if (NPC.ai[3] == 0)
            {
                NPC.ai[3] = 1;
                //Main.NewText(NPC.position.Y);
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSlimeRain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI);
            }

            if (NPC.ai[1] == 0) //telegraphs for where slime will fall
            {
                bool first = NPC.localAI[0] == 0;
                NPC.localAI[0] = Main.rand.Next(5, 9) * 120;
                if (first) //always start on the same side as the player
                {
                    if (player.Center.X < NPC.Center.X && NPC.localAI[0] > 1200)
                        NPC.localAI[0] -= 1200;
                    else if (player.Center.X > NPC.Center.X && NPC.localAI[0] < 1200)
                        NPC.localAI[0] += 1200;
                }
                else //after that, always be on opposite side from player
                {
                    if (player.Center.X < NPC.Center.X && NPC.localAI[0] < 1200)
                        NPC.localAI[0] += 1200;
                    else if (player.Center.X > NPC.Center.X && NPC.localAI[0] > 1200)
                        NPC.localAI[0] -= 1200;
                }
                NPC.localAI[0] += 60;

                Vector2 basePos = NPC.Center;
                basePos.X -= 1200;
                for (int i = -360; i <= 2760; i += 120) //spawn telegraphs
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (i + 60 == (int)NPC.localAI[0])
                            continue;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), basePos.X + i + 60, basePos.Y, 0f, 0f, ModContent.ProjectileType<MutantReticle>(), 0, 0f, Main.myPlayer);
                    }
                }

                if (FargoSoulsWorld.MasochistModeReal)
                {
                    NPC.ai[1] += 20; //less startup
                    NPC.ai[2] += 20; //stay synced
                }
            }

            if (NPC.ai[1] > 120 && NPC.ai[1] % 5 == 0) //rain down slime balls
            {
                SoundEngine.PlaySound(SoundID.Item34, player.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    void Slime(Vector2 pos, float off, Vector2 vel)
                    {
                        //dont flip in maso wave 3
                        int flip = FargoSoulsWorld.MasochistModeReal && NPC.ai[2] < 180 * 2 && Main.rand.NextBool() ? -1 : 1;
                        Vector2 spawnPos = pos + off * Vector2.UnitY * flip;
                        float ai0 = FargoSoulsUtil.ProjectileExists(ritualProj, ModContent.ProjectileType<MutantRitual>()) == null ? 0f : NPC.Distance(Main.projectile[ritualProj].Center);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel * flip, ModContent.ProjectileType<MutantSlimeBall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, ai0);
                    }

                    Vector2 basePos = NPC.Center;
                    basePos.X -= 1200;
                    float yOffset = -1300;

                    const float safeRange = 110;
                    for (int i = -360; i <= 2760; i += 75)
                    {
                        float xOffset = i + Main.rand.Next(75);
                        if (Math.Abs(xOffset - NPC.localAI[0]) < safeRange) //dont fall over safespot
                            continue;

                        Vector2 spawnPos = basePos;
                        spawnPos.X += xOffset;
                        Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(15f, 20f);

                        Slime(spawnPos, yOffset, velocity);
                    }

                    //spawn right on safespot borders
                    Slime(basePos + Vector2.UnitX * (NPC.localAI[0] + safeRange), yOffset, Vector2.UnitY * 20f);
                    Slime(basePos + Vector2.UnitX * (NPC.localAI[0] - safeRange), yOffset, Vector2.UnitY * 20f);
                }
            }
            if (++NPC.ai[1] > 180)
            {
                if (!AliveCheck(player))
                    return;
                NPC.ai[1] = 0;
            }

            const int masoMovingRainAttackTime = 180 * 3 - 60;
            if (FargoSoulsWorld.MasochistModeReal && NPC.ai[1] == 120 && NPC.ai[2] < masoMovingRainAttackTime && Main.rand.NextBool(3))
                NPC.ai[2] = masoMovingRainAttackTime;

            NPC.velocity = Vector2.Zero;

            const int timeToMove = 240;
            if (FargoSoulsWorld.MasochistModeReal)
            {
                if (NPC.ai[2] == masoMovingRainAttackTime)
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                if (NPC.ai[2] > masoMovingRainAttackTime + 30)
                {
                    if (NPC.ai[1] > 170) //let the balls keep falling
                        NPC.ai[1] -= 30;

                    if (NPC.localAI[1] == 0) //direction to move safespot towards
                    {
                        float safespotX = NPC.Center.X - 1200f + NPC.localAI[0];
                        NPC.localAI[1] = Math.Sign(NPC.Center.X - safespotX);
                    }

                    //move the safespot
                    NPC.localAI[0] += 1000f / timeToMove * NPC.localAI[1];
                }
            }

            int endTime = 180 * 3;
            if (FargoSoulsWorld.MasochistModeReal)
                endTime += timeToMove + (int)(120 * endTimeVariance) - 30;
            if (++NPC.ai[2] > endTime)
            {
                ChooseNextAttack(11, 16, 19, 20, FargoSoulsWorld.MasochistModeReal ? 26 : 29, 31, 33, 37, 39, 41, 42, 45);
            }
        }

        void PrepareFishron2()
        {
            if (!AliveCheck(player))
                return;

            Vector2 targetPos = player.Center;
            targetPos.X += 400 * (NPC.Center.X < targetPos.X ? -1 : 1);
            targetPos.Y -= 400;
            Movement(targetPos, 0.9f);

            if (++NPC.ai[1] > 60 || (FargoSoulsWorld.MasochistModeReal && NPC.Distance(targetPos) < 32)) //dive here
            {
                NPC.velocity.X = 35f * (NPC.position.X < player.position.X ? 1 : -1);
                NPC.velocity.Y = 10f;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.netUpdate = true;
                //NPC.TargetClosest();
            }
        }

        void PrepareOkuuSpheresP2()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 450;
            if (++NPC.ai[1] < 180 && NPC.Distance(targetPos) > 50)
            {
                Movement(targetPos, 0.8f);
            }
            else
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
            }
        }

        void OkuuSpheresP2()
        {
            NPC.velocity = Vector2.Zero;

            int endTime = 420 + (int)(360 * (endTimeVariance - 0.33f));

            if (++NPC.ai[1] > 10 && NPC.ai[3] > 60 && NPC.ai[3] < endTime - 60)
            {
                NPC.ai[1] = 0;
                float rotation = MathHelper.ToRadians(60) * (NPC.ai[3] - 45) / 240 * NPC.ai[2];
                int max = FargoSoulsWorld.MasochistModeReal ? 10 : 9;
                float speed = FargoSoulsWorld.MasochistModeReal ? 11f : 10f;
                SpawnSphereRing(max, speed, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), -1f, rotation);
                SpawnSphereRing(max, speed, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1f, rotation);
            }

            if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = Main.rand.NextBool() ? -1 : 1;
                NPC.ai[3] = Main.rand.NextFloat((float)Math.PI * 2);
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -2);
            }

            if (++NPC.ai[3] > endTime)
            {
                ChooseNextAttack(13, 19, 20, FargoSoulsWorld.MasochistModeReal ? 13 : 26, FargoSoulsWorld.MasochistModeReal ? 44 : 33, 41, 44);
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        void SpearTossDirectP2()
        {
            if (!AliveCheck(player))
                return;

            if (NPC.ai[1] == 0)
            {
                NPC.localAI[0] = MathHelper.WrapAngle((NPC.Center - player.Center).ToRotation()); //remember initial angle offset

                //random max number of attacks
                if (FargoSoulsWorld.EternityMode)
                    NPC.localAI[1] = Main.rand.Next(FargoSoulsWorld.MasochistModeReal ? 3 : 5, 9);
                else
                    NPC.localAI[1] = 5;

                if (FargoSoulsWorld.MasochistModeReal)
                    NPC.localAI[1] += 3;
                NPC.localAI[2] = Main.rand.NextBool() ? -1 : 1; //pick a random rotation direction
                NPC.netUpdate = true;
            }

            //slowly rotate in full circle around player
            Vector2 targetPos = player.Center + 500f * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 300 * NPC.ai[3] * NPC.localAI[2] + NPC.localAI[0]);
            if (NPC.Distance(targetPos) > 25)
                Movement(targetPos, 0.6f);

            ++NPC.ai[3]; //for keeping track of how much time has actually passed (ai1 jumps around)

            void Attack()
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = NPC.DirectionTo(player.Center) * 30f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -Vector2.Normalize(vel), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.8f), 0f, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<MutantSpearThrown>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.target);
                }
            };

            if (++NPC.ai[1] > 180)
            {
                NPC.netUpdate = true;
                NPC.ai[1] = 150;

                bool shouldAttack = true;
                if (++NPC.ai[2] > NPC.localAI[1])
                {
                    ChooseNextAttack(11, 16, 19, 20, FargoSoulsWorld.MasochistModeReal ? 44 : 26, 31, 33, 35, 42, 44, 45);
                    shouldAttack = false;
                }
                
                if (shouldAttack || FargoSoulsWorld.MasochistModeReal)
                {
                    Attack();
                }
            }
            else if (FargoSoulsWorld.MasochistModeReal && NPC.ai[1] == 165)
            {
                Attack();
            }
            else if (NPC.ai[1] == 151)
            {
                if (NPC.ai[2] > 0 && (NPC.ai[2] < NPC.localAI[1] || FargoSoulsWorld.MasochistModeReal) && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, 1);
            }
            else if (NPC.ai[1] == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MutantSpearAim>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, -1);
            }
        }

        void PrepareTwinRangsAndCrystals()
        {
            if (!AliveCheck(player))
                return;
            Vector2 targetPos = player.Center;
            targetPos.X += 500 * (NPC.Center.X < targetPos.X ? -1 : 1);
            if (NPC.Distance(targetPos) > 50)
                Movement(targetPos, 0.8f);
            if (++NPC.ai[1] > 45)
            {
                NPC.netUpdate = true;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                //NPC.TargetClosest();
            }
        }

        void TwinRangsAndCrystals()
        {
            NPC.velocity = Vector2.Zero;

            if (NPC.ai[3] == 0)
            {
                NPC.localAI[0] = NPC.DirectionFrom(player.Center).ToRotation();

                if (!FargoSoulsWorld.MasochistModeReal && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * 525, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1f);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4) * 350, Vector2.Zero, ModContent.ProjectileType<Projectiles.GlowRingHollow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2f);
                    }
                }
            }

            int ringDelay = FargoSoulsWorld.MasochistModeReal ? 12 : 15;
            int ringMax = FargoSoulsWorld.MasochistModeReal ? 5 : 4;
            if (NPC.ai[3] % ringDelay == 0 && NPC.ai[3] < ringDelay * ringMax)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float rotationOffset = MathHelper.TwoPi / ringMax * NPC.ai[3] / ringDelay + NPC.localAI[0];
                    int baseDelay = 60;
                    float flyDelay = 120 + NPC.ai[3] / ringDelay * (FargoSoulsWorld.MasochistModeReal ? 40 : 50);
                    int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 300f / baseDelay * Vector2.UnitX.RotatedBy(rotationOffset), ModContent.ProjectileType<MutantMark2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, baseDelay, baseDelay + flyDelay);
                    if (p != Main.maxProjectiles)
                    {
                        const int max = 5;
                        const float distance = 125f;
                        float rotation = MathHelper.TwoPi / max;
                        for (int i = 0; i < max; i++)
                        {
                            float myRot = rotation * i + rotationOffset;
                            Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(myRot);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantCrystalLeaf>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, Main.projectile[p].identity, myRot);
                        }
                    }
                }
            }

            if (NPC.ai[3] > 45 && --NPC.ai[1] < 0)
            {
                NPC.netUpdate = true;
                NPC.ai[1] = 20;
                NPC.ai[2] = NPC.ai[2] > 0 ? -1 : 1;

                SoundEngine.PlaySound(SoundID.Item92, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[3] < 330)
                {
                    const float retiRad = 525;
                    const float spazRad = 350;
                    float retiSpeed = 2 * (float)Math.PI * retiRad / 300;
                    float spazSpeed = 2 * (float)Math.PI * spazRad / 180;
                    float retiAcc = retiSpeed * retiSpeed / retiRad * NPC.ai[2];
                    float spazAcc = spazSpeed * spazSpeed / spazRad * -NPC.ai[2];
                    float rotationOffset = FargoSoulsWorld.MasochistModeReal ? MathHelper.PiOver4 : 0;
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i + rotationOffset) * retiSpeed, ModContent.ProjectileType<MutantRetirang>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, retiAcc, 300);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i + Math.PI / 4 + rotationOffset) * spazSpeed, ModContent.ProjectileType<MutantSpazmarang>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, spazAcc, 180);
                    }
                }
            }
            if (++NPC.ai[3] > 450)
            {
                ChooseNextAttack(11, 13, 16, 21, 24, 26, 29, 31, 33, 35, 39, 41, 44, 45);
            }
        }

        void EmpressSwordWave()
        {
            if (!AliveCheck(player))
                return;

            if (!FargoSoulsWorld.EternityMode)
            {
                NPC.ai[0]++; //dont do this attack in expert
                return;
            }

            //Vector2 targetPos = player.Center + 360 * NPC.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(10)); Movement(targetPos, 0.25f);
            NPC.velocity = Vector2.Zero;

            int attackThreshold = FargoSoulsWorld.MasochistModeReal ? 48 : 60;
            int timesToAttack = FargoSoulsWorld.MasochistModeReal ? 3 + (int)(endTimeVariance * 5) : 4;
            int startup = 90;

            if (NPC.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                NPC.ai[3] = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            void Sword(Vector2 pos, float ai0, float ai1, Vector2 vel)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos - vel * 60f, vel,
                        ProjectileID.FairyQueenLance, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, ai0, ai1);
                }
            }

            if (NPC.ai[1] >= startup && NPC.ai[1] < startup + attackThreshold * timesToAttack && --NPC.ai[2] < 0) //walls of swords
            {
                NPC.ai[2] = attackThreshold;

                SoundEngine.PlaySound(SoundID.Item163, player.Center);

                if (Math.Abs(MathHelper.WrapAngle(NPC.DirectionFrom(player.Center).ToRotation() - NPC.ai[3])) > MathHelper.PiOver2)
                    NPC.ai[3] += MathHelper.Pi; //swords always spawn closer to player

                const int maxHorizSpread = 1600 * 2;
                const int arenaRadius = 1200;
                int max = FargoSoulsWorld.MasochistModeReal ? 16 : 12;
                float gap = maxHorizSpread / max;

                float attackAngle = NPC.ai[3];// + Main.rand.NextFloat(MathHelper.ToDegrees(10)) * (Main.rand.NextBool() ? -1 : 1);
                Vector2 spawnOffset = -attackAngle.ToRotationVector2();

                //start by focusing on player
                Vector2 focusPoint = player.Center;

                //move focus point along grid closer so attack stays centered
                Vector2 home = NPC.Center;// FargoSoulsUtil.ProjectileExists(ritualProj, ModContent.ProjectileType<MutantRitual>()) == null ? NPC.Center : Main.projectile[ritualProj].Center;
                for (float i = 0; i < arenaRadius; i += gap)
                {
                    Vector2 newFocusPoint = focusPoint + gap * attackAngle.ToRotationVector2();
                    if ((home - newFocusPoint).Length() > (home - focusPoint).Length())
                        break;
                    focusPoint = newFocusPoint;
                }

                //doing it this way to guarantee it always remains aligned to grid
                float spawnDistance = 0;
                while (spawnDistance < arenaRadius)
                    spawnDistance += gap;

                float mirrorLength = 2f * (float)Math.Sqrt(2f * spawnDistance * spawnDistance);
                int swordCounter = 0;
                for (int i = -max; i <= max; i++)
                {
                    Vector2 spawnPos = focusPoint + spawnOffset * spawnDistance + spawnOffset.RotatedBy(MathHelper.PiOver2) * gap * i;
                    float Ai1 = swordCounter++ / (max * 2f + 1);

                    Vector2 randomOffset = Main.rand.NextVector2Unit();
                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        if (randomOffset.Length() < 0.5f)
                            randomOffset = 0.5f * randomOffset.SafeNormalize(Vector2.UnitX);
                        randomOffset *= 2f;
                    }

                    Sword(spawnPos, attackAngle + MathHelper.PiOver4, Ai1, randomOffset);
                    Sword(spawnPos, attackAngle - MathHelper.PiOver4, Ai1, randomOffset);

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        Sword(spawnPos + mirrorLength * (attackAngle + MathHelper.PiOver4).ToRotationVector2(), attackAngle + MathHelper.PiOver4 + MathHelper.Pi, Ai1, randomOffset);
                        Sword(spawnPos + mirrorLength * (attackAngle - MathHelper.PiOver4).ToRotationVector2(), attackAngle - MathHelper.PiOver4 + MathHelper.Pi, Ai1, randomOffset);
                    }
                }

                NPC.ai[3] += MathHelper.PiOver4 * (Main.rand.NextBool() ? -1 : 1) //rotate 90 degrees
                    + Main.rand.NextFloat(MathHelper.PiOver4 / 2) * (Main.rand.NextBool() ? -1 : 1); //variation

                NPC.netUpdate = true;
            }

            void MegaSwordSwarm(Vector2 target)
            {
                SoundEngine.PlaySound(SoundID.Item164, player.Center);

                float safeAngle = NPC.ai[3];
                float safeRange = MathHelper.ToRadians(10);
                int max = 60;
                for (int i = 0; i < max; i++)
                {
                    float rotationOffset = Main.rand.NextFloat(safeRange, MathHelper.Pi - safeRange);
                    Vector2 offset = Main.rand.NextFloat(600f, 2400f) * (safeAngle + rotationOffset).ToRotationVector2();
                    if (Main.rand.NextBool())
                        offset *= -1;

                    //if (FargoSoulsWorld.MasochistModeReal) //block one side so only one real exit exists
                    //    target += Main.rand.NextFloat(600) * safeAngle.ToRotationVector2();

                    Vector2 spawnPos = target + offset;
                    Vector2 vel = (target - spawnPos) / 60f;
                    Sword(spawnPos, vel.ToRotation(), (float)i / max, -vel * 0.75f);
                }
            }

            //massive sword barrage
            int swordSwarmTime = startup + attackThreshold * timesToAttack + 40;
            if (NPC.ai[1] == swordSwarmTime)
            {
                MegaSwordSwarm(player.Center);
                NPC.localAI[0] = player.Center.X;
                NPC.localAI[1] = player.Center.Y;
            }

            if (FargoSoulsWorld.MasochistModeReal && NPC.ai[1] == swordSwarmTime + 30)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    MegaSwordSwarm(new Vector2(NPC.localAI[0], NPC.localAI[1]) + 600 * i * NPC.ai[3].ToRotationVector2());
                }
            }

            if (++NPC.ai[1] > swordSwarmTime + (FargoSoulsWorld.MasochistModeReal ? 60 : 30))
            {
                ChooseNextAttack(11, 13, 16, 21, FargoSoulsWorld.MasochistModeReal ? 26 : 24, 29, 31, 35, 37, 39, 41, 45);
            }
        }

        void P2NextAttackPause() //choose next attack but actually, this also gives breathing space for mp to sync up
        {
            if (!AliveCheck(player))
                return;

            EModeSpecialEffects(); //manage these here, for case where players log out/rejoin in mp

            Vector2 targetPos = player.Center + NPC.DirectionFrom(player.Center) * 400;
            Movement(targetPos, 0.3f);
            if (NPC.Distance(targetPos) > 200) //faster if offscreen
                Movement(targetPos, 0.3f);

            if (++NPC.ai[1] > 60 || (NPC.Distance(targetPos) < 200 && NPC.ai[1] > (NPC.localAI[3] >= 3 ? 15 : 30)))
            {
                /*EModeGlobalNPC.PrintAI(npc);
                string output = "";
                foreach (float attack in attackHistory)
                    output += attack.ToString() + " ";
                Main.NewText(output);*/

                NPC.velocity *= FargoSoulsWorld.MasochistModeReal ? 0.25f : 0.75f;

                //NPC.TargetClosest();
                NPC.ai[0] = NPC.ai[2];
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.netUpdate = true;
            }
        }

        #endregion

        #region p3

        bool Phase3Transition()
        {
            bool retval = true;

            NPC.localAI[3] = 3;

            EModeSpecialEffects();

            //NPC.damage = 0;
            if (NPC.buffType[0] != 0)
                NPC.DelBuff(0);

            if (NPC.ai[1] == 0) //entering final phase, give healing
            {
                NPC.life = NPC.lifeMax;

                DramaticTransition(true);
            }

            if (NPC.ai[1] < 60 && !Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

            if (NPC.ai[1] == 360)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            }

            if (++NPC.ai[1] > 480)
            {
                retval = false; //dont drain life during this time, ensure it stays synced

                if (!AliveCheck(player))
                    return retval;
                Vector2 targetPos = player.Center;
                targetPos.Y -= 300;
                Movement(targetPos, 1f, true, false);
                if (NPC.Distance(targetPos) < 50 || NPC.ai[1] > 720)
                {
                    NPC.netUpdate = true;
                    NPC.velocity = Vector2.Zero;
                    NPC.localAI[0] = 0;
                    NPC.ai[0]--;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = NPC.DirectionFrom(player.Center).ToRotation();
                    NPC.ai[3] = (float)Math.PI / 20f;
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                    if (player.Center.X < NPC.Center.X)
                        NPC.ai[3] *= -1;
                    //EdgyBossText("But we're not done yet!");
                }
            }
            else
            {
                NPC.velocity *= 0.9f;

                //make you stop attacking
                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && NPC.Distance(Main.LocalPlayer.Center) < 3000)
                {
                    Main.LocalPlayer.controlUseItem = false;
                    Main.LocalPlayer.controlUseTile = false;
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().NoUsingItems = true;
                }

                if (--NPC.localAI[0] < 0)
                {
                    NPC.localAI[0] = Main.rand.Next(15);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                        int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }

            return retval;
        }

        void VoidRaysP3()
        {
            if (--NPC.ai[1] < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float speed = FargoSoulsWorld.MasochistModeReal && NPC.localAI[0] <= 40 ? 4f : 2f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed * Vector2.UnitX.RotatedBy(NPC.ai[2]), ModContent.ProjectileType<MutantMark1>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                }
                NPC.ai[1] = 1;
                NPC.ai[2] += NPC.ai[3];

                if (NPC.localAI[0] < 30)
                {
                    EModeSpecialEffects();
                    TryMasoP3Theme();
                }

                if (NPC.localAI[0]++ == 40 || NPC.localAI[0] == 80 || NPC.localAI[0] == 120)
                {
                    NPC.netUpdate = true;
                    NPC.ai[2] -= NPC.ai[3] / (FargoSoulsWorld.MasochistModeReal ? 3 : 2);
                }
                else if (NPC.localAI[0] >= (FargoSoulsWorld.MasochistModeReal ? 160 : 120))
                {
                    NPC.netUpdate = true;
                    NPC.ai[0]--;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.localAI[0] = 0;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }

            NPC.velocity = Vector2.Zero;
        }

        void OkuuSpheresP3()
        {
            if (NPC.ai[2] == 0)
            {
                if (!AliveCheck(player))
                    return;
                NPC.ai[2] = Main.rand.NextBool() ? -1 : 1;
                NPC.ai[3] = Main.rand.NextFloat((float)Math.PI * 2);
            }

            int endTime = 360 + 120;
            if (FargoSoulsWorld.MasochistModeReal)
                endTime += 360;
            
            if (++NPC.ai[1] > 10 && NPC.ai[3] > 60 && NPC.ai[3] < endTime - 120)
            {
                NPC.ai[1] = 0;
                float rotation = MathHelper.ToRadians(45) * (NPC.ai[3] - 60) / 240 * NPC.ai[2];
                int max = FargoSoulsWorld.MasochistModeReal ? 11 : 10;
                float speed = FargoSoulsWorld.MasochistModeReal ? 11f : 10f;
                SpawnSphereRing(max, speed, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), -0.75f, rotation);
                SpawnSphereRing(max, speed, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0.75f, rotation);
            }

            if (NPC.ai[3] < 30)
            {
                EModeSpecialEffects();
                TryMasoP3Theme();
            }

            if (++NPC.ai[3] > endTime)
            {
                NPC.netUpdate = true;
                NPC.ai[0]--;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                //NPC.TargetClosest();
            }
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }

            NPC.velocity = Vector2.Zero;
        }

        void BoundaryBulletHellP3()
        {
            if (NPC.localAI[0] == 0)
            {
                if (!AliveCheck(player))
                    return;
                NPC.localAI[0] = Math.Sign(NPC.Center.X - player.Center.X);
            }

            if (++NPC.ai[1] > 3)
            {
                SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                NPC.ai[1] = 0;
                NPC.ai[2] += (float)Math.PI / 5 / 420 * NPC.ai[3] * NPC.localAI[0] * (FargoSoulsWorld.MasochistModeReal ? 2f : 1);
                if (NPC.ai[2] > (float)Math.PI)
                    NPC.ai[2] -= (float)Math.PI * 2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int max = FargoSoulsWorld.MasochistModeReal ? 10 : 8;
                    for (int i = 0; i < max; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0f, -6f).RotatedBy(NPC.ai[2] + MathHelper.TwoPi / max * i),
                            ModContent.ProjectileType<MutantEye>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                    }
                }
            }

            if (NPC.ai[3] < 30)
            {
                EModeSpecialEffects();
                TryMasoP3Theme();
            }

            int endTime = 360;
            if (FargoSoulsWorld.MasochistModeReal)
                endTime += 360;
            if (++NPC.ai[3] > endTime)
            {
                //NPC.TargetClosest();
                NPC.ai[0]--;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }

            NPC.velocity = Vector2.Zero;
        }

        void FinalSpark()
        {
            void SpinLaser(bool useMasoSpeed)
            {
                float newRotation = NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation();
                float difference = MathHelper.WrapAngle(newRotation - NPC.ai[3]);
                float rotationDirection = 2f * (float)Math.PI * 1f / 6f / 60f;
                rotationDirection *= useMasoSpeed ? 0.525f : 1f;
                NPC.ai[3] += Math.Min(rotationDirection, Math.Abs(difference)) * Math.Sign(difference);
                if (useMasoSpeed)
                    NPC.ai[3] = NPC.ai[3].AngleLerp(newRotation, 0.015f);
            }

            //if targets are all dead, will despawn much more aggressively to reduce respawn cheese
            if (NPC.localAI[2] > 30)
            {
                NPC.localAI[2] += 1; //after 30 ticks of no target, despawn can't be stopped
                if (NPC.localAI[2] > 120)
                    AliveCheck(player, true);
                return;
            }

            if (--NPC.localAI[0] < 0) //just visual explosions
            {
                NPC.localAI[0] = Main.rand.Next(30);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                    int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                }
            }

            bool harderRings = FargoSoulsWorld.MasochistModeReal && NPC.ai[2] >= 420 - 90;
            int ringTime = harderRings ? 100 : 120;
            if (++NPC.ai[1] > ringTime)
            {
                NPC.ai[1] = 0;

                EModeSpecialEffects();
                TryMasoP3Theme();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int max = /*harderRings ? 11 :*/ 10;
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.damage);
                    SpawnSphereRing(max, 6f, damage, 0.5f);
                    SpawnSphereRing(max, 6f, damage, -.5f);
                }
            }

            if (NPC.ai[2] == 0)
            {
                if (!FargoSoulsWorld.MasochistModeReal)
                    NPC.localAI[1] = 1;
            }
            else if (NPC.ai[2] == 420 - 90) //dramatic telegraph
            {
                if (NPC.localAI[1] == 0) //maso do ordinary spark
                {
                    NPC.localAI[1] = 1;
                    NPC.ai[2] -= 600 + 180;

                    //bias in one direction
                    NPC.ai[3] -= MathHelper.ToRadians(20);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3]),
                            ModContent.ProjectileType<MutantGiantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.5f), 0f, Main.myPlayer, 0, NPC.whoAmI);
                    }

                    NPC.netUpdate = true;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        const int max = 8;
                        for (int i = 0; i < max; i++)
                        {
                            float offset = i - 0.5f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (NPC.ai[3] + MathHelper.TwoPi / max * offset).ToRotationVector2(), ModContent.ProjectileType<Projectiles.GlowLine>(), 0, 0f, Main.myPlayer, 13f, NPC.whoAmI);
                        }
                    }
                }
            }

            if (NPC.ai[2] < 420)
            {
                //disable it while doing maso's first ray
                if (NPC.localAI[1] == 0 || NPC.ai[2] > 420 - 90)
                    NPC.ai[3] = NPC.DirectionFrom(player.Center).ToRotation(); //hold it here for glow line effect
            }
            else
            {
                if (!Main.dedServ && !Terraria.Graphics.Effects.Filters.Scene["FargowiltasSouls:FinalSpark"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene.Activate("FargowiltasSouls:FinalSpark");
                }

                if (NPC.ai[1] % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 24f * Vector2.UnitX.RotatedBy(NPC.ai[3]), ModContent.ProjectileType<MutantEyeWavy>(), 0, 0f, Main.myPlayer,
                      Main.rand.NextFloat(0.5f, 1.25f) * (Main.rand.NextBool() ? -1 : 1), Main.rand.Next(10, 60));
                }
            }
            
            int endTime = 1020;
            if (FargoSoulsWorld.MasochistModeReal)
                endTime += 180;
            if (++NPC.ai[2] > endTime)
            {
                NPC.netUpdate = true;
                NPC.ai[0]--;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                FargoSoulsUtil.ClearAllProjectiles(2, NPC.whoAmI);
            }
            else if (NPC.ai[2] == 420)
            {
                NPC.netUpdate = true;

                //bias it in one direction
                NPC.ai[3] += MathHelper.ToRadians(20) * (FargoSoulsWorld.MasochistModeReal ? 1 : -1);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.ai[3]),
                        ModContent.ProjectileType<MutantGiantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 0.5f), 0f, Main.myPlayer, 0, NPC.whoAmI);
                }
            }
            else if (NPC.ai[2] < 300 && NPC.localAI[1] != 0) //charging up dust
            {
                float num1 = 0.99f;
                if (NPC.ai[2] >= 60)
                    num1 = 0.79f;
                if (NPC.ai[2] >= 120)
                    num1 = 0.58f;
                if (NPC.ai[2] >= 180)
                    num1 = 0.43f;
                if (NPC.ai[2] >= 240)
                    num1 = 0.33f;
                for (int i = 0; i < 9; ++i)
                {
                    if (Main.rand.NextFloat() >= num1)
                    {
                        float f = Main.rand.NextFloat() * 6.283185f;
                        float num2 = Main.rand.NextFloat();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + f.ToRotationVector2() * (110 + 600 * num2), 229, (f - 3.141593f).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);
                        dust.scale = 0.9f;
                        dust.fadeIn = 1.15f + num2 * 0.3f;
                        //dust.color = new Color(1f, 1f, 1f, num1) * (1f - num1);
                        dust.noGravity = true;
                        //dust.noLight = true;
                    }
                }
            }

            SpinLaser(FargoSoulsWorld.MasochistModeReal && NPC.ai[2] >= 420);

            if (AliveCheck(player))
                NPC.localAI[2] = 0;
            else
                NPC.localAI[2]++;

            NPC.velocity = Vector2.Zero; //prevents mutant from moving despite calling AliveCheck()
        }

        void DyingDramaticPause()
        {
            if (!AliveCheck(player))
                return;
            NPC.ai[3] -= (float)Math.PI / 6f / 60f;
            NPC.velocity = Vector2.Zero;
            if (++NPC.ai[1] > 120)
            {
                NPC.netUpdate = true;
                NPC.ai[0]--;
                NPC.ai[1] = 0;
                NPC.ai[3] = (float)-Math.PI / 2;
                NPC.netUpdate = true;
                if (Main.netMode != NetmodeID.MultiplayerClient) //shoot harmless mega ray
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * -1, ModContent.ProjectileType<MutantGiantDeathray2>(), 0, 0f, Main.myPlayer, 1, NPC.whoAmI);
                //EdgyBossText("I have not a single regret in my existence!");
            }
            if (--NPC.localAI[0] < 0)
            {
                NPC.localAI[0] = Main.rand.Next(15);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height));
                    int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        void DyingAnimationAndHandling()
        {
            NPC.velocity = Vector2.Zero;
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 12f;
            }
            if (--NPC.localAI[0] < 0)
            {
                NPC.localAI[0] = Main.rand.Next(5);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(240, 240);
                    int type = ModContent.ProjectileType<Projectiles.BossWeapons.PhantasmalBlast>();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, type, 0, 0f, Main.myPlayer);
                }
            }
            if (++NPC.ai[1] % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 24f * Vector2.UnitX.RotatedBy(NPC.ai[3]), ModContent.ProjectileType<MutantEyeWavy>(), 0, 0f, Main.myPlayer,
                    Main.rand.NextFloat(0.75f, 1.5f) * (Main.rand.NextBool() ? -1 : 1), Main.rand.Next(10, 90));
            }
            if (++NPC.alpha > 255)
            {
                NPC.alpha = 255;
                NPC.life = 0;
                NPC.dontTakeDamage = false;
                NPC.checkDead();
                if (Main.netMode != NetmodeID.MultiplayerClient && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                {
                    int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, modNPC.Type);
                    if (n != Main.maxNPCs)
                    {
                        Main.npc[n].homeless = true;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
                //EdgyBossText("Oh, right... my revive...");
            }
        }

        #endregion


        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 229, 0f, 0f, 0, default(Color), 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (FargoSoulsWorld.AngryMutant)
                damage *= 0.07f;
            return true;
        }

        public override bool CheckDead()
        {
            if (NPC.ai[0] == -7)
                return true;

            NPC.life = 1;
            NPC.active = true;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[0] > -1)
            {
                NPC.ai[0] = FargoSoulsWorld.EternityMode ? (NPC.ai[0] >= 10 ? -1 : 10) : -6;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                FargoSoulsUtil.ClearAllProjectiles(2, NPC.whoAmI, NPC.ai[0] < 0);
                //EdgyBossText("You're pretty good...");
            }
            return false;
        }

        public override void OnKill()
        {
            base.OnKill();

            if (!playerInvulTriggered && FargoSoulsWorld.EternityMode)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.Hitbox, ModContent.ItemType<PhantasmalEnergy>());
            }

            if (FargoSoulsWorld.EternityMode)
            {
                if (Main.LocalPlayer.active)
                {
                    if (!Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso && Main.netMode != NetmodeID.Server)
                        Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.MasochistModeUnlocked"), new Color(51, 255, 191, 0));
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso = true;
                }
                FargoSoulsWorld.CanPlayMaso = true;
            }

            FargoSoulsWorld.skipMutantP1 = 0;

            NPC.SetEventFlagCleared(ref FargoSoulsWorld.downedMutant, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MutantBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MutantTrophy>(), 10));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<MutantRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SpawnSack>(), 4));

            LeadingConditionRule emodeRule = new LeadingConditionRule(new EModeDropCondition());
            emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<Items.Accessories.Masomode.MutantEye>()));
            npcLoot.Add(emodeRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 4)
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
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, position, new Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);

            if (ShouldDrawAura)
                DrawAura(spriteBatch, position);

            return false;
        }

        public void DrawAura(SpriteBatch spriteBatch, Vector2 position)
        {
            // Outer ring.
            Color outerColor = Color.CadetBlue;
            outerColor.A = 0;
            spriteBatch.Draw(FargosTextureRegistry.SoftEdgeRing.Value, position, null, outerColor * 0.7f, 0f, FargosTextureRegistry.SoftEdgeRing.Value.Size() * 0.5f, 9.2f, SpriteEffects.None, 0f);
        }
    }
}