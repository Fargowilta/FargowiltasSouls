using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MeteorEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Meteor Enchantment");

            string tooltip =
@"Reduces momentum by 50%
You leave behind a trail of flames
A meteor shower initiates every few seconds while attacking
'Drop a draco on 'em'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(95, 71, 82);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Meteor");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.MeteorEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MeteorHelmet)
            .AddIngredient(ItemID.MeteorSuit)
            .AddIngredient(ItemID.MeteorLeggings)
            .AddIngredient(ItemID.SpaceGun)
            .AddIngredient(ItemID.SuperStarCannon)
            .AddIngredient(ItemID.MeteorStaff)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
