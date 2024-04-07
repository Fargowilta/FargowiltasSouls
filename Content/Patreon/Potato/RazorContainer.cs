using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Potato
{
	public class RazorContainer : PatreonModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = 10000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
			modPlayer.RazorContainer = true;


			//spawn a blade if none exist
			if (player.ownedProjectileCounts[ModContent.ProjectileType<RazorBlade>()] < 1)
			{
				Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, new Vector2(Main.rand.Next(-2, 2), -2), ModContent.ProjectileType<RazorBlade>(), 20, 2f, player.whoAmI);
			}
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.Wood, 10)
				.AddRecipeGroup(RecipeGroupID.IronBar, 15)
				.AddIngredient(ItemID.Chain, 6)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
