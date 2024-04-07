using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class WizardEnchant : BaseEnchant
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			// DisplayName.SetDefault("Wizard Enchantment");
			/* Tooltip.SetDefault(
@"Enhances the power of all other Enchantments to their Force effects
'I'm a what?'"); */
		}

		public override Color nameColor => new(50, 80, 193);

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.rare = ItemRarityID.Pink;
			Item.value = 100000;
		}

		public override void SafeModifyTooltips(List<TooltipLine> tooltips)
		{
			base.SafeModifyTooltips(tooltips);

			if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
				itemNameLine.OverrideColor = nameColor;

			FargoSoulsPlayer localSoulsPlayer = Main.LocalPlayer.FargoSouls();
			foreach (BaseEnchant enchant in localSoulsPlayer.EquippedEnchants)
			{
				if (enchant.Type == Type)
				{
					continue;
				}
				if (localSoulsPlayer.ForceEffect(enchant.Type))
				{
					if (enchant.wizardEffect().Length != 0)
						tooltips.Add(new TooltipLine(Mod, "wizard", $"[i:{enchant.Item.type}] " + enchant.wizardEffect()));
				}
				else
				{
					if (enchant.wizardEffect().Length != 0)
					{
						tooltips.Add(new TooltipLine(Mod, "wizard", $"[i:{enchant.Item.type}] " + enchant.wizardEffect()));
						tooltips[tooltips.Count - 1].OverrideColor = Color.Gray;
					}


				}
			}
		}
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.FargoSouls().WizardEnchantActive = true;
			player.FargoSouls().WizardTooltips = true;
		}
		public override void UpdateVanity(Player player)
		{
			player.FargoSouls().WizardTooltips = true;
		}
		public override void UpdateInventory(Player player)
		{
			player.FargoSouls().WizardTooltips = true;
		}
		public override void AddRecipes()
		{
			CreateRecipe()

			.AddIngredient(ItemID.WizardHat)
			//.AddIngredient(ItemID.AmethystRobe);
			//.AddIngredient(ItemID.TopazRobe);

			.AddIngredient(ItemID.SapphireRobe)
			.AddIngredient(ItemID.EmeraldRobe)
			.AddIngredient(ItemID.RubyRobe)
			.AddIngredient(ItemID.DiamondRobe)
			//amber robe
			.AddIngredient(ItemID.RareEnchantment)

			.AddTile(TileID.CrystalBall)
			.Register();
		}
	}
}
