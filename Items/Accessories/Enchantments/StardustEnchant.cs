using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class StardustEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Stardust Enchantment");
            Tooltip.SetDefault(
@"Double tap down to direct your empowered guardian
Press the Freeze Key to freeze time for 5 seconds
While time is frozen, your minions will continue to attack and Stardust Guardian gains a new attack
There is a 60 second cooldown for this effect
'The power of the Stand is yours'");
            DisplayName.AddTranslation(GameCulture.Chinese, "星尘魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"双击'下'键将你的守卫引至光标位置
按下'冻结'键后会冻结5秒时间
你的召唤物不受时间冻结影响且星尘守卫在时间冻结期间获得全新的攻击
此效果有60秒冷却时间
'替身之力归你所有'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(0, 174, 238);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Red;
            item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().StardustEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StardustHelmet);
            recipe.AddIngredient(ItemID.StardustBreastplate);
            recipe.AddIngredient(ItemID.StardustLeggings);
            //stardust wings
            //recipe.AddIngredient(ItemID.StardustPickaxe);
            recipe.AddIngredient(ItemID.StardustCellStaff); //estee pet
            recipe.AddIngredient(ItemID.StardustDragonStaff);
            recipe.AddIngredient(ItemID.RainbowCrystalStaff);
            //MoonlordTurretStaff
            

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
