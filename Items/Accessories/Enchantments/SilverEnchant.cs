using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
//using FargowiltasSouls.Projectiles.Minions;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class SilverEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Silver Enchantment");

            string tooltip =
@"Summons a sword familiar that scales with minion damage
Drastically increases minion speed
Reduces minion damage to compensate for increased speed
'Have you power enough to wield me?'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(180, 180, 204);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.SilverEnchantActive = true;
            modPlayer.AddMinion(Item, player.GetToggleValue("Silver"), ModContent.ProjectileType<SilverSword>(), 20, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.SilverHelmet)
            .AddIngredient(ItemID.SilverChainmail)
            .AddIngredient(ItemID.SilverGreaves)
            .AddIngredient(ItemID.SilverBroadsword)
            .AddIngredient(ItemID.SapphireStaff)
            .AddIngredient(ItemID.BlandWhip)
            //roasted duck

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
