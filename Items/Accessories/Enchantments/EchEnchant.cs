using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Fargowiltas.Items.Tiles;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
	public class EchEnchant : ModItem
	{
		private readonly Mod mutantmod = ModLoader.GetMod("Fargowiltas");
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ech Enchantment");
			Tooltip.SetDefault(":ech:"
				+ "\nHitting an enemy causes you to release 2 homing :ech:s"
				+ "\nOccasionally, you'll get :echbegone:, which is faster, larger, and deals 200% more damage"
			);
		}

		public override void ModifyTooltips(List<TooltipLine> list)
		{
			foreach (TooltipLine tooltipLine in list)
			{
				if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
				{
					tooltipLine.overrideColor = new Color(245, 190, 170);
				}
			}
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.accessory = true;
			ItemID.Sets.ItemNoGravity[item.type] = true;
			item.rare = 7;
			item.value = 100000;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<FargoPlayer>().EchEnchant = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CatMask);
			recipe.AddIngredient(ItemID.CatEars);
			recipe.AddIngredient(ItemID.CatShirt);
			recipe.AddIngredient(ItemID.CatPants);
			recipe.AddIngredient(ItemID.Meowmere);
			recipe.AddIngredient(ItemID.UnluckyYarn);
			// make ech painting obtainable or else :echceasebegone:
			recipe.AddIngredient(ModContent.ItemType<EchPainting>()); 
			
			recipe.AddTile(TileID.CrystalBall);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
