using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class GladiatorEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator Enchantment");
            Tooltip.SetDefault(
@"Spears will rain down on struck enemies
'Are you not entertained?'");
            DisplayName.AddTranslation(GameCulture.Chinese, "角斗士魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"长矛将倾泄在被攻击的敌人身上
“难道你不高兴吗？”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(156, 146, 78);
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
            item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().GladiatorEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.GladiatorHelmet);
            recipe.AddIngredient(ItemID.GladiatorBreastplate);
            recipe.AddIngredient(ItemID.GladiatorLeggings);
            recipe.AddIngredient(ItemID.Spear); //gladius
            recipe.AddIngredient(ItemID.Javelin, 300);
            recipe.AddIngredient(ItemID.BoneJavelin, 300);
            
            //recipe.AddIngredient(ItemID.TartarSauce);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
