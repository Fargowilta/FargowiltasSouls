using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class JungleEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Jungle Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "丛林魔石");
            
            string tooltip =
@"Grants a double spore jump
Allows the ability to dash slightly
Double tap a direction
'The wrath of the jungle dwells within'";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"使你获得孢子二段跳能力
'丛林之怒深藏其中'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(113, 151, 31);
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
            item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().JungleEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.JungleHat);
            recipe.AddIngredient(ItemID.JungleShirt);
            recipe.AddIngredient(ItemID.JunglePants);
            recipe.AddIngredient(ItemID.ThornChakram);
            recipe.AddIngredient(ItemID.JungleYoyo);
            //snapthorn
            //staff of regrowth
            recipe.AddIngredient(ItemID.JungleRose);
            //recipe.AddIngredient(ItemID.Buggy);
            //panda pet

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
