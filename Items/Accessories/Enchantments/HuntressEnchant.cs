using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class HuntressEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /**DisplayName.SetDefault("Huntress Enchantment");
            Tooltip.SetDefault(
@"Arrows will periodically fall towards your cursor
The arrow type is based on the first arrow in your inventory
Double tap down to create a localized rain of arrows at the cursor's position for a few seconds
This has a cooldown of 15 seconds
Explosive Traps recharge faster and oil enemies
Set oiled enemies on fire for extra damage
'The Hunt is On'");
            DisplayName.AddTranslation(GameCulture.Chinese, "女猎人魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"箭矢会定期落至你光标周围
箭矢的种类取决于你背包中第一个箭矢
双击'下'键后令箭雨倾斜在光标位置
此效果有15秒冷却时间
爆炸机关攻击速度更快且会造成涂油减益
点燃涂油的敌人以造成额外伤害
'狩猎开始了'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(122, 192, 76);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Yellow;
            item.value = 200000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().HuntressEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.HuntressWig);
            recipe.AddIngredient(ItemID.HuntressJerkin);
            recipe.AddIngredient(ItemID.HuntressPants);
            //recipe.AddIngredient(ItemID.HuntressBuckler);
            recipe.AddIngredient(ItemID.DD2ExplosiveTrapT2Popper);
            //tendon bow
            recipe.AddIngredient(ItemID.DaedalusStormbow);
            //shadiwflame bow
            recipe.AddIngredient(ItemID.DD2PhoenixBow);
            //dog pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
