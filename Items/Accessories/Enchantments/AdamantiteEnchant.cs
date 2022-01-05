using FargowiltasSouls.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Adamantite Enchantment");
            //Tooltip.SetDefault("One of your projectiles will split into 3 every second" +
            //    "\n'Three degrees of seperation'");

            //DisplayName.AddTranslation(GameCulture.Chinese, "精金魔石");
            //ooltip.AddTranslation(GameCulture.Chinese, "每秒会随机使你的一个弹幕分裂成三个" +
            //    "\n'一气化三清！'");
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips.TryFindTooltipLine("ItemName", out TooltipLine itemNameLine))
                itemNameLine.overrideColor = new Color(221, 85, 125);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Lime;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<FargoPlayer>().AdamantiteEnchant = true;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyAdamHead");
            recipe.AddIngredient(ItemID.AdamantiteBreastplate);
            recipe.AddIngredient(ItemID.AdamantiteLeggings);
            // Adamantite sword
            recipe.AddIngredient(ItemID.AdamantiteGlaive);
            // Trident
            recipe.AddIngredient(ItemID.TitaniumTrident);
            // Seedler
            recipe.AddIngredient(ItemID.CrystalSerpent);
            //recipe.AddIngredient(ItemID.VenomStaff);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
