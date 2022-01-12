using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RichMahoganyEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Rich Mahogany Enchantment");
            Tooltip.SetDefault(
@"All grappling hooks shoot, pull, and retract 1.5x as fast
'Guaranteed to keep you hooked'");
            DisplayName.AddTranslation(GameCulture.Chinese, "红木魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"所有钩爪的抛出速度、牵引速度和回收速度x1.5
'保证钩到你'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(181, 108, 100);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Green;
            item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().MahoganyEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.RichMahoganyHelmet);
            recipe.AddIngredient(ItemID.RichMahoganyBreastplate);
            recipe.AddIngredient(ItemID.RichMahoganyGreaves);
            //rich mahog sword
            recipe.AddIngredient(ItemID.IvyWhip);
            //grappling hook
            //mango/pineapple
            //recipe.AddIngredient(ItemID.Frog);
            recipe.AddIngredient(ItemID.Moonglow);
            recipe.AddIngredient(ItemID.DoNotStepontheGrass);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
