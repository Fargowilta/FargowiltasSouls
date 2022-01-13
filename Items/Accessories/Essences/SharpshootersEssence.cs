using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Essences
{
    public class SharpshootersEssence : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Sharpshooter's Essence");
            Tooltip.SetDefault(
@"18% increased ranged damage
10% chance to not consume ammo
5% increased ranged critical chance
'This is only the beginning..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "神射手精华");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"增加18%远程伤害
10%几率不消耗弹药
增加5%远程暴击率
'这只是个开始...'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color?(new Color(188, 253, 68));
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 150000;
            item.rare = ItemRarityID.LightRed;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.rangedDamage += .18f;
            player.rangedCrit += 5;
            player.GetModPlayer<FargoPlayer>().RangedEssence = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            //no others
            recipe.AddIngredient(ItemID.RangerEmblem);
            recipe.AddIngredient(ItemID.PainterPaintballGun);
            recipe.AddIngredient(ItemID.SnowballCannon);
            recipe.AddIngredient(ItemID.RedRyder);
            recipe.AddIngredient(ItemID.Harpoon);
            recipe.AddIngredient(ItemID.Musket);
            recipe.AddIngredient(ItemID.Boomstick);
            recipe.AddIngredient(ItemID.BeesKnees);
            recipe.AddIngredient(ItemID.HellwingBow);
            recipe.AddIngredient(ItemID.HallowedBar, 5);

            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
