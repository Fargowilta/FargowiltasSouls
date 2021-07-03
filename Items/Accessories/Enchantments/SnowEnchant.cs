using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SnowEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snow Enchantment");
            Tooltip.SetDefault(
@"Spawns a snowstorm at your cursor
Any projectiles or npcs in the snowstorm are slowed by 50%
'It's Burning Cold Outside'");
            DisplayName.AddTranslation(GameCulture.Chinese, "防雪魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"在光标位置生成暴雪
暴雪中的所有弹幕和NPC的速度减缓50%
“冷的要死”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(37, 195, 242);
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
            item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().SnowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.EskimoHood);
            recipe.AddIngredient(ItemID.EskimoCoat);
            recipe.AddIngredient(ItemID.EskimoPants);
            //hand warmer
            //fruitcake chakram
            recipe.AddIngredient(ItemID.IceBoomerang);
            //frost daggerfish
            recipe.AddIngredient(ItemID.FrostMinnow);
            recipe.AddIngredient(ItemID.AtlanticCod);
            //recipe.AddIngredient(ItemID.Fish); 

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
