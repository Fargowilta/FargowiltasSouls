global using FargowiltasSouls.Core.ModPlayers;
global using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Content.Sky;
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
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using FargowiltasSouls.Content.Items.Dyes;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Tiles;
using FargowiltasSouls.Content.UI;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Patreon.Volknet;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

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

        internal static Dictionary<int, int> ModProjDict = new();

        internal struct TextureBuffer
        {
            public static readonly Dictionary<int, Asset<Texture2D>> NPC = new();
            public static readonly Dictionary<int, Asset<Texture2D>> NPCHeadBoss = new();
            public static readonly Dictionary<int, Asset<Texture2D>> Gore = new();
            public static readonly Dictionary<int, Asset<Texture2D>> Golem = new();
            public static readonly Dictionary<int, Asset<Texture2D>> Extra = new();
            public static Asset<Texture2D> Ninja = null;
            public static Asset<Texture2D> BoneArm = null;
            public static Asset<Texture2D> BoneArm2 = null;
            public static Asset<Texture2D> Chain12 = null;
            public static Asset<Texture2D> Chain26 = null;
            public static Asset<Texture2D> Chain27 = null;
            public static Asset<Texture2D> Wof = null;
        }

        public override void Load()
        {
            Instance = this;

            SkyManager.Instance["FargowiltasSouls:AbomBoss"] = new AbomSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss2"] = new MutantSky2();

            SkyManager.Instance["FargowiltasSouls:MoonLordSky"] = new MoonLordSky();

            FreezeKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "冻结" : "Freeze", "P");
            GoldKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "金身" : "Turn Gold", "O");
            SmokeBombKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "投掷烟雾弹" : "Throw Smoke Bomb", "I");
            SpecialDashKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "特殊冲刺" : "Special Dash", "C");
            BombKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "突变炸弹" : "Bomb", "Z");
            SoulToggleKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "打开魂石效果设置" : "Open Soul Toggler", ".");
            PrecisionSealKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "玲珑圣印精确模式" : "Precision Movement", "LeftShift");
            MagicalBulbKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "魔法净化" : "Magical Cleanse", "N");
            FrigidSpellKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "寒霜咒语" : "Frigid Spell", "U");
            DebuffInstallKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "减益负载" : "Debuff Install", "Y");
            AmmoCycleKey = KeybindLoader.RegisterKeybind(this, FargoSoulsUtil.IsChinese() ? "弹药切换" : "Ammo Cycle", "L");


            ToggleLoader.Load();

            FargoUIManager.LoadUI();

            AddLocalizations();

            if (Main.netMode != NetmodeID.Server)
            {
                #region shaders

                //loading refs for shaders
                Ref<Effect> lcRef = new(Assets.Request<Effect>("Assets/Effects/Armor/LifeChampionShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> wcRef = new(Assets.Request<Effect>("Assets/Effects/Armor/WillChampionShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> gaiaRef = new(Assets.Request<Effect>("Assets/Effects/Armor/GaiaShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> textRef = new(Assets.Request<Effect>("Assets/Effects/TextShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> invertRef = new(Assets.Request<Effect>("Assets/Effects/Invert", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> finalSparkRef = new(Assets.Request<Effect>("Assets/Effects/FinalSpark", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> mutantDeathrayRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/MutantFinalDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> willDeathrayRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/WillDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> willBigDeathrayRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/WillBigDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> deviBigDeathrayRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/DeviTouhouDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> deviRingRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/DeviRingShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> genericDeathrayRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/GenericDeathrayShader", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> blobTrailRef = new(Assets.Request<Effect>("Assets/Effects/PrimitiveShaders/BlobTrailShader", AssetRequestMode.ImmediateLoad).Value);

                //loading shaders from refs
                GameShaders.Misc["LCWingShader"] = new MiscShaderData(lcRef, "LCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<LifeDye>(), new ArmorShaderData(lcRef, "LCArmor").UseColor(new Color(1f, 0.647f, 0.839f)).UseSecondaryColor(Color.Goldenrod));

                GameShaders.Misc["WCWingShader"] = new MiscShaderData(wcRef, "WCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<WillDye>(), new ArmorShaderData(wcRef, "WCArmor").UseColor(Color.DarkOrchid).UseSecondaryColor(Color.LightPink).UseImage("Images/Misc/noise"));

                GameShaders.Misc["GaiaShader"] = new MiscShaderData(gaiaRef, "GaiaGlow");
                GameShaders.Armor.BindShader(ModContent.ItemType<GaiaDye>(), new ArmorShaderData(gaiaRef, "GaiaArmor").UseColor(new Color(0.44f, 1, 0.09f)).UseSecondaryColor(new Color(0.5f, 1f, 0.9f)));

                //GameShaders.Misc["PulseUpwards"] = new MiscShaderData(textRef, "PulseUpwards");
                //GameShaders.Misc["PulseDiagonal"] = new MiscShaderData(textRef, "PulseDiagonal");
                //GameShaders.Misc["PulseCircle"] = new MiscShaderData(textRef, "PulseCircle");
                //GameShaders.Misc["FargowiltasSouls:MutantDeathray"] = new MiscShaderData(mutantDeathrayRef, "TrailPass");
                //GameShaders.Misc["FargowiltasSouls:WillDeathray"] = new MiscShaderData(willDeathrayRef, "TrailPass");
                //GameShaders.Misc["FargowiltasSouls:WillBigDeathray"] = new MiscShaderData(willBigDeathrayRef, "TrailPass");
                //GameShaders.Misc["FargowiltasSouls:DeviBigDeathray"] = new MiscShaderData(deviBigDeathrayRef, "TrailPass");
                //GameShaders.Misc["FargowiltasSouls:DeviRing"] = new MiscShaderData(deviRingRef, "TrailPass");
                //GameShaders.Misc["FargowiltasSouls:GenericDeathray"] = new MiscShaderData(genericDeathrayRef, "TrailPass");
                //GameShaders.Misc["FargowiltasSouls:BlobTrail"] = new MiscShaderData(blobTrailRef, "TrailPass");

                //Filters.Scene["FargowiltasSouls:FinalSpark"] = new Filter(new FinalSparkShader(finalSparkRef, "FinalSpark"), EffectPriority.High);
                //Filters.Scene["FargowiltasSouls:Invert"] = new Filter(new TimeStopShader(invertRef, "Main"), EffectPriority.VeryHigh);

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

            
            On_Player.CheckSpawn_Internal += LifeRevitalizer_CheckSpawn_Internal;
            On_Player.AddBuff += AddBuff;
        }
        private static bool LifeRevitalizer_CheckSpawn_Internal(
            On_Player.orig_CheckSpawn_Internal orig,
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
        
        private void AddBuff(
            Terraria.On_Player.orig_AddBuff orig,
            Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            FargoSoulsPlayer modPlayer = self.FargoSouls();
            if (Main.debuff[type]
                && timeToAdd > 3 //dont affect auras
                && !Main.buffNoTimeDisplay[type] //dont affect hidden time debuffs
                && !BuffID.Sets.NurseCannotRemoveDebuff[type] //only affect debuffs that nurse can cleanse
                && (modPlayer.ParryDebuffImmuneTime > 0
                    || modPlayer.BetsyDashing 
                    || modPlayer.GoldShell 
                    || modPlayer.ShellHide 
                    || modPlayer.MonkDashing > 0 
                    || self.GetEffectFields<AncientCobaltFields>().CobaltImmuneTimer > 0
                    || self.GetEffectFields<TitaniumFields>().TitaniumDRBuff)
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
        //    return WorldSavingSystem.EternityMode | orig(self, info);
        //}

        //private static bool IsMasterModeOrEMode_CanShowItemDropInUI(
        //    On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanShowItemDropInUI orig,
        //    Conditions.IsMasterMode self)
        //{
        //    // Use | instead of || so orig runs no matter what.
        //    return WorldSavingSystem.EternityMode | orig(self);
        //}

        //private static bool DropBasedOnMasterOrEMode_CanDrop(
        //    On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_CanDrop orig,
        //    DropBasedOnMasterMode self, DropAttemptInfo info)
        //{
        //    // Use | instead of || so orig runs no matter what.
        //    return (WorldSavingSystem.EternityMode && self.ruleForMasterMode.CanDrop(info)) | orig(self, info);
        //}

        //private static ItemDropAttemptResult DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction(
        //    On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction orig,
        //    DropBasedOnMasterMode self, DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
        //{
        //    ItemDropAttemptResult itemDropAttemptResult = orig(self, info, resolveAction);
        //    return WorldSavingSystem.EternityMode ? resolveAction(self.ruleForMasterMode, info) : itemDropAttemptResult;
        //}

        public override void Unload()
        {
            //On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanDrop -= IsMasterModeOrEMode_CanDrop;
            //On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanShowItemDropInUI -= IsMasterModeOrEMode_CanShowItemDropInUI;
            //On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.CanDrop -= DropBasedOnMasterOrEMode_CanDrop;
            //On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction -= DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction;

            //NPC.LunarShieldPowerMax = NPC.downedMoonlord ? 50 : 100;

            static void RestoreSprites(Dictionary<int, Asset<Texture2D>> buffer, Asset<Texture2D>[] original)
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

            DebuffIDs?.Clear();

            ModProjDict.Clear();

            Instance = null;

            On_Player.CheckSpawn_Internal -= LifeRevitalizer_CheckSpawn_Internal;
            On_Player.AddBuff -= AddBuff;
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
                        return WorldSavingSystem.EternityMode;

                    case "EternityVanillaBossBehaviour":
                        bool BehaviourWasOn = WorldSavingSystem.EternityVanillaBehaviour;
                        bool? arg = args[1] as bool?;
                        if (arg != null)
                        {
                            WorldSavingSystem.EternityVanillaBehaviour = (bool)arg;
                        }
                        return BehaviourWasOn;

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
                        return WorldSavingSystem.MasochistModeReal;

                    case "DownedMutant":
                        return WorldSavingSystem.DownedMutant;

                    case "DownedAbom":
                    case "DownedAbominationn":
                        return WorldSavingSystem.DownedAbom;

                    case "DownedChamp":
                    case "DownedChampion": //arg is internal string name of champ
                        return WorldSavingSystem.DownedBoss[(int)Enum.Parse<WorldSavingSystem.Downed>(args[1] as string, true)];

                    case "DownedEri":
                    case "DownedEridanus":
                    case "DownedCosmos":
                    case "DownedCosmosChamp":
                    case "DownedCosmosChampion":
                        return WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion];

                    case "DownedDevi":
                    case "DownedDeviantt":
                        return WorldSavingSystem.DownedDevi;

                    case "DownedFishronEX":
                        return WorldSavingSystem.DownedFishronEX;

                    case "PureHeart":
                        return Main.LocalPlayer.FargoSouls().PureHeart;

                    case "MutantAntibodies":
                        return Main.LocalPlayer.FargoSouls().MutantAntibodies;

                    case "SinisterIcon":
                        return Main.LocalPlayer.FargoSouls().SinisterIcon;

                    case "AbomAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>());

                    case "MutantAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>());

                    case "DeviAlive":
                    case "DeviBossAlive":
                    case "DevianttAlive":
                    case "DevianttBossAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>());

                    case "MutantPact":
                    case "MutantsPact":
                    case "MutantCreditCard":
                    case "MutantsCreditCard":
                        return Main.LocalPlayer.FargoSouls().MutantsCreditCard;

                    case "MutantDiscountCard":
                    case "MutantsDiscountCard":
                        return Main.LocalPlayer.FargoSouls().MutantsDiscountCard;

                    case "NekomiArmor":
                    case "NekomiArmour":
                        return Main.LocalPlayer.FargoSouls().NekomiSet;

                    case "EridanusArmor":
                    case "EridanusArmour":
                        return Main.LocalPlayer.FargoSouls().EridanusSet;

                    case "StyxArmor":
                    case "StyxArmour":
                        return Main.LocalPlayer.FargoSouls().StyxSet;

                    case "MutantArmor":
                    case "MutantArmour":
                        return Main.LocalPlayer.FargoSouls().MutantSetBonusItem != null;

                    case "GiftsReceived":
                        return Main.LocalPlayer.FargoSouls().ReceivedMasoGift;

                    case "GiveDevianttGifts":
                        Main.LocalPlayer.FargoSouls().ReceivedMasoGift = true;
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
                        Main.npcChatText = FargoSoulsUtil.IsChinese() ? "这个世界看起来比平时更艰难，所以我免费给你提供这些，仅此一次！如果你需要任何提示，请告诉我，好吗？" : "This world looks tougher than usual, so you can have these on the house just this once! Talk to me if you need any tips, yeah?";
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
            //Item.NewItem(null, player.Center, ItemID.LifeCrystal, 4);
            //Item.NewItem(null, player.Center, ItemID.ManaCrystal, 2);
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

            GiveItem("Fargowiltas", "AutoHouse", 2);
            GiveItem("Fargowiltas", "MiniInstaBridge", 2);
            //GiveItem("Fargowiltas", "HalfInstavator");

            Item.NewItem(null, player.Center, ModContent.ItemType<EurusSock>());
            Item.NewItem(null, player.Center, ModContent.ItemType<PuffInABottle>());
            //int bugnet = (Main.zenithWorld || Main.remixWorld) ? ItemID.FireproofBugNet : ItemID.BugNet;
            Item.NewItem(null, player.Center, ItemID.BugNet);
            Item.NewItem(null, player.Center, ItemID.Squirrel);
            //Item.NewItem(null, player.Center, ItemID.GrapplingHook);

            if (Main.zenithWorld || Main.remixWorld)
            {
                Item.NewItem(null, player.Center, ItemID.ObsidianSkinPotion, 5);
            }

            //only give once per world
            if (!WorldSavingSystem.ReceivedTerraStorage)
            {
                if (ModLoader.TryGetMod("MagicStorage", out Mod _))
                {
                    GiveItem("MagicStorage", "StorageHeart");
                    GiveItem("MagicStorage", "CraftingAccess");
                    GiveItem("MagicStorage", "StorageUnit", 4);

                    WorldSavingSystem.ReceivedTerraStorage = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData); //sync world in mp
                }
                else if (ModLoader.TryGetMod("MagicStorageExtra", out Mod _))
                {
                    GiveItem("MagicStorageExtra", "StorageHeart");
                    GiveItem("MagicStorageExtra", "CraftingAccess");
                    GiveItem("MagicStorageExtra", "StorageUnit", 4);

                    WorldSavingSystem.ReceivedTerraStorage = true;
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

                if (ModLoader.TryGetMod("Wikithis", out Mod wikithis) && !Main.dedServ)
                {
                    wikithis.Call("AddModURL", this, "https://terrariamods.wiki.gg/wiki/Fargo%27s_Mod/{}");

                    // You can also use call ID for some calls!
                    //wikithis.Call(0, this, "https://examplemod.wiki.gg/wiki/{}");

                    // Alternatively, you can use this instead, if your wiki is on terrariamods.fandom.com
                    //wikithis.Call(0, this, "https://terrariamods.fandom.com/wiki/Example_Mod/{}");
                    //wikithis.Call("AddModURL", this, "https://terrariamods.fandom.com/wiki/Example_Mod/{}");

                    // If there wiki on other languages (such as russian, spanish, chinese, etch), then you can also call that:
                    //wikithis.Call(0, this, "https://examplemod.wiki.gg/zh/wiki/{}", GameCulture.CultureName.Chinese)

                    // If you want to replace default icon for your mod, then call this. Icon should be 30x30, either way it will be cut.
                    //wikithis.Call("AddWikiTexture", this, ModContent.Request<Texture2D>(pathToIcon));
                    //wikithis.Call(3, this, ModContent.Request<Texture2D>(pathToIcon));
                }

                DebuffIDs = new List<int>
                {
                    BuffID.Bleeding,
                    BuffID.OnFire,
                    BuffID.Rabies,
                    BuffID.Confused,
                    BuffID.Weak,
                    BuffID.BrokenArmor,
                    BuffID.Darkness,
                    BuffID.Slow,
                    BuffID.Cursed,
                    BuffID.Poisoned,
                    BuffID.Silenced,
                    39,
                    44,
                    46,
                    47,
                    67,
                    68,
                    69,
                    70,
                    80,
                    88,
                    //BuffID.ManaSickness, this is mana sickness, why was it here?
                    103,
                    137,
                    144,
                    145,
                    149,
                    156,
                    160,
                    163,
                    164,
                    195,
                    196,
                    197,
                    199,
                    ModContent.BuffType<AnticoagulationBuff>(),
                    ModContent.BuffType<AntisocialBuff>(),
                    ModContent.BuffType<AtrophiedBuff>(),
                    ModContent.BuffType<BerserkedBuff>(),
                    ModContent.BuffType<BloodthirstyBuff>(),
                    ModContent.BuffType<ClippedWingsBuff>(),
                    ModContent.BuffType<CrippledBuff>(),
                    ModContent.BuffType<CurseoftheMoonBuff>(),
                    ModContent.BuffType<DefenselessBuff>(),
                    ModContent.BuffType<FlamesoftheUniverseBuff>(),
                    ModContent.BuffType<FlippedBuff>(),
                    ModContent.BuffType<FlippedHallowBuff>(),
                    ModContent.BuffType<FusedBuff>(),
                    ModContent.BuffType<GodEaterBuff>(),
                    ModContent.BuffType<GuiltyBuff>(),
                    ModContent.BuffType<HexedBuff>(),
                    ModContent.BuffType<HolyPriceBuff>(),
                    ModContent.BuffType<HypothermiaBuff>(),
                    ModContent.BuffType<InfestedBuff>(),
                    ModContent.BuffType<NeurotoxinBuff>(),
                    ModContent.BuffType<IvyVenomBuff>(),
                    ModContent.BuffType<JammedBuff>(),
                    ModContent.BuffType<LethargicBuff>(),
                    ModContent.BuffType<LightningRodBuff>(),
                    ModContent.BuffType<LihzahrdCurseBuff>(),
                    ModContent.BuffType<LivingWastelandBuff>(),
                    ModContent.BuffType<LovestruckBuff>(),
                    ModContent.BuffType<LowGroundBuff>(),
                    ModContent.BuffType<MarkedforDeathBuff>(),
                    ModContent.BuffType<MidasBuff>(),
                    ModContent.BuffType<MutantNibbleBuff>(),
                    ModContent.BuffType<NanoInjectionBuff>(),
                    ModContent.BuffType<NullificationCurseBuff>(),
                    ModContent.BuffType<OceanicMaulBuff>(),
                    ModContent.BuffType<OceanicSealBuff>(),
                    ModContent.BuffType<OiledBuff>(),
                    ModContent.BuffType<PurgedBuff>(),
                    ModContent.BuffType<PurifiedBuff>(),
                    ModContent.BuffType<RushJobBuff>(),
                    ModContent.BuffType<ReverseManaFlowBuff>(),
                    ModContent.BuffType<RottingBuff>(),
                    ModContent.BuffType<ShadowflameBuff>(),
                    ModContent.BuffType<SmiteBuff>(),
                    ModContent.BuffType<SqueakyToyBuff>(),
                    ModContent.BuffType<StunnedBuff>(),
                    ModContent.BuffType<SwarmingBuff>(),
                    ModContent.BuffType<UnstableBuff>(),

                    ModContent.BuffType<AbomFangBuff>(),
                    ModContent.BuffType<AbomPresenceBuff>(),
                    ModContent.BuffType<MutantFangBuff>(),
                    ModContent.BuffType<MutantPresenceBuff>(),

                    ModContent.BuffType<AbomRebirthBuff>(),

                    ModContent.BuffType<TimeFrozenBuff>()
                };

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
                //    //bossHealthBar.Call("hbSetBossHeadTexture", GetTexture("Content/NPCs/MutantBoss/MutantBoss_Head_Boss"));
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
                fargos.Call("AddSummon", 0.5f, "FargowiltasSouls", "SquirrelCoatofArms", new Func<bool>(() => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.TrojanSquirrel]), Item.buyPrice(0, 4));
                fargos.Call("AddSummon", 6.9f, "FargowiltasSouls", "DevisCurse", new Func<bool>(() => WorldSavingSystem.DownedDevi), Item.buyPrice(0, 17, 50));
                fargos.Call("AddSummon", 8.7f, "FargowiltasSouls", "MechLure", new Func<bool>(() => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.BanishedBaron]), Item.buyPrice(0, 22));
                fargos.Call("AddSummon", 11.49f, "FargowiltasSouls", "FragilePixieLamp", new Func<bool>(() => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.Lifelight]), Item.buyPrice(0, 45));
                fargos.Call("AddSummon", 18.009f, "FargowiltasSouls", "ChampionySigil", new Func<bool>(() => WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CosmosChampion]), Item.buyPrice(5));
                fargos.Call("AddSummon", 18.01f, "FargowiltasSouls", "AbomsCurse", new Func<bool>(() => WorldSavingSystem.DownedAbom), Item.buyPrice(10));
                //fargos.Call("AddSummon", 18.01f, "FargowiltasSouls", "TruffleWormEX", () => WorldSavingSystem.downedFishronEX, Item.buyPrice(10));
                fargos.Call("AddSummon", 18.02f, "FargowiltasSouls", "MutantsCurse", new Func<bool>(() => WorldSavingSystem.DownedMutant), Item.buyPrice(20));


            }
            catch (Exception e)
            {
                Logger.Warn("FargowiltasSouls PostSetupContent Error: " + e.StackTrace + e.Message);
            }
        }

        public static void ManageMusicTimestop(bool playMusicAgain)
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
            Color mutantColor = new(28, 222, 152);
            Color abomColor = new(255, 224, 53);
            Color deviColor = new(255, 51, 153);

            ColorTimer += 0.5f;

            if (ColorTimer >= 300)
                ColorTimer = 0;

            if (ColorTimer < 100)
                return Color.Lerp(mutantColor, abomColor, ColorTimer / 100);
            else if (ColorTimer < 200)
                return Color.Lerp(abomColor, deviColor, (ColorTimer - 100) / 100);
            else
                return Color.Lerp(deviColor, mutantColor, (ColorTimer - 200) / 100);
        }
        
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
                            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(FargoSoulsUtil.IsChinese() ? "猪龙鱼公爵EX已苏醒！" : "Duke Fishron EX has awoken!"), new Color(50, 100, 255));
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
                            FargoSoulsPlayer modPlayer = player.FargoSouls();
                            byte count = reader.ReadByte();
                            List<AccessoryEffect> keys = ToggleLoader.LoadedToggles.Keys.ToList();

                            for (int i = 0; i < count; i++)
                            {
                                modPlayer.Toggler.Toggles[keys[i]].ToggleBool = reader.ReadBoolean();
                            }
                        }
                        break;

                    case PacketID.SyncOneToggle: //sync single toggle
                        {
                            Player player = Main.player[reader.ReadByte()];
                            player.SetToggleValue(AccessoryEffectLoader.EffectType(reader.ReadString()), reader.ReadBoolean());
                        }
                        break;

                    case PacketID.SyncCanPlayMaso: //server acknowledges a CanPlayMaso player
                        if (Main.netMode == NetmodeID.Server)
                        {
                            WorldSavingSystem.CanPlayMaso = reader.ReadBoolean();
                        }
                        break;

                    case PacketID.SyncNanoCoreMode:
                        {
                            Player player = Main.player[reader.ReadByte()];
                            player.GetModPlayer<NanoPlayer>().NanoCoreMode = reader.Read7BitEncodedInt();
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

        public static bool NoZoneAllowWater(NPCSpawnInfo spawnInfo) => !spawnInfo.Sky && !spawnInfo.Player.ZoneMeteor && !spawnInfo.SpiderCave;

        public static bool NoZone(NPCSpawnInfo spawnInfo) => NoZoneAllowWater(spawnInfo) && !spawnInfo.Water;

        public static bool NormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.PlayerInTown && NoInvasion(spawnInfo);
        }

        public static bool NoZoneNormalSpawn(NPCSpawnInfo spawnInfo) => NormalSpawn(spawnInfo) && NoZone(spawnInfo);

        public static bool NoZoneNormalSpawnAllowWater(NPCSpawnInfo spawnInfo) => NormalSpawn(spawnInfo) && NoZoneAllowWater(spawnInfo);

        public static bool NoBiomeNormalSpawn(NPCSpawnInfo spawnInfo) => NormalSpawn(spawnInfo) && NoBiome(spawnInfo) && NoZone(spawnInfo);

    }
}
