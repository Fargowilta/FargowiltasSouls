using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
	public class SpiritForce : BaseForce
	{
		public override void SetStaticDefaults()
		{
			Enchants[Type] = new int[]
			{
				ModContent.ItemType<FossilEnchant>(),
				ModContent.ItemType<ForbiddenEnchant>(),
				ModContent.ItemType<HallowEnchant>(),
				ModContent.ItemType<AncientHallowEnchant>(),
				ModContent.ItemType<TikiEnchant>(),
				ModContent.ItemType<SpectreEnchant>()
			};
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SetActive(player);

			player.AddEffect<FossilEffect>(Item);
			player.AddEffect<FossilBones>(Item);
			ForbiddenEnchant.AddEffects(player, Item);
			player.AddEffect<HallowEffect>(Item);
			AncientHallowEnchant.AddEffects(player, Item);
			TikiEnchant.AddEffects(player, Item);
			player.AddEffect<SpectreEffect>(Item);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			foreach (int ench in Enchants[Type])
				recipe.AddIngredient(ench);
			recipe.AddTile<CrucibleCosmosSheet>();
			recipe.Register();
		}
	}
}
