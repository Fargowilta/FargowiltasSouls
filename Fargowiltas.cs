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
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs.AbomBoss;
using FargowiltasSouls.NPCs.Champions;
using FargowiltasSouls.NPCs.DeviBoss;
using FargowiltasSouls.NPCs.MutantBoss;
using FargowiltasSouls.Sky;
using Fargowiltas.Items.Summons.Deviantt;
using Fargowiltas.Items.Misc;
using Fargowiltas.Items.Explosives;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Items.Dyes;
using FargowiltasSouls.Toggler;
using System.Linq;
using FargowiltasSouls.Patreon;
using FargowiltasSouls.EternityMode;

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

        internal static float OldMusicFade;

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

            // Load EModeNPCMods
            foreach (Type type in Code.GetTypes().OrderBy(type => type.FullName, StringComparer.InvariantCulture))
            {
                if (type.IsSubclassOf(typeof(EModeNPCBehaviour)) && !type.IsAbstract)
                {
                    EModeNPCBehaviour mod = (EModeNPCBehaviour)Activator.CreateInstance(type);
                    mod.Load();
                }
            }

            // Just to make sure they're always in the same order
            EModeNPCBehaviour.AllEModeNpcBehaviours.OrderBy(m => m.GetType().FullName, StringComparer.InvariantCulture);

            SkyManager.Instance["FargowiltasSouls:AbomBoss"] = new AbomSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss2"] = new MutantSky2();

            FreezeKey = RegisterHotKey(Language.GetTextValue("Mods.FargowiltasSouls.HotKey.Freeze"), "P");//"Freeze Time"
            GoldKey = RegisterHotKey(Language.GetTextValue("Mods.FargowiltasSouls.HotKey.Gold"), "O");//"Turn Gold"
            SmokeBombKey = RegisterHotKey(Language.GetTextValue("Mods.FargowiltasSouls.HotKey.SmokeBomb"), "I");//"Throw Smoke Bomb"
            BetsyDashKey = RegisterHotKey(Language.GetTextValue("Mods.FargowiltasSouls.HotKey.BetsyDash"), "C");//"Fireball Dash"
            MutantBombKey = RegisterHotKey(Language.GetTextValue("Mods.FargowiltasSouls.HotKey.MutantBomb"), "Z");//"Mutant Bomb"
            SoulToggleKey = RegisterHotKey(Language.GetTextValue("Mods.FargowiltasSouls.HotKey.SoulToggle"), ".");//"Open Soul Toggler"

            ToggleLoader.Load();

            _userInterfaceManager = new UIManager();
            _userInterfaceManager.LoadUI();

            #region Toggles

            AddToggle("PresetHeader", "{$Mods.FargowiltasSouls.Toggle.PresetConfigs}", "Masochist", "ffffff");

            #region enchants

            AddToggle("WoodHeader", "{$Mods.FargowiltasSouls.Toggle.Timber}", "TimberForce", "ffffff");//"Force of Timber"
            AddToggle("BorealConfig", "{$Mods.FargowiltasSouls.Toggle.Boreal}", "BorealWoodEnchant", "8B7464");//"Boreal Snowballs"
            AddToggle("MahoganyConfig", "{$Mods.FargowiltasSouls.Toggle.Mahogany}", "RichMahoganyEnchant", "b56c64");//"Mahogany Hook Speed"
            AddToggle("EbonConfig", "{$Mods.FargowiltasSouls.Toggle.Ebon}", "EbonwoodEnchant", "645a8d");//"Ebonwood Shadowflame"
            AddToggle("ShadeConfig", "{$Mods.FargowiltasSouls.Toggle.Shade1}", "ShadewoodEnchant", "586876");//"Blood Geyser On Hit"
            AddToggle("ShadeOnHitConfig", "{$Mods.FargowiltasSouls.Toggle.Shade2}", "ShadewoodEnchant", "586876");//"Proximity Triggers On Hit Effects"
            AddToggle("PalmConfig", "{$Mods.FargowiltasSouls.Toggle.Palm}", "PalmWoodEnchant", "b78d56");//"Palmwood Sentry"
            AddToggle("PearlConfig", "{$Mods.FargowiltasSouls.Toggle.Pearl}", "PearlwoodEnchant", "ad9a5f");//"Pearlwood Rain"

            AddToggle("EarthHeader", "{$Mods.FargowiltasSouls.Toggle.Earth}", "EarthForce", "ffffff");//"Force of Earth"
            AddToggle("AdamantiteConfig", "{$Mods.FargowiltasSouls.Toggle.Adamantite}", "AdamantiteEnchant", "dd557d");//"Adamantite Projectile Splitting"
            AddToggle("CobaltConfig", "{$Mods.FargowiltasSouls.Toggle.Cobalt}", "CobaltEnchant", "3da4c4");//"Cobalt Shards"
            AddToggle("AncientCobaltConfig", "{$Mods.FargowiltasSouls.Toggle.AncientCobalt}", "AncientCobaltEnchant", "354c74");//"Ancient Cobalt Stingers"
            AddToggle("MythrilConfig", "{$Mods.FargowiltasSouls.Toggle.Mythril}", "MythrilEnchant", "9dd290");//"Mythril Weapon Speed"
            AddToggle("OrichalcumConfig", "{$Mods.FargowiltasSouls.Toggle.Orichalcum}", "OrichalcumEnchant", "eb3291");//"Orichalcum Petals"
            AddToggle("PalladiumConfig", "{$Mods.FargowiltasSouls.Toggle.Palladium1}", "PalladiumEnchant", "f5ac28");//"Palladium Healing"
            AddToggle("PalladiumOrbConfig", "{$Mods.FargowiltasSouls.Toggle.Palladium2}", "PalladiumEnchant", "f5ac28");//"Palladium Orbs"
            AddToggle("TitaniumConfig", "{$Mods.FargowiltasSouls.Toggle.Titanium}", "TitaniumEnchant", "828c88");//"Titanium Shadow Dodge"

            AddToggle("TerraHeader", "{$Mods.FargowiltasSouls.Toggle.Terra}", "TerraForce", "ffffff");//"Terra Force"
            AddToggle("CopperConfig", "{$Mods.FargowiltasSouls.Toggle.Copper}", "CopperEnchant", "d56617");//"Copper Lightning"
            AddToggle("IronMConfig", "{$Mods.FargowiltasSouls.Toggle.Iron1}", "IronEnchant", "988e83");//"Iron Magnet"
            AddToggle("IronSConfig", "{$Mods.FargowiltasSouls.Toggle.Iron2}", "IronEnchant", "988e83");//"Iron Shield"
            AddToggle("TinConfig", "{$Mods.FargowiltasSouls.Toggle.Tin}", "TinEnchant", "a28b4e");//"Tin Crits"
            AddToggle("TungstenConfig", "{$Mods.FargowiltasSouls.Toggle.Tungsten1}", "TungstenEnchant", "b0d2b2");//"Tungsten Item Effect"
            AddToggle("TungstenProjConfig", "{$Mods.FargowiltasSouls.Toggle.Tungsten2}", "TungstenEnchant", "b0d2b2");//"Tungsten Projectile Effect"
            AddToggle("ObsidianConfig", "{$Mods.FargowiltasSouls.Toggle.Obsidian}", "ObsidianEnchant", "453e73");//"Obsidian Explosions"

            AddToggle("WillHeader", "{$Mods.FargowiltasSouls.Toggle.Will}", "WillForce", "ffffff");//"Force of Will"
            AddToggle("GladiatorConfig", "{$Mods.FargowiltasSouls.Toggle.Gladiator}", "GladiatorEnchant", "9c924e");//"Gladiator Rain"
            AddToggle("GoldConfig", "{$Mods.FargowiltasSouls.Toggle.Gold}", "GoldEnchant", "e7b21c");//"Gold Lucky Coin"
            AddToggle("HuntressConfig", "{$Mods.FargowiltasSouls.Toggle.Huntress}", "HuntressEnchant", "7ac04c");//"Huntress Ability"
            AddToggle("ValhallaConfig", "{$Mods.FargowiltasSouls.Toggle.Valhalla}", "ValhallaKnightEnchant", "93651e");//"Squire/Valhalla Healing"
            AddToggle("SquirePanicConfig", "{$Mods.FargowiltasSouls.Toggle.Squire}", "SquireEnchant", "948f8c");//"Ballista Panic On Hit"

            AddToggle("LifeHeader", "{$Mods.FargowiltasSouls.Toggle.Life}", "LifeForce", "ffffff");//"Force of Life"
            AddToggle("BeeConfig", "{$Mods.FargowiltasSouls.Toggle.Bee}", "BeeEnchant", "FEF625");//"Bees"
            AddToggle("BeetleConfig", "{$Mods.FargowiltasSouls.Toggle.Beetle}", "BeetleEnchant", "6D5C85");//"Beetles"
            AddToggle("CactusConfig", "{$Mods.FargowiltasSouls.Toggle.Cactus}", "CactusEnchant", "799e1d");//"Cactus Needles"
            AddToggle("PumpkinConfig", "{$Mods.FargowiltasSouls.Toggle.Pumpkin}", "PumpkinEnchant", "e3651c");//"Grow Pumpkins"
            AddToggle("SpiderConfig", "{$Mods.FargowiltasSouls.Toggle.Spider}", "SpiderEnchant", "6d4e45");//"Spider Crits"
            AddToggle("TurtleConfig", "{$Mods.FargowiltasSouls.Toggle.Turtle}", "TurtleEnchant", "f89c5c");//"Turtle Shell Buff"

            AddToggle("NatureHeader", "{$Mods.FargowiltasSouls.Toggle.Nature}", "NatureForce", "ffffff");//"Force of Nature"
            AddToggle("ChlorophyteConfig", "{$Mods.FargowiltasSouls.Toggle.Chlorophyte}", "ChlorophyteEnchant", "248900");//"Chlorophyte Leaf Crystal"
            AddToggle("CrimsonConfig", "{$Mods.FargowiltasSouls.Toggle.Crimson}", "CrimsonEnchant", "C8364B");//"Crimson Regen"
            AddToggle("RainConfig", "{$Mods.FargowiltasSouls.Toggle.Rain}", "RainEnchant", "ffec00");//"Rain Clouds"
            AddToggle("FrostConfig", "{$Mods.FargowiltasSouls.Toggle.Frost}", "FrostEnchant", "7abdb9");//"Frost Icicles"
            AddToggle("SnowConfig", "{$Mods.FargowiltasSouls.Toggle.Snow}", "SnowEnchant", "25c3f2");//"Snowstorm"
            AddToggle("JungleConfig", "{$Mods.FargowiltasSouls.Toggle.Jungle1}", "JungleEnchant", "71971f");//"Jungle Spores"
            AddToggle("JungleDashConfig", "{$Mods.FargowiltasSouls.Toggle.Jungle2}", "JungleEnchant", "71971f");//"Jungle Dash"
            AddToggle("MoltenConfig", "{$Mods.FargowiltasSouls.Toggle.Molten1}", "MoltenEnchant", "c12b2b");//"Molten Inferno Buff"
            AddToggle("MoltenEConfig", "{$Mods.FargowiltasSouls.Toggle.Molten2}", "MoltenEnchant", "c12b2b");//"Molten Explosion On Hit"
            AddToggle("ShroomiteConfig", "{$Mods.FargowiltasSouls.Toggle.Shroomite1}", "ShroomiteEnchant", "008cf4");//"Shroomite Stealth"
            AddToggle("ShroomiteShroomConfig", "{$Mods.FargowiltasSouls.Toggle.Shroomite2}", "ShroomiteEnchant", "008cf4");//"Shroomite Mushrooms"

            AddToggle("ShadowHeader", "{$Mods.FargowiltasSouls.Toggle.ShadowForce}", "ShadowForce", "ffffff");//"Shadow Force"
            AddToggle("DarkArtConfig", "{$Mods.FargowiltasSouls.Toggle.DarkArt}", "DarkArtistEnchant", "9b5cb0");//"Flameburst Minion"
            AddToggle("ApprenticeConfig", "{$Mods.FargowiltasSouls.Toggle.Apprentice}", "ApprenticeEnchant", "5d86a6");//"Apprentice Effect"
            AddToggle("NecroConfig", "{$Mods.FargowiltasSouls.Toggle.Necro}", "NecroEnchant", "565643");//"Necro Graves"
            AddToggle("ShadowConfig", "{$Mods.FargowiltasSouls.Toggle.Shadow}", "ShadowEnchant", "42356f");//"Shadow Orbs"
            AddToggle("AncientShadowConfig", "{$Mods.FargowiltasSouls.Toggle.AncientShadow}", "AncientShadowEnchant", "42356f");//"Ancient Shadow Darkness"
            AddToggle("MonkConfig", "{$Mods.FargowiltasSouls.Toggle.Monk}", "MonkEnchant", "920520");//"Monk Dash"
            AddToggle("ShinobiDashConfig", "{$Mods.FargowiltasSouls.Toggle.Shinobi1}", "ShinobiEnchant", "935b18");//"Shinobi Teleport Dash"
            AddToggle("ShinobiConfig", "{$Mods.FargowiltasSouls.Toggle.Shinobi2}", "ShinobiEnchant", "935b18");//"Shinobi Through Walls"
            AddToggle("SupersonicTabiConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicTabi}", "SupersonicSoul", "935b18");//"Tabi Dash"
            AddToggle("SupersonicClimbingConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicClimbing}", "SupersonicSoul", "935b18");//"Tiger Climbing Gear"
            AddToggle("SpookyConfig", "{$Mods.FargowiltasSouls.Toggle.Spooky}", "SpookyEnchant", "644e74");//"Spooky Scythes"

            AddToggle("SpiritHeader", "{$Mods.FargowiltasSouls.Toggle.Spirit}", "SpiritForce", "ffffff");//"Force of Spirit"
            AddToggle("FossilConfig", "{$Mods.FargowiltasSouls.Toggle.Fossil}", "FossilEnchant", "8c5c3b");//"Fossil Bones On Hit"
            AddToggle("ForbiddenConfig", "{$Mods.FargowiltasSouls.Toggle.Forbidden}", "ForbiddenEnchant", "e7b21c");//"Forbidden Storm"
            AddToggle("HallowedConfig", "{$Mods.FargowiltasSouls.Toggle.Hallow1}", "HallowEnchant", "968564");//"Hallowed Enchanted Sword Familiar"
            AddToggle("HallowSConfig", "{$Mods.FargowiltasSouls.Toggle.Hallow2}", "HallowEnchant", "968564");//"Hallowed Shield"
            AddToggle("SilverConfig", "{$Mods.FargowiltasSouls.Toggle.Silver1}", "SilverEnchant", "b4b4cc");//"Silver Sword Familiar"
            AddToggle("SilverSpeedConfig", "{$Mods.FargowiltasSouls.Toggle.Silver2}", "SilverEnchant", "b4b4cc");//"Silver Minion Speed"
            AddToggle("SpectreConfig", "{$Mods.FargowiltasSouls.Toggle.Spectre}", "SpectreEnchant", "accdfc");//"Spectre Orbs"
            AddToggle("TikiConfig", "{$Mods.FargowiltasSouls.Toggle.Tiki}", "TikiEnchant", "56A52B");//"Tiki Minions"

            AddToggle("CosmoHeader", "{$Mods.FargowiltasSouls.Toggle.Cosmos}", "CosmoForce", "ffffff");//"Force of Cosmos"
            AddToggle("MeteorConfig", "{$Mods.FargowiltasSouls.Toggle.Meteor}", "MeteorEnchant", "5f4752");//"Meteor Shower"
            AddToggle("NebulaConfig", "{$Mods.FargowiltasSouls.Toggle.Nebula}", "NebulaEnchant", "fe7ee5");//"Nebula Boosters"
            AddToggle("SolarConfig", "{$Mods.FargowiltasSouls.Toggle.Solar1}", "SolarEnchant", "fe9e23");//"Solar Shield"
            AddToggle("SolarFlareConfig", "{$Mods.FargowiltasSouls.Toggle.Solar2}", "SolarEnchant", "fe9e23");//"Inflict Solar Flare"
            AddToggle("StardustConfig", "{$Mods.FargowiltasSouls.Toggle.Stardust}", "StardustEnchant", "00aeee");//"Stardust Guardian"
            AddToggle("VortexSConfig", "{$Mods.FargowiltasSouls.Toggle.Vortex1}", "VortexEnchant", "00f2aa");//"Vortex Stealth"
            AddToggle("VortexVConfig", "{$Mods.FargowiltasSouls.Toggle.Vortex2}", "VortexEnchant", "00f2aa");//"Vortex Voids"

            #endregion enchants

            #region masomode toggles

            //Masomode Header
            AddToggle("MasoHeader", "{$Mods.FargowiltasSouls.Toggle.MasoHeader1}", "MutantStatue", "ffffff");//"Eternity Mode"
            AddToggle("MasoHeader2", "{$Mods.FargowiltasSouls.Toggle.MasoHeader2}", "DeviatingEnergy", "ffffff");//"Eternity Mode Accessories"
            //AddToggle("MasoBossBG", "Mutant Bright Background", "Masochist", "ffffff");
            AddToggle("MasoBossRecolors", "{$Mods.FargowiltasSouls.Toggle.BossRecolors}", "Masochist", "ffffff");//"Boss Recolors (Toggle needs restart)"
            AddToggle("MasoAeolusConfig", "{$Mods.FargowiltasSouls.Toggle.Aeolus}", "AeolusBoots", "ffffff");//"Aeolus Jump"
            AddToggle("MasoIconConfig", "{$Mods.FargowiltasSouls.Toggle.SinisterIcon1}", "SinisterIcon", "ffffff");//"Sinister Icon Spawn Rates"
            AddToggle("MasoIconDropsConfig", "{$Mods.FargowiltasSouls.Toggle.SinisterIcon2}", "SinisterIcon", "ffffff");//"Sinister Icon Drops"
            AddToggle("MasoGrazeConfig", "{$Mods.FargowiltasSouls.Toggle.Graze}", "SparklingAdoration", "ffffff");//"Graze"
            AddToggle("MasoGrazeRingConfig", "{$Mods.FargowiltasSouls.Toggle.GrazeRadius}", "SparklingAdoration", "ffffff");//"Graze Radius Visual"
            AddToggle("MasoDevianttHeartsConfig", "{$Mods.FargowiltasSouls.Toggle.DeviHearts}", "SparklingAdoration", "ffffff");//"Homing Hearts On Hit"

            //supreme death fairy header
            AddToggle("SupremeFairyHeader", "{$Mods.FargowiltasSouls.Toggle.SupremeFairy}", "SupremeDeathbringerFairy", "ffffff");//"Supreme Deathbringer Fairy"
            AddToggle("MasoSlimeConfig", "{$Mods.FargowiltasSouls.Toggle.SlimyShield1}", "SlimyShield", "ffffff");//"Slimy Balls"
            AddToggle("SlimeFallingConfig", "{$Mods.FargowiltasSouls.Toggle.SlimyShield2}", "SlimyShield", "ffffff");//"Increased Fall Speed"
            AddToggle("MasoEyeConfig", "{$Mods.FargowiltasSouls.Toggle.AgitatingLens}", "AgitatingLens", "ffffff");//"Scythes When Dashing"
            AddToggle("MasoHoneyConfig", "{$Mods.FargowiltasSouls.Toggle.QueenStinger}", "QueenStinger", "ffffff");//"Honey When Hitting Enemies"
            AddToggle("MasoSkeleConfig", "{$Mods.FargowiltasSouls.Toggle.NecromanticBrew}", "NecromanticBrew", "ffffff");//"Skeletron Arms Minion"

            //bionomic
            AddToggle("BionomicHeader", "{$Mods.FargowiltasSouls.Toggle.Bionomic}", "BionomicCluster", "ffffff");//"Bionomic Cluster"
            AddToggle("MasoConcoctionConfig", "{$Mods.FargowiltasSouls.Toggle.TimsConcoction}", "TimsConcoction", "ffffff");//"Tim's Concoction"
            AddToggle("MasoCarrotConfig", "{$Mods.FargowiltasSouls.Toggle.OrdinaryCarrot}", "OrdinaryCarrot", "ffffff");//"Carrot View"
            AddToggle("MasoRainbowConfig", "{$Mods.FargowiltasSouls.Toggle.RainbowMatter}", "ConcentratedRainbowMatter", "ffffff");//"Rainbow Slime Minion"
            AddToggle("MasoFrigidConfig", "{$Mods.FargowiltasSouls.Toggle.FrigidGemstone}", "FrigidGemstone", "ffffff");//"Frostfireballs"
            AddToggle("MasoNymphConfig", "{$Mods.FargowiltasSouls.Toggle.NymphsPerfume}", "NymphsPerfume", "ffffff");//"Attacks Spawn Hearts"
            AddToggle("MasoSqueakConfig", "{$Mods.FargowiltasSouls.Toggle.SqueakyToy}", "SqueakyToy", "ffffff");//"Squeaky Toy On Hit"
            AddToggle("MasoPouchConfig", "{$Mods.FargowiltasSouls.Toggle.WretchedPouch}", "WretchedPouch", "ffffff");//"Shadowflame Tentacles"
            AddToggle("MasoClippedConfig", "{$Mods.FargowiltasSouls.Toggle.WyvernFeather}", "WyvernFeather", "ffffff");//"Inflict Clipped Wings"
            AddToggle("TribalCharmConfig", "{$Mods.FargowiltasSouls.Toggle.TribalCharm}", "TribalCharm", "ffffff");//"Tribal Charm Auto Swing"
            //AddToggle("WalletHeader", "Security Wallet", "SecurityWallet", "ffffff");

            //dubious
            AddToggle("DubiousHeader", "{$Mods.FargowiltasSouls.Toggle.Dubious}", "DubiousCircuitry", "ffffff");//"Dubious Circuitry"
            AddToggle("MasoLightningConfig", "{$Mods.FargowiltasSouls.Toggle.RemoteControl1}", "GroundStick", "ffffff");//"Inflict Lightning Rod"
            AddToggle("MasoProbeConfig", "{$Mods.FargowiltasSouls.Toggle.RemoteControl2}", "GroundStick", "ffffff");//"Probes Minion"

            //pure heart
            AddToggle("PureHeartHeader", "{$Mods.FargowiltasSouls.Toggle.PureHeart}", "PureHeart", "ffffff");//"Pure Heart"
            AddToggle("MasoEaterConfig", "{$Mods.FargowiltasSouls.Toggle.CorruptHeart}", "CorruptHeart", "ffffff");//"Tiny Eaters"
            AddToggle("MasoBrainConfig", "{$Mods.FargowiltasSouls.Toggle.GuttedHeart}", "GuttedHeart", "ffffff");//"Creeper Shield"

            //lump of flesh
            AddToggle("LumpofFleshHeader", "{$Mods.FargowiltasSouls.Toggle.LumpOfFlesh}", "LumpOfFlesh", "ffffff");//"Lump of Flesh"
            AddToggle("MasoPugentConfig", "{$Mods.FargowiltasSouls.Toggle.PungentEye}", "LumpOfFlesh", "ffffff");//"Pungent Eye Minion"

            //chalice
            AddToggle("ChaliceHeader", "{$Mods.FargowiltasSouls.Toggle.Chalice}", "ChaliceoftheMoon", "ffffff");//"Chalice of the Moon"
            AddToggle("MasoCultistConfig", "{$Mods.FargowiltasSouls.Toggle.Cultist}", "ChaliceoftheMoon", "ffffff");//"Cultist Minion"
            AddToggle("MasoPlantConfig", "{$Mods.FargowiltasSouls.Toggle.MagicalBulb}", "MagicalBulb", "ffffff");//"Plantera Minion"
            AddToggle("MasoGolemConfig", "{$Mods.FargowiltasSouls.Toggle.Lihzahrd1}", "LihzahrdTreasureBox", "ffffff");//"Lihzahrd Ground Pound"
            AddToggle("MasoBoulderConfig", "{$Mods.FargowiltasSouls.Toggle.Lihzahrd2}", "LihzahrdTreasureBox", "ffffff");//"Boulder Spray"
            AddToggle("MasoCelestConfig", "{$Mods.FargowiltasSouls.Toggle.CelestialRune1}", "CelestialRune", "ffffff");//"Celestial Rune Support"
            AddToggle("MasoVisionConfig", "{$Mods.FargowiltasSouls.Toggle.CelestialRune2}", "CelestialRune", "ffffff");//"Ancient Visions On Hit"

            //heart of the masochist
            AddToggle("HeartHeader", "{$Mods.FargowiltasSouls.Toggle.EternalHeart}", "HeartoftheMasochist", "ffffff");//"Heart of the Eternal"
            AddToggle("MasoPumpConfig", "{$Mods.FargowiltasSouls.Toggle.PumpCape}", "PumpkingsCape", "ffffff");//"Pumpking's Cape Support"
            AddToggle("IceQueensCrownConfig", "{$Mods.FargowiltasSouls.Toggle.IceQueensCrown1}", "IceQueensCrown", "ffffff");//"Freeze On Hit"
            AddToggle("MasoFlockoConfig", "{$Mods.FargowiltasSouls.Toggle.IceQueensCrown2}", "IceQueensCrown", "ffffff");//"Flocko Minion"
            AddToggle("MasoUfoConfig", "{$Mods.FargowiltasSouls.Toggle.SaucerConsole}", "SaucerControlConsole", "ffffff");//"Saucer Minion"
            AddToggle("MasoGravConfig", "{$Mods.FargowiltasSouls.Toggle.GalaGlobe1}", "GalacticGlobe", "ffffff");//"Gravity Control"
            AddToggle("MasoGrav2Config", "{$Mods.FargowiltasSouls.Toggle.GalaGlobe2}", "GalacticGlobe", "ffffff");//"Stabilized Gravity"
            AddToggle("MasoTrueEyeConfig", "{$Mods.FargowiltasSouls.Toggle.GalaGlobe3}", "GalacticGlobe", "ffffff");//"True Eyes Minion"

            //cyclonic fin
            AddToggle("CyclonicHeader", "{$Mods.FargowiltasSouls.Toggle.AbomWand}", "CyclonicFin", "ffffff");//"Abominable Wand"
            AddToggle("MasoFishronConfig", "{$Mods.FargowiltasSouls.Toggle.SpectralAbom}", "CyclonicFin", "ffffff");//"Spectral Abominationn"

            //mutant armor
            AddToggle("MutantArmorHeader", "{$Mods.FargowiltasSouls.Toggle.MutantArmorHeader}", "HeartoftheMasochist", "ffffff");//"True Mutant Armor"
            AddToggle("MasoAbomConfig", "{$Mods.FargowiltasSouls.Toggle.AbomMinion}", "MutantMask", "ffffff");//"Abominationn Minion"
            AddToggle("MasoRingConfig", "{$Mods.FargowiltasSouls.Toggle.PhantasmalRing}", "MutantMask", "ffffff");//"Phantasmal Ring Minion"
            AddToggle("MasoReviveDeathrayConfig", "{$Mods.FargowiltasSouls.Toggle.ReviveDeathray}", "MutantMask", "ffffff");//"Deathray When Revived"

            #endregion masomode toggles

            #region soul toggles

            AddToggle("SoulHeader", "{$Mods.FargowiltasSouls.Toggle.SoulHeader}", "UniverseSoul", "ffffff");//"Souls"
            AddToggle("MeleeConfig", "{$Mods.FargowiltasSouls.Toggle.MeleeSpeed}", "GladiatorsSoul", "ffffff");//"Melee Speed"
            AddToggle("MagmaStoneConfig", "{$Mods.FargowiltasSouls.Toggle.MagmaStone}", "GladiatorsSoul", "ffffff");//"Magma Stone"
            AddToggle("YoyoBagConfig", "{$Mods.FargowiltasSouls.Toggle.YoyoBag}", "GladiatorsSoul", "ffffff");//"Yoyo Bag"
            AddToggle("MoonCharmConfig", "{$Mods.FargowiltasSouls.Toggle.MoonCharm}", "GladiatorsSoul", "ffffff");//"Moon Charm"
            AddToggle("NeptuneShellConfig", "{$Mods.FargowiltasSouls.Toggle.NeptuneShell}", "GladiatorsSoul", "ffffff");//"Neptune's Shell"
            AddToggle("SniperConfig", "{$Mods.FargowiltasSouls.Toggle.SniperScope}", "SnipersSoul", "ffffff");//"Sniper Scope"
            AddToggle("UniverseConfig", "{$Mods.FargowiltasSouls.Toggle.AttackSpeed}", "UniverseSoul", "ffffff");//"Universe Attack Speed"
            AddToggle("MiningHuntConfig", "{$Mods.FargowiltasSouls.Toggle.Hunter}", "MinerEnchant", "ffffff");//"Mining Hunter Buff"
            AddToggle("MiningDangerConfig", "{$Mods.FargowiltasSouls.Toggle.Dangersense}", "MinerEnchant", "ffffff");//"Mining Dangersense Buff"
            AddToggle("MiningSpelunkConfig", "{$Mods.FargowiltasSouls.Toggle.Spelunker}", "MinerEnchant", "ffffff");//"Mining Spelunker Buff"
            AddToggle("MiningShineConfig", "{$Mods.FargowiltasSouls.Toggle.Shine}", "MinerEnchant", "ffffff");//"Mining Shine Buff"
            AddToggle("BuilderConfig", "{$Mods.FargowiltasSouls.Toggle.BuilderMode}", "WorldShaperSoul", "ffffff");//"Builder Mode"
            AddToggle("TrawlerSporeConfig", "{$Mods.FargowiltasSouls.Toggle.SporeSac}", "TrawlerSoul", "ffffff");//"Spore Sac"
            AddToggle("DefenseStarConfig", "{$Mods.FargowiltasSouls.Toggle.Stars}", "ColossusSoul", "ffffff");//"Stars On Hit"
            AddToggle("DefenseBeeConfig", "{$Mods.FargowiltasSouls.Toggle.Bees}", "ColossusSoul", "ffffff");//"Bees On Hit"
            AddToggle("DefensePanicConfig", "{$Mods.FargowiltasSouls.Toggle.Panic}", "ColossusSoul", "ffffff");//"Panic On Hit"
            AddToggle("DefenseFleshKnuckleConfig", "{$Mods.FargowiltasSouls.Toggle.Aggro}", "ColossusSoul", "ffffff");//"Flesh Knuckles Aggro"
            AddToggle("DefensePaladinConfig", "{$Mods.FargowiltasSouls.Toggle.PaladinShield}", "ColossusSoul", "ffffff");//"Paladin's Shield"
            AddToggle("RunSpeedConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicBaseSpeed}", "SupersonicSoul", "ffffff");//"Higher Base Run Speed"
            AddToggle("MomentumConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicMomentum}", "SupersonicSoul", "ffffff");//"No Momentum"
            AddToggle("SupersonicConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicSpeed}", "SupersonicSoul", "ffffff");//"Supersonic Speed Boosts"
            AddToggle("SupersonicJumpsConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicJumps}", "SupersonicSoul", "ffffff");//"Supersonic Jumps"
            AddToggle("SupersonicRocketBootsConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicRocketBoots}", "SupersonicSoul", "ffffff");//"Supersonic Rocket Boots"
            AddToggle("SupersonicCarpetConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicCarpet}", "SupersonicSoul", "ffffff");//"Supersonic Carpet"
            AddToggle("SupersonicFlowerConfig", "{$Mods.FargowiltasSouls.Toggle.SupersonicFlower}", "SupersonicSoul", "248900");//"Flower Boots"
            AddToggle("CthulhuShieldConfig", "{$Mods.FargowiltasSouls.Toggle.CthulhuShield}", "SupersonicSoul", "ffffff");//"Shield of Cthulhu"
            AddToggle("BlackBeltConfig", "{$Mods.FargowiltasSouls.Toggle.BlackBelt}", "SupersonicSoul", "ffffff");//"Black Belt"
            AddToggle("TrawlerConfig", "{$Mods.FargowiltasSouls.Toggle.TrawlerLures}", "TrawlerSoul", "ffffff");//"Trawler Extra Lures"
            AddToggle("TrawlerJumpConfig", "{$Mods.FargowiltasSouls.Toggle.TrawlerJump}", "TrawlerSoul", "ffffff");//"Trawler Jump"
            AddToggle("EternityConfig", "{$Mods.FargowiltasSouls.Toggle.Eternity}", "EternitySoul", "ffffff");//"Eternity Stacking"

            #endregion soul toggles

            #region pet toggles

            AddToggle("PetHeader", "{$Mods.FargowiltasSouls.Toggle.PetHeader}", ItemID.ZephyrFish, "ffffff");//"Pets"
            AddToggle("PetBlackCatConfig", "{$Mods.FargowiltasSouls.Toggle.BlackCat}", 1810, "ffffff");//"Black Cat Pet"
            AddToggle("PetCompanionCubeConfig", "{$Mods.FargowiltasSouls.Toggle.CompanionCube}", 3628, "ffffff");//"Companion Cube Pet"
            AddToggle("PetCursedSaplingConfig", "{$Mods.FargowiltasSouls.Toggle.CursedSapling}", 1837, "ffffff");//"Cursed Sapling Pet"
            AddToggle("PetDinoConfig", "{$Mods.FargowiltasSouls.Toggle.Dino}", 1242, "ffffff");//"Dino Pet"
            AddToggle("PetDragonConfig", "{$Mods.FargowiltasSouls.Toggle.Dragon}", 3857, "ffffff");//"Dragon Pet"
            AddToggle("PetEaterConfig", "{$Mods.FargowiltasSouls.Toggle.Eater}", 994, "ffffff");//"Eater Pet"
            AddToggle("PetEyeSpringConfig", "{$Mods.FargowiltasSouls.Toggle.EyeSpring}", 1311, "ffffff");//"Eye Spring Pet"
            AddToggle("PetFaceMonsterConfig", "{$Mods.FargowiltasSouls.Toggle.FaceMonster}", 3060, "ffffff");//"Face Monster Pet"
            AddToggle("PetGatoConfig", "{$Mods.FargowiltasSouls.Toggle.Gato}", 3855, "ffffff");//"Gato Pet"
            AddToggle("PetHornetConfig", "{$Mods.FargowiltasSouls.Toggle.Hornet}", 1170, "ffffff");//"Hornet Pet"
            AddToggle("PetLizardConfig", "{$Mods.FargowiltasSouls.Toggle.Lizard}", 1172, "ffffff");//"Lizard Pet"
            AddToggle("PetMinitaurConfig", "{$Mods.FargowiltasSouls.Toggle.MiniMinotaur}", 2587, "ffffff");//"Mini Minotaur Pet"
            AddToggle("PetParrotConfig", "{$Mods.FargowiltasSouls.Toggle.Parrot}", 1180, "ffffff");//"Parrot Pet"
            AddToggle("PetPenguinConfig", "{$Mods.FargowiltasSouls.Toggle.Penguin}", 669, "ffffff");//"Penguin Pet"
            AddToggle("PetPupConfig", "{$Mods.FargowiltasSouls.Toggle.Puppy}", 1927, "ffffff");//"Puppy Pet"
            AddToggle("PetSeedConfig", "{$Mods.FargowiltasSouls.Toggle.Seedling}", 1182, "ffffff");//"Seedling Pet"
            AddToggle("PetDGConfig", "{$Mods.FargowiltasSouls.Toggle.Skeletron}", 1169, "ffffff");//"Skeletron Pet"
            AddToggle("PetSnowmanConfig", "{$Mods.FargowiltasSouls.Toggle.Snowman}", 1312, "ffffff");//"Snowman Pet"
            AddToggle("PetGrinchConfig", "{$Mods.FargowiltasSouls.Toggle.Grinch}", ItemID.BabyGrinchMischiefWhistle, "ffffff");//"Grinch Pet"
            AddToggle("PetSpiderConfig", "{$Mods.FargowiltasSouls.Toggle.SpiderPet}", 1798, "ffffff");//"Spider Pet"
            AddToggle("PetSquashConfig", "{$Mods.FargowiltasSouls.Toggle.Squashling}", 1799, "ffffff");//"Squashling Pet"
            AddToggle("PetTikiConfig", "{$Mods.FargowiltasSouls.Toggle.TikiPet}", 1171, "ffffff");//"Tiki Pet"
            AddToggle("PetShroomConfig", "{$Mods.FargowiltasSouls.Toggle.Truffle}", 1181, "ffffff");//"Truffle Pet"
            AddToggle("PetTurtleConfig", "{$Mods.FargowiltasSouls.Toggle.TurtlePet}", 753, "ffffff");//"Turtle Pet"
            AddToggle("PetZephyrConfig", "{$Mods.FargowiltasSouls.Toggle.ZephyrFish}", 2420, "ffffff");//"Zephyr Fish Pet"
            AddToggle("PetHeartConfig", "{$Mods.FargowiltasSouls.Toggle.CrimsonHeart}", 3062, "ffffff");//"Crimson Heart Pet"
            AddToggle("PetNaviConfig", "{$Mods.FargowiltasSouls.Toggle.Fairy}", 425, "ffffff");//"Fairy Pet"
            AddToggle("PetFlickerConfig", "{$Mods.FargowiltasSouls.Toggle.Flickerwick}", 3856, "ffffff");//"Flickerwick Pet"
            AddToggle("PetLanternConfig", "{$Mods.FargowiltasSouls.Toggle.MagicLantern}", 3043, "ffffff");//"Magic Lantern Pet"
            AddToggle("PetOrbConfig", "{$Mods.FargowiltasSouls.Toggle.ShadowOrb}", 115, "ffffff");//"Shadow Orb Pet"
            AddToggle("PetSuspEyeConfig", "{$Mods.FargowiltasSouls.Toggle.SuspiciousEye}", 3577, "ffffff");//"Suspicious Eye Pet"
            AddToggle("PetWispConfig", "{$Mods.FargowiltasSouls.Toggle.Wisp}", 1183, "ffffff");//"Wisp Pet"

            #endregion pet toggles

            #region patreon toggles
            AddToggle("PatreonHeader", "{$Mods.FargowiltasSouls.Toggle.PatreonHeader}", "RoombaPet", "ffffff");//"Patreon Items (Toggles need restart)"
            AddToggle("PatreonRoomba", "{$Mods.FargowiltasSouls.Toggle.PatreonRoomba}", "RoombaPet", "ffffff");//"Roomba"
            AddToggle("PatreonOrb", "{$Mods.FargowiltasSouls.Toggle.PatreonOrb}", "ComputationOrb", "ffffff");//"Computation Orb"
            AddToggle("PatreonFishingRod", "{$Mods.FargowiltasSouls.Toggle.PatreonFishingRod}", "MissDrakovisFishingPole", "ffffff");//"Miss Drakovi's Fishing Pole"
            AddToggle("PatreonDoor", "{$Mods.FargowiltasSouls.Toggle.PatreonDoor}", "SquidwardDoor", "ffffff");//"Squidward Door"
            AddToggle("PatreonWolf", "{$Mods.FargowiltasSouls.Toggle.PatreonWolf}", "ParadoxWolfSoul", "ffffff");//"Paradox Wolf Soul"
            AddToggle("PatreonDove", "{$Mods.FargowiltasSouls.Toggle.PatreonDove}", "FigBranch", "ffffff");//"Fig Branch"
            AddToggle("PatreonKingSlime", "{$Mods.FargowiltasSouls.Toggle.PatreonKingSlime}", "MedallionoftheFallenKing", "ffffff");//"Medallion of the Fallen King"
            AddToggle("PatreonFishron", "{$Mods.FargowiltasSouls.Toggle.PatreonFishron}", "StaffOfUnleashedOcean", "ffffff");//"Staff Of Unleashed Ocean"
            AddToggle("PatreonPlant", "{$Mods.FargowiltasSouls.Toggle.PatreonPlant}", "PiranhaPlantVoodooDoll", "ffffff");//"Piranha Plant Voodoo Doll"
            AddToggle("PatreonDevious", "{$Mods.FargowiltasSouls.Toggle.PatreonDevious}", "DeviousAestheticus", "ffffff");//"Devious Aestheticus"
            AddToggle("PatreonVortex", "{$Mods.FargowiltasSouls.Toggle.PatreonVortex}", "VortexMagnetRitual", "ffffff");//"Vortex Ritual"
            AddToggle("PatreonPrime", "{$Mods.FargowiltasSouls.Toggle.PatreonPrime}", "PrimeStaff", "ffffff");//"Prime Staff"
            AddToggle("PatreonCrimetroid", "{$Mods.FargowiltasSouls.Toggle.PatreonCrimetroid}", "CrimetroidEgg", "ffffff");//"Crimetroid"
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

            OldMusicFade = 0;

            //game will reload golem textures, this helps prevent the crash on reload
            Main.NPCLoaded[NPCID.Golem] = false;
            Main.NPCLoaded[NPCID.GolemFistLeft] = false;
            Main.NPCLoaded[NPCID.GolemFistRight] = false;
            Main.NPCLoaded[NPCID.GolemHead] = false;
            Main.NPCLoaded[NPCID.GolemHeadFree] = false;

            EModeNPCBehaviour.AllEModeNpcBehaviours.Clear();

            ToggleLoader.Unload();
        }

        public override object Call(params object[] args)
        {
            try
            {
                string code = args[0].ToString();

                switch (code)
                {
                    case "Emode":
                    case "EMode":
                    case "EternityMode":
                    case "Masomode":
                    case "MasoMode":
                    case "MasochistMode":
                    case "RealMode":
                        return FargoSoulsWorld.EternityMode;

                    case "MasomodeReal":
                        return FargoSoulsWorld.MasochistModeReal;

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
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>());

                    case "MutantAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>());

                    case "DevianttAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>());

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
            Item.NewItem(player.Center, ItemID.SilverHammer);
            
            Item.NewItem(player.Center, ItemID.Torch, 100);
            Item.NewItem(player.Center, ItemID.LifeCrystal, 4);
            Item.NewItem(player.Center, ItemID.ManaCrystal, 4);
            Item.NewItem(player.Center, ItemID.RecallPotion, 15);
            if (Main.netMode != NetmodeID.SinglePlayer)
                Item.NewItem(player.Center, ItemID.WormholePotion, 15);

            //Item.NewItem(player.Center, ModContent.ItemType<DevianttsSundial>());
            //Item.NewItem(player.Center, ModContent.ItemType<EternityAdvisor>());

            Item.NewItem(player.Center, ModContent.ItemType<AutoHouse>(), 5);
            Item.NewItem(player.Center, ModContent.ItemType<MiniInstaBridge>(), 5);
            Item.NewItem(player.Center, ModContent.ItemType<Instavator>()); //replace this with half-vator in 1.4

            Item.NewItem(player.Center, ModContent.ItemType<EurusSock>());
            Item.NewItem(player.Center, ModContent.ItemType<PuffInABottle>());
            Item.NewItem(player.Center, ItemID.BugNet);
            Item.NewItem(player.Center, ItemID.GrapplingHook);

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
                    bossHealthBar.Call("hbSetColours", new Color(1f, 1f, 1f), new Color(1f, 1f, 0.5f), new Color(1f, 0f, 0f));
                    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<CosmosChampion>());

                    bossHealthBar.Call("hbStart");
                    bossHealthBar.Call("hbSetColours", new Color(1f, 0f, 1f), new Color(1f, 0.2f, 0.6f), new Color(1f, 0f, 0f));
                    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<DeviBoss>());

                    bossHealthBar.Call("RegisterDD2HealthBar", ModContent.NPCType<AbomBoss>());

                    bossHealthBar.Call("hbStart");
                    bossHealthBar.Call("hbSetColours", new Color(55, 255, 191), new Color(0f, 1f, 0f), new Color(0f, 0.5f, 1f));
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

        public void ManageMusicTimestop(bool playMusicAgain)
        {
            if (Main.dedServ)
                return;

            if (playMusicAgain)
            {
                if (OldMusicFade > 0)
                {
                    Main.musicFade[Main.curMusic] = OldMusicFade;
                    OldMusicFade = 0;
                }
            }
            else
            {
                if (OldMusicFade == 0)
                {
                    OldMusicFade = Main.musicFade[Main.curMusic];
                }
                else
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                        Main.musicFade[i] = 0f;
                }
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

        /*public void AddPartialRecipe(ModItem modItem, ModRecipe recipe, int tileType, int replacementItem)
        {
            RecipeGroup group = new RecipeGroup(() => $"{Lang.misc[37]} {modItem.DisplayName.GetDefault()} Material");
            foreach (Item i in recipe.requiredItem)
            {
                if (i == null || i.type == ItemID.None)
                    continue;
                group.ValidItems.Add(i.type);
            }
            string groupName = $"FargowiltasSouls:Any{modItem.Name}Material";
            RecipeGroup.RegisterGroup(groupName, group);

            ModRecipe partialRecipe = new ModRecipe(this);
            int originalItemsNeeded = group.ValidItems.Count / 2;
            partialRecipe.AddRecipeGroup(groupName, originalItemsNeeded);
            partialRecipe.AddIngredient(replacementItem, group.ValidItems.Count - originalItemsNeeded);
            partialRecipe.AddTile(tileType);
            partialRecipe.SetResult(modItem);
            partialRecipe.AddRecipe();
        }*/

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

                /*case 4: //moon lord vulnerability synchronization
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int ML = reader.ReadByte();
                        Main.npc[ML].GetGlobalNPC<EModeGlobalNPC>().Counter[0] = reader.ReadInt32();
                        EModeGlobalNPC.masoStateML = reader.ReadByte();
                    }
                    break;*/

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

                        LunaticCultist cultist = Main.npc[cult].GetEModeNPCMod<LunaticCultist>();
                        cultist.MeleeDamageCounter += reader.ReadInt32();
                        cultist.RangedDamageCounter += reader.ReadInt32();
                        cultist.MagicDamageCounter += reader.ReadInt32();
                        cultist.MinionDamageCounter += reader.ReadInt32();
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

                case 22: // New maso sync
                    {
                        int npcToSync = reader.ReadInt32();
                        Main.npc[npcToSync].GetGlobalNPC<NewEModeGlobalNPC>().NetRecieve(reader);
                    }
                    break;

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
                /*if (FargoSoulsGlobalNPC.FargoSoulsUtil.BossIsAlive(ref FargoSoulsGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
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