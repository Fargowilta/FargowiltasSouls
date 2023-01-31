using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.ItemDropRules.Conditions;
using FargowiltasSouls.Items.Placeables;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs
{
    public partial class EModeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        //masochist doom
        //public bool[] masoBool = new bool[4];
        //private int Stop = 0;

        public bool BeetleOffenseAura;
        public bool BeetleDefenseAura;
        public bool BeetleUtilAura;
        public int BeetleTimer;

        public bool PaladinsShield;
        public bool isWaterEnemy;
        public bool HasWhipDebuff;

        //public List<int> auraDebuffs = new List<int>();

        public static int slimeBoss = -1;
        public static int eyeBoss = -1;
        public static int eaterBoss = -1;
        public static int brainBoss = -1;
        public static int beeBoss = -1;
        public static int skeleBoss = -1;
        public static int deerBoss = -1;
        public static int wallBoss = -1;
        public static int retiBoss = -1;
        public static int spazBoss = -1;
        public static int destroyBoss = -1;
        public static int primeBoss = -1;
        public static int queenSlimeBoss = -1;
        public static int empressBoss = -1;
        public static int betsyBoss = -1;
        public static int fishBoss = -1;
        public static int cultBoss = -1;
        public static int moonBoss = -1;
        public static int guardBoss = -1;
        public static int fishBossEX = -1;
        public static bool spawnFishronEX;
        public static int deviBoss = -1;
        public static int abomBoss = -1;
        public static int mutantBoss = -1;
        public static int championBoss = -1;

        public static int eaterTimer;
        //public static int eaterResist;

        public override void ResetEffects(NPC npc)
        {
            PaladinsShield = false;
            HasWhipDebuff = false;

            if (BeetleTimer > 0 && --BeetleTimer <= 0)
            {
                BeetleDefenseAura = false;
                BeetleOffenseAura = false;
                BeetleUtilAura = false;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (!FargoSoulsWorld.EternityMode) return;

            npc.value = (int)(npc.value * 1.3);

            //VERY old masomode boss scaling numbers, leaving here in case we ever want to do the funny again
            // +2.5% hp each kill 
            // +1.25% damage each kill
            // max of 4x hp and 2.5x damage
            //pre hm get 8x and 5x
        }

        public override bool PreAI(NPC npc)
        {
            if (!FargoSoulsWorld.EternityMode)
                return base.PreAI(npc);

            //in pre-hm, enemies glow slightly at night
            if (!Main.dayTime && !Main.hardMode)
            {
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                if (y < Main.worldSurface && y > 0 && x > 0 && x < Main.maxTilesX)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile != null && tile.WallType == 0)
                    {
                        Lighting.AddLight(npc.Center, 0.5f, 0.5f, 0.5f);
                    }
                }
            }

            /*if (Stop > 0)
            {
                Stop--;
                npc.position = npc.oldPosition;
                npc.frameCounter = 0;
            }*/

            if (!npc.dontTakeDamage)
            {
                if (npc.position.Y / 16 < Main.worldSurface * 0.35f) //enemy in space
                    npc.AddBuff(BuffID.Suffocation, 2, true);
                else if (npc.position.Y / 16 > Main.maxTilesY - 200) //enemy in hell
                {
                    //because of funny bug where town npcs fall forever in mp, including into hell
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        npc.AddBuff(BuffID.OnFire, 2);
                }

                if (npc.wet && !npc.noTileCollide && !isWaterEnemy && npc.HasPlayerTarget)
                {
                    npc.AddBuff(ModContent.BuffType<Lethargic>(), 2, true);
                    if (Main.player[npc.target].ZoneCorrupt)
                        npc.AddBuff(BuffID.CursedInferno, 2, true);
                    if (Main.player[npc.target].ZoneCrimson)
                        npc.AddBuff(BuffID.Ichor, 2, true);
                    if (Main.player[npc.target].ZoneHallow)
                        npc.AddBuff(ModContent.BuffType<Smite>(), 2, true);
                    if (Main.player[npc.target].ZoneJungle)
                        npc.AddBuff(BuffID.Poisoned, 2, true);
                }
            }

            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                //switch (npc.type)
                //{
                //    case NPCID.EaterofWorldsHead:
                //    case NPCID.EaterofWorldsBody:
                //    case NPCID.EaterofWorldsTail:
                //        target.AddBuff(BuffID.CursedInferno, 180);
                //        target.AddBuff(ModContent.BuffType<Rotting>(), 600);
                //        break;

                //    default:
                //        break;
                //}

                if (BeetleUtilAura)
                {
                    target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Frozen, 30);
                }
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.2f);
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            //layers
            int y = spawnInfo.SpawnTileY;
            bool cavern = y >= Main.maxTilesY * 0.4f && y <= Main.maxTilesY * 0.8f;
            bool underground = y > Main.worldSurface && y <= Main.maxTilesY * 0.4f;
            bool surface = y < Main.worldSurface && !spawnInfo.Sky;
            bool wideUnderground = cavern || underground;
            bool underworld = spawnInfo.Player.ZoneUnderworldHeight;
            bool sky = spawnInfo.Sky;

            //times
            bool night = !Main.dayTime;
            bool day = Main.dayTime;

            //biomes
            bool noBiome = FargowiltasSouls.NoBiomeNormalSpawn(spawnInfo);
            bool ocean = spawnInfo.Player.ZoneBeach;
            bool dungeon = spawnInfo.Player.ZoneDungeon;
            bool meteor = spawnInfo.Player.ZoneMeteor;
            bool spiderCave = spawnInfo.SpiderCave;
            bool mushroom = spawnInfo.Player.ZoneGlowshroom;
            bool jungle = spawnInfo.Player.ZoneJungle;
            bool granite = spawnInfo.Granite;
            bool marble = spawnInfo.Marble;
            bool corruption = spawnInfo.Player.ZoneCorrupt;
            bool crimson = spawnInfo.Player.ZoneCrimson;
            bool snow = spawnInfo.Player.ZoneSnow;
            bool hallow = spawnInfo.Player.ZoneHallow;
            bool desert = spawnInfo.Player.ZoneDesert;

            bool nebulaTower = spawnInfo.Player.ZoneTowerNebula;
            bool vortexTower = spawnInfo.Player.ZoneTowerVortex;
            bool stardustTower = spawnInfo.Player.ZoneTowerStardust;
            bool solarTower = spawnInfo.Player.ZoneTowerSolar;

            bool water = spawnInfo.Water;

            //events
            bool goblinArmy = Main.invasionType == 1;
            bool frostLegion = Main.invasionType == 2;
            bool pirates = Main.invasionType == 3;
            bool oldOnesArmy = DD2Event.Ongoing && spawnInfo.Player.ZoneOldOneArmy;
            bool frostMoon = surface && night && Main.snowMoon;
            bool pumpkinMoon = surface && night && Main.pumpkinMoon;
            bool solarEclipse = surface && day && Main.eclipse;
            bool martianMadness = Main.invasionType == 4;
            bool lunarEvents = NPC.LunarApocalypseIsUp && (nebulaTower || vortexTower || stardustTower || solarTower);
            //bool monsterMadhouse = MMWorld.MMArmy;
            bool noEvent = Main.invasionType == 0 && !oldOnesArmy && !frostMoon && !pumpkinMoon && !solarEclipse && !lunarEvents;

            //no work?
            //is lava on screen
            //bool nearLava = Collision.LavaCollision(spawnInfo.Player.position, spawnInfo.SpawnTileX, spawnInfo.SpawnTileY);
            bool deepUnderground = !underworld && spawnInfo.Player.position.Y > Main.maxTilesY - Main.UnderworldLayer * 3;
            bool noInvasion = FargowiltasSouls.NoInvasion(spawnInfo);
            bool normalSpawn = !spawnInfo.PlayerInTown && noInvasion && !oldOnesArmy && noEvent;

            bool bossCanSpawn = FargoSoulsWorld.MasochistModeReal && !spawnInfo.Player.GetModPlayer<FargoSoulsPlayer>().SinisterIcon && !FargoSoulsUtil.AnyBossAlive();

            //MASOCHIST MODE
            if (FargoSoulsWorld.EternityMode)
            {
                //all the pre hardmode
                if (!Main.hardMode)
                {
                    //mutually exclusive world layers
                    if (surface)
                    {
                        if (night && normalSpawn)
                        {
                            if (noBiome)
                            {
                                pool[NPCID.CorruptBunny] = NPC.downedBoss1 ? .05f : .025f;
                                pool[NPCID.CrimsonBunny] = NPC.downedBoss1 ? .05f : .025f;
                            }

                            if (snow)
                            {
                                pool[NPCID.CorruptPenguin] = NPC.downedBoss1 ? .1f : .05f;
                                pool[NPCID.CrimsonPenguin] = NPC.downedBoss1 ? .1f : .05f;
                            }

                            if (ocean || Main.raining)
                            {
                                pool[NPCID.CorruptGoldfish] = NPC.downedBoss1 ? .1f : .05f;
                                pool[NPCID.CrimsonGoldfish] = NPC.downedBoss1 ? .1f : .05f;
                            }

                            if (NPC.downedBoss1)
                            {
                                if (jungle)
                                    pool[NPCID.DoctorBones] = .05f;

                                if (NPC.downedBoss3 && !NPC.downedMechBoss2 && bossCanSpawn)
                                    pool[NPCID.EyeofCthulhu] = Main.bloodMoon ? .0004f : .0002f;
                            }
                        }

                        if (normalSpawn && FargoSoulsWorld.downedAnyBoss)
                        {
                            if (snow)
                                pool[NPCID.IceGolem] = .005f;

                            if (desert)
                                pool[NPCID.SandElemental] = .005f;
                        }

                        if (Main.slimeRain && NPC.downedBoss2 && bossCanSpawn)
                            pool[NPCID.KingSlime] = 0.004f;
                    }
                    else if (wideUnderground)
                    {
                        if (deepUnderground && !jungle && !snow)
                        {
                            pool[NPCID.FireImp] = .02f;
                            pool[NPCID.LavaSlime] = .02f;
                        }

                        if (marble && NPC.downedBoss2)
                        {
                            pool[NPCID.Medusa] = .05f;
                        }

                        if (granite)
                        {
                            pool[NPCID.GraniteFlyer] = .1f;
                            pool[NPCID.GraniteGolem] = .1f;
                        }

                        if (cavern)
                        {
                            if (noBiome && NPC.downedBoss3)
                                pool[NPCID.DarkCaster] = .025f;
                        }

                        if (NPC.downedGoblins && !NPC.savedGoblin && !NPC.AnyNPCs(NPCID.BoundGoblin))
                            pool[NPCID.BoundGoblin] = .5f;

                        if (spiderCave && !NPC.savedStylist && !NPC.AnyNPCs(NPCID.WebbedStylist))
                            pool[NPCID.WebbedStylist] = .5f;
                    }
                    else if (underworld)
                    {
                        pool[NPCID.LeechHead] = .05f;
                        pool[NPCID.BlazingWheel] = .1f;
                        //if (!FargoSoulsUtil.BossIsAlive(ref wallBoss, NPCID.WallofFlesh))
                        //pool[NPCID.RedDevil] = .025f;
                    }
                    else if (sky)
                    {
                        if (normalSpawn)
                        {
                            pool[NPCID.AngryNimbus] = .02f;

                            if (FargoSoulsWorld.downedAnyBoss)
                                pool[NPCID.WyvernHead] = .005f;
                        }
                    }

                    //height-independent biomes
                    if (corruption)
                    {
                        if (NPC.downedBoss2)
                        {
                            pool[NPCID.SeekerHead] = .01f;
                            if (normalSpawn && NPC.downedBoss3 && !underworld && bossCanSpawn)
                                pool[NPCID.EaterofWorldsHead] = .0002f;
                        }
                    }

                    if (crimson)
                    {
                        if (NPC.downedBoss2)
                        {
                            pool[NPCID.IchorSticker] = .01f;
                            if (normalSpawn && NPC.downedBoss3 && !underworld && bossCanSpawn)
                                pool[NPCID.BrainofCthulhu] = .0002f;
                        }
                    }

                    if (mushroom)
                    {
                        pool[NPCID.FungiBulb] = .1f;
                        pool[NPCID.MushiLadybug] = .1f;
                        pool[NPCID.ZombieMushroom] = .1f;
                        pool[NPCID.ZombieMushroomHat] = .1f;
                        pool[NPCID.AnomuraFungus] = .1f;
                    }

                    if (ocean)
                    {
                        pool[NPCID.PigronCorruption] = .005f;
                        pool[NPCID.PigronCrimson] = .005f;
                        pool[NPCID.PigronHallow] = .005f;
                    }

                    if (!surface && normalSpawn)
                    {
                        pool[NPCID.Mimic] = .01f;
                        if (desert && NPC.downedBoss2)
                            pool[NPCID.DuneSplicerHead] = .005f;
                    }
                }
                else //all the hardmode
                {
                    //mutually exclusive world layers
                    if (surface && !lunarEvents)
                    {
                        if (day)
                        {
                            if (normalSpawn)
                            {
                                if (noBiome && bossCanSpawn)
                                    pool[NPCID.KingSlime] = Main.slimeRain ? .0004f : .0002f;

                                if (NPC.downedMechBossAny && (noBiome || dungeon))
                                    pool[NPCID.CultistArcherWhite] = .01f;

                                if (jungle)
                                    pool[NPCID.Parrot] = .05f;

                            }
                        }
                        else //night
                        {
                            if (Main.bloodMoon)
                            {
                                pool[NPCID.ChatteringTeethBomb] = .1f;
                                /*if (!sinisterIcon && !NPC.downedMechBoss2 && !FargoSoulsUtil.AnyBossAlive())
                                    pool[NPCID.EyeofCthulhu] = .004f;

                                if (NPC.downedPlantBoss)
                                {
                                    if (bossCanSpawn)
                                    {
                                        pool[NPCID.Retinazer] = .002f;
                                        pool[NPCID.Spazmatism] = .002f;
                                        pool[NPCID.TheDestroyer] = .002f;
                                        pool[NPCID.SkeletronPrime] = .002f;
                                    }
                                }*/
                            }

                            if (noInvasion && !oldOnesArmy && bossCanSpawn)
                                pool[NPCID.Clown] = 0.01f;

                            if (normalSpawn)
                            {
                                if (NPC.downedBoss1)
                                {
                                    if (noBiome)
                                    {
                                        pool[NPCID.CorruptBunny] = .05f;
                                        pool[NPCID.CrimsonBunny] = .05f;
                                    }

                                    if (snow)
                                    {
                                        pool[NPCID.CorruptPenguin] = .05f;
                                        pool[NPCID.CrimsonPenguin] = .05f;
                                    }

                                    if (ocean || Main.raining)
                                    {
                                        pool[NPCID.CorruptGoldfish] = .05f;
                                        pool[NPCID.CrimsonGoldfish] = .05f;
                                    }
                                }

                                if (bossCanSpawn && !NPC.downedMechBoss2)
                                    pool[NPCID.EyeofCthulhu] = .001f;

                                if (NPC.downedMechBossAny)
                                    pool[NPCID.Probe] = 0.01f;

                                if (NPC.downedPlantBoss) //GODLUL
                                {
                                    if (bossCanSpawn)
                                    {
                                        pool[NPCID.Retinazer] = .0001f;
                                        pool[NPCID.Spazmatism] = .0001f;
                                        pool[NPCID.TheDestroyer] = .0001f;
                                        pool[NPCID.SkeletronPrime] = .0001f;
                                    }

                                    //if (!spawnInfo.player.GetModPlayer<FargoSoulsPlayer>().SkullCharm)
                                    pool[NPCID.SkeletonSniper] = .005f;
                                    pool[NPCID.SkeletonCommando] = .005f;
                                    pool[NPCID.TacticalSkeleton] = .005f;
                                }
                            }

                            if (NPC.downedMechBossAny && noInvasion)
                            {
                                #region night pumpkin moon, frost moon
                                if (noBiome)
                                {
                                    pool[NPCID.Scarecrow1] = .01f;
                                    pool[NPCID.Scarecrow2] = .01f;
                                    pool[NPCID.Scarecrow3] = .01f;
                                    pool[NPCID.Scarecrow4] = .01f;
                                    pool[NPCID.Scarecrow5] = .01f;
                                    pool[NPCID.Scarecrow6] = .01f;
                                    pool[NPCID.Scarecrow7] = .01f;
                                    pool[NPCID.Scarecrow8] = .01f;
                                    pool[NPCID.Scarecrow9] = .01f;
                                    pool[NPCID.Scarecrow10] = .01f;

                                    if (NPC.downedHalloweenKing && bossCanSpawn)
                                    {
                                        //pool[NPCID.HeadlessHorseman] = .01f;
                                        pool[NPCID.Pumpking] = .0025f;
                                    }
                                }
                                else //in some biome
                                {
                                    if (hallow)
                                    {
                                        pool[NPCID.PresentMimic] = .01f;
                                    }
                                    else if (crimson || corruption)
                                    {
                                        pool[NPCID.Splinterling] = .05f;

                                        if (NPC.downedHalloweenTree && bossCanSpawn)
                                        {
                                            pool[NPCID.MourningWood] = .0025f;
                                        }
                                    }

                                    if (snow)
                                    {
                                        pool[NPCID.ZombieElf] = .02f;
                                        pool[NPCID.ZombieElfBeard] = .02f;
                                        pool[NPCID.ZombieElfGirl] = .02f;
                                        pool[NPCID.Yeti] = .01f;

                                        pool[NPCID.ElfArcher] = .05f;
                                        pool[NPCID.ElfCopter] = .01f;

                                        if (NPC.downedChristmasTree && bossCanSpawn)
                                        {
                                            pool[NPCID.Everscream] = .0025f;
                                        }

                                        if (NPC.downedChristmasSantank && bossCanSpawn)
                                        {
                                            pool[NPCID.SantaNK1] = .0025f;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        if (hallow)
                        {
                            if (!Main.raining)
                                pool[NPCID.RainbowSlime] = .001f;
                            pool[NPCID.GingerbreadMan] = .05f;
                        }

                        if (snow && noInvasion)
                        {
                            if (!Main.raining && !spawnInfo.PlayerInTown)
                                pool[NPCID.IceGolem] = .01f;
                            /*pool[NPCID.SnowBalla] = .04f;
                            pool[NPCID.MisterStabby] = .04f;
                            pool[NPCID.SnowmanGangsta] = .04f;*/
                        }

                        if (ocean)
                        {
                            if (night)
                            {
                                pool[NPCID.CreatureFromTheDeep] = .02f;
                            }

                            pool[NPCID.PigronCorruption] = .01f;
                            pool[NPCID.PigronCrimson] = .01f;
                            pool[NPCID.PigronHallow] = .01f;
                            if (NPC.downedFishron && bossCanSpawn)
                                pool[NPCID.DukeFishron] = .0002f;
                        }
                        else if (desert)
                        {
                            pool[NPCID.DesertBeast] = .05f;
                        }

                        if (NPC.downedMechBossAny && Main.raining)
                        {
                            pool[NPCID.LightningBug] = .1f;
                        }

                        if (corruption)
                        {
                            pool[NPCID.SeekerHead] = .1f;
                        }

                        if (crimson)
                        {
                            pool[NPCID.IchorSticker] = .1f;
                        }
                    }
                    else if (wideUnderground)
                    {
                        if (desert && !corruption && !crimson)
                        {
                            pool[NPCID.DesertDjinn] = .05f;
                        }

                        if (deepUnderground && !jungle && !snow)
                        {
                            pool[NPCID.FireImp] = .02f;
                            pool[NPCID.LavaSlime] = .02f;
                        }

                        if (cavern)
                        {
                            if (noBiome && NPC.downedBoss3)
                                pool[NPCID.DarkCaster] = .05f;
                        }

                        if (!NPC.savedWizard && !NPC.AnyNPCs(NPCID.BoundWizard))
                            pool[NPCID.BoundWizard] = .5f;

                        if (dungeon && night && normalSpawn && bossCanSpawn)
                            pool[NPCID.SkeletronHead] = .00005f;

                        if (NPC.downedMechBossAny)
                        {
                            if (snow && !Main.dayTime) //frost moon underground
                            {
                                if (underground)
                                    pool[NPCID.Nutcracker] = .05f;

                                if (cavern)
                                {
                                    pool[NPCID.Krampus] = .025f;
                                    if (NPC.downedChristmasIceQueen && bossCanSpawn)
                                        pool[NPCID.IceQueen] = .0025f;
                                }
                            }
                        }

                        if (NPC.downedAncientCultist && dungeon && bossCanSpawn)
                            pool[NPCID.CultistBoss] = 0.00002f;

                        if (spawnInfo.Player.ZoneUndergroundDesert)
                        {
                            if (!hallow && !corruption && !crimson)
                            {
                                pool[NPCID.SandShark] = .2f;
                            }
                            else
                            {
                                if (hallow)
                                    pool[NPCID.SandsharkHallow] = .2f;
                                if (corruption)
                                    pool[NPCID.SandsharkCorrupt] = .2f;
                                if (crimson)
                                    pool[NPCID.SandsharkCrimson] = .2f;
                            }
                        }
                    }
                    else if (underworld)
                    {
                        pool[NPCID.LeechHead] = .025f;
                        pool[NPCID.BoneSerpentHead] = .025f;
                        pool[NPCID.BlazingWheel] = .05f;

                        if (bossCanSpawn && !FargoSoulsUtil.BossIsAlive(ref wallBoss, NPCID.WallofFlesh))
                            pool[NPCID.TheHungryII] = .03f;

                        if (NPC.downedMechBossAny)
                        {
                            pool[NPCID.BlazingWheel] = .05f;
                        }

                        if (NPC.downedPlantBoss)// && !spawnInfo.player.GetModPlayer<FargoSoulsPlayer>().SkullCharm)
                        {
                            pool[NPCID.DiabolistRed] = .001f;
                            pool[NPCID.DiabolistWhite] = .001f;
                            pool[NPCID.Necromancer] = .001f;
                            pool[NPCID.NecromancerArmored] = .001f;
                            pool[NPCID.RaggedCaster] = .001f;
                            pool[NPCID.RaggedCasterOpenCoat] = .001f;
                        }

                        if (FargoSoulsWorld.downedBetsy && bossCanSpawn)
                            pool[NPCID.DD2Betsy] = .0002f;
                    }
                    else if (sky)
                    {
                        if (normalSpawn)
                        {
                            pool[NPCID.AngryNimbus] = .05f;

                            if (NPC.downedGolemBoss)
                            {
                                pool[NPCID.SolarCrawltipedeHead] = .03f;
                                pool[NPCID.VortexHornetQueen] = .03f;
                                pool[NPCID.NebulaBrain] = .03f;
                                pool[NPCID.StardustJellyfishBig] = .03f;
                                pool[NPCID.AncientCultistSquidhead] = .03f;
                                pool[NPCID.CultistDragonHead] = .03f;
                            }
                            else if (NPC.downedMechBossAny)
                            {
                                pool[NPCID.SolarCrawltipedeHead] = .001f;
                                pool[NPCID.VortexHornetQueen] = .001f;
                                pool[NPCID.NebulaBrain] = .001f;
                                pool[NPCID.StardustJellyfishBig] = .001f;
                            }

                            if (NPC.downedMoonlord && bossCanSpawn)
                            {
                                pool[NPCID.MoonLordCore] = 0.0002f;
                            }
                        }
                    }

                    //height-independent biomes
                    if (corruption)
                    {
                        if (normalSpawn && bossCanSpawn)
                            pool[NPCID.EaterofWorldsHead] = .0002f;
                    }

                    if (crimson)
                    {
                        if (normalSpawn && bossCanSpawn)
                            pool[NPCID.BrainofCthulhu] = .0002f;
                    }

                    if (jungle)
                    {
                        if (normalSpawn && bossCanSpawn)
                            pool[NPCID.QueenBee] = .0001f;

                        if (!surface)
                        {
                            pool[NPCID.BigMimicJungle] = .0025f;

                            if (NPC.downedGolemBoss && bossCanSpawn)
                                pool[NPCID.Plantera] = .00005f;
                        }
                    }

                    if (spawnInfo.Lihzahrd && spawnInfo.SpawnTileType == TileID.LihzahrdBrick)
                    {
                        pool[NPCID.BlazingWheel] = .1f;
                        pool[NPCID.SpikeBall] = .1f;

                        if (NPC.downedPlantBoss)// && !spawnInfo.player.GetModPlayer<FargoSoulsPlayer>().SkullCharm)
                        {
                            const float rate = .05f;
                            pool[NPCID.BigMimicJungle] = rate;

                            pool[NPCID.DiabolistRed] = rate / 6;
                            pool[NPCID.DiabolistWhite] = rate / 6;
                            pool[NPCID.Necromancer] = rate / 6;
                            pool[NPCID.NecromancerArmored] = rate / 6;
                            pool[NPCID.RaggedCaster] = rate / 6;
                            pool[NPCID.RaggedCasterOpenCoat] = rate / 6;
                        }
                    }

                    if (ocean && spawnInfo.Water)
                    {
                        pool[NPCID.AnglerFish] = .1f;
                    }
                }
            }

            /*if (monsterMadhouse)
            {
                pool.Clear();
                if (MMWorld.MMPoints >= 0) //Goblin Army
                {

                }
                if (MMWorld.MMPoints >= 90 && MMWorld.MMPoints < 270) //OOA 1
                {

                }
                if (MMWorld.MMPoints >= 180) //Pirates
                {

                }
                if (MMWorld.MMPoints >= 270 && MMWorld.MMPoints < 630) //OOA2
                {

                }
                if (MMWorld.MMPoints >= 360) //Eclipse
                {

                }
                if (MMWorld.MMPoints >= 450) //Pumpkin Moon
                {

                }
                if (MMWorld.MMPoints >= 540) //Frost Moon
                {

                }
                if (MMWorld.MMPoints >= 540) //Martians
                {

                }
                if (MMWorld.MMPoints >= 630) //OOA3
                {

                }
                if (MMWorld.MMPoints >= 720) //Lunar Events
                {

                }
            }*/
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (npc.type == NPCID.Painter && FargoSoulsWorld.downedMutant && NPC.AnyNPCs(ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()))
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, ModContent.ItemType<ScremPainting>());
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            #region tim's concoction drops

            void TimsConcoctionDrop(IItemDropRule rule)
            {
                TimsConcoctionDropCondition dropCondition = new TimsConcoctionDropCondition();
                IItemDropRule conditionalRule = new LeadingConditionRule(dropCondition);
                conditionalRule.OnSuccess(rule);
                npcLoot.Add(conditionalRule);
            }

            switch (npc.type)
            {
                case NPCID.BlueSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(npc.netID == NPCID.Pinky ? ItemID.TeleportationPotion : ItemID.RecallPotion));
                    break;

                case NPCID.DemonEye:
                case NPCID.DemonEyeOwl:
                case NPCID.DemonEyeSpaceship:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.NightOwlPotion));
                    break;

                case NPCID.MossHornet:
                case NPCID.Hornet:
                case NPCID.HornetFatty:
                case NPCID.HornetHoney:
                case NPCID.HornetLeafy:
                case NPCID.HornetSpikey:
                case NPCID.HornetStingy:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.RagePotion, 1, 1, 2));
                    break;

                case NPCID.Bee:
                case NPCID.BeeSmall:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.WrathPotion, 2));
                    break;

                case NPCID.GoblinPeon:
                case NPCID.GoblinThief:
                case NPCID.GoblinWarrior:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.BattlePotion));
                    break;

                case NPCID.JungleBat:
                case NPCID.IceBat:
                case NPCID.Vampire:
                case NPCID.VampireBat:
                case NPCID.GiantFlyingFox:
                case NPCID.Hellbat:
                case NPCID.Lavabat:
                case NPCID.IlluminantBat:
                case NPCID.CaveBat:
                case NPCID.GiantBat:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.SonarPotion));
                    break;

                case NPCID.AngryBones:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesSword:
                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.TitanPotion));
                    break;

                case NPCID.DarkCaster:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.PotionOfReturn, 1, 1, 6));
                    break;

                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                case NPCID.GiantTortoise:
                case NPCID.IceTortoise:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.ThornsPotion, 1, 1, 6));
                    break;

                case NPCID.Zombie:
                case NPCID.BaldZombie:
                case NPCID.FemaleZombie:
                case NPCID.PincushionZombie:
                case NPCID.SlimedZombie:
                case NPCID.TwiggyZombie:
                case NPCID.ZombiePixie:
                case NPCID.ZombieSuperman:
                case NPCID.ZombieSweater:
                case NPCID.ZombieXmas:
                case NPCID.SwampZombie:
                case NPCID.SmallSwampZombie:
                case NPCID.BigSwampZombie:
                case NPCID.ZombieDoctor:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.StinkPotion));
                    break;

                case NPCID.Antlion:
                case NPCID.WalkingAntlion:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.BuilderPotion, 1, 1, 3));
                    break;

                case NPCID.WallCreeper:
                case NPCID.WallCreeperWall:
                case NPCID.BlackRecluse:
                case NPCID.BlackRecluseWall:
                case NPCID.JungleCreeper:
                case NPCID.JungleCreeperWall:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.TrapsightPotion, 1, 1, 3));
                    break;

                case NPCID.FireImp:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.InfernoPotion, 1, 1, 3));
                    break;

                case NPCID.Harpy:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.CalmingPotion, 1, 1, 3));
                    break;

                case NPCID.Crab:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.CratePotion, 1, 1, 3));
                    break;

                case NPCID.FlyingFish:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.FishingPotion, 1, 1, 3));
                    break;

                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.WaterWalkingPotion, 1, 1, 3));
                    break;

                case NPCID.Piranha:
                case NPCID.Arapaima:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.GillsPotion, 1, 1, 3));
                    break;

                case NPCID.BloodZombie:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.RegenerationPotion, 1, 1, 3));
                    break;

                case NPCID.LavaSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.WarmthPotion, 1, 1, 3));
                    break;

                case NPCID.UmbrellaSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.FeatherfallPotion, 1, 1, 3));
                    break;

                case NPCID.Drippler:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.HeartreachPotion, 1, 1, 3));
                    break;

                case NPCID.GiantWormHead:
                case NPCID.DiggerHead:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.WormholePotion, 1, 1, 3));
                    break;

                case NPCID.GreekSkeleton:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.AmmoReservationPotion, 1, 1, 3));
                    break;

                case NPCID.GraniteFlyer:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.IronskinPotion, 1, 1, 3));
                    break;

                case NPCID.GraniteGolem:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.EndurancePotion, 1, 1, 3));
                    break;

                case NPCID.Shark:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.FlipperPotion, 1, 1, 3));
                    break;

                case NPCID.GoblinArcher:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.ArcheryPotion, 1, 1, 3));
                    break;

                case NPCID.GoblinSorcerer:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.ManaRegenerationPotion, 1, 1, 3));
                    break;

                case NPCID.PinkJellyfish:
                case NPCID.BlueJellyfish:
                case NPCID.GreenJellyfish:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.ShinePotion, 1, 1, 3));
                    break;

                case NPCID.Salamander:
                case NPCID.Salamander2:
                case NPCID.Salamander3:
                case NPCID.Salamander4:
                case NPCID.Salamander5:
                case NPCID.Salamander6:
                case NPCID.Salamander7:
                case NPCID.Salamander8:
                case NPCID.Salamander9:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.InvisibilityPotion, 1, 1, 3));
                    break;

                case NPCID.MotherSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.SummoningPotion, 1, 1, 6));
                    break;

                case NPCID.Nymph:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.LovePotion, 1, 1, 6));
                    break;

                case NPCID.Tumbleweed:
                case NPCID.DesertBeast:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.SwiftnessPotion, 1, 1, 6));
                    break;

                case NPCID.TombCrawlerHead:
                case NPCID.DuneSplicerHead:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.HunterPotion, 1, 1, 6));
                    break;

                case NPCID.DoctorBones:
                case NPCID.Mimic:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 1, 6));
                    break;

                case NPCID.UndeadMiner:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.MiningPotion, 1, 1, 6));
                    break;

                case NPCID.Tim:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.MagicPowerPotion, 1, 1, 6));
                    break;

                case NPCID.BoneSerpentHead:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 1, 12));
                    break;


                case NPCID.Gnome:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.LuckPotionLesser, 1, 1, 6));
                    break;
                case NPCID.DungeonSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.LuckPotion, 1, 1, 6));
                    break;
                case NPCID.Clown:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.LuckPotionGreater, 1, 1, 6));
                    break;


                case NPCID.ChaosElemental:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.TeleportationPotion, 1, 1, 3));
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.PotionOfReturn, 1, 1, 6));
                    break;

                case NPCID.RainbowSlime:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.RegenerationPotion, 1, 1, 3));
                    break;

                case NPCID.RuneWizard:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.MagicPowerPotion, 1, 1, 6));
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.ManaRegenerationPotion, 1, 1, 6));
                    break;

                case NPCID.GoblinSummoner:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.SummoningPotion, 1, 1, 12));
                    break;

                case NPCID.PirateCaptain:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.AmmoReservationPotion, 1, 1, 12));
                    break;

                case NPCID.WyvernHead:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.GravitationPotion, 1, 1, 12));
                    break;

                case NPCID.BigMimicJungle:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.RedPotion));
                    goto case NPCID.BigMimicCorruption;
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicHallow:
                    TimsConcoctionDrop(ItemDropRule.Common(ItemID.LifeforcePotion, 1, 1, 12));
                    break;

                default: break;
            }
            #endregion


            //if (npc.ModNPC == null || npc.ModNPC.Mod is FargowiltasSouls) //not for other mods
            //{
            int allowedRecursionDepth = 10;

            void AddDrop(IItemDropRule dropRule)
            {
                if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
                {
                    LeadingConditionRule noTwin = new LeadingConditionRule(new Conditions.MissingTwin());
                    noTwin.OnSuccess(dropRule);
                    npcLoot.Add(noTwin);
                }
                else if (npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail)
                {
                    LeadingConditionRule lastEater = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
                    lastEater.OnSuccess(dropRule);
                    npcLoot.Add(lastEater);
                }
                else
                {
                    npcLoot.Add(dropRule);
                }
            }

            void CheckMasterDropRule(IItemDropRule dropRule)
            {
                if (--allowedRecursionDepth > 0)
                {
                    foreach (IItemDropRuleChainAttempt chain in dropRule.ChainedRules)
                    {
                        CheckMasterDropRule(chain.RuleToChain);
                    }

                    if (dropRule is DropBasedOnMasterMode dropBasedOnMasterMode)
                    {
                        CheckMasterDropRule(dropBasedOnMasterMode.ruleForMasterMode);
                        //if (dropBasedOnMasterMode.ruleForMasterMode is CommonDrop masterDrop)
                        //{
                        //    IItemDropRule emodeDropRule = ItemDropRule.ByCondition(
                        //        new EModeNotMasterDropCondition(),
                        //        masterDrop.itemId,
                        //        masterDrop.chanceDenominator,
                        //        masterDrop.amountDroppedMinimum,
                        //        masterDrop.amountDroppedMaximum,
                        //        masterDrop.chanceNumerator
                        //    );
                        //    npcLoot.Add(emodeDropRule);
                        //}
                    }
                }
                allowedRecursionDepth++;

                //if (dropRule is CommonDrop drop)
                //{
                if (dropRule is ItemDropWithConditionRule itemDropWithCondition && itemDropWithCondition.condition is Conditions.IsMasterMode)
                {
                    IItemDropRule emodeDropRule = ItemDropRule.ByCondition(
                        new EModeNotMasterDropCondition(),
                        itemDropWithCondition.itemId,
                        itemDropWithCondition.chanceDenominator,
                        itemDropWithCondition.amountDroppedMinimum,
                        itemDropWithCondition.amountDroppedMaximum,
                        itemDropWithCondition.chanceNumerator
                    );
                    //itemDropWithCondition.OnFailedConditions(emodeDropRule, true);
                    AddDrop(emodeDropRule);
                }
                else if (dropRule is DropPerPlayerOnThePlayer dropPerPlayer && dropPerPlayer.condition is Conditions.IsMasterMode)
                {
                    IItemDropRule emodeDropRule = ItemDropRule.ByCondition(
                        new EModeNotMasterDropCondition(),
                        dropPerPlayer.itemId,
                        dropPerPlayer.chanceDenominator,
                        dropPerPlayer.amountDroppedMinimum,
                        dropPerPlayer.amountDroppedMaximum,
                        dropPerPlayer.chanceNumerator
                    );
                    //dropPerPlayer.OnFailedConditions(emodeDropRule, true);
                    AddDrop(emodeDropRule);
                }
                //}
            }

            foreach (IItemDropRule rule in npcLoot.Get())
            {
                CheckMasterDropRule(rule);
            }
            //}
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (FargoSoulsWorld.EternityMode && BeetleOffenseAura)
            {
                damage = (int)(damage * 1.25f);
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            //ModifyHitByEither(npc, player, ref damage, ref knockback, ref crit);

            if (FargoSoulsWorld.EternityMode)
            {
                if (NPCID.Sets.CountsAsCritter[npc.type]) //npc.catchItem != 0 && npc.lifeMax == 5)
                    player.AddBuff(ModContent.BuffType<Guilty>(), 300);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];

            //ModifyHitByEither(npc, player, ref damage, ref knockback, ref crit);

            if (FargoSoulsWorld.EternityMode)
            {
                if (NPCID.Sets.CountsAsCritter[npc.type] /*npc.catchItem != 0 && npc.lifeMax == 5*/ && projectile.friendly && !projectile.hostile && projectile.type != ProjectileID.FallingStar)
                    player.AddBuff(ModContent.BuffType<Guilty>(), 300);
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                if (BeetleDefenseAura)
                    damage *= 0.75;

                if (PaladinsShield)
                    damage *= 0.5;

                if (FargoSoulsWorld.MasochistModeReal && (npc.boss || (FargoSoulsUtil.AnyBossAlive() && npc.Distance(Main.npc[FargoSoulsGlobalNPC.boss].Center) < 3000)))
                    damage *= 0.9;
            }

            //normal damage calc
            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        //make aura enemies display them one day(tm)
        /*public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            for (int i = 0; i < auraDebuffs.Count; i++)
            {
                Texture2D buffIcon = Main.buffTexture[auraDebuffs[i]];
                Color buffColor = drawColor * 0.5f;
                Vector2 drawPos = npc.Top;
                drawPos.Y -= 32f;
                float mid = auraDebuffs.Count / 2f - 0.5f;
                drawPos.X -= 32f * (i - mid);
                Main.EntitySpriteDraw(buffIcon, drawPos - Main.screenPosition + new Vector2(0f, npc.gfxOffY), buffIcon.Bounds, buffColor, 0, buffIcon.Bounds.Size() / 2, 1f, SpriteEffects.None, 0);
            }
        }*/

        public static bool StealFromInventory(Player target, ref Item item)
        {
            if (target.GetModPlayer<FargoSoulsPlayer>().StealingCooldown <= 0 && !item.IsAir)
            {
                target.GetModPlayer<FargoSoulsPlayer>().StealingCooldown = 900; //trust me, keep these separate
                target.AddBuff(ModContent.BuffType<ThiefCD>(), 900);


                //int i = Item.NewItem(target.GetSource_DropAsItem("Stolen"), (int)target.position.X, (int)target.position.Y, target.width, target.height, item.type, item.stack, false, -1, false, false);
                //Vector2 position = Main.item[i].position;

                //Main.item[i] = item.Clone();
                //Main.item[i].whoAmI = i;
                //Main.item[i].position = position;
                //Main.item[i].stack = item.stack;

                //Main.item[i].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
                //Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                //Main.item[i].noGrabDelay = 100;
                //Main.item[i].newAndShiny = false;

                //if (Main.netMode == NetmodeID.MultiplayerClient)
                //    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);

                //item.TurnToAir();

                target.DropItem(target.GetSource_DropAsItem("Stolen"), target.Center, ref item);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Horde(NPC npc, int size)
        {
            int repeatTries = 50;

            for (int i = 0; i < size; i++)
            {
                Vector2 pos = new Vector2(npc.Center.X + Main.rand.NextFloat(-2f, 2f) * npc.width, npc.Center.Y);

                if (Collision.SolidCollision(pos, npc.width, npc.height))
                {
                    if (repeatTries > 0) //retry up to the max attempts
                    {
                        repeatTries -= 1;
                        i -= 1;
                    }
                    continue;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int j = NPC.NewNPC(npc.GetSource_FromAI(), (int)pos.X + npc.width / 2, (int)pos.Y + npc.height / 2, npc.type);
                    if (j != Main.maxNPCs)
                    {
                        NPC newNPC = Main.npc[j];
                        newNPC.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 5f;
                        newNPC.GetGlobalNPC<EModeNPCBehaviour>(false).FirstTick = false;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, j);
                    }
                }
            }
        }

        //for backwards compat
        public static void Aura(NPC npc, float distance, int buff, bool reverse = false, int dustid = DustID.GoldFlame, Color color = default)
        {
            Aura(npc, distance, reverse, dustid, color, buff);
        }

        public static void Aura(NPC npc, float distance, bool reverse = false, int dustid = -1, Color color = default, params int[] buffs)
        {
            Player p = Main.LocalPlayer;

            //if (FargowiltasSouls.Instance.MasomodeEXLoaded) distance *= reverse ? 0.5f : 2f;
            if (dustid != -1)
                FargoSoulsUtil.AuraDust(npc, distance, dustid, color, reverse);

            if (buffs.Length == 0 || buffs[0] < 0)
                return;

            //works because buffs are client side anyway :ech:
            float range = npc.Distance(p.Center);
            if (p.active && !p.dead && !p.ghost && (reverse ? (range > distance && range < Math.Max(3000f, distance * 2)) : range < distance))
            {
                foreach (int buff in buffs)
                {
                    FargoSoulsUtil.AddDebuffFixedDuration(p, buff, 2);
                }
            }
        }

        /*private void Shoot(NPC npc, int delay, float distance, int speed, int proj, int dmg, float kb, bool hostile = false, int dustID = -1)
        {
            int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
            if (t == -1)
                return;

            Player player = Main.player[t];
            //npc facing player target or if already started attack
            if (player.active && !player.dead && npc.direction == (Math.Sign(player.position.X - npc.position.X)) || Stop > 0)
            {
                //start the pause
                if (delay != 0 && Stop == 0 && npc.Distance(player.Center) < distance)
                {
                    Stop = delay;

                    //dust ring
                    if (dustID != -1)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 vector6 = Vector2.UnitY * 5f;
                            vector6 = vector6.RotatedBy((i - (20 / 2 - 1)) * 6.28318548f / 20) + npc.Center;
                            Vector2 vector7 = vector6 - npc.Center;
                            int d = Dust.NewDust(vector6 + vector7, 0, 0, dustID);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = vector7;
                            Main.dust[d].scale = 1.5f;
                        }
                    }

                }
                //half way through start attack
                else if (delay == 0 || Stop == delay / 2)
                {
                    Vector2 velocity = Vector2.Zero;

                    if (npc.Distance(player.Center) < distance || delay != 0)
                    {
                        velocity = Vector2.Normalize(player.Center - npc.Center) * speed;
                    }

                    if (velocity != Vector2.Zero)
                    {
                        int p = Projectile.NewProjectile(npc.Center, velocity, proj, dmg, kb, Main.myPlayer);
                        if (p < 1000)
                        {
                            if (hostile)
                            {
                                Main.projectile[p].friendly = false;
                                Main.projectile[p].hostile = true;
                            }
                        }

                        Counter[0] = 0;
                    } 
                }
            }
        }*/

        public static void CustomReflect(NPC npc, int dustID, int ratio = 1)
        {
            float distance = 2f * 16;

            Main.projectile.Where(x => x.active && x.friendly && !FargoSoulsUtil.IsSummonDamage(x, false)).ToList().ForEach(x =>
            {
                if (Vector2.Distance(x.Center, npc.Center) <= distance)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int dustId = Dust.NewDust(new Vector2(x.position.X, x.position.Y + 2f), x.width, x.height + 5, dustID, x.velocity.X * 0.2f, x.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                        Main.dust[dustId].noGravity = true;
                    }

                    // Set ownership
                    x.hostile = true;
                    x.friendly = false;
                    x.owner = Main.myPlayer;
                    x.damage /= ratio;

                    // Turn around
                    x.velocity *= -1f;

                    // Flip sprite
                    if (x.Center.X > npc.Center.X * 0.5f)
                    {
                        x.direction = 1;
                        x.spriteDirection = 1;
                    }
                    else
                    {
                        x.direction = -1;
                        x.spriteDirection = -1;
                    }

                    //x.netUpdate = true;
                }
            });
        }
    }
}
