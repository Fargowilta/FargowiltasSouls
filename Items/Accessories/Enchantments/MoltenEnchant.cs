using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MoltenEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Enchantment");
            Tooltip.SetDefault(
@"Nearby enemies are ignited
The closer they are to you the more damage they take
When you are hurt, you violently explode to damage nearby enemies
'They shall know the fury of hell' ");
            DisplayName.AddTranslation(GameCulture.Chinese, "熔融魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"引燃你附近的敌人
敌人离你越近，受到的伤害越高
你受到伤害时会剧烈爆炸并对附近的敌人造成伤害
“他们将感受到地狱的愤怒” ");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(193, 43, 43);
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
            player.GetModPlayer<FargoPlayer>().MoltenEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MoltenHelmet);
            recipe.AddIngredient(ItemID.MoltenBreastplate);
            recipe.AddIngredient(ItemID.MoltenGreaves);
            recipe.AddIngredient(ItemID.Sunfury);
            //recipe.AddIngredient(ItemID.MoltenFury);
            recipe.AddIngredient(ItemID.PhoenixBlaster);
            //recipe.AddIngredient(ItemID.DarkLance);
            //lavafly
            recipe.AddIngredient(ItemID.DemonsEye);
            //imp pet

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
