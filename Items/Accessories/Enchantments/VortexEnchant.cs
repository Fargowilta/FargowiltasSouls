using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class VortexEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Vortex Enchantment");
            Tooltip.SetDefault(
@"Double tap down to toggle stealth, reducing chance for enemies to target you but slowing movement
When entering stealth, spawn a vortex that draws in enemies and projectiles
While in stealth, your own projectiles will not be sucked in
'Tear into reality'");
            DisplayName.AddTranslation(GameCulture.Chinese, "星旋魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"双击'下'键切换至隐形模式，减少敌人以你为目标的几率，但大幅降低移动速度
进入隐形状态时生成一个会吸引敌人和弹幕的旋涡
处于隐形状态时你的弹幕不会被旋涡吸引
'撕裂现实'");**/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(0, 242, 170);
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
            player.GetModPlayer<FargoPlayer>().VortexEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.VortexHelmet);
            recipe.AddIngredient(ItemID.VortexBreastplate);
            recipe.AddIngredient(ItemID.VortexLeggings);
            //vortex wings
            recipe.AddIngredient(ItemID.VortexBeater);
            recipe.AddIngredient(ItemID.Phantasm);
            //chain gun
            //electrosphere launcher
            recipe.AddIngredient(ItemID.SDMG);
            //recipe.AddIngredient(ItemID.CompanionCube);

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
