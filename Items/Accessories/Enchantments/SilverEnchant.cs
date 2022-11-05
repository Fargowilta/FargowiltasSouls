using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    [AutoloadEquip(EquipType.Shield)]
    public class SilverEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Silver Enchantment");

            string tooltip =
@"Right Click to guard with your shield
Guard just before being hit to negate damage
'Reflection'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(180, 180, 204);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Silver");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.GetToggleValue("SilverS"))
            {
                //shield
                modPlayer.SilverEnchantItem = Item;
            }

            // modPlayer.AddMinion(Item, player.GetToggleValue("Silver"), ModContent.ProjectileType<SilverSword>(), 20, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.SilverHelmet)
            .AddIngredient(ItemID.SilverChainmail)
            .AddIngredient(ItemID.SilverGreaves)
            .AddIngredient(ItemID.EmptyBucket)
            .AddIngredient(ItemID.SilverBroadsword)
            .AddIngredient(ItemID.BlandWhip)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
