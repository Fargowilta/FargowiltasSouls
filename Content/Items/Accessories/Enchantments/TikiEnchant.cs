using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class TikiEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Tiki Enchantment");
			/* Tooltip.SetDefault(
@"Increases whip range by 20%
Whip your summons to make them work harder
'Aku Aku!'"); */
			//             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "提基魔石");
			//             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
			// @"在召唤栏用光后你仍可以召唤临时的哨兵和仆从
			// 'Aku Aku!'");
		}

		public override Color nameColor => new(86, 165, 43);


		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Lime;
			Item.value = 150000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			AddEffects(player, Item);
		}
		public static void AddEffects(Player player, Item item)
		{
			if (player.FargoSouls().ForceEffect<TikiEnchant>())
				player.whipRangeMultiplier += 0.2f;
			player.AddEffect<TikiEffect>(item);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ItemID.TikiMask)
			.AddIngredient(ItemID.TikiShirt)
			.AddIngredient(ItemID.TikiPants)
			//leaf wings
			.AddIngredient(ItemID.Blowgun)
			//toxic flask
			.AddIngredient(ItemID.PygmyStaff)
			.AddIngredient(ItemID.PirateStaff)
			//kaledoscope
			//.AddIngredient(ItemID.TikiTotem);

			.AddTile(TileID.CrystalBall)
			.Register();
		}
	}
	public class TikiEffect : AccessoryEffect
	{
		public override Header ToggleHeader => Header.GetHeader<SpiritHeader>();
		public override int ToggleItemType => ModContent.ItemType<TikiEnchant>();
	}
}
