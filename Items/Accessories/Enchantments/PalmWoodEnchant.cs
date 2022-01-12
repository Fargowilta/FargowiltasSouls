using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalmWoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Palm Wood Enchantment");
            Tooltip.SetDefault(
@"Double tap down to spawn a palm tree sentry that throws nuts at enemies
'Alarmingly calm'");
            DisplayName.AddTranslation(GameCulture.Chinese, "棕榈木魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"双击'下'键会召唤一个会向敌人扔橡子的棕榈树哨兵
'出奇的宁静'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(183, 141, 86);
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
            player.GetModPlayer<FargoPlayer>().PalmEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.PalmWoodHelmet);
            recipe.AddIngredient(ItemID.PalmWoodBreastplate);
            recipe.AddIngredient(ItemID.PalmWoodGreaves);
            recipe.AddIngredient(ItemID.PalmWoodSword);
            recipe.AddIngredient(ItemID.BreathingReed);
            //recipe.AddIngredient(ItemID.BlackInk);
            //coconut
            //seagull
            recipe.AddIngredient(ItemID.Tuna);
            //shark pet

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
