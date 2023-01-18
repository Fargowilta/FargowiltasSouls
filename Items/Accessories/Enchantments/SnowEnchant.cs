using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SnowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Snow Enchantment");
            Tooltip.SetDefault(
@"Your attacks briefly inflict Frostburn
Press the Freeze Key to chill everything for 15 seconds
There is a 60 second cooldown for this effect
'It's Burning Cold Outside'");
        }

        protected override Color nameColor => new Color(37, 195, 242);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().SnowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.EskimoHood)
            .AddIngredient(ItemID.EskimoCoat)
            .AddIngredient(ItemID.EskimoPants)
            //hand warmer
            //fruitcake chakram
            .AddIngredient(ItemID.IceBoomerang)
            .AddIngredient(ItemID.FrostMinnow)
            .AddIngredient(ItemID.AtlanticCod)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
