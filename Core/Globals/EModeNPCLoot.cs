using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Pets;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Globals
{
    public partial class EModeNPCLoot : GlobalNPC
    {
        #region NPC Lists
        static List<int> EvilCritters = new List<int>
            {
                NPCID.CorruptBunny,
                NPCID.CrimsonBunny,
                NPCID.CorruptGoldfish,
                NPCID.CrimsonGoldfish,
                NPCID.CorruptPenguin,
                NPCID.CrimsonPenguin
            };
        static List<int> Mimics = new List<int>
            {
                NPCID.Mimic,
                NPCID.PresentMimic,
                NPCID.IceMimic,
                NPCID.BigMimicCorruption,
                NPCID.BigMimicCrimson,
                NPCID.BigMimicHallow,
                NPCID.BigMimicJungle
            };
        static List<int> HardmodeDesertEnemies = new List<int>
            {
                NPCID.DesertBeast,
                NPCID.DesertScorpionWalk,
                NPCID.DesertScorpionWall,
                NPCID.DesertLamiaDark,
                NPCID.DesertLamiaLight,
                NPCID.DesertGhoul,
                NPCID.DesertGhoulCorruption,
                NPCID.DesertGhoulCrimson,
                NPCID.DesertGhoulHallow,
                NPCID.DesertDjinn
            };
        static List<int> EarlyBirdEnemies = new List<int>
        {
            NPCID.WyvernHead,
            NPCID.WyvernBody,
            NPCID.WyvernBody2,
            NPCID.WyvernBody3,
            NPCID.WyvernLegs,
            NPCID.WyvernTail,
            NPCID.Mimic,
            NPCID.IceMimic,
            NPCID.Medusa,
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,
            NPCID.PigronHallow,
            NPCID.IchorSticker,
            NPCID.SeekerHead,
            NPCID.AngryNimbus,
            NPCID.RedDevil,
            NPCID.MushiLadybug,
            NPCID.AnomuraFungus,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.IceGolem,
            NPCID.SandElemental
        };
        static List<int> Hornets = new List<int>
        {
            NPCID.Hornet,
            NPCID.HornetFatty,
            NPCID.HornetHoney,
            NPCID.HornetLeafy,
            NPCID.HornetSpikey,
            NPCID.HornetStingy,
            NPCID.BigHornetFatty,
            NPCID.BigHornetHoney,
            NPCID.BigHornetLeafy,
            NPCID.BigHornetSpikey,
            NPCID.BigHornetStingy,
            NPCID.BigMossHornet,
            NPCID.GiantMossHornet,
            NPCID.LittleHornetFatty,
            NPCID.LittleHornetHoney,
            NPCID.LittleHornetLeafy,
            NPCID.LittleHornetSpikey,
            NPCID.LittleHornetStingy,
            NPCID.LittleMossHornet,
            NPCID.MossHornet,
            NPCID.TinyMossHornet
        };
        static List<int> MushroomEnemies = new List<int>
        {
            NPCID.FungiBulb,
            NPCID.GiantFungiBulb,
            NPCID.AnomuraFungus,
            NPCID.MushiLadybug,
            NPCID.SporeBat,
            NPCID.ZombieMushroom,
            NPCID.ZombieMushroomHat,
            NPCID.SporeSkeleton,
            NPCID.FungoFish
        };
        #endregion
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LeadingConditionRule emodeRule = new(new EModeDropCondition());
            switch (npc.type)
            {
                #region Bosses
                case NPCID.EyeofCthulhu:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<AgitatingLens>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrate, 5));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.FallenStar, 5));
                    }
                    break;
                case NPCID.DD2Betsy:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<BetsysHeart>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.GoldenCrateHard, 5));
                    }
                    break;
                case NPCID.BrainofCthulhu:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GuttedHeart>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.CrimsonFishingCrate, 5));

                        //to make up for no loot until dead
                        emodeRule.OnSuccess(ItemDropRule.Common(ItemID.TissueSample, 1, 60, 60));
                        emodeRule.OnSuccess(ItemDropRule.Common(ItemID.CrimtaneOre, 1, 200, 200));
                    }
                    break;
                case NPCID.Deerclops:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<Deerclawps>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<DeerSinew>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.FrozenCrate, 5));
                    }
                    break;
                case NPCID.TheDestroyer:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GroundStick>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
                    }
                    break;
                case NPCID.DukeFishron:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantAntibodies>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantsCreditCard>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.OceanCrateHard, 5));
                        emodeRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                            ItemID.FuzzyCarrot,
                            ItemID.AnglerHat,
                            ItemID.AnglerVest,
                            ItemID.AnglerPants,
                            ItemID.GoldenFishingRod,
                            ItemID.GoldenBugNet,
                            ItemID.FishHook,
                            ItemID.HighTestFishingLine,
                            ItemID.AnglerEarring,
                            ItemID.TackleBox,
                            ItemID.FishermansGuide,
                            ItemID.WeatherRadio,
                            ItemID.Sextant,
                            ItemID.FinWings,
                            ItemID.BottomlessBucket,
                            ItemID.SuperAbsorbantSponge,
                            ItemID.HotlineFishingHook,
                            ItemID.PirateMap
                        ));
                    }
                    break;
                case NPCID.DungeonGuardian:
                    {
                        emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SinisterIcon>()));
                    }
                    break;
                case NPCID.EaterofWorldsBody or NPCID.EaterofWorldsHead or NPCID.EaterofWorldsTail: //just to be sure
                    {
                        LeadingConditionRule lastEater = new(new Conditions.LegacyHack_IsABoss());
                        emodeRule.OnSuccess(lastEater);
                        lastEater.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<DarkenedHeart>()));
                        lastEater.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.CorruptFishingCrate, 5));

                        //to make up for no loot until dead
                        lastEater.OnSuccess(ItemDropRule.Common(ItemID.ShadowScale, 1, 60, 60));
                        lastEater.OnSuccess(ItemDropRule.Common(ItemID.DemoniteOre, 1, 200, 200));
                    }
                    break;
                case NPCID.HallowBoss:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PrecisionSeal>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
                    }
                    break;
                case NPCID.Golem:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<LihzahrdTreasureBox>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.GoldenCrateHard, 5));
                    }
                    break;
                case NPCID.IceQueen:
                    {
                        emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<IceQueensCrown>(), 5));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Present, 1, 1, 5));
                    }
                    break;
                case NPCID.KingSlime:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<SlimyShield>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.WoodenCrate, 5));
                    }
                    break;
                case NPCID.CultistBoss:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<CelestialRune>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantsPact>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.DungeonFishingCrateHard, 5));
                    }
                    break;
                case NPCID.MartianSaucer:
                case NPCID.MartianSaucerCore:
                    {
                        emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SaucerControlConsole>(), 5));
                    }
                    break;
                case NPCID.MoonLordCore:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GalacticGlobe>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LunarOre, 150));
                    }
                    break;
                case NPCID.Plantera:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MagicalBulb>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.JungleFishingCrateHard, 5));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LifeFruit, 3));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.ChlorophyteOre, 200));
                    }
                    break;
                case NPCID.Pumpking:
                    {
                        emodeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PumpkingsCape>(), 5));
                        emodeRule.OnSuccess(ItemDropRule.Common(ItemID.GoodieBag, 1, 1, 5));
                        emodeRule.OnSuccess(ItemDropRule.Common(ItemID.BloodyMachete, 10));
                    }
                    break;
                case NPCID.QueenBee:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<QueenStinger>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.JungleFishingCrate, 5));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HerbBag, 5));
                    }
                    break;
                case NPCID.QueenSlimeBoss:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GelicWings>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
                    }
                    break;
                case NPCID.SkeletronHead:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<NecromanticBrew>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.DungeonFishingCrate, 5));
                    }
                    break;
                case NPCID.SkeletronPrime:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<ReinforcedPlating>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
                    }
                    break;
                case NPCID.Retinazer or NPCID.Spazmatism:
                    {
                        LeadingConditionRule noTwin = new(new Conditions.MissingTwin());
                        emodeRule.OnSuccess(noTwin);
                        noTwin.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<FusedLens>()));
                        noTwin.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrateHard, 5));
                    }
                    break;
                case NPCID.WallofFlesh:
                    {
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PungentEyeball>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<MutantsDiscountCard>()));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
                        emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LavaCrateHard, 5));
                    }
                    break;
                #endregion
                #region Normal Enemies
                case NPCID.CaveBat:
                case NPCID.GiantBat:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<RabiesShot>(), 5));
                    break;
                case NPCID.Clown:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.PartyGirlGrenade, 1, 1, 10));
                    break;
                case NPCID.BloodNautilus:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<DreadShell>(), 5));
                    break;
                case var _ when EvilCritters.Contains(npc.type):
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<SqueakyToy>(), 10));
                    break;
                case NPCID.EyeballFlyingFish or NPCID.ZombieMerman:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FrogLeg, 10));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.BalloonPufferfish, 10));
                    break;
                case NPCID.GiantWormHead or NPCID.DiggerHead:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.WormTooth, 1, 3, 9));
                    break;
                case var _ when Mimics.Contains(npc.type):
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GoldenCrate));
                    switch (npc.type)
                    {
                        case NPCID.BigMimicCorruption:
                            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CorruptFishingCrateHard));
                            break;
                        case NPCID.BigMimicCrimson:
                            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CrimsonFishingCrateHard));
                            break;
                        case NPCID.BigMimicHallow:
                            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.HallowedFishingCrateHard));
                            break;
                        case NPCID.BigMimicJungle:
                            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.JungleFishingCrateHard));
                            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<TribalCharm>(), 5));
                            break;
                    }
                    break;
                case NPCID.LostGirl or NPCID.Nymph:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<NymphsPerfume>(), 5));
                    break;
                case NPCID.RuneWizard:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<MysticSkull>(), 5));
                    break;
                case NPCID.BlackRecluse or NPCID.BlackRecluseWall:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.SpiderEgg, 50));
                    break;
                case NPCID.Tim:
                    npcLoot.Add(ItemDropRule.ByCondition(new EModeDropCondition(), ModContent.ItemType<TimsConcoction>(), 5));
                    break;
                case NPCID.WalkingAntlion:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new DownedEvilBossDropCondition(), ItemID.FastClock, 50));
                    break;
                case NPCID.DuneSplicerBody or NPCID.DuneSplicerHead or NPCID.DuneSplicerTail:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrate));
                    break;
                case var _ when HardmodeDesertEnemies.Contains(npc.type):
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.DesertFossil, 3, 1, 10));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FlyingCarpet, 100));
                    if (npc.type == NPCID.DesertBeast)
                    {
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.PocketMirror, 50));
                    }
                    break;
                #endregion
                case NPCID.SandElemental:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.SandstorminaBottle, 20));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrate));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.OasisCrateHard));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SandsofTime>(), 5));

                    FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsPreHardmode(), ModContent.ItemType<SandsofTime>()));
                    break;
                case NPCID.DarkCaster:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.WaterBolt, 50));
                    break;
                case NPCID.Everscream:
                case NPCID.SantaNK1:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Present, 1, 1, 5));
                    break;
                case NPCID.GoblinSummoner:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<WretchedPouch>(), 5));
                    break;
                case NPCID.Pixie:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.EmpressButterfly, 100));
                    break;
                case NPCID.RainbowSlime:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<ConcentratedRainbowMatter>(), 10));
                    break;
                case NPCID.LavaSlime:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.LavaCharm, 100));
                    break;
                case NPCID.Derpling:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.TrifoldMap, 50));
                    break;
                case NPCID.DoctorBones:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<SkullCharm>(), 10));
                    break;
                case var _ when Hornets.Contains(npc.type):
                    if (npc.type == NPCID.MossHornet)
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Stinger, 2));

                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.JungleGrassSeeds, 10));
                    break;
                case NPCID.Piranha:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.AdhesiveBandage, 50));
                    break;
                case NPCID.AngryTrapper:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Vine, 2));
                    break;
                case NPCID.BrainScrambler:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.BrainScrambler, 100));
                    break;
                case NPCID.CultistArcherWhite:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.Find<ModItem>("Fargowiltas", "CultistSummon").Type, 100));
                    break;
                case var _ when MushroomEnemies.Contains(npc.type):
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GlowingMushroom, 1, 1, 5));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.MushroomGrassSeeds, 5));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.TruffleWorm, 20));
                    break;
                case NPCID.Crab:
                case NPCID.SeaSnail:
                case NPCID.Squid:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Starfish, 10, 1, 3));
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Seashell, 10, 1, 3));
                    break;
                case NPCID.PirateShipCannon:
                    {
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<SecurityWallet>(), 5));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CoinGun, 50));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.LuckyCoin, 50));
                    }
                    break;
                case NPCID.PirateCaptain:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.Find<ModItem>("Fargowiltas", "GoldenDippingVat").Type, 15));
                    break;
                case NPCID.MourningWood:
                    {
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.GoodieBag, 1, 1, 5));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.BloodyMachete, 10));
                    }
                    break;
                case NPCID.WyvernHead:
                    {
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.CloudinaBottle, 20));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FloatingIslandFishingCrate));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.FloatingIslandFishingCrateHard));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<WyvernFeather>(), 5));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<WyvernFeather>()));
                    }
                    break;
                case NPCID.IceGolem:
                    {
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.BlizzardinaBottle, 20));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FrozenCrate));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.FrozenCrateHard));
                        FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<FrigidGemstone>(), 5));

                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<FrigidGemstone>()));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.SnowGlobe));
                    }
                    break;
                case NPCID.MisterStabby:
                case NPCID.SnowBalla:
                case NPCID.SnowmanGangsta:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<OrdinaryCarrot>(), 50));
                    break;
                case NPCID.Shark:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<HokeyBall>(), 100));
                    break;
                case NPCID.SporeBat:
                    FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Shroomerang, 10));
                    break;

            }
            #region early bird
            if (EarlyBirdEnemies.Contains(npc.type))
            {
                switch (npc.type)
                {
                    case NPCID.Medusa:
                        npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.MedusaHead && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        break;

                    case NPCID.WyvernHead:
                        npcLoot.RemoveWhere(rule => rule is DropBasedOnExpertMode drop && drop.ruleForNormalMode is CommonDrop drop2 && drop2.itemId == ItemID.SoulofFlight && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        break;

                    case NPCID.PigronHallow:
                    case NPCID.PigronCorruption:
                    case NPCID.PigronCrimson:
                        npcLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.condition is Conditions.DontStarveIsUp && drop.itemId == ItemID.HamBat && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        npcLoot.RemoveWhere(rule => rule is ItemDropWithConditionRule drop && drop.condition is Conditions.DontStarveIsNotUp && drop.itemId == ItemID.HamBat && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        break;

                    case NPCID.RedDevil:
                        npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.UnholyTrident && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.DemonScythe, 3));
                        break;

                    case NPCID.IchorSticker:
                        npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.Ichor && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TheUndertaker, ItemID.TheRottedFork, ItemID.CrimsonRod, ItemID.CrimsonHeart, ItemID.PanicNecklace));
                        break;

                    case NPCID.SeekerHead:
                        npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.CursedFlame && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.BallOHurt, ItemID.BandofStarpower, ItemID.Musket, ItemID.ShadowOrb, ItemID.Vilethorn));
                        break;

                    case NPCID.Mimic:
                        npcLoot.RemoveWhere(rule => rule is OneFromOptionsDropRule drop && drop.dropIds.Contains(ItemID.DualHook) && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook));
                        break;

                    case NPCID.IceMimic:
                        npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.ToySled && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.OneFromOptions(1, ItemID.TitanGlove, ItemID.PhilosophersStone, ItemID.CrossNecklace, ItemID.DualHook));
                        break;

                    case NPCID.AngryNimbus:
                        npcLoot.RemoveWhere(rule => rule is CommonDrop drop && drop.itemId == ItemID.NimbusRod && FargoSoulsUtil.LockEarlyBirdDrop(npcLoot, rule));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.FloatingIslandFishingCrate));
                        break;

                    case NPCID.DuneSplicerHead:
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.SandstorminaBottle, 3));
                        FargoSoulsUtil.AddEarlyBirdDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrate));
                        break;

                    default: break;
                }

            }
            #endregion
            npcLoot.Add(emodeRule);
        }
    }
    public class EModeFirstKillDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            IItemDropRule rule = null;

            if (npc.type == ModContent.NPCType<TrojanSquirrel>())
            {
                rule = FirstKillDrop(2, ItemID.LifeCrystal);
            }

            switch (npc.type)
            {
                case NPCID.KingSlime:
                    rule = FirstKillDrop(2, ItemID.LifeCrystal);
                    break;
                case NPCID.EyeofCthulhu:
                    rule = FirstKillDrop(3, ItemID.LifeCrystal);
                    break;
            }

            if (rule is not null)
                npcLoot.Add(rule);
        }

        /*
        public override void OnKill(NPC npc)
        {
            switch (npc.type)
            {
                //Set the flags here instead of ModifyNPCLoot in order to let loot happen properly
                case NPCID.KingSlime:
                    NPC.SetEventFlagCleared(ref StorageWorld.kingSlimeDiamond, -1);
                    break;
                case NPCID.EyeofCthulhu:
                    NPC.SetEventFlagCleared(ref StorageWorld.boss1Diamond, -1);
                    break;
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    if (npc.boss)
                        NPC.SetEventFlagCleared(ref StorageWorld.boss2Diamond, -1);
                    break;
                case NPCID.BrainofCthulhu:
                    NPC.SetEventFlagCleared(ref StorageWorld.boss2Diamond, -1);
                    break;
                case NPCID.SkeletronHead:
                    NPC.SetEventFlagCleared(ref StorageWorld.boss3Diamond, -1);
                    break;
                case NPCID.QueenBee:
                    NPC.SetEventFlagCleared(ref StorageWorld.queenBeeDiamond, -1);
                    break;
                case NPCID.WallofFlesh:
                    NPC.SetEventFlagCleared(ref StorageWorld.hardmodeDiamond, -1);
                    break;
                case NPCID.TheDestroyer:
                    NPC.SetEventFlagCleared(ref StorageWorld.mechBoss1Diamond, -1);
                    break;
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    NPC.SetEventFlagCleared(ref StorageWorld.mechBoss2Diamond, -1);
                    break;
                case NPCID.SkeletronPrime:
                    NPC.SetEventFlagCleared(ref StorageWorld.mechBoss3Diamond, -1);
                    break;
                case NPCID.Plantera:
                    NPC.SetEventFlagCleared(ref StorageWorld.plantBossDiamond, -1);
                    break;
                case NPCID.Golem:
                    NPC.SetEventFlagCleared(ref StorageWorld.golemBossDiamond, -1);
                    break;
                case NPCID.DukeFishron:
                    NPC.SetEventFlagCleared(ref StorageWorld.fishronDiamond, -1);
                    break;
                case NPCID.CultistBoss:
                    NPC.SetEventFlagCleared(ref StorageWorld.ancientCultistDiamond, -1);
                    break;
                case NPCID.MoonLordCore:
                    NPC.SetEventFlagCleared(ref StorageWorld.moonlordDiamond, -1);
                    break;
                case NPCID.QueenSlimeBoss:
                    NPC.SetEventFlagCleared(ref StorageWorld.queenSlimeDiamond, -1);
                    break;
                case NPCID.HallowBoss:
                    NPC.SetEventFlagCleared(ref StorageWorld.empressDiamond, -1);
                    break;
            }
        }
        */

        private static IItemDropRule Drop(int count, int itemID) => ItemDropRule.Common(itemID, minimumDropped: count, maximumDropped: count);

        public static IItemDropRule FirstKillDrop(int amount, int itemID)
        {
            IItemDropRule rule = new LeadingConditionRule(new FirstKillCondition());
            rule.OnSuccess(Drop(amount, itemID));
            return rule;
        }
    }

    internal class FirstKillCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) =>
            !info.IsInSimulation &&
            WorldSavingSystem.EternityMode &&
            info.npc.type switch
            {
                NPCID.KingSlime => !NPC.downedSlimeKing,
                NPCID.EyeofCthulhu => !NPC.downedBoss1,
                _ => info.npc.type == ModContent.NPCType<TrojanSquirrel>() && !WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel] //needed outside switch because modded npctype not constant
            };

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.FirstKill");
    }
}
