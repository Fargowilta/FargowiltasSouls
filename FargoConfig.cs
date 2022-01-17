using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasSouls
{
    class SoulConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public static SoulConfig Instance => ModContent.GetInstance<SoulConfig>();

        private void SetAll(bool val)
        {
            //bool backgroundValue = MutantBackground;
            bool recolorsValue = BossRecolors;

            IEnumerable<FieldInfo> configs = typeof(SoulConfig).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(
                i => i.FieldType == true.GetType() && !i.Name.Contains("Patreon"));
            foreach (FieldInfo config in configs)
            {
                config.SetValue(this, val);
            }

            //MutantBackground = backgroundValue;
            BossRecolors = recolorsValue;

            /*IEnumerable<FieldInfo> walletConfigs = typeof(WalletToggles).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(i => i.FieldType == true.GetType());
            foreach (FieldInfo walletConfig in walletConfigs)
            {
                walletConfig.SetValue(walletToggles, val);
            }*/

            /*IEnumerable<FieldInfo> thoriumConfigs = typeof(ThoriumToggles).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(i => i.FieldType == true.GetType());
            foreach (FieldInfo thoriumConfig in thoriumConfigs)
            {
                thoriumConfig.SetValue(thoriumToggles, val);
            }

            IEnumerable<FieldInfo> calamityConfigs = typeof(CalamityToggles).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(i => i.FieldType == true.GetType());
            foreach (FieldInfo calamityConfig in calamityConfigs)
            {
                calamityConfig.SetValue(calamityToggles, val);
            }*/
        }

        [Header("$Mods.FargowiltasSouls.PresetHeader")]
        [Label("$Mods.FargowiltasSouls.FargoConfigs.AllOn")]
        public bool PresetA
        {
            get => false;
            set
            {
                if (value)
                {
                    SetAll(true);
                }
            }
        }
        [Label("$Mods.FargowiltasSouls.FargoConfigs.AllOff")]
        public bool PresetB
        {
            get => false;
            set
            {
                if (value)
                {
                    SetAll(false);
                }
            }
        }
        /*[Label("Minimal Effects Only")]
        public bool PresetC
        {
            get => false;
            set
            {
                if (value)
                {
                    SetAll(false);

                    //MythrilSpeed = true;
                    //PalladiumHeal = true;
                    //IronMagnet = true;
                    //CthulhuShield = true;
                    //TinCrit = true;
                    //BeetleEffect = true;
                    //SpiderCrits = true;
                    //ShinobiTabi = true;
                    //NebulaBoost = true;
                    //SolarShield = true;
                    //Graze = true;
                    //SinisterIconDrops = true;
                    //NymphPerfume = true;
                    //TribalCharm = true;
                    //StabilizedGravity = true;
                }
            }
        }*/

        [Label("$Mods.FargowiltasSouls.FargoConfigs.EffectToggler")]
        [Description("$Mods.FargowiltasSouls.FargoConfigs.EffectTogglerDescription")]
        [DefaultValue(false)]
        public bool HideTogglerWhenInventoryIsClosed;

        [Label("$Mods.FargowiltasSouls.FargoConfigs.BossMusic")]
        [DefaultValue(true)]
        public bool MutantMusicIsRePrologue;

        #region maso accessories

        [Header("$Mods.FargowiltasSouls.MasoHeader")]
        /*[Label("$Mods.FargowiltasSouls.MasoBossBG")]
        [DefaultValue(true)]
        public bool MutantBackground;*/

        [Label("$Mods.FargowiltasSouls.MasoBossRecolors")]
        [DefaultValue(true)]
        public bool BossRecolors;

        /*[Label("$Mods.FargowiltasSouls.WalletHeader")]
        public WalletToggles walletToggles = new WalletToggles();*/
        #endregion

        [Header("$Mods.FargowiltasSouls.PatreonHeader")]
        [Label("$Mods.FargowiltasSouls.PatreonRoomba")]
        [DefaultValue(true)]
        public bool PatreonRoomba;

        [Label("$Mods.FargowiltasSouls.PatreonOrb")]
        [DefaultValue(true)]
        public bool PatreonOrb;

        [Label("$Mods.FargowiltasSouls.PatreonFishingRod")]
        [DefaultValue(true)]
        public bool PatreonFishingRod;

        [Label("$Mods.FargowiltasSouls.PatreonDoor")]
        [DefaultValue(true)]
        public bool PatreonDoor;

        [Label("$Mods.FargowiltasSouls.PatreonWolf")]
        [DefaultValue(true)]
        public bool PatreonWolf;

        [Label("$Mods.FargowiltasSouls.PatreonDove")]
        [DefaultValue(true)]
        public bool PatreonDove;

        [Label("$Mods.FargowiltasSouls.PatreonKingSlime")]
        [DefaultValue(true)]
        public bool PatreonKingSlime;

        [Label("$Mods.FargowiltasSouls.PatreonFishron")]
        [DefaultValue(true)]
        public bool PatreonFishron;

        [Label("$Mods.FargowiltasSouls.PatreonPlant")]
        [DefaultValue(true)]
        public bool PatreonPlant;

        [Label("$Mods.FargowiltasSouls.PatreonDevious")]
        [DefaultValue(true)]
        public bool PatreonDevious;

        [Label("$Mods.FargowiltasSouls.PatreonVortex")]
        [DefaultValue(true)]
        public bool PatreonVortex;

        [Label("$Mods.FargowiltasSouls.PatreonPrime")]
        [DefaultValue(true)]
        public bool PatreonPrime;

        [Label("$Mods.FargowiltasSouls.PatreonCrimetroid")]
        [DefaultValue(true)]
        public bool PatreonCrimetroid;



        /*[Label("$Mods.FargowiltasSouls.ThoriumHeader")]
        public ThoriumToggles thoriumToggles = new ThoriumToggles();

        [Label("$Mods.FargowiltasSouls.CalamityHeader")]
        public CalamityToggles calamityToggles = new CalamityToggles();*/


        //soa soon tm

        // Proper cloning of reference types is required because behind the scenes many instances of ModConfig classes co-exist.
        /*public override ModConfig Clone()
        {
            var clone = (SoulConfig)base.Clone();

            clone.walletToggles = walletToggles == null ? null : new WalletToggles();
            clone.thoriumToggles = thoriumToggles == null ? null : new ThoriumToggles();
            clone.calamityToggles = calamityToggles == null ? null : new CalamityToggles();

            return clone;
        }*/

        public bool GetValue(bool toggle, bool checkForMutantPresence = true)
        {
            return checkForMutantPresence && Main.player[Main.myPlayer].GetModPlayer<FargoPlayer>().MutantPresence ? false : toggle;
        }
    }

    /*public class WalletToggles
    {
        [Label("Warding")]
        [DefaultValue(true)]
        public bool Warding;

        [Label("Violent")]
        [DefaultValue(true)]
        public bool Violent;

        [Label("Quick")]
        [DefaultValue(true)]
        public bool Quick;

        [Label("Lucky")]
        [DefaultValue(true)]
        public bool Lucky;

        [Label("Menacing")]
        [DefaultValue(true)]
        public bool Menacing;

        [Label("Legendary")]
        [DefaultValue(true)]
        public bool Legendary;

        [Label("Unreal")]
        [DefaultValue(true)]
        public bool Unreal;

        [Label("Mythical")]
        [DefaultValue(true)]
        public bool Mythical;

        [Label("Godly")]
        [DefaultValue(true)]
        public bool Godly;

        [Label("Demonic")]
        [DefaultValue(true)]
        public bool Demonic;

        [Label("Ruthless")]
        [DefaultValue(true)]
        public bool Ruthless;

        [Label("Light")]
        [DefaultValue(true)]
        public bool Light;

        [Label("Deadly")]
        [DefaultValue(true)]
        public bool Deadly;

        [Label("Rapid")]
        [DefaultValue(true)]
        public bool Rapid;
    }*/

    public class ThoriumToggles
    {
        [Label("$Mods.FargowiltasSouls.ThoriumCrystalScorpionConfig")]
        [DefaultValue(true)]
        public bool CrystalScorpion;

        [Label("$Mods.FargowiltasSouls.ThoriumHeadMirrorConfig")]
        [DefaultValue(true)]
        public bool HeadMirror;

        [Label("$Mods.FargowiltasSouls.ThoriumAirWalkersConfig")]
        [DefaultValue(true)]
        public bool AirWalkers;

        [Label("$Mods.FargowiltasSouls.ThoriumGlitterPetConfig")]
        [DefaultValue(true)]
        public bool GlitterPet;

        [Label("$Mods.FargowiltasSouls.ThoriumCoinPetConfig")]
        [DefaultValue(true)]
        public bool CoinPet;

        [Label("$Mods.FargowiltasSouls.ThoriumBoxPetConfig")]
        [DefaultValue(true)]
        public bool BoxPet;

        [Header("$Mods.FargowiltasSouls.MuspelheimForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumBeeBootiesConfig")]
        [DefaultValue(true)]
        public bool BeeBooties;

        [Label("$Mods.FargowiltasSouls.ThoriumSaplingMinionConfig")]
        [DefaultValue(true)]
        public bool SaplingMinion;

        [Header("$Mods.FargowiltasSouls.JotunheimForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumJellyfishPetConfig")]
        [DefaultValue(true)]
        public bool JellyfishPet;

        [Label("$Mods.FargowiltasSouls.ThoriumTideFoamConfig")]
        [DefaultValue(true)]
        public bool TideFoam;

        [Label("$Mods.FargowiltasSouls.ThoriumYewCritsConfig")]
        [DefaultValue(true)]
        public bool YewCrits;

        [Label("$Mods.FargowiltasSouls.ThoriumCryoDamageConfig")]
        [DefaultValue(true)]
        public bool CryoDamage;

        [Label("$Mods.FargowiltasSouls.ThoriumOwlPetConfig")]
        [DefaultValue(true)]
        public bool OwlPet;

        [Label("$Mods.FargowiltasSouls.ThoriumIcyBarrierConfig")]
        [DefaultValue(true)]
        public bool IcyBarrier;

        [Label("$Mods.FargowiltasSouls.ThoriumWhisperingTentaclesConfig")]
        [DefaultValue(true)]
        public bool WhisperingTentacles;

        [Label("$Mods.FargowiltasSouls.ThoriumSpiritPetConfig")]
        [DefaultValue(true)]
        public bool SpiritPet;

        [Label("$Mods.FargowiltasSouls.ThoriumWarlockWispsConfig")]
        [DefaultValue(true)]
        public bool WarlockWisps;

        [Label("$Mods.FargowiltasSouls.ThoriumBiotechProbeConfig")]
        [DefaultValue(true)]
        public bool BiotechProbe;

        [Header("$Mods.FargowiltasSouls.NiflheimForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumMixTapeConfig")]
        [DefaultValue(true)]
        public bool MixTape;

        [Label("$Mods.FargowiltasSouls.ThoriumCyberStatesConfig")]
        [DefaultValue(true)]
        public bool CyberStates;

        [Label("$Mods.FargowiltasSouls.ThoriumMetronomeConfig")]
        [DefaultValue(true)]
        public bool Metronome;

        [Label("$Mods.FargowiltasSouls.ThoriumMarchingBandConfig")]
        [DefaultValue(true)]
        public bool MarchingBand;

        [Header("$Mods.FargowiltasSouls.SvartalfheimForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumEyeoftheStormConfig")]
        [DefaultValue(true)]
        public bool EyeoftheStorm;

        [Label("$Mods.FargowiltasSouls.ThoriumBronzeLightningConfig")]
        [DefaultValue(true)]
        public bool BronzeLightning;

       

        [Label("$Mods.FargowiltasSouls.ThoriumConduitShieldConfig")]
        [DefaultValue(true)]
        public bool ConduitShield;

        [Label("$Mods.FargowiltasSouls.ThoriumOmegaPetConfig")]
        [DefaultValue(true)]
        public bool OmegaPet;

        [Label("$Mods.FargowiltasSouls.ThoriumIFOPetConfig")]
        [DefaultValue(true)]
        public bool IFOPet;

        [Header("$Mods.FargowiltasSouls.MidgardForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumLodestoneConfig")]
        [DefaultValue(true)]
        public bool LodestoneResist;

        [Label("$Mods.FargowiltasSouls.ThoriumBeholderEyeConfig")]
        [DefaultValue(true)]
        public bool BeholderEye;

        [Label("$Mods.FargowiltasSouls.ThoriumIllumiteMissileConfig")]
        [DefaultValue(true)]
        public bool IllumiteMissile;

        [Label("$Mods.FargowiltasSouls.ThoriumTerrariumSpiritsConfig")]
        [DefaultValue(true)]
        public bool TerrariumSpirits;

        [Label("$Mods.FargowiltasSouls.ThoriumDiverConfig")]
        [DefaultValue(true)]
        public bool ThoriumDivers;

        [Label("$Mods.FargowiltasSouls.ThoriumCrietzConfig")]
        [DefaultValue(true)]
        public bool Crietz;

        [Label("$Mods.FargowiltasSouls.ThoriumJesterBellConfig")]
        [DefaultValue(true)]
        public bool JesterBell;

        [Label("$Mods.FargowiltasSouls.ThoriumManaBootsConfig")]
        [DefaultValue(true)]
        public bool ManaBoots;

        [Label("$Mods.FargowiltasSouls.ThoriumWhiteDwarfConfig")]
        [DefaultValue(true)]
        public bool WhiteDwarf;

        [Label("$Mods.FargowiltasSouls.ThoriumCelestialAuraConfig")]
        [DefaultValue(true)]
        public bool CelestialAura;

        [Label("$Mods.FargowiltasSouls.ThoriumAscensionStatueConfig")]
        [DefaultValue(true)]
        public bool AscensionStatue;

        [Header("$Mods.FargowiltasSouls.HelheimForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumSpiritWispsConfig")]
        [DefaultValue(true)]
        public bool SpiritTrapperWisps;

        [Label("$Mods.FargowiltasSouls.ThoriumDreadConfig")]
        [DefaultValue(true)]
        public bool DreadSpeed;

        [Label("$Mods.FargowiltasSouls.ThoriumDragonFlamesConfig")]
        [DefaultValue(true)]
        public bool DragonFlames;

        [Label("$Mods.FargowiltasSouls.ThoriumWyvernPetConfig")]
        [DefaultValue(true)]
        public bool WyvernPet;

        [Label("$Mods.FargowiltasSouls.ThoriumDemonBloodConfig")]
        [DefaultValue(true)]
        public bool DemonBloodEffect;

        [Label("$Mods.FargowiltasSouls.ThoriumFleshDropsConfig")]
        [DefaultValue(true)]
        public bool FleshDrops;

        [Label("$Mods.FargowiltasSouls.ThoriumVampireGlandConfig")]
        [DefaultValue(true)]
        public bool VampireGland;

        [Label("$Mods.FargowiltasSouls.ThoriumBlisterPetConfig")]
        [DefaultValue(true)]
        public bool BlisterPet;

        [Label("$Mods.FargowiltasSouls.ThoriumBerserkerConfig")]
        [DefaultValue(true)]
        public bool BerserkerEffect;

        [Label("$Mods.FargowiltasSouls.ThoriumSlagStompersConfig")]
        [DefaultValue(true)]
        public bool SlagStompers;

        [Label("$Mods.FargowiltasSouls.ThoriumSpringStepsConfig")]
        [DefaultValue(true)]
        public bool SpringSteps;

        [Label("$Mods.FargowiltasSouls.ThoriumHarbingerOverchargeConfig")]
        [DefaultValue(true)]
        public bool HarbingerOvercharge;

        [Header("$Mods.FargowiltasSouls.AsgardForce")]
        [Label("$Mods.FargowiltasSouls.ThoriumTideGlobulesConfig")]
        [DefaultValue(true)]
        public bool TideGlobules;

        [Label("$Mods.FargowiltasSouls.ThoriumTideDaggersConfig")]
        [DefaultValue(true)]
        public bool TideDaggers;

        [Label("$Mods.FargowiltasSouls.ThoriumAssassinDamageConfig")]
        [DefaultValue(true)]
        public bool AssassinDamage;

        [Label("$Mods.FargowiltasSouls.ThoriumpyromancerBurstsConfig")]
        [DefaultValue(true)]
        public bool PyromancerBursts;

    }

    public class CalamityToggles
    {
        [Label("$Mods.FargowiltasSouls.CalamityElementalQuiverConfig")]
        [DefaultValue(true)]
        public bool ElementalQuiver;

        //aerospec
        [Header("$Mods.FargowiltasSouls.AnnihilationForce")]
        [Label("$Mods.FargowiltasSouls.CalamityValkyrieMinionConfig")]
        [DefaultValue(true)]
        public bool ValkyrieMinion;

        [Label("$Mods.FargowiltasSouls.CalamityGladiatorLocketConfig")]
        [DefaultValue(true)]
        public bool GladiatorLocket;

        [Label("$Mods.FargowiltasSouls.CalamityUnstablePrismConfig")]
        [DefaultValue(true)]
        public bool UnstablePrism;

        [Label("$Mods.FargowiltasSouls.CalamityRotomConfig")]
        [DefaultValue(true)]
        public bool RotomPet;

        //statigel
        [Label("$Mods.FargowiltasSouls.CalamityFungalSymbiote")]
        [DefaultValue(true)]
        public bool FungalSymbiote;

        //hydrothermic
        [Label("$Mods.FargowiltasSouls.CalamityAtaxiaEffectsConfig")]
        [DefaultValue(true)]
        public bool AtaxiaEffects;

        [Label("$Mods.FargowiltasSouls.CalamityChaosMinionConfig")]
        [DefaultValue(true)]
        public bool ChaosMinion;

        [Label("$Mods.FargowiltasSouls.CalamityHallowedRuneConfig")]
        [DefaultValue(true)]
        public bool HallowedRune;

        [Label("$Mods.FargowiltasSouls.CalamityEtherealExtorterConfig")]
        [DefaultValue(true)]
        public bool EtherealExtorter;

        //xeroc
        [Label("$Mods.FargowiltasSouls.CalamityXerocEffectsConfig")]
        [DefaultValue(true)]
        public bool XerocEffects;

        //fearmonger
        [Label("$Mods.FargowiltasSouls.CalamityTheEvolutionConfig")]
        [DefaultValue(true)]
        public bool TheEvolution;

        [Label("$Mods.FargowiltasSouls.CalamityStatisBeltOfCursesConfig")]
        [DefaultValue(true)]
        public bool StatisBeltOfCurses;

        //reaver
        [Header("$Mods.FargowiltasSouls.DevastationForce")]
        [Label("$Mods.FargowiltasSouls.CalamityReaverEffectsConfig")]
        [DefaultValue(true)]
        public bool ReaverEffects;

        [Label("$Mods.FargowiltasSouls.CalamityReaverMinionConfig")]
        [DefaultValue(true)]
        public bool ReaverMinion;

        [Label("$Mods.FargowiltasSouls.CalamityFabledTurtleConfig")]
        [DefaultValue(false)]
        public bool FabledTurtleShell;

        [Label("$Mods.FargowiltasSouls.CalamitySparksConfig")]
        [DefaultValue(true)]
        public bool SparksPet;

        //plague reaper
        [Label("$Mods.FargowiltasSouls.CalamityPlagueHiveConfig")]
        [DefaultValue(true)]
        public bool PlagueHive;

        [Label("$Mods.FargowiltasSouls.CalamityPlaguedFuelPackConfig")]
        [DefaultValue(true)]
        public bool PlaguedFuelPack;

        [Label("$Mods.FargowiltasSouls.CalamityTheBeeConfig")]
        [DefaultValue(true)]
        public bool TheBee;

        [Label("$Mods.FargowiltasSouls.CalamityTheCamperConfig")]
        [DefaultValue(false)]
        public bool TheCamper;

        //demonshade
        [Label("$Mods.FargowiltasSouls.CalamityDevilMinionConfig")]
        [DefaultValue(true)]
        public bool RedDevilMinion;

        [Label("$Mods.FargowiltasSouls.CalamityProfanedSoulConfig")]
        [DefaultValue(true)]
        public bool ProfanedSoulCrystal;

        [Label("$Mods.FargowiltasSouls.CalamityLeviConfig")]
        [DefaultValue(true)]
        public bool LeviPet;

        [Label("$Mods.FargowiltasSouls.CalamityScalConfig")]
        [DefaultValue(true)]
        public bool ScalPet;

        //daedalus
        [Header("$Mods.FargowiltasSouls.DesolationForce")]
        [Label("$Mods.FargowiltasSouls.CalamityDaedalusEffectsConfig")]
        [DefaultValue(true)]
        public bool DaedalusEffects;

        [Label("$Mods.FargowiltasSouls.CalamityDaedalusMinionConfig")]
        [DefaultValue(true)]
        public bool DaedalusMinion;

        [Label("$Mods.FargowiltasSouls.CalamityPermafrostPotionConfig")]
        [DefaultValue(true)]
        public bool PermafrostPotion;

        [Label("$Mods.FargowiltasSouls.CalamityRegeneratorConfig")]
        [DefaultValue(false)]
        public bool Regenerator;

        [Label("$Mods.FargowiltasSouls.CalamityKendraConfig")]
        [DefaultValue(true)]
        public bool KendraPet;

        [Label("$Mods.FargowiltasSouls.CalamityBearConfig")]
        [DefaultValue(true)]
        public bool BearPet;

        [Label("$Mods.FargowiltasSouls.CalamityThirdSageConfig")]
        [DefaultValue(true)]
        public bool ThirdSagePet;

        //astral
        [Label("$Mods.FargowiltasSouls.CalamityAstralStarsConfig")]
        [DefaultValue(true)]
        public bool AstralStars;

        [Label("$Mods.FargowiltasSouls.CalamityHideofAstrumDeusConfig")]
        [DefaultValue(true)]
        public bool HideofAstrumDeus;

        [Label("$Mods.FargowiltasSouls.CalamityGravistarSabatonConfig")]
        [DefaultValue(true)]
        public bool GravistarSabaton;

        [Label("$Mods.FargowiltasSouls.CalamityAstrophageConfig")]
        [DefaultValue(true)]
        public bool AstrophagePet;

        //omega blue
        [Label("$Mods.FargowiltasSouls.CalamityOmegaTentaclesConfig")]
        [DefaultValue(true)]
        public bool OmegaTentacles;

        [Label("$Mods.FargowiltasSouls.CalamityDivingSuitConfig")]
        [DefaultValue(false)]
        public bool DivingSuit;

        [Label("$Mods.FargowiltasSouls.CalamityReaperToothNecklaceConfig")]
        [DefaultValue(false)]
        public bool ReaperToothNecklace;

        [Label("$Mods.FargowiltasSouls.CalamityMutatedTruffleConfig")]
        [DefaultValue(true)]
        public bool MutatedTruffle;

        //victide
        [Label("$Mods.FargowiltasSouls.CalamityUrchinConfig")]
        [DefaultValue(true)]
        public bool UrchinMinion;

        [Label("$Mods.FargowiltasSouls.CalamityLuxorGiftConfig")]
        [DefaultValue(true)]
        public bool LuxorGift;










        //organize more later ech


        [Label("$Mods.FargowiltasSouls.CalamityBloodflareEffectsConfig")]
        [DefaultValue(true)]
        public bool BloodflareEffects;

        [Label("$Mods.FargowiltasSouls.CalamityPolterMinesConfig")]
        [DefaultValue(true)]
        public bool PolterMines;

        [Label("$Mods.FargowiltasSouls.CalamitySilvaEffectsConfig")]
        [DefaultValue(true)]
        public bool SilvaEffects;

        [Label("$Mods.FargowiltasSouls.CalamitySilvaMinionConfig")]
        [DefaultValue(true)]
        public bool SilvaMinion;

        [Label("$Mods.FargowiltasSouls.CalamityGodlyArtifactConfig")]
        [DefaultValue(true)]
        public bool GodlySoulArtifact;

        [Label("$Mods.FargowiltasSouls.CalamityYharimGiftConfig")]
        [DefaultValue(true)]
        public bool YharimGift;

        [Label("$Mods.FargowiltasSouls.CalamityFungalMinionConfig")]
        [DefaultValue(true)]
        public bool FungalMinion;

        [Label("$Mods.FargowiltasSouls.CalamityPoisonSeawaterConfig")]
        [DefaultValue(true)]
        public bool PoisonSeawater;

        [Label("$Mods.FargowiltasSouls.CalamityAkatoConfig")]
        [DefaultValue(true)]
        public bool AkatoPet;
        
        [Label("$Mods.FargowiltasSouls.CalamityGodSlayerEffectsConfig")]
        [DefaultValue(true)]
        public bool GodSlayerEffects;

        [Label("$Mods.FargowiltasSouls.CalamityMechwormMinionConfig")]
        [DefaultValue(true)]
        public bool MechwormMinion;

        [Label("$Mods.FargowiltasSouls.CalamityNebulousCoreConfig")]
        [DefaultValue(true)]
        public bool NebulousCore;

        [Label("$Mods.FargowiltasSouls.CalamityChibiiConfig")]
        [DefaultValue(true)]
        public bool ChibiiPet;

        [Label("$Mods.FargowiltasSouls.CalamityAuricEffectsConfig")]
        [DefaultValue(true)]
        public bool AuricEffects;

        [Label("$Mods.FargowiltasSouls.CalamityWaifuMinionsConfig")]
        [DefaultValue(true)]
        public bool WaifuMinions;

        [Label("$Mods.FargowiltasSouls.CalamityShellfishMinionConfig")]
        [DefaultValue(true)]
        public bool ShellfishMinion;

        [Label("$Mods.FargowiltasSouls.CalamityAmidiasPendantConfig")]
        [DefaultValue(true)]
        public bool AmidiasPendant;

        [Label("$Mods.FargowiltasSouls.CalamityGiantPearlConfig")]
        [DefaultValue(true)]
        public bool GiantPearl;

        [Label("$Mods.FargowiltasSouls.CalamityDannyConfig")]
        [DefaultValue(true)]
        public bool DannyPet;

        [Label("$Mods.FargowiltasSouls.CalamityBrimlingConfig")]
        [DefaultValue(true)]
        public bool BrimlingPet;

        [Label("$Mods.FargowiltasSouls.CalamityTarragonEffectsConfig")]
        [DefaultValue(true)]
        public bool TarragonEffects;

        [Label("$Mods.FargowiltasSouls.CalamityRadiatorConfig")]
        [DefaultValue(true)]
        public bool RadiatorPet;

        [Label("$Mods.FargowiltasSouls.CalamityGhostBellConfig")]
        [DefaultValue(true)]
        public bool GhostBellPet;

        [Label("$Mods.FargowiltasSouls.CalamityFlakConfig")]
        [DefaultValue(true)]
        public bool FlakPet;

        [Label("$Mods.FargowiltasSouls.CalamityFoxConfig")]
        [DefaultValue(true)]
        public bool FoxPet;

        [Label("$Mods.FargowiltasSouls.CalamitySirenConfig")]
        [DefaultValue(true)]
        public bool SirenPet;
    }
}
