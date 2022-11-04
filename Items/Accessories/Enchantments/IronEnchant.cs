using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class IronEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Iron Enchantment");

            string tooltip =
@"You attract items from a larger range
33% chance to not consume crafting ingredients at an Iron Anvil
'Strike while the iron is hot'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(152, 142, 131);
        public override string wizardEffect => "Increases item range by 4x, crafting bonus now affects Mythril Anvil";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 40000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.IronEnchantItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronHelmet)
            .AddIngredient(ItemID.IronChainmail)
            .AddIngredient(ItemID.IronGreaves)
            .AddIngredient(ItemID.IronHammer)
            .AddIngredient(ItemID.IronAnvil)
            .AddIngredient(ItemID.Apricot) //(high in iron pog)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
