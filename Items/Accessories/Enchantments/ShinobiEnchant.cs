using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class ShinobiEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Shinobi Infiltrator Enchantment");

            string tooltip =
@"Dash into any walls, to teleport through them to the next opening
Allows the ability to dash
Double tap a direction
Greatly enhances Lightning Aura effectiveness
'Village Hidden in the Wall'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(147, 91, 24);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //tele thru wall
            modPlayer.ShinobiEffect(hideVisual);
            //monk dash mayhem
            modPlayer.MonkEffect();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MonkAltHead)
            .AddIngredient(ItemID.MonkAltShirt)
            .AddIngredient(ItemID.MonkAltPants)
            .AddIngredient(null, "MonkEnchant")
            .AddIngredient(ItemID.ChainGuillotines)
            .AddIngredient(ItemID.PsychoKnife)
            //code 2
            //flower pow
            //stynger

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
