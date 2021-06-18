using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CrimsonEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Enchantment");
            Tooltip.SetDefault(
@"After taking a hit, regen is greatly increased until the half the hit is healed off
If you take another hit before it's healed, you lose the heal in addition to normal damage
'The blood of your enemy is your rebirth'");
            DisplayName.AddTranslation(GameCulture.Chinese, "猩红魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"在你受到伤害后大幅增加你的生命恢复速度，直至你恢复的生命量等同于这次受到的伤害量的一半
如果你在完成恢复前再次受伤，则不会触发增加生命恢复的效果
“你从敌人的血中重生”");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(200, 54, 75);
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
            player.GetModPlayer<FargoPlayer>().CrimsonEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimsonHelmet);
            recipe.AddIngredient(ItemID.CrimsonScalemail);
            recipe.AddIngredient(ItemID.CrimsonGreaves);
            //blood axe tging
            recipe.AddIngredient(ItemID.TheUndertaker);
            recipe.AddIngredient(ItemID.TheMeatball);
            recipe.AddIngredient(ItemID.CrimsonHeart);

            //blood rain bow
            //flesh catcher rod
            //recipe.AddIngredient(ItemID.BoneRattle);
            //recipe.AddIngredient(ItemID.CrimsonHeart);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
