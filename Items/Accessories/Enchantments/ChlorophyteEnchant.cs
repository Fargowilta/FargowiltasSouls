using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ChlorophyteEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chlorophyte Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "叶绿魔石");
            
            string tooltip =
@"Summons a ring of leaf crystals to shoot at nearby enemies
Grants a double spore jump
While using wings, spores will continuously spawn
'The jungle's essence crystallizes around you'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"召唤一圈叶状水晶射向附近的敌人
使你获得孢子二段跳能力
使用翅膀进行飞行时，你周围会不断生成孢子
“丛林的精华在你周围结晶”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(36, 137, 0);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Lime;
            item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //crystal
            modPlayer.ChloroEffect(hideVisual);
            modPlayer.JungleEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyChloroHead");
            recipe.AddIngredient(ItemID.ChlorophytePlateMail);
            recipe.AddIngredient(ItemID.ChlorophyteGreaves);
            recipe.AddIngredient(null, "JungleEnchant");
            recipe.AddIngredient(ItemID.ChlorophyteWarhammer);
            recipe.AddIngredient(ItemID.ChlorophyteClaymore);
            //grape juice
            //recipe.AddIngredient(ItemID.Seedling);
            //plantero pet

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
