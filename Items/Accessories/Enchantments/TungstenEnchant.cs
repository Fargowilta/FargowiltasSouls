using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TungstenEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tungsten Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "钨魔石");
            
            string tooltip =
@"150% increased sword size
Every half second a projectile will be doubled in size
Enlarged swords and projectiles deal 10% more damage and have an additional chance to crit
'Bigger is always better'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"剑的尺寸增加150%
每过0.5秒便会使一个弹幕的尺寸翻倍
尺寸变大的剑和弹幕会额外造成10%伤害并且暴击率增加
“大就是好”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(176, 210, 178);
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
            item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().TungstenEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TungstenHelmet);
            recipe.AddIngredient(ItemID.TungstenChainmail);
            recipe.AddIngredient(ItemID.TungstenGreaves);
            //tungsten sword
            //ruler
            recipe.AddIngredient(ItemID.CandyCaneSword);
            recipe.AddIngredient(ItemID.GreenPhaseblade);
            recipe.AddIngredient(ItemID.EmeraldStaff);
            //recipe.AddIngredient(ItemID.Snail);
            //recipe.AddIngredient(ItemID.Sluggy);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
