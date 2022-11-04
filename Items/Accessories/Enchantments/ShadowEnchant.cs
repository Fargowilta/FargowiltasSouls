using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShadowEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shadow Enchantment");
            Tooltip.SetDefault(
@"Two Shadow Orbs will orbit around you
Attacking a Shadow Orb will cause it to release a burst of homing shadow energy
'You feel your body slip into the deepest of shadows'");
        }

        protected override Color nameColor => new Color(66, 53, 111);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().ShadowEffect(hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ShadowHelmet)
            .AddIngredient(ItemID.ShadowScalemail)
            .AddIngredient(ItemID.ShadowGreaves)
            .AddIngredient(ItemID.Musket)
            .AddIngredient(ItemID.WarAxeoftheNight)
            .AddIngredient(ItemID.ShadowOrb)

            //ball o hurt
            //demon bow

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
