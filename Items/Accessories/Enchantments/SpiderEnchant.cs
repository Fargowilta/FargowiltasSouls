using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SpiderEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Spider Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "蜘蛛魔石");
            
            string tooltip =
@"Your minions and sentries can now crit with a 15% chance
'Arachnophobia is punishable by arachnid induced death'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"你的仆从和哨兵现在可以造成暴击且有15%基础暴击率
'对恐蛛症者可惩罚他们死于蜘蛛之口'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(109, 78, 69);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().SpiderEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiderMask);
            recipe.AddIngredient(ItemID.SpiderBreastplate);
            recipe.AddIngredient(ItemID.SpiderGreaves);
            recipe.AddIngredient(ItemID.SpiderStaff);
            recipe.AddIngredient(ItemID.QueenSpiderStaff);
            recipe.AddIngredient(ItemID.WebSlinger);
            //web rope coil
            //rainbow string
            //fried egg
            //recipe.AddIngredient(ItemID.SpiderEgg);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
