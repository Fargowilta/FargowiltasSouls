using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CopperEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Copper Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "铜魔石");
            
            string tooltip =
@"Attacks have a chance to shock enemies with lightning
'Behold'";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"攻击有几率释放闪电击打敌人
'凝视'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(213, 102, 23);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Orange;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().CopperEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CopperHelmet);
            recipe.AddIngredient(ItemID.CopperChainmail);
            recipe.AddIngredient(ItemID.CopperGreaves);
            recipe.AddIngredient(ItemID.CopperShortsword);
            recipe.AddIngredient(ItemID.AmethystStaff);
            //recipe.AddIngredient(ItemID.PurplePhaseblade);
            //thunder zapper
            //recipe.AddIngredient(ItemID.Wire, 20);
            //daybloom
            recipe.AddIngredient(ItemID.FirstEncounter);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
