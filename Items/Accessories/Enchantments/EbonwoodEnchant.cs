using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class EbonwoodEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Ebonwood Enchantment");
            Tooltip.SetDefault(
@"You have an aura of Shadowflame
'Untapped potential'");
            DisplayName.AddTranslation(GameCulture.Chinese, "乌木魔石");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"一圈暗影焰光环环绕着你
'未开发的潜力'");*/
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(100, 90, 141);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Green;
            item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().EbonEffect();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.EbonwoodHelmet);
            recipe.AddIngredient(ItemID.EbonwoodBreastplate);
            recipe.AddIngredient(ItemID.EbonwoodGreaves);
            recipe.AddIngredient(ItemID.EbonwoodSword);
            //recipe.AddIngredient(ItemID.EbonwoodBow);
            //recipe.AddIngredient(ItemID.Deathweed);
            recipe.AddIngredient(ItemID.VileMushroom);
            //elderberry/blackcurrant
            //recipe.AddIngredient(ItemID.Ebonkoi);
            recipe.AddIngredient(ItemID.LightlessChasms);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
