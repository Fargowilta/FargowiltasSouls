using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TikiEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Tiki Enchantment");
            Tooltip.SetDefault(
@"You may continue to summon temporary minions and sentries after maxing out on your slots
Reduces attack speed of summon weapons when effect is activated
'Aku Aku!'");
            DisplayName.AddTranslation(GameCulture.Chinese, "提基魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"在召唤栏用光后你仍可以召唤临时的哨兵和仆从
'Aku Aku!'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(86, 165, 43);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Lime;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().TikiEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TikiMask);
            recipe.AddIngredient(ItemID.TikiShirt);
            recipe.AddIngredient(ItemID.TikiPants);
            //leaf wings
            recipe.AddIngredient(ItemID.Blowgun);
            //toxic flask
            recipe.AddIngredient(ItemID.PygmyStaff);
            recipe.AddIngredient(ItemID.PirateStaff);
            //kaledoscope
            //recipe.AddIngredient(ItemID.TikiTotem);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
