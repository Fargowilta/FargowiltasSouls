using FargowiltasSouls.Items;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Accessories.Souls;
using FargowiltasSouls.Items.Armor;
using FargowiltasSouls.Items.Consumables;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.Items.Summons;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls
    {
        private void AddLocalizations()
        {
            #region helpers

            void Add(string key, string message)
            {
                ModTranslation text = LocalizationLoader.CreateTranslation(Instance, key);
                text.SetDefault(message);
                LocalizationLoader.AddTranslation(text);
            }

            void AddBossSpawnInfo(string bossName, string spawnInfo)
            {
                Add($"BossChecklist.{bossName}SpawnInfo", spawnInfo);
            }

            void AddToggle(string toggle, int item, string color = "ffffff")
            {
                ModTranslation text = LocalizationLoader.CreateTranslation(Instance, toggle);
                text.SetDefault($"[i:{item}] [c/{color}:{{$Mods.{Name}.Toggler.{toggle}}}]");
                LocalizationLoader.AddTranslation(text);
            }

            #endregion helpers

            Add("Message.MasochistExtraTooltip", $"[i:{ModContent.ItemType<MutantsPact>()}] {{$Mods.{Name}.Message.MasochistPreTooltip}}");


            #region boss spawn info

            if (FargoSoulsUtil.IsChinese())
            {
                AddBossSpawnInfo("DeviBoss", $"使用[i:{ModContent.ItemType<DevisCurse>()}]召唤");
                AddBossSpawnInfo("AbomBoss", $"使用[i:{ModContent.ItemType<AbomsCurse>()}]召唤");
                AddBossSpawnInfo("MutantBoss", $"在突变体和憎恶存活时将[i:{ModContent.ItemType<AbominationnVoodooDoll>()}]投入岩浆池中。");

                AddBossSpawnInfo("TimberChampion", $"白天时在地表使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("TerraChampion", $"在地下使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("EarthChampion", $"在地狱使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("NatureChampion", $"在地下雪原使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("LifeChampion", $"白天时在神圣之地使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("ShadowChampion", $"夜晚时在腐化之地或猩红之地使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("SpiritChampion", $"在地下沙漠使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("WillChampion", $"在海洋使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");
                AddBossSpawnInfo("CosmosChampion", $"在太空使用[i:{ModContent.ItemType<SigilOfChampions>()}]召唤。");

                AddBossSpawnInfo("TrojanSquirrel", $"使用[i:{ModContent.ItemType<SquirrelCoatofArms>()}]召唤");
            }
            else
            {
                AddBossSpawnInfo("DeviBoss", $"Spawn by using [i:{ModContent.ItemType<DevisCurse>()}]");
                AddBossSpawnInfo("AbomBoss", $"Spawn by using [i:{ModContent.ItemType<AbomsCurse>()}]");
                AddBossSpawnInfo("MutantBoss", $"Throw [i:{ModContent.ItemType<AbominationnVoodooDoll>()}] into a pool of lava while Abominationn is alive in Mutant's presence.");

                AddBossSpawnInfo("TimberChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] on the surface during day.");
                AddBossSpawnInfo("TerraChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] underground.");
                AddBossSpawnInfo("EarthChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the underworld.");
                AddBossSpawnInfo("NatureChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in underground snow.");
                AddBossSpawnInfo("LifeChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the Hallow at day.");
                AddBossSpawnInfo("ShadowChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the Corruption or Crimson at night.");
                AddBossSpawnInfo("SpiritChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in the underground desert.");
                AddBossSpawnInfo("WillChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] at the ocean.");
                AddBossSpawnInfo("CosmosChampion", $"Spawn by using [i:{ModContent.ItemType<SigilOfChampions>()}] in space.");

                AddBossSpawnInfo("TrojanSquirrel", $"Spawn by using [i:{ModContent.ItemType<SquirrelCoatofArms>()}]");
            }

            #endregion boss spawn info


            #region Toggles

            //AddToggle("PresetHeader", ModContent.ItemType<Masochist>());

            #region enchants

            AddToggle("WoodHeader", ModContent.ItemType<TimberForce>());
            AddToggle("BorealConfig", ModContent.ItemType<BorealWoodEnchant>(), "8B7464");
            AddToggle("MahoganyConfig", ModContent.ItemType<RichMahoganyEnchant>(), "b56c64");
            AddToggle("EbonConfig", ModContent.ItemType<EbonwoodEnchant>(), "645a8d");
            AddToggle("ShadeConfig", ModContent.ItemType<ShadewoodEnchant>(), "586876");
            AddToggle("ShadeOnHitConfig", ModContent.ItemType<ShadewoodEnchant>(), "586876");
            AddToggle("PalmConfig", ModContent.ItemType<PalmWoodEnchant>(), "b78d56");
            AddToggle("PearlConfig", ModContent.ItemType<PearlwoodEnchant>(), "ad9a5f");

            AddToggle("EarthHeader", ModContent.ItemType<EarthForce>());
            AddToggle("AdamantiteConfig", ModContent.ItemType<AdamantiteEnchant>(), "dd557d");
            AddToggle("CobaltConfig", ModContent.ItemType<CobaltEnchant>(), "3da4c4");
            AddToggle("AncientCobaltConfig", ModContent.ItemType<AncientCobaltEnchant>(), "354c74");
            AddToggle("MythrilConfig", ModContent.ItemType<MythrilEnchant>(), "9dd290");
            AddToggle("OrichalcumConfig", ModContent.ItemType<OrichalcumEnchant>(), "eb3291");
            AddToggle("PalladiumConfig", ModContent.ItemType<PalladiumEnchant>(), "f5ac28");
            AddToggle("PalladiumOrbConfig", ModContent.ItemType<PalladiumEnchant>(), "f5ac28");
            AddToggle("TitaniumConfig", ModContent.ItemType<TitaniumEnchant>(), "828c88");

            AddToggle("TerraHeader", ModContent.ItemType<TerraForce>());
            AddToggle("CopperConfig", ModContent.ItemType<CopperEnchant>(), "d56617");
            AddToggle("IronMConfig", ModContent.ItemType<IronEnchant>(), "988e83");
            AddToggle("IronSConfig", ModContent.ItemType<IronEnchant>(), "988e83");
            AddToggle("TinConfig", ModContent.ItemType<TinEnchant>(), "a28b4e");
            AddToggle("TungstenConfig", ModContent.ItemType<TungstenEnchant>(), "b0d2b2");
            AddToggle("TungstenProjConfig", ModContent.ItemType<TungstenEnchant>(), "b0d2b2");
            AddToggle("ObsidianConfig", ModContent.ItemType<ObsidianEnchant>(), "453e73");

            AddToggle("WillHeader", ModContent.ItemType<WillForce>());
            AddToggle("GladiatorConfig", ModContent.ItemType<GladiatorEnchant>(), "9c924e");
            AddToggle("GoldConfig", ModContent.ItemType<GoldEnchant>(), "e7b21c");
            AddToggle("GoldToPiggyConfig", ModContent.ItemType<GoldEnchant>(), "e7b21c");
            AddToggle("HuntressConfig", ModContent.ItemType<HuntressEnchant>(), "7ac04c");
            AddToggle("RedRidingRainConfig", ModContent.ItemType<RedRidingEnchant>(), "c01b3c");
            AddToggle("ValhallaConfig", ModContent.ItemType<ValhallaKnightEnchant>(), "93651e");
            AddToggle("SquirePanicConfig", ModContent.ItemType<SquireEnchant>(), "948f8c");

            AddToggle("LifeHeader", ModContent.ItemType<LifeForce>());
            AddToggle("BeeConfig", ModContent.ItemType<BeeEnchant>(), "FEF625");
            AddToggle("BeetleConfig", ModContent.ItemType<BeetleEnchant>(), "6D5C85");
            AddToggle("CactusConfig", ModContent.ItemType<CactusEnchant>(), "799e1d");
            AddToggle("PumpkinConfig", ModContent.ItemType<PumpkinEnchant>(), "e3651c");
            AddToggle("SpiderConfig", ModContent.ItemType<SpiderEnchant>(), "6d4e45");
            AddToggle("TurtleConfig", ModContent.ItemType<TurtleEnchant>(), "f89c5c");

            AddToggle("NatureHeader", ModContent.ItemType<NatureForce>());
            AddToggle("ChlorophyteConfig", ModContent.ItemType<ChlorophyteEnchant>(), "248900");
            AddToggle("CrimsonConfig", ModContent.ItemType<CrimsonEnchant>(), "C8364B");
            AddToggle("RainConfig", ModContent.ItemType<RainEnchant>(), "ffec00");
            AddToggle("RainInnerTubeConfig", ModContent.ItemType<RainEnchant>(), "ffec00");
            AddToggle("FrostConfig", ModContent.ItemType<FrostEnchant>(), "7abdb9");
            AddToggle("JungleConfig", ModContent.ItemType<JungleEnchant>(), "71971f");
            AddToggle("JungleDashConfig", ModContent.ItemType<JungleEnchant>(), "71971f");
            AddToggle("MoltenConfig", ModContent.ItemType<MoltenEnchant>(), "c12b2b");
            AddToggle("MoltenEConfig", ModContent.ItemType<MoltenEnchant>(), "c12b2b");
            AddToggle("ShroomiteConfig", ModContent.ItemType<ShroomiteEnchant>(), "008cf4");
            AddToggle("ShroomiteShroomConfig", ModContent.ItemType<ShroomiteEnchant>(), "008cf4");

            AddToggle("ShadowHeader", ModContent.ItemType<ShadowForce>());
            AddToggle("DarkArtConfig", ModContent.ItemType<DarkArtistEnchant>(), "9b5cb0");
            AddToggle("ApprenticeConfig", ModContent.ItemType<ApprenticeEnchant>(), "5d86a6");
            AddToggle("NecroConfig", ModContent.ItemType<NecroEnchant>(), "565643");
            AddToggle("NecroGloveConfig", ModContent.ItemType<NecroEnchant>(), "565643");
            AddToggle("ShadowConfig", ModContent.ItemType<ShadowEnchant>(), "42356f");
            AddToggle("AncientShadowConfig", ModContent.ItemType<AncientShadowEnchant>(), "42356f");
            AddToggle("MonkConfig", ModContent.ItemType<MonkEnchant>(), "920520");
            AddToggle("ShinobiDashConfig", ModContent.ItemType<ShinobiEnchant>(), "935b18");
            AddToggle("ShinobiConfig", ModContent.ItemType<ShinobiEnchant>(), "935b18");
            AddToggle("SpookyConfig", ModContent.ItemType<SpookyEnchant>(), "644e74");
            AddToggle("CrystalDashConfig", ModContent.ItemType<CrystalAssassinEnchant>(), "249dcf");
            AddToggle("CrystalGelatinConfig", ModContent.ItemType<CrystalAssassinEnchant>(), "249dcf");

            AddToggle("SpiritHeader", ModContent.ItemType<SpiritForce>());
            AddToggle("FossilConfig", ModContent.ItemType<FossilEnchant>(), "8c5c3b");
            AddToggle("ForbiddenConfig", ModContent.ItemType<ForbiddenEnchant>(), "e7b21c");
            AddToggle("HallowDodgeConfig", ModContent.ItemType<HallowEnchant>(), "968564");
            AddToggle("HallowedConfig", ModContent.ItemType<AncientHallowEnchant>(), "968564");
            AddToggle("HallowSConfig", ModContent.ItemType<AncientHallowEnchant>(), "968564");
            AddToggle("SilverConfig", ModContent.ItemType<SilverEnchant>(), "b4b4cc");
            AddToggle("SilverSpeedConfig", ModContent.ItemType<SilverEnchant>(), "b4b4cc");
            AddToggle("SpectreConfig", ModContent.ItemType<SpectreEnchant>(), "accdfc");
            AddToggle("TikiConfig", ModContent.ItemType<TikiEnchant>(), "56A52B");

            AddToggle("CosmoHeader", ModContent.ItemType<CosmoForce>());
            AddToggle("MeteorConfig", ModContent.ItemType<MeteorEnchant>(), "5f4752");
            AddToggle("NebulaConfig", ModContent.ItemType<NebulaEnchant>(), "fe7ee5");
            AddToggle("SolarConfig", ModContent.ItemType<SolarEnchant>(), "fe9e23");
            AddToggle("SolarFlareConfig", ModContent.ItemType<SolarEnchant>(), "fe9e23");
            AddToggle("StardustConfig", ModContent.ItemType<StardustEnchant>(), "00aeee");
            AddToggle("VortexSConfig", ModContent.ItemType<VortexEnchant>(), "00f2aa");
            AddToggle("VortexVConfig", ModContent.ItemType<VortexEnchant>(), "00f2aa");

            #endregion enchants

            #region masomode toggles

            //Masomode Header
            AddToggle("MasoHeader", ModContent.ItemType<MutantStatue>());
            AddToggle("MasoBossRecolors", ModContent.ItemType<Masochist>());
            AddToggle("MasoCanPlay", ModContent.ItemType<Masochist>(), "ff0000");

            AddToggle("MasoHeader2", ModContent.ItemType<DeviatingEnergy>());
            AddToggle("DeerSinewDashConfig", ModContent.ItemType<DeerSinew>());
            AddToggle("MasoAeolusConfig", ModContent.ItemType<AeolusBoots>());
            AddToggle("MasoAeolusFlowerConfig", ModContent.ItemType<AeolusBoots>());
            AddToggle("MasoIconConfig", ModContent.ItemType<SinisterIcon>());
            AddToggle("MasoIconDropsConfig", ModContent.ItemType<SinisterIcon>());
            AddToggle("MasoGrazeConfig", ModContent.ItemType<SparklingAdoration>());
            AddToggle("MasoGrazeRingConfig", ModContent.ItemType<SparklingAdoration>());
            AddToggle("MasoDevianttHeartsConfig", ModContent.ItemType<SparklingAdoration>());
            AddToggle("PrecisionSealHurtboxConfig", ModContent.ItemType<PrecisionSeal>());

            //supreme death fairy header
            AddToggle("SupremeFairyHeader", ModContent.ItemType<SupremeDeathbringerFairy>());
            AddToggle("MasoSlimeConfig", ModContent.ItemType<SlimyShield>());
            AddToggle("SlimeFallingConfig", ModContent.ItemType<SlimyShield>());
            AddToggle("MasoEyeConfig", ModContent.ItemType<AgitatingLens>());
            AddToggle("MasoEyeInstallConfig", ModContent.ItemType<AgitatingLens>());
            AddToggle("MasoSkeleConfig", ModContent.ItemType<NecromanticBrew>());
            AddToggle("MasoSkeleSpinConfig", ModContent.ItemType<NecromanticBrew>());

            //bionomic
            AddToggle("BionomicHeader", ModContent.ItemType<BionomicCluster>());
            AddToggle("MasoConcoctionConfig", ModContent.ItemType<TimsConcoction>());
            AddToggle("MasoCarrotConfig", ModContent.ItemType<OrdinaryCarrot>());
            AddToggle("MasoRainbowConfig", ModContent.ItemType<ConcentratedRainbowMatter>());
            AddToggle("MasoHealingPotionConfig", ModContent.ItemType<ConcentratedRainbowMatter>());
            AddToggle("MasoNymphConfig", ModContent.ItemType<NymphsPerfume>());
            AddToggle("MasoSqueakConfig", ModContent.ItemType<Items.Accessories.Masomode.SqueakyToy>());
            AddToggle("MasoPouchConfig", ModContent.ItemType<WretchedPouch>());
            AddToggle("MasoClippedConfig", ModContent.ItemType<WyvernFeather>());
            AddToggle("TribalCharmConfig", ModContent.ItemType<TribalCharm>());
            AddToggle("TribalCharmClickBonusConfig", ModContent.ItemType<TribalCharm>());
            //AddToggle("WalletHeader", ModContent.ItemType<SecurityWallet>());

            //dubious
            AddToggle("DubiousHeader", ModContent.ItemType<DubiousCircuitry>());
            AddToggle("FusedLensInstallConfig", ModContent.ItemType<FusedLens>());
            AddToggle("MasoLightningConfig", ModContent.ItemType<GroundStick>());
            AddToggle("MasoProbeConfig", ModContent.ItemType<GroundStick>());

            //pure heart
            AddToggle("PureHeartHeader", ModContent.ItemType<PureHeart>());
            AddToggle("MasoEaterConfig", ModContent.ItemType<DarkenedHeart>());
            AddToggle("MasoBrainConfig", ModContent.ItemType<GuttedHeart>());

            //lump of flesh
            AddToggle("LumpofFleshHeader", ModContent.ItemType<LumpOfFlesh>());
            AddToggle("MasoPungentCursorConfig", ModContent.ItemType<PungentEyeball>());
            AddToggle("MasoPugentConfig", ModContent.ItemType<LumpOfFlesh>());
            AddToggle("DreadShellParryConfig", ModContent.ItemType<DreadShell>());
            AddToggle("DeerclawpsConfig", ModContent.ItemType<Deerclawps>());

            //chalice
            AddToggle("ChaliceHeader", ModContent.ItemType<ChaliceoftheMoon>());
            AddToggle("MasoCultistConfig", ModContent.ItemType<ChaliceoftheMoon>());
            AddToggle("MasoPlantConfig", ModContent.ItemType<MagicalBulb>());
            AddToggle("MasoGolemConfig", ModContent.ItemType<LihzahrdTreasureBox>());
            AddToggle("MasoBoulderConfig", ModContent.ItemType<LihzahrdTreasureBox>());
            AddToggle("MasoCelestConfig", ModContent.ItemType<CelestialRune>());
            AddToggle("MasoVisionConfig", ModContent.ItemType<CelestialRune>());

            //heart of the masochist
            AddToggle("HeartHeader", ModContent.ItemType<HeartoftheMasochist>());
            AddToggle("MasoPumpConfig", ModContent.ItemType<PumpkingsCape>());
            AddToggle("IceQueensCrownConfig", ModContent.ItemType<IceQueensCrown>());
            AddToggle("MasoUfoConfig", ModContent.ItemType<SaucerControlConsole>());
            AddToggle("MasoGravConfig", ModContent.ItemType<GalacticGlobe>());
            AddToggle("MasoGrav2Config", ModContent.ItemType<GalacticGlobe>());
            AddToggle("MasoTrueEyeConfig", ModContent.ItemType<GalacticGlobe>());

            //cyclonic fin
            AddToggle("CyclonicHeader", ModContent.ItemType<AbominableWand>());
            AddToggle("MasoFishronConfig", ModContent.ItemType<AbominableWand>());

            //mutant armor
            AddToggle("MutantArmorHeader", ModContent.ItemType<HeartoftheMasochist>());
            AddToggle("MasoAbomConfig", ModContent.ItemType<MutantMask>());
            AddToggle("MasoRingConfig", ModContent.ItemType<MutantMask>());
            AddToggle("MasoReviveDeathrayConfig", ModContent.ItemType<MutantMask>());

            #endregion masomode toggles

            #region soul toggles

            AddToggle("UniverseHeader", ModContent.ItemType<UniverseSoul>());
            AddToggle("MeleeConfig", ModContent.ItemType<BerserkerSoul>());
            AddToggle("MagmaStoneConfig", ModContent.ItemType<BerserkerSoul>());
            AddToggle("YoyoBagConfig", ModContent.ItemType<BerserkerSoul>());
            AddToggle("MoonCharmConfig", ModContent.ItemType<BerserkerSoul>());
            AddToggle("NeptuneShellConfig", ModContent.ItemType<BerserkerSoul>());
            AddToggle("SniperConfig", ModContent.ItemType<SnipersSoul>());
            AddToggle("ManaFlowerConfig", ModContent.ItemType<ArchWizardsSoul>());
            AddToggle("UniverseConfig", ModContent.ItemType<UniverseSoul>());

            AddToggle("WorldShaperHeader", ModContent.ItemType<WorldShaperSoul>());
            AddToggle("MiningHuntConfig", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningDangerConfig", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningSpelunkConfig", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningShineConfig", ModContent.ItemType<MinerEnchant>());
            AddToggle("BuilderConfig", ModContent.ItemType<WorldShaperSoul>());

            AddToggle("ColossusHeader", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseStarConfig", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseBrainConfig", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseBeeConfig", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefensePanicConfig", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseFleshKnuckleConfig", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefensePaladinConfig", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseFrozenConfig", ModContent.ItemType<ColossusSoul>());

            AddToggle("FlightMasteryHeader", ModContent.ItemType<FlightMasterySoul>());
            AddToggle("FlightMasteryInsigniaConfig", ModContent.ItemType<FlightMasterySoul>());
            AddToggle("FlightMasteryGravityConfig", ModContent.ItemType<FlightMasterySoul>());

            AddToggle("SupersonicHeader", ModContent.ItemType<SupersonicSoul>());
            AddToggle("RunSpeedConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("MomentumConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicTabiConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicClimbingConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicJumpsConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicRocketBootsConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicCarpetConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicFlowerConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("CthulhuShieldConfig", ModContent.ItemType<SupersonicSoul>());
            AddToggle("BlackBeltConfig", ModContent.ItemType<SupersonicSoul>());

            AddToggle("TrawlerHeader", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerSporeConfig", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerConfig", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerJumpConfig", ModContent.ItemType<TrawlerSoul>());

            AddToggle("EternityHeader", ModContent.ItemType<EternitySoul>());
            AddToggle("EternityConfig", ModContent.ItemType<EternitySoul>());

            #endregion soul toggles

            #region pet toggles

            AddToggle("PetHeader", ItemID.ZephyrFish);
            AddToggle("PetBlackCatConfig", ItemID.UnluckyYarn);
            AddToggle("PetCompanionCubeConfig", ItemID.CompanionCube);
            AddToggle("PetCursedSaplingConfig", ItemID.CursedSapling);
            AddToggle("PetDinoConfig", ItemID.AmberMosquito);
            AddToggle("PetDragonConfig", ItemID.DD2PetDragon);
            AddToggle("PetEaterConfig", ItemID.EatersBone);
            AddToggle("PetEyeSpringConfig", ItemID.EyeSpring);
            AddToggle("PetFaceMonsterConfig", ItemID.BoneRattle);
            AddToggle("PetGatoConfig", ItemID.DD2PetGato);
            AddToggle("PetHornetConfig", ItemID.Nectar);
            AddToggle("PetLizardConfig", ItemID.LizardEgg);
            AddToggle("PetMinitaurConfig", ItemID.TartarSauce);
            AddToggle("PetParrotConfig", ItemID.ParrotCracker);
            AddToggle("PetPenguinConfig", ItemID.Fish);
            AddToggle("PetPupConfig", ItemID.DogWhistle);
            AddToggle("PetSeedConfig", ItemID.Seedling);
            AddToggle("PetDGConfig", ItemID.BoneKey);
            AddToggle("PetSnowmanConfig", ItemID.ToySled);
            AddToggle("PetGrinchConfig", ItemID.BabyGrinchMischiefWhistle);
            AddToggle("PetSpiderConfig", ItemID.SpiderEgg);
            AddToggle("PetSquashConfig", ItemID.MagicalPumpkinSeed);
            AddToggle("PetTikiConfig", ItemID.TikiTotem);
            AddToggle("PetShroomConfig", ItemID.StrangeGlowingMushroom);
            AddToggle("PetTurtleConfig", ItemID.Seaweed);
            AddToggle("PetZephyrConfig", ItemID.ZephyrFish);
            AddToggle("PetHeartConfig", ItemID.CrimsonHeart);
            AddToggle("PetNaviConfig", ItemID.FairyBell);
            AddToggle("PetFlickerConfig", ItemID.DD2OgrePetItem);
            AddToggle("PetLanternConfig", ItemID.MagicLantern);
            AddToggle("PetOrbConfig", ItemID.ShadowOrb);
            AddToggle("PetSuspEyeConfig", ItemID.SuspiciousLookingTentacle);
            AddToggle("PetWispConfig", ItemID.WispinaBottle);

            #endregion pet toggles

            #region patreon toggles
            AddToggle("PatreonHeader", ModContent.ItemType<Patreon.Gittle.RoombaPet>());
            AddToggle("PatreonRoomba", ModContent.ItemType<Patreon.Gittle.RoombaPet>());
            AddToggle("PatreonOrb", ModContent.ItemType<Patreon.Daawnz.ComputationOrb>());
            AddToggle("PatreonFishingRod", ModContent.ItemType<Patreon.Sasha.MissDrakovisFishingPole>());
            AddToggle("PatreonDoor", ModContent.ItemType<Patreon.Sam.SquidwardDoor>());
            AddToggle("PatreonWolf", ModContent.ItemType<Patreon.ParadoxWolf.ParadoxWolfSoul>());
            AddToggle("PatreonDove", ModContent.ItemType<Patreon.ManliestDove.FigBranch>());
            AddToggle("PatreonKingSlime", ModContent.ItemType<Patreon.Catsounds.MedallionoftheFallenKing>());
            AddToggle("PatreonFishron", ModContent.ItemType<Patreon.DemonKing.StaffOfUnleashedOcean>());
            AddToggle("PatreonPlant", ModContent.ItemType<Patreon.LaBonez.PiranhaPlantVoodooDoll>());
            AddToggle("PatreonDevious", ModContent.ItemType<Patreon.DevAesthetic.DeviousAestheticus>());
            AddToggle("PatreonVortex", ModContent.ItemType<Patreon.GreatestKraken.VortexMagnetRitual>());
            AddToggle("PatreonPrime", ModContent.ItemType<Patreon.Purified.PrimeStaff>());
            AddToggle("PatreonCrimetroid", ModContent.ItemType<Patreon.Shucks.CrimetroidEgg>());
            AddToggle("PatreonNanoCore", ModContent.ItemType<Patreon.Volknet.NanoCore>());
            #endregion patreon toggles

            #endregion Toggles


        }
    }
}
