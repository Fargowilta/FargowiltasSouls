using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class LeadEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Lead Enchantment");

            string tooltip =
@"You take 40% less from damage over time
Attacks inflict enemies with Lead Poisoning
Lead Poisoning deals damage over time and spreads to nearby enemies
'Not recommended for eating'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(67, 69, 88);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Lead");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            LeadEffect(player, Item);
        }

        public static void LeadEffect(Player player, Item item)
        {
            player.GetModPlayer<FargoSoulsPlayer>().LeadEnchantItem = item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LeadHelmet)
                .AddIngredient(ItemID.LeadChainmail)
                .AddIngredient(ItemID.LeadGreaves)
                .AddIngredient(ItemID.LeadShortsword)
                .AddIngredient(ItemID.GrayPaint, 100)
                .AddIngredient(ItemID.Peach)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
