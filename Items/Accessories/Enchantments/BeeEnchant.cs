using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BeeEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bee Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "蜜蜂魔石");
           
            string tooltip =
@"Increases the strength of friendly bees
Your piercing attacks spawn bees
'According to all known laws of aviation, there is no way a bee should be able to fly'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加友好蜜蜂的力量
穿透类弹幕在击中敌人时会生成蜜蜂
“根据目前所知的所有航空原理, 蜜蜂应该根本不可能会飞”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(254, 246, 37);
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
            player.GetModPlayer<FargoPlayer>().BeeEffect(hideVisual); //add effect
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.BeeHeadgear);
            recipe.AddIngredient(ItemID.BeeBreastplate);
            recipe.AddIngredient(ItemID.BeeGreaves);
            recipe.AddIngredient(ItemID.HiveBackpack);
            //stinger necklace
            recipe.AddIngredient(ItemID.BeeGun);
            //recipe.AddIngredient(ItemID.WaspGun);
            //recipe.AddIngredient(ItemID.Beenade, 50);
            //honey bomb
            recipe.AddIngredient(ItemID.Honeyfin);
            //recipe.AddIngredient(ItemID.Nectar);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
