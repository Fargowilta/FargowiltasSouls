using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RainEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Rain Enchantment");
            Tooltip.SetDefault(
@"Grants immunity to Wet
Spawns a miniature storm to follow you around
Shooting it will make it grow
At maximum size, attacks will turn into lightning bolts
'Come again some other day'");
            DisplayName.AddTranslation(GameCulture.Chinese, "雨云魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"使你免疫潮湿减益
召唤一个微型风暴跟着你
向其射击会使其变大
尺寸达到最大时攻击会转化为闪电
'改日再来'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(255, 236, 0);
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
            player.GetModPlayer<FargoPlayer>().RainEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.RainHat);
            recipe.AddIngredient(ItemID.RainCoat);
            recipe.AddIngredient(ItemID.UmbrellaHat);
            //inner tube
            recipe.AddIngredient(ItemID.Umbrella);
            //tragic umbrella
            recipe.AddIngredient(ItemID.NimbusRod);
            recipe.AddIngredient(ItemID.WaterGun);
            //recipe.AddIngredient(ItemID.RainbowBrick, 50);
            //volt bunny pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
