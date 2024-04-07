using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
	public class EarthForce : BaseForce
	{
		public override void SetStaticDefaults()
		{
			Enchants[Type] = new int[]
			{
				ModContent.ItemType<CobaltEnchant>(),
				ModContent.ItemType<PalladiumEnchant>(),
				ModContent.ItemType<MythrilEnchant>(),
				ModContent.ItemType<OrichalcumEnchant>(),
				ModContent.ItemType<AdamantiteEnchant>(),
				ModContent.ItemType<TitaniumEnchant>()
			};
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			SetActive(player);
			player.AddEffect<AncientCobaltEffect>(Item);
			player.AddEffect<CobaltEffect>(Item);
			player.AddEffect<PalladiumEffect>(Item);
			player.AddEffect<PalladiumHealing>(Item);
			player.AddEffect<MythrilEffect>(Item);
			player.AddEffect<OrichalcumEffect>(Item);
			player.AddEffect<AdamantiteEffect>(Item);
			player.AddEffect<TitaniumEffect>(Item);

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			foreach (int ench in Enchants[Type])
				recipe.AddIngredient(ench);
			recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
			recipe.Register();
		}
	}
}
