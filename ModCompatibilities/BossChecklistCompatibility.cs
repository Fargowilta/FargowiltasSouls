using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.Items.Summons;
using FargowiltasSouls.Items.Tiles;
using FargowiltasSouls.Items.Weapons.BossDrops;
using FargowiltasSouls.NPCs.AbomBoss;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.NPCs.DeviBoss;
using FargowiltasSouls.NPCs.MutantBoss;
using FargowiltasSouls.Patreon.Catsounds;
using FargowiltasSouls.Patreon.Daawnz;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.ModCompatibilities
{
    public class BossChecklistCompatibility : ModCompatibility
    {
        public BossChecklistCompatibility(Mod callerMod) : base(callerMod, "BossChecklist")
        {
        }

        public void Initialize()
        {
            if (ModInstance != null)
            {
                InitializeBosses();
                InitializeMinibosses();
                InitializeBossLoot();
            }
        }

        private void InitializeBosses()
        {
            Mod musicMod = ModLoader.GetMod("FargowiltasMusic");

            List<int> deviCollection = new List<int>
            {
                ModContent.ItemType<DeviTrophy>()
            };

            List<int> abomCollection = new List<int>
            {
                ModContent.ItemType<AbomTrophy>()
            };

            List<int> mutantCollection = new List<int>
            {
                ModContent.ItemType<MutantTrophy>()
            };

            if (musicMod != null)
            {
                deviCollection.Add(musicMod.ItemType("DeviMusicBox"));
                abomCollection.Add(musicMod.ItemType("AbomMusicBox"));
                mutantCollection.Add(musicMod.ItemType("MutantMusicBox"));
            }

            AddBoss(ModContent.NPCType<DeviBoss>(),
                5.01f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Deviantt.Name"),
                () => FargoSoulsWorld.downedDevi,
                ModContent.ItemType<DevisCurse>(),
                deviCollection,
                ModContent.ItemType<DeviatingEnergy>(),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spawn1") + $"[i:{ModContent.ItemType<DevisCurse>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Deviantt.Despawn"),
                "FargowiltasSouls/NPCs/DeviBoss/DeviBoss_Still",
                "FargowiltasSouls/NPCs/DeviBoss/DeviBoss_Head_Boss");

            AddBoss(ModContent.NPCType<AbomBoss>(),
                14.01f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Abom.Name"),
                () => FargoSoulsWorld.downedAbom,
                ModContent.ItemType<AbomsCurse>(),
                abomCollection,
                ModContent.ItemType<AbomEnergy>(),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spawn1") + $"[i:{ModContent.ItemType<AbomsCurse>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Abom.Despawn"),
                "FargowiltasSouls/NPCs/DeviBoss/AbomBoss_Still",
                "FargowiltasSouls/NPCs/DeviBoss/AbomBoss_Head_Boss");

            AddBoss(ModContent.NPCType<MutantBoss>(),
                ModLoader.GetMod("CalamityMod") != null ? 20f : 14.03f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Mutant.Name"),
                () => FargoSoulsWorld.downedMutant,
                ModContent.ItemType<AbominationnVoodooDoll>(),
                mutantCollection,
                ModContent.ItemType<Sadism>(),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Mutant.Spawn1") +$"[i:{ModContent.ItemType<AbominationnVoodooDoll>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Mutant.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Mutant.Despawn"),
                "FargowiltasSouls/NPCs/DeviBoss/MutantBoss_Still",
                "FargowiltasSouls/NPCs/DeviBoss/MutantBoss_Head_Boss");
        }

        private void InitializeMinibosses()
        {
            AddMiniboss(ModContent.NPCType<TimberChampion>(),
                14.0001f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Timber.Name"),
                () => FargoSoulsWorld.downedChampions[0],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<WoodEnchant>(),
                    ModContent.ItemType<BorealWoodEnchant>(),
                    ModContent.ItemType<RichMahoganyEnchant>(),
                    ModContent.ItemType<EbonwoodEnchant>(),
                    ModContent.ItemType<ShadewoodEnchant>(),
                    ModContent.ItemType<PalmWoodEnchant>(),
                    ModContent.ItemType<PearlwoodEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Timber.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Timber.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Timber.Despawn"),
                "FargowiltasSouls/NPCs/Champions/TimberChampion_Still",
                "FargowiltasSouls/NPCs/Champions/TimberChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<TerraChampion>(),
                14.001f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Terra.Name"),
                () => FargoSoulsWorld.downedChampions[1],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<CopperEnchant>(),
                    ModContent.ItemType<TinEnchant>(),
                    ModContent.ItemType<IronEnchant>(),
                    ModContent.ItemType<LeadEnchant>(),
                    ModContent.ItemType<TungstenEnchant>(),
                    ModContent.ItemType<ObsidianEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Terra.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Terra.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Terra.Despawn"),
                "FargowiltasSouls/NPCs/Champions/TerraChampion_Still",
                "FargowiltasSouls/NPCs/Champions/TerraChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<EarthChampion>(),
                14.002f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Earth.Name"),
                () => FargoSoulsWorld.downedChampions[2],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<CobaltEnchant>(),
                    ModContent.ItemType<PalladiumEnchant>(),
                    ModContent.ItemType<MythrilEnchant>(),
                    ModContent.ItemType<OrichalcumEnchant>(),
                    ModContent.ItemType<AdamantiteEnchant>(),
                    ModContent.ItemType<TitaniumEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Earth.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Earth.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Earth.Despawn"),
                "FargowiltasSouls/NPCs/Champions/EarthChampion_Still",
                "FargowiltasSouls/NPCs/Champions/EarthChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<NatureChampion>(),
                14.003f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Nature.Name"),
                () => FargoSoulsWorld.downedChampions[3],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<CrimsonEnchant>(),
                    ModContent.ItemType<MoltenEnchant>(),
                    ModContent.ItemType<RainEnchant>(),
                    ModContent.ItemType<FrostEnchant>(),
                    ModContent.ItemType<ChlorophyteEnchant>(),
                    ModContent.ItemType<ShroomiteEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Nature.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Nature.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Nature.Despawn"),
               "FargowiltasSouls/NPCs/Champions/NatureChampion_Still",
                "FargowiltasSouls/NPCs/Champions/NatureChampionHead_Head_Boss");

            AddMiniboss(ModContent.NPCType<LifeChampion>(),
                14.004f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Life.Name"),
                () => FargoSoulsWorld.downedChampions[4],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<PumpkinEnchant>(),
                    ModContent.ItemType<BeeEnchant>(),
                    ModContent.ItemType<SpiderEnchant>(),
                    ModContent.ItemType<TurtleEnchant>(),
                    ModContent.ItemType<BeetleEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Life.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Life.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Life.Despawn"),
                "FargowiltasSouls/NPCs/Champions/LifeChampion_Still",
                "FargowiltasSouls/NPCs/Champions/LifeChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<ShadowChampion>(),
                14.005f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Shadow.Name"),
                () => FargoSoulsWorld.downedChampions[5],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<AncientShadowEnchant>(),
                    ModContent.ItemType<NecroEnchant>(),
                    ModContent.ItemType<SpookyEnchant>(),
                    ModContent.ItemType<ShinobiEnchant>(),
                    ModContent.ItemType<DarkArtistEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Shadow.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Shadow.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Shadow.Despawn"),
                "FargowiltasSouls/NPCs/Champions/ShadowChampion_Still",
                "FargowiltasSouls/NPCs/Champions/ShadowChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<SpiritChampion>(),
                14.006f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spirit.Name"),
                () => FargoSoulsWorld.downedChampions[6],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<FossilEnchant>(),
                    ModContent.ItemType<ForbiddenEnchant>(),
                    ModContent.ItemType<HallowEnchant>(),
                    ModContent.ItemType<TikiEnchant>(),
                    ModContent.ItemType<SpectreEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spirit.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spirit.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Spirit.Despawn"),
                "FargowiltasSouls/NPCs/Champions/SpiritChampion_Still",
                "FargowiltasSouls/NPCs/Champions/SpiritChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<WillChampion>(),
                14.007f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Will.Name"),
                () => FargoSoulsWorld.downedChampions[7],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<GoldEnchant>(),
                    ModContent.ItemType<PlatinumEnchant>(),
                    ModContent.ItemType<GladiatorEnchant>(),
                    ModContent.ItemType<RedRidingEnchant>(),
                    ModContent.ItemType<ValhallaKnightEnchant>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Will.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Will.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Will.Despawn"),
                "FargowiltasSouls/NPCs/Champions/WillChampion_Still",
                "FargowiltasSouls/NPCs/Champions/WillChampion_Head_Boss");

            AddMiniboss(ModContent.NPCType<CosmosChampion>(),
                14.008f,
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Cosmos.Name"),
                () => FargoSoulsWorld.downedChampions[8],
                ModContent.ItemType<SigilOfChampions>(),
                new List<int>
                {
                    ModContent.ItemType<SolarEnchant>(),
                    ModContent.ItemType<NebulaEnchant>(),
                    ModContent.ItemType<VortexEnchant>(),
                    ModContent.ItemType<StardustEnchant>(),
                    ModContent.ItemType<MeteorEnchant>(),
                    ModContent.ItemType<LunarCrystal>()
                },
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Cosmos.Spawn1") + $"[i:{ModContent.ItemType<SigilOfChampions>()}]" + Language.GetTextValue("Mods.FargowiltasSouls.BCL.Cosmos.Spawn2"),
                Language.GetTextValue("Mods.FargowiltasSouls.BCL.Cosmos.Despawn"),
                "FargowiltasSouls/NPCs/Champions/CosmosChampion_Still",
                "FargowiltasSouls/NPCs/Champions/CosmosChampion_Head_Boss");
        }

        private void InitializeBossLoot()
        {
            // Mini-bosses
            AddToBossLoot("PirateShip", ModContent.ItemType<SecurityWallet>()); // Coin gun & lucky coin are already dropped
            AddToBossLoot("MourningWood", ItemID.BloodyMachete);
            AddToBossLoot("Pumpking", ItemID.BladedGlove, ModContent.ItemType<PumpkingsCape>());
            AddToBossLoot("IceQueen", ModContent.ItemType<IceQueensCrown>());
            AddToBossLoot("MartianSaucerCore", ModContent.ItemType<SaucerControlConsole>());

            AddToBossLoot(new string[2] { "MourningWood", "Pumpking" }, ItemID.GoodieBag);
            AddToBossLoot(new string[3] { "Everscream", "SantaNK1", "IceQueen" }, ItemID.Present);

            // Bosses
            AddToBossLoot("KingSlime", ItemID.LifeCrystal, ModContent.ItemType<SlimyShield>(), ModContent.ItemType<MedallionoftheFallenKing>(), ModContent.ItemType<SlimeKingsSlasher>());
            AddToBossLoot("EyeofCthulhu", ItemID.HerbBag, ModContent.ItemType<AgitatingLens>(), ModContent.ItemType<LeashOfCthulhu>());
            AddToBossLoot("EaterofWorldsHead", ItemID.CorruptFishingCrate, ModContent.ItemType<CorruptHeart>(), ModContent.ItemType<EaterStaff>());
            AddToBossLoot("BrainofCthulhu", ItemID.CrimsonFishingCrate, ModContent.ItemType<GuttedHeart>(), ModContent.ItemType<BrainStaff>());
            AddToBossLoot("SkeletronHead", ItemID.DungeonFishingCrate, ModContent.ItemType<NecromanticBrew>(), ModContent.ItemType<BoneZone>());
            AddToBossLoot("QueenBee", ModContent.ItemType<QueenStinger>(), ModContent.ItemType<TheSmallSting>());
            AddToBossLoot("WallofFlesh", ModContent.ItemType<PungentEyeball>(), ItemID.HallowedFishingCrate, ModContent.ItemType<MutantsDiscountCard>(), ModContent.ItemType<FleshHand>()); // Shadow crates as well, but AFAIK those were removed
            AddToBossLoot("TheTwins", ModContent.ItemType<FusedLens>(), ModContent.ItemType<TwinRangs>());
            AddToBossLoot("TheDestroyer", ModContent.ItemType<GroundStick>(), ModContent.ItemType<DestroyerGun>());
            AddToBossLoot("SkeletronPrime", ModContent.ItemType<ReinforcedPlating>(), ModContent.ItemType<RefractorBlaster>());
            AddToBossLoot("Plantera", ModContent.ItemType<MagicalBulb>(), ItemID.LifeFruit, ItemID.ChlorophyteOre, ModContent.ItemType<Dicer>());
            AddToBossLoot("Golem", ModContent.ItemType<LihzahrdTreasureBox>(), ModContent.ItemType<ComputationOrb>(), ModContent.ItemType<RockSlide>());
            AddToBossLoot("DD2Betsy", ModContent.ItemType<BetsysHeart>());
            AddToBossLoot("DukeFishron", ModContent.ItemType<MutantAntibodies>(), ItemID.Bacon, ModContent.ItemType<MutantsPact>(), ModContent.ItemType<FishStick>(), ItemID.FuzzyCarrot, ItemID.AnglerHat, ItemID.AnglerVest, ItemID.AnglerPants, ItemID.GoldenFishingRod, ItemID.GoldenBugNet, ItemID.FishHook, ItemID.HighTestFishingLine, ItemID.AnglerEarring, ItemID.TackleBox, ItemID.FishermansGuide, ItemID.WeatherRadio, ItemID.Sextant, ItemID.FinWings, ItemID.BottomlessBucket, ItemID.SuperAbsorbantSponge, ItemID.HotlineFishingHook);
            AddToBossLoot("CultistBoss", ModContent.ItemType<CelestialRune>(), ModContent.ItemType<CelestialSeal>());
            AddToBossLoot("MoonLord", ModContent.ItemType<GalacticGlobe>());

            AddToBossLoot(new string[2] { "KingSlime", "EyeofCthulhu" }, ItemID.WoodenCrate);
            AddToBossLoot(new string[2] { "QueenBee", "Plantera" }, ItemID.JungleFishingCrate);
            AddToBossLoot(new string[3] { "TheTwins", "TheDestroyer", "SkeletronPrime" }, ItemID.IronCrate, ItemID.FallenStar);
            AddToBossLoot(new string[3] { "Golem", "DD2Betsy", "DukeFishron" }, ItemID.GoldenCrate);
        }

        public void AddBoss(int npcID, float progression, string npcName, Func<bool> downedBoss, int spawnItemID, List<int> collectionItemIDs, int lootItemID, string spawnInfo, string despawnMessage, string texture, string overrideHeadIconTexutre) => ModInstance.Call("AddBoss", progression, npcID, ModContent.GetInstance<Fargowiltas>(), npcName, downedBoss, spawnItemID, collectionItemIDs, lootItemID, spawnInfo, despawnMessage, texture, overrideHeadIconTexutre);

        public void AddMiniboss(int npcID, float progression, string npcName, Func<bool> downedBoss, int spawnItemID, List<int> lootItemIDs, string spawnInfo, string despawnMessage, string texture, string overrideHeadIconTexutre) => ModInstance.Call("AddMiniBoss", progression, npcID, ModContent.GetInstance<Fargowiltas>(), npcName, downedBoss, spawnItemID, new List<int> { }, lootItemIDs, spawnInfo, despawnMessage, texture, overrideHeadIconTexutre);

        public void AddToBossLoot(string[] npcs, string modName, params int[] itemIDs)
        {
            List<int> items = new List<int>();

            foreach (int item in itemIDs)
                items.Add(item);

            foreach (string npc in npcs)
                ModInstance.Call("AddToBossLoot", modName, npc, items);
        }

        public void AddToBossLoot(string npc, string modName, params int[] itemIDs) => AddToBossLoot(new string[1] { npc }, modName, itemIDs);

        public void AddToBossLoot(string npc, params int[] itemIDs) => AddToBossLoot(new string[1] { npc }, "Terraria", itemIDs);

        public void AddToBossLoot(string[] npcs, params int[] itemIDs) => AddToBossLoot(npcs, "Terraria", itemIDs);
    }
}
