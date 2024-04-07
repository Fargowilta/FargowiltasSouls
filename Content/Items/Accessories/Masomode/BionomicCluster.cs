using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
	public class BionomicCluster : SoulsItem
	{
		public override bool Eternity => true;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bionomic Cluster");
			/* Tooltip.SetDefault("Grants immunity to Frostburn, Shadowflame, and Squeaky Toy" +
                "\nGrants immunity to Flames of the Universe, Clipped Wings, Crippled, Webbed, and Purified" +
                "\nGrants immunity to Lovestruck, Stinky, Midas, and Hexed" +
                "\nUse to teleport to your last death point" +
                "\nYour attacks can inflict Clipped Wings and produce hearts" +
                "\nAttacks have a chance to squeak and deal 1 damage to you" +
                "\nWhile attacking, gain massively increased damage and shadowflame, but less move and defenses and attack speed bonuses" +
                "\nSummons a friendly rainbow slime" +
                "\nCertain enemies will drop potions when defeated" +
                "\nWhen attacking by manually clicking, increases non-summon damage by 30%" +
                "\n[c/00FFFF:Following effects work passively from inventory or vanity slots:]" +
                "\n    Grants immunity to Mighty Wind, Suffocation, Chilled, and Guilty" +
                "\n    You have autofire and improved night vision" +
                "\n    Automatically uses healing & mana potions when needed and increases pickup range for mana stars" +
                "\n    You respawn with more life and when no boss is alive, respawn faster" +
                "\n    Press the Frigid Spell key to cast Ice Rod" +
                "\n    Right click to zoom and drastically improves reforges" +
                "\n    Stabilizes gravity in space and in liquids" +
                "\n'The amalgamate born of a thousand common enemies'"); */

			// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "生态集群");
			// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使你免疫霜冻、暗影焰、吱吱作响的玩具、愧疚、强风和窒息减益" +
			//     "\n使你免疫宇宙之火、剪除羽翼、残废、被网住和净化减益" +
			//     "\n使你免热恋、恶臭、迈达斯、邪咒减益，同时免疫仙人掌刺伤和敌人的偷取物品效果" +
			//     "\n攻击会造成剪除羽翼减益并生成霜火球和红心" +
			//     "\n允许所有武器自动挥舞、增强夜视效果并在非Boss战期间加快你的重生速度" +
			//     "\n在需要时自动使用魔力药水\r\n你在受到伤害时有几率发出吱吱声，并使这次受到的伤害降至1点" +
			//     "\n在你受到伤害后会发射暗影焰触手，你在重生时以更多生命重生" +
			//     "\n大多敌人在死亡时会掉落随机的药水，减少50%重铸价格" +
			//     "\n召唤一只彩虹史莱姆" +
			//     "\n使用此饰品后会将你传送至上一次死亡时的地点，右键缩放视域" +
			//     "\n'由上千普通敌人融合而成'");

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(0, 6);
			Item.defense = 6;
			Item.useTime = 180;
			Item.useAnimation = 180;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item6;
		}

		public static void PassiveEffect(Player player, Item item)
		{
			player.buffImmune[BuffID.WindPushed] = true;
			player.buffImmune[BuffID.Suffocation] = true;
			player.buffImmune[BuffID.Chilled] = true;
			player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
			player.buffImmune[ModContent.BuffType<LoosePocketsBuff>()] = true;

			player.nightVision = true;

			player.manaMagnet = true;
			player.manaFlower = true;
			player.AddEffect<MasoCarrotEffect>(item);

			FargoSoulsPlayer fargoPlayer = player.FargoSouls();
			fargoPlayer.SandsofTime = true;
			fargoPlayer.CactusImmune = true;
			fargoPlayer.SecurityWallet = true;
			fargoPlayer.TribalCharm = true;
			fargoPlayer.NymphsPerfumeRespawn = true;
			fargoPlayer.ConcentratedRainbowMatter = true;
			player.AddEffect<RainbowHealEffect>(item);
			fargoPlayer.FrigidGemstoneItem = item;
			player.AddEffect<StabilizedGravity>(item);
		}

		public override void UpdateInventory(Player player) => PassiveEffect(player, Item);
		public override void UpdateVanity(Player player) => PassiveEffect(player, Item);

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			PassiveEffect(player, Item);

			FargoSoulsPlayer fargoPlayer = player.FargoSouls();

			// Concentrated rainbow matter
			player.buffImmune[ModContent.BuffType<FlamesoftheUniverseBuff>()] = true;
			player.AddEffect<RainbowSlimeMinion>(Item);

			// Dragon fang
			player.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
			player.buffImmune[ModContent.BuffType<CrippledBuff>()] = true;
			player.AddEffect<ClippedEffect>(Item);

			// Frigid gemstone
			player.buffImmune[BuffID.Frostburn] = true;

			// Wretched pouch
			player.buffImmune[BuffID.ShadowFlame] = true;
			player.buffImmune[ModContent.BuffType<ShadowflameBuff>()] = true;
			player.AddEffect<WretchedPouchEffect>(Item);

			// Sands of time
			player.buffImmune[BuffID.WindPushed] = true;
			fargoPlayer.SandsofTime = true;

			// Squeaky toy
			player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToyBuff>()] = true;
			player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
			player.AddEffect<SqueakEffect>(Item);

			// Tribal charm
			player.buffImmune[BuffID.Webbed] = true;
			player.buffImmune[ModContent.BuffType<PurifiedBuff>()] = true;
			fargoPlayer.TribalCharm = true;
			fargoPlayer.TribalCharmEquipped = true;
			player.AddEffect<TribalCharmClickBonus>(Item);

			// Mystic skull
			player.buffImmune[BuffID.Suffocation] = true;
			player.manaMagnet = true;
			player.manaFlower = true;

			// Security wallet
			player.buffImmune[ModContent.BuffType<MidasBuff>()] = true;
			fargoPlayer.SecurityWallet = true;

			// Carrot
			player.nightVision = true;
			player.AddEffect<MasoCarrotEffect>(Item);

			// Nymph's perfume
			player.buffImmune[BuffID.Lovestruck] = true;
			player.buffImmune[ModContent.BuffType<LovestruckBuff>()] = true;
			player.buffImmune[ModContent.BuffType<HexedBuff>()] = true;
			player.buffImmune[BuffID.Stinky] = true;
			fargoPlayer.NymphsPerfumeRespawn = true;
			player.AddEffect<NymphPerfumeEffect>(Item);

			// Tim's concoction
			player.AddEffect<TimsConcoctionEffect>(Item);

			// Wyvern feather
			player.AddEffect<WyvernBalls>(Item);
		}

		public override void UseItemFrame(Player player) => SandsofTime.Use(player);
		public override bool? UseItem(Player player) => true;

		public override void AddRecipes()
		{
			CreateRecipe()

			.AddIngredient(ModContent.ItemType<ConcentratedRainbowMatter>())
			.AddIngredient(ModContent.ItemType<WyvernFeather>())
			.AddIngredient(ModContent.ItemType<FrigidGemstone>())
			.AddIngredient(ModContent.ItemType<SandsofTime>())
			.AddIngredient(ModContent.ItemType<SqueakyToy>())
			.AddIngredient(ModContent.ItemType<TribalCharm>())
			.AddIngredient(ModContent.ItemType<MysticSkull>())
			.AddIngredient(ModContent.ItemType<SecurityWallet>())
			.AddIngredient(ModContent.ItemType<OrdinaryCarrot>())
			.AddIngredient(ModContent.ItemType<WretchedPouch>())
			.AddIngredient(ModContent.ItemType<NymphsPerfume>())
			.AddIngredient(ModContent.ItemType<TimsConcoction>())
			.AddIngredient(ItemID.HallowedBar, 5)
			.AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

			.AddTile(TileID.MythrilAnvil)

			.Register();
		}
		public override bool CanRightClick() => true;
		public override void RightClick(Player player)
		{
			player.ReplaceItem(Item, ModContent.ItemType<BionomicClusterInactive>());
		}
	}
	public class BionomicClusterInactive : SoulsItem
	{
		public override bool Eternity => true;

		public override void SetStaticDefaults()
		{
			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 0;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.sellPrice(0, 6);
			Item.defense = 6;
			Item.useTime = 180;
			Item.useAnimation = 180;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item6;
		}

		public static void PassiveEffect(Player player, Item item)
		{
			player.buffImmune[BuffID.WindPushed] = true;
			player.buffImmune[BuffID.Suffocation] = true;
			player.buffImmune[BuffID.Chilled] = true;
			player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
			player.buffImmune[ModContent.BuffType<LoosePocketsBuff>()] = true;

			player.nightVision = true;

			player.manaMagnet = true;
			player.manaFlower = true;
			player.AddEffect<MasoCarrotEffect>(item);

			FargoSoulsPlayer fargoPlayer = player.FargoSouls();
			fargoPlayer.SandsofTime = true;
			fargoPlayer.SecurityWallet = true;
			fargoPlayer.TribalCharm = true;
			fargoPlayer.NymphsPerfumeRespawn = true;
			fargoPlayer.ConcentratedRainbowMatter = true;
			player.AddEffect<RainbowHealEffect>(item);
			fargoPlayer.FrigidGemstoneItem = item;
			player.AddEffect<StabilizedGravity>(item);
		}

		public override void UpdateInventory(Player player) { return; }//PassiveEffect(player, Item);
		public override void UpdateVanity(Player player) { return; }//PassiveEffect(player, Item);

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			PassiveEffect(player, Item);

			FargoSoulsPlayer fargoPlayer = player.FargoSouls();

			// Concentrated rainbow matter
			player.buffImmune[ModContent.BuffType<FlamesoftheUniverseBuff>()] = true;
			player.AddEffect<RainbowSlimeMinion>(Item);

			// Dragon fang
			player.buffImmune[ModContent.BuffType<ClippedWingsBuff>()] = true;
			player.buffImmune[ModContent.BuffType<CrippledBuff>()] = true;
			player.AddEffect<ClippedEffect>(Item);

			// Frigid gemstone
			player.buffImmune[BuffID.Frostburn] = true;

			// Wretched pouch
			player.buffImmune[BuffID.ShadowFlame] = true;
			player.buffImmune[ModContent.BuffType<ShadowflameBuff>()] = true;
			player.AddEffect<WretchedPouchEffect>(Item);

			// Sands of time
			player.buffImmune[BuffID.WindPushed] = true;
			fargoPlayer.SandsofTime = true;

			// Squeaky toy
			player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToyBuff>()] = true;
			player.buffImmune[ModContent.BuffType<GuiltyBuff>()] = true;
			player.AddEffect<SqueakEffect>(Item);

			// Tribal charm
			player.buffImmune[BuffID.Webbed] = true;
			player.buffImmune[ModContent.BuffType<PurifiedBuff>()] = true;
			fargoPlayer.TribalCharm = true;
			fargoPlayer.TribalCharmEquipped = true;
			player.AddEffect<TribalCharmClickBonus>(Item);

			// Mystic skull
			player.buffImmune[BuffID.Suffocation] = true;
			player.manaMagnet = true;
			player.manaFlower = true;

			// Security wallet
			player.buffImmune[ModContent.BuffType<MidasBuff>()] = true;
			fargoPlayer.SecurityWallet = true;

			// Carrot
			player.nightVision = true;
			player.AddEffect<MasoCarrotEffect>(Item);

			// Nymph's perfume
			player.buffImmune[BuffID.Lovestruck] = true;
			player.buffImmune[ModContent.BuffType<LovestruckBuff>()] = true;
			player.buffImmune[ModContent.BuffType<HexedBuff>()] = true;
			player.buffImmune[BuffID.Stinky] = true;
			fargoPlayer.NymphsPerfumeRespawn = true;
			player.AddEffect<NymphPerfumeEffect>(Item);

			// Tim's concoction
			player.AddEffect<TimsConcoctionEffect>(Item);
		}

		public override void UseItemFrame(Player player) => SandsofTime.Use(player);
		public override bool? UseItem(Player player) => true;

		public override bool CanRightClick() => true;
		public override void RightClick(Player player)
		{
			player.ReplaceItem(Item, ModContent.ItemType<BionomicCluster>());
		}
	}
}
