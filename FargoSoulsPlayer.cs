using FargowiltasSouls.Buffs;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Accessories.Souls;
using FargowiltasSouls.Items.Armor;
using FargowiltasSouls.Items.Dyes;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.ChallengerItems;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Projectiles.Pets;
using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace FargowiltasSouls
{
    public partial class FargoSoulsPlayer : ModPlayer
    {
        public ToggleBackend Toggler = new ToggleBackend();
        public Dictionary<string, bool> TogglesToSync = new Dictionary<string, bool>();
        public IList<string> disabledToggles = new List<string>();

        public bool IsStandingStill;
        public float AttackSpeed;
        public float WingTimeModifier = 1f;

        public bool FreeEaterSummon = true;
        public int Screenshake;

        //        public bool Wood;
        public Item QueenStingerItem;
        public bool EridanusSet;
        public bool EridanusEmpower;
        public int EridanusTimer;
        public bool GaiaSet;
        public bool GaiaOffense;
        public bool StyxSet;
        public int StyxMeter;
        public int StyxTimer;
        public bool NekomiSet;
        public int NekomiMeter;
        public int NekomiTimer;

        //        //minions
        public bool BrainMinion;
        public bool EaterMinion;
        public bool BigBrainMinion;
        public bool DukeFishron;

        //mount
        public bool SquirrelMount;

        //pet
        public bool SeekerOfAncientTreasures;
        public bool AccursedSarcophagus;
        public bool BabySilhouette;
        public bool BabyLifelight;
        public bool BiteSizeBaron;
        public bool ChibiDevi;
        public bool MutantSpawn;
        public bool BabyAbom;

        public bool PetsActive;

        #region enchantments
        //force of timber
        public Item BorealEnchantItem;
        public int BorealCD;
        public Item MahoganyEnchantItem;
        public bool MahoganyCanUseDR;
        public Item PalmEnchantItem;
        public Item PearlwoodEnchantItem;
        public int PearlwoodCD;
        public Item EbonwoodEnchantItem;
        public Item ShadewoodEnchantItem;
        public int ShadewoodCD;
        public Item WoodEnchantItem;
		public bool WoodEnchantDiscount;
        //force of terra
        public Item CopperEnchantItem;
        public int CopperProcCD;
        public Item IronEnchantItem;
        public bool GuardRaised;
        public int ParryDebuffImmuneTime;
        public Item LeadEnchantItem;
        public Item ObsidianEnchantItem;
        public int ObsidianCD;
        public bool LavaWet;
        public Item SilverEnchantItem;
        public Item TinEnchantItem;
        public float TinCritMax;
        public float TinCrit = 5;
        public int TinProcCD;
        public bool TinCritBuffered;
        public Item TungstenEnchantItem;
        public int TungstenCD;
        //force of earth
        public Item AdamantiteEnchantItem;
        public bool AdamantiteCanSplit;
        public Item CobaltEnchantItem;
        public bool CanCobaltJump;
        public bool JustCobaltJumped;
        public int CobaltImmuneTimer;
        public Item MythrilEnchantItem;
        public int MythrilTimer;
        public int MythrilMaxTime => EarthForce ? 300 : 180;
        public float MythrilMaxSpeedBonus => EarthForce ? 1.75f : 1.5f;
        public Item OriEnchantItem;
        public Item PalladEnchantItem;
        public int PalladCounter;
        public Item TitaniumEnchantItem;
        public bool TitaniumDRBuff;
        public bool TitaniumCD;
        //force of nature

        //force of life

        //force of spirit

        //force of shadow

        //force of will

        //force of cosmos
        public Item MeteorEnchantItem;
        public int MeteorTimer;
        public int MeteorCD = 60;
        public bool MeteorShower;
















        public bool AncientHallowEnchantActive;
        public bool AncientShadowEnchantActive;
        public bool ApprenticeEnchantActive;
        public int ApprenticeCD;
        public bool BeeEnchantActive;
        public int BeeCD;
        
        public bool CactusEnchantActive;
        public int CactusProcCD;
        public bool ChloroEnchantActive;
        public Item ChloroEnchantItem;

        public bool CrimsonEnchantActive;
        public int CrimsonRegenAmount;

        public bool DarkArtistEnchantActive;
        public bool DarkArtistSpawn;
        public int DarkArtistSpawnCD;
        public bool ForbiddenEnchantActive;
        public Item FossilEnchantItem;
        public bool FrostEnchantActive;
        public int IcicleCount;
        private int icicleCD;
        public bool GladiatorEnchantActive;
        public int GladiatorCD;
        public bool GoldEnchantActive;
        public bool GoldEnchMoveCoins;
        public bool GoldShell;
        private int goldHP;
        public bool HallowEnchantActive;
        public bool HuntressEnchantActive;
        public int HuntressStage;
        public int HuntressCD;
       
        public bool JungleEnchantActive;
        public int JungleCD;
        
        

        
        public bool MoltenEnchantActive;
        public bool MonkEnchantActive;
        public int MonkDashing;
        private int monkTimer;

        

        public bool NecroEnchantActive;
        public int NecroCD;
        public Item NinjaEnchantItem;
        public bool CrystalEnchantActive;
        public Projectile CrystalSmokeBombProj = null;
        public bool FirstStrike;
        public int SmokeBombCD;
        


       
        public int PumpkinSpawnCD;
        public bool RainEnchantActive;
        public Item RedRidingEnchantItem;
        public int RedRidingArrowCD;
        
        public bool ShadowEnchantActive;
        public bool ShinobiEnchantActive;
        public int dashCD;
        public bool ShroomEnchantActive;
        
        public bool PlatinumEnchantActive;
        public bool SnowEnchantActive;
        public bool SnowVisual;
        public bool SolarEnchantActive;
        public bool SpectreEnchantActive;
        public int SpectreCD;
        public bool SpiderEnchantActive;
        public bool SpookyEnchantActive;
        public bool SquireEnchantActive;
        public bool squireReduceIframes;
        public bool StardustEnchantActive;
        public bool FreezeTime;
        public int freezeLength;
        public const int TIMESTOP_DURATION = 540; //300
        public bool ChillSnowstorm;
        public int chillLength;
        public int CHILL_DURATION => FrostEnchantActive ? 60 * 20 : 60 * 15;
        public bool TikiEnchantActive;
        
        public bool TurtleEnchantActive;
        public int TurtleCounter;
        public int TurtleShellHP = 25;
        private int turtleRecoverCD = 240;
        public bool ShellHide;
        public bool ValhallaEnchantActive;
        public bool VortexEnchantActive;
        public bool VortexStealth;
        public bool WizardEnchantActive;
        public List<BaseEnchant> EquippedEnchants = new List<BaseEnchant>();
        
        public bool NebulaEnchantActive;
        public bool BeetleEnchantActive;
        public int BeetleEnchantDefenseTimer;

        public int CritterAttackTimer;

        public bool CosmoForce;
        public bool EarthForce;
        public bool LifeForce;
        public bool NatureForce;
        public bool SpiritForce;
        public bool ShadowForce;
        public bool TerraForce;
        public bool WillForce;
        public bool WoodForce;

        #endregion

        //        //soul effects
        public bool MagicSoul;
        public bool RangedSoul;
        public bool RangedEssence;
        public bool BuilderMode;
        public bool UniverseSoul;
        public bool UniverseCore;
        public bool FishSoul1;
        public bool FishSoul2;
        public bool TerrariaSoul;
        public bool VoidSoul;
        public int HealTimer;
        public int HurtTimer;
        public bool Eternity;
        public float TinEternityDamage;

        //maso items
        public Item SlimyShieldItem;
        public bool SlimyShieldFalling;
        public Item AgitatingLensItem;
        public int AgitatingLensCD;
        public Item DarkenedHeartItem;
        public int DarkenedHeartCD;
        public bool GuttedHeart;
        public int GuttedHeartCD = 60; //should prevent spawning despite disabled toggle when loading into world
        public Item NecromanticBrewItem;
        public float NecromanticBrewRotation;
        public Item DeerclawpsItem;
        public int IsDashingTimer;
        public bool DeerSinewNerf;
        public int DeerSinewFreezeCD;
        public bool PureHeart;
        public bool PungentEyeballMinion;
        public bool CrystalSkullMinion;
        public bool FusedLens;
        public bool FusedLensCanDebuff;
        public bool GroundStick;
        public bool Supercharged;
        public bool Probes;
        public bool MagicalBulb;
        public bool PlanterasChild;
        public bool SkullCharm;
        public bool PungentEyeball;
        public bool LumpOfFlesh;
        public Item PumpkingsCapeItem;
        public Item LihzahrdTreasureBoxItem;
        public int GroundPound;
        public Item BetsysHeartItem;
        public bool BetsyDashing;
        public int SpecialDashCD;
        public bool MutantAntibodies;
        public Item GravityGlobeEXItem;
        public Item CelestialRuneItem;
        public bool AdditionalAttacks;
        public int AdditionalAttacksTimer;
        public bool MoonChalice;
        public bool LunarCultist;
        public bool TrueEyes;
        public Item AbomWandItem;
        public int AbomWandCD;
        public bool MasochistSoul;
        public bool MasochistHeart;
        public bool MutantsPactSlot;
        public bool HasClickedWrench;
        public bool SandsofTime;
        public bool DragonFang;
		public bool StabilizedGravity;
        public bool SecurityWallet;
        public Item FrigidGemstoneItem;
        public int FrigidGemstoneCD;
        public Item WretchedPouchItem;
        public int WretchedPouchCD;
        public bool NymphsPerfume;
        public bool NymphsPerfumeRespawn;
        public int NymphsPerfumeRestoreLife;
        public int NymphsPerfumeCD = 30;
        public bool SqueakyAcc;
        public bool RainbowSlime;
        public bool SkeletronArms;
        public bool IceQueensCrown;
        public bool CirnoGraze;
        public bool MiniSaucer;
        public bool CanAmmoCycle;
        public bool TribalCharm;
        public bool TribalCharmEquipped;
        public bool TribalCharmClickBonus;
        public bool SupremeDeathbringerFairy;
        public bool GodEaterImbue;
        public Item MutantSetBonusItem;
        public bool AbomMinion;
        public bool PhantasmalRing;
        public bool MutantsDiscountCard;
        public bool MutantsCreditCard;
        public bool DeerSinew;
        public bool RabiesVaccine;
        public bool TwinsEX;
        public bool TimsConcoction;
        public bool ReceivedMasoGift;
        public bool DeviGraze;
        public bool Graze;
        public float GrazeRadius;
        public int DeviGrazeCounter;
        public int CirnoGrazeCounter;
        public double DeviGrazeBonus;
        public Item DevianttHeartItem;
        public int DevianttHeartsCD;
        public Item MutantEyeItem;
        public bool MutantEyeVisual;
        public int MutantEyeCD;
        public bool AbominableWandRevived;
        public bool AbomRebirth;
        public bool WasHurtBySomething;
        public bool PrecisionSeal;
        public bool PrecisionSealHurtbox;
        public bool PrecisionSealNoDashNoJump;
        public Item GelicWingsItem;
        public bool ConcentratedRainbowMatter;

        //debuffs
        public bool Hexed;
        public bool Unstable;
        private int unstableCD;
        public bool Fused;
        public bool Shadowflame;
        public bool Oiled;
        public bool DeathMarked;
        public bool Hypothermia;
        public bool noDodge;
        public bool noSupersonic;
        public bool NoMomentum;
        public bool Bloodthirsty;
        public bool Unlucky;
        public bool DisruptedFocus;
        public bool SinisterIcon;
        public bool SinisterIconDrops;

        public bool Smite;
        public bool Anticoagulation;
        public bool GodEater;               //defense removed, endurance removed, colossal DOT
        public bool FlamesoftheUniverse;    //activates various vanilla debuffs
        public bool MutantNibble;           //moon bite effect, feral bite effect, disables lifesteal
        public int StatLifePrevious = -1;   //used for mutantNibble
        public bool Asocial;                //disables minions, disables pets
        public bool WasAsocial;
        public bool HidePetToggle0 = true;
        public bool HidePetToggle1 = true;
        public bool Kneecapped;             //disables running :v
        public bool Defenseless;            //-30 defense, no damage reduction, cross necklace and knockback prevention effects disabled
        public bool Purified;               //purges all other buffs
        public bool Infested;               //weak DOT that grows exponentially stronger
        public int MaxInfestTime;
        public bool FirstInfection = true;
        public float InfestedDust;
        public bool Rotting;                //inflicts DOT and almost every stat reduced
        public bool SqueakyToy;             //all attacks do one damage and make squeaky noises
        public bool Atrophied;              //melee speed and damage reduced. maybe Player cannot fire melee projectiles?
        public bool Jammed;                 //ranged damage and speed reduced, all non-custom ammo set to baseline ammos
        public bool Slimed;
        public byte lightningRodTimer;
        public bool ReverseManaFlow;
        public bool CurseoftheMoon;
        public bool OceanicMaul;
        public int MaxLifeReduction;
        public int CurrentLifeReduction;
        public int LifeReductionUpdateTimer;
        public bool Midas;
        public bool MutantPresence;
        public bool MutantFang;
        public bool DevianttPresence;
        public bool Swarming;
        public bool LowGround;
        public bool Flipped;
        public bool Mash;
        public bool[] MashPressed = new bool[4];
        public int MashCounter;
        public int StealingCooldown;
        public bool LihzahrdCurse;
        //public bool LihzahrdBlessing;
        public bool Berserked;
        public bool CerebralMindbreak;
        public bool NanoInjection;
        public bool Stunned;
        public bool HaveCheckedAttackSpeed;



        public int ReallyAwfulDebuffCooldown;

        public bool BoxofGizmos;



        
        public Item DreadShellItem;
        public int DreadShellVulnerabilityTimer;
        public int shieldTimer;
        public int shieldCD;
        public bool wasHoldingShield;
        public int LightslingerHitShots;

        public int NoUsingItems;

        public bool HasDash;

        public int WeaponUseTimer;

        //        private Mod dbzMod = ModLoader.GetMod("DBZMOD");

        public Dictionary<int, bool> KnownBuffsToPurify = new Dictionary<int, bool>();

        public bool DoubleTap
        {
            get
            {
                return Main.ReversedUpDownArmorSetBonuses ?
                    Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[1] > 0 && Player.doubleTapCardinalTimer[1] != 15
                    : Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[0] > 0 && Player.doubleTapCardinalTimer[0] != 15;
            }
        }

        public bool IsStillHoldingInSameDirectionAsMovement
            => (Player.velocity.X > 0 && Player.controlRight)
            || (Player.velocity.X < 0 && Player.controlLeft)
            || Player.dashDelay < 0
            || IsDashingTimer > 0;

        //grapple check needed because grapple state extends dash state forever
        public bool IsInADashState
            => (Player.dashDelay == -1 || IsDashingTimer > 0) && Player.grapCount <= 0;

        public override void SaveData(TagCompound tag)
        {
            var playerData = new List<string>();
            if (MutantsPactSlot) playerData.Add("MutantsPactSlot");
            if (MutantsDiscountCard) playerData.Add("MutantsDiscountCard");
            if (MutantsCreditCard) playerData.Add("MutantsCreditCard");
            if (ReceivedMasoGift) playerData.Add("ReceivedMasoGift");
            if (RabiesVaccine) playerData.Add("RabiesVaccine");
            if (DeerSinew) playerData.Add("DeerSinew");
            if (HasClickedWrench) playerData.Add("HasClickedWrench");
            tag.Add($"{Mod.Name}.{Player.name}.Data", playerData);

            var togglesOff = new List<string>();
            foreach (KeyValuePair<string, Toggle> entry in Toggler.Toggles)
            {
                if (!Toggler.Toggles[entry.Key].ToggleBool)
                    togglesOff.Add(entry.Key);
            }
            tag.Add($"{Mod.Name}.{Player.name}.TogglesOff", togglesOff);

            Toggler.Save();
        }

        public override void LoadData(TagCompound tag)
        {
            var playerData = tag.GetList<string>($"{Mod.Name}.{Player.name}.Data");
            MutantsPactSlot = playerData.Contains("MutantsPactSlot");
            MutantsDiscountCard = playerData.Contains("MutantsDiscountCard");
            MutantsCreditCard = playerData.Contains("MutantsCreditCard");
            ReceivedMasoGift = playerData.Contains("ReceivedMasoGift");
            RabiesVaccine = playerData.Contains("RabiesVaccine");
            DeerSinew = playerData.Contains("DeerSinew");
            HasClickedWrench = playerData.Contains("HasClickedWrench");

            disabledToggles = tag.GetList<string>($"{Mod.Name}.{Player.name}.TogglesOff");
        }

        public override void Initialize()
        {
            Toggler.LoadInMenu();
        }

        public override void OnEnterWorld(Player Player)
        {
            Toggler.LoadPlayerToggles(this);
            disabledToggles.Clear();

            if (!ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod))
            {
                Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.NoMusic1"), Color.LimeGreen);
                Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.NoMusic2"), Color.LimeGreen);
            }

            if (Toggler.CanPlayMaso)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    FargoSoulsWorld.CanPlayMaso = true;
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    //Main.NewText("send it");
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)FargowiltasSouls.PacketID.SyncCanPlayMaso);
                    packet.Write(Toggler.CanPlayMaso);
                    packet.Send();
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Mash)
            {
                Player.doubleTapCardinalTimer[0] = 0;
                Player.doubleTapCardinalTimer[1] = 0;
                Player.doubleTapCardinalTimer[2] = 0;
                Player.doubleTapCardinalTimer[3] = 0;

                const int increment = 1;

                if (triggersSet.Up)
                {
                    if (!MashPressed[0])
                        MashCounter += increment;
                    MashPressed[0] = true;
                }
                else
                {
                    MashPressed[0] = false;
                }

                if (triggersSet.Left)
                {
                    if (!MashPressed[1])
                        MashCounter += increment;
                    MashPressed[1] = true;
                }
                else
                {
                    MashPressed[1] = false;
                }

                if (triggersSet.Right)
                {
                    if (!MashPressed[2])
                        MashCounter += increment;
                    MashPressed[2] = true;
                }
                else
                {
                    MashPressed[2] = false;
                }

                if (triggersSet.Down)
                {
                    if (!MashPressed[3])
                        MashCounter += increment;
                    MashPressed[3] = true;
                }
                else
                {
                    MashPressed[3] = false;
                }
            }

            if (FargowiltasSouls.GoldKey.JustPressed && GoldEnchantActive)
            {
                GoldKey();
            }

            if (GoldShell)
            {
                return;
            }

            if (FargowiltasSouls.FreezeKey.JustPressed)
            {
                if (StardustEnchantActive && !Player.HasBuff(ModContent.BuffType<TimeStopCD>()))
                {
                    int cooldownInSeconds = 60;
                    if (CosmoForce)
                        cooldownInSeconds = 50;
                    if (TerrariaSoul)
                        cooldownInSeconds = 40;
                    if (Eternity)
                        cooldownInSeconds = 30;
                    Player.ClearBuff(ModContent.BuffType<TimeFrozen>());
                    Player.AddBuff(ModContent.BuffType<TimeStopCD>(), cooldownInSeconds * 60);

                    FreezeTime = true;
                    freezeLength = TIMESTOP_DURATION;

                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/ZaWarudo"), Player.Center);
                }
                else if (SnowEnchantActive && !Player.HasBuff(ModContent.BuffType<SnowstormCD>()))
                {
                    Player.AddBuff(ModContent.BuffType<SnowstormCD>(), 60 * 60);

                    ChillSnowstorm = true;
                    chillLength = CHILL_DURATION;

                    SoundEngine.PlaySound(SoundID.Item27, Player.Center);

                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire, 0, 0, 0, default, 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 9f;
                    }
                }
            }


            if (FargowiltasSouls.SmokeBombKey.JustPressed && CrystalEnchantActive && SmokeBombCD == 0)
            {
                CrystalAssassinEnchant.SmokeBombKey(this);
            }

            if (FargowiltasSouls.SpecialDashKey.JustPressed && (BetsysHeartItem != null || QueenStingerItem != null))
            {
                SpecialDashKey();
            }

            if (FargowiltasSouls.MagicalBulbKey.JustPressed && MagicalBulb)
            {
                MagicalBulbKey();
            }

            if (FrigidGemstoneItem != null)
            {
                if (FrigidGemstoneCD > 0)
                    FrigidGemstoneCD--;

                if (FargowiltasSouls.FrigidSpellKey.Current)
                    FrigidGemstoneKey();
            }

            if (PrecisionSeal)
            {
                if (SoulConfig.Instance.PrecisionSealIsHold)
                {
                    PrecisionSealNoDashNoJump = FargowiltasSouls.PrecisionSealKey.Current;
                }
                else
                {
                    if (FargowiltasSouls.PrecisionSealKey.JustPressed)
                        PrecisionSealNoDashNoJump = !PrecisionSealNoDashNoJump;
                }
            }
            else
            {
                PrecisionSealNoDashNoJump = false;
            }

            if (PrecisionSealNoDashNoJump)
            {
                Player.doubleTapCardinalTimer[2] = 0;
                Player.doubleTapCardinalTimer[3] = 0;
            }

            if (FargowiltasSouls.BombKey.JustPressed)
            {
                BombKey();
            }

            if (FargowiltasSouls.DebuffInstallKey.JustPressed)
            {
                DebuffInstallKey();
            }

            if (FargowiltasSouls.AmmoCycleKey.JustPressed)
            {
                AmmoCycleKey();
            }

            //            if (triggersSet.Left && Player.confused && Player.gravControl)
            //            {
            //                Player.gravDir *= -1;
            //            }

            if (FargowiltasSouls.SoulToggleKey.JustPressed)
            {
                FargowiltasSouls.UserInterfaceManager.ToggleSoulToggler();
            }
        }

        public override void ResetEffects()
        {
            HasDash = false;

            AttackSpeed = 1f;
            if (Screenshake > 0)
                Screenshake--;

            if (NoUsingItems > 0)
                NoUsingItems--;

            //            Wood = false;

            WingTimeModifier = 1f;

            NinjaEnchantItem = null;

            QueenStingerItem = null;
            EridanusSet = false;
            GaiaSet = false;
            StyxSet = false;
            NekomiSet = false;

            BrainMinion = false;
            EaterMinion = false;
            BigBrainMinion = false;
            DukeFishron = false;

            SquirrelMount = false;

            SeekerOfAncientTreasures = false;
            AccursedSarcophagus = false;
            BabyLifelight = false;
            BabySilhouette = false;
            BiteSizeBaron = false;
            ChibiDevi = false;
            MutantSpawn = false;
            BabyAbom = false;

            //            #region enchantments 
            PetsActive = true;
            ShadowEnchantActive = false;
            CrimsonEnchantActive = false;
            //CrimsonRegen = false;
            SpectreEnchantActive = false;
            BeeEnchantActive = false;
            SpiderEnchantActive = false;
            StardustEnchantActive = false;
            MythrilEnchantItem = null;
            FossilEnchantItem = null;
            JungleEnchantActive = false;
            ShroomEnchantActive = false;
            CobaltEnchantItem = null;
            SpookyEnchantActive = false;
            NebulaEnchantActive = false;
            BeetleEnchantActive = false;
            HallowEnchantActive = false;
            AncientHallowEnchantActive = false;
            ChloroEnchantActive = false;
            ChloroEnchantItem = null;
            VortexEnchantActive = false;
            AdamantiteEnchantItem = null;
            FrostEnchantActive = false;
            PalladEnchantItem = null;
            OriEnchantItem = null;
            MeteorEnchantItem = null;
            MoltenEnchantActive = false;
            CopperEnchantItem = null;
            PlatinumEnchantActive = false;
            CrystalEnchantActive = false;
            FirstStrike = false;
            IronEnchantItem = null;
            TurtleEnchantActive = false;
            ShellHide = false;
            LeadEnchantItem = null;
            GladiatorEnchantActive = false;
            GoldEnchantActive = false;
            GoldShell = false;
            CactusEnchantActive = false;
            ForbiddenEnchantActive = false;
            NecroEnchantActive = false;
            ObsidianEnchantItem = null;
            LavaWet = false;
            TinEnchantItem = null;
            TikiEnchantActive = false;
            SolarEnchantActive = false;
            ShinobiEnchantActive = false;
            ValhallaEnchantActive = false;
            DarkArtistEnchantActive = false;
            RedRidingEnchantItem = null;
            TungstenEnchantItem = null;

            MahoganyEnchantItem = null;
            BorealEnchantItem = null;
            WoodEnchantItem = null;
			WoodEnchantDiscount = false;
            PalmEnchantItem = null;
            EbonwoodEnchantItem = null;
            ShadewoodEnchantItem = null;
            PearlwoodEnchantItem = null;

            RainEnchantActive = false;
            AncientShadowEnchantActive = false;
            SquireEnchantActive = false;
            ApprenticeEnchantActive = false;
            HuntressEnchantActive = false;
            MonkEnchantActive = false;
            SnowEnchantActive = false;
            SnowVisual = false;
            TitaniumEnchantItem = null;
            TitaniumDRBuff = false;
            TitaniumCD = false;

            CosmoForce = false;
            EarthForce = false;
            LifeForce = false;
            NatureForce = false;
            SpiritForce = false;
            TerraForce = false;
            ShadowForce = false;
            WillForce = false;
            WoodForce = false;

            //            #endregion

            //souls
            MagicSoul = false;
            RangedSoul = false;
            RangedEssence = false;
            BuilderMode = false;
            UniverseSoul = false;
            UniverseCore = false;
            FishSoul1 = false;
            FishSoul2 = false;
            TerrariaSoul = false;
            VoidSoul = false;
            Eternity = false;

            //maso
            SlimyShieldItem = null;
            AgitatingLensItem = null;
            DarkenedHeartItem = null;
            GuttedHeart = false;
            NecromanticBrewItem = null;
            DeerclawpsItem = null;
            DeerSinewNerf = false;
            PureHeart = false;
            PungentEyeballMinion = false;
            CrystalSkullMinion = false;
            FusedLens = false;
            FusedLensCanDebuff = false;
            GroundStick = false;
            Supercharged = false;
            Probes = false;
            MagicalBulb = false;
            PlanterasChild = false;
            SkullCharm = false;
            PungentEyeball = false;
            LumpOfFlesh = false;
            PumpkingsCapeItem = null;
            LihzahrdTreasureBoxItem = null;
            BetsysHeartItem = null;
            BetsyDashing = false;
            MutantAntibodies = false;
            GravityGlobeEXItem = null;
            CelestialRuneItem = null;
            AdditionalAttacks = false;
            MoonChalice = false;
            LunarCultist = false;
            TrueEyes = false;
            AbomWandItem = null;
            MasochistSoul = false;
            MasochistHeart = false;
            SandsofTime = false;
            DragonFang = false;
			StabilizedGravity = false;
            SecurityWallet = false;
            FrigidGemstoneItem = null;
            WretchedPouchItem = null;
            NymphsPerfume = false;
            NymphsPerfumeRespawn = false;
            SqueakyAcc = false;
            RainbowSlime = false;
            SkeletronArms = false;
            IceQueensCrown = false;
            CirnoGraze = false;
            MiniSaucer = false;
            CanAmmoCycle = false;
            TribalCharm = false;
            TribalCharmEquipped = false;
            SupremeDeathbringerFairy = false;
            GodEaterImbue = false;
            MutantSetBonusItem = null;
            AbomMinion = false;
            PhantasmalRing = false;
            TwinsEX = false;
            TimsConcoction = false;
            DeviGraze = false;
            Graze = false;
            GrazeRadius = 100f;
            DevianttHeartItem = null;
            MutantEyeItem = null;
            MutantEyeVisual = false;
            AbomRebirth = false;
            WasHurtBySomething = false;
            PrecisionSeal = false;
            PrecisionSealHurtbox = false;
            GelicWingsItem = null;
            ConcentratedRainbowMatter = false;

            //debuffs
            Hexed = false;
            Unstable = false;
            Fused = false;
            Shadowflame = false;
            Oiled = false;
            Slimed = false;
            noDodge = false;
            noSupersonic = false;
            NoMomentum = false;
            Bloodthirsty = false;
            DisruptedFocus = false;
            SinisterIcon = false;
            SinisterIconDrops = false;

            Smite = false;
            Anticoagulation = false;
            GodEater = false;
            FlamesoftheUniverse = false;
            MutantNibble = false;
            Asocial = false;
            Kneecapped = false;
            Defenseless = false;
            Purified = false;
            Infested = false;
            Rotting = false;
            SqueakyToy = false;
            Atrophied = false;
            Jammed = false;
            ReverseManaFlow = false;
            CurseoftheMoon = false;
            OceanicMaul = false;
            DeathMarked = false;
            Hypothermia = false;
            Midas = false;
            MutantPresence = MutantPresence ? Player.HasBuff(ModContent.BuffType<Buffs.Boss.MutantPresence>()) : false;
            MutantFang = false;
            DevianttPresence = false;
            Swarming = false;
            LowGround = false;
            Flipped = false;
            LihzahrdCurse = false;
            //LihzahrdBlessing = false;
            Berserked = false;
            CerebralMindbreak = false;
            NanoInjection = false;
            Stunned = false;
            HaveCheckedAttackSpeed = false;
            BoxofGizmos = false;
            //IronEnchantShield = false;
            SilverEnchantItem = null;
            DreadShellItem = null;

            EquippedEnchants.Clear();

            if (WizardEnchantActive)
            {
                WizardEnchantActive = false;
                for (int i = 3; i <= 9; i++)
                {
                    if (!Player.armor[i].IsAir && (Player.armor[i].type == ModContent.ItemType<WizardEnchant>() || Player.armor[i].type == ModContent.ItemType<Items.Accessories.Forces.CosmoForce>()))
                    {
                        WizardEnchantActive = true;
                        CosmoForce = true;
                        EarthForce = true;
                        LifeForce = true;
                        NatureForce = true;
                        ShadowForce = true;
                        SpiritForce = true;
                        TerraForce = true;
                        WillForce = true;
                        WoodForce = true;
                        break;
                    }
                }
            }

            if (!Mash && MashCounter > 0)
                MashCounter--;
            Mash = false;
        }

        public override void OnRespawn(Player Player)
        {
            if (NymphsPerfumeRespawn)
            {
                NymphsPerfumeRestoreLife = 6;
            }
        }

        public override void UpdateDead()
        {
            bool wasSandsOfTime = SandsofTime;
            bool wasNymphsPerfumeRespawn = NymphsPerfumeRespawn;

            ResetEffects();

            SandsofTime = wasSandsOfTime;
            NymphsPerfumeRespawn = wasNymphsPerfumeRespawn;

            if (SandsofTime && !FargoSoulsUtil.AnyBossAlive() && Player.respawnTimer > 10)
                Player.respawnTimer -= Eternity ? 6 : 1;

            if (FargoSoulsWorld.MasochistModeReal && FargoSoulsUtil.AnyBossAlive())
            {
                if (Player.respawnTimer < 10)
                {
                    Player.respawnTimer = 10;
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && Main.npc[FargoSoulsGlobalNPC.boss].HasValidTarget && Main.npc[FargoSoulsGlobalNPC.boss].HasPlayerTarget)
                {
                    Player.Center = Main.player[Main.npc[FargoSoulsGlobalNPC.boss].target].Center;
                }
            }

            BeetleEnchantDefenseTimer = 0;

            ReallyAwfulDebuffCooldown = 0;
            ParryDebuffImmuneTime = 0;

            WingTimeModifier = 1f;
            FreeEaterSummon = true;

            AbominableWandRevived = false;

            EridanusTimer = 0;
            StyxMeter = 0;
            StyxTimer = 0;
            NekomiMeter = 0;
            NekomiTimer = 0;

            CirnoGrazeCounter = 0;

            //debuffs
            unstableCD = 0;
            lightningRodTimer = 0;

            BuilderMode = false;
            NoUsingItems = 0;

            FreezeTime = false;
            freezeLength = 0;

            ChillSnowstorm = false;
            chillLength = 0;

            SlimyShieldFalling = false;
            DarkenedHeartCD = 60;
            GuttedHeartCD = 60;
            IsDashingTimer = 0;
            GroundPound = 0;
            NymphsPerfumeCD = 30;
            WretchedPouchCD = 0;

            DeviGrazeBonus = 0;
            MutantEyeCD = 60;

            Mash = false;
            WizardEnchantActive = false;
            MashCounter = 0;

            MaxLifeReduction = 0;
            CurrentLifeReduction = 0;

            MythrilTimer = MythrilMaxTime;
        }

        public override void PreUpdate()
        {
            if (Player.CCed)
            {
                Player.doubleTapCardinalTimer[2] = 2;
                Player.doubleTapCardinalTimer[3] = 2;
            }

            if (HurtTimer > 0)
                HurtTimer--;

            IsStandingStill = Math.Abs(Player.velocity.X) < 0.05 && Math.Abs(Player.velocity.Y) < 0.05;


            //            Player.npcTypeNoAggro[0] = true;

            if (!Infested && !FirstInfection)
                FirstInfection = true;

            //            if (Eternity && TinCrit < 50)
            //                TinCrit = 50;
            //            else if(TerrariaSoul && TinCrit < 20)
            //                TinCrit = 20;
            //            else if (TerraForce && TinCrit < 10)
            //                TinCrit = 10;



            //            if (VortexStealth && !VortexEnchant)
            //                VortexStealth = false;

            if (Unstable && Player.whoAmI == Main.myPlayer)
            {
                if (unstableCD == 0)
                {
                    Vector2 pos = Player.position;

                    int x = Main.rand.Next((int)pos.X - 500, (int)pos.X + 500);
                    int y = Main.rand.Next((int)pos.Y - 500, (int)pos.Y + 500);
                    Vector2 teleportPos = new Vector2(x, y);

                    while (Collision.SolidCollision(teleportPos, Player.width, Player.height) && teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                    {
                        x = Main.rand.Next((int)pos.X - 500, (int)pos.X + 500);
                        y = Main.rand.Next((int)pos.Y - 500, (int)pos.Y + 500);
                        teleportPos = new Vector2(x, y);
                    }

                    Player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                    unstableCD = 60;
                }
                unstableCD--;
            }

            if (BeeEnchantActive && BeeCD > 0)
                BeeCD--;

            if (GoldShell)
            {
                GoldUpdate();
            }

            //horizontal dash
            if (MonkDashing > 0)
            {
                MonkDashing--;

                //no loss of height
                //Player.maxFallSpeed = 0f;
                //Player.fallStart = (int)(Player.position.Y / 16f);
                //Player.gravity = 0f;
                //Player.position.Y = Player.oldPosition.Y;


                //Main.NewText(MonkDashing);

                /*if (MonkDashing == 0)
                {
                    Player.velocity *= 0f;
                    Player.dashDelay = 0;
                }*/
            }
            //vertical dash
            else if (MonkDashing < 0)
            {
                MonkDashing++;

                Player.immune = true;
                Player.maxFallSpeed *= 30f;
                Player.gravity = 1.5f;

                /*if (MonkDashing == 0)
                {
                    Player.velocity *= 0.5f;
                    Player.dashDelay = 0;
                }*/
            }
        }

        public override void PostUpdateBuffs()
        {
            if (Berserked && !Player.CCed)
            {
                Player.controlUseItem = true;
                Player.releaseUseItem = true;
            }
        }

        public override void ModifyLuck(ref float luck)
        {
            if (Unlucky)
                luck -= 1.0f;

            Unlucky = false;
        }

        public override void PostUpdateEquips()
        {
            if (MahoganyEnchantItem != null)
                RichMahoganyEnchant.PostUpdate(Player);

            if (NoMomentum && !Player.mount.Active)
            {
                if (Player.vortexStealthActive && Math.Abs(Player.velocity.X) > 6)
                    Player.vortexStealthActive = false;

                Player.runAcceleration *= 5f;
                Player.runSlowdown *= 5f;

                if (!IsStillHoldingInSameDirectionAsMovement)
                    Player.runSlowdown += 7f;
            }

            if (TribalCharmEquipped)
            {
                Items.Accessories.Masomode.TribalCharm.Effects(this);
            }

            if (PungentEyeball && Player.whoAmI == Main.myPlayer && Player.GetToggleValue("MasoPungentCursor"))
            {
                const float distance = 16 * 5;

                foreach (NPC n in Main.npc.Where(n => n.active && !n.dontTakeDamage && n.lifeMax > 5 && !n.friendly))
                {
                    if (Vector2.Distance(Main.MouseWorld, FargoSoulsUtil.ClosestPointInHitbox(n.Hitbox, Main.MouseWorld)) < distance)
                    {
                        n.AddBuff(ModContent.BuffType<PungentGaze>(), 2, true);
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    Vector2 spawnPos = Main.MouseWorld + Main.rand.NextVector2CircularEdge(distance, distance);
                    Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, DustID.GemRuby, 0, 0, 100, Color.White)];
                    dust.scale = 0.5f;
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.NextBool(3))
                    {
                        dust.velocity += Vector2.Normalize(Main.MouseWorld - dust.position) * Main.rand.NextFloat(5f);
                        dust.position += dust.velocity * 5f;
                    }
                    dust.noGravity = true;
                }
            }

            if (DarkenedHeartItem != null)
            {
                if (!IsStillHoldingInSameDirectionAsMovement)
                    Player.runSlowdown += 0.2f;
            }

            if (!StardustEnchantActive)
                FreezeTime = false;

            if (TungstenEnchantItem != null && TungstenCD > 0)
                TungstenCD--;

            UpdateShield();

            if (ShadowEnchantActive)
                ShadowEffectPostEquips();

            Player.wingTimeMax = (int)(Player.wingTimeMax * WingTimeModifier);

            if (MutantAntibodies && Player.wet)
            {
                Player.wingTime = Player.wingTimeMax;
                Player.AddBuff(ModContent.BuffType<Refreshed>(), 30 * 60);
            }

            if (StyxSet)
            {
                Player.accDreamCatcher = true; //dps counter is on

                //even if you attack weaker enemies or with less dps, you'll eventually get a charge
                if (StyxTimer > 0 && --StyxTimer == 1) //yes, 1, to avoid a possible edge case of frame perfect attacks blocking this
                {
                    int dps = Player.getDPS();
                    if (dps != 0)
                    {
                        int diff = StyxCrown.MINIMUM_DPS - dps;
                        if (diff > 0)
                            StyxMeter += diff;
                    }

                    //if (Player.getDPS() == 0) Main.NewText("bug! styx timer ran with 0 dps, show this to terry");
                }
            }
            else
            {
                StyxMeter = 0;
                StyxTimer = 0;
            }

            if (!GaiaSet)
                GaiaOffense = false;

            if (!EridanusSet)
                EridanusEmpower = false;

            if (QueenStingerItem != null)
            {
                if (Player.honey)
                    Player.GetArmorPenetration(DamageClass.Generic) += 10;
            }

            if (BeetleEnchantDefenseTimer > 0)
                BeetleEnchantDefenseTimer--;

            if (RabiesVaccine)
                Player.buffImmune[BuffID.Rabies] = true;

            if (AbomWandItem != null)
                AbomWandUpdate();

            if (Flipped && !Player.gravControl)
            {
                Player.gravControl = true;
                Player.controlUp = false;
                Player.gravDir = -1f;
                //Player.fallStart = (int)(Player.position.Y / 16f);
                //Player.jump = 0;
            }

            if (DevianttHeartItem != null)
            {
                if (DevianttHeartsCD > 0)
                    DevianttHeartsCD--;
            }

            if ((BetsysHeartItem != null || QueenStingerItem != null) && SpecialDashCD > 0 && --SpecialDashCD == 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, Player.Center);

                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, 87, 0, 0, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                }
            }

            if (GuttedHeart)
                GuttedHeartEffect();

            if (SlimyShieldItem != null || LihzahrdTreasureBoxItem != null || GelicWingsItem != null)
                OnLandingEffects();

            if (AgitatingLensItem != null)
                AgitatingLensEffect();

            if (WretchedPouchItem != null)
                WretchedPouchEffect();

            if (PalladEnchantItem != null)
                PalladiumEnchant.PalladiumUpdate(this);

            if (TitaniumDRBuff)
            {
                float diff = 1f - Player.endurance;
                diff *= EarthForce ? 0.75f : 0.5f;
                Player.endurance += diff;
            }

            if (noDodge)
            {
                Player.onHitDodge = false;
                Player.shadowDodge = false;
                Player.blackBelt = false;
                Player.brainOfConfusionItem = null;
                GuttedHeart = false;
            }

            #region dashes

            if (Player.dashType != 0)
                HasDash = true;

            if (SolarEnchantActive)
                SolarEffect();

            if (ShinobiEnchantActive)
                ShinobiDashChecks();

            if (JungleEnchantActive)
                JungleEffect();

            if (DeerSinew)
                DeerSinewEffect();

            if (LihzahrdTreasureBoxItem != null)
                LihzahrdTreasureBoxUpdate();

            #endregion dashes

            if (PrecisionSealNoDashNoJump)
            {
                Player.dashType = 0;
                Player.hasJumpOption_Cloud = false;
                Player.hasJumpOption_Sandstorm = false;
                Player.hasJumpOption_Blizzard = false;
                Player.hasJumpOption_Fart = false;
                Player.hasJumpOption_Sail = false;
                Player.hasJumpOption_Unicorn = false;
                JungleJumping = false;
                CanJungleJump = false;
                dashCD = 2;
                IsDashingTimer = 0;
                HasDash = false;
                if (lihzahrdFallCD < 2)
                    lihzahrdFallCD = 2;
            }

            if (DeerclawpsItem != null && IsInADashState)
            {
                DeerclawpsAttack(Player.Bottom);
            }

            if (NecromanticBrewItem != null && IsInADashState && Player.GetToggleValue("MasoSkeleSpin"))
            {
                //if (NecromanticBrewRotation == 0)
                //{
                //    NecromanticBrewRotation = 0.001f;
                //    Player.velocity.X *= 1.1f;
                //}

                float dashSpeedBoost = 0.5f * Player.velocity.X;
                Player.position.X += dashSpeedBoost;
                if (Collision.SolidCollision(Player.position, Player.width, Player.height))
                    Player.position.X -= dashSpeedBoost;

                //Collision.StepUp(ref Player.position, ref Player.velocity, Player.width, Player.height, ref Player.stepSpeed, ref Player.gfxOffY, (int)Player.gravDir, Player.controlUp);

                Player.noKnockback = true;
                Player.thorns = 4f;

                NecromanticBrewRotation += 0.6f * Math.Sign(Player.velocity.X == 0 ? Player.direction : Player.velocity.X);
                Player.fullRotation = NecromanticBrewRotation;
                Player.fullRotationOrigin = Player.Center - Player.position;
            }
            else if (NecromanticBrewRotation != 0)
            {
                Player.fullRotation = 0f;
                NecromanticBrewRotation = 0f;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            //these are here so that emode minion nerf can properly detect the real set bonuses over in EModePlayer postupdateequips
            if (SquireEnchantActive)
                Player.setSquireT2 = true;

            if (ValhallaEnchantActive)
                Player.setSquireT3 = true;

            if (ApprenticeEnchantActive)
                Player.setApprenticeT2 = true;

            if (DarkArtistEnchantActive)
                Player.setApprenticeT3 = true;

            if (MonkEnchantActive)
                Player.setMonkT2 = true;

            if (ShinobiEnchantActive)
                Player.setMonkT3 = true;


            if (Player.channel && WeaponUseTimer < 2)
                WeaponUseTimer = 2;
            if (--WeaponUseTimer < 0)
                WeaponUseTimer = 0;

            if (IsDashingTimer > 0)
            {
                IsDashingTimer--;
                Player.dashDelay = -1;
            }

            if (GoldEnchMoveCoins)
            {
                ChestUI.MoveCoins(Player.inventory, Player.bank.item);
                GoldEnchMoveCoins = false;
            }

            if (SpectreCD > 0)
                SpectreCD--;

            if (ParryDebuffImmuneTime > 0)
            {
                ParryDebuffImmuneTime--;
                DreadShellVulnerabilityTimer = 0;
            }
            else if (DreadShellVulnerabilityTimer > 0)
            {
                DreadShellVulnerabilityTimer--;

                Player.statDefense -= 30;
                Player.endurance -= 0.3f;
            }

            if (++frameCounter >= 60)
                frameCounter = 0;

            if (HealTimer > 0)
                HealTimer--;

            if (TinEnchantItem != null)
                TinEnchant.TinPostUpdate(this);

            if (NebulaEnchantActive)
                NebulaEffect();

            if (LowGround)
            {
                Player.waterWalk = false;
                Player.waterWalk2 = false;
            }

            if (dashCD > 0)
                dashCD--;

            if (ReallyAwfulDebuffCooldown > 0)
                ReallyAwfulDebuffCooldown--;

            if (OceanicMaul && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron))
            {
                Player.statLifeMax2 /= 5;
                if (Player.statLifeMax2 < 100)
                    Player.statLifeMax2 = 100;
            }

            if (StealingCooldown > 0 && !Player.dead)
                StealingCooldown--;

            if (LihzahrdCurse && Framing.GetTileSafely(Player.Center).WallType == WallID.LihzahrdBrickUnsafe)
            {
                Player.dangerSense = false;
                Player.InfoAccMechShowWires = false;
            }

            if (Graze) //decrease graze bonus over time
            {
                if (++DeviGrazeCounter > 60)
                {
                    DeviGrazeCounter = 0;
                    if (DeviGrazeBonus > 0f)
                        DeviGrazeBonus -= 0.01;
                }

                if (CirnoGrazeCounter > 0)
                    CirnoGrazeCounter--;
            }

            if (StabilizedGravity && Player.GetToggleValue("MasoGrav2", false))
                Player.gravity = Math.Max(Player.gravity, Player.defaultGravity);

            if (Atrophied)
            {
                Player.GetDamage(DamageClass.Melee) *= 0.01f;
                Player.GetCritChance(DamageClass.Melee) /= 100;
            }

            if (Slimed)
            {
                //slowed effect
                Player.moveSpeed *= .75f;
                Player.jump /= 2;
            }

            if (GodEater)
            {
                Player.statDefense = 0;
                Player.endurance = 0;
            }

            if (MutantNibble) //disables lifesteal, mostly
            {
                if (Player.statLife > 0 && StatLifePrevious > 0 && Player.statLife > StatLifePrevious)
                    Player.statLife = StatLifePrevious;
            }

            if (Defenseless)
            {
                Player.statDefense -= 30;
                Player.endurance = 0;
                Player.longInvince = false;
                Player.noKnockback = false;
            }

            if (Purified)
            {
                //tries to remove all buffs/debuffs
                for (int i = Player.MaxBuffs - 1; i >= 0; i--)
                {
                    if (Player.buffType[i] > 0
                        && !Main.debuff[Player.buffType[i]] && !Main.buffNoTimeDisplay[Player.buffType[i]]
                        && !BuffID.Sets.TimeLeftDoesNotDecrease[Player.buffType[i]])
                    {
                        if (!KnownBuffsToPurify.ContainsKey(Player.buffType[i]))
                            KnownBuffsToPurify[Player.buffType[i]] = true;

                        Player.DelBuff(i);
                    }
                }

                foreach (int b in KnownBuffsToPurify.Keys)
                {
                    Player.buffImmune[b] = true;
                }
            }

            if (Asocial)
            {
                KillPets();
                Player.maxMinions = 0;
                Player.maxTurrets = 0;
            }
            else if (WasAsocial) //should only occur when above debuffs end
            {
                Player.hideMisc[0] = HidePetToggle0;
                Player.hideMisc[1] = HidePetToggle1;

                WasAsocial = false;
            }

            if (Rotting)
            {
                Player.moveSpeed *= 0.9f;
                //Player.statLifeMax2 -= Player.statLifeMax / 5;
                Player.statDefense -= 10;
                Player.endurance -= 0.1f;
                AttackSpeed -= 0.1f;
                Player.GetDamage(DamageClass.Generic) -= 0.1f;
            }

            if (Kneecapped)
            {
                Player.accRunSpeed = Player.maxRunSpeed;
            }

            ManageLifeReduction();

            if (Eternity)
                Player.statManaMax2 = 999;
            else if (UniverseSoul)
                Player.statManaMax2 += 300;

            if (AdditionalAttacks && AdditionalAttacksTimer > 0)
                AdditionalAttacksTimer--;

            if (MutantPresence || DevianttPresence)
            {
                Player.statDefense /= 2;
                Player.endurance /= 2;
                Player.shinyStone = false;
            }

            StatLifePrevious = Player.statLife;

            if (EbonwoodEnchantItem != null)
                EbonwoodEnchant.EbonwoodEffect(Player);


            if (TitaniumDRBuff && prevDyes == null)
            {
                prevDyes = new List<int>();
                byte reflectiveSilver = (byte)GameShaders.Armor.GetShaderIdFromItemId(ItemID.ReflectiveSilverDye);

                for (int i = 0; i < Player.dye.Length; i++)
                {
                    prevDyes.Add(Player.dye[i].dye);
                    Player.dye[i].dye = reflectiveSilver;
                }

                for (int j = 0; j < Player.miscDyes.Length; j++)
                {
                    prevDyes.Add(Player.miscDyes[j].dye);
                    Player.miscDyes[j].dye = reflectiveSilver;
                }

                Player.UpdateDyes();
            }
            else if(!Player.HasBuff(ModContent.BuffType<TitaniumDRBuff>()) && prevDyes != null)
            {
                for (int i = 0; i < Player.dye.Length; i++)
                {
                    Player.dye[i].dye = prevDyes[i];
                }

                for (int j = 0; j < Player.miscDyes.Length; j++)
                {
                    Player.miscDyes[j].dye = prevDyes[j + Player.dye.Length];
                }

                Player.UpdateDyes();

                prevDyes = null;
            }
            
        }

        List<int> prevDyes = null;

        public void ManageLifeReduction()
        {
            if (OceanicMaul && LifeReductionUpdateTimer <= 0)
                LifeReductionUpdateTimer = 1; //trigger life reduction behaviour

            if (LifeReductionUpdateTimer > 0)
            {
                const int threshold = 30;
                if (LifeReductionUpdateTimer++ > threshold)
                {
                    LifeReductionUpdateTimer = 1;

                    if (OceanicMaul) //with maul, real max life gradually decreases to the desired point
                    {
                        if (MutantFang) //update faster
                            LifeReductionUpdateTimer = threshold - 10;

                        int newLifeReduction = CurrentLifeReduction + 5;
                        if (newLifeReduction > MaxLifeReduction)
                            newLifeReduction = MaxLifeReduction;
                        if (newLifeReduction > Player.statLifeMax2 - 100) //i.e. max life wont go below 100
                            newLifeReduction = Player.statLifeMax2 - 100;

                        if (CurrentLifeReduction < newLifeReduction)
                        {
                            CurrentLifeReduction = newLifeReduction;
                            CombatText.NewText(Player.Hitbox, Color.DarkRed, Language.GetTextValue($"Mods.{Mod.Name}.Message.OceanicMaulLifeDown"));
                        }
                    }
                    else //after maul wears off, real max life gradually recovers to normal value
                    {
                        CurrentLifeReduction -= 5;
                        if (MaxLifeReduction > CurrentLifeReduction)
                            MaxLifeReduction = CurrentLifeReduction;
                        CombatText.NewText(Player.Hitbox, Color.DarkGreen, Language.GetTextValue($"Mods.{Mod.Name}.Message.OceanicMaulLifeUp"));
                    }
                }
            }

            if (CurrentLifeReduction > 0)
            {
                if (CurrentLifeReduction > Player.statLifeMax2 - 100) //i.e. max life wont go below 100
                    CurrentLifeReduction = Player.statLifeMax2 - 100;
                Player.statLifeMax2 -= CurrentLifeReduction;
                //if (Player.statLife > Player.statLifeMax2) Player.statLife = Player.statLifeMax2;
            }
            else if (!OceanicMaul) //deactivate behaviour
            {
                CurrentLifeReduction = 0;
                MaxLifeReduction = 0;
                LifeReductionUpdateTimer = 0;
            }
        }

        public override void PostUpdate()
        {
            if (!FreeEaterSummon && !Main.npc.Any(n => n.active && (n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail)))
            {
                FreeEaterSummon = true;
            }

            if (NymphsPerfumeRestoreLife > 0 && --NymphsPerfumeRestoreLife == 0)
            {
                if (Player.statLife < Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;
                //doing it like this so it accounts for your lifeMax after respawn
                //regular OnRespawn() doesnt account for lifeforce, and is lowered by dying with oceanic maul
            }

            ConcentratedRainbowMatterTryAutoHeal();
        }

        public override float UseSpeedMultiplier(Item item)
        {
            int useTime = item.useTime;
            int useAnimate = item.useAnimation;

            if (useTime <= 0 || useAnimate <= 0 || item.damage <= 0 || HaveCheckedAttackSpeed)
                return base.UseSpeedMultiplier(item);

            HaveCheckedAttackSpeed = true;

            if (!Berserked && !TribalCharm && BoxofGizmos && !item.autoReuse && !Player.FeralGloveReuse(item))
            {
                int targetUseTime = useTime + 6;
                while (useTime / AttackSpeed < targetUseTime)
                {
                    AttackSpeed -= .05f;
                }
            }

            if (Berserked)
            {
                AttackSpeed += .1f;
            }

            if (MagicSoul && item.CountsAsClass(DamageClass.Magic))
            {
                AttackSpeed += .2f;
            }

            if (MythrilEnchantItem != null )
            {
                MythrilEnchant.CalcMythrilAttackSpeed(this, item);
            }

            if (WretchedPouchItem != null && !MasochistSoul && AttackSpeed > 1f)
            {
                float diff = AttackSpeed - 1f;
                diff /= 2;
                AttackSpeed -= diff;
            }

            if (NinjaEnchantItem != null && Player.GetToggleValue("NinjaSpeed"))
            {
                AttackSpeed *= 2;
            }

            //checks so weapons dont break
            while (useTime / AttackSpeed < 1)
            {
                AttackSpeed -= .01f;
            }

            //modify attack speed so it rounds up
            int useTimeRoundUp = (int)Math.Round(useTime / AttackSpeed, MidpointRounding.ToPositiveInfinity);
            if (useTimeRoundUp < useTime) //sanity check
            {
                while (useTime / AttackSpeed < useTimeRoundUp)
                {
                    AttackSpeed -= .01f; //small increments to avoid skipping past any integers
                }
            }

            while (useAnimate / AttackSpeed < 3)
            {
                AttackSpeed -= .01f;
            }

            if (AttackSpeed < .1f)
                AttackSpeed = .1f;

            return AttackSpeed;
        }

        public override void UpdateBadLifeRegen()
        {
            if (Player.electrified && Player.wet)
                Player.lifeRegen -= 16;

            void DamageOverTime(int badLifeRegen, bool affectLifeRegenCount = false)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                if (affectLifeRegenCount && Player.lifeRegenCount > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= badLifeRegen;
            }

            if (NanoInjection)
                DamageOverTime(10);

            if (Shadowflame)
                DamageOverTime(10);

            if (GodEater)
            {
                DamageOverTime(170, true);

                Player.lifeRegenCount -= 70;
            }

            if (MutantNibble)
                DamageOverTime(0, true);

            if (Infested)
                DamageOverTime(InfestedExtraDot());

            if (Rotting)
                DamageOverTime(2);

            if (CurseoftheMoon)
                DamageOverTime(20);

            if (Oiled && Player.lifeRegen < 0)
            {
                Player.lifeRegen *= 2;
            }

            if (MutantPresence)
            {
                if (Player.lifeRegen > 5)
                    Player.lifeRegen = 5;
            }

            if (FlamesoftheUniverse)
                DamageOverTime((30 + 50 + 48 + 30) / 2, true);

            if (Smite)
                DamageOverTime(0, true);

            if (Anticoagulation)
                DamageOverTime(4, true);

            if (Player.lifeRegen < 0)
            {
                if (TerraForce)
                {
                    Player.lifeRegen = (int)(Player.lifeRegen * 0.4f);
                }
                else if (LeadEnchantItem != null)
                {
                    Player.lifeRegen = (int)(Player.lifeRegen * 0.6f);
                }

                FusedLensCanDebuff = true;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //            if (squireReduceIframes && (SquireEnchant || ValhallaEnchant))
            //            {
            //                if (Main.rand.NextBool(3))
            //                {
            //                    float scale = ValhallaEnchant ? 2f : 1.5f;
            //                    int type = ValhallaEnchant ? 87 : 91;
            //                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, type, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 87, default(Color), scale);
            //                    Main.dust[dust].noGravity = true;
            //                    Main.dust[dust].velocity *= 1.8f;
            //                    Main.dust[dust].velocity.Y -= 0.5f;
            //                    if (Main.rand.NextBool(4))
            //                    {
            //                        Main.dust[dust].noGravity = false;
            //                        Main.dust[dust].scale *= 0.5f;
            //                    }
            //                    Main.PlayerDrawDust.Add(dust);
            //                }
            //                fullBright = true;
            //            }

            if (Shadowflame)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.Shadowflame, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                fullBright = true;
            }

            if (Rotting)
            {
                if (drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.Blood, Player.velocity.X * 0.1f, Player.velocity.Y * 0.1f, 0, default(Color), 2f);
                    Main.dust[dust].noGravity = Main.rand.NextBool();
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
            }

            if (Purified)
            {
                if (drawInfo.shadow == 0f)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, 91, 0.0f, 0.0f, 100, default, 2.5f);
                    Dust dust = Main.dust[index2];
                    Main.dust[index2].velocity *= 2f;
                    Main.dust[index2].noGravity = true;
                    drawInfo.DustCache.Add(index2);
                }
            }

            if (Smite)
            {
                if (drawInfo.shadow == 0f)
                {
                    Color color = Main.DiscoColor;
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, 91, 0.0f, 0.0f, 100, color, 2.5f);
                    Main.dust[index2].velocity *= 2f;
                    Main.dust[index2].noGravity = true;
                    drawInfo.DustCache.Add(index2);
                }
            }

            if (Anticoagulation)
            {
                if (drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].scale += 1f;
                }
            }

            if (Hexed)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, DustID.BubbleBlock, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].color = Color.GreenYellow;
                    drawInfo.DustCache.Add(dust);
                }
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int index2 = Dust.NewDust(Player.position, Player.width, Player.height, 106, 0.0f, 0.0f, 100, default, 2.5f);
                    Dust dust = Main.dust[index2];
                    Main.dust[index2].noGravity = true;
                    drawInfo.DustCache.Add(index2);
                }
                fullBright = true;
            }

            if (Infested)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, 44, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), InfestedDust);
                    Main.dust[dust].noGravity = true;
                    //Main.dust[dust].velocity *= 1.8f;
                    // Main.dust[dust].velocity.Y -= 0.5f;
                    drawInfo.DustCache.Add(dust);
                }
                fullBright = true;
            }

            if (CurrentLifeReduction > 0)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood);
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].scale += 1f;
                    drawInfo.DustCache.Add(d);
                }
            }

            if (GodEater)
            {
                if (Main.rand.NextBool(3) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, 86, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    drawInfo.DustCache.Add(dust);
                }
                r *= 0.15f;
                g *= 0.03f;
                b *= 0.09f;
                fullBright = true;
            }

            if (FlamesoftheUniverse)
            {
                /*drawInfo.drawPlayer.onFire = true;
                drawInfo.drawPlayer.onFire2 = true;
                drawInfo.drawPlayer.onFrostBurn = true;
                drawInfo.drawPlayer.ichor = true;
                drawInfo.drawPlayer.burned = true;*/
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, 21, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5), 2.5f);
                    Main.dust[d].velocity.Y -= 1;
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                    drawInfo.DustCache.Add(d);
                }
                fullBright = true;
            }

            if (CurseoftheMoon)
            {
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(Player.Center, 0, 0, 229, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                    drawInfo.DustCache.Add(d);
                }
                if (Main.rand.NextBool(5))
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, 229, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 1f;
                    Main.dust[d].velocity *= 2f;
                    drawInfo.DustCache.Add(d);
                }
            }

            if (DeathMarked)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position - new Vector2(2f, 2f), Player.width, Player.height, 109, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, default(Color), 1.5f);
                    Main.dust[dust].velocity.Y--;
                    if (!Main.rand.NextBool(3))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                    }
                    drawInfo.DustCache.Add(dust);
                }
                r *= 0.2f;
                g *= 0.2f;
                b *= 0.2f;
                fullBright = true;
            }

            if (Fused)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position + new Vector2(Player.width / 2, Player.height / 5), 0, 0, DustID.Torch, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 0, default(Color), 2f);
                    Main.dust[dust].velocity.Y -= 2f;
                    Main.dust[dust].velocity *= 2f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].scale += 0.5f;
                        Main.dust[dust].noGravity = true;
                    }
                    drawInfo.DustCache.Add(dust);
                }
            }

            if (Supercharged)
            {
                if (Main.rand.NextBool() && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, 229, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f);
                    Main.dust[dust].scale += 0.5f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    if (Main.rand.NextBool(3))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }

            if (ForbiddenEnchantActive && drawInfo.shadow == 0f)
            {
                Color color12 = Player.GetImmuneAlphaPure(Lighting.GetColor((int)(drawInfo.Position.X + Player.width * 0.5) / 16, (int)(drawInfo.Position.Y + Player.height * 0.5) / 16, Color.White), drawInfo.shadow);
                Color color21 = Color.Lerp(color12, value2: Color.White, 0.7f);

                Texture2D texture2D2 = TextureAssets.Extra[74].Value;
                Texture2D texture = TextureAssets.GlowMask[217].Value;
                bool flag8 = !Player.setForbiddenCooldownLocked;
                int num52 = (int)((Player.miscCounter / 300f * 6.28318548f).ToRotationVector2().Y * 6f);
                float num53 = (Player.miscCounter / 75f * 6.28318548f).ToRotationVector2().X * 4f;
                Color color22 = new Color(80, 70, 40, 0) * (num53 / 8f + 0.5f) * 0.8f;
                if (!flag8)
                {
                    num52 = 0;
                    num53 = 2f;
                    color22 = new Color(80, 70, 40, 0) * 0.3f;
                    color21 = color21.MultiplyRGB(new Color(0.5f, 0.5f, 1f));
                }
                Vector2 vector4 = new Vector2(((int)(drawInfo.Position.X - Main.screenPosition.X - (Player.bodyFrame.Width / 2) + (Player.width / 2))), ((int)(drawInfo.Position.Y - Main.screenPosition.Y + Player.height - Player.bodyFrame.Height + 4f))) + Player.bodyPosition + new Vector2((Player.bodyFrame.Width / 2), (Player.bodyFrame.Height / 2));
                vector4 += new Vector2((float)(-(float)Player.direction * 10), (float)(-20 + num52));
                DrawData value = new DrawData(texture2D2, vector4, null, color21, Player.bodyRotation, texture2D2.Size() / 2f, 1f, drawInfo.playerEffect, 0);

                int num6 = 0;
                if (Player.dye[1] != null)
                {
                    num6 = Player.dye[1].dye;
                }
                value.shader = num6;
                drawInfo.DrawDataCache.Add(value);
                for (float num54 = 0f; num54 < 4f; num54 += 1f)
                {
                    value = new DrawData(texture, vector4 + (num54 * 1.57079637f).ToRotationVector2() * num53, null, color22, Player.bodyRotation, texture2D2.Size() / 2f, 1f, drawInfo.playerEffect, 0);
                    drawInfo.DrawDataCache.Add(value);
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.hostile)
                return;

            if (SpiderEnchantActive && FargoSoulsUtil.IsSummonDamage(proj))
            {
                if (Main.rand.Next(100) < Player.ActualClassCrit(DamageClass.Summon))
                    crit = true;
            }

            if (apprenticeBonusDamage)
            {
                if (ShadowForce)
                {
                    damage = (int)(damage * 2.5f);
                }
                else
                {
                    damage = (int)(damage * 1.5f);
                }

                apprenticeBonusDamage = false;
                apprenticeSwitchReady = false;
                ApprenticeCD = 0;

                //dust
                int dustId = Dust.NewDust(new Vector2(proj.position.X, proj.position.Y + 2f), proj.width, proj.height + 5, DustID.FlameBurst, 0, 0, 100, Color.Black, 2f);
                Main.dust[dustId].noGravity = true;

                int blastDamage = damage;
                if (!TerrariaSoul)
                    blastDamage = Math.Min(blastDamage, FargoSoulsUtil.HighestDamageTypeScaling(Player, 300));
                Projectile.NewProjectile(Player.GetSource_Misc(""), target.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, blastDamage, 0, Player.whoAmI);
            }

            if (Hexed || (ReverseManaFlow && proj.CountsAsClass(DamageClass.Magic)))
            {
                target.life += damage;
                target.HealEffect(damage);

                if (target.life > target.lifeMax)
                {
                    target.life = target.lifeMax;
                }

                damage = 0;
                knockback = 0;
                crit = false;

                return;

            }

            if (SqueakyToy)
            {
                damage = 1;
                Squeak(target.Center);
                return;
            }

            if (Asocial && FargoSoulsUtil.IsSummonDamage(proj, true, false))
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }

            if (Atrophied && (proj.CountsAsClass(DamageClass.Melee) || proj.CountsAsClass(DamageClass.Throwing)))
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }

            if (TungstenEnchantItem != null && proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TungstenScale != 1)
            {
                TungstenEnchant.TungstenModifyDamage(Player, ref damage, ref crit, proj.DamageType);
            }

            if (HuntressEnchantActive && proj.GetGlobalProjectile<FargoSoulsGlobalProjectile>().HuntressProj == 1)
            {
                HuntressEnchant.HuntressBonus(this, proj, target, ref damage);
            }

            ModifyHitNPCBoth(target, ref damage, ref crit, proj.DamageType);
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
			if (NinjaEnchantItem != null && Player.GetToggleValue("NinjaSpeed"))
			{
				damage /= 2;
			}
			
            if (Hexed || (ReverseManaFlow && item.CountsAsClass(DamageClass.Magic)))
            {
                target.life += damage;
                target.HealEffect(damage);

                if (target.life > target.lifeMax)
                {
                    target.life = target.lifeMax;
                }

                damage = 0;
                knockback = 0;
                crit = false;

                return;

            }

            if (SqueakyToy)
            {
                damage = 1;
                Squeak(target.Center);
                return;
            }

            if (Atrophied)
            {
                damage = 0;
                knockback = 0;
                crit = false;
            }

            if (TungstenEnchantItem != null && Toggler != null && Player.GetToggleValue("Tungsten")
                && (TerraForce || item.shoot == ProjectileID.None))
            {
                TungstenEnchant.TungstenModifyDamage(Player, ref damage, ref crit, item.DamageType);
            }

            ModifyHitNPCBoth(target, ref damage, ref crit, item.DamageType);
        }

        public void ModifyHitNPCBoth(NPC target, ref int damage, ref bool crit, DamageClass damageClass)
        {
            if (crit)
            {
                if (Eternity)
                    damage *= 5;
                else if (UniverseCore)
                    damage *= 2;

                if (SpiderEnchantActive && damageClass.CountsAsClass(DamageClass.Summon) && !TerrariaSoul)
                    damage = (int)Math.Round(damage * 0.75);
            }

            if (DeerSinewNerf)
            {
                float ratio = Math.Min(Player.velocity.Length() / 20f, 1f);
                damage = (int)Math.Round(damage * MathHelper.Lerp(1f, 0.85f, ratio));
            }

            if (CerebralMindbreak)
                damage = (int)(0.7 * damage);

            if (FirstStrike)
            {
                crit = true;
                damage = (int)(damage * 1.5f);
                Player.ClearBuff(ModContent.BuffType<FirstStrike>());
                //target.defense -= 5;
                target.AddBuff(BuffID.BrokenArmor, 600);
            }
        }

        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            if (!SqueakyToy) return;
            damage = 1;
            Squeak(target.Center);
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (!SqueakyToy) return;
            damage = 1;
            Squeak(target.Center);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            if (proj.minion)// && proj.type != ModContent.ProjectileType<CelestialRuneAncientVision>() && proj.type != ModContent.ProjectileType<SpookyScythe>())
                TryAdditionalAttacks(proj.damage, proj.DamageType);

            OnHitNPCEither(target, damage, knockback, crit, proj.DamageType, projectile: proj);

            if (OriEnchantItem != null && proj.type == ProjectileID.FlowerPetal)
            {
                target.AddBuff(ModContent.BuffType<OriPoison>(), 300);
                target.immune[proj.owner] = 2;
            }
        }

        private void OnHitNPCEither(NPC target, int damage, float knockback, bool crit, DamageClass damageClass, Projectile projectile = null, Item item = null)
        {
            //doing this so that damage-inheriting effects dont double dip or explode due to taking on crit boost
            int GetBaseDamage()
            {
                int baseDamage = damage;
                if (projectile != null)
                    baseDamage = projectile.damage;
                else if (item != null)
                    baseDamage = Player.GetWeaponDamage(item);
                return baseDamage;
            }

            if (StyxSet)
            {
                StyxMeter += damage;
                if (StyxTimer <= 0 && !target.friendly && target.lifeMax > 5 && target.type != NPCID.TargetDummy)
                    StyxTimer = 60;
            }

            if (BeetleEnchantActive && Player.beetleOffense && damageClass != DamageClass.Melee)
            {
                Player.beetleCounter += damage;
            }

            if (PearlwoodEnchantItem != null && Player.GetToggleValue("Pearl") && PearlwoodCD == 0 && !(projectile != null && projectile.type == ProjectileID.FairyQueenMagicItemShot && projectile.usesIDStaticNPCImmunity && projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames))
            {
                PearlwoodEnchant.PearlwoodStarDrop(this, target, GetBaseDamage());
            }

            if (BeeEnchantActive && Player.GetToggleValue("Bee") && BeeCD <= 0 && target.realLife == -1
                && (projectile == null || (projectile.type != ProjectileID.Bee && projectile.type != ProjectileID.GiantBee && projectile.maxPenetrate != 1 && !projectile.usesLocalNPCImmunity && !projectile.usesIDStaticNPCImmunity && projectile.owner == Main.myPlayer)))
            {
                bool force = LifeForce;
                if (force || Main.rand.NextBool())
                {
                    int beeDamage = GetBaseDamage();
                    if (beeDamage > 0)
                    {
                        if (!TerrariaSoul)
                            beeDamage = Math.Min(beeDamage, FargoSoulsUtil.HighestDamageTypeScaling(Player, 300));

                        float beeKB = projectile != null ? projectile.knockBack : item != null ? item.knockBack : knockback;

                        int p = Projectile.NewProjectile(item != null ? Player.GetSource_ItemUse(item) : projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-35, 36) * 0.2f, Main.rand.Next(-35, 36) * 0.2f,
                            force ? ProjectileID.GiantBee : Player.beeType(), beeDamage, Player.beeKB(beeKB), Player.whoAmI);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].DamageType = damageClass;
                    }
                    BeeCD = 15;
                }
            }

            if (PalladEnchantItem != null && !Player.onHitRegen)
            {
                Player.AddBuff(BuffID.RapidHealing, Math.Min(300, damage / 3)); //heal time based on damage dealt, capped at 5sec
            }

            if (CopperEnchantItem != null && crit)
            {
                CopperEnchant.CopperProc(this, target);
            }

            if (ShadewoodEnchantItem != null)
            {
                ShadewoodEnchant.ShadewoodProc(this, target, projectile);
            }

            if (TitaniumEnchantItem != null && (projectile == null || projectile.type != ProjectileID.TitaniumStormShard))
            {
                TitaniumEnchant.TitaniumShards(this, Player);
            }


            if (Player.GetToggleValue("Obsidian") && ObsidianEnchantItem != null && ObsidianCD == 0)
            {
                ObsidianEnchant.ObsidianProc(this, target, GetBaseDamage());
            }        

            if (DevianttHeartItem != null && DevianttHeartsCD <= 0 && Player.GetToggleValue("MasoDevianttHearts")
                && (projectile == null || (projectile.type != ModContent.ProjectileType<FriendRay>() && projectile.type != ModContent.ProjectileType<FriendHeart>())))
            {
                DevianttHeartsCD = AbomWandItem == null ? 600 : 300;

                if (Main.myPlayer == Player.whoAmI)
                {
                    Vector2 offset = 300 * Player.DirectionFrom(Main.MouseWorld);
                    for (int i = -3; i <= 3; i++)
                    {
                        Vector2 spawnPos = Player.Center + offset.RotatedBy(Math.PI / 7 * i);
                        Vector2 speed = Vector2.Normalize(Main.MouseWorld - spawnPos);

                        int baseHeartDamage = AbomWandItem == null ? 17 : 170;
                        //heartDamage = (int)(heartDamage * Player.ActualClassDamage(DamageClass.Summon));

                        float ai1 = (Main.MouseWorld - spawnPos).Length() / 17;

                        if (MutantEyeItem == null)
                            FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(DevianttHeartItem), spawnPos, 17f * speed, ModContent.ProjectileType<FriendHeart>(), baseHeartDamage, 3f, Player.whoAmI, -1, ai1);
                        else
                            FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(DevianttHeartItem), spawnPos, speed, ModContent.ProjectileType<FriendRay>(), baseHeartDamage, 3f, Player.whoAmI, (float)Math.PI / 7 * i);

                        FargoSoulsUtil.HeartDust(spawnPos, speed.ToRotation());
                    }
                }
            }

            if (SnowEnchantActive)
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }

            if (GodEaterImbue)
            {
                /*if (target.FindBuffIndex(ModContent.BuffType<GodEater>()) < 0 && target.aiStyle != 37)
                {
                    if (target.type != ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
                    {
                        target.DelBuff(4);
                        target.buffImmune[ModContent.BuffType<GodEater>()] = false;
                    }
                }*/
                target.AddBuff(ModContent.BuffType<GodEater>(), 420);
            }

            if (GladiatorEnchantActive && Player.whoAmI == Main.myPlayer && Player.GetToggleValue("Gladiator") && GladiatorCD <= 0 && (projectile == null || projectile.type != ModContent.ProjectileType<GladiatorJavelin>()))
            {
                GladiatorEnchant.GladiatorSpearDrop(this, target, GetBaseDamage());
            }

            if (SolarEnchantActive && Player.GetToggleValue("SolarFlare") && Main.rand.NextBool(4))
                target.AddBuff(ModContent.BuffType<SolarFlare>(), 300);

            if (TinEnchantItem != null)
            {
                TinEnchant.TinOnHitEnemy(this, damage, crit);
            }

            if (LeadEnchantItem != null)
            {
                target.AddBuff(ModContent.BuffType<LeadPoison>(), 30);
            }


            //            /*if (PalladEnchant && !TerrariaSoul && palladiumCD == 0 && !target.immortal && !Player.moonLeech)
            //            {
            //                int heal = damage / 10;

            //                if ((EarthForce) && heal > 16)
            //                    heal = 16;
            //                else if (!EarthForce && !WizardEnchant && heal > 8)
            //                    heal = 8;
            //                else if (heal < 1)
            //                    heal = 1;
            //                Player.statLife += heal;
            //                Player.HealEffect(heal);
            //                palladiumCD = 240;
            //            }*/

            if (NymphsPerfume && NymphsPerfumeCD <= 0 && !target.immortal && !Player.moonLeech)
            {
                NymphsPerfumeCD = 600;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemID.Heart);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)FargowiltasSouls.PacketID.RequestPerfumeHeart);
                    netMessage.Write((byte)Player.whoAmI);
                    netMessage.Write((byte)target.whoAmI);
                    netMessage.Send();
                }
            }

            if (UniverseCore)
                target.AddBuff(ModContent.BuffType<FlamesoftheUniverse>(), 240);

            if (MasochistSoul)
            {
                target.AddBuff(ModContent.BuffType<Sadism>(), 600);
                //if (target.FindBuffIndex(ModContent.BuffType<Sadism>()) < 0 && target.aiStyle != 37)
                //{
                //    if (target.type != ModContent.NPCType<MutantBoss>())
                //    {
                //        target.DelBuff(4);
                //        target.buffImmune[ModContent.BuffType<Sadism>()] = false;
                //    }
                //    target.AddBuff(ModContent.BuffType<Sadism>(), 600);
                //}
            }

            if (FusedLens)
            {
                if (Player.onFire2 || FusedLensCanDebuff)
                    target.AddBuff(BuffID.CursedInferno, 360);
                if (Player.ichor || FusedLensCanDebuff)
                    target.AddBuff(BuffID.Ichor, 360);
            }

            if (!TerrariaSoul)
            {
                if (AncientShadowEnchantActive && Player.GetToggleValue("AncientShadow") && (projectile == null || projectile.type != ProjectileID.ShadowFlame) && Main.rand.NextBool(5))
                    target.AddBuff(BuffID.Darkness, 600, true);
            }

            if (Supercharged)
            {
                target.AddBuff(BuffID.Electrified, 240);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 60);
            }

            if (GoldEnchantActive)
                target.AddBuff(BuffID.Midas, 120, true);

            if (DragonFang && !target.boss && !target.buffImmune[ModContent.BuffType<ClippedWings>()] && Main.rand.NextBool(10))
            {
                target.velocity.X = 0f;
                target.velocity.Y = 10f;
                target.AddBuff(ModContent.BuffType<ClippedWings>(), 240);
                target.netUpdate = true;
            }

            if (SpectreEnchantActive && Player.GetToggleValue("Spectre") && !target.immortal && SpectreCD <= 0 && Main.rand.NextBool())
            {
                if (projectile == null)
                {
                    //forced orb spawn reeeee
                    float num = 4f;
                    float speedX = Main.rand.Next(-100, 101);
                    float speedY = Main.rand.Next(-100, 101);
                    float num2 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                    num2 = num / num2;
                    speedX *= num2;
                    speedY *= num2;
                    Projectile p = FargoSoulsUtil.NewProjectileDirectSafe(Player.GetSource_Misc(""), target.position, new Vector2(speedX, speedY), ProjectileID.SpectreWrath, damage / 2, 0, Player.whoAmI, target.whoAmI);

                    if ((SpiritForce || (crit && Main.rand.NextBool(5))) && p != null)
                    {
                        SpectreHeal(target, p);
                        SpectreCD = SpiritForce ? 5 : 20;
                    }
                }
                else if (projectile.type != ProjectileID.SpectreWrath)
                {
                    SpectreHurt(projectile);

                    if (SpiritForce || (crit && Main.rand.NextBool(5)))
                        SpectreHeal(target, projectile);

                    SpectreCD = SpiritForce ? 5 : 20;
                }
            }

            if (AbomWandItem != null)
            {
                //target.AddBuff(ModContent.BuffType<OceanicMaul>(), 900);
                //target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);
                if (crit && AbomWandCD <= 0 && Player.GetToggleValue("MasoFishron") && (projectile == null || projectile.type != ModContent.ProjectileType<AbomScytheFriendly>()))
                {
                    AbomWandCD = 360;

                    float screenX = Main.screenPosition.X;
                    if (Player.direction < 0)
                        screenX += Main.screenWidth;
                    float screenY = Main.screenPosition.Y;
                    screenY += Main.rand.Next(Main.screenHeight);
                    Vector2 spawn = new Vector2(screenX, screenY);
                    Vector2 vel = target.Center - spawn;
                    vel.Normalize();
                    vel *= 27f;

                    int dam = 150;
                    if (MutantEyeItem != null)
                        dam *= 3;

                    if (projectile != null && FargoSoulsUtil.IsSummonDamage(projectile))
                    {
                        int p = FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(AbomWandItem), spawn, vel, ModContent.ProjectileType<SpectralAbominationn>(), dam, 10f, Player.whoAmI, target.whoAmI);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].DamageType = DamageClass.Summon;
                    }
                    else
                    {
                        dam = (int)(dam * Player.ActualClassDamage(damageClass));

                        int p = Projectile.NewProjectile(Player.GetSource_Accessory(AbomWandItem), spawn, vel, ModContent.ProjectileType<SpectralAbominationn>(), dam, 10f, Player.whoAmI, target.whoAmI);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].DamageType = damageClass;
                    }
                }
            }

            if (DarkenedHeartItem != null)
                DarkenedHeartAttack(projectile);

            if (NebulaEnchantActive)
                NebulaOnHit(target, projectile, damageClass);
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || target.friendly)
                return;

            OnHitNPCEither(target, damage, knockback, crit, item.DamageType, item: item);
        }

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (ShroomEnchantActive && Player.GetToggleValue("ShroomiteShroom"))
                ShroomiteMeleeEffect(item, hitbox);
        }

        public override bool CanBeHitByNPC(NPC npc, ref int CooldownSlot)
        {
            if (BetsyDashing || GoldShell)
                return false;
            return true;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (BetsyDashing || GoldShell)
                return false;
            if (PrecisionSealHurtbox && !proj.Colliding(proj.Hitbox, GetPrecisionHurtbox()))
                return false;
            return true;
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (NecromanticBrewItem != null && IsInADashState)
            {
                damage /= 4;
            }



            if (GladiatorEnchantActive && Player.direction == Math.Sign(npc.Center.X - Player.Center.X))
                Player.noKnockback = true;

            if (Smite)
                damage = (int)(damage * 1.1);

            if (npc.coldDamage && Hypothermia)
                damage = (int)(damage * 1.2);

            if (npc.GetGlobalNPC<FargoSoulsGlobalNPC>().CurseoftheMoon)
                damage = (int)(damage * 0.8);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (GroundStick)
            {
                GroundStickCheck(proj, ref damage);
            }



            if (GladiatorEnchantActive && Player.direction == Math.Sign(proj.Center.X - Player.Center.X))
                Player.noKnockback = true;

            if (Smite)
                damage = (int)(damage * 1.2);

            if (proj.coldDamage && Hypothermia)
                damage = (int)(damage * 1.2);

            //if (npc.GetGlobalNPC<FargoSoulsGlobalNPC>().CurseoftheMoon)
            //damage = (int)(damage * 0.8);
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            OnHitByEither(npc, null, damage, crit);
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            OnHitByEither(null, proj, damage, crit);
        }

        public void OnHitByEither(NPC npc, Projectile proj, int damage, bool crit)
        {
            if (Anticoagulation && Main.myPlayer == Player.whoAmI)
            {
                Entity source = null;
                if (npc != null)
                    source = npc;
                else if (proj != null)
                    source = proj;

                int type = ModContent.ProjectileType<Bloodshed>();
                for (int i = 0; i < 6; i++)
                {
                    if (Main.rand.NextBool(Player.ownedProjectileCounts[type] + 2))
                    {
                        const float speed = 12f;
                        Projectile.NewProjectile(Player.GetSource_OnHurt(source), Player.Center, Main.rand.NextVector2Circular(speed, speed), type, 0, 0f, Main.myPlayer, 0f);
                    }
                }
            }

            if (ModContent.GetInstance<SoulConfig>().BigTossMode)
            {
                AddBuffNoStack(ModContent.BuffType<Stunned>(), 120);

                Vector2 attacker = default;
                if (npc != null)
                    attacker = npc.Center;
                else if (proj != null)
                    attacker = proj.Center;
                if (attacker != default)
                    Player.velocity = Vector2.Normalize(Player.Center - attacker) * 30;
            }
        }

        public void ConcentratedRainbowMatterTryAutoHeal()
        {
            if (ConcentratedRainbowMatter
                && Player.statLife < Player.statLifeMax2
                && Player.potionDelay <= 0
                && Player.GetToggleValue("MasoHealingPotion", false))
            {
                Item potion = Player.QuickHeal_GetItemToUse();
                if (potion != null)
                {
                    int heal = getHealMultiplier(potion.healLife);
                    if (Player.statLife < Player.statLifeMax2 - heal && //only heal when full benefit (no wasted overheal)
                        (Player.statLife < Player.statLifeMax2 * 0.4 || //heal when very low or when danger nearby (not after respawn in safety)
                        Main.npc.Any(n => n.active && n.damage > 0 && !n.friendly
                                     && Player.Distance(n.Center) < 1200 && (n.noTileCollide || Collision.CanHitLine(Player.Center, 0, 0, n.Center, 0, 0)))))
                    {
                        Player.QuickHeal();
                    }
                }
            }
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>()))
                ((NPCs.DeviBoss.DeviBoss)Main.npc[EModeGlobalNPC.deviBoss].ModNPC).playerInvulTriggered = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>()))
                ((NPCs.AbomBoss.AbomBoss)Main.npc[EModeGlobalNPC.abomBoss].ModNPC).playerInvulTriggered = true;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()))
                ((NPCs.MutantBoss.MutantBoss)Main.npc[EModeGlobalNPC.mutantBoss].ModNPC).playerInvulTriggered = true;

            if (TryParryAttack(damage))
                return false;

            if (DeathMarked)
            {
                damage = (int)(damage * 1.5);
            }

            if (Player.whoAmI == Main.myPlayer && !noDodge && SqueakyAcc && Player.GetToggleValue("MasoSqueak") && Main.rand.NextBool(10))
            {
                Squeak(Player.Center);
                damage = 1;
            }

            if (CrimsonEnchantActive && Player.GetToggleValue("Crimson"))
            {
                CrimsonEnchant.CrimsonHurt(Player, this, ref damage);
            }

            if (StyxSet && !BetsyDashing && !GoldShell && damage > 1 && Player.ownedProjectileCounts[ModContent.ProjectileType<StyxArmorScythe>()] > 0)
            {
                int scythesSacrificed = 0;
                const int maxSacrifice = 4;
                const double maxDR = 0.20;
                int scytheType = ModContent.ProjectileType<StyxArmorScythe>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == scytheType && Main.projectile[i].owner == Player.whoAmI)
                    {
                        if (Player.whoAmI == Main.myPlayer)
                            Main.projectile[i].Kill();
                        if (++scythesSacrificed >= maxSacrifice)
                            break;
                    }
                }

                damage = (int)(damage * (1.0 - maxDR / maxSacrifice * scythesSacrificed));
            }

            if (DeerSinewNerf && DeerSinewFreezeCD <= 0 && (damageSource.SourceNPCIndex != -1 || (damageSource.SourceProjectileIndex != -1 && Main.projectile[damageSource.SourceProjectileIndex].aiStyle != ProjAIStyleID.FallingTile)))
            {
                DeerSinewFreezeCD = 120;
                FargoSoulsUtil.AddDebuffFixedDuration(Player, BuffID.Frozen, 20);
            }

            return true;
        }

        public void OnHurtEffects(double damage)
        {
            if (HurtTimer <= 0)
            {
                HurtTimer = 20;

                if (CelestialRuneItem != null && Player.GetToggleValue("MasoVision"))
                {
                    if (MoonChalice)
                    {
                        int dam = 50;
                        if (MasochistSoul)
                            dam *= 2;
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), Player.Center, Main.rand.NextVector2Circular(20, 20),
                                    ModContent.ProjectileType<AncientVision>(), (int)(dam * Player.ActualClassDamage(DamageClass.Summon)), 6f, Player.whoAmI);
                        }
                    }
                    else
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(CelestialRuneItem), Player.Center, new Vector2(0, -10), ModContent.ProjectileType<AncientVision>(),
                            (int)(40 * Player.ActualClassDamage(DamageClass.Summon)), 3f, Player.whoAmI);
                    }
                }
            }

            if (CobaltEnchantItem != null)
                CobaltEnchant.CobaltHurt(Player, damage);

            if (FossilEnchantItem != null)
                FossilEnchant.FossilHurt(this, (int)damage);
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            WasHurtBySomething = true;

            MahoganyCanUseDR = false;
			
			if (Player.HasBuff(ModContent.BuffType<TitaniumDRBuff>())
				&& !Player.HasBuff(ModContent.BuffType<TitaniumCD>()))
			{
				Player.AddBuff(ModContent.BuffType<TitaniumCD>(), 60 * 10);
			}

            if (NekomiSet)
            {
                const int heartsLost = 1;
                int meterPerHeart = NekomiHood.MAX_METER / NekomiHood.MAX_HEARTS;
                int meterLost = meterPerHeart * heartsLost;

                int heartsToConsume = NekomiMeter / meterPerHeart;
                if (heartsToConsume > heartsLost)
                    heartsToConsume = heartsLost;
                Player.AddBuff(BuffID.RapidHealing, heartsToConsume * 60 * 5 / heartsLost);

                NekomiMeter -= meterLost;
                if (NekomiMeter < 0)
                    NekomiMeter = 0;
            }

            if (BeetleEnchantActive)
                BeetleHurt();

            if (TinEnchantItem != null)
                TinEnchant.TinHurt(this);

            if (ShellHide)
            {
                TurtleShellHP--;

                //some funny dust
                const int max = 30;
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 5f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + Main.LocalPlayer.Center;
                    Vector2 vector7 = vector6 - Main.LocalPlayer.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.GoldFlame, 0f, 0f, 0, default(Color), 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }
            }

            OnHurtEffects(damage);

            if (Midas && Main.myPlayer == Player.whoAmI)
                Player.DropCoins();

            DeviGrazeBonus = 0;
            DeviGrazeCounter = 0;
        }

        private PlayerDeathReason DeathByLocalization(string key)
        {
            string death = Language.GetTextValue($"Mods.FargowiltasSouls.DeathMessage.{key}", Player.name);
            return PlayerDeathReason.ByCustomReason(death);
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retVal = true;

            if (Player.statLife <= 0) //revives
            {
                if (Player.whoAmI == Main.myPlayer && retVal && AbomRebirth)
                {
                    if (!WasHurtBySomething)
                    {
                        Player.statLife = 1;
                        return false; //short circuits the rest, this is deliberate
                    }
                }

                if (Player.whoAmI == Main.myPlayer && retVal && MutantSetBonusItem != null && Player.FindBuffIndex(ModContent.BuffType<MutantRebirth>()) == -1)
                {
                    Player.statLife = Player.statLifeMax2;
                    Player.HealEffect(Player.statLifeMax2);
                    Player.immune = true;
                    Player.immuneTime = 180;
                    Player.hurtCooldowns[0] = 180;
                    Player.hurtCooldowns[1] = 180;
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.Revived");
                    Main.NewText(text, Color.LimeGreen);
                    Player.AddBuff(ModContent.BuffType<MutantRebirth>(), 120 * 60);
                    retVal = false;

                    Projectile.NewProjectile(Player.GetSource_Accessory(MutantSetBonusItem), Player.Center, -Vector2.UnitY, ModContent.ProjectileType<GiantDeathray>(), (int)(7000 * Player.ActualClassDamage(DamageClass.Magic)), 10f, Player.whoAmI);
                }

                if (Player.whoAmI == Main.myPlayer && retVal && FossilEnchantItem != null && Player.FindBuffIndex(ModContent.BuffType<FossilReviveCD>()) == -1)
                {
                    FossilEnchant.FossilRevive(this);
                    retVal = false;
                }

                if (Player.whoAmI == Main.myPlayer && retVal && AbomWandItem != null && !AbominableWandRevived)
                {
                    AbominableWandRevived = true;
                    int heal = 1;
                    Player.statLife = heal;
                    Player.HealEffect(heal);
                    Player.immune = true;
                    Player.immuneTime = 120;
                    Player.hurtCooldowns[0] = 120;
                    Player.hurtCooldowns[1] = 120;
                    string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.Revived");
                    CombatText.NewText(Player.Hitbox, Color.Yellow, text, true);
                    Main.NewText(text, Color.Yellow);
                    Player.AddBuff(ModContent.BuffType<AbomRebirth>(), 900);
                    retVal = false;
                    for (int i = 0; i < 24; i++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(AbomWandItem), Player.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 16f),
                            ModContent.ProjectileType<StyxArmorScythe2>(), 0, 10f, Main.myPlayer, -60 - Main.rand.Next(60), -1);
                    }
                }
            }

            //killed by damage over time
            if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (Infested)
                    damageSource = DeathByLocalization("Infested");

                if (Anticoagulation)
                    damageSource = DeathByLocalization("Anticoagulation");

                if (Rotting)
                    damageSource = DeathByLocalization("Rotting");

                if (Shadowflame)
                    damageSource = DeathByLocalization("Shadowflame");

                if (NanoInjection)
                    damageSource = DeathByLocalization("NanoInjection");

                if (GodEater || FlamesoftheUniverse || CurseoftheMoon || MutantFang)
                    damageSource = DeathByLocalization("DivineWrath");
            }

            /*if (MutantPresence)
            {
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was penetrated.");
            }*/

            if (StatLifePrevious > 0 && Player.statLife > StatLifePrevious)
                StatLifePrevious = Player.statLife;

            return retVal;
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (GaiaOffense) //set armor and accessory shaders to gaia shader if set bonus is triggered
            {
                int gaiaShader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<GaiaDye>());
                drawInfo.cBody = gaiaShader;
                drawInfo.cHead = gaiaShader;
                drawInfo.cLegs = gaiaShader;
                drawInfo.cWings = gaiaShader;
                drawInfo.cHandOn = gaiaShader;
                drawInfo.cHandOff = gaiaShader;
                drawInfo.cShoe = gaiaShader;
            }

            if (GuardRaised)
            {
                Player.bodyFrame.Y = Player.bodyFrame.Height * 10;
                if (shieldTimer > 0)
                {
                    List<int> shaders = new List<int>();
                    shaders.Add(GameShaders.Armor.GetShaderIdFromItemId(ItemID.ReflectiveSilverDye));
                    if (DreadShellItem != null)
                        shaders.Add(GameShaders.Armor.GetShaderIdFromItemId(ItemID.BloodbathDye));
                    if (PumpkingsCapeItem != null)
                        shaders.Add(GameShaders.Armor.GetShaderIdFromItemId(ItemID.PixieDye));

                    int shader = shaders[(int)(Main.GameUpdateCount / 4 % shaders.Count)];
                    drawInfo.cBody = shader;
                    drawInfo.cHead = shader;
                    drawInfo.cLegs = shader;
                    drawInfo.cWings = shader;
                    drawInfo.cHandOn = shader;
                    drawInfo.cHandOff = shader;
                    drawInfo.cShoe = shader;
                    drawInfo.cBack = shader;
                    drawInfo.cBackpack = shader;
                    drawInfo.cShield = shader;
                    drawInfo.cNeck = shader;
                    drawInfo.cHandOn = shader;
                    drawInfo.cHandOff = shader;
                    drawInfo.cBalloon = shader;
                    drawInfo.cBalloonFront = shader;
                    drawInfo.cFace = shader;
                    drawInfo.cFaceHead = shader;
                    drawInfo.cFront = shader;
                }
            }
        }

        public void AddPet(bool toggle, bool vanityToggle, int buff, int proj)
        {
            if (vanityToggle)
            {
                PetsActive = false;
                return;
            }

            if (Player.whoAmI == Main.myPlayer && toggle && Player.FindBuffIndex(buff) == -1 && Player.ownedProjectileCounts[proj] < 1)
            {
                Projectile p = Main.projectile[Projectile.NewProjectile(Player.GetSource_Misc("Pet"), Player.Center.X, Player.Center.Y, 0f, -1f, proj, 0, 0f, Player.whoAmI)];
                p.netUpdate = true;
            }
        }

        public void AddMinion(Item item, bool toggle, int proj, int damage, float knockback)
        {
            if (Player.whoAmI != Main.myPlayer) return;
            if (Player.ownedProjectileCounts[proj] < 1 && Player.whoAmI == Main.myPlayer && toggle)
            {
                FargoSoulsUtil.NewSummonProjectile(Player.GetSource_Accessory(item), Player.Center, -Vector2.UnitY, proj, damage, knockback, Main.myPlayer);
            }
        }

        private void KillPets()
        {
            int petId = Player.miscEquips[0].buffType;
            int lightPetId = Player.miscEquips[1].buffType;

            Player.buffImmune[petId] = true;
            Player.buffImmune[lightPetId] = true;

            Player.ClearBuff(petId);
            Player.ClearBuff(lightPetId);

            //memorizes Player selections
            if (!WasAsocial)
            {
                HidePetToggle0 = Player.hideMisc[0];
                HidePetToggle1 = Player.hideMisc[1];

                WasAsocial = true;
            }

            //disables pet and light pet too!
            if (!Player.hideMisc[0])
            {
                Player.TogglePet();
            }

            if (!Player.hideMisc[1])
            {
                Player.ToggleLight();
            }

            Player.hideMisc[0] = true;
            Player.hideMisc[1] = true;
        }

        public void Squeak(Vector2 center)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(new SoundStyle($"FargowiltasSouls/Sounds/SqueakyToy/squeak{Main.rand.Next(1, 7)}"), center);
        }

        private int InfestedExtraDot()
        {
            int buffIndex = Player.FindBuffIndex(ModContent.BuffType<Infested>());
            if (buffIndex == -1)
            {
                buffIndex = Player.FindBuffIndex(ModContent.BuffType<Neurotoxin>());
                if (buffIndex == -1)
                    return 0;
            }

            int timeLeft = Player.buffTime[buffIndex];
            float baseVal = (float)(MaxInfestTime - timeLeft) / 90; //change the denominator to adjust max power of DOT
            int modifier = (int)(baseVal * baseVal + 4);

            InfestedDust = baseVal / 10 + 1f;
            if (InfestedDust > 5f)
                InfestedDust = 5f;

            return modifier * 2;
        }

        //        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        //        {
        //            if (bait.type == ModContent.ItemType<TruffleWormEX>())
        //            {
        //                caughtType = 0;
        //                bool spawned = false;
        //                for (int i = 0; i < 1000; i++)
        //                {
        //                    if (Main.projectile[i].active && Main.projectile[i].bobber
        //                        && Main.projectile[i].owner == Player.whoAmI && Player.whoAmI == Main.myPlayer)
        //                    {
        //                        Main.projectile[i].ai[0] = 2f; //cut fishing lines
        //                        Main.projectile[i].netUpdate = true;

        //                        if (!spawned && Main.projectile[i].wet && FargoSoulsWorld.EternityMode && !NPC.AnyNPCs(NPCID.DukeFishron)) //should spawn boss
        //                        {
        //                            spawned = true;
        //                            if (Main.netMode == NetmodeID.SinglePlayer) //singlePlayer
        //                            {
        //                                EModeGlobalNPC.spawnFishronEX = true;
        //                                NPC.NewNPC((int)Main.projectile[i].Center.X, (int)Main.projectile[i].Center.Y + 100,
        //                                    NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, Player.whoAmI);
        //                                EModeGlobalNPC.spawnFishronEX = false;
        //                                Main.NewText("Duke Fishron EX has awoken!", 50, 100, 255);
        //                            }
        //                            else if (Main.netMode == NetmodeID.MultiPlayerClient) //MP, broadcast(?) packet from spawning Player's client
        //                            {
        //                                var netMessage = mod.GetPacket();
        //                                netMessage.Write((byte)FargowiltasSouls.PacketID.SpawnFishronEX);
        //                                netMessage.Write((byte)Player.whoAmI);
        //                                netMessage.Write((int)Main.projectile[i].Center.X);
        //                                netMessage.Write((int)Main.projectile[i].Center.Y + 100);
        //                                netMessage.Send();
        //                            }
        //                            else if (Main.netMode == NetmodeID.Server)
        //                            {
        //                                ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("???????"), Color.White);
        //                            }
        //                        }
        //                    }
        //                }
        //                if (spawned)
        //                {
        //                    bait.stack--;
        //                    if (bait.stack <= 0)
        //                        bait.SetDefaults(0);
        //                }
        //            }
        //        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (GuttedHeart)
                GuttedHeartNurseHeal();
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (weapon.CountsAsClass(DamageClass.Ranged))
            {
                if (RangedEssence && Main.rand.NextBool(10))
                    return false;
                if (RangedSoul && Main.rand.NextBool(5))
                    return false;
            }
            if (GaiaSet && Main.rand.NextBool(10))
                return false;
            return true;
        }

        public int frameCounter = 0;
        public int frameSnow = 1;
        public int frameMutantAura = 0;
        //public int frameMutantLightning = 0;

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (BetsyDashing || ShellHide || GoldShell)
            {
                foreach (var layer in PlayerDrawLayerLoader.Layers)
                {
                    //if (layer.Mod == null) //Only hide vanilla layers, optional. Does not hide modded layers added as Between or Multiple, but will hide layers marked as Before/After regardless
                    //{
                    layer.Hide();
                    //}
                }
            }

            //if (SquirrelMount)
            //{
            //    foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
            //    {
            //        layer.


            //        if (layer != PlayerLayer.MountBack && PlayerLayer != PlayerLayer.MountFront && PlayerLayer != PlayerLayer.MiscEffectsFront && PlayerLayer != PlayerLayer.MiscEffectsBack)
            //        {
            //            PlayerLayer.visible = false;
            //        }
            //    }
            //}

        }


        //public override void HideDrawLayers(PlayerDrawSet drawInfo)
        //{
        //    //base.HideDrawLayers(drawInfo);

        //    if (BetsyDashing || ShellHide || GoldShell)
        //    {

        //        //drawInfo.headOnlyRender = true;
        //        drawInfo.DrawDataCache.Clear();
        //        //drawInfo.drawPlayer.invis = true;
        //    }
        //}

        //public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        //{
        //    if (BetsyDashing || ShellHide || GoldShell)
        //    {
        //        drawInfo.DrawDataCache.Clear();
        //    }
        //}

        public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
        {

            //if (BetsyDashing || ShellHide || GoldShell) //dont draw Player 
            //{
            //    foreach (KeyValuePair<PlayerDrawLayer, PlayerDrawLayer.Position> entry in positions)
            //    {
            //        positions.Remove(entry.Key);
            //    }
            //}



            //            if (SquirrelMount)
            //            {
            //                foreach (PlayerLayer PlayerLayer in layers)
            //                {
            //                    if (PlayerLayer != PlayerLayer.MountBack && PlayerLayer != PlayerLayer.MountFront && PlayerLayer != PlayerLayer.MiscEffectsFront && PlayerLayer != PlayerLayer.MiscEffectsBack)
            //                    {
            //                        PlayerLayer.visible = false;
            //                    }
            //                }
            //            }
        }

        public int getHealMultiplier(int heal)
        {
            float bonus = 0f;

            if ((SquireEnchantActive || ValhallaEnchantActive) && Player.GetToggleValue("Valhalla", false))
            {
                if (Eternity)
                    bonus = 4f;
                else if (WillForce && ValhallaEnchantActive)
                    bonus = 1f / 2f;
                else if (ValhallaEnchantActive || (WillForce && SquireEnchantActive))
                    bonus = 1f / 3f;
                else if (SquireEnchantActive)
                    bonus = 1f / 4f;
            }

            heal = (int)(heal * (1f + bonus));

            return heal;
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            healValue = getHealMultiplier(healValue);
        }

        public void HealPlayer(int amount)
        {
            amount = getHealMultiplier(amount);

            Player.statLife += amount;
            if (Player.statLife > Player.statLifeMax2)
                Player.statLife = Player.statLifeMax2;
            Player.HealEffect(amount);
        }

        public override void ModifyScreenPosition()
        {
            if (Screenshake > 0)
                Main.screenPosition += Main.rand.NextVector2Circular(7, 7);
        }

        public override void clientClone(ModPlayer clientClone)
        {
            FargoSoulsPlayer modPlayer = clientClone as FargoSoulsPlayer;
            modPlayer.Toggler = Toggler;
        }

        public void SyncToggle(string key)
        {
            if (!TogglesToSync.ContainsKey(key))
                TogglesToSync.Add(key, Player.GetToggle(key).ToggleBool);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            foreach (KeyValuePair<string, bool> toggle in TogglesToSync)
            {
                ModPacket packet = Mod.GetPacket();

                packet.Write((byte)FargowiltasSouls.PacketID.SyncOneToggle);
                packet.Write((byte)Player.whoAmI);
                packet.Write(toggle.Key);
                packet.Write(toggle.Value);

                packet.Send(toWho, fromWho);
            }

            TogglesToSync.Clear();
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            FargoSoulsPlayer modPlayer = clientPlayer as FargoSoulsPlayer;
            if (modPlayer.Toggler.Toggles != Toggler.Toggles)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)FargowiltasSouls.PacketID.SyncTogglesOnJoin);
                packet.Write((byte)Player.whoAmI);
                packet.Write((byte)Toggler.Toggles.Count);

                for (int i = 0; i < Toggler.Toggles.Count; i++)
                {
                    packet.Write(Toggler.Toggles.Values.ElementAt(i).ToggleBool);
                }

                packet.Send();
            }
        }

        public void AddBuffNoStack(int buff, int duration)
        {
            if (!Player.HasBuff(buff) && ReallyAwfulDebuffCooldown <= 0)
            {
                Player.AddBuff(buff, duration);
                int d = Player.FindBuffIndex(buff);
                if (d != -1) //if debuff successfully applied
                    ReallyAwfulDebuffCooldown = Player.buffTime[d] + 240;
            }
        }

        public void TryAdditionalAttacks(int damage, DamageClass damageType)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

            if (CactusEnchantActive)
            {
                CactusEnchant.CactusSelfProc(this);
            }

            if (BorealEnchantItem != null && Player.GetToggleValue("Boreal") && BorealCD <= 0)
            {
                BorealCD = 60;

                if (WoodForce)
                {
                    BorealCD = 30;
                }

                BorealWoodEnchant.BorealSnowballs(this, damage);
            }

            if (AdditionalAttacks && AdditionalAttacksTimer <= 0)
            {
                AdditionalAttacksTimer = 60;

                if (CelestialRuneItem != null && Player.GetToggleValue("MasoCelest"))
                {
                    CelestialRuneSupportAttack(damage, damageType);
                }
            }
        }

        public Rectangle GetPrecisionHurtbox()
        {
            Rectangle hurtbox = Player.Hitbox;
            hurtbox.X += hurtbox.Width / 2;
            hurtbox.Y += hurtbox.Height / 2;
            hurtbox.Width = Math.Min(hurtbox.Width, hurtbox.Height);
            hurtbox.Height = Math.Min(hurtbox.Width, hurtbox.Height);
            hurtbox.X -= hurtbox.Width / 2;
            hurtbox.Y -= hurtbox.Height / 2;
            return hurtbox;
        }

        
    }
}
