using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PlatinumEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Platinum Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "铂金魔石");
            
            string tooltip =
@"20% chance for enemies to drop 2x loot
'Its value is immeasurable'";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch = 
@"敌人死亡时有20%的几率获得两倍的战利品
“无价之宝”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(83, 103, 143);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.LightRed;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            modPlayer.PlatinumEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumHelmet);
            recipe.AddIngredient(ItemID.PlatinumChainmail);
            recipe.AddIngredient(ItemID.PlatinumGreaves);
            recipe.AddIngredient(ItemID.PlatinumCrown);
            //recipe.AddIngredient(ItemID.PlatinumBroadsword);
            recipe.AddIngredient(ItemID.DiamondStaff);
            recipe.AddIngredient(ItemID.WhitePhaseblade);
            //recipe.AddIngredient(ItemID.TaxCollectorsStickOfDoom);
            //recipe.AddIngredient(ItemID.BeamSword);
            //recipe.AddIngredient(ItemID.DiamondRing);
            //diamond squirrel

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
