using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class GoldEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Gold Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "金魔石");
            
            string tooltip =
@"Your attacks inflict Midas
Press the Gold hotkey to be encased in a Golden Shell
You will not be able to move or attack, but will be immune to all damage
Press again to exit early
Effects of Greedy Ring
'Gold makes the world go round'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"攻击会造成迈达斯减益
按下'金身'键后会将你包裹在一个黄金壳中
被包裹时你无法移动或攻击，但你免疫所有伤害
再次按下'金身'键会使你提前离开黄金壳
拥有贪婪戒指效果
'黄金使世界运转'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(231, 178, 28);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Pink;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.GoldEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldHelmet);
            recipe.AddIngredient(ItemID.GoldChainmail);
            recipe.AddIngredient(ItemID.GoldGreaves);
            recipe.AddIngredient(ItemID.GoldCrown);
            recipe.AddIngredient(ItemID.GreedyRing);
            //recipe.AddIngredient(ItemID.CoinGun);
            recipe.AddIngredient(ItemID.SquirrelGold);
            //gold goldfish
            //ruby bunny
            //recipe.AddIngredient(ItemID.ParrotCracker);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
