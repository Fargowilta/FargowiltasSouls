using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class JungleEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Jungle Enchantment");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "丛林魔石");

            string tooltip =
@"Grants a double spore jump
Allows the ability to dash slightly
Double tap a direction
'The wrath of the jungle dwells within'";
            Tooltip.SetDefault(tooltip);
            //             string tooltip_ch =
            // @"使你获得孢子二段跳能力
            // '丛林之怒深藏其中'";
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        protected override Color nameColor => new Color(113, 151, 31);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().JungleEnchantActive = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.JungleHat)
            .AddIngredient(ItemID.JungleShirt)
            .AddIngredient(ItemID.JunglePants)
            .AddIngredient(ItemID.ThornChakram)
            .AddIngredient(ItemID.JungleYoyo)
            //snapthorn
            //staff of regrowth
            .AddIngredient(ItemID.JungleRose)
            //.AddIngredient(ItemID.Buggy);
            //panda pet

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
