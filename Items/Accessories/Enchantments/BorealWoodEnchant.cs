using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BorealWoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boreal Wood Enchantment");
            Tooltip.SetDefault(
@"Attacks will periodically be accompanied by several snowballs
'The cooler wood'");
            DisplayName.AddTranslation(GameCulture.Chinese, "针叶木魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"攻击时定期释放几个雪球
“冷木”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(139, 116, 100);
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
            player.GetModPlayer<FargoPlayer>().BorealEnchant = true;
            player.GetModPlayer<FargoPlayer>().AdditionalAttacks = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BorealWoodHelmet);
            recipe.AddIngredient(ItemID.BorealWoodBreastplate);
            recipe.AddIngredient(ItemID.BorealWoodGreaves);
            //recipe.AddIngredient(ItemID.BorealWoodSword);
            //recipe.AddIngredient(ItemID.BorealWoodBow);
            recipe.AddIngredient(ItemID.Snowball, 300);
            recipe.AddIngredient(ItemID.Shiverthorn);
            //cherry/plum
            //recipe.AddIngredient(ItemID.Penguin);
            recipe.AddIngredient(ItemID.ColdWatersintheWhiteLand);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
