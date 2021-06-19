using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RedRidingEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Riding Enchantment");
            Tooltip.SetDefault(
@"Arrows will periodically fall towards your cursor
Double tap down to create a rain of arrows that follows the cursor's position for a few seconds
The arrow type is based on the first arrow in your inventory
This has a cooldown of 10 seconds
Greatly enhances Explosive Traps effectiveness
'Big Bad Red Riding Hood'");
            DisplayName.AddTranslation(GameCulture.Chinese, "红色骑术魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"箭矢会定期落至你光标周围
箭矢的种类取决于你背包中第一个箭矢
双击“下”键后令箭雨倾泄在光标位置
此效果有10秒冷却时间
大幅强化爆炸陷阱的效果
“小红帽大坏蛋！”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(192, 27, 60);
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
            item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.RedRidingEffect(hideVisual);
            modPlayer.HuntressEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HuntressAltHead);
            recipe.AddIngredient(ItemID.HuntressAltShirt);
            recipe.AddIngredient(ItemID.HuntressAltPants);
            recipe.AddIngredient(null, "HuntressEnchant");
            //eventide
            recipe.AddIngredient(ItemID.Marrow);
            recipe.AddIngredient(ItemID.DD2BetsyBow);
            //recipe.AddIngredient(ItemID.DogWhistle); //werewolf pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
