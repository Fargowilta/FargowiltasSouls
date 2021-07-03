using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class LeadEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Enchantment");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "铅魔石");
            
            string tooltip =
@"Attacks may inflict enemies with Lead Poisoning
Lead Poisoning deals damage over time and spreads to nearby enemies
'Not recommended for eating'";
            Tooltip.SetDefault(tooltip);
            string tooltip_ch =
@"攻击有几率造成铅中毒减益
铅中毒减益持续造成伤害并且会扩散至周围的敌人
“不建议食用”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(67, 69, 88);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Blue;
            item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().LeadEnchant = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LeadHelmet);
            recipe.AddIngredient(ItemID.LeadChainmail);
            recipe.AddIngredient(ItemID.LeadGreaves);
            //recipe.AddIngredient(ItemID.LeadPickaxe);
            //lead axe
            recipe.AddIngredient(ItemID.LeadShortsword);
            //lead bow
            //black paint
            recipe.AddIngredient(ItemID.GrayPaint, 100);
            recipe.AddIngredient(ItemID.SulphurButterfly);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
