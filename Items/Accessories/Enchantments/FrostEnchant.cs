using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class FrostEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "冰霜魔石");
            
            string tooltip =
@"Icicles will start to appear around you
Attacking will launch them towards the cursor
When they hit an enemy they are frozen solid for up to 5 seconds
All hostile projectiles move at half speed
'Let's coat the world in a deep freeze'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"你的周围会出现冰锥
攻击时会将冰锥发射至光标位置
冰锥击中敌人时会使其短暂冻结并受到25%额外伤害5秒
在光标位置生成暴雪，其中的敌对弹幕飞行速度减半
“让我们给这个世界披上一层厚厚的冰衣”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(122, 189, 185);
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
            player.GetModPlayer<FargoPlayer>().FrostEffect(hideVisual);
            player.GetModPlayer<FargoPlayer>().SnowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FrostHelmet);
            recipe.AddIngredient(ItemID.FrostBreastplate);
            recipe.AddIngredient(ItemID.FrostLeggings);
            recipe.AddIngredient(ModContent.ItemType<SnowEnchant>());
            recipe.AddIngredient(ItemID.Frostbrand);
            recipe.AddIngredient(ItemID.IceBow);
            //frost staff
            //coolwhip
            //recipe.AddIngredient(ItemID.BlizzardStaff);
            //recipe.AddIngredient(ItemID.ToySled);
            //recipe.AddIngredient(ItemID.BabyGrinchMischiefWhistle);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
