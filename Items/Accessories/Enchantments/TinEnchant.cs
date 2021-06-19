using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TinEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tin Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "锡魔石");
           
            string tooltip =
@"Sets your critical strike chance to 5%
Every crit will increase it by 5% up to double your current critical strike chance
Getting hit drops your crit back down
'Return of the Crit'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"将你的基础暴击率设为5%
每次暴击时都会增加5%暴击率，增加的暴击率的最大值为你当前最大暴击率数值x2
被击中后会将暴击率恢复为正常值
“暴击回归”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(162, 139, 78);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Blue;
            item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().TinEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.TinHelmet);
            recipe.AddIngredient(ItemID.TinChainmail);
            recipe.AddIngredient(ItemID.TinGreaves);
            //tin sword
            //recipe.AddIngredient(ItemID.TinBow);
            recipe.AddIngredient(ItemID.TopazStaff);
            recipe.AddIngredient(ItemID.YellowPhaseblade);
            //lemon
            //some unused butterfly
            recipe.AddIngredient(ItemID.Daylight);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
