using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Titanium Enchantment");
            Tooltip.SetDefault(
@"Briefly become invulnerable after striking an enemy
'Hit me with your best shot'");
            DisplayName.AddTranslation(GameCulture.Chinese, "钛金魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"攻击敌人后会使你无敌一小段时间
'Hit me with your best shot'（某歌曲名）");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(130, 140, 136);
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
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().TitaniumEffect(); //new set bonus soon
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyTitaHead");
            recipe.AddIngredient(ItemID.TitaniumBreastplate);
            recipe.AddIngredient(ItemID.TitaniumLeggings);
            //recipe.AddIngredient(ItemID.TitaniumDrill);
            recipe.AddIngredient(ItemID.TitaniumSword);
            recipe.AddIngredient(ItemID.Rockfish);
            recipe.AddIngredient(ItemID.WhitePhasesaber);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
