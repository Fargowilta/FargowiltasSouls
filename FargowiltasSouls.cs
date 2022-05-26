using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Dyes;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.EternityMode;
using FargowiltasSouls.Shaders;
using FargowiltasSouls.Sky;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using FargowiltasSouls.ModCompatibilities;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls : Mod
    {
        internal static ModKeybind FreezeKey;
        internal static ModKeybind GoldKey;
        internal static ModKeybind SmokeBombKey;
        internal static ModKeybind BetsyDashKey;
        internal static ModKeybind MutantBombKey;
        internal static ModKeybind SoulToggleKey;
        internal static ModKeybind PrecisionSealKey;

        internal static List<int> DebuffIDs;

        internal static FargowiltasSouls Instance;

        internal bool LoadedNewSprites;

        internal static float OldMusicFade;

        public UserInterface CustomResources;

        internal static Dictionary<int, int> ModProjDict = new Dictionary<int, int>();

        internal struct TextureBuffer
        {
            public static readonly Dictionary<int, Asset<Texture2D>> NPC = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> NPCHeadBoss = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> Gore = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> Golem = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> Extra = new Dictionary<int, Asset<Texture2D>>();
            public static Asset<Texture2D> Ninja = null;
            public static Asset<Texture2D> BoneArm = null;
            public static Asset<Texture2D> BoneArm2 = null;
            public static Asset<Texture2D> Chain12 = null;
            public static Asset<Texture2D> Chain26 = null;
            public static Asset<Texture2D> Chain27 = null;
            public static Asset<Texture2D> Wof = null;
        }

        public static UIManager UserInterfaceManager => Instance._userInterfaceManager;
        private UIManager _userInterfaceManager;

        //        #region Compatibilities

        //        public CalamityCompatibility CalamityCompatibility { get; private set; }
        //        public bool CalamityLoaded => CalamityCompatibility != null;

        //        public ThoriumCompatibility ThoriumCompatibility { get; private set; }
        //        public bool ThoriumLoaded => ThoriumCompatibility != null;

        //        public SoACompatibility SoACompatibility { get; private set; }
        //        public bool SoALoaded => SoACompatibility != null;

        //        public MasomodeEXCompatibility MasomodeEXCompatibility { get; private set; }
        //        public bool MasomodeEXLoaded => MasomodeEXCompatibility != null;

        //        public BossChecklistCompatibility BossChecklistCompatibility { get; private set; }
        //        public bool BossChecklistLoaded => BossChecklistCompatibility != null;

        //        #endregion Compatibilities

        //public Fargowiltas()
        //{
        //    Properties = new ModProperties
        //    {
        //        Autoload = true,
        //        AutoloadGores = true,
        //        AutoloadSounds = true
        //    };
        //}

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

            SkyManager.Instance["FargowiltasSouls:MoonLordSky"] = new MoonLordSky();

            FreezeKey = KeybindLoader.RegisterKeybind(this, "Freeze", "P");
            GoldKey = KeybindLoader.RegisterKeybind(this, "Turn Gold", "O");
            SmokeBombKey = KeybindLoader.RegisterKeybind(this, "Throw Smoke Bomb", "I");
            BetsyDashKey = KeybindLoader.RegisterKeybind(this, "Fireball Dash", "C");
            MutantBombKey = KeybindLoader.RegisterKeybind(this, "Mutant Bomb", "Z");
            SoulToggleKey = KeybindLoader.RegisterKeybind(this, "Open Soul Toggler", ".");
            PrecisionSealKey = KeybindLoader.RegisterKeybind(this, "Precision Movement", "LeftShift");

            ToggleLoader.Load();

            _userInterfaceManager = new UIManager();
            _userInterfaceManager.LoadUI();

            AddLocalizations();

            if (Main.netMode != NetmodeID.Server)
            {
                #region shaders

                //loading refs for shaders
                Ref<Effect> lcRef = new Ref<Effect>(Assets.Request<Effect>("Effects/LifeChampionShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> wcRef = new Ref<Effect>(Assets.Request<Effect>("Effects/WillChampionShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> gaiaRef = new Ref<Effect>(Assets.Request<Effect>("Effects/GaiaShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> textRef = new Ref<Effect>(Assets.Request<Effect>("Effects/TextShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> invertRef = new Ref<Effect>(Assets.Request<Effect>("Effects/Invert", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> finalSparkRef = new Ref<Effect>(Assets.Request<Effect>("Effects/FinalSpark", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> shockwaveRef = new Ref<Effect>(Assets.Request<Effect>("Effects/ShockwaveEffect", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.

                //loading shaders from refs
                GameShaders.Misc["LCWingShader"] = new MiscShaderData(lcRef, "LCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<LifeDye>(), new ArmorShaderData(lcRef, "LCArmor").UseColor(new Color(1f, 0.647f, 0.839f)).UseSecondaryColor(Color.Goldenrod));

                GameShaders.Misc["WCWingShader"] = new MiscShaderData(wcRef, "WCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<WillDye>(), new ArmorShaderData(wcRef, "WCArmor").UseColor(Color.DarkOrchid).UseSecondaryColor(Color.LightPink).UseImage("Images/Misc/noise"));

                GameShaders.Misc["GaiaShader"] = new MiscShaderData(gaiaRef, "GaiaGlow");
                GameShaders.Armor.BindShader(ModContent.ItemType<GaiaDye>(), new ArmorShaderData(gaiaRef, "GaiaArmor").UseColor(new Color(0.44f, 1, 0.09f)).UseSecondaryColor(new Color(0.5f, 1f, 0.9f)));

                GameShaders.Misc["PulseUpwards"] = new MiscShaderData(textRef, "PulseUpwards");
                GameShaders.Misc["PulseDiagonal"] = new MiscShaderData(textRef, "PulseDiagonal");
                GameShaders.Misc["PulseCircle"] = new MiscShaderData(textRef, "PulseCircle");

                Filters.Scene["FargowiltasSouls:FinalSpark"] = new Filter(new FinalSparkShader(finalSparkRef, "FinalSpark"), EffectPriority.High);
                Filters.Scene["FargowiltasSouls:Invert"] = new Filter(new TimeStopShader(invertRef, "Main"), EffectPriority.VeryHigh);

                Filters.Scene["FargowiltasSouls:Solar"] = new Filter(Filters.Scene["MonolithSolar"].GetShader(), EffectPriority.Medium);
                Filters.Scene["FargowiltasSouls:Vortex"] = new Filter(Filters.Scene["MonolithVortex"].GetShader(), EffectPriority.Medium);
                Filters.Scene["FargowiltasSouls:Nebula"] = new Filter(Filters.Scene["MonolithNebula"].GetShader(), EffectPriority.Medium);
                Filters.Scene["FargowiltasSouls:Stardust"] = new Filter(Filters.Scene["MonolithStardust"].GetShader(), EffectPriority.Medium);

                //Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(shockwaveRef, "Shockwave"), EffectPriority.VeryHigh);
                //Filters.Scene["Shockwave"].Load();

                #endregion shaders
            }

            //            PatreonMiscMethods.Load(this);

            //On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanDrop += IsMasterModeOrEMode_CanDrop;
            //On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanShowItemDropInUI += IsMasterModeOrEMode_CanShowItemDropInUI;
            //On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.CanDrop += DropBasedOnMasterOrEMode_CanDrop;
            //On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction += DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction;
        }

        private static bool IsMasterModeOrEMode_CanDrop(
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanDrop orig,
            Conditions.IsMasterMode self, DropAttemptInfo info)
        {
            // Use | instead of || so orig runs no matter what.
            return FargoSoulsWorld.EternityMode | orig(self, info);
        }

        private static bool IsMasterModeOrEMode_CanShowItemDropInUI(
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanShowItemDropInUI orig,
            Conditions.IsMasterMode self)
        {
            // Use | instead of || so orig runs no matter what.
            return FargoSoulsWorld.EternityMode | orig(self);
        }

        private static bool DropBasedOnMasterOrEMode_CanDrop(
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_CanDrop orig,
            DropBasedOnMasterMode self, DropAttemptInfo info)
        {
            // Use | instead of || so orig runs no matter what.
            return (FargoSoulsWorld.EternityMode && self.ruleForMasterMode.CanDrop(info)) | orig(self, info);
        }

        private static ItemDropAttemptResult DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction(
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction orig,
            DropBasedOnMasterMode self, DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
        {
            ItemDropAttemptResult itemDropAttemptResult = orig(self, info, resolveAction);
            return FargoSoulsWorld.EternityMode ? resolveAction(self.ruleForMasterMode, info) : itemDropAttemptResult;
        }

        public override void Unload()
        {
            //On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanDrop -= IsMasterModeOrEMode_CanDrop;
            //On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanShowItemDropInUI -= IsMasterModeOrEMode_CanShowItemDropInUI;
            //On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.CanDrop -= DropBasedOnMasterOrEMode_CanDrop;
            //On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction -= DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction;

            NPC.LunarShieldPowerExpert = 150;

            void RestoreSprites(Dictionary<int, Asset<Texture2D>> buffer, Asset<Texture2D>[] original)
            {
                foreach (KeyValuePair<int, Asset<Texture2D>> pair in buffer)
                    original[pair.Key] = pair.Value;

                buffer.Clear();
            }

            RestoreSprites(TextureBuffer.NPC, TextureAssets.Npc);
            RestoreSprites(TextureBuffer.NPCHeadBoss, TextureAssets.NpcHeadBoss);
            RestoreSprites(TextureBuffer.Gore, TextureAssets.Gore);
            RestoreSprites(TextureBuffer.Golem, TextureAssets.Golem);
            RestoreSprites(TextureBuffer.Extra, TextureAssets.Extra);

            if (TextureBuffer.Ninja != null)
                TextureAssets.Ninja = TextureBuffer.Ninja;
            if (TextureBuffer.BoneArm != null)
                TextureAssets.BoneArm = TextureBuffer.BoneArm;
            if (TextureBuffer.BoneArm2 != null)
                TextureAssets.BoneArm2 = TextureBuffer.BoneArm2;
            if (TextureBuffer.Chain12 != null)
                TextureAssets.Chain12 = TextureBuffer.Chain12;
            if (TextureBuffer.Chain26 != null)
                TextureAssets.Chain26 = TextureBuffer.Chain26;
            if (TextureBuffer.Chain27 != null)
                TextureAssets.Chain27 = TextureBuffer.Chain27;
            if (TextureBuffer.Wof != null)
                TextureAssets.Wof = TextureBuffer.Wof;

            ToggleLoader.Unload();
            EModeNPCBehaviour.Unload();

            SoulToggler.RemoveItemTags = null;
            ToggleBackend.ConfigPath = null;

            FreezeKey = null;
            GoldKey = null;
            SmokeBombKey = null;
            BetsyDashKey = null;
            MutantBombKey = null;
            SoulToggleKey = null;
            PrecisionSealKey = null;

            if (DebuffIDs != null)
                DebuffIDs.Clear();

            ModProjDict.Clear();

            _userInterfaceManager = null;

            Instance = null;
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
                        return FargoSoulsWorld.EternityMode;

                    case "Masomode":
                    case "MasoMode":
                    case "MasochistMode":
                    case "ForgottenMode":
                    case "Forgor":
                    case "ForgorMode":
                    case "MasomodeReal":
                    case "MasoModeReal":
                    case "MasochistModeReal":
                    case "RealMode":
                    case "GetReal":
                        return FargoSoulsWorld.MasochistModeReal;

                    case "DownedMutant":
                        return FargoSoulsWorld.downedMutant;

                    case "DownedAbom":
                    case "DownedAbominationn":
                        return FargoSoulsWorld.downedAbom;

                    case "DownedChamp":
                    case "DownedChampion": //arg is internal string name of champ
                        return FargoSoulsWorld.downedBoss[(int)Enum.Parse<FargoSoulsWorld.Downed>(args[1] as string, true)];

                    case "DownedEri":
                    case "DownedEridanus":
                    case "DownedCosmos":
                    case "DownedCosmosChamp":
                    case "DownedCosmosChampion":
                        return FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CosmosChampion];

                    case "DownedDevi":
                    case "DownedDeviantt":
                        return FargoSoulsWorld.downedDevi;

                    case "DownedFishronEX":
                        return FargoSoulsWorld.downedFishronEX;

                    case "PureHeart":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().PureHeart;

                    case "MutantAntibodies":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantAntibodies;

                    case "SinisterIcon":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().SinisterIcon;

                    case "AbomAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());

                    case "MutantAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());

                    case "DeviAlive":
                    case "DeviBossAlive":
                    case "DevianttAlive":
                    case "DevianttBossAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());

                    case "MutantPact":
                    case "MutantsPact":
                    case "MutantCreditCard":
                    case "MutantsCreditCard":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantsCreditCard;

                    case "MutantDiscountCard":
                    case "MutantsDiscountCard":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantsDiscountCard;

                    case "EridanusArmor":
                    case "EridanusArmour":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().EridanusEmpower;

                    case "GiftsReceived":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().ReceivedMasoGift;

                    case "GiveDevianttGifts":
                        Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().ReceivedMasoGift = true;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            DropDevianttsGift(Main.LocalPlayer);
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = GetPacket(); // Broadcast item request to server
                            netMessage.Write((byte)PacketID.RequestDeviGift);
                            netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                            netMessage.Send();
                        }
                        Main.npcChatText = "This world looks tougher than usual, so you can have these on the house just this once! Talk to me if you need any tips, yeah?";
                        break;

                    case "SummonCrit":
                    case "SummonCritChance":
                    case "GetSummonCrit":
                    case "GetSummonCritChance":
                    case "SummonerCrit":
                    case "SummonerCritChance":
                    case "GetSummonerCrit":
                    case "GetSummonerCritChance":
                    case "MinionCrit":
                    case "MinionCritChance":
                    case "GetMinionCrit":
                    case "GetMinionCritChance":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().SpiderEnchantActive ? (int)Main.LocalPlayer.ActualClassCrit(DamageClass.Summon) : 0;
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
            Item.NewItem(null, player.Center, ItemID.SilverPickaxe);
            Item.NewItem(null, player.Center, ItemID.SilverAxe);
            Item.NewItem(null, player.Center, ItemID.SilverHammer);

            Item.NewItem(null, player.Center, ItemID.Torch, 100);
            Item.NewItem(null, player.Center, ItemID.LifeCrystal, 4);
            Item.NewItem(null, player.Center, ItemID.ManaCrystal, 4);
            Item.NewItem(null, player.Center, ItemID.RecallPotion, 15);
            if (Main.netMode != NetmodeID.SinglePlayer)
                Item.NewItem(null, player.Center, ItemID.WormholePotion, 15);

            //Item.NewItem(null, player.Center, ModContent.ItemType<DevianttsSundial>());
            Item.NewItem(null, player.Center, ModContent.ItemType<EternityAdvisor>());

            void GiveItem(string modName, string itemName, int amount = 1)
            {
                if (ModContent.TryFind(modName, itemName, out ModItem modItem))
                    Item.NewItem(null, player.Center, modItem.Type, amount);
            }

            GiveItem("Fargowiltas", "AutoHouse", 5);
            GiveItem("Fargowiltas", "MiniInstaBridge", 5);
            GiveItem("Fargowiltas", "HalfInstavator");

            Item.NewItem(null, player.Center, ModContent.ItemType<EurusSock>());
            Item.NewItem(null, player.Center, ModContent.ItemType<PuffInABottle>());
            Item.NewItem(null, player.Center, ItemID.BugNet);
            Item.NewItem(null, player.Center, ItemID.GrapplingHook);

            //only give once per world
            if (!FargoSoulsWorld.ReceivedTerraStorage)
            {
                if (ModLoader.TryGetMod("MagicStorage", out Mod magicStorage))
                {
                    GiveItem("MagicStorage", "StorageHeart");
                    GiveItem("MagicStorage", "CraftingAccess");
                    GiveItem("MagicStorage", "StorageUnit", 16);

                    FargoSoulsWorld.ReceivedTerraStorage = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData); //sync world in mp
                }
                else if (ModLoader.TryGetMod("MagicStorageExtra", out Mod magicStorageExtra))
                {
                    GiveItem("MagicStorageExtra", "StorageHeart");
                    GiveItem("MagicStorageExtra", "CraftingAccess");
                    GiveItem("MagicStorageExtra", "StorageUnit", 16);

                    FargoSoulsWorld.ReceivedTerraStorage = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData); //sync world in mp
                }
            }
        }

        //bool sheet
        public override void PostSetupContent()
        {
            try
            {
                //CalamityCompatibility = new CalamityCompatibility(this).TryLoad() as CalamityCompatibility;
                //ThoriumCompatibility = new ThoriumCompatibility(this).TryLoad() as ThoriumCompatibility;
                //SoACompatibility = new SoACompatibility(this).TryLoad() as SoACompatibility;
                //MasomodeEXCompatibility = new MasomodeEXCompatibility(this).TryLoad() as MasomodeEXCompatibility;
                //BossChecklistCompatibility = (BossChecklistCompatibility)new BossChecklistCompatibility(this).TryLoad();

                //if (BossChecklistCompatibility != null)
                //    BossChecklistCompatibility.Initialize();

                BossChecklistCompatibility();

                DebuffIDs = new List<int> { BuffID.Bleeding, BuffID.OnFire, BuffID.Rabies, BuffID.Confused, BuffID.Weak, BuffID.BrokenArmor, BuffID.Darkness, BuffID.Slow, BuffID.Cursed, BuffID.Poisoned, BuffID.Silenced, 39, 44, 46, 47, 67, 68, 69, 70, 80,
                            88, 94, 103, 137, 144, 145, 149, 156, 160, 163, 164, 195, 196, 197, 199 };
                DebuffIDs.Add(ModContent.BuffType<Anticoagulation>());
                DebuffIDs.Add(ModContent.BuffType<Antisocial>());
                DebuffIDs.Add(ModContent.BuffType<Atrophied>());
                DebuffIDs.Add(ModContent.BuffType<Berserked>());
                DebuffIDs.Add(ModContent.BuffType<Bloodthirsty>());
                DebuffIDs.Add(ModContent.BuffType<ClippedWings>());
                DebuffIDs.Add(ModContent.BuffType<Crippled>());
                DebuffIDs.Add(ModContent.BuffType<CurseoftheMoon>());
                DebuffIDs.Add(ModContent.BuffType<Defenseless>());
                DebuffIDs.Add(ModContent.BuffType<FlamesoftheUniverse>());
                DebuffIDs.Add(ModContent.BuffType<Flipped>());
                DebuffIDs.Add(ModContent.BuffType<FlippedHallow>());
                DebuffIDs.Add(ModContent.BuffType<Fused>());
                DebuffIDs.Add(ModContent.BuffType<GodEater>());
                DebuffIDs.Add(ModContent.BuffType<Guilty>());
                DebuffIDs.Add(ModContent.BuffType<Hexed>());
                DebuffIDs.Add(ModContent.BuffType<HolyPrice>());
                DebuffIDs.Add(ModContent.BuffType<Hypothermia>());
                DebuffIDs.Add(ModContent.BuffType<Infested>());
                DebuffIDs.Add(ModContent.BuffType<Neurotoxin>());
                DebuffIDs.Add(ModContent.BuffType<IvyVenom>());
                DebuffIDs.Add(ModContent.BuffType<Jammed>());
                DebuffIDs.Add(ModContent.BuffType<Lethargic>());
                DebuffIDs.Add(ModContent.BuffType<LightningRod>());
                DebuffIDs.Add(ModContent.BuffType<LihzahrdCurse>());
                DebuffIDs.Add(ModContent.BuffType<LivingWasteland>());
                DebuffIDs.Add(ModContent.BuffType<Lovestruck>());
                DebuffIDs.Add(ModContent.BuffType<LowGround>());
                DebuffIDs.Add(ModContent.BuffType<MarkedforDeath>());
                DebuffIDs.Add(ModContent.BuffType<Midas>());
                DebuffIDs.Add(ModContent.BuffType<MutantNibble>());
                DebuffIDs.Add(ModContent.BuffType<NanoInjection>());
                DebuffIDs.Add(ModContent.BuffType<NullificationCurse>());
                DebuffIDs.Add(ModContent.BuffType<OceanicMaul>());
                DebuffIDs.Add(ModContent.BuffType<OceanicSeal>());
                DebuffIDs.Add(ModContent.BuffType<Oiled>());
                DebuffIDs.Add(ModContent.BuffType<Purged>());
                DebuffIDs.Add(ModContent.BuffType<Purified>());
                DebuffIDs.Add(ModContent.BuffType<RushJob>());
                DebuffIDs.Add(ModContent.BuffType<ReverseManaFlow>());
                DebuffIDs.Add(ModContent.BuffType<Rotting>());
                DebuffIDs.Add(ModContent.BuffType<Shadowflame>());
                DebuffIDs.Add(ModContent.BuffType<Smite>());
                DebuffIDs.Add(ModContent.BuffType<Buffs.Masomode.SqueakyToy>());
                DebuffIDs.Add(ModContent.BuffType<Stunned>());
                DebuffIDs.Add(ModContent.BuffType<Swarming>());
                DebuffIDs.Add(ModContent.BuffType<Unstable>());

                DebuffIDs.Add(ModContent.BuffType<AbomFang>());
                DebuffIDs.Add(ModContent.BuffType<AbomPresence>());
                DebuffIDs.Add(ModContent.BuffType<MutantFang>());
                DebuffIDs.Add(ModContent.BuffType<MutantPresence>());

                DebuffIDs.Add(ModContent.BuffType<AbomRebirth>());

                DebuffIDs.Add(ModContent.BuffType<TimeFrozen>());

                //Mod bossHealthBar = ModLoader.GetMod("FKBossHealthBar");
                //if (bossHealthBar != null)
                //{
                //    //bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<BabyGuardian>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TimberChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TimberChampionHead>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<EarthChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<LifeChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<WillChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<ShadowChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<SpiritChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TerraChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<NatureChampion>());

                //    bossHealthBar.Call("hbStart");
                //    bossHealthBar.Call("hbSetColours", new Color(1f, 1f, 1f), new Color(1f, 1f, 0.5f), new Color(1f, 0f, 0f));
                //    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<CosmosChampion>());

                //    bossHealthBar.Call("hbStart");
                //    bossHealthBar.Call("hbSetColours", new Color(1f, 0f, 1f), new Color(1f, 0.2f, 0.6f), new Color(1f, 0f, 0f));
                //    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<DeviBoss>());

                //    bossHealthBar.Call("RegisterDD2HealthBar", ModContent.NPCType<AbomBoss>());

                //    bossHealthBar.Call("hbStart");
                //    bossHealthBar.Call("hbSetColours", new Color(55, 255, 191), new Color(0f, 1f, 0f), new Color(0f, 0.5f, 1f));
                //    //bossHealthBar.Call("hbSetBossHeadTexture", GetTexture("NPCs/MutantBoss/MutantBoss_Head_Boss"));
                //    bossHealthBar.Call("hbSetTexture",
                //        bossHealthBar.GetTexture("UI/MoonLordBarStart"), null,
                //        bossHealthBar.GetTexture("UI/MoonLordBarEnd"), null);
                //    bossHealthBar.Call("hbSetTextureExpert",
                //        bossHealthBar.GetTexture("UI/MoonLordBarStart_Exp"), null,
                //        bossHealthBar.GetTexture("UI/MoonLordBarEnd_Exp"), null);
                //    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<MutantBoss>());
                //}

                //mutant shop
                Mod fargos = ModLoader.GetMod("Fargowiltas");
                fargos.Call("AddSummon", 0.5f, "FargowiltasSouls", "SquirrelCoatofArms", new Func<bool>(() => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.TrojanSquirrel]), Item.buyPrice(0, 4));
                fargos.Call("AddSummon", 6.9f, "FargowiltasSouls", "DevisCurse", new Func<bool>(() => FargoSoulsWorld.downedDevi), Item.buyPrice(0, 17, 50));
                fargos.Call("AddSummon", 17.009f, "FargowiltasSouls", "ChampionySigil", new Func<bool>(() => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CosmosChampion]), Item.buyPrice(5));
                fargos.Call("AddSummon", 17.01f, "FargowiltasSouls", "AbomsCurse", new Func<bool>(() => FargoSoulsWorld.downedAbom), Item.buyPrice(10));
                //fargos.Call("AddSummon", 17.01f, "FargowiltasSouls", "TruffleWormEX", () => FargoSoulsWorld.downedFishronEX, Item.buyPrice(10));
                fargos.Call("AddSummon", 17.02f, "FargowiltasSouls", "MutantsCurse", new Func<bool>(() => FargoSoulsWorld.downedMutant), Item.buyPrice(20));
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

        //        /*public void AddPartialRecipe(ModItem modItem, ModRecipe recipe, int tileType, int replacementItem)
        //        {
        //            RecipeGroup group = new RecipeGroup(() => $"{Lang.misc[37]} {modItem.DisplayName.GetDefault()} Material");
        //            foreach (Item i in recipe.requiredItem)
        //            {
        //                if (i == null || i.type == ItemID.None)
        //                    continue;
        //                group.ValidItems.Add(i.type);
        //            }
        //            string groupName = $"FargowiltasSouls:Any{modItem.Name}Material";
        //            RecipeGroup.RegisterGroup(groupName, group);

        //            ModRecipe partialRecipe = new ModRecipe(this);
        //            int originalItemsNeeded = group.ValidItems.Count / 2;
        //            partialRecipe.AddRecipeGroup(groupName, originalItemsNeeded);
        //            partialRecipe.AddIngredient(replacementItem, group.ValidItems.Count - originalItemsNeeded);
        //            partial.AddTile(tileType);
        //            partialRecipe.SetResult(modItem);
        //            partialRecipe.AddRecipe();
        //        }*/

        //        public override void AddRecipes()
        //        {
        //            ModRecipe recipe = new ModRecipe(this);
        //            .AddIngredient(ItemID.SoulofLight, 7)
        //            .AddIngredient(ItemID.SoulofNight, 7)
        //            .AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5)
        //            .AddTile(TileID.MythrilAnvil)
        //            recipe.SetResult(ModContent.ItemType<JungleChest>());
        //            .Register();

        //            recipe = new ModRecipe(this);
        //            .AddIngredient(ItemID.WizardHat)
        //            .AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5)
        //            .AddTile(TileID.MythrilAnvil)
        //            recipe.SetResult(ModContent.ItemType<RuneOrb>());
        //            .Register();

        //            recipe = new ModRecipe(this);
        //            .AddIngredient(ItemID.LifeCrystal)
        //            .AddTile(TileID.CookingPots)
        //            recipe.SetResult(ModContent.ItemType<HeartChocolate>());
        //            .Register();

        //            /*recipe = new ModRecipe(this);
        //            recipe.AddRecipeGroup("FargowiltasSouls:AnyBonesBanner", 2);
        //            .AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5)
        //            .AddTile(TileID.Anvils)
        //            recipe.SetResult(ModContent.ItemType<InnocuousSkull>());
        //            .Register();*/
        //        }

        string AnyItem(int id) => $"{Lang.misc[37]} {Lang.GetItemName(id)}";
        string AnyItem(string fargoSoulsLocalizationKey) => $"{Lang.misc[37]} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.{fargoSoulsLocalizationKey}")}";
        string ItemXOrY(int id1, int id2) => $"{Lang.GetItemName(id1)} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.Or")} {Lang.GetItemName(id2)}";

        public override void AddRecipeGroups()
        {
            RecipeGroup group;

            //drax
            group = new RecipeGroup(() => AnyItem(ItemID.Drax), ItemID.Drax, ItemID.PickaxeAxe);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyDrax", group);

            //dungeon enemies
            group = new RecipeGroup(() => AnyItem("BonesBanner"), ItemID.AngryBonesBanner, ItemID.BlueArmoredBonesBanner, ItemID.HellArmoredBonesBanner, ItemID.RustyArmoredBonesBanner);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBonesBanner", group);

            //cobalt
            group = new RecipeGroup(() => AnyItem(ItemID.CobaltRepeater), ItemID.CobaltRepeater, ItemID.PalladiumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltRepeater", group);

            //mythril
            group = new RecipeGroup(() => AnyItem(ItemID.MythrilRepeater), ItemID.MythrilRepeater, ItemID.OrichalcumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilRepeater", group);

            //adamantite
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteRepeater), ItemID.AdamantiteRepeater, ItemID.TitaniumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantiteRepeater", group);

            //evil wood
            group = new RecipeGroup(() => ItemXOrY(ItemID.Ebonwood, ItemID.Shadewood), ItemID.Ebonwood, ItemID.Shadewood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyEvilWood", group);

            //any adamantite
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteBar), ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantite", group);

            //shroomite head
            group = new RecipeGroup(() => AnyItem(ItemID.ShroomiteHelmet), ItemID.ShroomiteHelmet, ItemID.ShroomiteMask, ItemID.ShroomiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyShroomHead", group);

            //orichalcum head
            group = new RecipeGroup(() => AnyItem(ItemID.OrichalcumHelmet), ItemID.OrichalcumHelmet, ItemID.OrichalcumMask, ItemID.OrichalcumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyOriHead", group);

            //palladium head
            group = new RecipeGroup(() => AnyItem(ItemID.PalladiumHelmet), ItemID.PalladiumHelmet, ItemID.PalladiumMask, ItemID.PalladiumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPallaHead", group);

            //cobalt head
            group = new RecipeGroup(() => AnyItem(ItemID.CobaltHelmet), ItemID.CobaltHelmet, ItemID.CobaltHat, ItemID.CobaltMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltHead", group);

            //mythril head
            group = new RecipeGroup(() => AnyItem(ItemID.MythrilHelmet), ItemID.MythrilHelmet, ItemID.MythrilHat, ItemID.MythrilHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilHead", group);

            //titanium head
            group = new RecipeGroup(() => AnyItem(ItemID.TitaniumHelmet), ItemID.TitaniumHelmet, ItemID.TitaniumMask, ItemID.TitaniumHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyTitaHead", group);

            //hallowed head
            group = new RecipeGroup(() => AnyItem(ItemID.HallowedHelmet), ItemID.HallowedHelmet, ItemID.HallowedHeadgear, ItemID.HallowedHeadgear, ItemID.HallowedHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyHallowHead", group);

            //ancient hallow
            group = new RecipeGroup(() => AnyItem(ItemID.AncientHallowedHelmet), ItemID.AncientHallowedHelmet, ItemID.AncientHallowedHeadgear, ItemID.AncientHallowedHood, ItemID.AncientHallowedMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAncientHallowHead", group);

            //adamantite head
            group = new RecipeGroup(() => AnyItem(ItemID.AdamantiteHelmet), ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamHead", group);

            //chloro head
            group = new RecipeGroup(() => AnyItem(ItemID.ChlorophyteHelmet), ItemID.ChlorophyteHelmet, ItemID.ChlorophyteMask, ItemID.ChlorophyteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyChloroHead", group);

            //spectre head
            group = new RecipeGroup(() => ItemXOrY(ItemID.SpectreHood, ItemID.SpectreMask), ItemID.SpectreHood, ItemID.SpectreMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySpectreHead", group);

            //beetle body
            group = new RecipeGroup(() => ItemXOrY(ItemID.BeetleShell, ItemID.BeetleScaleMail), ItemID.BeetleShell, ItemID.BeetleScaleMail);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBeetle", group);

            //            //phasesabers
            //            group = new RecipeGroup(() => AnyItem("Phasesaber"), ItemID.RedPhasesaber, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.WhitePhasesaber,
            //                ItemID.YellowPhasesaber);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPhasesaber", group);

            //            //vanilla butterflies
            //            group = new RecipeGroup(() => AnyItem("Butterfly"), ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly,
            //                ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyButterfly", group);

            //vanilla squirrels
            group = new RecipeGroup(() => AnyItem(ItemID.Squirrel),
                ItemID.Squirrel,
                ItemID.SquirrelRed,
                ItemID.SquirrelGold,
                ItemID.GemSquirrelAmber,
                ItemID.GemSquirrelAmethyst,
                ItemID.GemSquirrelDiamond,
                ItemID.GemSquirrelEmerald,
                ItemID.GemSquirrelRuby,
                ItemID.GemSquirrelSapphire,
                ItemID.GemSquirrelTopaz
            );
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySquirrel", group);

            //            //vanilla fish
            //            group = new RecipeGroup(() => AnyItem("CommonFish"), ItemID.AtlanticCod, ItemID.Bass, ItemID.Trout, ItemID.RedSnapper, ItemID.Salmon, ItemID.Tuna);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCommonFish", group);

            //vanilla birds
            group = new RecipeGroup(() => AnyItem(ItemID.Bird), ItemID.Bird, ItemID.BlueJay, ItemID.Cardinal, ItemID.GoldBird, ItemID.Duck, ItemID.MallardDuck, ItemID.Grebe, ItemID.Penguin, ItemID.Seagull);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBird", group);

            //            //vanilla scorpions
            //            group = new RecipeGroup(() => AnyItem(ItemID.Scorpion), ItemID.Scorpion, ItemID.BlackScorpion);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyScorpion", group);

            //            //gold pick
            //            group = new RecipeGroup(() => AnyItem(ItemID.GoldPickaxe), ItemID.GoldPickaxe, ItemID.PlatinumPickaxe);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldPickaxe", group);

            //            //fish trash
            //            group = new RecipeGroup(() => AnyItem("FishingTrash"), ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyFishingTrash", group);

            //vanilla rotten chunk/vertebrae
            group = new RecipeGroup(() => ItemXOrY(ItemID.RottenChunk, ItemID.Vertebrae), ItemID.RottenChunk, ItemID.Vertebrae);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyRottenChunk", group);
        }

        internal enum PacketID : byte
        {
            RequestGuttedCreeper,
            RequestPerfumeHeart,
            SyncCultistDamageCounterToServer,
            RequestCreeperHeal,
            RequestDeviGift,
            SyncEModeNPC,
            SpawnFishronEX,
            SyncFishronEXLife,
            SyncTogglesOnJoin,
            SyncOneToggle,
            SyncCanPlayMaso,
            SpawnBossTryFromNPC
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            byte data = reader.ReadByte();
            if (Enum.IsDefined(typeof(PacketID), data))
            {
                switch ((PacketID)data)
                {
                    case PacketID.RequestGuttedCreeper: //server side spawning creepers
                        if (Main.netMode == NetmodeID.Server)
                        {
                            byte p = reader.ReadByte();
                            int multiplier = reader.ReadByte();
                            int n = NPC.NewNPC(NPC.GetBossSpawnSource(p), (int)Main.player[p].Center.X, (int)Main.player[p].Center.Y, ModContent.NPCType<CreeperGutted>(), 0,
                                p, 0f, multiplier, 0);
                            if (n != Main.maxNPCs)
                            {
                                Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                        }
                        break;

                    case PacketID.RequestPerfumeHeart: //client to server
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int p = reader.ReadByte();
                            int n = reader.ReadByte();
                            Item.NewItem(Main.player[p].GetSource_OnHit(Main.npc[n]), Main.npc[n].Hitbox, ItemID.Heart);
                        }
                        break;

                    case PacketID.SyncCultistDamageCounterToServer: //client to server
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

                    case PacketID.RequestCreeperHeal: //refresh creeper
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            byte player = reader.ReadByte();
                            NPC creeper = Main.npc[reader.ReadByte()];
                            if (creeper.active && creeper.type == ModContent.NPCType<CreeperGutted>() && creeper.ai[0] == player)
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



                    case PacketID.RequestDeviGift: //devi gifts
                        if (Main.netMode == NetmodeID.Server)
                        {
                            Player player = Main.player[reader.ReadByte()];
                            DropDevianttsGift(player);
                        }
                        break;


                    case PacketID.SyncEModeNPC: // New maso sync
                        {
                            int npcToSync = reader.ReadInt32();
                            int npcType = reader.ReadInt32();
                            int bytesLength = reader.ReadInt32();
                            //Logger.Debug($"got {npcToSync} {npcType}, real is {Main.npc[npcToSync].active} {Main.npc[npcToSync].type}");
                            if (Main.npc[npcToSync].active && Main.npc[npcToSync].type == npcType)
                            {
                                Main.npc[npcToSync].GetGlobalNPC<NewEModeGlobalNPC>().NetRecieve(reader);
                            }
                            else if (bytesLength > 0) //in case of desync between client/server, just clear the rest of the message from the buffer
                            {
                                reader.ReadBytes(bytesLength);
                            }
                        }
                        break;

                    case PacketID.SpawnFishronEX: //server side spawning fishron EX
                        if (Main.netMode == NetmodeID.Server)
                        {
                            byte target = reader.ReadByte();
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            EModeGlobalNPC.spawnFishronEX = true;
                            NPC.NewNPC(NPC.GetBossSpawnSource(target), x, y, NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, target);
                            EModeGlobalNPC.spawnFishronEX = false;
                            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Duke Fishron EX has awoken!"), new Color(50, 100, 255));
                        }
                        break;

                    case PacketID.SyncFishronEXLife: //confirming fish EX max life
                        {
                            int f = reader.ReadInt32();
                            Main.npc[f].lifeMax = reader.ReadInt32();
                        }
                        break;

                    case PacketID.SyncTogglesOnJoin: //sync toggles on join
                        {
                            Player player = Main.player[reader.ReadByte()];
                            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                            byte count = reader.ReadByte();
                            List<string> keys = ToggleLoader.LoadedToggles.Keys.ToList();

                            for (int i = 0; i < count; i++)
                            {
                                modPlayer.Toggler.Toggles[keys[i]].ToggleBool = reader.ReadBoolean();
                            }
                        }
                        break;

                    case PacketID.SyncOneToggle: //sync single toggle
                        {
                            Player player = Main.player[reader.ReadByte()];
                            player.SetToggleValue(reader.ReadString(), reader.ReadBoolean());
                        }
                        break;

                    case PacketID.SyncCanPlayMaso: //server acknowledges a CanPlayMaso player
                        if (Main.netMode == NetmodeID.Server)
                        {
                            FargoSoulsWorld.CanPlayMaso = reader.ReadBoolean();
                        }
                        break;

                    case PacketID.SpawnBossTryFromNPC:
                        if (Main.netMode == NetmodeID.Server)
                        {
                            int p = reader.ReadInt32();
                            int originalType = reader.ReadInt32();
                            int bossType = reader.ReadInt32();
                            FargoSoulsUtil.SpawnBossTryFromNPC(p, originalType, bossType);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        //        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        //        {
        //            if (Main.musicVolume != 0 && Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
        //            {
        //                if (MMWorld.MMArmy && priority <= MusicPriority.Environment)
        //                {
        //                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MonsterMadhouse");
        //                    priority = MusicPriority.Event;
        //                }
        //                /*if (FargoSoulsGlobalNPC.FargoSoulsUtil.BossIsAlive(ref FargoSoulsGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
        //                    && Main.player[Main.myPlayer].Distance(Main.npc[FargoSoulsGlobalNPC.mutantBoss].Center) < 3000)
        //                {
        //                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/SteelRed");
        //                    priority = (MusicPriority)12;
        //                }*/
        //            }
        //        }

        public static bool NoInvasion(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.Invasion && (!Main.pumpkinMoon && !Main.snowMoon || spawnInfo.SpawnTileY > Main.worldSurface || Main.dayTime) &&
                   (!Main.eclipse || spawnInfo.SpawnTileY > Main.worldSurface || !Main.dayTime);
        }

        public static bool NoBiome(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            return !player.ZoneJungle && !player.ZoneDungeon && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHallow && !player.ZoneSnow && !player.ZoneUndergroundDesert;
        }

        public static bool NoZoneAllowWater(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.Sky && !spawnInfo.Player.ZoneMeteor && !spawnInfo.SpiderCave;
        }

        public static bool NoZone(NPCSpawnInfo spawnInfo)
        {
            return NoZoneAllowWater(spawnInfo) && !spawnInfo.Water;
        }

        public static bool NormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.PlayerInTown && NoInvasion(spawnInfo);
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
    }
}