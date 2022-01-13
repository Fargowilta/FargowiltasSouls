using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.Items.Tiles;
using FargowiltasSouls.Projectiles;
using Fargowiltas.NPCs;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.NPCs
{
    public partial class EModeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        //masochist doom
        public bool[] masoBool = new bool[4];
        public bool FirstTick = false;
        private int Stop = 0;
        public bool BeetleOffenseAura = false;
        public bool BeetleDefenseAura = false;
        public bool BeetleUtilAura = false;
        private int beetleTimer = 0;

        public bool PaladinsShield = false;
        public bool isWaterEnemy;
        public int[] Counter = new int[4];
        public byte SharkCount = 0;
        public bool canHurt;

        //public List<int> auraDebuffs = new List<int>();

        public static int boss = -1;
        public static int slimeBoss = -1;
        public static int eyeBoss = -1;
        public static int eaterBoss = -1;
        public static int brainBoss = -1;
        public static int beeBoss = -1;
        public static int skeleBoss = -1;
        public static int wallBoss = -1;
        public static int retiBoss = -1;
        public static int spazBoss = -1;
        public static int destroyBoss = -1;
        public static int primeBoss = -1;
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

            if (beetleTimer > 0 && --beetleTimer <= 0)
            {
                BeetleDefenseAura = false;
                BeetleOffenseAura = false;
                BeetleUtilAura = false;
            }
            
        }

        public override void SetDefaults(NPC npc)
        {
            Counter[2] = 600; //legacy

            canHurt = true;

            if (!FargoSoulsWorld.EternityMode) return;

            npc.value = (int)(npc.value * 1.3);

            switch (npc.type)
            {
                case NPCID.SolarFlare:
                    npc.noTileCollide = true;
                    if (FargoSoulsUtil.BossIsAlive(ref cultBoss, NPCID.CultistBoss) && npc.Distance(Main.npc[cultBoss].Center) < 3000)
                        npc.damage = (int)(npc.damage * .6);
                    break;

                case NPCID.Pixie:
                    npc.noTileCollide = true;
                    break;

                case NPCID.DungeonGuardian:
                    npc.boss = true;
                    npc.lifeMax /= 4;
                    break;

                case NPCID.WyvernHead:
                    if (Main.hardMode)
                    {
                        npc.lifeMax *= 2;
                    }
                    else
                    {
                        npc.defense /= 2;
                    }
                    Counter[0] = Main.rand.Next(180);
                    break;

                case NPCID.Moth:
                    npc.lifeMax *= 2;
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
                    npc.Opacity /= 5;
                    break;

                case NPCID.Mothron:
                case NPCID.MothronSpawn:
                    npc.knockBackResist *= .1f;
                    break;

                case NPCID.Butcher:
                    npc.knockBackResist = 0f;
                    break;

                case NPCID.ChaosElemental:
                    npc.buffImmune[BuffID.Confused] = true;
                    break;

                case NPCID.LostGirl:
                case NPCID.Nymph:
                    npc.lavaImmune = true;
                    npc.buffImmune[BuffID.Confused] = true;
                    if (Main.hardMode)
                    {
                        npc.lifeMax *= 4;
                        npc.damage *= 2;
                        npc.defense *= 2;
                    }
                    break;

                case NPCID.Shark:
                    Counter[1] = Main.rand.Next(60);
                    break;

                case NPCID.Piranha:
                    Counter[1] = Main.rand.Next(120);
                    break;

                case NPCID.DD2SkeletonT1:
                case NPCID.DD2SkeletonT3:
                    Counter[0] = Main.rand.Next(180);
                    break;

                case NPCID.BloodFeeder:
                    npc.lifeMax *= 4;
                    break;

                case NPCID.VileSpit:
                    npc.scale *= 2f;
                    if (FargoSoulsUtil.BossIsAlive(ref eaterBoss, NPCID.EaterofWorldsHead))
                        npc.damage = (int)(npc.damage * 0.75);
                    break;

                case NPCID.PirateShip:
                    npc.noTileCollide = true;
                    break;

                case NPCID.PirateShipCannon:
                    npc.noTileCollide = true;
                    Counter[1] = Main.rand.Next(10);
                    break;

                case NPCID.MisterStabby:
                case NPCID.AnglerFish:
                    npc.Opacity /= 5;
                    break;

                case NPCID.SolarSolenian:
                    npc.knockBackResist = 0f;
                    break;

                case NPCID.Hellhound:
                    npc.lavaImmune = true;
                    break;

                case NPCID.WalkingAntlion:
                    npc.knockBackResist = .4f;
                    break;

                case NPCID.Tumbleweed:
                    npc.knockBackResist = .1f;
                    break;

                case NPCID.DesertBeast:
                    npc.lifeMax *= 2;
                    npc.knockBackResist = 0f;
                    break;

                case NPCID.DuneSplicerHead:
                case NPCID.DuneSplicerBody:
                case NPCID.DuneSplicerTail:
                    if (Main.hardMode)
                        npc.lifeMax *= 3;
                    else
                    {
                        npc.defense /= 2;
                        npc.damage /= 2;
                    }
                    break;

                case NPCID.Parrot:
                    npc.noTileCollide = true;
                    break;

                case NPCID.SolarSroller:
                    npc.scale += 0.5f;
                    break;

                case NPCID.ChaosBall:
                    npc.dontTakeDamage = Main.hardMode;
                    break;

                case NPCID.DoctorBones:
                case NPCID.Lihzahrd:
                case NPCID.FlyingSnake:
                    npc.trapImmune = true;
                    break;

                case NPCID.SolarCrawltipedeTail:
                    npc.trapImmune = true;
                    break;

                case NPCID.SolarGoop:
                    npc.noTileCollide = true;
                    npc.buffImmune[BuffID.OnFire] = true;
                    npc.lavaImmune = true;
                    break;

                case NPCID.Clown:
                    npc.lifeMax *= 2;
                    break;

                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerNebula:
                case NPCID.LunarTowerStardust:
                case NPCID.LunarTowerVortex:
                    npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
                    break;

                case NPCID.CultistArcherWhite:
                    npc.chaseable = true;
                    npc.lavaImmune = false;
                    npc.value = Item.buyPrice(0, 1);
                    npc.lifeMax *= 2;
                    break;

                case NPCID.Reaper:
                    Counter[2] = 0;
                    break;

                case NPCID.EnchantedSword:
                case NPCID.CursedHammer:
                case NPCID.CrimsonAxe:
                    npc.scale = 2f;
                    npc.lifeMax *= 4;
                    npc.defense *= 2;
                    npc.knockBackResist = 0f;
                    break;

                case NPCID.Pumpking:
                case NPCID.IceQueen:
                    npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
                    break;

                case NPCID.Mimic:
                case NPCID.Medusa:
                case NPCID.PigronCorruption:
                case NPCID.PigronCrimson:
                case NPCID.PigronHallow:
                case NPCID.IchorSticker:
                case NPCID.SeekerHead:
                case NPCID.AngryNimbus:
                case NPCID.RedDevil:
                case NPCID.MushiLadybug:
                case NPCID.AnomuraFungus:
                case NPCID.ZombieMushroom:
                case NPCID.ZombieMushroomHat:
                    if (!Main.hardMode)
                    {
                        npc.defense /= 2;
                    }
                    break;

                case NPCID.Derpling:
                    npc.scale *= .4f;
                    break;

                case NPCID.BlazingWheel:
                    npc.scale *= 1.5f;
                    break;

                default:
                    break;
            }

            // +2.5% hp each kill 
            // +1.25% damage each kill
            // max of 4x hp and 2.5x damage
            //pre hm get 8x and 5x
            switch (npc.type)
            {
                #region boss scaling
                
                case NPCID.Hornet:
                case NPCID.HornetFatty:
                case NPCID.HornetHoney:
                case NPCID.HornetLeafy:
                case NPCID.HornetSpikey:
                case NPCID.HornetStingy:
                case NPCID.Bee:
                case NPCID.BeeSmall:
                    npc.buffImmune[BuffID.Poisoned] = true;
                    break;
                    
                case NPCID.SolarGoop:
                case NPCID.SolarFlare:
                    npc.buffImmune[BuffID.OnFire] = true;
                    npc.buffImmune[BuffID.Suffocation] = true;
                    break;

                case NPCID.SolarCrawltipedeHead:
                case NPCID.SolarCrawltipedeBody:
                case NPCID.SolarCrawltipedeTail:
                case NPCID.VortexHornetQueen:
                case NPCID.NebulaBrain:
                case NPCID.StardustJellyfishBig:
                case NPCID.MartianProbe:
                    npc.buffImmune[BuffID.Suffocation] = true;
                    break;

                #endregion

                #region enemy biome immunities

                case NPCID.Hellbat:
                case NPCID.LavaSlime:
                case NPCID.FireImp:
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                case NPCID.BoneSerpentBody:
                case NPCID.BoneSerpentHead:
                case NPCID.BoneSerpentTail:
                case NPCID.Lavabat:
                case NPCID.RedDevil:
                case NPCID.BurningSphere:
                    npc.buffImmune[BuffID.OnFire] = true;
                    break;
                    
                case NPCID.WyvernBody:
                case NPCID.WyvernBody2:
                case NPCID.WyvernBody3:
                case NPCID.WyvernHead:
                case NPCID.WyvernLegs:
                case NPCID.WyvernTail:
                case NPCID.AngryNimbus:
                    npc.buffImmune[BuffID.Suffocation] = true;
                    break;

                case NPCID.BlueJellyfish:
                case NPCID.Crab:
                case NPCID.PinkJellyfish:
                case NPCID.Piranha:
                case NPCID.SeaSnail:
                case NPCID.Shark:
                case NPCID.Squid:
                case NPCID.AnglerFish:
                case NPCID.Arapaima:
                case NPCID.BloodFeeder:
                case NPCID.BloodJelly:
                case NPCID.FungoFish:
                case NPCID.GreenJellyfish:
                case NPCID.Goldfish:
                case NPCID.CorruptGoldfish:
                case NPCID.CrimsonGoldfish:
                case NPCID.WaterSphere:
                case NPCID.Frog:
                case NPCID.GoldFrog:
                case NPCID.Grubby:
                case NPCID.Sluggy:
                case NPCID.Buggy:
                    isWaterEnemy = true;
                    break;

                #endregion

                default:
                    break;
            }

            //if (npc.boss) npc.npcSlots += 100;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return canHurt;
        }

        public override bool? CanHitNPC(NPC npc, NPC target)
        {
            if (!canHurt)
                return false;

            return null;
        }

        private void transformZombie(NPC npc, int armedId = -1)
        {
            if (Main.LocalPlayer.ZoneSnow && Main.rand.NextBool())
            {
                npc.Transform(NPCID.ZombieEskimo);
            }

            if (Main.rand.NextBool(8))
                Horde(npc, 6);
            if (armedId != -1 && Main.rand.NextBool(5))
                npc.Transform(armedId);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.boss)
            {
                boss = npc.whoAmI;
            }

            if (!Main.dayTime && !Main.hardMode)
            {
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                if (y < Main.worldSurface && y > 0 && x > 0 && x < Main.maxTilesX)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile != null && tile.wall == 0)
                    {
                        Lighting.AddLight(npc.Center, 0.5f, 0.5f, 0.5f);
                    }
                }
            }

            if (!FirstTick)
            {
                //transformations, hordes, and other first tick maso stuff
                if (FargoSoulsWorld.EternityMode)
                {
                    switch (npc.type)
                    {
                        case NPCID.Zombie: transformZombie(npc, NPCID.ArmedZombie); break;
                        case NPCID.ZombieEskimo: transformZombie(npc, NPCID.ArmedZombieEskimo); break;
                        case NPCID.PincushionZombie: transformZombie(npc, NPCID.ArmedZombiePincussion); break;
                        case NPCID.FemaleZombie: transformZombie(npc, NPCID.ArmedZombieCenx); break;
                        case NPCID.SlimedZombie: transformZombie(npc, NPCID.ArmedZombieSlimed); break;
                        case NPCID.TwiggyZombie: transformZombie(npc, NPCID.ArmedZombieTwiggy); break;
                        case NPCID.SwampZombie: transformZombie(npc, NPCID.ArmedZombieSwamp); break;
                        case NPCID.Skeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton); break;
                        case NPCID.HeadacheSkeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton2); break;
                        case NPCID.MisassembledSkeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton3); break;
                        case NPCID.PantlessSkeleton: if (Main.rand.NextBool(5)) npc.Transform(NPCID.BoneThrowingSkeleton4); break;
                        case NPCID.JungleSlime: if (Main.rand.NextBool(5)) npc.Transform(NPCID.SpikedJungleSlime); break;
                        case NPCID.IceSlime: if (Main.rand.NextBool(5)) npc.Transform(NPCID.SpikedIceSlime); break;

                        /*case NPCID.CaveBat:
                        case NPCID.JungleBat:
                        case NPCID.Hellbat:
                        case NPCID.IceBat:
                        case NPCID.GiantBat:
                        case NPCID.IlluminantBat:
                        case NPCID.Lavabat:
                        case NPCID.GiantFlyingFox:
                            if (Main.rand.NextBool(4))
                                Horde(npc, Main.rand.Next(5) + 1);
                            break;*/

                        case NPCID.Shark:
                            if (Main.rand.NextBool(4))
                                Horde(npc, 2);
                            break;

                        case NPCID.WyvernHead:
                            if (Main.hardMode && Main.rand.NextBool(4))
                                Horde(npc, 2);
                            break;

                        case NPCID.Wolf:
                            if (Main.rand.NextBool(3))
                                Horde(npc, 5);
                            break;

                        case NPCID.FlyingFish:
                            if (Main.rand.NextBool(4))
                                Horde(npc, 3);
                            break;

                        case NPCID.ChaosElemental:
                            if (Main.rand.NextBool(3))
                                Horde(npc, Main.rand.Next(3, 10));
                            break;

                        case NPCID.Ghost:
                            if (Main.rand.NextBool(5))
                                Horde(npc, 3);
                            break;

                        case NPCID.GreekSkeleton:
                            if (Main.rand.NextBool(3))
                                Horde(npc, 3);
                            break;

                        case NPCID.Demon:
                        case NPCID.RedDevil:
                            if (Main.hardMode && Main.rand.NextBool(5))
                                Horde(npc, 5);
                            break;

                        case NPCID.GiantTortoise:
                        case NPCID.IceTortoise:
                            if (Main.rand.NextBool(4))
                                Horde(npc, 5);
                            break;
                            
                        case NPCID.CultistArcherWhite:
                            if (Main.rand.NextBool(5) && NPC.downedGolemBoss)
                                Horde(npc, 7);
                            break;

                        case NPCID.FlyingAntlion:
                            if (Main.rand.NextBool(3))
                                Horde(npc, 3);
                            break;

                        case NPCID.Crawdad:
                        case NPCID.GiantShelly:
                            if (Main.rand.NextBool(5)) //pick a random salamander
                                npc.Transform(Main.rand.Next(498, 507));
                            break;

                        case NPCID.Salamander:
                        case NPCID.Salamander2:
                        case NPCID.Salamander3:
                        case NPCID.Salamander4:
                        case NPCID.GiantShelly2:
                            if (Main.rand.NextBool(5)) //pick a random crawdad
                                npc.Transform(Main.rand.Next(494, 496));
                            break;

                        case NPCID.Salamander5:
                        case NPCID.Salamander6:
                        case NPCID.Salamander7:
                        case NPCID.Salamander8:
                        case NPCID.Crawdad2:
                            if (Main.rand.NextBool(5)) //pick a random shelly
                                npc.Transform(Main.rand.Next(496, 498));
                            break;

                        case NPCID.Goldfish:
                        case NPCID.GoldfishWalker:
                        case NPCID.BlueJellyfish:
                            if (Main.rand.NextBool(6)) //random sharks
                                npc.Transform(NPCID.Shark);
                            break;

                        //sandshark swapping
                        case NPCID.SandShark:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.Next(NPCID.SandsharkCorrupt, NPCID.SandsharkHallow + 1));
                            break;

                        //ghoul swapping
                        case NPCID.DesertGhoul:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.Next(NPCID.DesertGhoulCorruption, NPCID.DesertGhoulHallow + 1));
                            break;

                        //armored bones swapping
                        case NPCID.RustyArmoredBonesAxe:
                        case NPCID.RustyArmoredBonesFlail:
                        case NPCID.RustyArmoredBonesSword:
                        case NPCID.RustyArmoredBonesSwordNoArmor:
                        case NPCID.BlueArmoredBones:
                        case NPCID.BlueArmoredBonesMace:
                        case NPCID.BlueArmoredBonesNoPants:
                        case NPCID.BlueArmoredBonesSword:
                        case NPCID.HellArmoredBones:
                        case NPCID.HellArmoredBonesMace:
                        case NPCID.HellArmoredBonesSpikeShield:
                        case NPCID.HellArmoredBonesSword:
                            if (Main.rand.NextBool(5))
                                npc.Transform(Main.rand.Next(NPCID.RustyArmoredBonesAxe, NPCID.HellArmoredBonesSword + 1));
                            break;

                        case NPCID.Mothron:
                            NPC.NewNPC((int)npc.Center.X - 100, (int)npc.Center.Y, NPCID.MothronSpawn);
                            NPC.NewNPC((int)npc.Center.X + 100, (int)npc.Center.Y, NPCID.MothronSpawn);
                            break;

                        case NPCID.BlueSlime:
                            if (Main.slimeRain)
                            {
                                if (Main.rand.NextBool(8))
                                {
                                    int[] slimes = { NPCID.RedSlime, NPCID.PurpleSlime, NPCID.YellowSlime, NPCID.BlackSlime, NPCID.SlimeSpiked };

                                    npc.SetDefaults(slimes[Main.rand.Next(slimes.Length)]);

                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                                }
                                /*else if (Main.rand.NextBool(50))
                                {
                                    npc.SetDefaults(NPCID.Pinky);

                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                                }*/
                            }
                                
                            break;


                        case NPCID.VortexLarva:
                            //if (Main.rand.NextBool()) npc.Transform(NPCID.VortexHornet);
                            break;

                        case NPCID.Bee:
                        case NPCID.BeeSmall:
                            if (Main.rand.NextBool(5))
                                switch ((Main.hardMode && !FargoSoulsUtil.BossIsAlive(ref beeBoss, NPCID.QueenBee)) ? Main.rand.Next(16, 21) : Main.rand.Next(16))
                                {
                                    case 0: npc.Transform(NPCID.Hornet); break;
                                    case 1: npc.Transform(NPCID.HornetFatty); break;
                                    case 2: npc.Transform(NPCID.HornetHoney); break;
                                    case 3: npc.Transform(NPCID.HornetLeafy); break;
                                    case 4: npc.Transform(NPCID.HornetSpikey); break;
                                    case 5: npc.Transform(NPCID.HornetStingy); break;
                                    case 6: npc.Transform(NPCID.LittleHornetFatty); break;
                                    case 7: npc.Transform(NPCID.LittleHornetHoney); break;
                                    case 8: npc.Transform(NPCID.LittleHornetLeafy); break;
                                    case 9: npc.Transform(NPCID.LittleHornetSpikey); break;
                                    case 10: npc.Transform(NPCID.LittleHornetStingy); break;
                                    case 11: npc.Transform(NPCID.BigHornetFatty); break;
                                    case 12: npc.Transform(NPCID.BigHornetHoney); break;
                                    case 13: npc.Transform(NPCID.BigHornetLeafy); break;
                                    case 14: npc.Transform(NPCID.BigHornetSpikey); break;
                                    case 15: npc.Transform(NPCID.BigHornetStingy); break;
                                    case 16: npc.Transform(NPCID.MossHornet); break;
                                    case 17: npc.Transform(NPCID.BigMossHornet); break;
                                    case 18: npc.Transform(NPCID.GiantMossHornet); break;
                                    case 19: npc.Transform(NPCID.LittleMossHornet); break;
                                    case 20: npc.Transform(NPCID.TinyMossHornet); break;
                                }
                            break;

                        case NPCID.Hornet:
                        case NPCID.HornetFatty:
                        case NPCID.HornetHoney:
                        case NPCID.HornetLeafy:
                        case NPCID.HornetSpikey:
                        case NPCID.HornetStingy:
                            /*if (Main.hardMode && !FargoSoulsUtil.BossIsAlive(ref beeBoss, NPCID.QueenBee))
                                switch (Main.rand.Next(5))
                                {
                                    case 0: npc.Transform(NPCID.MossHornet); break;
                                    case 1: npc.Transform(NPCID.BigMossHornet); break;
                                    case 2: npc.Transform(NPCID.GiantMossHornet); break;
                                    case 3: npc.Transform(NPCID.LittleMossHornet); break;
                                    case 4: npc.Transform(NPCID.TinyMossHornet); break;
                                }*/
                            break;

                        case NPCID.MeteorHead:
                            if (NPC.downedGolemBoss && Main.rand.NextBool(4))
                                npc.Transform(NPCID.SolarCorite);
                            break;

                        case NPCID.MothronEgg:
                            npc.Transform(NPCID.MothronSpawn);
                            break;

                        case NPCID.DiabolistRed:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.NextBool() ? NPCID.Necromancer : NPCID.RaggedCaster);
                            break;

                        case NPCID.DiabolistWhite:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.NextBool() ? NPCID.NecromancerArmored : NPCID.RaggedCasterOpenCoat);
                            break;

                        case NPCID.Necromancer:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistRed : NPCID.RaggedCaster);
                            break;

                        case NPCID.NecromancerArmored:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistWhite : NPCID.RaggedCasterOpenCoat);
                            break;

                        case NPCID.RaggedCaster:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistRed : NPCID.Necromancer);
                            break;

                        case NPCID.RaggedCasterOpenCoat:
                            if (Main.rand.NextBool(4))
                                npc.Transform(Main.rand.NextBool() ? NPCID.DiabolistWhite : NPCID.NecromancerArmored);
                            break;

                        default:
                            break;
                    }

                }

                FirstTick = true;
            }

            if (npc.aiStyle == 1)
            {
                npc.TargetClosest(true);
            }

            if (Stop > 0)
            {
                Stop--;
                npc.position = npc.oldPosition;
                npc.frameCounter = 0;
            }

            if (FargoSoulsWorld.EternityMode)
            {
                if (!npc.dontTakeDamage)
                {
                    if (npc.position.Y / 16 < Main.worldSurface * 0.35f) //enemy in space
                        npc.AddBuff(BuffID.Suffocation, 2);
                    else if (npc.position.Y / 16 > Main.maxTilesY - 200) //enemy in hell
                        npc.AddBuff(BuffID.OnFire, 2);

                    if (npc.wet && !npc.noTileCollide && !isWaterEnemy && npc.HasPlayerTarget)
                    {
                        npc.AddBuff(ModContent.BuffType<Lethargic>(), 2);
                        if (Main.player[npc.target].ZoneCorrupt)
                            npc.AddBuff(BuffID.CursedInferno, 2);
                        if (Main.player[npc.target].ZoneCrimson)
                            npc.AddBuff(BuffID.Ichor, 2);
                        if (Main.player[npc.target].ZoneHoly)
                            npc.AddBuff(BuffID.Confused, 2);
                        if (Main.player[npc.target].ZoneJungle)
                            npc.AddBuff(BuffID.Poisoned, 2);
                    }
                }

                if (!FargoSoulsWorld.SwarmActive)
                {
                    switch (npc.type)
                    {
                        #region bosses
                        case NPCID.SolarFlare:
                            npc.position += npc.velocity * Math.Min(0.5f, ++Counter[0] / 60f - 1f);
                            break;

                        #endregion
                            
                        case NPCID.CultistArcherWhite:
                            if (npc.ai[1] > 0)
                            {
                                if (npc.ai[1] == 41) //skip vanilla shooting
                                    npc.ai[1] = 39;

                                if (npc.ai[1] > 10 && npc.ai[1] < 40 && npc.ai[1] % 10 == 5 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.Y -= Math.Abs(speed.X) * 0.1f; //account for gravity
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();
                                    speed *= 12f;

                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<CultistArrow>(), npc.damage * 4 / 9, 0f, Main.myPlayer);
                                }
                            }
                            break;

                        case NPCID.DD2EterniaCrystal:
                            if (DD2Event.Ongoing && DD2Event.TimeLeftBetweenWaves > 600)
                                DD2Event.TimeLeftBetweenWaves = 600;

                            //cant use HasValidTarget for this because that returns true even if betsy is targeting the crystal (npc.target seems to become -1)
                            if (FargoSoulsUtil.BossIsAlive(ref betsyBoss, NPCID.DD2Betsy) && Main.npc[betsyBoss].HasPlayerTarget
                                && Main.player[Main.npc[betsyBoss].target].active && !Main.player[Main.npc[betsyBoss].target].dead && !Main.player[Main.npc[betsyBoss].target].ghost
                                && npc.Distance(Main.player[Main.npc[betsyBoss].target].Center) < 3000)
                            {
                                Counter[0] = 30; //even if betsy targets crystal, wait before becoming fully vulnerable
                                if (npc.life < npc.lifeMax && npc.life < 500)
                                    npc.life++;
                            }

                            if (Counter[0] > 0)
                                Counter[0]--;
                            break;

                        case NPCID.DesertBeast:
                            Aura(npc, 250, ModContent.BuffType<Infested>(), false, 188);
                            break;

                        case NPCID.CochinealBeetle: //damage up
                            Aura(npc, 400, -1, false, 60);
                            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != NPCID.CochinealBeetle && n.Distance(npc.Center) < 400))
                            {
                                n.GetGlobalNPC<EModeGlobalNPC>().BeetleOffenseAura = true;
                                n.GetGlobalNPC<EModeGlobalNPC>().beetleTimer = 60;
                                if (Main.rand.NextBool())
                                {
                                    int d = Dust.NewDust(n.position, n.width, n.height, 60, 0f, -1.5f, 0, new Color());
                                    Main.dust[d].velocity *= 0.5f;
                                    Main.dust[d].noLight = true;
                                }
                            }
                            break;
                        case NPCID.CyanBeetle: //freeze
                            Aura(npc, 400, -1, false, 187);
                            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != NPCID.CyanBeetle && n.Distance(npc.Center) < 400))
                            {
                                n.GetGlobalNPC<EModeGlobalNPC>().BeetleUtilAura = true;
                                n.GetGlobalNPC<EModeGlobalNPC>().beetleTimer = 60;

                                if (Main.rand.NextBool())
                                {
                                    int d = Dust.NewDust(n.position, n.width, n.height, 187, 0f, -1.5f, 0, new Color());
                                    Main.dust[d].velocity *= 0.5f;
                                    Main.dust[d].noLight = true;
                                }
                            }
                            break;
                        case NPCID.LacBeetle: //defense up
                            Aura(npc, 400, -1, false, 21);
                            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != NPCID.LacBeetle && n.Distance(npc.Center) < 400))
                            {
                                n.GetGlobalNPC<EModeGlobalNPC>().BeetleDefenseAura = true;
                                n.GetGlobalNPC<EModeGlobalNPC>().beetleTimer = 60;
                                if (Main.rand.NextBool())
                                {
                                    int d = Dust.NewDust(n.position, n.width, n.height, 21, 0f, -1.5f, 0, new Color());
                                    Main.dust[d].velocity *= 0.5f;
                                    Main.dust[d].noLight = true;
                                }
                            }

                            break;

                        case NPCID.EnchantedSword:
                        case NPCID.CursedHammer:
                        case NPCID.CrimsonAxe:
                            npc.position += npc.velocity / 2f;
                            Aura(npc, 300, BuffID.WitheredArmor, true, 119);
                            Aura(npc, 300, BuffID.WitheredWeapon, true, 14);
                            if (npc.ai[0] == 2f) //spinning up
                                npc.ai[1] += 6f * (1f - (float)npc.life / npc.lifeMax); //FINISH SPINNING FASTER
                            break;

                        case NPCID.Ghost:
                            Aura(npc, 100, BuffID.Cursed, false, 20);
                            break;

                        case NPCID.Snatcher:
                        case NPCID.ManEater:
                            if (++Counter[0] > 300 && npc.Distance(new Vector2((int)npc.ai[0] * 16, (int)npc.ai[1] * 16)) < 500)
                            {
                                Player target = Main.player[npc.target];
                                Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 15;
                                npc.velocity = velocity;
                                Counter[0] = 0;
                            }

                            if (npc.HasValidTarget && Main.player[npc.target].statLife < 100)
                                SharkCount = 2;
                            else
                                SharkCount = 0;
                            break;

                        case NPCID.AngryTrapper:
                            if (++Counter[0] > 120 && npc.Distance(new Vector2((int)npc.ai[0] * 16, (int)npc.ai[1] * 16)) < 1000 && npc.HasValidTarget)
                            {
                                Player target = Main.player[npc.target];
                                Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 15;
                                npc.velocity = velocity;
                                Counter[0] = 0;
                            }

                            if (npc.HasValidTarget && Main.player[npc.target].statLife < 180)
                                SharkCount = 2;
                            else
                                SharkCount = 0;
                            break;

                        case NPCID.Mummy:
                        case NPCID.DarkMummy:
                        case NPCID.LightMummy:
                            Aura(npc, 500, BuffID.Slow, false, 0);
                            break;

                        case NPCID.Derpling:
                            Aura(npc, 200, BuffID.Bleeding, false, DustID.Blood);

                            if (npc.Distance(Main.player[Main.myPlayer].Center) < 200)
                            {
                                Counter[0]++;

                                if (Counter[0] > 10)
                                {
                                    Player target = Main.player[Main.myPlayer];
                                    target.statLife -= 5;

                                    if (target.statLife < 0)
                                    {
                                        target.KillMe(PlayerDeathReason.ByCustomReason(target.name + Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.KilledbyDerpling")), 999, 0);
                                    }

                                    if (npc.life < npc.lifeMax)
                                    {
                                        npc.life += 5;
                                    }

                                    npc.HealEffect(5);

                                    Counter[0] = 0;
                                }
                            }
                                
                            break;

                        case NPCID.FaceMonster:
                            Aura(npc, 150, BuffID.Obstructed, false, 199);
                            break;

                        case NPCID.IlluminantBat:
                            if (masoBool[0])
                            {
                                if (!masoBool[1] && ++Counter[1] > 15) //MP sync
                                {
                                    masoBool[1] = true;
                                    NetUpdateMaso(npc.whoAmI);
                                }
                                npc.alpha = 200;
                                if (npc.lifeMax > 100)
                                    npc.lifeMax = 100;
                                if (npc.life > npc.lifeMax)
                                    npc.life = npc.lifeMax;
                            }
                            else if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 1000)
                            {
                                if (++Counter[0] >= 600)
                                {
                                    Counter[0] = 0;
                                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.IlluminantBat) < 10)
                                    {
                                        int bat = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.IlluminantBat);
                                        if (bat < Main.maxNPCs)
                                        {
                                            Main.npc[bat].velocity.X = Main.rand.Next(-5, 6);
                                            Main.npc[bat].velocity.Y = Main.rand.Next(-5, 6);
                                            Main.npc[bat].GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = true;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, bat);
                                        }
                                    }
                                }
                            }
                            break;

                        case NPCID.MeteorHead:
                            Counter[0]++;
                            if (Counter[0] >= 120)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1 && npc.Distance(Main.player[t].Center) < 600 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    npc.velocity *= 5;
                                    npc.netUpdate = true;
                                }
                            }
                            Aura(npc, 100, BuffID.Burning, false, DustID.Fire);
                            break;

                        case NPCID.BoneSerpentHead:
                            Counter[0]++;
                            if (Counter[0] >= 300)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1 && npc.Distance(Main.player[t].Center) < 600 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.BurningSphere);
                                    if (n != 200 && Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                            break;

                        case NPCID.BlueSlime:
                            //always be bouncing
                            if (npc.netID == NPCID.Pinky)
                            {
                                npc.ai[0] = -2000;
                            }
                            break;
                        
                        case NPCID.ZombieRaincoat:
                            if (npc.wet)
                            {
                                //slime ai
                                npc.aiStyle = 1;
                            }
                            else
                            {
                                //zombie ai
                                npc.aiStyle = 3;
                            }
                            goto case NPCID.Zombie;

                        case NPCID.UmbrellaSlime:
                            if (npc.wet)
                            {
                                Counter[0] = 30;
                            }

                            if (Counter[0] > 0)
                            {
                                Counter[0]--;
                            }

                            if (Counter[0] <= 0 && npc.velocity.Y > 0)
                            {
                                npc.velocity.Y /= 10;
                            }
                            break;

                        case NPCID.FlyingFish:
                            Counter[0]++;
                            if (Counter[0] >= 70)
                                Shoot(npc, 0, 250, 10, ProjectileID.WaterStream, npc.damage / 4, 1, true);
                            break;

                        case NPCID.ArmoredSkeleton:
                            Counter[0]++;
                            if (Counter[0] >= 300)
                                Shoot(npc, 0, 500, 10, ProjectileID.SwordBeam, npc.damage / 4, 1, true, DustID.AmberBolt);
                            break;

                        case NPCID.Vulture:
                            if (npc.ai[0] != 0f)
                            {
                                Counter[0]++;
                                if (Counter[0] >= 150)
                                    Shoot(npc, 30, 500, 10, ModContent.ProjectileType<VultureFeather>(), npc.damage / 4, 1);
                            }
                            break;

                        case NPCID.DoctorBones:
                            Counter[0]++;
                            if (Counter[0] >= 600)
                                Shoot(npc, 120, 1000, 14, ProjectileID.Boulder, npc.damage, 2);
                            break;

                        case NPCID.Crab:
                            Counter[0]++;
                            if (Counter[0] >= 300)
                                Shoot(npc, 30, 800, 14, ProjectileID.Bubble, npc.damage / 4, 1, true);
                            break;

                        case NPCID.ArmoredViking:
                            Counter[0]++;
                            if (Counter[0] >= 10)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget //collision check to reduce spam when not relevant
                                    && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                                {
                                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 14f;
                                    Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<IceSickleHostile>(), npc.damage / 4, 0f, Main.myPlayer);
                                }
                            }
                            break;

                        case NPCID.Crawdad:
                        case NPCID.Crawdad2:
                            Counter[0]++;
                            if (Counter[0] >= 300)
                            {
                                Counter[0] = 0;
                                Shoot(npc, 30, 800, 14, ProjectileID.Bubble, npc.damage / 4, 1, true);
                                /*int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1)
                                {
                                    Player player = Main.player[t];
                                    if (npc.Distance(player.Center) < 800 & Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 velocity = Vector2.Normalize(player.Center - npc.Center) * 10;
                                        int bubble = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.DetonatingBubble);
                                        if (bubble < 200)
                                        {
                                            Main.npc[bubble].velocity = velocity;
                                            Main.npc[bubble].damage = npc.damage / 2;
                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, bubble);
                                        }
                                    }
                                }*/
                            }
                            break;

                        case NPCID.WallCreeperWall:
                        case NPCID.BloodCrawlerWall:
                        case NPCID.JungleCreeperWall:
                            if (++Counter[0] >= 360)
                                Shoot(npc, 60, 400, 14, ProjectileID.WebSpit, 9, 0, false, DustID.Web);
                            break;

                        case NPCID.SeekerHead:
                            Counter[0]++;
                            if (Counter[0] >= 10)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1 && npc.Distance(Main.player[t].Center) < 500)
                                    Projectile.NewProjectile(npc.Center, npc.velocity, ProjectileID.EyeFire, npc.damage / 4, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.Demon:
                            //Counter++;


                            if (npc.ai[0] == 100f)
                            {
                                //Counter = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1 && npc.Distance(Main.player[t].Center) < 800 && Main.netMode != NetmodeID.MultiplayerClient)
                                    FargoSoulsUtil.XWay(6, npc.Center, ProjectileID.DemonSickle, 1, npc.damage / 4, .5f);
                            }
                            break;

                        case NPCID.VoodooDemon:
                            if (npc.lavaWet && npc.HasValidTarget && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                            {
                                npc.buffImmune[BuffID.OnFire] = false;
                                npc.AddBuff(BuffID.OnFire, 600);
                            }
                            if (!npc.HasBuff(BuffID.OnFire))
                            {
                                Counter[2] = 600;
                            }
                            else
                            {
                                Main.PlaySound(SoundID.NPCKilled, (int)npc.position.X, (int)npc.position.Y, 10, 1f, 0.5f);
                                Counter[2]--;
                                if (Counter[2] <= 0 && !FargoSoulsUtil.BossIsAlive(ref wallBoss, NPCID.WallofFlesh) && npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int guide = NPC.FindFirstNPC(NPCID.Guide);
                                    if (guide != -1 && Main.npc[guide].active)
                                    {
                                        Main.npc[guide].StrikeNPC(9999, 0f, 0);
                                        NPC.SpawnWOF(Main.player[npc.target].Center);
                                        /*if (Main.netMode == NetmodeID.SinglePlayer)
                                            Main.NewText("Wall of Flesh has awoken!", 175, 75, 255);
                                        else if (Main.netMode == NetmodeID.Server)
                                            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Wall of Flesh has awoken!"), new Color(175, 75, 255));*/
                                    }
                                    npc.Transform(NPCID.Demon);
                                }
                            }
                            if (Counter[2] < 600 - 60)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, 0f, 0f, 0, default(Color), (1f - Counter[2] / 600f) * 5f);
                                    Main.dust[d].noGravity = !Main.rand.NextBool(5);
                                    Main.dust[d].noLight = true;
                                    Main.dust[d].velocity *= Main.rand.NextFloat(12f);
                                }
                            }
                            break;

                        case NPCID.Piranha:
                            masoBool[0] = npc.HasValidTarget && Main.player[npc.target].bleed && Main.player[npc.target].ZoneJungle;
                            Counter[0]++;
                            if (Counter[0] >= 120) //swarm
                            {
                                Counter[0] = 0;
                                if (Main.rand.NextBool() && masoBool[0] && Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.Piranha) <= 6)
                                {
                                    int piranha = NPC.NewNPC((int)npc.Center.X + Main.rand.Next(-20, 20), (int)npc.Center.Y + Main.rand.Next(-20, 20), NPCID.Piranha);
                                    if (piranha != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, piranha);
                                }
                            }
                            if (masoBool[0] && npc.wet && ++Counter[1] > 240) //initiate jump
                            {
                                Counter[1] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (Main.rand.NextBool() && t != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    const float gravity = 0.3f;
                                    const float time = 120f;
                                    Vector2 distance;
                                    if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                                        distance = Main.player[t].Center - npc.Center;
                                    else
                                        distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                                    distance.X = distance.X / time;
                                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                                    npc.ai[1] = 120f;
                                    npc.ai[2] = distance.X;
                                    npc.ai[3] = distance.Y;
                                    npc.netUpdate = true;
                                }
                            }
                            if (npc.ai[1] > 0f) //while jumping
                            {
                                npc.ai[1]--;
                                npc.noTileCollide = true;
                                npc.velocity.X = npc.ai[2];
                                npc.velocity.Y = npc.ai[3];
                                npc.ai[3] += 0.3f;
                                masoBool[0] = false;

                                int num22 = 5;
                                for (int index1 = 0; index1 < num22; ++index1)
                                {
                                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                                    Main.dust[index2].noGravity = true;
                                    Main.dust[index2].noLight = true;
                                    Main.dust[index2].velocity /= 4f;
                                    Main.dust[index2].velocity -= npc.velocity;
                                }
                            }
                            else
                            {
                                if (npc.noTileCollide)
                                    npc.noTileCollide = Collision.SolidCollision(npc.position, npc.width, npc.height);
                            }
                            break;

                        case NPCID.Arapaima:
                            if (++Counter[1] > 420) //initiate jump
                            {
                                Counter[1] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (npc.life < npc.lifeMax && t != -1 && Main.player[t].wet && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    const float gravity = 0.3f;
                                    const float time = 120f;
                                    Vector2 distance;
                                    if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                                        distance = Main.player[t].Center - npc.Center;
                                    else
                                        distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                                    distance.X = distance.X / time;
                                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                                    npc.ai[1] = 120f;
                                    npc.ai[2] = distance.X;
                                    npc.ai[3] = distance.Y;
                                    npc.netUpdate = true;
                                }
                            }
                            if (npc.ai[1] > 0f) //while jumping
                            {
                                npc.ai[1]--;
                                npc.noTileCollide = true;
                                npc.velocity.X = npc.ai[2];
                                npc.velocity.Y = npc.ai[3];
                                npc.ai[3] += 0.3f;

                                int num22 = 5;
                                for (int index1 = 0; index1 < num22; ++index1)
                                {
                                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                                    Main.dust[index2].noGravity = true;
                                    Main.dust[index2].noLight = true;
                                    Main.dust[index2].velocity /= 4f;
                                    Main.dust[index2].velocity -= npc.velocity;
                                }
                            }
                            else
                            {
                                if (npc.noTileCollide) //compensate for long body
                                    npc.noTileCollide = Collision.SolidCollision(npc.position + Vector2.UnitX * npc.width / 4, npc.width / 2, npc.height);
                            }
                            break;

                        case NPCID.Shark:
                            if (npc.life < npc.lifeMax / 2 && --Counter[1] < 0) //initiate jump
                            {
                                Counter[1] = 360;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    const float gravity = 0.3f;
                                    const float time = 90;
                                    Vector2 distance;
                                    if (Main.player[t].active && !Main.player[t].dead && !Main.player[t].ghost)
                                        distance = Main.player[t].Center - npc.Center;
                                    else
                                        distance = new Vector2(npc.Center.X < Main.player[t].Center.X ? -300 : 300, -100);
                                    distance.X = distance.X / time;
                                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                                    npc.ai[1] = time;
                                    npc.ai[2] = distance.X;
                                    npc.ai[3] = distance.Y;
                                    npc.netUpdate = true;
                                }
                            }
                            if (npc.ai[1] > 0f) //while jumping
                            {
                                npc.ai[1]--;
                                npc.noTileCollide = true;
                                npc.velocity.X = npc.ai[2];
                                npc.velocity.Y = npc.ai[3];
                                npc.ai[3] += 0.3f;

                                int num22 = 5;
                                for (int index1 = 0; index1 < num22; ++index1)
                                {
                                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                                    int index2 = Dust.NewDust(npc.position, npc.width, npc.height, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                                    Main.dust[index2].noGravity = true;
                                    Main.dust[index2].noLight = true;
                                    Main.dust[index2].velocity /= 4f;
                                    Main.dust[index2].velocity -= npc.velocity;
                                }
                            }
                            else
                            {
                                if (npc.noTileCollide) //compensate for long body
                                    npc.noTileCollide = Collision.SolidCollision(npc.position + Vector2.UnitX * npc.width / 4, npc.width / 2, npc.height);
                            }
                            goto case NPCID.SandShark;
                        case NPCID.SandShark:
                        case NPCID.SandsharkCorrupt:
                        case NPCID.SandsharkCrimson:
                        case NPCID.SandsharkHallow:
                            Counter[0]++;
                            if (Counter[0] >= 240)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1 && SharkCount < 5)
                                {
                                    Player player = Main.player[t];
                                    if (player.bleed && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        SharkCount++;
                                        npc.netUpdate = true;
                                        if (Main.netMode == NetmodeID.Server)
                                        {
                                            var netMessage = mod.GetPacket();
                                            netMessage.Write((byte)6);
                                            netMessage.Write((byte)npc.whoAmI);
                                            netMessage.Write(SharkCount);
                                            netMessage.Send();
                                        }
                                    }
                                }
                            }
                            if (SharkCount > 0)
                                npc.damage = (int)(npc.defDamage * (1f + SharkCount / 2f));
                            break;

                        case NPCID.BlackRecluse:
                        case NPCID.BlackRecluseWall:
                            Counter[0]++;
                            if (Counter[0] >= 10)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1)
                                {
                                    Player player = Main.player[t];
                                    int b = player.FindBuffIndex(BuffID.Webbed);
                                    masoBool[0] = (b != -1); //remember if target is webbed until counter activates again
                                    if (masoBool[0])
                                        player.AddBuff(ModContent.BuffType<Defenseless>(), player.buffTime[b]);
                                }
                            }

                            if (masoBool[0])
                            {
                                npc.position += npc.velocity;
                                SharkCount = 1;
                            }
                            else
                            {
                                SharkCount = 0;
                            }
                            break;

                        case NPCID.LunarTowerNebula:
                            if (!masoBool[0])
                            {
                                masoBool[0] = true;
                                masoBool[2] = NPC.LunarApocalypseIsUp;
                                npc.damage += 100;
                                npc.defDamage += 100;
                                npc.netUpdate = true;
                                npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
                                
                                if (Main.netMode == NetmodeID.MultiplayerClient) //request sync
                                {
                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)18);
                                    netMessage.Write((byte)npc.whoAmI);
                                    netMessage.Write(npc.type);
                                    netMessage.Send();
                                }
                            }

                            if (masoBool[2])
                            {
                                if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
                                    NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
                                Aura(npc, 5000, ModContent.BuffType<Atrophied>(), dustid: 58);
                                Aura(npc, 5000, ModContent.BuffType<Jammed>(), dustid: 58);
                                Aura(npc, 5000, ModContent.BuffType<Antisocial>(), dustid: 58);
                            }

                            if (npc.dontTakeDamage)
                            {
                                npc.life = npc.lifeMax;
                            }
                            else
                            {
                                if (++Counter[1] > 180)
                                {
                                    Counter[1] = 0;
                                    npc.TargetClosest(false);
                                    for (int i = 0; i < 40; ++i)
                                    {
                                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 242, 0.0f, 0.0f, 0, new Color(), 1f);
                                        Dust dust = Main.dust[d];
                                        dust.velocity *= 4f;
                                        Main.dust[d].noGravity = true;
                                        Main.dust[d].scale += 1.5f;
                                    }
                                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) < 3000)
                                    {
                                        int x = (int)Main.player[npc.target].Center.X / 16;
                                        int y = (int)Main.player[npc.target].Center.Y / 16;
                                        for (int i = 0; i < 100; i++)
                                        {
                                            int newX = x + Main.rand.Next(10, 31) * (Main.rand.NextBool() ? 1 : -1);
                                            int newY = y + Main.rand.Next(-15, 16);
                                            Vector2 newPos = new Vector2(newX * 16, newY * 16);
                                            if (!Collision.SolidCollision(newPos, npc.width, npc.height))
                                            {
                                                //npc.Center = newPos;
                                                Projectile.NewProjectile(newPos, Vector2.Zero, ModContent.ProjectileType<GlowRingHollow>(), 0, 0f, Main.myPlayer, 10, npc.whoAmI);
                                                break;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < 40; ++i)
                                    {
                                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 242, 0.0f, 0.0f, 0, new Color(), 1f);
                                        Dust dust = Main.dust[d];
                                        dust.velocity *= 4f;
                                        Main.dust[d].noGravity = true;
                                        Main.dust[d].scale += 1.5f;
                                    }
                                    Main.PlaySound(SoundID.Item8, npc.Center);
                                    npc.netUpdate = true;
                                }

                                if (++Counter[2] > 60)
                                {
                                    Counter[2] = 0;
                                    npc.TargetClosest(false);
                                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) < 5000)
                                    {
                                        for (int i = 0; i < 3; i++)
                                        {
                                            Vector2 position = Main.player[npc.target].Center;
                                            position.X += Main.rand.Next(-150, 151);
                                            position.Y -= Main.rand.Next(600, 801);
                                            Vector2 speed = Main.player[npc.target].Center - position;
                                            speed.Normalize();
                                            speed *= 10f;
                                            Projectile.NewProjectile(position, speed, ProjectileID.NebulaLaser, 40, 0f, Main.myPlayer);
                                        }
                                    }
                                }

                                if (++Counter[3] > 60)
                                {
                                    Counter[3] = 0;
                                    npc.TargetClosest(false);
                                    if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 4000)
                                    {
                                        const int heal = 2000;
                                        npc.life += heal;
                                        if (npc.life > npc.lifeMax)
                                            npc.life = npc.lifeMax;
                                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                                    }
                                }

                                masoBool[1] = true;
                            }
                            break;

                        case NPCID.LunarTowerSolar:
                            if (!masoBool[0])
                            {
                                masoBool[0] = true;
                                masoBool[2] = NPC.LunarApocalypseIsUp;
                                npc.damage += 200;
                                npc.defDamage += 200;
                                npc.netUpdate = true;
                                npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;

                                if (Main.netMode == NetmodeID.MultiplayerClient) //request sync
                                {
                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)18);
                                    netMessage.Write((byte)npc.whoAmI);
                                    netMessage.Write(npc.type);
                                    netMessage.Send();
                                }
                            }

                            if (masoBool[2])
                            {
                                if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
                                    NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
                                Aura(npc, 5000, ModContent.BuffType<ReverseManaFlow>(), dustid: DustID.SolarFlare);
                                Aura(npc, 5000, ModContent.BuffType<Jammed>(), dustid: DustID.SolarFlare);
                                Aura(npc, 5000, ModContent.BuffType<Antisocial>(), dustid: DustID.SolarFlare);
                            }

                            if (npc.dontTakeDamage)
                            {
                                npc.life = npc.lifeMax;
                            }
                            else
                            {
                                if (++Counter[2] > 240)
                                {
                                    Counter[2] = 0;
                                    npc.TargetClosest(false);
                                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        const float rotate = (float)Math.PI / 4f;
                                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                        speed.Normalize();
                                        speed *= 5f;
                                        for (int i = -2; i <= 2; i++)
                                            Projectile.NewProjectile(npc.Center, speed.RotatedBy(i * rotate), ProjectileID.CultistBossFireBall, 40, 0f, Main.myPlayer);
                                    }
                                }
                                masoBool[1] = true;

                                if (++Counter[3] > 60)
                                {
                                    Counter[3] = 0;
                                    npc.TargetClosest(false);
                                    if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 4000)
                                    {
                                        const int heal = 2000;
                                        npc.life += heal;
                                        if (npc.life > npc.lifeMax)
                                            npc.life = npc.lifeMax;
                                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                                    }
                                }
                            }
                            break;

                        case NPCID.LunarTowerStardust:
                            if (!masoBool[0])
                            {
                                masoBool[0] = true;
                                masoBool[2] = NPC.LunarApocalypseIsUp;
                                npc.damage += 100;
                                npc.defDamage += 100;
                                npc.netUpdate = true;
                                npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;

                                if (Main.netMode == NetmodeID.MultiplayerClient) //request sync
                                {
                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)18);
                                    netMessage.Write((byte)npc.whoAmI);
                                    netMessage.Write(npc.type);
                                    netMessage.Send();
                                }
                            }

                            if (masoBool[2])
                            {
                                if (NPC.ShieldStrengthTowerStardust > NPC.LunarShieldPowerExpert)
                                    NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
                                Aura(npc, 5000, ModContent.BuffType<Atrophied>(), dustid: 20);
                                Aura(npc, 5000, ModContent.BuffType<Jammed>(), dustid: 20);
                                Aura(npc, 5000, ModContent.BuffType<ReverseManaFlow>(), dustid: 20);
                            }

                            if (npc.dontTakeDamage)
                            {
                                npc.life = npc.lifeMax;
                            }
                            else
                            {
                                if (++Counter[2] > 420)
                                {
                                    Counter[2] = 0;
                                    npc.TargetClosest(false);
                                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        const float rotate = (float)Math.PI / 12f;
                                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                        speed.Normalize();
                                        speed *= 8f;
                                        for (int i = 0; i < 24; i++)
                                        {
                                            Vector2 vel = speed.RotatedBy(rotate * i);
                                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.AncientLight, 0,
                                                0f, (Main.rand.NextFloat() - 0.5f) * 0.3f * 6.28318548202515f / 60f, vel.X, vel.Y);
                                            if (n != Main.maxNPCs)
                                            {
                                                Main.npc[n].velocity = vel;
                                                Main.npc[n].netUpdate = true;
                                                if (Main.netMode == NetmodeID.Server)
                                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                            }
                                        }
                                    }
                                }

                                if (++Counter[3] > 60)
                                {
                                    Counter[3] = 0;
                                    npc.TargetClosest(false);
                                    if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 4000)
                                    {
                                        const int heal = 2000;
                                        npc.life += heal;
                                        if (npc.life > npc.lifeMax)
                                            npc.life = npc.lifeMax;
                                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                                    }
                                }

                                masoBool[1] = true;
                            }
                            break;

                        case NPCID.LunarTowerVortex:
                            if (!masoBool[0])
                            {
                                masoBool[0] = true;
                                masoBool[2] = NPC.LunarApocalypseIsUp;
                                npc.damage += 100;
                                npc.defDamage += 100;
                                npc.netUpdate = true;
                                npc.buffImmune[ModContent.BuffType<ClippedWings>()] = true;

                                if (Main.netMode == NetmodeID.MultiplayerClient) //request sync
                                {
                                    var netMessage = mod.GetPacket();
                                    netMessage.Write((byte)18);
                                    netMessage.Write((byte)npc.whoAmI);
                                    netMessage.Write(npc.type);
                                    netMessage.Send();
                                }
                            }

                            if (masoBool[2])
                            {
                                if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
                                    NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
                                Aura(npc, 5000, ModContent.BuffType<Atrophied>(), dustid: DustID.Vortex);
                                Aura(npc, 5000, ModContent.BuffType<ReverseManaFlow>(), dustid: DustID.Vortex);
                                Aura(npc, 5000, ModContent.BuffType<Antisocial>(), dustid: DustID.Vortex);
                            }

                            if (npc.dontTakeDamage)
                            {
                                npc.life = npc.lifeMax;
                                if (++Counter[1] > 180)
                                {
                                    Counter[1] = 0;
                                    npc.netUpdate = true;
                                }
                            }
                            else
                            {
                                if (++Counter[1] > 360) //triggers "shield going down" animation
                                {
                                    Counter[1] = 0;
                                    npc.ai[3] = 1f;
                                    npc.netUpdate = true;
                                }

                                npc.reflectingProjectiles = npc.ai[3] != 0f;
                                if (npc.reflectingProjectiles) //dust
                                {
                                    for (int i = 0; i < 20; i++)
                                    {
                                        Vector2 offset = new Vector2();
                                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                        offset.X += (float)(Math.Sin(angle) * npc.height / 2);
                                        offset.Y += (float)(Math.Cos(angle) * npc.height / 2);
                                        Dust dust = Main.dust[Dust.NewDust(
                                            npc.Center + offset - new Vector2(4, 4), 0, 0,
                                            DustID.Vortex, 0, 0, 100, Color.White, 1f
                                            )];
                                        dust.noGravity = true;
                                    }
                                }

                                if (++Counter[2] > 240)
                                {
                                    Counter[2] = 0;
                                    npc.TargetClosest(false);
                                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 speed = Main.player[npc.target].Center + Main.player[npc.target].velocity * 15f - npc.Center;
                                        speed.Normalize();
                                        speed *= 4f;
                                        Projectile.NewProjectile(npc.Center, speed, ProjectileID.CultistBossLightningOrb, 30, 0f, Main.myPlayer);
                                    }
                                }

                                if (++Counter[3] > 60)
                                {
                                    Counter[3] = 0;
                                    npc.TargetClosest(false);
                                    if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 4000)
                                    {
                                        const int heal = 2000;
                                        npc.life += heal;
                                        if (npc.life > npc.lifeMax)
                                            npc.life = npc.lifeMax;
                                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, heal);
                                    }
                                }

                                masoBool[1] = true;
                            }
                            break;

                        case NPCID.Splinterling:
                            Counter[0]++;
                            if (Counter[0] >= 60)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, Main.rand.Next(-3, 4), Main.rand.Next(-5, 0),
                                        Main.rand.Next(326, 329), npc.damage / 4, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.FlyingSnake:
                            if (masoBool[0]) //after reviving
                            {
                                if (npc.buffType[0] != 0)
                                    npc.DelBuff(0);
                                npc.position += npc.velocity;
                                npc.knockBackResist = 0f;
                                //npc.damage = npc.defDamage * 3 / 2;
                                SharkCount = 1;
                            }
                            break;

                        case NPCID.Lihzahrd:
                            Counter[0]++;
                            if (Counter[0] >= 200)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1)
                                {
                                    Player player = Main.player[t];
                                    if (player.active && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 velocity = player.Center - npc.Center;
                                        velocity.Normalize();
                                        velocity *= 12f;
                                        Projectile.NewProjectile(npc.Center, velocity, ProjectileID.PoisonDartTrap, 30, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            break;
                        case NPCID.LihzahrdCrawler:
                            if (++Counter[1] > 30)
                            {
                                Counter[1] = -90;
                                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 10f;
                                    Projectile.NewProjectile(npc.Center, vel, ProjectileID.Fireball, npc.damage / 5, 0f, Main.myPlayer);
                                }
                            }
                            goto case NPCID.Lihzahrd;

                        case NPCID.StardustCellSmall:
                            if (npc.ai[0] >= 240f)
                            {
                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                    break;

                                int newType;
                                switch (Main.rand.Next(4))
                                {
                                    case 0: newType = NPCID.StardustJellyfishBig; break;
                                    case 1: newType = NPCID.StardustSpiderBig; break;
                                    case 2: newType = NPCID.StardustWormHead; break;
                                    case 3: newType = NPCID.StardustCellBig; break;
                                    default: newType = NPCID.StardustCellBig; break;
                                }

                                npc.Transform(newType);
                            }
                            break;

                        case NPCID.Pumpking:
                            if (++Counter[0] > 300)
                            {
                                Counter[0] = 0;
                                if (npc.whoAmI == NPC.FindFirstNPC(NPCID.Pumpking) && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int j = -1; j <= 1; j++) //fire these to either side of target
                                    {
                                        if (j == 0)
                                            continue;

                                        for (int i = 0; i < 20; i++)
                                        {
                                            Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(Math.PI / 6 * (Main.rand.NextDouble() - 0.5) + Math.PI / 2 * j);
                                            float ai0 = Main.rand.NextFloat(1.04f, 1.05f);
                                            float ai1 = Main.rand.NextFloat(0.03f);
                                            Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<PumpkingFlamingScythe>(), npc.damage / 2, 0f, Main.myPlayer, ai0, ai1);
                                        }
                                    }
                                }
                            }
                            /*if (++Counter[2] >= 12)
                            {
                                Counter[2] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1)
                                {
                                    Player player = Main.player[t];
                                    Vector2 distance = player.Center - npc.Center;
                                    if (Math.Abs(distance.X) < npc.width && Main.netMode != NetmodeID.MultiplayerClient) //flame rain if player roughly below me
                                        Projectile.NewProjectile(npc.Center.X, npc.position.Y, Main.rand.Next(-3, 4), Main.rand.Next(-4, 0), Main.rand.Next(326, 329), npc.damage / 5, 0f, Main.myPlayer);
                                }
                            }*/
                            break;

                        case NPCID.SolarCorite:
                            Aura(npc, 250, BuffID.Burning, false, DustID.Fire);
                            break;

                        case NPCID.NebulaHeadcrab:
                            Counter[0]++;
                            if (Counter[0] >= 300)
                            {
                                if (npc.ai[0] != 5f && npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) //if not latched on player
                                    Projectile.NewProjectile(npc.Center, 6 * npc.DirectionTo(Main.player[npc.target].Center), ProjectileID.NebulaLaser, npc.damage / 4, 0, Main.myPlayer);
                                Counter[0] = (short)Main.rand.Next(120);
                            }
                            break;

                        case NPCID.IceQueen:
                            /*Counter[0]++;

                            short countCap = 14;
                            if (npc.life < npc.lifeMax * 3 / 4)
                                countCap--;
                            if (npc.life < npc.lifeMax / 2)
                                countCap -= 2;
                            if (npc.life < npc.lifeMax / 4)
                                countCap -= 3;
                            if (npc.life < npc.lifeMax / 10)
                                countCap -= 4;

                            if (Counter[0] > countCap)
                            {
                                Counter[0] = 0;
                                if (++Counter[1] > 25)
                                {
                                    Counter[1] = 0;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Flocko);
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                    }
                                }
                                Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                                speed.Normalize();
                                speed *= 12f;
                                Vector2 spawn = npc.Center;
                                spawn.Y -= 20f;
                                spawn += speed * 4f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(spawn, speed, ProjectileID.FrostShard, 30, 0f, Main.myPlayer);
                            }*/

                            if (npc.ai[0] == 2) //stationary, spinning
                            {
                                if (++Counter[2] > 75)
                                {
                                    Counter[2] = 0;
                                    if (npc.whoAmI == NPC.FindFirstNPC(npc.type) && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < 16; i++)
                                        {
                                            Projectile.NewProjectile(npc.Center, 9f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(Math.PI / 8 * i),
                                                ProjectileID.FrostWave, npc.damage / 5, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (++Counter[3] > 120)
                                {
                                    Counter[3] = 0;

                                    if (npc.whoAmI == NPC.FindFirstNPC(npc.type) && Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 speed = new Vector2(Main.rand.NextFloat(40f), Main.rand.NextFloat(-20f, 20f));
                                        Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<QueenFlocko>(), npc.damage / 5, 0f, Main.myPlayer, npc.whoAmI, -1);
                                        Projectile.NewProjectile(npc.Center, -speed, ModContent.ProjectileType<QueenFlocko>(), npc.damage / 5, 0f, Main.myPlayer, npc.whoAmI, 1);
                                    }
                                }
                            }
                            break;

                        case NPCID.Eyezor:
                            Counter[0]++;
                            if (Counter[0] >= 8)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1)
                                {
                                    Player player = Main.player[t];
                                    if (player.active)
                                    {
                                        Vector2 velocity = player.Center - npc.Center;
                                        velocity.Normalize();
                                        velocity *= 4f;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(npc.Center, velocity, ProjectileID.EyeFire, npc.damage / 5, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            break;

                        case NPCID.VortexHornetQueen:
                            Counter[2]++;
                            if (Counter[2] >= 180)
                            {
                                Counter[2] = Main.rand.Next(30);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LightningVortexHostile>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.SolarCrawltipedeTail:
                            Counter[0]++;
                            if (Counter[0] >= 4)
                            {
                                Counter[0] = 0;
                                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                                if (t != -1)
                                {
                                    Vector2 distance = Main.player[t].Center - npc.Center;
                                    if (distance.Length() < 400f)
                                    {
                                        distance.Normalize();
                                        distance *= 6f;
                                        int p = Projectile.NewProjectile(npc.Center, distance, ProjectileID.FlamesTrap, npc.damage / 4, 0f, Main.myPlayer);
                                        Main.projectile[p].friendly = false;
                                        Main.PlaySound(SoundID.Item34, npc.Center);
                                    }
                                }
                            }
                            break;

                        case NPCID.Nailhead:
                            Counter[0]++;
                            if (Counter[0] >= 90)
                            {
                                Counter[0] = (short)Main.rand.Next(60);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    //npc entire block is fucked
                                    int length = Main.rand.Next(3, 6);
                                    int[] numArray = new int[length];
                                    int maxValue = 0;
                                    for (int index = 0; index < (int)byte.MaxValue; ++index)
                                    {
                                        if (Main.player[index].active && !Main.player[index].dead && Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[index].position, Main.player[index].width, Main.player[index].height))
                                        {
                                            numArray[maxValue] = index;
                                            ++maxValue;
                                            if (maxValue == length)
                                                break;
                                        }
                                    }
                                    if (maxValue > 1)
                                    {
                                        for (int index1 = 0; index1 < 100; ++index1)
                                        {
                                            int index2 = Main.rand.Next(maxValue);
                                            int index3 = index2;
                                            while (index3 == index2)
                                                index3 = Main.rand.Next(maxValue);
                                            int num1 = numArray[index2];
                                            numArray[index2] = numArray[index3];
                                            numArray[index3] = num1;
                                        }
                                    }

                                    Vector2 vector2_1 = new Vector2(-1f, -1f);

                                    for (int index = 0; index < maxValue; ++index)
                                    {
                                        Vector2 vector2_2 = Main.npc[numArray[index]].Center - npc.Center;
                                        vector2_2.Normalize();
                                        vector2_1 += vector2_2;
                                    }

                                    vector2_1.Normalize();

                                    for (int index = 0; index < length; ++index)
                                    {
                                        float num1 = Main.rand.Next(8, 13);
                                        Vector2 vector2_2 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                        vector2_2.Normalize();

                                        if (maxValue > 0)
                                        {
                                            vector2_2 += vector2_1;
                                            vector2_2.Normalize();
                                        }
                                        vector2_2 *= num1;

                                        if (maxValue > 0)
                                        {
                                            --maxValue;
                                            vector2_2 = Main.player[numArray[maxValue]].Center - npc.Center;
                                            vector2_2.Normalize();
                                            vector2_2 *= num1;
                                        }

                                        Projectile.NewProjectile(npc.Center.X, npc.position.Y + npc.width / 4f, vector2_2.X, vector2_2.Y, ProjectileID.Nail, (int)(npc.damage * 0.15), 1f);
                                    }
                                }
                            }
                            break;

                        case NPCID.VortexRifleman: //default: if (npc.localAI[2] >= 360f + Main.rand.Next(360) && etc)
                            if (npc.localAI[2] >= 180f + Main.rand.Next(180) && npc.Distance(Main.player[npc.target].Center) < 400f && Math.Abs(npc.DirectionTo(Main.player[npc.target].Center).Y) < 0.5f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                            {
                                npc.localAI[2] = 0f;
                                Vector2 vector2_1 = npc.Center;
                                vector2_1.X += npc.direction * 30f;
                                vector2_1.Y += 2f;

                                Vector2 vec = npc.DirectionTo(Main.player[npc.target].Center) * 7f;
                                if (vec.HasNaNs())
                                    vec = new Vector2(npc.direction * 8f, 0f);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int Damage = Main.expertMode ? 50 : 75;
                                    for (int index = 0; index < 4; ++index)
                                    {
                                        Vector2 vector2_2 = vec + Utils.RandomVector2(Main.rand, -0.8f, 0.8f);
                                        Projectile.NewProjectile(vector2_1.X, vector2_1.Y, vector2_2.X, vector2_2.Y, ModContent.ProjectileType<StormDiverBullet>(), Damage, 1f, Main.myPlayer);
                                    }
                                }

                                Main.PlaySound(SoundID.Item36, npc.Center);
                            }
                            break;

                        case NPCID.ElfCopter:
                            if (npc.localAI[0] >= 14f)
                            {
                                npc.localAI[0] = 0f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float num8 = Main.player[npc.target].Center.X - npc.Center.X;
                                    float num9 = Main.player[npc.target].Center.Y - npc.Center.Y;
                                    float num10 = num8 + Main.rand.Next(-35, 36);
                                    float num11 = num9 + Main.rand.Next(-35, 36);
                                    float num12 = num10 * (1f + Main.rand.Next(-20, 21) * 0.015f);
                                    float num13 = num11 * (1f + Main.rand.Next(-20, 21) * 0.015f);
                                    float num14 = 10f / (float)Math.Sqrt(num12 * num12 + num13 * num13);
                                    float num15 = num12 * num14;
                                    float num16 = num13 * num14;
                                    float SpeedX = num15 * (1f + Main.rand.Next(-20, 21) * 0.0125f);
                                    float SpeedY = num16 * (1f + Main.rand.Next(-20, 21) * 0.0125f);
                                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, SpeedX, SpeedY, ModContent.ProjectileType<ElfCopterBullet>(), 32, 0f, Main.myPlayer);
                                }
                            }
                            break;

                        case NPCID.TacticalSkeleton: //num3 = 120, damage = 40/50, num8 = 0
                            if (npc.ai[2] > 0f && npc.ai[1] <= 65f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int index = 0; index < 6; ++index)
                                    {
                                        float num6 = Main.player[npc.target].Center.X - npc.Center.X;
                                        float num10 = Main.player[npc.target].Center.Y - npc.Center.Y;
                                        float num11 = 11f / (float)Math.Sqrt(num6 * num6 + num10 * num10);
                                        float num12;
                                        float num18 = num12 = num6 + Main.rand.Next(-40, 41);
                                        float num19;
                                        float num20 = num19 = num10 + Main.rand.Next(-40, 41);
                                        float SpeedX = num18 * num11;
                                        float SpeedY = num20 * num11;
                                        int damage = Main.expertMode ? 40 : 50;
                                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, SpeedX, SpeedY, ModContent.ProjectileType<TacticalSkeletonBullet>(), damage, 0f, Main.myPlayer);
                                    }
                                }
                                Main.PlaySound(SoundID.Item38, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.ai[3] = 0f; //specific to me
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.SkeletonSniper: //num3 = 200, num8 = 0
                            if (npc.ai[2] > 0f && npc.ai[1] <= 105f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.X += Main.rand.Next(-40, 41) * 0.2f;
                                    speed.Y += Main.rand.Next(-40, 41) * 0.2f;
                                    speed.Normalize();
                                    speed *= 11f;

                                    int damage = Main.expertMode ? 80 : 100;
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<SniperBullet>(), damage, 0f, Main.myPlayer);
                                }
                                Main.PlaySound(SoundID.Item40, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.SkeletonArcher: //damage = 28/35, ID.VenomArrow
                            if (npc.ai[2] > 0f && npc.ai[1] <= 40f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.Y -= Math.Abs(speed.X) * 0.075f; //account for gravity (default *0.1f)
                                    speed.X += Main.rand.Next(-24, 25);
                                    speed.Y += Main.rand.Next(-24, 25);
                                    speed.Normalize();
                                    speed *= 11f;

                                    int damage = Main.expertMode ? 28 : 35;
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<SkeletonArcherArrow>(), damage, 0f, Main.myPlayer);
                                }
                                Main.PlaySound(SoundID.Item5, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.SkeletonCommando: //num3 = 90, num5 = 4f, damage = 48/60, ID.RocketSkeleton
                            if (npc.ai[2] > 0f && npc.ai[1] <= 50f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();

                                    int damage = Main.expertMode ? 48 : 60;
                                    Projectile.NewProjectile(npc.Center, 4f * speed, ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(10f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                                    Projectile.NewProjectile(npc.Center, 3f * speed.RotatedBy(MathHelper.ToRadians(-10f)), ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
                                }
                                Main.PlaySound(SoundID.Item11, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.ElfArcher: //num3 = 110, damage = 36/45, tsunami
                            if (npc.ai[2] > 0f && npc.ai[1] <= 60f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.Y -= Math.Abs(speed.X) * 0.1f; //account for gravity
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();
                                    Vector2 spinningpoint = speed;
                                    speed *= 8f;

                                    int damage = Main.expertMode ? 36 : 45;

                                    //tsunami code lol
                                    float num3 = 0.3141593f;
                                    int num4 = 5;
                                    spinningpoint *= 40f;
                                    bool flag4 = Collision.CanHit(npc.Center, 0, 0, npc.Center + spinningpoint, 0, 0);
                                    for (int index1 = 0; index1 < num4; ++index1)
                                    {
                                        float num8 = index1 - (num4 - 1f) / 2f;
                                        Vector2 vector2_5 = spinningpoint.RotatedBy(num3 * num8);
                                        if (!flag4)
                                            vector2_5 -= spinningpoint;
                                        int p = Projectile.NewProjectile(npc.Center + vector2_5, speed, ModContent.ProjectileType<ElfArcherArrow>(), damage, 0f, Main.myPlayer);
                                        Main.projectile[p].noDropItem = true;
                                    }
                                }
                                Main.PlaySound(SoundID.Item5, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.PirateCrossbower: //num3 = 80, num5 = 16f, num8 = Math.Abs(num7) * .08f, damage = 32/40, num12 = 800f?
                            if (npc.ai[2] > 0f && npc.ai[1] <= 45f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();
                                    speed *= 11f;

                                    int damage = Main.expertMode ? 32 : 40;
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PirateCrossbowerArrow>(), damage, 0f, Main.myPlayer);
                                }
                                Main.PlaySound(SoundID.Item5, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.PirateDeadeye: //num3 = 40, num5 = 14f, num8 = 0f, damage = 20/25, num12 = 550f?
                            if (npc.ai[2] > 0f && npc.ai[1] <= 25f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();
                                    speed *= 14f;

                                    int damage = Main.expertMode ? 20 : 25;
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<PirateDeadeyeBullet>(), damage, 0f, Main.myPlayer);
                                }
                                Main.PlaySound(SoundID.Item11, npc.Center);
                                npc.ai[2] = 0f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.PirateCaptain: //60 delay for cannonball, 8 for bullets
                            if (npc.ai[2] > 0f && npc.localAI[2] >= 20f && npc.ai[1] <= 30)
                            {
                                //npc.localAI[2]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.Y -= Math.Abs(speed.X) * 0.2f; //account for gravity
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();
                                    speed *= 11f;
                                    npc.localAI[2] = 0f;
                                    for (int i = 0; i < 15; i++)
                                    {
                                        Vector2 cannonSpeed = speed;
                                        cannonSpeed.X += Main.rand.Next(-10, 11) * 0.3f;
                                        cannonSpeed.Y += Main.rand.Next(-10, 11) * 0.3f;
                                        Projectile.NewProjectile(npc.Center, cannonSpeed, ProjectileID.CannonballHostile, Main.expertMode ? 80 : 100, 0f, Main.myPlayer);
                                    }
                                }
                                //npc.ai[2] = 0f;
                                //npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            break;

                        case NPCID.SolarGoop:
                            Counter[0]++;
                            if (Counter[0] >= 300)
                            {
                                npc.life = 0;
                                npc.checkDead();
                                npc.active = false;
                            }

                            if (npc.HasPlayerTarget)
                            {
                                Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                speed.Normalize();
                                speed *= 12f;

                                npc.velocity.X += speed.X / 100f;

                                if (npc.velocity.Length() > 16f)
                                {
                                    npc.velocity.Normalize();
                                    npc.velocity *= 16f;
                                }
                            }
                            else
                            {
                                npc.TargetClosest(false);
                            }

                            npc.dontTakeDamage = true;
                            break;

                        case NPCID.Pixie:
                            if (npc.HasPlayerTarget)
                            {
                                if (npc.velocity.Y < 0f && npc.position.Y < Main.player[npc.target].position.Y)
                                    npc.velocity.Y = 0f;
                                if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 200)
                                    Counter[0]++;
                            }
                            if (Counter[0] >= 60)
                            {
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Navi").WithVolume(1f).WithPitchVariance(.5f), npc.Center);
                                Counter[0] = 0;
                            }
                            Aura(npc, 100, ModContent.BuffType<Buffs.Masomode.SqueakyToy>());
                            break;

                        case NPCID.Clown:
                            /*if (!masoBool[0]) //roar when spawn
                            {
                                masoBool[0] = true;
                                Main.PlaySound(SoundID.Roar, npc.Center, 0);
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                    Main.NewText("A Clown has begun ticking!", 175, 75, 255);
                                else if (Main.netMode == NetmodeID.Server)
                                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("A Clown has begun ticking!"), new Color(175, 75, 255));
                            }*/

                            Counter[0]++;
                            if (Counter[0] >= 300)
                            {
                                Counter[0] = 0;
                                SharkCount++;
                                if (SharkCount >= 5)
                                {
                                    npc.life = 0;
                                    npc.HitEffect();
                                    Main.PlaySound(npc.DeathSound, npc.Center);
                                    npc.active = false;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        if (FargoSoulsUtil.AnyBossAlive())
                                        {
                                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.BouncyGrenade, 60, 8f, Main.myPlayer);
                                        }
                                        else
                                        {
                                            /*for (int i = 0; i < 30; i++)
                                            {
                                                int p = Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), Main.rand.Next(-500, 501) / 100f, Main.rand.Next(-1000, 101) / 100f, ProjectileID.BouncyGrenade, 200, 8f, Main.myPlayer);
                                                Main.projectile[p].timeLeft -= Main.rand.Next(120);
                                            }*/

                                            for (int i = 0; i < 30; i++)
                                            {
                                                int type = ProjectileID.Grenade;
                                                int damage = 250;
                                                float knockback = 8f;
                                                switch (Main.rand.Next(10))
                                                {
                                                    case 0:
                                                    case 1:
                                                    case 2: type = ProjectileID.HappyBomb; break;
                                                    case 3:
                                                    case 4:
                                                    case 5:
                                                    case 6: type = ProjectileID.BouncyGrenade; break;
                                                    case 7:
                                                    case 8:
                                                    case 9: type = ProjectileID.StickyGrenade; break;
                                                }

                                                int p = Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), Main.rand.Next(-1000, 1001) / 100f, Main.rand.Next(-2000, 101) / 100f, type, damage, knockback, Main.myPlayer);
                                                Main.projectile[p].timeLeft += Main.rand.Next(-120, 120);
                                            }

                                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ClownBomb>(), 100, 8f, Main.myPlayer);
                                        }
                                    }

                                    if (Main.netMode == NetmodeID.SinglePlayer)
                                        Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.ClownExplosion"), 175, 75, 255);
                                    else if (Main.netMode == NetmodeID.Server)
                                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.ClownExplosion")), new Color(175, 75, 255));
                                }
                            }
                            break;

                        case NPCID.Paladin:
                            if (masoBool[0]) //small paladin
                            {
                                if (!masoBool[1] && ++Counter[0] > 15)
                                {
                                    masoBool[1] = true;
                                    if (Main.netMode == NetmodeID.Server) //MP sync
                                    {
                                        var netMessage = mod.GetPacket();
                                        netMessage.Write((byte)3);
                                        netMessage.Write((byte)npc.whoAmI);
                                        netMessage.Write(npc.lifeMax);
                                        netMessage.Write(npc.scale);
                                        netMessage.Send();
                                        npc.netUpdate = true;
                                    }
                                }
                            }
                            Aura(npc, 800f, BuffID.BrokenArmor, false, 246);
                            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != NPCID.Paladin && n.Distance(npc.Center) < 800f))
                            {
                                n.GetGlobalNPC<EModeGlobalNPC>().PaladinsShield = true;
                                if (Main.rand.NextBool())
                                {
                                    int d = Dust.NewDust(n.position, n.width, n.height, 246, 0f, -1.5f, 0, new Color());
                                    Main.dust[d].velocity *= 0.5f;
                                    Main.dust[d].noLight = true;
                                }
                            }
                            break;
                            
                        case NPCID.HoppinJack:
                            Counter[0]++;
                            if (Counter[0] >= 20 && npc.velocity.X != 0)
                            {
                                Counter[0] = 0;
                                Projectile.NewProjectile(npc.Center.X, npc.position.Y, Main.rand.Next(-3, 4), Main.rand.Next(-4, 0), Main.rand.Next(326, 329), npc.damage / 5, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.Antlion:
                            Counter[0]++;
                            if (Counter[0] >= 30)
                            {
                                foreach (Player p in Main.player.Where(x => x.active && !x.dead))
                                {
                                    if (p.HasBuff(ModContent.BuffType<Stunned>()) && npc.Distance(p.Center) < 250)
                                    {
                                        Vector2 velocity = Vector2.Normalize(npc.Center - p.Center) * 5f;
                                        p.velocity += velocity;
                                    }
                                }
                                Counter[0] = 0;
                            }

                            //sand balls
                            if (Counter[1] > 0)
                            {
                                if (Counter[1] == 75)
                                {
                                    Main.PlaySound(SoundID.Item5, npc.position);
                                }

                                Counter[1]--;
                            }

                            if (Counter[1] <= 0)
                            {
                                float num265 = 12f;
                                Vector2 pos = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                float velocityX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - pos.X;
                                float velocityY = Main.player[npc.target].position.Y - pos.Y;
                                float num268 = (float)Math.Sqrt((double)(velocityX * velocityX + velocityY * velocityY));
                                num268 = num265 / num268;
                                velocityX *= num268 * 1.5f;
                                velocityY *= num268 * 1.5f;

                                if (Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.target].Center.Y <= npc.Center.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    int num269 = 10;
                                    int num270 = 31;
                                    int proj = Projectile.NewProjectile(pos.X, pos.Y, velocityX, velocityY, num270, num269, 0f, Main.myPlayer, 0f, 0f);
                                    if (proj != Main.maxProjectiles)
                                    {
                                        Main.projectile[proj].ai[0] = 2f;
                                        Main.projectile[proj].timeLeft = 300;
                                        Main.projectile[proj].friendly = false;
                                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                                    }
                                    npc.netUpdate = true;

                                    Counter[1] = 75;
                                }
                            }

                            //never fire sand balls from vanilla
                            npc.ai[0] = 10;
                            break;

                        case NPCID.AngryNimbus:
                            Counter[0]++;
                            if (Counter[0] >= 360)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(new Vector2(npc.Center.X + 100, npc.Center.Y), Vector2.Zero, ProjectileID.VortexVortexLightning, 0, 1, Main.myPlayer, 0, 1);
                                    Projectile.NewProjectile(new Vector2(npc.Center.X - 100, npc.Center.Y), Vector2.Zero, ProjectileID.VortexVortexLightning, 0, 1, Main.myPlayer, 0, 1);
                                }
                            }
                            break;

                        case NPCID.Unicorn:

                            if (Math.Abs(npc.velocity.X) >= 3f)
                            {
                                //spawn rainbows in mid jump only
                                if (Counter[0]++ >= 3)
                                {
                                    int direction = npc.velocity.X > 0 ? 1 : -1;
                                    int p = Projectile.NewProjectile(new Vector2(npc.Center.X - direction * (npc.width / 2), npc.Center.Y), npc.velocity, ProjectileID.RainbowBack, npc.damage / 3, 1);
                                    if (p < 1000)
                                    {
                                        Main.projectile[p].friendly = false;
                                        Main.projectile[p].hostile = true;
                                        Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().Rainbow = true;
                                    }

                                    Counter[0] = 0;
                                }
                            }

                            //jump initiated
                           /* if (npc.velocity.Y <= -6 && !masoBool[0])
                            {
                                masoBool[0] = true;
                                Counter[1] = 20;
                            }

                            //spawn rainbows in mid jump only
                            if (Counter[1] > 0 && Counter[0]++ >= 3)
                            {
                                if (npc.velocity.Length() > 3)
                                {
                                    int direction = npc.velocity.X > 0 ? 1 : -1;
                                    int p = Projectile.NewProjectile(new Vector2(npc.Center.X - direction * (npc.width / 2), npc.Center.Y), npc.velocity, ProjectileID.RainbowBack, npc.damage / 3, 1);
                                    if (p < 1000)
                                    {
                                        Main.projectile[p].friendly = false;
                                        Main.projectile[p].hostile = true;
                                        Main.projectile[p].GetGlobalProjectile<FargoGlobalProjectile>().Rainbow = true;
                                    }
                                }
                                
                                Counter[0] = 0;
                                Counter[1]--;

                                if (Counter[1] == 0)
                                {
                                    masoBool[0] = false;
                                }
                            }*/
                            break;

                        case NPCID.Reaper:
                            Aura(npc, 40, ModContent.BuffType<MarkedforDeath>(), false, 199);
                            Counter[0]++;
                            if (Counter[0] >= 420)
                            {
                                Counter[0] = 0;
                                npc.TargetClosest();
                                Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 10;
                                npc.velocity = velocity;
                                masoBool[0] = true;
                                Counter[2] = 5;
                            }

                            if (masoBool[0])
                            {
                                Counter[1]++;
                                if (Counter[1] >= 10)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.DeathSickle, (int)(npc.damage / 2), 1f, Main.myPlayer);
                                    Main.projectile[p].hostile = true;
                                    Main.projectile[p].friendly = false;

                                    Counter[1] = 0;
                                    Counter[2]--;

                                    if (Counter[2] <= 0)
                                        masoBool[0] = false;
                                }
                            }
                            break;

                        case NPCID.Werewolf:
                            Aura(npc, 200, ModContent.BuffType<Berserked>(), false, 60);
                            break;

                        case NPCID.BloodZombie:
                            Aura(npc, 300, BuffID.Bleeding, false, 5);
                            break;

                        case NPCID.PossessedArmor:
                            Aura(npc, 400, BuffID.BrokenArmor, false, 37);
                            break;

                        case NPCID.ShadowFlameApparition:
                            Aura(npc, 100, ModContent.BuffType<Shadowflame>(), false, DustID.Shadowflame);
                            break;

                        case NPCID.BlueJellyfish:
                        case NPCID.PinkJellyfish:
                        case NPCID.GreenJellyfish:
                        case NPCID.BloodJelly:
                            if (npc.wet && npc.ai[1] == 1f) //when they be electrocuting
                            {
                                Player p = Main.player[Main.myPlayer];
                                if (npc.Distance(p.Center) < 200 && p.wet && Collision.CanHitLine(p.Center, 2, 2, npc.Center, 2, 2))
                                    p.AddBuff(BuffID.Electrified, 2);

                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 offset = new Vector2();
                                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                    offset.X += (float)(Math.Sin(angle) * 200);
                                    offset.Y += (float)(Math.Cos(angle) * 200);
                                    if (Framing.GetTileSafely(npc.Center + offset - new Vector2(4, 4)).liquid == 0) //dont display outside liquids
                                        continue;
                                    Dust dust = Main.dust[Dust.NewDust(
                                        npc.Center + offset - new Vector2(4, 4), 0, 0,
                                        DustID.Electric, 0, 0, 100, Color.White, 1f
                                        )];
                                    dust.velocity = npc.velocity;
                                    if (Main.rand.NextBool(3))
                                        dust.velocity += Vector2.Normalize(offset) * -5f;
                                    dust.noGravity = true;
                                }
                            }
                            break;

                        case NPCID.Wraith:
                            Aura(npc, 80, BuffID.Obstructed, false, 199);

                            npc.aiStyle = 5;
                            break;

                        case NPCID.MartianSaucerCore:
                            Aura(npc, 200, BuffID.VortexDebuff, false, DustID.Vortex);
                            if (!npc.dontTakeDamage && npc.HasValidTarget)
                            {
                                if ((npc.ai[3] - 60) % 120 == 0)
                                {
                                    Counter[0] = 20;
                                }

                                if (Counter[0] > 0)
                                {
                                    Counter[0]--;
                                    if (++Counter[1] > 2)
                                    {
                                        Counter[1] = 0;
                                        Vector2 speed = 14f * npc.DirectionTo(Main.player[npc.target].Center).RotatedBy((Main.rand.NextDouble() - 0.5) * 0.785398185253143 / 5.0);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                            Projectile.NewProjectile(npc.Center, speed, ProjectileID.SaucerLaser, 15, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            break;

                        case NPCID.ToxicSludge:
                            Aura(npc, 200, BuffID.Poisoned, false, 188);
                            break;

                        case NPCID.GiantTortoise:
                        case NPCID.IceTortoise:
                            npc.reflectingProjectiles = 
                                npc.ai[0] == 3f //spinning
                                && npc.HasValidTarget //while near player or line of sight
                                && (npc.Distance(Main.player[npc.target].Center) < 10 * 16 || Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0));
                            break;

                        case NPCID.SpikeBall:
                        case NPCID.BlazingWheel:
                            if (!masoBool[1])
                            {
                                masoBool[1] = true;
                                if (Framing.GetTileSafely(npc.Center).wall == WallID.LihzahrdBrickUnsafe)
                                {
                                    npc.damage = 150;
                                    npc.defDamage = 150;
                                }
                                int p = npc.FindClosestPlayer();
                                if (p != -1 || !Main.player[p].ZoneDungeon)
                                    masoBool[0] = true;
                            }
                            if (masoBool[0])
                            {
                                if (++Counter[0] > 1800)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<FuseBomb>(), npc.damage / 4, 0f, Main.myPlayer);
                                    npc.life = 0;
                                    npc.HitEffect();
                                    npc.StrikeNPCNoInteraction(999999, 0f, 0);
                                }
                                else if (Counter[0] > 1800 - 300)
                                {
                                    int dust = Dust.NewDust(npc.Center, 0, 0, DustID.Fire, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default(Color), 2f);
                                    Main.dust[dust].velocity *= 2f;
                                    if (Main.rand.NextBool(4))
                                    {
                                        Main.dust[dust].scale += 0.5f;
                                        Main.dust[dust].noGravity = true;
                                    }
                                }
                            }
                            break;

                        case NPCID.GraniteGolem:
                            if (npc.ai[2] < 0f) //while shielding, reflect
                                CustomReflect(npc, DustID.Granite, 2);
                            break;
                            
                        case NPCID.DD2GoblinT3:
                        case NPCID.DD2GoblinBomberT3:
                        case NPCID.DD2JavelinstT3:
                        case NPCID.DD2DrakinT3:
                            if (FargoSoulsUtil.BossIsAlive(ref betsyBoss, NPCID.DD2Betsy))
                                npc.active = false;
                            break;

                        case NPCID.DungeonGuardian:
                            guardBoss = npc.whoAmI;
                            npc.damage = npc.defDamage;
                            npc.defense = npc.defDefense;
                            while (npc.buffType[0] != 0)
                            {
                                npc.buffImmune[npc.buffType[0]] = true;
                                npc.DelBuff(0);
                            }
                            /*if (npc.velocity.Length() < 5f) //old spam bones and skulls code
                            {
                                npc.velocity.Normalize();
                                npc.velocity *= 5f;
                            }
                            if (--Counter < 0)
                            {
                                Counter = 60;
                                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.X += Main.rand.Next(-20, 21);
                                    speed.Y += Main.rand.Next(-20, 21);
                                    speed.Normalize();
                                    speed *= 3f;
                                    speed += npc.velocity * 2f;
                                    Projectile.NewProjectile(npc.Center, speed, ProjectileID.Skull, npc.damage / 4, 0, Main.myPlayer, -1f, 0f);
                                }
                            }
                            if (++Counter2 > 6)
                            {
                                Counter2 = 0;
                                Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                speed.Normalize();
                                speed *= 6f;
                                speed += npc.velocity * 1.25f;
                                speed.Y -= Math.Abs(speed.X) * 0.2f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<SkeletronBone>(), npc.damage / 4, 0f, Main.myPlayer);
                            }*/

                            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && npc.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
                            {
                                Main.LocalPlayer.immune = false;
                                Main.LocalPlayer.immuneTime = 0;
                                Main.LocalPlayer.hurtCooldowns[0] = 0;
                                Main.LocalPlayer.hurtCooldowns[1] = 0;
                            }

                            if (npc.HasValidTarget && npc.ai[1] == 2f) //while actually attacking
                            {
                                npc.position -= npc.velocity; //offset regular velocity

                                float speed = 6f; //base speed
                                float compareSpeed = Math.Max(Math.Abs(Main.player[npc.target].velocity.X), Math.Abs(Main.player[npc.target].velocity.Y));
                                compareSpeed *= 1.02f; //always outrun slightly (player can move diagonally)
                                if (speed < compareSpeed)
                                    speed = compareSpeed;

                                npc.position += Vector2.Normalize(npc.velocity) * speed;
                            }

                            if (!masoBool[2]) //teleport closer
                            {
                                masoBool[2] = true;
                                npc.TargetClosest(false);
                                if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) > 800 && npc.Distance(Main.player[npc.target].Center) < 3000)
                                {
                                    for (int i = 0; i < 50; i++)
                                    {
                                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, 0f, 0f, 0, Color.White, 2.5f);
                                        Main.dust[d].noGravity = true;
                                        Main.dust[d].velocity *= 12f;
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        npc.Center = Main.player[npc.target].Center + 800 * Vector2.UnitX.RotatedByRandom(2 * Math.PI);

                                    for (int i = 0; i < 50; i++)
                                    {
                                        int d = Dust.NewDust(npc.position, npc.width, npc.height, 112, 0f, 0f, 0, Color.White, 2.5f);
                                        Main.dust[d].noGravity = true;
                                        Main.dust[d].velocity *= 12f;
                                    }
                                }
                                npc.netUpdate = true;
                            }

                            if (++Counter[0] < 90)
                            {
                                if (!masoBool[0] && npc.HasValidTarget)
                                {
                                    masoBool[0] = true;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(Main.player[npc.target].Center - Vector2.UnitY * 1500, Vector2.UnitY,
                                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                    }
                                }

                                if (++Counter[1] > 1) //spray bone rain above player
                                {
                                    Main.PlaySound(SoundID.Item1, npc.Center);

                                    Counter[1] = 0;

                                    Vector2 spawnPos = Main.player[npc.target].Center;
                                    spawnPos.X += Main.rand.NextFloat(-100, 100);
                                    spawnPos.Y -= Main.rand.NextFloat(700, 800);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(spawnPos, Vector2.UnitY * Main.rand.NextFloat(10f, 20f),
                                            ModContent.ProjectileType<SkeletronBone>(), npc.damage / 20, 0f, Main.myPlayer);
                                    }
                                }
                            }
                            else if (Counter[0] < 220)
                            {
                                masoBool[0] = false;
                                if (!masoBool[1] && Counter[1] > 30)
                                {
                                    masoBool[1] = true;
                                    for (int i = 0; i < 6; i++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center).RotatedBy(Math.PI / 3 * i),
                                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                                        }
                                    }
                                }

                                if (++Counter[1] > 60) //homing skulls
                                {
                                    Counter[1] = 0;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                        speed.X += Main.rand.Next(-20, 21);
                                        speed.Y += Main.rand.Next(-20, 21);
                                        speed.Normalize();
                                        speed *= 3f;
                                        for (int i = 0; i < 6; i++)
                                        {
                                            Projectile.NewProjectile(npc.Center, speed.RotatedBy(Math.PI / 3 * i),
                                                ProjectileID.Skull, npc.damage / 20, 0, Main.myPlayer, -1f, 0f);
                                        }
                                    }
                                }
                            }
                            else if (Counter[0] < 280)
                            {
                                //nothing
                            }
                            else if (Counter[0] < 410)
                            {
                                masoBool[1] = false;
                                if (!masoBool[0] && Counter[1] > 90)
                                {
                                    masoBool[0] = true;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(Main.player[npc.target].Center, Vector2.UnitX.RotatedBy(Math.PI / 2 * i),
                                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                            Projectile.NewProjectile(Main.player[npc.target].Center + 160 * Vector2.UnitY.RotatedBy(Math.PI / 2 * i), Vector2.UnitX.RotatedBy(Math.PI / 2 * i),
                                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                            Projectile.NewProjectile(Main.player[npc.target].Center + -160 * Vector2.UnitY.RotatedBy(Math.PI / 2 * i), Vector2.UnitX.RotatedBy(Math.PI / 2 * i),
                                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                        }
                                    }
                                }

                                if (++Counter[1] > 120) //wall of babies from all sides
                                {
                                    Counter[1] = 0;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            for (int j = -2; j <= 2; j++)
                                            {
                                                Vector2 spawnPos = new Vector2(1200, 80 * j);
                                                Vector2 vel = -18 * Vector2.UnitX;
                                                spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * i);
                                                vel = vel.RotatedBy(Math.PI / 2 * i);
                                                Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<Projectiles.Champions.ShadowGuardian>(),
                                                    npc.damage / 20, 0f, Main.myPlayer);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Counter[0] < 540)
                            {
                                masoBool[0] = false;
                                if (!masoBool[1] && Counter[1] > 90)
                                {
                                    masoBool[1] = true;
                                    for (int i = 0; i < 16; i++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(Main.player[npc.target].Center, Vector2.UnitX.RotatedBy(Math.PI / 8 * i),
                                                ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                        }
                                    }
                                }

                                if (++Counter[1] > 120) // ring of guardians
                                {
                                    Counter[1] = 0;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        const int max = 16;
                                        Vector2 baseOffset = npc.DirectionTo(Main.player[npc.target].Center);
                                        for (int i = 0; i < max; i++)
                                        {
                                            Projectile.NewProjectile(Main.player[npc.target].Center + 1000 * baseOffset.RotatedBy(2 * Math.PI / max * i),
                                                -10f * baseOffset.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<Projectiles.DeviBoss.DeviGuardian>(),
                                                npc.damage / 20, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                            else if (Counter[0] < 700) //mindless bone spray
                            {
                                masoBool[1] = false;
                                if (!masoBool[0])
                                {
                                    masoBool[0] = true;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(npc.Center + new Vector2(0, -1500), Vector2.UnitY,
                                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                                        Projectile.NewProjectile(npc.Center + new Vector2(-200, -1500), Vector2.UnitY,
                                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                                        Projectile.NewProjectile(npc.Center + new Vector2(200, -1500), Vector2.UnitY,
                                            ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, -1f, npc.whoAmI);
                                    }
                                }

                                if (++Counter[1] > 2)
                                {
                                    Main.PlaySound(SoundID.Item1, npc.Center);

                                    Counter[1] = 0;
                                    Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                    speed.Normalize();
                                    speed *= 6f;
                                    speed += npc.velocity * 1.25f;
                                    speed.Y -= Math.Abs(speed.X) * 0.2f;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<SkeletronBone>(), npc.damage / 20, 0f, Main.myPlayer);
                                }
                            }
                            else if (Counter[0] < 820) //fuck everywhere except where you're standing
                            {
                                masoBool[0] = false;
                                if (!masoBool[1] && Counter[1] > 40)
                                {
                                    masoBool[1] = true;
                                    for (int i = 0; i < 4; i++) //from the cardinals
                                    {
                                        Vector2 spawnPos = Main.player[npc.target].Center + 1000 * Vector2.UnitX.RotatedBy(Math.PI / 2 * i);
                                        for (int j = -1; j <= 1; j++) //to both sides
                                        {
                                            if (j == 0)
                                                continue;

                                            Vector2 baseVel = Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(15) * j);
                                            for (int k = 0; k < 7; k++) //a fan of skulls
                                            {
                                                if (k % 2 == 1) //only draw every other ray
                                                    continue;

                                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                                {
                                                    Projectile.NewProjectile(spawnPos, baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
                                                        ModContent.ProjectileType<GuardianDeathraySmall>(), 0, 0f, Main.myPlayer, npc.target, -1f);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (++Counter[1] > 70)
                                {
                                    Counter[1] = 0;
                                    for (int i = 0; i < 4; i++) //from the cardinals
                                    {
                                        Vector2 spawnPos = Main.player[npc.target].Center + 1000 * Vector2.UnitX.RotatedBy(Math.PI / 2 * i);
                                        for (int j = -1; j <= 1; j++) //to both sides
                                        {
                                            if (j == 0)
                                                continue;

                                            Vector2 baseVel = 22f * Main.player[npc.target].DirectionFrom(spawnPos).RotatedBy(MathHelper.ToRadians(15) * j);
                                            for (int k = 0; k < 7; k++) //a fan of skulls
                                            {
                                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                                {
                                                    Projectile.NewProjectile(spawnPos, baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
                                                        ModContent.ProjectileType<Projectiles.Champions.ShadowGuardian>(),
                                                        npc.damage / 20, 0f, Main.myPlayer);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                masoBool[0] = false;
                                masoBool[1] = false;
                                masoBool[2] = false;
                                Counter[0] = 0;
                            }
                            break;

                        case NPCID.MisterStabby:
                            if (masoBool[0])
                                npc.position.X += npc.velocity.X / 2;
                            break;

                        case NPCID.SnowmanGangsta:
                            if (++Counter[0] > 300)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                                {
                                    for (int index = 0; index < 6; ++index)
                                    {
                                        Vector2 Speed = Main.player[npc.target].Center - npc.Center;
                                        Speed.X += Main.rand.Next(-40, 41);
                                        Speed.Y += Main.rand.Next(-40, 41);
                                        Speed.Normalize();
                                        Speed *= 11f;
                                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, Speed.X, Speed.Y, ProjectileID.BulletSnowman, 20, 0f, Main.myPlayer);
                                    }
                                }
                                Main.PlaySound(SoundID.Item38, npc.Center);
                            }
                            break;

                        case NPCID.SnowBalla:
                            if (npc.ai[2] == 8f)
                            {
                                npc.velocity.X = 0f;
                                npc.velocity.Y = 0f;
                                float num3 = 10f;
                                Vector2 vector2 = new Vector2(npc.position.X + npc.width * 0.5f - npc.direction * 12, npc.position.Y + npc.height * 0.25f);
                                float num4 = Main.player[npc.target].position.X + Main.player[npc.target].width / 2f - vector2.X;
                                float num5 = Main.player[npc.target].position.Y - vector2.Y;
                                float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
                                float num7 = num3 / num6;
                                float SpeedX = num4 * num7;
                                float SpeedY = num5 * num7;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int Damage = 35;
                                    int Type = 109;
                                    int p = Projectile.NewProjectile(vector2.X, vector2.Y, SpeedX, SpeedY, Type, Damage, 0f, Main.myPlayer);
                                    Main.projectile[p].ai[0] = 2f;
                                    Main.projectile[p].timeLeft = 300;
                                    Main.projectile[p].friendly = false;
                                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                                    npc.netUpdate = true;
                                }
                            }
                            break;

                        case NPCID.PirateShip:
                            if (npc.HasPlayerTarget)
                            {
                                if (npc.velocity.Y < 0f && npc.position.Y + npc.height < Main.player[npc.target].position.Y)
                                    npc.velocity.Y = 0f;
                            }
                            break;

                        case NPCID.PirateShipCannon:
                            if (!masoBool[0] && Counter[0] == 360 - 90 && NPC.FindFirstNPC(npc.type) == npc.whoAmI)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI);
                            }

                            if (++Counter[0] > 360)
                            {
                                masoBool[0] = !masoBool[0];
                                Counter[0] = masoBool[0] ? 180 : 0;

                                NetUpdateMaso(npc.whoAmI);
                            }

                            if (masoBool[0] && ++Counter[1] > 10)
                            {
                                Counter[1] = -Main.rand.Next(5);
                                if (npc.HasPlayerTarget)
                                {
                                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                                    speed.X += Main.rand.Next(-40, 41);
                                    speed.Y += Main.rand.Next(-40, 41);
                                    speed.Normalize();
                                    speed *= 14f;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.Center, speed, ProjectileID.BulletDeadeye /*ModContent.ProjectileType<PirateDeadeyeBullet>()*/, 15, 0f, Main.myPlayer);

                                    Main.PlaySound(SoundID.Item11, npc.Center);
                                }
                            }
                            break;

                        case NPCID.IceGolem:
                        case NPCID.Yeti:
                            if (++Counter[0] > 60)
                            {
                                Counter[0] = 0;
                                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, new Vector2(6f, 0f).RotatedByRandom(2 * Math.PI),
                                        ModContent.ProjectileType<FrostfireballHostile>(), npc.damage / 5, 0f, Main.myPlayer, npc.target, 30f);
                                }
                            }
                            break;

                        case NPCID.Hornet:
                        case NPCID.HornetFatty:
                        case NPCID.HornetHoney:
                        case NPCID.HornetLeafy:
                        case NPCID.HornetSpikey:
                        case NPCID.HornetStingy:
                        case NPCID.MossHornet:
                            if (npc.HasPlayerTarget)
                            {
                                bool shouldNotTileCollide = npc.HasValidTarget
                                    && Main.player[npc.target].GetModPlayer<FargoPlayer>().Swarming
                                    && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0);
                                if (shouldNotTileCollide)
                                    npc.noTileCollide = true;
                                else if (npc.noTileCollide && !Collision.SolidCollision(npc.position, npc.width, npc.height)) //still intangible, but should stop, and isnt on tiles
                                    npc.noTileCollide = false;

                                if (npc.noTileCollide || (npc.HasValidTarget && Main.player[npc.target].GetModPlayer<FargoPlayer>().Swarming))
                                {
                                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 44, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f);
                                    Main.dust[d].noGravity = true;
                                }
                            }
                            break;

                        case NPCID.VileSpit:
                            /*if (--Counter2 < 0)
                            {
                                Counter2 = 12;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int p = Projectile.NewProjectile(npc.Center, npc.velocity, ProjectileID.CorruptSpray, 0, 0f, Main.myPlayer, 8f);
                                    if (p != 1000)
                                        Main.projectile[p].timeLeft = 12;
                                }
                            }*/
                            if (++Counter[0] > 600)
                                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                            break;

                        case NPCID.AnglerFish:
                            if (!masoBool[0]) //make light while invisible
                                Lighting.AddLight(npc.Center, 0.1f, 0.5f, 0.5f);
                            break;

                        case NPCID.Psycho: //alpha is controlled by vanilla ai so npc is necessary
                            if (Counter[0] < 200)
                                Counter[0] += 2;
                            if (npc.alpha < Counter[0])
                                npc.alpha = Counter[0];
                            break;

                        case NPCID.DD2SkeletonT1:
                        case NPCID.DD2SkeletonT3:
                            if (++Counter[0] > 420)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.ChaosBall);
                                    if (n != 200 && Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                            break;

                        case NPCID.DD2WitherBeastT2:
                        case NPCID.DD2WitherBeastT3:
                            Aura(npc, 300, BuffID.WitheredArmor, false, 119);
                            Aura(npc, 300, BuffID.WitheredWeapon, false, 14);
                            if (FargoSoulsUtil.BossIsAlive(ref betsyBoss, NPCID.DD2Betsy))
                                npc.active = false;
                            break;

                        case NPCID.DD2DarkMageT1:
                            Aura(npc, 600, ModContent.BuffType<Lethargic>(), false, 254);
                            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != npc.type && n.Distance(npc.Center) < 600))
                            {
                                n.GetGlobalNPC<EModeGlobalNPC>().PaladinsShield = true;
                                if (Main.rand.NextBool())
                                {
                                    int d = Dust.NewDust(n.position, n.width, n.height, 254, 0f, -3f, 0, new Color(), 1.5f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].noLight = true;
                                }
                            }
                            break;
                        case NPCID.DD2DarkMageT3:
                            Aura(npc, 900, ModContent.BuffType<Lethargic>(), false, 254);
                            foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.type != npc.type && n.Distance(npc.Center) < 900))
                            {
                                n.GetGlobalNPC<EModeGlobalNPC>().PaladinsShield = true;
                                if (Main.rand.NextBool())
                                {
                                    int d = Dust.NewDust(n.position, n.width, n.height, 254, 0f, -3f, 0, new Color(), 1.5f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].noLight = true;
                                }
                            }
                            break;

                        case NPCID.DD2WyvernT1:
                        case NPCID.DD2WyvernT2:
                        case NPCID.DD2WyvernT3:
                            if (++Counter[0] >= 180)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Shoot(npc, 0, 300, 6, ProjectileID.Fireball, npc.damage / 4, 0f);
                            }
                            break;

                        case NPCID.DD2LightningBugT3:
                            Aura(npc, 400, ModContent.BuffType<LightningRod>(), false, DustID.Vortex);
                            if (++Counter[0] > 240)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.VortexVortexLightning, 0, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.DD2KoboldFlyerT2:
                        case NPCID.DD2KoboldFlyerT3:
                            if (++Counter[0] > 60)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5f),
                                        ModContent.ProjectileType<GoblinSpikyBall>(), npc.damage / 5, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.LavaSlime:
                            if (npc.velocity.Y < 0f)
                            {
                                masoBool[0] = true;
                            }
                            else if (npc.velocity.Y > 0f) //coming down
                            {
                                //when below target, in hell, with line of sight
                                if (masoBool[0] && Main.netMode != NetmodeID.MultiplayerClient && npc.HasValidTarget && npc.Bottom.Y > Main.player[npc.target].Bottom.Y
                                    && npc.Center.ToTileCoordinates().Y > Main.maxTilesY - 200 && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                                {
                                    masoBool[0] = false;
                                    //Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT1Explosion, 0, 0, Main.myPlayer);

                                    int tileX = (int)(npc.Center.X + npc.velocity.X) / 16;
                                    int tileY = (int)(npc.Center.Y + npc.velocity.Y) / 16;
                                    Tile tile = Framing.GetTileSafely(tileX, tileY);
                                    if (tile != null && !tile.active() && tile.liquid == 0)
                                    {
                                        tile.liquidType(1);
                                        tile.liquid = 255;
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendTileSquare(-1, tileX, tileY, 1);
                                        WorldGen.SquareTileFrame(tileX, tileY, true);
                                        npc.velocity.Y = 0;
                                        npc.netUpdate = true;
                                    }
                                }
                            }
                            else //npc vel y = 0
                            {
                                masoBool[0] = false;
                            }
                            break;

                        case NPCID.DD2OgreT2:
                        case NPCID.DD2OgreT3:
                            Aura(npc, 500, BuffID.Stinky, false, 188);
                            break;

                        case NPCID.Nymph:
                            npc.knockBackResist = 0f;
                            Aura(npc, 250, ModContent.BuffType<Lovestruck>(), true, DustID.PinkFlame);
                            if (--Counter[0] < 0)
                            {
                                Counter[0] = 300;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 1000)
                                {
                                    Vector2 spawnVel = npc.DirectionFrom(Main.player[npc.target].Center) * 10f;
                                    for (int i = -3; i < 3; i++)
                                        Projectile.NewProjectile(npc.Center, spawnVel.RotatedBy(Math.PI / 7 * i), ModContent.ProjectileType<FakeHeart2>(), 20, 0f, Main.myPlayer, 30, 90 + 10 * i);
                                }
                            }
                            break;

                        case NPCID.SandElemental:
                            if (npc.HasValidTarget)
                                Main.player[npc.target].ZoneSandstorm = true;
                            if (++Counter[0] % 60 == 0)
                            {
                                if (NPC.AnyNPCs(NPCID.DuneSplicerHead)) //effectively, timer starts counting up when splicers are dead
                                {
                                    Counter[0] = 0;
                                }
                                else if (Counter[0] >= 360 && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.DuneSplicerHead, npc.whoAmI, 0f, 0f, 0f, 0f, npc.target);
                                    if (n != 200 && Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                            if (++Counter[1] > 360)
                            {
                                Counter[1] = 0;
                                if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 target = Main.player[npc.target].Center;
                                    target.Y -= 150;
                                    Projectile.NewProjectile(target, Vector2.Zero, ProjectileID.SandnadoHostileMark, 0, 0f, Main.myPlayer);

                                    int length = (int)npc.Distance(target) / 10;
                                    Vector2 offset = npc.DirectionTo(target) * 10f;
                                    for (int i = 0; i < length; i++) //dust warning line for sandnado
                                    {
                                        int d = Dust.NewDust(npc.Center + offset * i, 0, 0, 269, 0f, 0f, 0, new Color());
                                        Main.dust[d].noLight = true;
                                        Main.dust[d].scale = 1.25f;
                                    }
                                }
                            }
                            break;

                        case NPCID.FungiSpore:
                            npc.dontTakeDamage = Counter[0] < 60;
                            if (npc.dontTakeDamage)
                                Counter[0]++;
                            break;

                        case NPCID.MothronSpawn:
                            Aura(npc, 300, ModContent.BuffType<Buffs.Masomode.SqueakyToy>());
                            break;

                        /*case NPCID.Poltergeist:
                            if (++Counter > 180)
                            {
                                Counter = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                                {
                                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 4f;
                                    int p = Projectile.NewProjectile(npc.Center, vel, ProjectileID.LostSoulHostile, npc.damage / 4, 0f, Main.myPlayer);
                                    if (p != 1000)
                                        Main.projectile[p].timeLeft = 300;
                                }
                            }
                            break;*/

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
                            if (++Counter[0] > 180) //spray bones
                            {
                                if (++Counter[1] > 6 && npc.HasValidTarget && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                                {
                                    Counter[1] = 0;
                                    Vector2 speed = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                    speed.Normalize();
                                    speed *= 6f;
                                    speed.Y -= Math.Abs(speed.X) * 0.2f;
                                    speed.Y -= 3f;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(npc.Center, speed, ProjectileID.SkeletonBone, npc.damage / 4, 0f, Main.myPlayer);
                                }
                                if (Counter[0] > 300)
                                    Counter[0] = 0;
                            }
                            if (npc.justHit)
                                Counter[2] += 20;
                            if (++Counter[2] > 300) //shoot baby guardians
                            {
                                Counter[2] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasValidTarget && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center), ModContent.ProjectileType<SkeletronGuardian2>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.Butcher:
                            npc.position.X += npc.velocity.X;
                            break;

                        case NPCID.DesertScorpionWall:
                            if (++Counter[0] > 240)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                                {
                                    Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 14;
                                    Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<VenomSpit>(), 9, 0, Main.myPlayer);
                                }
                            }
                            break;

                        case NPCID.Mothron:
                       

                            if (--Counter[0] < 0)
                            {
                                Counter[0] = 20 + (int)(100f * npc.life / npc.lifeMax);
                                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 spawnPos = npc.Center + new Vector2(30f * -npc.direction, 30f);
                                    Vector2 vel = Main.player[npc.target].Center - spawnPos
                                        + new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-40, 41));
                                    vel.Normalize();
                                    vel *= 10f;
                                    Projectile.NewProjectile(spawnPos, vel, ProjectileID.Stinger, npc.defDamage / 8, 0f, Main.myPlayer);
                                }
                            }

                            if (--Counter[1] < 0)
                            {
                                Counter[1] = 60 + (int)(120f * npc.life / npc.lifeMax);
                                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 spawnPos = npc.Center;
                                    spawnPos.X += 45f * npc.direction;
                                    Vector2 vel = Vector2.Normalize(Main.player[npc.target].Center - spawnPos) * 9f;
                                    Projectile.NewProjectile(spawnPos, vel, ProjectileID.EyeLaser, npc.defDamage / 5, 0f, Main.myPlayer);
                                }
                            }

                            npc.defense = npc.defDefense;
                            npc.reflectingProjectiles = npc.ai[0] >= 4f;
                            if (npc.reflectingProjectiles)
                            {
                                npc.defense *= 5;
                                if (npc.buffType[0] != 0)
                                    npc.DelBuff(0);
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 228, npc.velocity.X * .4f, npc.velocity.Y * .4f, 0, Color.White, 3f);
                                Main.dust[d].velocity *= 6f;
                                Main.dust[d].noGravity = true;
                            }
                            break;

                        case NPCID.Moth:
                            npc.position += npc.velocity;
                            for (int i = 0; i < 2; i++)
                            {
                                int d = Dust.NewDust(npc.position, npc.width, npc.height, 70);
                                Main.dust[d].scale += 1f;
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity *= 5f;
                            }
                            if (++Counter[0] > 6)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, Main.rand.NextVector2Unit() * 12f,
                                        ModContent.ProjectileType<MothDust>(), npc.damage / 5, 0f, Main.myPlayer);
                            }
                            break;

                        case NPCID.WyvernHead:
                            if (++Counter[0] > 240)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity != Vector2.Zero)
                                {
                                    const int max = 12;
                                    Vector2 vel = Vector2.Normalize(npc.velocity) * 1.5f;
                                    for (int i = 0; i < max; i++)
                                    {
                                        Projectile.NewProjectile(npc.Center, vel.RotatedBy(2f * Math.PI / max * i),
                                            ModContent.ProjectileType<LightBall>(), npc.damage / 5, 0f, Main.myPlayer, 0f, .01f * npc.direction);
                                    }
                                }
                            }
                            break;

                        case NPCID.HeadlessHorseman:
                            if (++Counter[0] > 360)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                                {
                                    Vector2 vel = (Main.player[npc.target].Center - npc.Center) / 60f;
                                    if (vel.Length() < 12f)
                                        vel = Vector2.Normalize(vel) * 12f;
                                    Projectile.NewProjectile(npc.Center, vel, ModContent.ProjectileType<HorsemansBlade>(),
                                        npc.damage / 5, 0f, Main.myPlayer, npc.target);
                                }
                            }
                            break;

                        case NPCID.SwampThing:
                            /*if (++Counter[0] > 300)
                            {
                                Counter[0] = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                                    Projectile.NewProjectile(npc.Center, npc.DirectionTo(Main.player[npc.target].Center) * 12f,
                                        ProjectileID.DD2OgreSpit, npc.damage / 4, 0, Main.myPlayer);
                            }*/
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
                            if (npc.ai[2] >= 45f && npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int tileX = (int)(npc.position.X + npc.width / 2 + 15 * npc.direction) / 16;
                                int tileY = (int)(npc.position.Y + npc.height - 15) / 16 - 1;
                                Tile tile = Framing.GetTileSafely(tileX, tileY);
                                if (tile.type == TileID.ClosedDoor || tile.type == TileID.TallGateClosed)
                                {
                                    //WorldGen.KillTile(tileX, tileY);
                                    WorldGen.OpenDoor(tileX, tileY, npc.direction);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, tileX, tileY);
                                }
                            }

                            break;

                        default:
                            break;
                    }
                }
            }

            return true;
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (SharkCount != 0)
            {
                drawColor.R = (byte)(SharkCount * 20 + 155);
                drawColor.G /= (byte)(SharkCount + 1);
                drawColor.B /= (byte)(SharkCount + 1);
                return drawColor;
            }

            return null;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.BlueSlime:
                        switch (npc.netID)
                        {
                            case NPCID.BlackSlime:
                                target.AddBuff(BuffID.Slimed, 120);
                                target.AddBuff(BuffID.Darkness, 600);
                                break;

                            case NPCID.Pinky:
                                target.AddBuff(BuffID.Slimed, 120);
                                target.GetModPlayer<FargoPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 120);
                                Vector2 velocity = Vector2.Normalize(target.Center - npc.Center) * 30;
                                target.velocity = velocity;
                                break;

                            default:
                                target.AddBuff(BuffID.Slimed, 120);
                                break;
                        }
                        break;

                    case NPCID.SlimeRibbonGreen:
                    case NPCID.SlimeRibbonRed:
                    case NPCID.SlimeRibbonWhite:
                    case NPCID.SlimeRibbonYellow:
                    case NPCID.SlimeMasked:
                        target.AddBuff(BuffID.Slimed, 120);
                        break;

                    case NPCID.UmbrellaSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(BuffID.Wet, 600);
                        break;

                    case NPCID.IceSlime:
                    case NPCID.SpikedIceSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                        break;

                    case NPCID.JungleSlime:
                    case NPCID.SpikedJungleSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(BuffID.Poisoned, 180);
                        break;

                    case NPCID.MotherSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Antisocial>(), 1200);
                        break;

                    case NPCID.LavaSlime:
                        target.AddBuff(ModContent.BuffType<Oiled>(), 900);
                        target.AddBuff(BuffID.OnFire, 300);
                        break;

                    case NPCID.DungeonSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(BuffID.Blackout, 300);
                        break;

                    case NPCID.ToxicSludge:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Infested>(), 360);
                        break;

                    case NPCID.CorruptSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Rotting>(), 1200);
                        break;

                    case NPCID.Crimslime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 300);
                        break;

                    case NPCID.Gastropod:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Fused>(), 1800);
                        break;

                    case NPCID.IlluminantSlime:
                        target.AddBuff(BuffID.Slimed, 120);
                        target.AddBuff(ModContent.BuffType<Purified>(), 300);
                        break;

                    case NPCID.EaterofWorldsHead:
                    case NPCID.EaterofWorldsBody:
                    case NPCID.EaterofWorldsTail:
                        target.AddBuff(BuffID.CursedInferno, 180);
                        target.AddBuff(ModContent.BuffType<Rotting>(), 600);
                        break;

                    case NPCID.CursedSkull:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Cursed, 30);
                        break;

                    case NPCID.Snatcher:
                    case NPCID.ManEater:
                        target.AddBuff(BuffID.Bleeding, 300);
                        if (target.statLife + damage < 100)
                            target.KillMe(PlayerDeathReason.ByCustomReason(target.name + Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.KilledbyManEater")), 999, 0);
                        break;

                    case NPCID.DevourerHead:
                        target.AddBuff(BuffID.BrokenArmor, 600);
                        break;

                    case NPCID.AngryTrapper:
                        target.AddBuff(BuffID.Bleeding, 300);
                        if (target.statLife + damage < 180)
                            target.KillMe(PlayerDeathReason.ByCustomReason(target.name + Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.KilledbyAngryTrapper")), 999, 0);
                        break;

                    case NPCID.CaveBat:
                        target.AddBuff(BuffID.Bleeding, 300);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.Hellbat:
                        target.AddBuff(BuffID.OnFire, 240);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.JungleBat:
                        target.AddBuff(BuffID.Poisoned, 240);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.IceBat:
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.Lavabat:
                        target.AddBuff(BuffID.Burning, 240);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.GiantBat:
                        target.AddBuff(BuffID.Confused, 240);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.IlluminantBat:
                        target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.GiantFlyingFox:
                        target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 300);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.VampireBat:
                    case NPCID.Vampire:
                        target.AddBuff(BuffID.Weak, 600);
                        target.AddBuff(BuffID.Rabies, 3600);
                        npc.life += damage;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage);
                        npc.damage = (int)(npc.damage * 1.1f);
                        npc.defDamage = (int)(npc.defDamage * 1.1f);
                        npc.netUpdate = true;
                        break;

                    case NPCID.SnowFlinx:
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
                        break;

                    case NPCID.Medusa:
                        //target.AddBuff(ModContent.BuffType<Flipped>(), 60);
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Stoned, 60);
                        break;

                    case NPCID.SpikeBall:
                        target.AddBuff(BuffID.BrokenArmor, 1200);
                        if (masoBool[0])
                            target.AddBuff(ModContent.BuffType<Defenseless>(), 1200);
                        break;

                    case NPCID.BlazingWheel:
                        target.AddBuff(BuffID.OnFire, 300);
                        if (masoBool[0])
                            target.AddBuff(BuffID.Burning, 300);
                        break;

                    case NPCID.Shark:
                    case NPCID.SandShark:
                    case NPCID.SandsharkCorrupt:
                    case NPCID.SandsharkCrimson:
                    case NPCID.SandsharkHallow:
                    case NPCID.Piranha:
                        target.AddBuff(BuffID.Bleeding, 240);
                        break;

                    case NPCID.GraniteFlyer:
                    case NPCID.GraniteGolem:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Stoned, 60);
                        break;

                    case NPCID.AnomuraFungus:
                    case NPCID.FungiSpore:
                    case NPCID.MushiLadybug:
                        target.AddBuff(BuffID.Poisoned, 300);
                        break;

                    case NPCID.WaterSphere:
                        target.AddBuff(BuffID.Wet, 1200);
                        break;

                    case NPCID.GiantShelly:
                    case NPCID.GiantShelly2:
                        target.AddBuff(BuffID.Slow, 120);
                        break;

                    case NPCID.Squid:
                        target.AddBuff(BuffID.Obstructed, 120);
                        break;

                    case NPCID.BloodZombie:
                        target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 240);
                        break;

                    case NPCID.Drippler:
                        target.AddBuff(ModContent.BuffType<Rotting>(), 600);
                        break;

                    case NPCID.ChaosBall:
                    case NPCID.ShadowFlameApparition:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        break;

                    case NPCID.Tumbleweed:
                        target.AddBuff(ModContent.BuffType<Crippled>(), 300);
                        break;

                    case NPCID.PigronCorruption:
                    case NPCID.PigronCrimson:
                    case NPCID.PigronHallow:
                        target.AddBuff(ModContent.BuffType<Buffs.Masomode.SqueakyToy>(), 120);
                        target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 50;
                        target.AddBuff(ModContent.BuffType<OceanicMaul>(), 1800);
                        break;

                    case NPCID.CorruptBunny:
                    case NPCID.CrimsonBunny:
                    case NPCID.CorruptGoldfish:
                    case NPCID.CrimsonGoldfish:
                    case NPCID.CorruptPenguin:
                    case NPCID.CrimsonPenguin:
                    case NPCID.GingerbreadMan:
                        target.AddBuff(ModContent.BuffType<Buffs.Masomode.SqueakyToy>(), 120);
                        break;

                    case NPCID.FaceMonster:
                        target.AddBuff(BuffID.Rabies, 900);
                        break;

                    case NPCID.SeaSnail:
                        target.AddBuff(BuffID.OgreSpit, 300);
                        break;

                    case NPCID.SwampThing:
                        target.AddBuff(BuffID.OgreSpit, 300);
                        break;

                    case NPCID.Frankenstein:
                        target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
                        break;

                    case NPCID.Butcher:
                        target.AddBuff(ModContent.BuffType<Berserked>(), 600);
                        target.AddBuff(BuffID.Bleeding, 600);
                        break;

                    case NPCID.ThePossessed:
                        target.AddBuff(ModContent.BuffType<Hexed>(), 240);
                        break;

                    case NPCID.Wolf:
                        target.AddBuff(ModContent.BuffType<Crippled>(), 300);
                        target.AddBuff(BuffID.Rabies, 900);
                        break;

                    case NPCID.Werewolf:
                        target.AddBuff(BuffID.Rabies, 1800);
                        break;

                    //all armored bones
                    case 269:
                    case 270:
                    case 271:
                    case 272:
                    case 273:
                    case 274:
                    case 275:
                    case 276:
                    case 277:
                    case 278:
                    case 279:
                    case 280:
                        target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 180);
                        break;

                    case NPCID.GiantTortoise:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        break;

                    case NPCID.IceTortoise:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        if (Main.rand.NextBool(3))
                            target.AddBuff(BuffID.Frozen, 60);
                        break;

                    case NPCID.Bee:
                    case NPCID.BeeSmall:
                    case NPCID.MossHornet:
                    case NPCID.Hornet:
                    case NPCID.HornetFatty:
                    case NPCID.HornetHoney:
                    case NPCID.HornetLeafy:
                    case NPCID.HornetSpikey:
                    case NPCID.HornetStingy:
                        target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        target.AddBuff(ModContent.BuffType<Swarming>(), 600);
                        break;

                    case NPCID.Paladin:
                        target.AddBuff(ModContent.BuffType<Lethargic>(), 600);
                        break;

                    case NPCID.Hellhound:
                        target.AddBuff(BuffID.Rabies, 3600);
                        //target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
                        break;

                    case NPCID.Reaper:
                        target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 600);
                        break;

                    case NPCID.ChaosElemental:
                        target.AddBuff(ModContent.BuffType<Unstable>(), 240);
                        break;

                    case NPCID.PirateCaptain:
                    case NPCID.PirateCorsair:
                    case NPCID.PirateCrossbower:
                    case NPCID.PirateDeadeye:
                    case NPCID.PirateShipCannon:
                    case NPCID.PirateDeckhand:
                        if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoPlayer>().SecurityWallet && Main.rand.NextBool())
                        {
                            //try stealing mouse item, then selected item
                            bool stolen = StealFromInventory(target, ref Main.mouseItem);
                            if (!stolen)
                                stolen = StealFromInventory(target, ref target.inventory[target.selectedItem]);
                            if (stolen)
                            {
                                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), new Color(255, 50, 50));
                                CombatText.NewText(target.Hitbox, new Color(255, 50, 50), Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), true);
                            }
                        }

                        target.AddBuff(ModContent.BuffType<Midas>(), 600);
                        //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        break;

                    case NPCID.Parrot:
                        target.AddBuff(ModContent.BuffType<Buffs.Masomode.SqueakyToy>(), 120);
                        target.AddBuff(ModContent.BuffType<Midas>(), 600);
                        //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        break;

                    case NPCID.ScutlixRider:
                    case NPCID.GigaZapper:
                    case NPCID.MartianEngineer:
                    case NPCID.MartianOfficer:
                    case NPCID.RayGunner:
                    case NPCID.GrayGrunt:
                    case NPCID.BrainScrambler:
                    case NPCID.MartianDrone:
                    case NPCID.MartianWalker:
                    case NPCID.MartianTurret:
                        target.AddBuff(BuffID.Electrified, 300);
                        //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        break;

                    case NPCID.Scutlix:
                        target.AddBuff(ModContent.BuffType<Buffs.Masomode.SqueakyToy>(), 180);
                        //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        break;

                    case NPCID.Zombie:
                    case NPCID.ArmedZombie:
                    case NPCID.ArmedZombieCenx:
                    case NPCID.ArmedZombiePincussion:
                    case NPCID.ArmedZombieSlimed:
                    case NPCID.ArmedZombieSwamp:
                    case NPCID.ArmedZombieTwiggy:
                    case NPCID.BaldZombie:
                    case NPCID.FemaleZombie:
                    case NPCID.PincushionZombie:
                    case NPCID.SlimedZombie:
                    case NPCID.TwiggyZombie:
                    case NPCID.ZombiePixie:
                    case NPCID.ZombieRaincoat:
                    case NPCID.ZombieSuperman:
                    case NPCID.ZombieSweater:
                    case NPCID.ZombieXmas:
                    case NPCID.SwampZombie:
                    case NPCID.SmallSwampZombie:
                    case NPCID.BigSwampZombie:
                    case NPCID.ZombieDoctor:
                        target.AddBuff(ModContent.BuffType<Rotting>(), 300);
                        break;

                    case NPCID.ZombieMushroom:
                    case NPCID.ZombieMushroomHat:
                        target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        goto case NPCID.Zombie;

                    case NPCID.ZombieEskimo:
                    case NPCID.ArmedZombieEskimo:
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                        goto case NPCID.Zombie;

                    case NPCID.Corruptor:
                        target.AddBuff(BuffID.Weak, 600);
                        target.AddBuff(ModContent.BuffType<Rotting>(), 900);
                        break;

                    case NPCID.Mummy:
                    case NPCID.LightMummy:
                    case NPCID.DarkMummy:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Webbed, 60);
                        break;

                    case NPCID.Derpling:
                        target.AddBuff(ModContent.BuffType<Lethargic>(), 900);
                        break;

                    case NPCID.DesertBeast:
                        //target.AddBuff(ModContent.BuffType<Infested>(), 600);
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Stoned, 60);
                        break;

                    case NPCID.FlyingSnake:
                        //target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 600);
                        break;

                    case NPCID.Lihzahrd:
                    case NPCID.LihzahrdCrawler:
                        target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        //target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 120);
                        break;

                    case NPCID.BoneLee:
                        target.AddBuff(BuffID.Obstructed, 60);
                        target.velocity.X = npc.velocity.Length() * npc.direction;
                        break;

                    case NPCID.MourningWood:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 180);
                        break;

                    case NPCID.Poltergeist:
                        target.AddBuff(BuffID.Silenced, 180);
                        break;

                    case NPCID.Pumpking:
                    case NPCID.PumpkingBlade:
                        target.AddBuff(ModContent.BuffType<Rotting>(), 900);
                        target.AddBuff(ModContent.BuffType<LivingWasteland>(), 900);
                        break;

                    case NPCID.Flocko:
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                        target.AddBuff(BuffID.Frostburn, 180);
                        break;

                    case NPCID.IceQueen:
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
                        target.AddBuff(BuffID.Frostburn, 180);
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Frozen, 30);
                        break;

                    case NPCID.VortexLarva:
                    case NPCID.VortexHornet:
                    case NPCID.VortexHornetQueen:
                    case NPCID.VortexSoldier:
                    case NPCID.VortexRifleman:
                        target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
                        break;

                    case NPCID.ZombieElf:
                    case NPCID.ZombieElfBeard:
                    case NPCID.ZombieElfGirl:
                        target.AddBuff(ModContent.BuffType<Rotting>(), 900);
                        break;

                    case NPCID.Krampus:
                        if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoPlayer>().SecurityWallet)
                        {
                            //try stealing mouse item, then selected item
                            bool stolen = StealFromInventory(target, ref Main.mouseItem);
                            if (!stolen)
                                stolen = StealFromInventory(target, ref target.inventory[target.selectedItem]);
                            
                            for (int i = 0; i < 15; i++)
                            {
                                int toss = Main.rand.Next(3, 8 + target.extraAccessorySlots); //pick random accessory slot
                                if (Main.rand.NextBool(3) && target.armor[toss + 10].stack > 0) //chance to pick vanity slot if accessory is there
                                    toss += 10;
                                if (StealFromInventory(target, ref target.armor[toss]))
                                {
                                    stolen = true;
                                    break;
                                }
                            }

                            if (stolen)
                            {
                                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), new Color(255, 50, 50));
                                CombatText.NewText(target.Hitbox, new Color(255, 50, 50), Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), true);
                            }
                        }
                        break;

                    case NPCID.Wraith:
                        target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoPlayer>().SecurityWallet)
                        {
                            bool IsSoul(int type)
                            {
                                return type == ItemID.SoulofFlight || type == ItemID.SoulofFright || type == ItemID.SoulofLight || type == ItemID.SoulofMight || type == ItemID.SoulofNight || type == ItemID.SoulofSight;
                            };

                            bool stolen = false;
                            if (IsSoul(Main.mouseItem.type) && StealFromInventory(target, ref Main.mouseItem))
                            {
                                stolen = true;
                            }
                            else
                            {
                                for (int j = 0; j < target.inventory.Length; j++)
                                {
                                    Item item = target.inventory[j];

                                    if (IsSoul(item.type))
                                    {
                                        if (StealFromInventory(target, ref target.inventory[j]))
                                            stolen = true;
                                        break;
                                    }
                                }
                            }

                            if (stolen)
                            {
                                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), new Color(255, 50, 50));
                                CombatText.NewText(target.Hitbox, new Color(255, 50, 50), Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), true);
                            }
                        }
                        break;

                    case NPCID.Clown:
                        target.AddBuff(ModContent.BuffType<Fused>(), 1800);
                        break;

                    case NPCID.UndeadMiner:
                        target.AddBuff(ModContent.BuffType<Lethargic>(), 600);
                        target.AddBuff(BuffID.Blackout, 300);
                        target.AddBuff(BuffID.NoBuilding, 300);
                        if (target.whoAmI == Main.myPlayer && !target.GetModPlayer<FargoPlayer>().SecurityWallet)
                        {
                            bool stolen = false;
                            for (int i = 0; i < 59; i++)
                            {
                                if (target.inventory[i].pick != 0 || target.inventory[i].hammer != 0 || target.inventory[i].axe != 0)
                                {
                                    if (StealFromInventory(target, ref target.inventory[i]))
                                        stolen = true;
                                }
                            }
                            if (stolen)
                            {
                                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), new Color(255, 50, 50));
                                CombatText.NewText(target.Hitbox, new Color(255, 50, 50), Language.GetTextValue("Mods.FargowiltasSouls.EternityMode.Stolen"), true);
                            }
                        }
                        break;

                    case NPCID.DD2WyvernT1:
                    case NPCID.DD2WyvernT2:
                    case NPCID.DD2WyvernT3:
                        target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
                        target.AddBuff(BuffID.Rabies, 3600);
                        break;

                    case NPCID.DD2KoboldFlyerT2:
                    case NPCID.DD2KoboldFlyerT3:
                    case NPCID.DD2KoboldWalkerT2:
                    case NPCID.DD2KoboldWalkerT3:
                        target.AddBuff(ModContent.BuffType<Fused>(), 1800);
                        break;

                    case NPCID.DD2OgreT2:
                    case NPCID.DD2OgreT3:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 60);
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        target.AddBuff(BuffID.BrokenArmor, 300);
                        break;

                    case NPCID.DD2LightningBugT3:
                        target.AddBuff(BuffID.Electrified, 300);
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Webbed, 60);
                        break;

                    case NPCID.DD2SkeletonT1:
                    case NPCID.DD2SkeletonT3:
                        target.AddBuff(ModContent.BuffType<Shadowflame>(), 300);
                        target.AddBuff(ModContent.BuffType<Rotting>(), 1200);
                        break;

                    case NPCID.DD2GoblinT1:
                    case NPCID.DD2GoblinT2:
                    case NPCID.DD2GoblinT3:
                        target.AddBuff(BuffID.Poisoned, 300);
                        target.AddBuff(BuffID.Bleeding, 300);
                        break;

                    case NPCID.SolarCrawltipedeHead:
                    case NPCID.SolarCrawltipedeBody:
                    case NPCID.SolarCrawltipedeTail:
                    case NPCID.SolarCorite:
                    case NPCID.SolarSolenian:
                    case NPCID.SolarDrakomire:
                    case NPCID.SolarDrakomireRider:
                    case NPCID.SolarSpearman:
                    case NPCID.SolarSroller:
                        target.AddBuff(BuffID.OnFire, 600);
                        target.AddBuff(BuffID.Burning, 300);
                        break;

                    case NPCID.DesertScorpionWalk:
                    case NPCID.DesertScorpionWall:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
                        break;

                    case NPCID.MisterStabby:
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                        break;

                    case NPCID.SnowBalla:
                    case NPCID.SnowmanGangsta:
                        target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
                        target.AddBuff(BuffID.Frostburn, 300);
                        break;

                    case NPCID.NebulaBeast:
                        target.AddBuff(BuffID.Rabies, 3600);
                        goto case NPCID.NebulaBrain;
                    case NPCID.NebulaHeadcrab:
                    case NPCID.NebulaBrain:
                    case NPCID.NebulaSoldier:
                        target.AddBuff(ModContent.BuffType<Berserked>(), 300);
                        target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
                        break;

                    case NPCID.StardustCellBig:
                    case NPCID.StardustCellSmall:
                    case NPCID.StardustWormHead:
                    case NPCID.StardustWormBody:
                    case NPCID.StardustWormTail:
                    case NPCID.StardustSpiderBig:
                    case NPCID.StardustSpiderSmall:
                    case NPCID.StardustJellyfishBig:
                    case NPCID.StardustJellyfishSmall:
                    case NPCID.StardustSoldier:
                        target.AddBuff(BuffID.Obstructed, 20);
                        target.AddBuff(BuffID.Blackout, 300);
                        break;

                    case NPCID.LunarTowerNebula:
                    case NPCID.LunarTowerSolar:
                    case NPCID.LunarTowerStardust:
                    case NPCID.LunarTowerVortex:
                        target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
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
                        target.AddBuff(BuffID.Poisoned, 300);
                        break;

                    case NPCID.VileSpit:
                        target.AddBuff(ModContent.BuffType<Rotting>(), 240);
                        break;

                    case NPCID.DungeonGuardian:
                        target.AddBuff(ModContent.BuffType<GodEater>(), 420);
                        target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 420);
                        target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 420);
                        target.immune = false;
                        target.immuneTime = 0;
                        break;

                    case NPCID.EnchantedSword:
                        target.AddBuff(ModContent.BuffType<Purified>(), 300);
                        break;

                    case NPCID.CursedHammer:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        break;

                    case NPCID.CrimsonAxe:
                        target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        break;

                    case NPCID.WalkingAntlion:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Dazed, 60);
                        break;

                    case NPCID.AnglerFish:
                        target.AddBuff(BuffID.Bleeding, 300);
                        break;

                    case NPCID.BloodFeeder:
                        target.AddBuff(BuffID.Bleeding, 300);
                        npc.life += damage * 2;
                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage * 2);
                        npc.damage = (int)(npc.damage * 1.1f);
                        npc.defDamage = (int)(npc.defDamage * 1.1f);
                        npc.netUpdate = true;
                        break;

                    case NPCID.Psycho:
                        target.AddBuff(BuffID.Obstructed, 120);
                        break;

                    case NPCID.LostGirl:
                    case NPCID.Nymph:
                        target.AddBuff(ModContent.BuffType<Lovestruck>(), 240);
                        npc.life += damage * 2;
                        if (npc.life > npc.lifeMax)
                            npc.life = npc.lifeMax;
                        CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage * 2);
                        npc.netUpdate = true;
                        break;

                    case NPCID.FloatyGross:
                        target.AddBuff(BuffID.OgreSpit, 240);
                        break;

                    case NPCID.WyvernHead:
                    case NPCID.WyvernBody:
                    case NPCID.WyvernBody2:
                    case NPCID.WyvernBody3:
                    case NPCID.WyvernLegs:
                    case NPCID.WyvernTail:
                        target.AddBuff(ModContent.BuffType<Crippled>(), 240);
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 240);
                        break;

                    case NPCID.DuneSplicerHead:
                    case NPCID.DuneSplicerBody:
                    case NPCID.DuneSplicerTail:
                        target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
                        break;

                    case NPCID.BloodCrawler:
                    case NPCID.BloodCrawlerWall:
                    case NPCID.JungleCreeper:
                    case NPCID.JungleCreeperWall:
                    case NPCID.WallCreeper:
                    case NPCID.WallCreeperWall:
                    case NPCID.BlackRecluse:
                    case NPCID.BlackRecluseWall:
                        target.AddBuff(ModContent.BuffType<Infested>(), 300);
                        break;

                    case NPCID.DungeonSpirit:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Cursed, 30);
                        break;

                    case NPCID.Mothron:
                        target.AddBuff(BuffID.Rabies, 3600);
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(ModContent.BuffType<Stunned>(), 60);
                        break;

                    case NPCID.MothronSpawn:
                        target.AddBuff(BuffID.Rabies, 1800);
                        target.AddBuff(ModContent.BuffType<Guilty>(), 300);
                        break;

                    case NPCID.DeadlySphere:
                        target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                        break;

                    case NPCID.HeadlessHorseman:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Cursed, 30);
                        target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                        break;

                    case NPCID.Yeti:
                        target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Frozen, 30);
                        break;

                    default:
                        break;
                }

                if (BeetleUtilAura)
                {
                    target.GetModPlayer<FargoPlayer>().AddBuffNoStack(BuffID.Frozen, 30);
                }
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if (FargoSoulsWorld.EternityMode)
            {
                spawnRate = (int)(spawnRate * 0.9);
                maxSpawns = (int)(maxSpawns * 1.25f);
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            //layers
            int y = spawnInfo.spawnTileY;
            bool cavern = y >= Main.maxTilesY * 0.4f && y <= Main.maxTilesY * 0.8f;
            bool underground = y > Main.worldSurface && y <= Main.maxTilesY * 0.4f;
            bool surface = y < Main.worldSurface && !spawnInfo.sky;
            bool wideUnderground = cavern || underground;
            bool underworld = spawnInfo.player.ZoneUnderworldHeight;
            bool sky = spawnInfo.sky;

            //times
            bool night = !Main.dayTime;
            bool day = Main.dayTime;

            //biomes
            bool noBiome = Fargowiltas.NoBiomeNormalSpawn(spawnInfo);
            bool ocean = spawnInfo.player.ZoneBeach;
            bool dungeon = spawnInfo.player.ZoneDungeon;
            bool meteor = spawnInfo.player.ZoneMeteor;
            bool spiderCave = spawnInfo.spiderCave;
            bool mushroom = spawnInfo.player.ZoneGlowshroom;
            bool jungle = spawnInfo.player.ZoneJungle;
            bool granite = spawnInfo.granite;
            bool marble = spawnInfo.marble;
            bool corruption = spawnInfo.player.ZoneCorrupt;
            bool crimson = spawnInfo.player.ZoneCrimson;
            bool snow = spawnInfo.player.ZoneSnow;
            bool hallow = spawnInfo.player.ZoneHoly;
            bool desert = spawnInfo.player.ZoneDesert;

            bool nebulaTower = spawnInfo.player.ZoneTowerNebula;
            bool vortexTower = spawnInfo.player.ZoneTowerVortex;
            bool stardustTower = spawnInfo.player.ZoneTowerStardust;
            bool solarTower = spawnInfo.player.ZoneTowerSolar;

            bool water = spawnInfo.water;

            //events
            bool goblinArmy = Main.invasionType == 1;
            bool frostLegion = Main.invasionType == 2;
            bool pirates = Main.invasionType == 3;
            bool oldOnesArmy = DD2Event.Ongoing && spawnInfo.player.ZoneOldOneArmy;
            bool frostMoon = surface && night && Main.snowMoon;
            bool pumpkinMoon = surface && night && Main.pumpkinMoon;
            bool solarEclipse = surface && day && Main.eclipse;
            bool martianMadness = Main.invasionType == 4;
            bool lunarEvents = NPC.LunarApocalypseIsUp && (nebulaTower || vortexTower || stardustTower || solarTower);
            bool monsterMadhouse = MMWorld.MMArmy;

            //no work?
            //is lava on screen
            bool nearLava = Collision.LavaCollision(spawnInfo.player.position, spawnInfo.spawnTileX, spawnInfo.spawnTileY);
            bool noInvasion = Fargowiltas.NoInvasion(spawnInfo);
            bool normalSpawn = !spawnInfo.playerInTown && noInvasion && !oldOnesArmy;

            bool sinisterIcon = spawnInfo.player.GetModPlayer<FargoPlayer>().SinisterIcon;

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

                                if (NPC.downedBoss3 && !NPC.downedMechBoss2 && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                                    pool[NPCID.EyeofCthulhu] = Main.bloodMoon ? .0004f : .0002f;
                            }
                        }

                        if (Main.slimeRain && NPC.downedBoss2 && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                            pool[NPCID.KingSlime] = 0.004f;
                    }
                    else if (wideUnderground)
                    {
                        if (nearLava)
                        {
                            pool[NPCID.FireImp] = .05f;
                            pool[NPCID.LavaSlime] = .05f;
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
                            pool[NPCID.WyvernHead] = .005f;
                        }
                    }

                    //height-independent biomes
                    if (corruption)
                    {
                        if (NPC.downedBoss2)
                        {
                            pool[NPCID.SeekerHead] = .01f;
                            if (normalSpawn && NPC.downedBoss3 && !underworld && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                                pool[NPCID.EaterofWorldsHead] = .0002f;
                        }
                    }

                    if (crimson)
                    {
                        if (NPC.downedBoss2)
                        {
                            pool[NPCID.IchorSticker] = .01f;
                            if (normalSpawn && NPC.downedBoss3 && !underworld && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
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
                                if (noBiome && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                                    pool[NPCID.KingSlime] = Main.slimeRain ? .0004f : .0002f;

                                if (NPC.downedMechBossAny && (noBiome || dungeon))
                                    pool[NPCID.CultistArcherWhite] = .01f;

                                if(jungle)
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
                                    if (!sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                                    {
                                        pool[NPCID.Retinazer] = .002f;
                                        pool[NPCID.Spazmatism] = .002f;
                                        pool[NPCID.TheDestroyer] = .002f;
                                        pool[NPCID.SkeletronPrime] = .002f;
                                    }
                                }*/
                            }
                            
                            if (noInvasion && !oldOnesArmy && !sinisterIcon)
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

                                if (!sinisterIcon && !NPC.downedMechBoss2 && !FargoSoulsUtil.AnyBossAlive())
                                    pool[NPCID.EyeofCthulhu] = .001f;

                                if (NPC.downedMechBossAny)
                                    pool[NPCID.Probe] = 0.1f;

                                if (NPC.downedPlantBoss) //GODLUL
                                {
                                    if (!sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                                    {
                                        pool[NPCID.Retinazer] = .0001f;
                                        pool[NPCID.Spazmatism] = .0001f;
                                        pool[NPCID.TheDestroyer] = .0001f;
                                        pool[NPCID.SkeletronPrime] = .0001f;
                                    }

                                    //if (!spawnInfo.player.GetModPlayer<FargoPlayer>().SkullCharm)
                                    pool[NPCID.SkeletonSniper] = .02f;
                                    pool[NPCID.SkeletonCommando] = .02f;
                                    pool[NPCID.TacticalSkeleton] = .02f;
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

                                    if (NPC.downedHalloweenKing && !sinisterIcon)
                                    {
                                        //pool[NPCID.HeadlessHorseman] = .01f;
                                        pool[NPCID.Pumpking] = .0025f;
                                    }
                                }
                                else //in some biome
                                {
                                    if (hallow)
                                    {
                                        pool[NPCID.PresentMimic] = .05f;
                                    }
                                    else if (crimson || corruption)
                                    {
                                        pool[NPCID.Splinterling] = .05f;

                                        if (NPC.downedHalloweenTree && !sinisterIcon)
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

                                        if (NPC.downedChristmasTree && !sinisterIcon)
                                        {
                                            pool[NPCID.Everscream] = .0025f;
                                        }

                                        if (NPC.downedChristmasSantank && !sinisterIcon)
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
                            if (!Main.raining)
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
                            if (NPC.downedFishron && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
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

                        if (nearLava)
                        {
                            pool[NPCID.FireImp] = .02f;
                            pool[NPCID.LavaSlime] = .02f;
                        }

                        if (cavern)
                        {
                            if (noBiome && NPC.downedBoss3)
                                pool[NPCID.DarkCaster] = .05f;
                        }

                        if (dungeon && night && normalSpawn && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
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
                                    if (NPC.downedChristmasIceQueen && !sinisterIcon)
                                        pool[NPCID.IceQueen] = .0025f;
                                }
                            }
                        }

                        if (NPC.downedPlantBoss)// && !spawnInfo.player.GetModPlayer<FargoPlayer>().SkullCharm)
                        {
                            pool[NPCID.DiabolistRed] = .001f;
                            pool[NPCID.DiabolistWhite] = .001f;
                            pool[NPCID.Necromancer] = .001f;
                            pool[NPCID.NecromancerArmored] = .001f;
                            pool[NPCID.RaggedCaster] = .001f;
                            pool[NPCID.RaggedCasterOpenCoat] = .001f;
                        }

                        if (NPC.downedAncientCultist && dungeon && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                            pool[NPCID.CultistBoss] = 0.00002f;

                        if (spawnInfo.player.ZoneUndergroundDesert)
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

                        if (!sinisterIcon && !FargoSoulsUtil.BossIsAlive(ref wallBoss, NPCID.WallofFlesh))
                            pool[NPCID.TheHungryII] = .03f;

                        if (NPC.downedMechBossAny)
                        {
                            pool[NPCID.BlazingWheel] = .05f;
                        }
                        
                        if (NPC.downedPlantBoss)// && !spawnInfo.player.GetModPlayer<FargoPlayer>().SkullCharm)
                        {
                            pool[NPCID.DiabolistRed] = .001f;
                            pool[NPCID.DiabolistWhite] = .001f;
                            pool[NPCID.Necromancer] = .001f;
                            pool[NPCID.NecromancerArmored] = .001f;
                            pool[NPCID.RaggedCaster] = .001f;
                            pool[NPCID.RaggedCasterOpenCoat] = .001f;
                        }

                        if (FargoSoulsWorld.downedBetsy && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                            pool[NPCID.DD2Betsy] = .0002f;
                    }
                    else if (sky)
                    {
                        if (normalSpawn && !FargoSoulsUtil.AnyBossAlive())
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

                            if (NPC.downedMoonlord && !sinisterIcon)
                            {
                                pool[NPCID.MoonLordCore] = 0.0002f;
                            }
                        }
                    }

                    //height-independent biomes
                    if (corruption)
                    {
                        if (normalSpawn && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                            pool[NPCID.EaterofWorldsHead] = .0002f;
                    }

                    if (crimson)
                    {
                        if (normalSpawn && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                            pool[NPCID.BrainofCthulhu] = .0002f;
                    }

                    if (jungle)
                    {
                        if (normalSpawn && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                            pool[NPCID.QueenBee] = .0001f;

                        if (!surface)
                        {
                            pool[NPCID.BigMimicJungle] = .0025f;

                            if (NPC.downedGolemBoss && !sinisterIcon && !FargoSoulsUtil.AnyBossAlive())
                                pool[NPCID.Plantera] = .00005f;
                        }
                    }

                    if (spawnInfo.lihzahrd && spawnInfo.spawnTileType == TileID.LihzahrdBrick)
                    {
                        pool[NPCID.BlazingWheel] = .1f;
                        pool[NPCID.SpikeBall] = .1f;
                    }

                    if (ocean && spawnInfo.water)
                    {
                        pool[NPCID.AnglerFish] = .1f;
                    }
                }

                //maybe make moon lord core handle these spawns...?
                /*if (FargoSoulsUtil.BossIsAlive(ref moonBoss, NPCID.MoonLordCore))
                {
                    pool[NPCID.SolarCrawltipedeHead] = 1f;
                    pool[NPCID.StardustJellyfishBig] = 3f;
                    pool[NPCID.VortexRifleman] = 3f;
                    pool[NPCID.NebulaBrain] = 2f;

                    pool[NPCID.AncientCultistSquidhead] = 3f;
                    pool[NPCID.CultistDragonHead] = .5f;
                }*/
            }

            if (monsterMadhouse)
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
            }
        }

        public override bool PreNPCLoot(NPC npc)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.Medusa:
                        if (!Main.hardMode && !npc.SpawnedFromStatue)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            if (Main.rand.NextBool(10))
                                Item.NewItem(npc.Hitbox, ItemID.PocketMirror);
                            return false;
                        }
                        break;

                    case NPCID.WyvernHead:
                        if (!Main.hardMode)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            Item.NewItem(npc.Hitbox, ItemID.FloatingIslandFishingCrate);
                            if (Main.rand.NextBool(5))
                                Item.NewItem(npc.Hitbox, ModContent.ItemType<WyvernFeather>());
                            return false;
                        }
                        break;

                    case NPCID.RedDevil:
                        if (!Main.hardMode)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            if (Main.rand.NextBool(3))
                                Item.NewItem(npc.Hitbox, ItemID.DemonScythe);
                            return false;
                        }
                        break;

                    case NPCID.IchorSticker:
                        if (!Main.hardMode)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            int[] drops = { ItemID.TheUndertaker, ItemID.TheRottedFork, ItemID.CrimsonRod, ItemID.CrimsonHeart, ItemID.PanicNecklace };
                            Item.NewItem(npc.Hitbox, drops[Main.rand.Next(drops.Length)]);
                            return false;
                        }
                        break;

                    case NPCID.SeekerHead:
                        if (!Main.hardMode)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            int[] drops = { ItemID.BallOHurt, ItemID.BandofStarpower, ItemID.Musket, ItemID.ShadowOrb, ItemID.Vilethorn };
                            Item.NewItem(npc.Hitbox, drops[Main.rand.Next(drops.Length)]);
                            return false;
                        }
                        break;

                    case NPCID.Mimic:
                        if (!Main.hardMode && !npc.SpawnedFromStatue)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            int[] drops = { ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook, ItemID.StarCloak };
                            Item.NewItem(npc.Hitbox, drops[Main.rand.Next(drops.Length)]);
                            return false;
                        }
                        break;

                    case NPCID.AngryNimbus:
                        if (!Main.hardMode)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            Item.NewItem(npc.Hitbox, ItemID.FloatingIslandFishingCrate);
                            return false;
                        }
                        break;

                    case NPCID.DuneSplicerHead:
                        if (!Main.hardMode)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            if (Main.rand.NextBool(3))
                                Item.NewItem(npc.Hitbox, ItemID.SandstorminaBottle); //desert crate soon
                            return false;
                        }
                        break;

                    case NPCID.PigronCorruption:
                    case NPCID.PigronCrimson:
                    case NPCID.PigronHallow:
                        if (!Main.hardMode && !npc.SpawnedFromStatue)
                        {
                            Item.NewItem(npc.Hitbox, ItemID.GoldCoin, 1 + Main.rand.Next(5));
                            Item.NewItem(npc.Hitbox, ItemID.Bacon, 1 + Main.rand.Next(15));
                            return false;
                        }
                        break;

                    default:
                        break;
                }
            }

            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            if (FargoSoulsWorld.EternityMode && !npc.SpawnedFromStatue)
            {
                if (npc.lastInteraction != -1 && Main.player[npc.lastInteraction].GetModPlayer<FargoPlayer>().TimsConcoction)
                {
                    switch (npc.type)
                    {
                        case NPCID.BlueSlime:
                            Item.NewItem(npc.Hitbox, npc.netID == NPCID.Pinky ? ItemID.TeleportationPotion : ItemID.RecallPotion);
                            break;

                        case NPCID.DemonEye:
                        case NPCID.DemonEyeOwl:
                        case NPCID.DemonEyeSpaceship:
                            Item.NewItem(npc.Hitbox, ItemID.NightOwlPotion);
                            break;

                        case NPCID.MossHornet:
                        case NPCID.Hornet:
                        case NPCID.HornetFatty:
                        case NPCID.HornetHoney:
                        case NPCID.HornetLeafy:
                        case NPCID.HornetSpikey:
                        case NPCID.HornetStingy:
                            Item.NewItem(npc.Hitbox, ItemID.RagePotion);
                            break;

                        case NPCID.Bee:
                        case NPCID.BeeSmall:
                            if (Main.rand.NextBool(3))
                                Item.NewItem(npc.Hitbox, ItemID.WrathPotion);
                            break;

                        case NPCID.GoblinPeon:
                        case NPCID.GoblinThief:
                        case NPCID.GoblinWarrior:
                            Item.NewItem(npc.Hitbox, ItemID.BattlePotion);
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
                            Item.NewItem(npc.Hitbox, ItemID.SonarPotion);
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
                            Item.NewItem(npc.Hitbox, ItemID.ThornsPotion);
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
                            Item.NewItem(npc.Hitbox, ItemID.StinkPotion);
                            break;

                        case NPCID.Antlion:
                            Item.NewItem(npc.Hitbox, ItemID.BuilderPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.WallCreeper:
                        case NPCID.WallCreeperWall:
                        case NPCID.BlackRecluse:
                        case NPCID.BlackRecluseWall:
                        case NPCID.JungleCreeper:
                        case NPCID.JungleCreeperWall:
                            Item.NewItem(npc.Hitbox, ItemID.TrapsightPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.FireImp:
                            Item.NewItem(npc.Hitbox, ItemID.InfernoPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.Harpy:
                            Item.NewItem(npc.Hitbox, ItemID.CalmingPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.Crab:
                            Item.NewItem(npc.Hitbox, Main.rand.NextBool() ? ItemID.FishingPotion : ItemID.CratePotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.IceSlime:
                        case NPCID.SpikedIceSlime:
                            Item.NewItem(npc.Hitbox, ItemID.WaterWalkingPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.Piranha:
                        case NPCID.Arapaima:
                            Item.NewItem(npc.Hitbox, ItemID.GillsPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.BloodZombie:
                            Item.NewItem(npc.Hitbox, ItemID.RegenerationPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.LavaSlime:
                            Item.NewItem(npc.Hitbox, ItemID.WarmthPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.UmbrellaSlime:
                            Item.NewItem(npc.Hitbox, ItemID.FeatherfallPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.Drippler:
                            Item.NewItem(npc.Hitbox, ItemID.HeartreachPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.GiantWormHead:
                        case NPCID.DiggerHead:
                            Item.NewItem(npc.Hitbox, ItemID.WormholePotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.GreekSkeleton:
                            Item.NewItem(npc.Hitbox, ItemID.AmmoReservationPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.GraniteFlyer:
                            Item.NewItem(npc.Hitbox, ItemID.IronskinPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.GraniteGolem:
                            Item.NewItem(npc.Hitbox, ItemID.EndurancePotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.Shark:
                            Item.NewItem(npc.Hitbox, ItemID.FlipperPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.GoblinArcher:
                            Item.NewItem(npc.Hitbox, ItemID.ArcheryPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.GoblinSorcerer:
                            Item.NewItem(npc.Hitbox, ItemID.ManaRegenerationPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.PinkJellyfish:
                        case NPCID.BlueJellyfish:
                        case NPCID.GreenJellyfish:
                        case NPCID.AnglerFish:
                            Item.NewItem(npc.Hitbox, ItemID.ShinePotion, Main.rand.Next(3) + 1);
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
                            Item.NewItem(npc.Hitbox, ItemID.InvisibilityPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.MotherSlime:
                            Item.NewItem(npc.Hitbox, ItemID.SummoningPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.Nymph:
                            Item.NewItem(npc.Hitbox, ItemID.LovePotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.Tumbleweed:
                        case NPCID.DesertBeast:
                            Item.NewItem(npc.Hitbox, ItemID.SwiftnessPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.TombCrawlerHead:
                        case NPCID.DuneSplicerHead:
                            Item.NewItem(npc.Hitbox, ItemID.HunterPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.DoctorBones:
                            case NPCID.Mimic:
                            Item.NewItem(npc.Hitbox, ItemID.SpelunkerPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.UndeadMiner:
                            Item.NewItem(npc.Hitbox, ItemID.MiningPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.Tim:
                            Item.NewItem(npc.Hitbox, ItemID.MagicPowerPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.BoneSerpentHead:
                            Item.NewItem(npc.Hitbox, ItemID.ObsidianSkinPotion, Main.rand.Next(12) + 1);
                            break;


                        //todo:
                        //gnome drops lesser luck
                        //dungeon slime drops luck
                        //clown drops greater luck


                        case NPCID.ChaosElemental:
                            Item.NewItem(npc.Hitbox, ItemID.TeleportationPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.RainbowSlime:
                            Item.NewItem(npc.Hitbox, ItemID.RegenerationPotion, Main.rand.Next(3) + 1);
                            break;

                        case NPCID.RuneWizard:
                            Item.NewItem(npc.Hitbox, ItemID.MagicPowerPotion, Main.rand.Next(6) + 1);
                            Item.NewItem(npc.Hitbox, ItemID.ManaRegenerationPotion, Main.rand.Next(6) + 1);
                            break;

                        case NPCID.GoblinSummoner:
                            Item.NewItem(npc.Hitbox, ItemID.SummoningPotion, Main.rand.Next(12) + 1);
                            break;

                        case NPCID.PirateCaptain:
                            Item.NewItem(npc.Hitbox, ItemID.AmmoReservationPotion, Main.rand.Next(12) + 1);
                            break;

                        case NPCID.WyvernHead:
                            Item.NewItem(npc.Hitbox, ItemID.GravitationPotion, Main.rand.Next(12) + 1);
                            break;

                        case NPCID.BigMimicCorruption:
                        case NPCID.BigMimicCrimson:
                        case NPCID.BigMimicHallow:
                        case NPCID.BigMimicJungle:
                            Item.NewItem(npc.Hitbox, ItemID.LifeforcePotion, Main.rand.Next(12) + 1);
                            break;

                        default:
                            break;
                    }
                }

                switch (npc.type)
                {
                    case NPCID.BrainScrambler:
                        if (Main.rand.NextBool(100))
                            Item.NewItem(npc.Hitbox, ItemID.BrainScrambler);
                        break;

                    case NPCID.GiantWormHead:
                    case NPCID.DiggerHead:
                        Item.NewItem(npc.Hitbox, ItemID.WormTooth, Main.rand.Next(3, 9));
                        break;

                    case NPCID.BlackRecluse:
                    case NPCID.BlackRecluseWall:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ItemID.SpiderEgg);
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
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<RabiesShot>());
                        break;

                    case NPCID.CorruptBunny:
                    case NPCID.CrimsonBunny:
                    case NPCID.CorruptGoldfish:
                    case NPCID.CrimsonGoldfish:
                    case NPCID.CorruptPenguin:
                    case NPCID.CrimsonPenguin:
                        if (Main.rand.Next(Main.hardMode ? 10 : 25) == 0)
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<Items.Accessories.Masomode.SqueakyToy>());
                        break;
                        
                    case NPCID.DesertBeast:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ItemID.PocketMirror);
                        goto case NPCID.DesertGhoul;
                    case NPCID.DesertScorpionWalk:
                    case NPCID.DesertScorpionWall:
                    case NPCID.DesertLamiaDark:
                    case NPCID.DesertLamiaLight:
                    case NPCID.DesertGhoul:
                    case NPCID.DesertGhoulCorruption:
                    case NPCID.DesertGhoulCrimson:
                    case NPCID.DesertGhoulHallow:
                        if (Main.rand.NextBool(3))
                            Item.NewItem(npc.Hitbox, ItemID.DesertFossil, Main.rand.Next(10) + 1);
                        break;

                    case NPCID.Crab:
                    case NPCID.Squid:
                    case NPCID.SeaSnail:
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.position, npc.width, npc.height,
                                Main.rand.NextBool() ? ItemID.Starfish : ItemID.Seashell, Main.rand.Next(3) + 1);
                        break;

                    case NPCID.DoctorBones:
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<SkullCharm>());
                        break;

                    case NPCID.BlueArmoredBones:
                    case NPCID.BlueArmoredBonesMace:
                    case NPCID.BlueArmoredBonesNoPants:
                    case NPCID.BlueArmoredBonesSword:
                    case NPCID.HellArmoredBones:
                    case NPCID.HellArmoredBonesMace:
                    case NPCID.HellArmoredBonesSpikeShield:
                    case NPCID.HellArmoredBonesSword:
                    case NPCID.RustyArmoredBonesAxe:
                    case NPCID.RustyArmoredBonesFlail:
                    case NPCID.RustyArmoredBonesSword:
                    case NPCID.RustyArmoredBonesSwordNoArmor:
                    case NPCID.SkeletonSniper:
                    case NPCID.TacticalSkeleton:
                    case NPCID.SkeletonCommando:
                    case NPCID.DiabolistRed:
                    case NPCID.DiabolistWhite:
                    case NPCID.Necromancer:
                    case NPCID.NecromancerArmored:
                    case NPCID.RaggedCaster:
                    case NPCID.RaggedCasterOpenCoat:
                        Item.NewItem(npc.Hitbox, ItemID.Bone);
                        break;

                    case NPCID.IceGolem:
                        Item.NewItem(npc.Hitbox, ModLoader.GetMod("Fargowiltas").ItemType("IceCrate"));
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<FrigidGemstone>());
                        if (Main.rand.NextBool(20))
                            Item.NewItem(npc.Hitbox, ItemID.BlizzardinaBottle);
                        break;

                    case NPCID.WyvernHead:
                        Item.NewItem(npc.Hitbox, ItemID.FloatingIslandFishingCrate);
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<WyvernFeather>());
                        if (Main.rand.NextBool(20))
                            Item.NewItem(npc.Hitbox, ItemID.CloudinaBottle);
                        break;

                    case NPCID.SandElemental:
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<SandsofTime>());
                        if (Main.rand.NextBool(20))
                            Item.NewItem(npc.Hitbox, ItemID.SandstorminaBottle);
                        break;

                    case NPCID.PirateCaptain:
                        if (Main.rand.NextBool(15))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<GoldenDippingVat>());
                        break;

                    case NPCID.PirateShip:
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<SecurityWallet>());
                        if (Main.rand.NextBool(100))
                            Item.NewItem(npc.Hitbox, ItemID.CoinGun);
                        if (Main.rand.NextBool(100))
                            Item.NewItem(npc.Hitbox, ItemID.LuckyCoin);
                        break;

                    case NPCID.Nymph:
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<NymphsPerfume>());
                        break;

                    case NPCID.MourningWood:
                        Item.NewItem(npc.Hitbox, ItemID.GoodieBag, Main.rand.Next(5) + 1);
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.Hitbox, ItemID.BloodyMachete);
                        break;

                    case NPCID.Pumpking:
                        Item.NewItem(npc.Hitbox, ItemID.GoodieBag, Main.rand.Next(5) + 1);
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.Hitbox, ItemID.BladedGlove);
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<PumpkingsCape>());

                        break;

                    case NPCID.Everscream:
                    case NPCID.SantaNK1:
                        Item.NewItem(npc.Hitbox, ItemID.Present, Main.rand.Next(5) + 1);
                        break;

                    case NPCID.IceQueen:
                        Item.NewItem(npc.Hitbox, ItemID.Present, Main.rand.Next(5) + 1);
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<IceQueensCrown>());
                        break;

                    case NPCID.MartianSaucerCore:
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<SaucerControlConsole>());
                        break;

                    case NPCID.LavaSlime:
                        if (Main.rand.NextBool(100))
                            Item.NewItem(npc.Hitbox, ItemID.LavaCharm);
                        break;

                    case NPCID.DesertDjinn:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ItemID.FlyingCarpet);
                        break;

                    case NPCID.SnowBalla:
                    case NPCID.SnowmanGangsta:
                    case NPCID.MisterStabby:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<OrdinaryCarrot>());
                        break;

                    case NPCID.AngryTrapper:
                        if (Main.rand.NextBool())
                            Item.NewItem(npc.Hitbox, ItemID.Vine);
                        break;

                    case NPCID.MossHornet:
                        if (Main.rand.NextBool())
                            Item.NewItem(npc.Hitbox, ItemID.Stinger);
                        goto case NPCID.Hornet;
                    case NPCID.Hornet:
                    case NPCID.HornetFatty:
                    case NPCID.HornetHoney:
                    case NPCID.HornetLeafy:
                    case NPCID.HornetSpikey:
                    case NPCID.HornetStingy:
                        if (Main.rand.NextBool(10))
                            Item.NewItem(npc.Hitbox, ItemID.JungleGrassSeeds);
                        break;

                    case NPCID.FungiBulb:
                    case NPCID.GiantFungiBulb:
                    case NPCID.AnomuraFungus:
                    case NPCID.MushiLadybug:
                    case NPCID.ZombieMushroom:
                    case NPCID.ZombieMushroomHat:
                    case NPCID.FungoFish:
                        Item.NewItem(npc.Hitbox, ItemID.GlowingMushroom, Main.rand.Next(5) + 1);
                        if (Main.rand.NextBool(5))
                            Item.NewItem(npc.Hitbox, ItemID.MushroomGrassSeeds);
                        if (Main.rand.NextBool(20))
                            Item.NewItem(npc.Hitbox, ItemID.TruffleWorm);
                        break;

                    case NPCID.Demon:
                    case NPCID.RedDevil:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ItemID.Blindfold);
                        break;

                    case NPCID.Piranha:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ItemID.AdhesiveBandage);
                        break;

                    case NPCID.Derpling:
                        if (Main.rand.NextBool(50))
                            Item.NewItem(npc.Hitbox, ItemID.TrifoldMap);
                        break;

                    case NPCID.Clown:
                        Item.NewItem(npc.Hitbox, ItemID.PartyGirlGrenade, Main.rand.Next(10) + 1);
                        break;

                    #region boss drops
                    case NPCID.DungeonGuardian:
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<SinisterIcon>());
                        break;
                    #endregion

                    case NPCID.Painter:
                        if (FargoSoulsWorld.downedMutant && NPC.AnyNPCs(ModContent.NPCType<MutantBoss.MutantBoss>()))
                            Item.NewItem(npc.Hitbox, ModContent.ItemType<ScremPainting>());
                        break;

                    default:
                        break;
                }
            }
        }

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.Drippler:
                        if (Main.rand.NextBool(3) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                int n = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), NPCID.DemonEye);
                                if (n != 200)
                                {
                                    Main.npc[n].velocity = new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3));
                                    Main.npc[n].netUpdate = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        break;

                    case NPCID.AngryBones:
                    case NPCID.AngryBonesBig:
                    case NPCID.AngryBonesBigHelmet:
                    case NPCID.AngryBonesBigMuscle:
                        if (Main.rand.NextBool(5) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.CursedSkull);
                            if (n < 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        goto case 269;

                    //all armored bones
                    case 269:
                    case 270:
                    case 271:
                    case 272:
                    case 273:
                    case 274:
                    case 275:
                    case 276:
                    case 277:
                    case 278:
                    case 279:
                    case 280:
                        /*if (Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.lastInteraction].GetModPlayer<FargoPlayer>().NecromanticBrew)
                        {
                            int chance = (bool)ModLoader.GetMod("Fargowiltas").Call("GetDownedEnemy", "babyGuardian") ? 100 : 10;
                            if (Main.rand.Next(chance) == 0)
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<BabyGuardian>());
                                if (n < 200 && Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }*/
                        break;

                    case NPCID.DungeonSlime:
                        if (NPC.downedPlantBoss && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int paladin = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), NPCID.Paladin);
                            if (paladin != 200)
                            {
                                Vector2 center = Main.npc[paladin].Center;
                                Main.npc[paladin].width = (int)(Main.npc[paladin].width * .65f);
                                Main.npc[paladin].height = (int)(Main.npc[paladin].height * .65f);
                                Main.npc[paladin].scale = .65f;
                                Main.npc[paladin].Center = center;
                                Main.npc[paladin].lifeMax /= 2;
                                Main.npc[paladin].life = Main.npc[paladin].lifeMax;
                                Main.npc[paladin].GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = true;
                                Main.npc[paladin].velocity = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 1));
                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, paladin);
                            }
                        }
                        break;

                    case NPCID.BlueSlime:
                        switch (npc.netID)
                        {
                            case NPCID.YellowSlime:
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (Main.rand.Next(3) != 0)
                                            continue;

                                        int spawn = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), 1);
                                        if (spawn != 200)
                                        {
                                            Main.npc[spawn].SetDefaults(NPCID.PurpleSlime);
                                            Main.npc[spawn].velocity.X = npc.velocity.X * 2f;
                                            Main.npc[spawn].velocity.Y = npc.velocity.Y;

                                            NPC spawn2 = Main.npc[spawn];
                                            spawn2.velocity.X = spawn2.velocity.X + (Main.rand.Next(-20, 20) * 0.1f + i * npc.direction * 0.3f);
                                            NPC spawn3 = Main.npc[spawn];
                                            spawn3.velocity.Y = spawn3.velocity.Y - (Main.rand.Next(0, 10) * 0.1f + i);
                                            Main.npc[spawn].ai[0] = -1000 * Main.rand.Next(3);

                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                                        }
                                    }
                                }
                                break;

                            case NPCID.PurpleSlime:
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (Main.rand.Next(3) != 0)
                                            continue;

                                        int spawn = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), 1);

                                        if (spawn != 200)
                                        {
                                            Main.npc[spawn].SetDefaults(NPCID.RedSlime);
                                            Main.npc[spawn].velocity.X = npc.velocity.X * 2f;
                                            Main.npc[spawn].velocity.Y = npc.velocity.Y;

                                            NPC spawn2 = Main.npc[spawn];
                                            spawn2.velocity.X = spawn2.velocity.X + (Main.rand.Next(-20, 20) * 0.1f + i * npc.direction * 0.3f);
                                            NPC spawn3 = Main.npc[spawn];
                                            spawn3.velocity.Y = spawn3.velocity.Y - (Main.rand.Next(0, 10) * 0.1f + i);
                                            Main.npc[spawn].ai[0] = -1000 * Main.rand.Next(3);

                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                                        }
                                    }
                                }
                                break;

                            case NPCID.RedSlime:
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (Main.rand.Next(3) != 0)
                                            continue;

                                        int spawn = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), 1);
                                        Main.npc[spawn].SetDefaults(NPCID.GreenSlime);
                                        Main.npc[spawn].velocity.X = npc.velocity.X * 2f;
                                        Main.npc[spawn].velocity.Y = npc.velocity.Y;

                                        NPC spawn2 = Main.npc[spawn];
                                        spawn2.velocity.X = spawn2.velocity.X + (Main.rand.Next(-20, 20) * 0.1f + i * npc.direction * 0.3f);
                                        NPC spawn3 = Main.npc[spawn];
                                        spawn3.velocity.Y = spawn3.velocity.Y - (Main.rand.Next(0, 10) * 0.1f + i);
                                        Main.npc[spawn].ai[0] = -1000 * Main.rand.Next(3);

                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                                    }
                                }
                                break;

                            case NPCID.Pinky:
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int i = 0; i < 3; i++)
                                    {
                                        if (Main.rand.Next(3) != 0)
                                            continue;

                                        int spawn = NPC.NewNPC((int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), 1);

                                        if (spawn != 200)
                                        {
                                            Main.npc[spawn].SetDefaults(i < 2 ? NPCID.YellowSlime : NPCID.MotherSlime);
                                            Main.npc[spawn].velocity.X = npc.velocity.X * 2f;
                                            Main.npc[spawn].velocity.Y = npc.velocity.Y;

                                            NPC spawn2 = Main.npc[spawn];
                                            spawn2.velocity.X = spawn2.velocity.X + (Main.rand.Next(-20, 20) * 0.1f + i * npc.direction * 0.3f);
                                            NPC spawn3 = Main.npc[spawn];
                                            spawn3.velocity.Y = spawn3.velocity.Y - (Main.rand.Next(0, 10) * 0.1f + i);
                                            Main.npc[spawn].ai[0] = -1000 * Main.rand.Next(3);

                                            if (Main.netMode == NetmodeID.Server)
                                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawn);
                                        }
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                        break;

                    case NPCID.DrManFly:
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 10; i++)
                                Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)),
                                    ProjectileID.DrManFlyFlask, npc.damage / 2, 1f, Main.myPlayer);
                        }
                        break;

                    case NPCID.Splinterling:
                        if (!NPC.downedPlantBoss)
                        {
                            npc.active = false;
                            Main.PlaySound(npc.DeathSound, npc.Center);
                            return false;
                        }
                        break;

                    case NPCID.FlyingSnake:
                        if (!masoBool[0])
                        {
                            masoBool[0] = true;
                            npc.life = npc.lifeMax;
                            npc.active = true;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                npc.netUpdate = true;
                                NetUpdateMaso(npc.whoAmI);
                            }
                            return false;
                        }
                        break;

                    case NPCID.Lihzahrd:
                    case NPCID.LihzahrdCrawler:
                        /*if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.UnitY * -6, ProjectileID.SpikyBallTrap, 30, 0f, Main.myPlayer);*/
                        break;

                    case NPCID.Clown:
                        
                        break;

                    case NPCID.Shark:
                        if (Main.hardMode && Main.rand.NextBool(4) && Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.Cthulunado, npc.damage / 2, 0f, Main.myPlayer, 16, 11);
                        break;

                    case NPCID.IchorSticker:
                        FargoSoulsUtil.XWay(5, npc.Center, ProjectileID.GoldenShowerHostile, 4, npc.damage / 4, 2);
                        break;

                    case NPCID.ZombieMushroom:
                    case NPCID.ZombieMushroomHat:
                    case NPCID.AnomuraFungus:
                    case NPCID.MushiLadybug:
                    case NPCID.FungoFish:
                    case NPCID.FungiBulb:
                    case NPCID.GiantFungiBulb:
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.hardMode)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                int spore = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.FungiSpore);
                                if (spore < 200)
                                {
                                    Main.npc[spore].damage = npc.damage / 3;
                                    Main.npc[spore].velocity = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)) / 2;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spore);
                                }
                            }
                        }
                        break;

                    case NPCID.Hornet:
                    case NPCID.HornetFatty:
                    case NPCID.HornetHoney:
                    case NPCID.HornetLeafy:
                    case NPCID.HornetSpikey:
                    case NPCID.HornetStingy:
                        if (FargoSoulsUtil.BossIsAlive(ref beeBoss, NPCID.QueenBee))
                        {
                            npc.active = false;
                            Main.PlaySound(npc.DeathSound, npc.Center);
                            return false;
                        }
                        break;

                    case NPCID.IlluminantBat:
                        if (masoBool[0])
                        {
                            npc.active = false;
                            Main.PlaySound(npc.DeathSound, npc.Center);
                            return false;
                        }
                        break;

                    case NPCID.Gastropod:
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                        {
                            Vector2 vel = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 4f;
                            for (int i = 0; i < 12; i++)
                                Projectile.NewProjectile(npc.Center, vel.RotatedBy(2 * Math.PI / 12 * i), ProjectileID.PinkLaser, npc.damage / 5, 0f, Main.myPlayer);
                        }
                        break;

                    case NPCID.DD2GoblinBomberT1:
                    case NPCID.DD2GoblinBomberT2:
                    case NPCID.DD2GoblinBomberT3:
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 3; i++)
                                Projectile.NewProjectile(npc.Center, new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-9f, -6f)),
                                  ProjectileID.DD2GoblinBomb, npc.damage / 4, 0, Main.myPlayer);
                        }
                        break;

                    case NPCID.PossessedArmor:
                        if (Main.rand.NextBool() && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Ghost);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        break;

                    case NPCID.Mummy:
                    case NPCID.DarkMummy:
                    case NPCID.LightMummy:
                        if (Main.rand.NextBool(5) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.WallCreeper);
                                if (n != 200)
                                {
                                    Main.npc[n].velocity.X = Main.rand.NextFloat(-5f, 5f);
                                    Main.npc[n].velocity.Y = Main.rand.NextFloat(-10f, 0f);
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        break;

                    case NPCID.Skeleton:
                    case NPCID.HeadacheSkeleton:
                    case NPCID.MisassembledSkeleton:
                    case NPCID.PantlessSkeleton:
                    case NPCID.SkeletonTopHat:
                    case NPCID.SkeletonAstonaut:
                    case NPCID.SkeletonAlien:
                    case NPCID.BoneThrowingSkeleton:
                    case NPCID.BoneThrowingSkeleton2:
                    case NPCID.BoneThrowingSkeleton3:
                    case NPCID.BoneThrowingSkeleton4:
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                Vector2 speed = new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-100, 1));
                                speed.Normalize();
                                speed *= Main.rand.NextFloat(3f, 6f);
                                speed.Y -= Math.Abs(speed.X) * 0.2f;
                                speed.Y -= 3f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.Center, speed, ProjectileID.SkeletonBone, npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                        break;

                    case NPCID.SolarSpearman:
                        int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                        if (t != -1 && Main.player[t].active && !Main.player[t].dead && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 velocity = Main.player[t].Center - npc.Center;
                            velocity.Normalize();
                            velocity *= 14f;
                            Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<DrakanianDaybreak>(), npc.damage / 4, 1f, Main.myPlayer);
                        }
                        Main.PlaySound(SoundID.Item1, npc.Center);
                        if (Main.rand.NextBool())
                        {
                            npc.Transform(NPCID.SolarSolenian);
                            return false;
                        }
                        break; //goto case NPCID.SolarSolenian;

                    /*case NPCID.SolarSolenian:
                    case NPCID.SolarCorite:
                    case NPCID.SolarCrawltipedeHead:
                    case NPCID.SolarCrawltipedeBody:
                    case NPCID.SolarCrawltipedeTail:
                    case NPCID.SolarDrakomire:
                    case NPCID.SolarDrakomireRider:
                    case NPCID.SolarSroller:
                        if (NPC.TowerActiveSolar && NPC.ShieldStrengthTowerSolar > 0)
                        {
                            int p = NPC.FindFirstNPC(NPCID.LunarTowerSolar);
                            if (p != -1 && Main.npc[p].active && npc.lastInteraction != -1 && Main.player[npc.lastInteraction].Distance(Main.npc[p].Center) < 5000)
                            {
                                break;
                            }
                            else //if pillar active, but out of range, dont contribute to shield
                            {
                                Main.PlaySound(npc.DeathSound, npc.Center);
                                return false;
                            }
                        }
                        break;

                    case NPCID.VortexHornet:
                    case NPCID.VortexHornetQueen:
                    case NPCID.VortexLarva:
                    case NPCID.VortexRifleman:
                    case NPCID.VortexSoldier:
                        if (NPC.TowerActiveVortex && NPC.ShieldStrengthTowerVortex > 0)
                        {
                            int p = NPC.FindFirstNPC(NPCID.LunarTowerVortex);
                            if (p != -1 && Main.npc[p].active && npc.lastInteraction != -1 && Main.player[npc.lastInteraction].Distance(Main.npc[p].Center) < 5000)
                            {
                                break;
                            }
                            else //if pillar active, but out of range, dont contribute to shield
                            {
                                Main.PlaySound(npc.DeathSound, npc.Center);
                                return false;
                            }
                        }
                        break;*/

                    case NPCID.NebulaBrain:
                        if (npc.HasValidTarget)
                        {
                            Player target = Main.player[npc.target];
                            Vector2 boltVel = target.Center - npc.Center;
                            boltVel.Normalize();
                            boltVel *= 4.5f;

                            for (int i = 0; i < (int)npc.localAI[2] / 60; i++)
                            {
                                Vector2 spawnPos = npc.position;
                                spawnPos.X += Main.rand.Next(npc.width);
                                spawnPos.Y += Main.rand.Next(npc.height);

                                Vector2 boltVel2 = boltVel.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 21)));
                                boltVel2 *= Main.rand.NextFloat(0.8f, 1.2f);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(spawnPos, boltVel2, ProjectileID.NebulaLaser, 48, 0f, Main.myPlayer);
                            }
                        }
                        break; //goto case NPCID.NebulaSoldier;

                    /*case NPCID.NebulaHeadcrab:
                    case NPCID.NebulaBeast:
                    case NPCID.NebulaSoldier:
                        if (NPC.TowerActiveNebula && NPC.ShieldStrengthTowerNebula > 0)
                        {
                            int p = NPC.FindFirstNPC(NPCID.LunarTowerNebula);
                            if (p != -1 && Main.npc[p].active && npc.lastInteraction != -1 && Main.player[npc.lastInteraction].Distance(Main.npc[p].Center) < 5000)
                            {
                                break;
                            }
                            else //if pillar active, but out of range, dont contribute to shield
                            {
                                Main.PlaySound(npc.DeathSound, npc.Center);
                                return false;
                            }
                        }
                        break;*/
                        
                    case NPCID.StardustJellyfishBig:
                    case NPCID.StardustSoldier:
                    case NPCID.StardustSpiderBig:
                    case NPCID.StardustWormHead:
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int p = NPC.FindFirstNPC(NPCID.LunarTowerStardust);
                            if (p != -1 && NPC.CountNPCS(NPCID.StardustCellSmall) < 10 && Main.npc[p].active && npc.Distance(Main.npc[p].Center) < 4000) //in tower range
                            {
                                for (int i = 0; i < 3; i++) //spawn stardust cells
                                {
                                    int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.StardustCellSmall);
                                    if (n < Main.maxNPCs)
                                    {
                                        Main.npc[n].velocity.X = Main.rand.Next(-10, 11);
                                        Main.npc[n].velocity.Y = Main.rand.Next(-10, 11);
                                        if (Main.netMode == NetmodeID.Server)
                                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                    }
                                }
                            }
                        }

                        /*if (NPC.TowerActiveStardust && NPC.ShieldStrengthTowerStardust > 0)
                        {
                            int p = NPC.FindFirstNPC(NPCID.LunarTowerStardust);
                            if (p != -1 && Main.npc[p].active && npc.lastInteraction != -1 && Main.player[npc.lastInteraction].Distance(Main.npc[p].Center) < 5000)
                            {
                                break;
                            }
                            else //if pillar active, but out of range, dont contribute to shield
                            {
                                Main.PlaySound(npc.DeathSound, npc.Center);
                                return false;
                            }
                        }*/
                        break;

                    case NPCID.SmallSlimedZombie:
                    case NPCID.SlimedZombie:
                    case NPCID.BigSlimedZombie:
                    case NPCID.ArmedZombieSlimed:
                        if (Main.rand.NextBool() && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.BlueSlime);
                            if (n != 200 && Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                        break;

                    default:
                        break;
                }
            }

            return true;
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (FargoSoulsWorld.EternityMode && BeetleOffenseAura)
            {
                damage = (int)(damage * 1.25f);
            }
        }

        private void ModifyHitByEither(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.LunarTowerNebula:
                    case NPCID.LunarTowerSolar:
                    case NPCID.LunarTowerStardust:
                    case NPCID.LunarTowerVortex:
                        if (npc.Distance(player.Center) > 2500)
                            damage = 0;
                        else
                            damage /= 2;
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
                        if (!masoBool[0])
                        {
                            masoBool[0] = true;
                            npc.Opacity *= 5;
                        }
                        break;

                    case NPCID.MisterStabby:
                    case NPCID.AnglerFish:
                        if (!masoBool[0])
                        {
                            masoBool[0] = true;
                            npc.Opacity *= 5;
                        }
                        break;

                    case NPCID.BoneLee:
                        if (Main.rand.NextBool(10) && npc.HasPlayerTarget && player.whoAmI == npc.target && player.active && !player.dead && !player.ghost)
                        {
                            Vector2 teleportTarget = player.Center;
                            float offset = 100f * -player.direction;
                            teleportTarget.X += offset;
                            teleportTarget.Y -= 50f;
                            if (!Collision.CanHit(teleportTarget, 1, 1, player.position, player.width, player.height))
                            {
                                teleportTarget.X -= offset * 2f;
                                if (!Collision.CanHit(teleportTarget, 1, 1, player.position, player.width, player.height))
                                    break;
                            }
                            FargoSoulsUtil.GrossVanillaDodgeDust(npc);
                            npc.Center = teleportTarget;
                            npc.netUpdate = true;
                            FargoSoulsUtil.GrossVanillaDodgeDust(npc);
                        }
                        break;

                    case NPCID.ForceBubble:
                        if (Main.rand.NextBool(3) && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 velocity = player.Center - npc.Center;
                            velocity.Normalize();
                            velocity *= 10f;
                            int Damage = Main.expertMode ? 28 : 35;
                            Projectile.NewProjectile(npc.Center, velocity, ProjectileID.MartianTurretBolt, Damage, 0f, Main.myPlayer);
                        }
                        break;

                    case NPCID.IceTortoise:
                        float reduction = (float)npc.life / npc.lifeMax;
                        if (reduction < 0.5f)
                            reduction = 0.5f;
                        damage = (int)(damage * reduction);
                        break;

                    case NPCID.DungeonGuardian:
                        damage = 1;
                        break;

                    case NPCID.Psycho:
                        Counter[0] = 0;
                        break;

                    case NPCID.Nymph:
                        if (player.loveStruck)
                        {
                            /*npc.life += damage;
                            if (npc.life > npc.lifeMax)
                                npc.life = npc.lifeMax;
                            CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage);*/

                            Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                            float ai1 = 30 + Main.rand.Next(30);
                            Projectile.NewProjectile(player.Center, speed, ModContent.ProjectileType<HostileHealingHeart>(), damage, 0f, Main.myPlayer, npc.whoAmI, ai1);

                            damage = 0;
                            npc.netUpdate = true;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            ModifyHitByEither(npc, player, ref damage, ref knockback, ref crit);

            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.GiantTortoise:
                        player.Hurt(PlayerDeathReason.ByCustomReason(player.name + Language.GetTextValue("Mods.FargowiltasSouls.GlobalNPC.KilledbyGiantTortoise")), damage / 2, 0);
                        break;

                    default:
                        break;
                }

                if (npc.catchItem != 0 && npc.lifeMax == 5)
                    player.AddBuff(ModContent.BuffType<Guilty>(), 300);
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];

            ModifyHitByEither(npc, player, ref damage, ref knockback, ref crit);

            if (FargoSoulsWorld.EternityMode)
            {
                if (npc.catchItem != 0 && npc.lifeMax == 5 && projectile.friendly && !projectile.hostile && projectile.type != ProjectileID.FallingStar)
                    player.AddBuff(ModContent.BuffType<Guilty>(), 300);
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            /*if (npc.friendly)
            {
                if (FargoSoulsUtil.BossIsAlive(ref deviBoss, ModContent.NPCType<DeviBoss.DeviBoss>()) && npc.Distance(Main.npc[deviBoss].Center) < 2000)
                    damage *= 8;
            }*/

            if (FargoSoulsWorld.EternityMode)
            {
                if (npc.friendly && npc.type == NPCID.DD2EterniaCrystal && Counter[0] > 0)
                    damage = 1;

                if (BeetleDefenseAura)
                    damage *= 0.75;

                if (PaladinsShield)
                    damage /= 2;
            }

            //normal damage calc
            return true;
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.GiantShelly:
                    case NPCID.GiantShelly2:
                        if (npc.ai[0] == 3f)
                        {
                            Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 10;
                            int p = Projectile.NewProjectile(npc.Center, velocity, ProjectileID.Stinger, npc.damage / 4, 1, Main.myPlayer);
                            FargoGlobalProjectile.SplitProj(Main.projectile[p], 5, MathHelper.Pi / 5, 1);

                            //FargoSoulsUtil.XWay(6, npc.Center, ProjectileID.Stinger, 3, npc.damage / 4, 1);
                        }

                        break;
                }
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (npc.type)
                {
                    case NPCID.Zombie:
                    case NPCID.BaldZombie:
                    case NPCID.FemaleZombie:
                    case NPCID.PincushionZombie:
                    case NPCID.SlimedZombie:
                    case NPCID.TwiggyZombie:
                    case NPCID.ZombiePixie:
                    case NPCID.ZombieRaincoat:
                    case NPCID.ZombieSuperman:
                    case NPCID.ZombieSweater:
                    case NPCID.ZombieXmas:
                    case NPCID.SwampZombie:
                    case NPCID.SmallSwampZombie:
                    case NPCID.BigSwampZombie:
                    case NPCID.ZombieDoctor:
                    case NPCID.BloodZombie:
                        if (target.townNPC && target.life < damage)
                        {
                            target.Transform(npc.type);
                        }
                        break;
                }
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            /*for (int i = 0; i < auraDebuffs.Count; i++)
            {
                Texture2D buffIcon = Main.buffTexture[auraDebuffs[i]];
                Color buffColor = drawColor * 0.5f;
                Vector2 drawPos = npc.Top;
                drawPos.Y -= 32f;
                float mid = auraDebuffs.Count / 2f - 0.5f;
                drawPos.X -= 32f * (i - mid);
                Main.spriteBatch.Draw(buffIcon, drawPos - Main.screenPosition + new Vector2(0f, npc.gfxOffY), buffIcon.Bounds, buffColor, 0, buffIcon.Bounds.Size() / 2, 1f, SpriteEffects.None, 0f);
            }*/
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (FargoSoulsWorld.EternityMode && npc.type == NPCID.Nurse && firstButton)
            {
                if (Main.player[Main.myPlayer].HasBuff(ModContent.BuffType<Recovering>()))
                    return false;
                else if (FargoSoulsUtil.AnyBossAlive())
                    Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Recovering>(), 7200);
            }
            return true;
        }

        public bool StealFromInventory(Player target, ref Item item)
        {
            if (target.GetModPlayer<FargoPlayer>().StealingCooldown <= 0 && !item.IsAir)
            {
                target.GetModPlayer<FargoPlayer>().StealingCooldown = 360; //trust me, keep these separate
                target.AddBuff(ModContent.BuffType<ThiefCD>(), 360);

                int i = Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, item.type, item.stack, false, -1, false, false);
                Vector2 position = Main.item[i].position;

                Main.item[i] = item.Clone();
                Main.item[i].whoAmI = i;
                Main.item[i].position = position;
                Main.item[i].stack = item.stack;

                Main.item[i].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
                Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                Main.item[i].noGrabDelay = 100;
                Main.item[i].newAndShiny = false;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 1f);

                item.TurnToAir();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void NetUpdateMaso(int npc) //MAKE SURE THAT YOU CALL THIS FROM THE GLOBALNPC INSTANCE OF THE NPC ITSELF
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            var netMessage = mod.GetPacket();
            netMessage.Write((byte)2);
            netMessage.Write((byte)npc);
            netMessage.Write(masoBool[0]); //these are the variables of the instance THAT CALLS THIS METHOD
            netMessage.Write(masoBool[1]); //rule of thumb is to only call this method server-side
            netMessage.Write(masoBool[2]);
            netMessage.Write(masoBool[3]);
            netMessage.Write(Counter[0]);
            netMessage.Write(Counter[1]);
            netMessage.Write(Counter[2]);
            netMessage.Write(Counter[3]);
            netMessage.Send();
        }

        public static void Horde(NPC npc, int size)
        {
            for (int i = 0; i < size; i++)
            {
                Vector2 pos = new Vector2(npc.Center.X + Main.rand.NextFloat(-2f, 2f) * npc.width, npc.Center.Y);
                if (!Collision.SolidCollision(pos, npc.width, npc.height) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int j = NPC.NewNPC((int)pos.X + npc.width / 2, (int)pos.Y + npc.height / 2, npc.type);
                    if (j != Main.maxNPCs)
                    {
                        NPC newNPC = Main.npc[j];
                        newNPC.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 5f;
                        newNPC.GetGlobalNPC<EModeGlobalNPC>().FirstTick = true;
                        newNPC.GetGlobalNPC<NewEModeGlobalNPC>().FirstTick = false;
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

        public static void Aura(NPC npc, float distance, bool reverse = false, int dustid = DustID.GoldFlame, Color color = default, params int[] buffs)
        {
            Player p = Main.player[Main.myPlayer];

            if (Fargowiltas.Instance.MasomodeEXLoaded)
                distance *= reverse ? 0.5f : 2f;

            const int baseDistance = 500;
            const int baseMax = 20;
            
            int dustMax = (int)(distance / baseDistance * baseMax);
            if (dustMax < 10)
                dustMax = 10;
            if (dustMax > 40)
                dustMax = 40;

            float dustScale = distance / baseDistance;
            if (dustScale < 0.75f)
                dustScale = 0.75f;
            if (dustScale > 2f)
                dustScale = 2f;

            for (int i = 0; i < dustMax; i++)
            {
                Vector2 spawnPos = npc.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                Vector2 offset = spawnPos - p.Center;
                if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) //dont spawn dust if its pointless
                    continue;
                Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, dustid, 0, 0, 100, Color.White, dustScale)];
                dust.velocity = npc.velocity;
                if (Main.rand.NextBool(3))
                {
                    dust.velocity += Vector2.Normalize(npc.Center - dust.position) * Main.rand.NextFloat(5f) * (reverse ? -1f : 1f);
                    dust.position += dust.velocity * 5f;
                }
                dust.noGravity = true;
                if (color != default)
                    dust.color = color;
            }

            if (buffs.Length == 0 || buffs[0] < 0)
                return;

            //works because buffs are client side anyway :ech:
            float range = npc.Distance(p.Center);
            if (p.active && !p.dead && !p.ghost && (reverse ? (range > distance && range < Math.Max(3000f, distance * 2)) : range < distance))
            {
                foreach (int buff in buffs)
                {
                    p.AddBuff(buff, BuffLoader.LongerExpertDebuff(buff) && Main.expertMode && Main.expertDebuffTime >= 2 ? 1 : 2);
                }
            }
        }

        private void Shoot(NPC npc, int delay, float distance, int speed, int proj, int dmg, float kb, bool hostile = false, int dustID = -1)
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
        }

        private void CustomReflect(NPC npc, int dustID, int ratio = 1)
        {
            float distance = 2f * 16;

            Main.projectile.Where(x => x.active && x.friendly && !FargoSoulsUtil.IsMinionDamage(x, false)).ToList().ForEach(x =>
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
