using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ForbiddenEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "禁戒魔石");
            
            string tooltip =
@"Double tap down to call an ancient storm to the cursor location
Any projectiles shot through your storm gain 30% damage
'Walk like an Egyptian'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"双击“下”键将远古风暴召唤至光标位置
穿过远古风暴的弹幕会额外获得30%伤害
“走路像个埃及人”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
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
            player.GetModPlayer<FargoPlayer>().ForbiddenEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AncientBattleArmorHat);
            recipe.AddIngredient(ItemID.AncientBattleArmorShirt);
            recipe.AddIngredient(ItemID.AncientBattleArmorPants);
            //sun mask/moon mask
            recipe.AddIngredient(ItemID.DjinnsCurse);
            recipe.AddIngredient(ItemID.SpiritFlame);
            recipe.AddIngredient(ItemID.SkyFracture);
            //sky fracture
            //recipe.AddIngredient(ItemID.RainbowRod);

            //recipe.AddRecipeGroup("FargowiltasSouls:AnyScorpion");
            //fennec fox pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
