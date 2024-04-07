using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class MinerEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Miner Enchantment");
			/* Tooltip.SetDefault(
@"50% increased mining speed
Shows the location of enemies, traps, and treasures
Effects of Night Owl, Spelunker, Hunter, Shine, and Dangersense Potions
'The planet trembles with each swing of your pick'"); */
			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "矿工魔石");
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
			// @"增加50%挖掘速度
			// 高亮标记敌人、陷阱和宝藏
			// 你会散发光芒
			// '大地随着你的每一次挥镐而颤动'");
		}

		public override Color nameColor => new(95, 117, 151);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Blue;
			Item.value = 20000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			FargoSoulsPlayer modPlayer = player.FargoSouls();
			float speed = modPlayer.ForceEffect<MinerEnchant>() ? .75f : .5f;
			AddEffects(player, speed, Item);
		}

		public static void AddEffects(Player player, float pickSpeed, Item item)
		{
			player.pickSpeed -= pickSpeed;
			player.nightVision = true;

			player.AddEffect<MiningSpelunk>(item);
			player.AddEffect<MiningHunt>(item);
			player.AddEffect<MiningDanger>(item);
			player.AddEffect<MiningShine>(item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.UltrabrightHelmet)
				.AddIngredient(ItemID.MiningShirt)
				.AddIngredient(ItemID.MiningPants)
				.AddIngredient(ItemID.AncientChisel)
				.AddIngredient(ItemID.CopperPickaxe)
				.AddIngredient(ItemID.GravediggerShovel)
				.AddTile(TileID.DemonAltar)
				.Register();
		}
	}
	public class MiningSpelunk : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
		public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
		public override void PostUpdateEquips(Player player)
		{
			player.findTreasure = true;
		}
	}
	public class MiningHunt : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
		public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
		public override void PostUpdateEquips(Player player)
		{
			player.detectCreature = true;
		}
	}
	public class MiningDanger : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
		public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
		public override void PostUpdateEquips(Player player)
		{
			player.dangerSense = true;
		}
	}
	public class MiningShine : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
		public override int ToggleItemType => ModContent.ItemType<MinerEnchant>();
		public override void PostUpdateEquips(Player player)
		{
			Lighting.AddLight(player.Center, 0.8f, 0.8f, 0);
		}
	}
}
