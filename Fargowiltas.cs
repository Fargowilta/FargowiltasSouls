using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using FargowiltasSouls.ModCompatibilities;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs.AbomBoss;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.NPCs.DeviBoss;
using FargowiltasSouls.NPCs.MutantBoss;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Sky;
using Fargowiltas.Items.Summons.Deviantt;
using Fargowiltas.Items.Misc;
using Fargowiltas.Items.Explosives;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Items.Dyes;
using FargowiltasSouls.UI;
using FargowiltasSouls.Toggler;
using System.Linq;
using FargowiltasSouls.Patreon;

namespace FargowiltasSouls
{
    public partial class Fargowiltas : Mod
    {
        internal static ModHotKey FreezeKey;
        internal static ModHotKey GoldKey;
        internal static ModHotKey SmokeBombKey;
        internal static ModHotKey BetsyDashKey;
        internal static ModHotKey MutantBombKey;
        internal static ModHotKey SoulToggleKey;

        internal static List<int> DebuffIDs;

        internal static Fargowiltas Instance;

        internal bool LoadedNewSprites;

        internal static float OldVolume;

        public UserInterface CustomResources;

        internal static readonly Dictionary<int, int> ModProjDict = new Dictionary<int, int>();

        public static UIManager UserInterfaceManager => Instance._userInterfaceManager;
        private UIManager _userInterfaceManager;

        #region Compatibilities

        public CalamityCompatibility CalamityCompatibility { get; private set; }
        public bool CalamityLoaded => CalamityCompatibility != null;

        public ThoriumCompatibility ThoriumCompatibility { get; private set; }
        public bool ThoriumLoaded => ThoriumCompatibility != null;

        public SoACompatibility SoACompatibility { get; private set; }
        public bool SoALoaded => SoACompatibility != null;

        public MasomodeEXCompatibility MasomodeEXCompatibility { get; private set; }
        public bool MasomodeEXLoaded => MasomodeEXCompatibility != null;

        public BossChecklistCompatibility BossChecklistCompatibility { get; private set; }
        public bool BossChecklistLoaded => BossChecklistCompatibility != null;

        #endregion Compatibilities

        public Fargowiltas()
        {
            Properties = new ModProperties
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

        public override void Load()
        {
            Instance = this;

            SkyManager.Instance["FargowiltasSouls:AbomBoss"] = new AbomSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss2"] = new MutantSky2();

            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                FreezeKey = RegisterHotKey("冻结时间", "P");
                GoldKey = RegisterHotKey("金身", "O");
                SmokeBombKey = RegisterHotKey("Throw Smoke Bomb", "I");
                BetsyDashKey = RegisterHotKey("Betsy Dash", "C");
                MutantBombKey = RegisterHotKey("Mutant Bomb", "Z");
                SoulToggleKey = RegisterHotKey("Open Soul Toggler", ".");
            }
            else
            {
                FreezeKey = RegisterHotKey("Freeze Time", "P");
                GoldKey = RegisterHotKey("Turn Gold", "O");
                SmokeBombKey = RegisterHotKey("Throw Smoke Bomb", "I");
                BetsyDashKey = RegisterHotKey("Fireball Dash", "C");
                MutantBombKey = RegisterHotKey("Mutant Bomb", "Z");
                SoulToggleKey = RegisterHotKey("Open Soul Toggler", ".");
            }

            ToggleLoader.Load();

            _userInterfaceManager = new UIManager();
            _userInterfaceManager.LoadUI();

            #region Toggles

            AddToggle("PresetHeader", "Preset Configurations", "Masochist", "ffffff");

            #region enchants

            AddToggle("WoodHeader", "Force of Timber", "TimberForce", "ffffff");
            AddToggle("BorealConfig", "Boreal Snowballs", "BorealWoodEnchant", "8B7464");
            AddToggle("MahoganyConfig", "Mahogany Hook Speed", "RichMahoganyEnchant", "b56c64");
            AddToggle("EbonConfig", "Ebonwood Shadowflame", "EbonwoodEnchant", "645a8d");
            AddToggle("ShadeConfig", "Blood Geyser On Hit", "ShadewoodEnchant", "586876");
            AddToggle("ShadeOnHitConfig", "Proximity Triggers On Hit Effects", "ShadewoodEnchant", "586876");
            AddToggle("PalmConfig", "Palmwood Sentry", "PalmWoodEnchant", "b78d56");
            AddToggle("PearlConfig", "Pearlwood Rain", "PearlwoodEnchant", "ad9a5f");

            AddToggle("EarthHeader", "Force of Earth", "EarthForce", "ffffff");
            AddToggle("AdamantiteConfig", "Adamantite Projectile Splitting", "AdamantiteEnchant", "dd557d");
            AddToggle("CobaltConfig", "Cobalt Shards", "CobaltEnchant", "3da4c4");
            AddToggle("AncientCobaltConfig", "Ancient Cobalt Stingers", "AncientCobaltEnchant", "354c74");
            AddToggle("MythrilConfig", "Mythril Weapon Speed", "MythrilEnchant", "9dd290");
            AddToggle("OrichalcumConfig", "Orichalcum Petals", "OrichalcumEnchant", "eb3291");
            AddToggle("PalladiumConfig", "Palladium Healing", "PalladiumEnchant", "f5ac28");
            AddToggle("PalladiumOrbConfig", "Palladium Orbs", "PalladiumEnchant", "f5ac28");
            AddToggle("TitaniumConfig", "Titanium Shadow Dodge", "TitaniumEnchant", "828c88");

            AddToggle("TerraHeader", "Terra Force", "TerraForce", "ffffff");
            AddToggle("CopperConfig", "Copper Lightning", "CopperEnchant", "d56617");
            AddToggle("IronMConfig", "Iron Magnet", "IronEnchant", "988e83");
            AddToggle("IronSConfig", "Iron Shield", "IronEnchant", "988e83");
            AddToggle("TinConfig", "Tin Crits", "TinEnchant", "a28b4e");
            AddToggle("TungstenConfig", "Tungsten Item Effect", "TungstenEnchant", "b0d2b2");
            AddToggle("TungstenProjConfig", "Tungsten Projectile Effect", "TungstenEnchant", "b0d2b2");
            AddToggle("ObsidianConfig", "Obsidian Explosions", "ObsidianEnchant", "453e73");

            AddToggle("WillHeader", "Force of Will", "WillForce", "ffffff");
            AddToggle("GladiatorConfig", "Gladiator Rain", "GladiatorEnchant", "9c924e");
            AddToggle("GoldConfig", "Gold Lucky Coin", "GoldEnchant", "e7b21c");
            AddToggle("HuntressConfig", "Huntress Ability", "HuntressEnchant", "7ac04c");
            AddToggle("ValhallaConfig", "Squire/Valhalla Healing", "ValhallaKnightEnchant", "93651e");
            AddToggle("SquirePanicConfig", "Ballista Panic On Hit", "SquireEnchant", "948f8c");

            AddToggle("LifeHeader", "Force of Life", "LifeForce", "ffffff");
            AddToggle("BeeConfig", "Bees", "BeeEnchant", "FEF625");
            AddToggle("BeetleConfig", "Beetles", "BeetleEnchant", "6D5C85");
            AddToggle("CactusConfig", "Cactus Needles", "CactusEnchant", "799e1d");
            AddToggle("PumpkinConfig", "Grow Pumpkins", "PumpkinEnchant", "e3651c");
            AddToggle("SpiderConfig", "Spider Crits", "SpiderEnchant", "6d4e45");
            AddToggle("TurtleConfig", "Turtle Shell Buff", "TurtleEnchant", "f89c5c");

            AddToggle("NatureHeader", "Force of Nature", "NatureForce", "ffffff");
            AddToggle("ChlorophyteConfig", "Chlorophyte Leaf Crystal", "ChlorophyteEnchant", "248900");
            AddToggle("CrimsonConfig", "Crimson Regen", "CrimsonEnchant", "C8364B");
            AddToggle("RainConfig", "Rain Clouds", "RainEnchant", "ffec00");
            AddToggle("FrostConfig", "Frost Icicles", "FrostEnchant", "7abdb9");
            AddToggle("SnowConfig", "Snowstorm", "SnowEnchant", "25c3f2");
            AddToggle("JungleConfig", "Jungle Spores", "JungleEnchant", "71971f");
            AddToggle("MoltenConfig", "Molten Inferno Buff", "MoltenEnchant", "c12b2b");
            AddToggle("MoltenEConfig", "Molten Explosion On Hit", "MoltenEnchant", "c12b2b");
            AddToggle("ShroomiteConfig", "Shroomite Stealth", "ShroomiteEnchant", "008cf4");
            AddToggle("ShroomiteShroomConfig", "Shroomite Mushrooms", "ShroomiteEnchant", "008cf4");

            AddToggle("ShadowHeader", "Shadow Force", "ShadowForce", "ffffff");
            AddToggle("DarkArtConfig", "Flameburst Minion", "DarkArtistEnchant", "9b5cb0");
            AddToggle("ApprenticeConfig", "Apprentice Effect", "ApprenticeEnchant", "5d86a6");
            AddToggle("NecroConfig", "Necro Graves", "NecroEnchant", "565643");
            AddToggle("ShadowConfig", "Shadow Orbs", "ShadowEnchant", "42356f");
            AddToggle("AncientShadowConfig", "Ancient Shadow Darkness", "AncientShadowEnchant", "42356f");
            AddToggle("MonkConfig", "Monk Dash", "MonkEnchant", "920520");
            AddToggle("ShinobiDashConfig", "Shinobi Teleport Dash", "ShinobiEnchant", "935b18");
            AddToggle("ShinobiConfig", "Shinobi Through Walls", "ShinobiEnchant", "935b18");
            AddToggle("SupersonicTabiConfig", "Tabi Dash", "SupersonicSoul", "935b18");
            AddToggle("SupersonicClimbingConfig", "Tiger Climbing Gear", "SupersonicSoul", "935b18");
            AddToggle("SpookyConfig", "Spooky Scythes", "SpookyEnchant", "644e74");

            AddToggle("SpiritHeader", "Force of Spirit", "SpiritForce", "ffffff");
            AddToggle("FossilConfig", "Fossil Bones On Hit", "FossilEnchant", "8c5c3b");
            AddToggle("ForbiddenConfig", "Forbidden Storm", "ForbiddenEnchant", "e7b21c");
            AddToggle("HallowedConfig", "Hallowed Enchanted Sword Familiar", "HallowEnchant", "968564");
            AddToggle("HallowSConfig", "Hallowed Shield", "HallowEnchant", "968564");
            AddToggle("SilverConfig", "Silver Sword Familiar", "SilverEnchant", "b4b4cc");
            AddToggle("SilverSpeedConfig", "Silver Minion Speed", "SilverEnchant", "b4b4cc");
            AddToggle("SpectreConfig", "Spectre Orbs", "SpectreEnchant", "accdfc");
            AddToggle("TikiConfig", "Tiki Minions", "TikiEnchant", "56A52B");

            AddToggle("CosmoHeader", "Force of Cosmos", "CosmoForce", "ffffff");
            AddToggle("MeteorConfig", "Meteor Shower", "MeteorEnchant", "5f4752");
            AddToggle("NebulaConfig", "Nebula Boosters", "NebulaEnchant", "fe7ee5");
            AddToggle("SolarConfig", "Solar Shield", "SolarEnchant", "fe9e23");
            AddToggle("SolarFlareConfig", "Inflict Solar Flare", "SolarEnchant", "fe9e23");
            AddToggle("StardustConfig", "Stardust Guardian", "StardustEnchant", "00aeee");
            AddToggle("VortexSConfig", "Vortex Stealth", "VortexEnchant", "00f2aa");
            AddToggle("VortexVConfig", "Vortex Voids", "VortexEnchant", "00f2aa");

            #endregion enchants

            #region masomode toggles

            //Masomode Header
            AddToggle("MasoHeader", "Eternity Mode", "MutantStatue", "ffffff");
            AddToggle("MasoHeader2", "Eternity Mode Accessories", "DeviatingEnergy", "ffffff");
            //AddToggle("MasoBossBG", "Mutant Bright Background", "Masochist", "ffffff");
            AddToggle("MasoBossRecolors", "Boss Recolors (Toggle needs restart)", "Masochist", "ffffff");
            AddToggle("MasoAeolusConfig", "Aeolus Jump", "AeolusBoots", "ffffff");
            AddToggle("MasoIconConfig", "Sinister Icon Spawn Rates", "SinisterIcon", "ffffff");
            AddToggle("MasoIconDropsConfig", "Sinister Icon Drops", "SinisterIcon", "ffffff");
            AddToggle("MasoGrazeConfig", "Graze", "SparklingAdoration", "ffffff");
            AddToggle("MasoDevianttHeartsConfig", "Attacks Spawn Homing Hearts", "SparklingAdoration", "ffffff");

            //supreme death fairy header
            AddToggle("SupremeFairyHeader", "Supreme Deathbringer Fairy", "SupremeDeathbringerFairy", "ffffff");
            AddToggle("MasoSlimeConfig", "Slimy Balls", "SlimyShield", "ffffff");
            AddToggle("SlimeFallingConfig", "Increased Fall Speed", "SlimyShield", "ffffff");
            AddToggle("MasoEyeConfig", "Scythes When Dashing", "AgitatingLens", "ffffff");
            AddToggle("MasoHoneyConfig", "Honey When Hitting Enemies", "QueenStinger", "ffffff");
            AddToggle("MasoSkeleConfig", "Skeletron Arms Minion", "NecromanticBrew", "ffffff");

            //bionomic
            AddToggle("BionomicHeader", "Bionomic Cluster", "BionomicCluster", "ffffff");
            AddToggle("MasoConcoctionConfig", "Tim's Concoction", "TimsConcoction", "ffffff");
            AddToggle("MasoCarrotConfig", "Carrot View", "OrdinaryCarrot", "ffffff");
            AddToggle("MasoRainbowConfig", "Rainbow Slime Minion", "ConcentratedRainbowMatter", "ffffff");
            AddToggle("MasoFrigidConfig", "Frostfireballs", "FrigidGemstone", "ffffff");
            AddToggle("MasoNymphConfig", "Attacks Spawn Hearts", "NymphsPerfume", "ffffff");
            AddToggle("MasoSqueakConfig", "Squeaky Toy On Hit", "SqueakyToy", "ffffff");
            AddToggle("MasoPouchConfig", "Tentacles On Hit", "WretchedPouch", "ffffff");
            AddToggle("MasoClippedConfig", "Inflict Clipped Wings", "WyvernFeather", "ffffff");
            AddToggle("TribalCharmConfig", "Tribal Charm Auto Swing", "TribalCharm", "ffffff");
            //AddToggle("WalletHeader", "Security Wallet", "SecurityWallet", "ffffff");

            //dubious
            AddToggle("DubiousHeader", "Dubious Circuitry", "DubiousCircuitry", "ffffff");
            AddToggle("MasoLightningConfig", "Inflict Lightning Rod", "GroundStick", "ffffff");
            AddToggle("MasoProbeConfig", "Probes Minion", "GroundStick", "ffffff");

            //pure heart
            AddToggle("PureHeartHeader", "Pure Heart", "PureHeart", "ffffff");
            AddToggle("MasoEaterConfig", "Tiny Eaters", "CorruptHeart", "ffffff");
            AddToggle("MasoBrainConfig", "Creeper Shield", "GuttedHeart", "ffffff");

            //lump of flesh
            AddToggle("LumpofFleshHeader", "Lump of Flesh", "LumpOfFlesh", "ffffff");
            AddToggle("MasoPugentConfig", "Pungent Eye Minion", "LumpOfFlesh", "ffffff");

            //chalice
            AddToggle("ChaliceHeader", "Chalice of the Moon", "ChaliceoftheMoon", "ffffff");
            AddToggle("MasoCultistConfig", "Cultist Minion", "ChaliceoftheMoon", "ffffff");
            AddToggle("MasoPlantConfig", "Plantera Minion", "MagicalBulb", "ffffff");
            AddToggle("MasoGolemConfig", "Lihzahrd Ground Pound", "LihzahrdTreasureBox", "ffffff");
            AddToggle("MasoBoulderConfig", "Boulder Spray", "LihzahrdTreasureBox", "ffffff");
            AddToggle("MasoCelestConfig", "Celestial Rune Support", "CelestialRune", "ffffff");
            AddToggle("MasoVisionConfig", "Ancient Visions On Hit", "CelestialRune", "ffffff");

            //heart of the masochist
            AddToggle("HeartHeader", "Heart of the Eternal", "HeartoftheMasochist", "ffffff");
            AddToggle("MasoPumpConfig", "Pumpking's Cape Support", "PumpkingsCape", "ffffff");
            AddToggle("MasoFlockoConfig", "Flocko Minion", "IceQueensCrown", "ffffff");
            AddToggle("MasoUfoConfig", "Saucer Minion", "SaucerControlConsole", "ffffff");
            AddToggle("MasoGravConfig", "Gravity Control", "GalacticGlobe", "ffffff");
            AddToggle("MasoGrav2Config", "Stabilized Gravity", "GalacticGlobe", "ffffff");
            AddToggle("MasoTrueEyeConfig", "True Eyes Minion", "GalacticGlobe", "ffffff");

            //cyclonic fin
            AddToggle("CyclonicHeader", "Abominable Wand", "CyclonicFin", "ffffff");
            AddToggle("MasoFishronConfig", "Spectral Abominationn", "CyclonicFin", "ffffff");

            //mutant armor
            AddToggle("MutantArmorHeader", "True Mutant Armor", "HeartoftheMasochist", "ffffff");
            AddToggle("MasoAbomConfig", "Abominationn Minion", "MutantMask", "ffffff");
            AddToggle("MasoRingConfig", "Phantasmal Ring Minion", "MutantMask", "ffffff");
            AddToggle("MasoReviveDeathrayConfig", "Deathray When Revived", "MutantMask", "ffffff");

            #endregion masomode toggles

            #region soul toggles

            AddToggle("SoulHeader", "Souls", "UniverseSoul", "ffffff");
            AddToggle("MeleeConfig", "Melee Speed", "GladiatorsSoul", "ffffff");
            AddToggle("MagmaStoneConfig", "Magma Stone", "GladiatorsSoul", "ffffff");
            AddToggle("YoyoBagConfig", "Yoyo Bag", "GladiatorsSoul", "ffffff");
            AddToggle("MoonCharmConfig", "Moon Charm", "GladiatorsSoul", "ffffff");
            AddToggle("NeptuneShellConfig", "Neptune's Shell", "GladiatorsSoul", "ffffff");
            AddToggle("SniperConfig", "Sniper Scope", "SnipersSoul", "ffffff");
            AddToggle("UniverseConfig", "Universe Attack Speed", "UniverseSoul", "ffffff");
            AddToggle("MiningHuntConfig", "Mining Hunter Buff", "MinerEnchant", "ffffff");
            AddToggle("MiningDangerConfig", "Mining Dangersense Buff", "MinerEnchant", "ffffff");
            AddToggle("MiningSpelunkConfig", "Mining Spelunker Buff", "MinerEnchant", "ffffff");
            AddToggle("MiningShineConfig", "Mining Shine Buff", "MinerEnchant", "ffffff");
            AddToggle("BuilderConfig", "Builder Mode", "WorldShaperSoul", "ffffff");
            AddToggle("TrawlerSporeConfig", "Spore Sac", "TrawlerSoul", "ffffff");
            AddToggle("DefenseStarConfig", "Stars On Hit", "ColossusSoul", "ffffff");
            AddToggle("DefenseBeeConfig", "Bees On Hit", "ColossusSoul", "ffffff");
            AddToggle("DefensePanicConfig", "Panic On Hit", "ColossusSoul", "ffffff");
            AddToggle("DefenseFleshKnuckleConfig", "Flesh Knuckles Aggro", "ColossusSoul", "ffffff");
            AddToggle("RunSpeedConfig", "Higher Base Run Speed", "SupersonicSoul", "ffffff");
            AddToggle("MomentumConfig", "No Momentum", "SupersonicSoul", "ffffff");
            AddToggle("SupersonicConfig", "Supersonic Speed Boosts", "SupersonicSoul", "ffffff");
            AddToggle("SupersonicJumpsConfig", "Supersonic Jumps", "SupersonicSoul", "ffffff");
            AddToggle("SupersonicRocketBootsConfig", "Supersonic Rocket Boots", "SupersonicSoul", "ffffff");
            AddToggle("SupersonicCarpetConfig", "Supersonic Carpet", "SupersonicSoul", "ffffff");
            AddToggle("SupersonicFlowerConfig", "Flower Boots", "SupersonicSoul", "248900");
            AddToggle("CthulhuShieldConfig", "Shield of Cthulhu", "SupersonicSoul", "ffffff");
            AddToggle("BlackBeltConfig", "Black Belt", "SupersonicSoul", "ffffff");
            AddToggle("TrawlerConfig", "Trawler Extra Lures", "TrawlerSoul", "ffffff");
            AddToggle("TrawlerJumpConfig", "Trawler Jump", "TrawlerSoul", "ffffff");
            AddToggle("EternityConfig", "Eternity Stacking", "EternitySoul", "ffffff");

            #endregion soul toggles

            #region pet toggles

            AddToggle("PetHeader", "Pets", ItemID.ZephyrFish, "ffffff");
            AddToggle("PetBlackCatConfig", "Black Cat Pet", 1810, "ffffff");
            AddToggle("PetCompanionCubeConfig", "Companion Cube Pet", 3628, "ffffff");
            AddToggle("PetCursedSaplingConfig", "Cursed Sapling Pet", 1837, "ffffff");
            AddToggle("PetDinoConfig", "Dino Pet", 1242, "ffffff");
            AddToggle("PetDragonConfig", "Dragon Pet", 3857, "ffffff");
            AddToggle("PetEaterConfig", "Eater Pet", 994, "ffffff");
            AddToggle("PetEyeSpringConfig", "Eye Spring Pet", 1311, "ffffff");
            AddToggle("PetFaceMonsterConfig", "Face Monster Pet", 3060, "ffffff");
            AddToggle("PetGatoConfig", "Gato Pet", 3855, "ffffff");
            AddToggle("PetHornetConfig", "Hornet Pet", 1170, "ffffff");
            AddToggle("PetLizardConfig", "Lizard Pet", 1172, "ffffff");
            AddToggle("PetMinitaurConfig", "Mini Minotaur Pet", 2587, "ffffff");
            AddToggle("PetParrotConfig", "Parrot Pet", 1180, "ffffff");
            AddToggle("PetPenguinConfig", "Penguin Pet", 669, "ffffff");
            AddToggle("PetPupConfig", "Puppy Pet", 1927, "ffffff");
            AddToggle("PetSeedConfig", "Seedling Pet", 1182, "ffffff");
            AddToggle("PetDGConfig", "Skeletron Pet", 1169, "ffffff");
            AddToggle("PetSnowmanConfig", "Snowman Pet", 1312, "ffffff");
            AddToggle("PetGrinchConfig", "Grinch Pet", ItemID.BabyGrinchMischiefWhistle, "ffffff");
            AddToggle("PetSpiderConfig", "Spider Pet", 1798, "ffffff");
            AddToggle("PetSquashConfig", "Squashling Pet", 1799, "ffffff");
            AddToggle("PetTikiConfig", "Tiki Pet", 1171, "ffffff");
            AddToggle("PetShroomConfig", "Truffle Pet", 1181, "ffffff");
            AddToggle("PetTurtleConfig", "Turtle Pet", 753, "ffffff");
            AddToggle("PetZephyrConfig", "Zephyr Fish Pet", 2420, "ffffff");
            AddToggle("PetHeartConfig", "Crimson Heart Pet", 3062, "ffffff");
            AddToggle("PetNaviConfig", "Fairy Pet", 425, "ffffff");
            AddToggle("PetFlickerConfig", "Flickerwick Pet", 3856, "ffffff");
            AddToggle("PetLanternConfig", "Magic Lantern Pet", 3043, "ffffff");
            AddToggle("PetOrbConfig", "Shadow Orb Pet", 115, "ffffff");
            AddToggle("PetSuspEyeConfig", "Suspicious Eye Pet", 3577, "ffffff");
            AddToggle("PetWispConfig", "Wisp Pet", 1183, "ffffff");

            #endregion pet toggles

            #region patreon toggles
            AddToggle("PatreonHeader", "Patreon Items (Toggles need restart)", "RoombaPet", "ffffff");
            AddToggle("PatreonRoomba", "Roomba", "RoombaPet", "ffffff");
            AddToggle("PatreonOrb", "Computation Orb", "ComputationOrb", "ffffff");
            AddToggle("PatreonFishingRod", "Miss Drakovi's Fishing Pole", "MissDrakovisFishingPole", "ffffff");
            AddToggle("PatreonDoor", "Squidward Door", "SquidwardDoor", "ffffff");
            AddToggle("PatreonWolf", "Paradox Wolf Soul", "ParadoxWolfSoul", "ffffff");
            AddToggle("PatreonDove", "Fig Branch", "FigBranch", "ffffff");
            AddToggle("PatreonKingSlime", "Medallion of the Fallen King", "MedallionoftheFallenKing", "ffffff");
            AddToggle("PatreonFishron", "Staff Of Unleashed Ocean", "StaffOfUnleashedOcean", "ffffff");
            AddToggle("PatreonPlant", "Piranha Plant Voodoo Doll", "PiranhaPlantVoodooDoll", "ffffff");
            AddToggle("PatreonDevious", "Devious Aestheticus", "DeviousAestheticus", "ffffff");
            AddToggle("PatreonVortex", "Vortex Ritual", "VortexMagnetRitual", "ffffff");
            AddToggle("PatreonPrime", "prime", "DeviousAestheticus", "ffffff");
            #endregion patreon toggles

            #endregion Toggles

            if (Main.netMode != NetmodeID.Server)
            {
                #region shaders

                //loading refs for shaders
                Ref<Effect> lcRef = new Ref<Effect>(GetEffect("Effects/LifeChampionShader"));
                Ref<Effect> wcRef = new Ref<Effect>(GetEffect("Effects/WillChampionShader"));
                Ref<Effect> gaiaRef = new Ref<Effect>(GetEffect("Effects/GaiaShader"));
                Ref<Effect> textRef = new Ref<Effect>(GetEffect("Effects/TextShader"));
                Ref<Effect> invertRef = new Ref<Effect>(GetEffect("Effects/Invert"));
                Ref<Effect> shockwaveRef = new Ref<Effect>(GetEffect("Effects/ShockwaveEffect")); // The path to the compiled shader file.

                //loading shaders from refs
                GameShaders.Misc["LCWingShader"] = new MiscShaderData(lcRef, "LCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<LifeDye>(), new ArmorShaderData(lcRef, "LCArmor").UseColor(new Color(1f, 0.647f, 0.839f)).UseSecondaryColor(Color.Goldenrod));

                GameShaders.Misc["WCWingShader"] = new MiscShaderData(wcRef, "WCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<WillDye>(), new ArmorShaderData(wcRef, "WCArmor").UseColor(Color.DarkOrchid).UseSecondaryColor(Color.LightPink).UseImage("Images/Misc/Noise"));

                GameShaders.Misc["GaiaShader"] = new MiscShaderData(gaiaRef, "GaiaGlow");
                GameShaders.Armor.BindShader(ModContent.ItemType<GaiaDye>(), new ArmorShaderData(gaiaRef, "GaiaArmor").UseColor(new Color(0.44f, 1, 0.09f)).UseSecondaryColor(new Color(0.5f, 1f, 0.9f)));

                GameShaders.Misc["PulseUpwards"] = new MiscShaderData(textRef, "PulseUpwards");
                GameShaders.Misc["PulseDiagonal"] = new MiscShaderData(textRef, "PulseDiagonal");
                GameShaders.Misc["PulseCircle"] = new MiscShaderData(textRef, "PulseCircle");

                Filters.Scene["FargowiltasSouls:Invert"] = new Filter(new TimeStopShader(invertRef, "Main"), EffectPriority.VeryHigh);

                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(shockwaveRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();

                #endregion shaders
            }

            PatreonMiscMethods.Load(this);
        }

        public void AddToggle(string toggle, string name, string item, string color)
        {
            ModTranslation text = CreateTranslation(toggle);
            text.SetDefault($"[i:{ItemType(item)}] {name}");
            AddTranslation(text);
        }

        //for vanilla items reeeee
        public void AddToggle(string toggle, string name, int item, string color)
        {
            ModTranslation text = CreateTranslation(toggle);
            text.SetDefault($"[i:{item}] {name}");
            AddTranslation(text);
        }

        public override void Unload()
        {
            NPC.LunarShieldPowerExpert = 150;

            if (DebuffIDs != null)
                DebuffIDs.Clear();

            if (OldVolume > 0 && OldVolume > Main.musicVolume)
            {
                Main.musicVolume = OldVolume;
                OldVolume = 0;
            }

            //game will reload golem textures, this helps prevent the crash on reload
            Main.NPCLoaded[NPCID.Golem] = false;
            Main.NPCLoaded[NPCID.GolemFistLeft] = false;
            Main.NPCLoaded[NPCID.GolemFistRight] = false;
            Main.NPCLoaded[NPCID.GolemHead] = false;
            Main.NPCLoaded[NPCID.GolemHeadFree] = false;

            ToggleLoader.Unload();
        }

        public override object Call(params object[] args)
        {
            try
            {
                string code = args[0].ToString();

                switch (code)
                {
                    case "Masomode":
                    case "MasoMode":
                    case "MasochistMode":
                    case "Emode":
                    case "EMode":
                    case "EternityMode":
                        return FargoSoulsWorld.MasochistMode;

                    case "DownedMutant":
                        return FargoSoulsWorld.downedMutant;

                    case "DownedAbom":
                    case "DownedAbominationn":
                        return FargoSoulsWorld.downedAbom;

                    case "DownedChamp":
                    case "DownedChampion":
                        return FargoSoulsWorld.downedChampions[(int)args[1]];

                    case "DownedEri":
                    case "DownedEridanus":
                    case "DownedCosmos":
                    case "DownedCosmosChamp":
                    case "DownedCosmosChampion":
                        return FargoSoulsWorld.downedChampions[8];

                    case "DownedDevi":
                    case "DownedDeviantt":
                        return FargoSoulsWorld.downedDevi;

                    case "DownedFishronEX":
                        return FargoSoulsWorld.downedFishronEX;

                    case "PureHeart":
                        return Main.LocalPlayer.GetModPlayer<FargoPlayer>().PureHeart;

                    case "MutantAntibodies":
                        return Main.LocalPlayer.GetModPlayer<FargoPlayer>().MutantAntibodies;

                    case "SinisterIcon":
                        return Main.LocalPlayer.GetModPlayer<FargoPlayer>().SinisterIcon;

                    case "AbomAlive":
                        return EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>());

                    case "MutantAlive":
                        return EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>());

                    case "DevianttAlive":
                        return EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>());

                    case "MutantPact":
                        return Main.LocalPlayer.GetModPlayer<FargoPlayer>().MutantsPact;

                    case "MutantDiscountCard":
                        return Main.LocalPlayer.GetModPlayer<FargoPlayer>().MutantsDiscountCard;

                    /*case "DevianttGifts":

                        Player player = Main.LocalPlayer;
                        FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

                        if (!fargoPlayer.ReceivedMasoGift)
                        {
                            fargoPlayer.ReceivedMasoGift = true;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                DropDevianttsGift(player);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                var netMessage = GetPacket(); // Broadcast item request to server
                                netMessage.Write((byte)14);
                                netMessage.Write((byte)player.whoAmI);
                                netMessage.Send();
                            }

                            return true;
                        }

                        break;*/

                    case "GiftsReceived":
                        Player player = Main.LocalPlayer;
                        FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
                        return fargoPlayer.ReceivedMasoGift;

                    case "GiveDevianttGifts":
                        Player player2 = Main.LocalPlayer;
                        FargoPlayer fargoPlayer2 = player2.GetModPlayer<FargoPlayer>();
                        fargoPlayer2.ReceivedMasoGift = true;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            DropDevianttsGift(player2);
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = GetPacket(); // Broadcast item request to server
                            netMessage.Write((byte)14);
                            netMessage.Write((byte)player2.whoAmI);
                            netMessage.Send();
                        }

                        //Main.npcChatText = "This world looks tougher than usual, so you can have these on the house just this once! Talk to me if you need any tips, yeah?";

                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Call Error: " + e.StackTrace + e.Message);
            }

            return base.Call(args);
        }

        public static void DropDevianttsGift(Player player)
        {
            Item.NewItem(player.Center, ItemID.SilverPickaxe);
            Item.NewItem(player.Center, ItemID.SilverAxe);
            Item.NewItem(player.Center, ItemID.BugNet);
            Item.NewItem(player.Center, ItemID.LifeCrystal, 4);
            Item.NewItem(player.Center, ItemID.ManaCrystal, 4);
            Item.NewItem(player.Center, ItemID.RecallPotion, 15);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                Item.NewItem(player.Center, ItemID.WormholePotion, 15);
            }
            Item.NewItem(player.Center, ModContent.ItemType<DevianttsSundial>());
            Item.NewItem(player.Center, ModContent.ItemType<EternityAdvisor>());
            Item.NewItem(player.Center, ModContent.ItemType<AutoHouse>(), 3);
            Item.NewItem(player.Center, ModContent.ItemType<EurusSock>());
            Item.NewItem(player.Center, ModContent.ItemType<PuffInABottle>());

            //only give once per world
            if (ModLoader.GetMod("MagicStorage") != null && !FargoSoulsWorld.ReceivedTerraStorage)
            {
                Item.NewItem(player.Center, ModLoader.GetMod("MagicStorage").ItemType("StorageHeart"));
                Item.NewItem(player.Center, ModLoader.GetMod("MagicStorage").ItemType("CraftingAccess"));
                Item.NewItem(player.Center, ModLoader.GetMod("MagicStorage").ItemType("StorageUnit"), 16);

                FargoSoulsWorld.ReceivedTerraStorage = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData); //sync world in mp
            }
            else if (ModLoader.GetMod("MagicStorageExtra") != null && !FargoSoulsWorld.ReceivedTerraStorage)
            {
                Item.NewItem(player.Center, ModLoader.GetMod("MagicStorageExtra").ItemType("StorageHeart"));
                Item.NewItem(player.Center, ModLoader.GetMod("MagicStorageExtra").ItemType("CraftingAccess"));
                Item.NewItem(player.Center, ModLoader.GetMod("MagicStorageExtra").ItemType("StorageUnit"), 16);

                FargoSoulsWorld.ReceivedTerraStorage = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData); //sync world in mp
            }
        }

        //bool sheet
        public override void PostSetupContent()
        {
            try
            {
                CalamityCompatibility = new CalamityCompatibility(this).TryLoad() as CalamityCompatibility;
                ThoriumCompatibility = new ThoriumCompatibility(this).TryLoad() as ThoriumCompatibility;
                SoACompatibility = new SoACompatibility(this).TryLoad() as SoACompatibility;
                MasomodeEXCompatibility = new MasomodeEXCompatibility(this).TryLoad() as MasomodeEXCompatibility;
                BossChecklistCompatibility = (BossChecklistCompatibility)new BossChecklistCompatibility(this).TryLoad();

                if (BossChecklistCompatibility != null)
                    BossChecklistCompatibility.Initialize();
                
                DebuffIDs = new List<int> { BuffID.Bleeding, BuffID.OnFire, BuffID.Rabies, BuffID.Confused, BuffID.Weak, BuffID.BrokenArmor, BuffID.Darkness, BuffID.Slow, BuffID.Cursed, BuffID.Poisoned, BuffID.Silenced, 39, 44, 46, 47, 67, 68, 69, 70, 80,
                    88, 94, 103, 137, 144, 145, 149, 156, 160, 163, 164, 195, 196, 197, 199 };
                DebuffIDs.Add(BuffType("Antisocial"));
                DebuffIDs.Add(BuffType("Atrophied"));
                DebuffIDs.Add(BuffType("Berserked"));
                DebuffIDs.Add(BuffType("Bloodthirsty"));
                DebuffIDs.Add(BuffType("ClippedWings"));
                DebuffIDs.Add(BuffType("Crippled"));
                DebuffIDs.Add(BuffType("CurseoftheMoon"));
                DebuffIDs.Add(BuffType("Defenseless"));
                DebuffIDs.Add(BuffType("FlamesoftheUniverse"));
                DebuffIDs.Add(BuffType("Flipped"));
                DebuffIDs.Add(BuffType("FlippedHallow"));
                DebuffIDs.Add(BuffType("Fused"));
                DebuffIDs.Add(BuffType("GodEater"));
                DebuffIDs.Add(BuffType("Guilty"));
                DebuffIDs.Add(BuffType("Hexed"));
                DebuffIDs.Add(BuffType("HolyPrice"));
                DebuffIDs.Add(BuffType("Hypothermia"));
                DebuffIDs.Add(BuffType("Infested"));
                DebuffIDs.Add(BuffType("InfestedEX"));
                DebuffIDs.Add(BuffType("IvyVenom"));
                DebuffIDs.Add(BuffType("Jammed"));
                DebuffIDs.Add(BuffType("Lethargic"));
                DebuffIDs.Add(BuffType("LightningRod"));
                DebuffIDs.Add(BuffType("LihzahrdCurse"));
                DebuffIDs.Add(BuffType("LivingWasteland"));
                DebuffIDs.Add(BuffType("Lovestruck"));
                DebuffIDs.Add(BuffType("LowGround"));
                DebuffIDs.Add(BuffType("MarkedforDeath"));
                DebuffIDs.Add(BuffType("Midas"));
                DebuffIDs.Add(BuffType("MutantNibble"));
                DebuffIDs.Add(BuffType("NanoInjection"));
                DebuffIDs.Add(BuffType("NullificationCurse"));
                DebuffIDs.Add(BuffType("OceanicMaul"));
                DebuffIDs.Add(BuffType("OceanicSeal"));
                DebuffIDs.Add(BuffType("Oiled"));
                DebuffIDs.Add(BuffType("Purified"));
                DebuffIDs.Add(BuffType("Recovering"));
                DebuffIDs.Add(BuffType("ReverseManaFlow"));
                DebuffIDs.Add(BuffType("Rotting"));
                DebuffIDs.Add(BuffType("Shadowflame"));
                DebuffIDs.Add(BuffType("SqueakyToy"));
                DebuffIDs.Add(BuffType("Stunned"));
                DebuffIDs.Add(BuffType("Swarming"));
                DebuffIDs.Add(BuffType("Unstable"));

                DebuffIDs.Add(BuffType("AbomFang"));
                DebuffIDs.Add(BuffType("AbomPresence"));
                DebuffIDs.Add(BuffType("MutantFang"));
                DebuffIDs.Add(BuffType("MutantPresence"));

                DebuffIDs.Add(BuffType("AbomRebirth"));

                DebuffIDs.Add(BuffType("TimeFrozen"));

                Mod bossHealthBar = ModLoader.GetMod("FKBossHealthBar");
                if (bossHealthBar != null)
                {
                    //bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<BabyGuardian>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TimberChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TimberChampionHead>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<EarthChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<LifeChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<WillChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<ShadowChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<SpiritChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TerraChampion>());
                    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<NatureChampion>());

                    bossHealthBar.Call("hbStart");
                    bossHealthBar.Call("hbSetColours", new Color(1f, 1f, 1f), new Color(1f, 1f, 0f), new Color(1f, 0f, 0f));
                    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<CosmosChampion>());

                    bossHealthBar.Call("hbStart");
                    bossHealthBar.Call("hbSetColours", new Color(1f, 0f, 1f), new Color(1f, 0.2f, 0.6f), new Color(1f, 0f, 0f));
                    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<DeviBoss>());

                    bossHealthBar.Call("RegisterDD2HealthBar", ModContent.NPCType<AbomBoss>());

                    bossHealthBar.Call("hbStart");
                    bossHealthBar.Call("hbSetColours", new Color(0f, 1f, 0f), new Color(0f, 1f, 1f), new Color(0f, 0.6f, 1f));
                    //bossHealthBar.Call("hbSetBossHeadTexture", GetTexture("NPCs/MutantBoss/MutantBoss_Head_Boss"));
                    bossHealthBar.Call("hbSetTexture",
                        bossHealthBar.GetTexture("UI/MoonLordBarStart"), null,
                        bossHealthBar.GetTexture("UI/MoonLordBarEnd"), null);
                    bossHealthBar.Call("hbSetTextureExpert",
                        bossHealthBar.GetTexture("UI/MoonLordBarStart_Exp"), null,
                        bossHealthBar.GetTexture("UI/MoonLordBarEnd_Exp"), null);
                    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<MutantBoss>());
                }

                //mutant shop
                Mod fargos = ModLoader.GetMod("Fargowiltas");
                fargos.Call("AddSummon", 5.01f, "FargowiltasSouls", "DevisCurse", (Func<bool>)(() => FargoSoulsWorld.downedDevi), Item.buyPrice(0, 17, 50));
                fargos.Call("AddSummon", 14.01f, "FargowiltasSouls", "AbomsCurse", (Func<bool>)(() => FargoSoulsWorld.downedAbom), 10000000);
                fargos.Call("AddSummon", 14.02f, "FargowiltasSouls", "TruffleWormEX", (Func<bool>)(() => FargoSoulsWorld.downedFishronEX), 10000000);
                fargos.Call("AddSummon", 14.03f, "FargowiltasSouls", "MutantsCurse", (Func<bool>)(() => FargoSoulsWorld.downedMutant), 20000000);
            }
            catch (Exception e)
            {
                Logger.Warn("FargowiltasSouls PostSetupContent Error: " + e.StackTrace + e.Message);
            }
        }
        static float ColorTimer;
        public static Color EModeColor()
        {
            Color mutantColor = new Color(28, 222, 152);
            Color abomColor = new Color(255, 224, 53);
            Color deviColor = new Color(255, 51, 153);
            ColorTimer += 0.5f;
            if (ColorTimer >= 300)
            {
                ColorTimer = 0;
            }

            if (ColorTimer < 100)
                return Color.Lerp(mutantColor, abomColor, ColorTimer / 100);
            else if (ColorTimer < 200)
                return Color.Lerp(abomColor, deviColor, (ColorTimer - 100) / 100);
            else
                return Color.Lerp(deviColor, mutantColor, (ColorTimer - 200) / 100);
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.SoulofLight, 7);
            recipe.AddIngredient(ItemID.SoulofNight, 7);
            recipe.AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ModContent.ItemType<JungleChest>());
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.WizardHat);
            recipe.AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ModContent.ItemType<RuneOrb>());
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.LifeCrystal);
            recipe.AddTile(TileID.CookingPots);
            recipe.SetResult(ModContent.ItemType<HeartChocolate>());
            recipe.AddRecipe();

            /*recipe = new ModRecipe(this);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyBonesBanner", 2);
            recipe.AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ModContent.ItemType<InnocuousSkull>());
            recipe.AddRecipe();*/
        }

        public override void AddRecipeGroups()
        {
            //drax
            RecipeGroup group = new RecipeGroup(() => Lang.misc[37] + " Drax", ItemID.Drax, ItemID.PickaxeAxe);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyDrax", group);

            //dungeon enemies
            group = new RecipeGroup(() => Lang.misc[37] + " Angry or Armored Bones Banner", ItemID.AngryBonesBanner, ItemID.BlueArmoredBonesBanner, ItemID.HellArmoredBonesBanner, ItemID.RustyArmoredBonesBanner);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBonesBanner", group);

            //cobalt
            group = new RecipeGroup(() => Lang.misc[37] + " Cobalt Repeater", ItemID.CobaltRepeater, ItemID.PalladiumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltRepeater", group);

            //mythril
            group = new RecipeGroup(() => Lang.misc[37] + " Mythril Repeater", ItemID.MythrilRepeater, ItemID.OrichalcumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilRepeater", group);

            //adamantite
            group = new RecipeGroup(() => Lang.misc[37] + " Adamantite Repeater", ItemID.AdamantiteRepeater, ItemID.TitaniumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantiteRepeater", group);

            //evil wood
            group = new RecipeGroup(() => Lang.misc[37] + " Evil Wood", ItemID.Ebonwood, ItemID.Shadewood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyEvilWood", group);

            //any adamantite
            group = new RecipeGroup(() => Lang.misc[37] + " Adamantite Bar", ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantite", group);

            //shroomite head
            group = new RecipeGroup(() => Lang.misc[37] + " Shroomite Head Piece", ItemID.ShroomiteHeadgear, ItemID.ShroomiteMask, ItemID.ShroomiteHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyShroomHead", group);

            //orichalcum head
            group = new RecipeGroup(() => Lang.misc[37] + " Orichalcum Head Piece", ItemID.OrichalcumHeadgear, ItemID.OrichalcumMask, ItemID.OrichalcumHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyOriHead", group);

            //palladium head
            group = new RecipeGroup(() => Lang.misc[37] + " Palladium Head Piece", ItemID.PalladiumHeadgear, ItemID.PalladiumMask, ItemID.PalladiumHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPallaHead", group);

            //cobalt head
            group = new RecipeGroup(() => Lang.misc[37] + " Cobalt Head Piece", ItemID.CobaltHelmet, ItemID.CobaltHat, ItemID.CobaltMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltHead", group);

            //mythril head
            group = new RecipeGroup(() => Lang.misc[37] + " Mythril Head Piece", ItemID.MythrilHat, ItemID.MythrilHelmet, ItemID.MythrilHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilHead", group);

            //titanium head
            group = new RecipeGroup(() => Lang.misc[37] + " Titanium Head Piece", ItemID.TitaniumHeadgear, ItemID.TitaniumMask, ItemID.TitaniumHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyTitaHead", group);

            //hallowed head
            group = new RecipeGroup(() => Lang.misc[37] + " Hallowed Head Piece", ItemID.HallowedMask, ItemID.HallowedHeadgear, ItemID.HallowedHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyHallowHead", group);

            //adamantite head
            group = new RecipeGroup(() => Lang.misc[37] + " Adamantite Head Piece", ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamHead", group);

            //chloro head
            group = new RecipeGroup(() => Lang.misc[37] + " Chlorophyte Head Piece", ItemID.ChlorophyteMask, ItemID.ChlorophyteHelmet, ItemID.ChlorophyteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyChloroHead", group);

            //spectre head
            group = new RecipeGroup(() => Lang.misc[37] + " Spectre Head Piece", ItemID.SpectreHood, ItemID.SpectreMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySpectreHead", group);

            //beetle body
            group = new RecipeGroup(() => Lang.misc[37] + " Beetle Body", ItemID.BeetleShell, ItemID.BeetleScaleMail);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBeetle", group);

            //phasesabers
            group = new RecipeGroup(() => Lang.misc[37] + " Phasesaber", ItemID.RedPhasesaber, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.WhitePhasesaber,
                ItemID.YellowPhasesaber);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPhasesaber", group);

            //vanilla butterflies
            group = new RecipeGroup(() => Lang.misc[37] + " Butterfly", ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly,
                ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyButterfly", group);

            //vanilla squirrels
            group = new RecipeGroup(() => Lang.misc[37] + " Squirrel", ItemID.Squirrel, ItemID.SquirrelRed);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySquirrel", group);

            //vanilla squirrels
            group = new RecipeGroup(() => Lang.misc[37] + " Common Fish", ItemID.AtlanticCod, ItemID.Bass, ItemID.Trout, ItemID.RedSnapper, ItemID.Salmon, ItemID.Tuna);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCommonFish", group);

            //vanilla birds
            group = new RecipeGroup(() => Lang.misc[37] + " Bird", ItemID.Bird, ItemID.BlueJay, ItemID.Cardinal, ItemID.GoldBird, ItemID.Duck, ItemID.MallardDuck);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBird", group);

            //vanilla scorpions
            group = new RecipeGroup(() => Lang.misc[37] + " Scorpion", ItemID.Scorpion, ItemID.BlackScorpion);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyScorpion", group);

            //gold pick
            group = new RecipeGroup(() => Lang.misc[37] + " Gold Pickaxe", ItemID.GoldPickaxe, ItemID.PlatinumPickaxe);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldPickaxe", group);

            //fish trash
            group = new RecipeGroup(() => Lang.misc[37] + " Fishing Trash", ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyFishingTrash", group);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            switch (reader.ReadByte())
            {
                case 0: //server side spawning creepers
                    if (Main.netMode == NetmodeID.Server)
                    {
                        byte p = reader.ReadByte();
                        int multiplier = reader.ReadByte();
                        int n = NPC.NewNPC((int)Main.player[p].Center.X, (int)Main.player[p].Center.Y, NPCType("CreeperGutted"), 0,
                            p, 0f, multiplier, 0f);
                        if (n != 200)
                        {
                            Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    break;

                case 1: //server side synchronize pillar data request
                    if (Main.netMode == NetmodeID.Server)
                    {
                        byte pillar = reader.ReadByte();
                        if (!Main.npc[pillar].GetGlobalNPC<EModeGlobalNPC>().masoBool[1])
                        {
                            Main.npc[pillar].GetGlobalNPC<EModeGlobalNPC>().masoBool[1] = true;
                            Main.npc[pillar].GetGlobalNPC<EModeGlobalNPC>().SetDefaults(Main.npc[pillar]);
                            Main.npc[pillar].life = Main.npc[pillar].lifeMax;
                        }
                    }
                    break;

                case 2: //net updating maso
                    EModeGlobalNPC fargoNPC = Main.npc[reader.ReadByte()].GetGlobalNPC<EModeGlobalNPC>();
                    fargoNPC.masoBool[0] = reader.ReadBoolean();
                    fargoNPC.masoBool[1] = reader.ReadBoolean();
                    fargoNPC.masoBool[2] = reader.ReadBoolean();
                    fargoNPC.masoBool[3] = reader.ReadBoolean();
                    fargoNPC.Counter[0] = reader.ReadInt32();
                    fargoNPC.Counter[1] = reader.ReadInt32();
                    fargoNPC.Counter[2] = reader.ReadInt32();
                    fargoNPC.Counter[3] = reader.ReadInt32();
                    break;

                case 3: //rainbow slime/paladin, MP clients syncing to server
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        byte npc = reader.ReadByte();
                        Main.npc[npc].lifeMax = reader.ReadInt32();
                        float newScale = reader.ReadSingle();
                        Main.npc[npc].position = Main.npc[npc].Center;
                        Main.npc[npc].width = (int)(Main.npc[npc].width / Main.npc[npc].scale * newScale);
                        Main.npc[npc].height = (int)(Main.npc[npc].height / Main.npc[npc].scale * newScale);
                        Main.npc[npc].scale = newScale;
                        Main.npc[npc].Center = Main.npc[npc].position;
                    }
                    break;

                case 4: //moon lord vulnerability synchronization
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int ML = reader.ReadByte();
                        Main.npc[ML].GetGlobalNPC<EModeGlobalNPC>().Counter[0] = reader.ReadInt32();
                        EModeGlobalNPC.masoStateML = reader.ReadByte();
                    }
                    break;

                case 5: //retinazer laser MP sync
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int reti = reader.ReadByte();
                        Main.npc[reti].GetGlobalNPC<EModeGlobalNPC>().masoBool[2] = reader.ReadBoolean();
                        Main.npc[reti].GetGlobalNPC<EModeGlobalNPC>().Counter[0] = reader.ReadInt32();
                    }
                    break;

                case 6: //shark MP sync
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int shark = reader.ReadByte();
                        Main.npc[shark].GetGlobalNPC<EModeGlobalNPC>().SharkCount = reader.ReadByte();
                    }
                    break;

                case 7: //client to server activate dark caster family
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int caster = reader.ReadByte();
                        if (Main.npc[caster].GetGlobalNPC<EModeGlobalNPC>().Counter[1] == 0)
                            Main.npc[caster].GetGlobalNPC<EModeGlobalNPC>().Counter[1] = reader.ReadInt32();
                    }
                    break;

                case 8: //server to clients reset counter
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int caster = reader.ReadByte();
                        Main.npc[caster].GetGlobalNPC<EModeGlobalNPC>().Counter[1] = 0;
                    }
                    break;

                case 9: //client to server, request heart spawn
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int n = reader.ReadByte();
                        Item.NewItem(Main.npc[n].Hitbox, ItemID.Heart);
                    }
                    break;

                case 10: //client to server, sync cultist data
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int cult = reader.ReadByte();
                        EModeGlobalNPC cultNPC = Main.npc[cult].GetGlobalNPC<EModeGlobalNPC>();
                        cultNPC.Counter[0] += reader.ReadInt32();
                        cultNPC.Counter[1] += reader.ReadInt32();
                        cultNPC.Counter[2] += reader.ReadInt32();
                        Main.npc[cult].localAI[3] += reader.ReadSingle();
                    }
                    break;

                case 11: //refresh creeper
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        byte player = reader.ReadByte();
                        NPC creeper = Main.npc[reader.ReadByte()];
                        if (creeper.active && creeper.type == NPCType("CreeperGutted") && creeper.ai[0] == player)
                        {
                            int damage = creeper.lifeMax - creeper.life;
                            creeper.life = creeper.lifeMax;
                            if (damage > 0)
                                CombatText.NewText(creeper.Hitbox, CombatText.HealLife, damage);
                            if (Main.netMode == NetmodeID.Server)
                                creeper.netUpdate = true;
                        }
                    }
                    break;

                case 12: //prime limbs spin
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int n = reader.ReadByte();
                        EModeGlobalNPC limb = Main.npc[n].GetGlobalNPC<EModeGlobalNPC>();
                        limb.masoBool[2] = reader.ReadBoolean();
                        limb.Counter[0] = reader.ReadInt32();
                        Main.npc[n].localAI[3] = reader.ReadSingle();
                    }
                    break;

                case 13: //prime limbs swipe
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int n = reader.ReadByte();
                        EModeGlobalNPC limb = Main.npc[n].GetGlobalNPC<EModeGlobalNPC>();
                        limb.Counter[0] = reader.ReadInt32();
                        limb.Counter[1] = reader.ReadInt32();
                    }
                    break;

                case 14: //devi gifts
                    if (Main.netMode == NetmodeID.Server)
                    {
                        Player player = Main.player[reader.ReadByte()];
                        DropDevianttsGift(player);
                    }
                    break;

                case 15: //sync npc counter array
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int n = reader.ReadByte();
                        EModeGlobalNPC eNPC = Main.npc[n].GetGlobalNPC<EModeGlobalNPC>();
                        for (int i = 0; i < eNPC.Counter.Length; i++)
                            eNPC.Counter[i] = reader.ReadInt32();
                    }
                    break;

                case 16: //client requesting a client side item from server
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int p = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int netID = reader.ReadInt32();
                        byte prefix = reader.ReadByte();
                        int stack = reader.ReadInt32();

                        int i = Item.NewItem(Main.player[p].Hitbox, type, stack, true, prefix);
                        Main.itemLockoutTime[i] = 54000;

                        var netMessage = GetPacket();
                        netMessage.Write((byte)17);
                        netMessage.Write(p);
                        netMessage.Write(type);
                        netMessage.Write(netID);
                        netMessage.Write(prefix);
                        netMessage.Write(stack);
                        netMessage.Write(i);
                        netMessage.Send();

                        Main.item[i].active = false;
                    }
                    break;

                case 17: //client-server handshake, spawn client-side item
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int p = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int netID = reader.ReadInt32();
                        byte prefix = reader.ReadByte();
                        int stack = reader.ReadInt32();
                        int i = reader.ReadInt32();

                        if (Main.myPlayer == p)
                        {
                            Main.item[i].netDefaults(netID);

                            Main.item[i].active = true;
                            Main.item[i].spawnTime = 0;
                            Main.item[i].owner = p;

                            Main.item[i].Prefix(prefix);
                            Main.item[i].stack = stack;
                            Main.item[i].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[i].noGrabDelay = 100;
                            Main.item[i].newAndShiny = false;

                            Main.item[i].position = Main.player[p].position;
                            Main.item[i].position.X += Main.rand.NextFloat(Main.player[p].Hitbox.Width);
                            Main.item[i].position.Y += Main.rand.NextFloat(Main.player[p].Hitbox.Height);
                        }
                    }
                    break;

                case 18: //client to server, requesting pillar sync
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int n = reader.ReadByte();
                        int type = reader.ReadInt32();
                        if (Main.npc[n].active && Main.npc[n].type == type)
                        {
                            Main.npc[n].GetGlobalNPC<EModeGlobalNPC>().NetUpdateMaso(n);
                        }
                    }
                    break;

                /*case 19: //client to all others, synchronize extra updates
                    {
                        int p = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int extraUpdates = reader.ReadInt32();
                        if (Main.projectile[p].active && Main.projectile[p].type == type)
                            Main.projectile[p].extraUpdates = extraUpdates;
                    }
                    break;*/

                case 77: //server side spawning fishron EX
                    if (Main.netMode == NetmodeID.Server)
                    {
                        byte target = reader.ReadByte();
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        EModeGlobalNPC.spawnFishronEX = true;
                        NPC.NewNPC(x, y, NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, target);
                        EModeGlobalNPC.spawnFishronEX = false;
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Duke Fishron EX has awoken!"), new Color(50, 100, 255));
                    }
                    break;

                case 78: //confirming fish EX max life
                    {
                        int f = reader.ReadInt32();
                        Main.npc[f].lifeMax = reader.ReadInt32();
                    }
                    break;

                case 79: //sync toggles on join
                    {
                        Player player = Main.player[reader.ReadByte()];
                        FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
                        byte count = reader.ReadByte();
                        List<string> keys = ToggleLoader.LoadedToggles.Keys.ToList();

                        for (int i = 0; i < count; i++)
                        {
                            modPlayer.Toggler.Toggles[keys[i]].ToggleBool = reader.ReadBoolean();
                        }
                    }
                    break;

                case 80: //sync single toggle
                    {
                        Player player = Main.player[reader.ReadByte()];
                        player.SetToggleValue(reader.ReadString(), reader.ReadBoolean());
                    }
                    break;

                default:
                    break;
            }

            //BaseMod Stuff
            /*MsgType msg = (MsgType)reader.ReadByte();
            if (msg == MsgType.ProjectileHostility) //projectile hostility and ownership
            {
                int owner = reader.ReadInt32();
                int projID = reader.ReadInt32();
                bool friendly = reader.ReadBoolean();
                bool hostile = reader.ReadBoolean();
                if (Main.projectile[projID] != null)
                {
                    Main.projectile[projID].owner = owner;
                    Main.projectile[projID].friendly = friendly;
                    Main.projectile[projID].hostile = hostile;
                }
                if (Main.netMode == NetmodeID.Server) MNet.SendBaseNetMessage(0, owner, projID, friendly, hostile);
            }
            else
            if (msg == MsgType.SyncAI) //sync AI array
            {
                int classID = reader.ReadByte();
                int id = reader.ReadInt16();
                int aitype = reader.ReadByte();
                int arrayLength = reader.ReadByte();
                float[] newAI = new float[arrayLength];
                for (int m = 0; m < arrayLength; m++)
                {
                    newAI[m] = reader.ReadSingle();
                }
                if (classID == 0 && Main.npc[id] != null && Main.npc[id].active && Main.npc[id].modNPC != null && Main.npc[id].modNPC is ParentNPC)
                {
                    ((ParentNPC)Main.npc[id].modNPC).SetAI(newAI, aitype);
                }
                else
                if (classID == 1 && Main.projectile[id] != null && Main.projectile[id].active && Main.projectile[id].modProjectile != null && Main.projectile[id].modProjectile is ParentProjectile)
                {
                    ((ParentProjectile)Main.projectile[id].modProjectile).SetAI(newAI, aitype);
                }
                if (Main.netMode == NetmodeID.Server) BaseNet.SyncAI(classID, id, newAI, aitype);
            }*/
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.musicVolume != 0 && Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
            {
                if (MMWorld.MMArmy && priority <= MusicPriority.Environment)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MonsterMadhouse");
                    priority = MusicPriority.Event;
                }
                /*if (FargoSoulsGlobalNPC.BossIsAlive(ref FargoSoulsGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
                    && Main.player[Main.myPlayer].Distance(Main.npc[FargoSoulsGlobalNPC.mutantBoss].Center) < 3000)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/SteelRed");
                    priority = (MusicPriority)12;
                }*/
            }
        }

        public static bool NoInvasion(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.invasion && (!Main.pumpkinMoon && !Main.snowMoon || spawnInfo.spawnTileY > Main.worldSurface || Main.dayTime) &&
                   (!Main.eclipse || spawnInfo.spawnTileY > Main.worldSurface || !Main.dayTime);
        }

        public static bool NoBiome(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.player;
            return !player.ZoneJungle && !player.ZoneDungeon && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneSnow && !player.ZoneUndergroundDesert;
        }

        public static bool NoZoneAllowWater(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.sky && !spawnInfo.player.ZoneMeteor && !spawnInfo.spiderCave;
        }

        public static bool NoZone(NPCSpawnInfo spawnInfo)
        {
            return NoZoneAllowWater(spawnInfo) && !spawnInfo.water;
        }

        public static bool NormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.playerInTown && NoInvasion(spawnInfo);
        }

        public static bool NoZoneNormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return NormalSpawn(spawnInfo) && NoZone(spawnInfo);
        }

        public static bool NoZoneNormalSpawnAllowWater(NPCSpawnInfo spawnInfo)
        {
            return NormalSpawn(spawnInfo) && NoZoneAllowWater(spawnInfo);
        }

        public static bool NoBiomeNormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return NormalSpawn(spawnInfo) && NoBiome(spawnInfo) && NoZone(spawnInfo);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            UserInterfaceManager.UpdateUI(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
            UserInterfaceManager.ModifyInterfaceLayers(layers);
        }
    }

    internal enum MsgType : byte
    {
        ProjectileHostility,
        SyncAI
    }
}