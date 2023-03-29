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
using FargowiltasSouls.Tiles;
using FargowiltasSouls.Toggler;
using FargowiltasSouls.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal static ModKeybind SpecialDashKey;
        internal static ModKeybind BombKey;
        internal static ModKeybind SoulToggleKey;
        internal static ModKeybind PrecisionSealKey;
        internal static ModKeybind MagicalBulbKey;
        internal static ModKeybind FrigidSpellKey;
        internal static ModKeybind DebuffInstallKey;
        internal static ModKeybind AmmoCycleKey;

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

            SkyManager.Instance["FargowiltasSouls:AbomBoss"] = new AbomSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss2"] = new MutantSky2();

            SkyManager.Instance["FargowiltasSouls:MoonLordSky"] = new MoonLordSky();

            string LocalizedKeyBind(string key) => FargoSoulsUtil.GetModTranslation($"Keybind.{key}");
            FreezeKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("Freeze"), "P");
            GoldKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("Gold"), "O");
            SmokeBombKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("SmokeBomb"), "I");
            SpecialDashKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("SpecialDash"), "C");
            BombKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("Bomb"), "Z");
            SoulToggleKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("SoulToggle"), ".");
            PrecisionSealKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("PrecisionSeal"), "LeftShift");
            MagicalBulbKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("MagicalBulb"), "N");
            FrigidSpellKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("FrigidSpell"), "U");
            DebuffInstallKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("DebuffInstall"), "Y");
            AmmoCycleKey = KeybindLoader.RegisterKeybind(this, LocalizedKeyBind("AmmoCycle"), "L");

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
                Ref<Effect> mutantDeathrayRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/MutantFinalDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> willDeathrayRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/WillDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> willBigDeathrayRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/WillBigDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> deviBigDeathrayRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/DeviTouhouDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> deviRingRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/DeviRingShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> genericDeathrayRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/GenericDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> blobTrailRef = new(Assets.Request<Effect>("Effects/PrimitiveShaders/BlobTrailShader", AssetRequestMode.ImmediateLoad).Value);
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
                GameShaders.Misc["FargowiltasSouls:MutantDeathray"] = new MiscShaderData(mutantDeathrayRef, "TrailPass");
                GameShaders.Misc["FargowiltasSouls:WillDeathray"] = new MiscShaderData(willDeathrayRef, "TrailPass");
                GameShaders.Misc["FargowiltasSouls:WillBigDeathray"] = new MiscShaderData(willBigDeathrayRef, "TrailPass");
                GameShaders.Misc["FargowiltasSouls:DeviBigDeathray"] = new MiscShaderData(deviBigDeathrayRef, "TrailPass");
                GameShaders.Misc["FargowiltasSouls:DeviRing"] = new MiscShaderData(deviRingRef, "TrailPass");
                GameShaders.Misc["FargowiltasSouls:GenericDeathray"] = new MiscShaderData(genericDeathrayRef, "TrailPass");
                GameShaders.Misc["FargowiltasSouls:BlobTrail"] = new MiscShaderData(blobTrailRef, "TrailPass");

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

            On.Terraria.Player.CheckSpawn_Internal += LifeRevitalizer_CheckSpawn_Internal;
            On.Terraria.NPC.checkArmorPenetration += CheckArmorPenetration;
            On.Terraria.Player.AddBuff += AddBuff;
        }

        private static bool LifeRevitalizer_CheckSpawn_Internal(
            On.Terraria.Player.orig_CheckSpawn_Internal orig,
            int x, int y)
        {
            if (orig(x, y))
                return true;

            //Main.NewText($"{x} {y}");

            int revitalizerType = ModContent.TileType<LifeRevitalizerPlaced>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -3; j <= -1; j++)
                {
                    int newX = x + i;
                    int newY = y + j;
                    
                    if (!WorldGen.InWorld(newX, newY))
                        return false;

                    Tile tile = Framing.GetTileSafely(newX, newY);
                    if (tile.TileType != revitalizerType)
                        return false;
                }
            }

            return true;
        }

        private int CheckArmorPenetration(
            On.Terraria.NPC.orig_checkArmorPenetration orig,
            NPC self,
            int armorPenetration)
        {
            FargoSoulsGlobalNPC globalNPC = self.GetGlobalNPC<FargoSoulsGlobalNPC>();
            if (globalNPC.OceanicMaul)
                armorPenetration += 20;
            if (globalNPC.CurseoftheMoon)
                armorPenetration += 10;
            if (globalNPC.Rotting)
                armorPenetration += 10;
            return orig(self, armorPenetration);
        }

        private void AddBuff(
            On.Terraria.Player.orig_AddBuff orig,
            Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            FargoSoulsPlayer modPlayer = self.GetModPlayer<FargoSoulsPlayer>();
            if (Main.debuff[type]
                && timeToAdd > 2 //dont affect auras
                && !Main.buffNoTimeDisplay[type] //dont affect hidden time debuffs
                && (modPlayer.ParryDebuffImmuneTime > 0
                    || modPlayer.BetsyDashing 
                    || modPlayer.GoldShell 
                    || modPlayer.ShellHide 
                    || modPlayer.MonkDashing > 0 
                    || modPlayer.CobaltImmuneTimer > 0)
                && DebuffIDs.Contains(type))
            {
                return; //doing it this way so that debuffs previously had are retained, but existing debuffs also cannot be extended by reapplying
            }

            orig(self, type, timeToAdd, quiet, foodHack);
        }

        //private static bool IsMasterModeOrEMode_CanDrop(
        //    On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanDrop orig,
        //    Conditions.IsMasterMode self, DropAttemptInfo info)
        //{
        //    // Use | instead of || so orig runs no matter what.
        //    return FargoSoulsWorld.EternityMode | orig(self, info);
        //}

        //private static bool IsMasterModeOrEMode_CanShowItemDropInUI(
        //    On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanShowItemDropInUI orig,
        //    Conditions.IsMasterMode self)
        //{
        //    // Use | instead of || so orig runs no matter what.
        //    return FargoSoulsWorld.EternityMode | orig(self);
        //}

        //private static bool DropBasedOnMasterOrEMode_CanDrop(
        //    On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_CanDrop orig,
        //    DropBasedOnMasterMode self, DropAttemptInfo info)
        //{
        //    // Use | instead of || so orig runs no matter what.
        //    return (FargoSoulsWorld.EternityMode && self.ruleForMasterMode.CanDrop(info)) | orig(self, info);
        //}

        //private static ItemDropAttemptResult DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction(
        //    On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction orig,
        //    DropBasedOnMasterMode self, DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
        //{
        //    ItemDropAttemptResult itemDropAttemptResult = orig(self, info, resolveAction);
        //    return FargoSoulsWorld.EternityMode ? resolveAction(self.ruleForMasterMode, info) : itemDropAttemptResult;
        //}

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

            SoulToggler.RemoveItemTags = null;
            ToggleBackend.ConfigPath = null;

            FreezeKey = null;
            GoldKey = null;
            SmokeBombKey = null;
            SpecialDashKey = null;
            BombKey = null;
            SoulToggleKey = null;
            PrecisionSealKey = null;
            MagicalBulbKey = null;
            FrigidSpellKey = null;
            DebuffInstallKey = null;
            AmmoCycleKey = null;

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

                    case "NekomiArmor":
                    case "NekomiArmour":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().NekomiSet;

                    case "EridanusArmor":
                    case "EridanusArmour":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().EridanusSet;

                    case "StyxArmor":
                    case "StyxArmour":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().StyxSet;

                    case "MutantArmor":
                    case "MutantArmour":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantSetBonusItem != null;

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
                        //Main.npcChatText = FargoSoulsUtil.IsChinese() ? "这个世界看起来比平时更艰难，所以我免费给你提供这些，仅此一次！如果你需要任何提示，请告诉我，好吗？" : "This world looks tougher than usual, so you can have these on the house just this once! Talk to me if you need any tips, yeah?";
                        Main.npcChatText = Language.GetTextValue("Mods.FargowiltasSouls.Dialogues.GiveDevianttGifts");
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
                        return (int)Main.LocalPlayer.ActualClassCrit(DamageClass.Summon);
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

            Item.NewItem(null, player.Center, ItemID.WaterCandle);

            Item.NewItem(null, player.Center, ItemID.Torch, 200);
            Item.NewItem(null, player.Center, ItemID.LifeCrystal, 4);
            Item.NewItem(null, player.Center, ItemID.ManaCrystal, 4);
            Item.NewItem(null, player.Center, ItemID.LesserHealingPotion, 15);
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

                BossChecklistCompatibility();

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
                fargos.Call("AddSummon", 11.49f, "FargowiltasSouls", "FragilePixieLamp", new Func<bool>(() => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.LifeChallenger]), Item.buyPrice(0, 45));
                fargos.Call("AddSummon", 18.009f, "FargowiltasSouls", "ChampionySigil", new Func<bool>(() => FargoSoulsWorld.downedBoss[(int)FargoSoulsWorld.Downed.CosmosChampion]), Item.buyPrice(5));
                fargos.Call("AddSummon", 18.01f, "FargowiltasSouls", "AbomsCurse", new Func<bool>(() => FargoSoulsWorld.downedAbom), Item.buyPrice(10));
                //fargos.Call("AddSummon", 18.01f, "FargowiltasSouls", "TruffleWormEX", () => FargoSoulsWorld.downedFishronEX, Item.buyPrice(10));
                fargos.Call("AddSummon", 18.02f, "FargowiltasSouls", "MutantsCurse", new Func<bool>(() => FargoSoulsWorld.downedMutant), Item.buyPrice(20));
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

        internal enum PacketID : byte
        {
            RequestGuttedCreeper,
            RequestPerfumeHeart,
            SyncCultistDamageCounterToServer,
            RequestCreeperHeal,
            RequestDeviGift,
            //SyncEModeNPC,
            SpawnFishronEX,
            SyncFishronEXLife,
            SyncTogglesOnJoin,
            SyncOneToggle,
            SyncCanPlayMaso,
            SyncNanoCoreMode
            //SpawnBossTryFromNPC
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

                            LunaticCultist cultist = Main.npc[cult].GetGlobalNPC<LunaticCultist>();
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


                    //case PacketID.SyncEModeNPC: // New maso sync
                    //    {
                    //        int npcToSync = reader.ReadInt32();
                    //        int npcType = reader.ReadInt32();
                    //        int bytesLength = reader.ReadInt32();
                    //        //Logger.Debug($"got {npcToSync} {npcType}, real is {Main.npc[npcToSync].active} {Main.npc[npcToSync].type}");
                    //        if (Main.npc[npcToSync].active && Main.npc[npcToSync].type == npcType)
                    //        {
                    //            Main.npc[npcToSync].GetGlobalNPC<NewEModeGlobalNPC>().NetRecieve(reader);
                    //        }
                    //        else if (bytesLength > 0) //in case of desync between client/server, just clear the rest of the message from the buffer
                    //        {
                    //            reader.ReadBytes(bytesLength);
                    //        }
                    //    }
                    //    break;

                    case PacketID.SpawnFishronEX: //server side spawning fishron EX
                        if (Main.netMode == NetmodeID.Server)
                        {
                            byte target = reader.ReadByte();
                            int x = reader.ReadInt32();
                            int y = reader.ReadInt32();
                            EModeGlobalNPC.spawnFishronEX = true;
                            NPC.NewNPC(NPC.GetBossSpawnSource(target), x, y, NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, target);
                            EModeGlobalNPC.spawnFishronEX = false;
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.FargowiltasSouls.Message.SpawnFishronEX"), new Color(50, 100, 255));
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

                    case PacketID.SyncNanoCoreMode:
                        {
                            Player player = Main.player[reader.ReadByte()];
                            player.GetModPlayer<Patreon.Volknet.NanoPlayer>().NanoCoreMode = reader.Read7BitEncodedInt();
                        }
                        break;

                    //case PacketID.SpawnBossTryFromNPC:
                    //    if (Main.netMode == NetmodeID.Server)
                    //    {
                    //        int p = reader.ReadInt32();
                    //        int originalType = reader.ReadInt32();
                    //        int bossType = reader.ReadInt32();
                    //        FargoSoulsUtil.SpawnBossTryFromNPC(p, originalType, bossType);
                    //    }
                    //    break;

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
