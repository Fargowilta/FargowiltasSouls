using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
	public class ChaliceoftheMoon : SoulsItem
	{
		public override bool Eternity => true;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Chalice of the Moon");
			/* Tooltip.SetDefault(@"Grants immunity to Venom, Ivy Venom, Burning, Fused, Low Ground, and Marked for Death
Grants immunity to Swarming, Atrophied, Jammed, Reverse Mana Flow, and Antisocial
Press the Magical Cleanse key to cure yourself of most debuffs
Increases life regeneration based on how much light you receive
Double tap DOWN in the air to fastfall
Fastfall will create a fiery eruption on impact after falling a certain distance
When you land after a jump, you create a burst of boulders
You fire additional attacks depending on weapon type and erupt into Ancient Visions when injured
Summons a friendly Cultist and plant to fight at your side
'Consume it'"); */
			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "月之杯");
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"月亮的微笑
			// 免疫毒液, 常春藤毒, 燃烧, 导火线, 低地和死亡标记
			// 免疫蜂群, 萎缩, 卡壳, 反魔力流和反社交
			// 增加生命回复
			// 在空中按'下'键快速下落
			// 在一定高度使用快速下落, 会在撞击地面时产生猛烈的火焰喷发
			// 根据武器类型定期发动额外的攻击
			// 受伤时爆发尖钉球和远古幻象攻击敌人
			// 召唤友善的邪教徒和植物为你而战");

			Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 54;
			Item.accessory = true;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.sellPrice(0, 7);
			Item.defense = 8;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			FargoSoulsPlayer fargoPlayer = player.FargoSouls();

			//magical bulb
			MagicalBulb.AddEffects(player, Item);

			//lihzahrd treasure
			player.buffImmune[BuffID.Burning] = true;
			player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
			player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()] = true;
			player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
			fargoPlayer.LihzahrdTreasureBoxItem = Item;
			player.AddEffect<LihzahrdGroundPound>(Item);
			player.AddEffect<LihzahrdBoulders>(Item);

			//celestial rune
			player.buffImmune[ModContent.BuffType<MarkedforDeathBuff>()] = true;
			player.AddEffect<CelestialRuneAttacks>(Item);

			//chalice
			player.buffImmune[ModContent.BuffType<AtrophiedBuff>()] = true;
			player.buffImmune[ModContent.BuffType<JammedBuff>()] = true;
			player.buffImmune[ModContent.BuffType<ReverseManaFlowBuff>()] = true;
			player.buffImmune[ModContent.BuffType<AntisocialBuff>()] = true;
			fargoPlayer.MoonChalice = true;
			player.AddEffect<CultistMinionEffect>(Item);

		}

		public override void AddRecipes()
		{
			CreateRecipe()

			.AddIngredient(ModContent.ItemType<MagicalBulb>())
			.AddIngredient(ModContent.ItemType<LihzahrdTreasureBox>())
			.AddIngredient(ModContent.ItemType<CelestialRune>())
			.AddIngredient(ItemID.FragmentSolar, 1)
			.AddIngredient(ItemID.FragmentVortex, 1)
			.AddIngredient(ItemID.FragmentNebula, 1)
			.AddIngredient(ItemID.FragmentStardust, 1)
			.AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

			.AddTile(TileID.LunarCraftingStation)

			.Register();
		}
	}
	public class CultistMinionEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
		public override int ToggleItemType => ModContent.ItemType<ChaliceoftheMoon>();
		public override bool MinionEffect => true;
		public override void PostUpdateEquips(Player player)
		{
			if (!player.HasBuff<SouloftheMasochistBuff>())
				player.AddBuff(ModContent.BuffType<LunarCultistBuff>(), 2);
		}
	}
}