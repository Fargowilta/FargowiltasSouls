using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ValhallaKnightEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(147, 101, 30);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Yellow;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().ValhallaEnchantActive = true;
            player.AddEffect<ValhallaDash>(Item);
            SquireEnchant.SquireEffect(player, Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireAltHead)
            .AddIngredient(ItemID.SquireAltShirt)
            .AddIngredient(ItemID.SquireAltPants)
            .AddIngredient(ItemID.VikingHelmet)
            .AddIngredient(null, "SquireEnchant")
            .AddIngredient(ItemID.ShadowJoustingLance)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class ValhallaDash : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<ValhallaKnightEnchant>();
    }
}
