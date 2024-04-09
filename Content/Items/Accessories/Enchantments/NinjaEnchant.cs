using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class NinjaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Ninja Enchantment");
            /* Tooltip.SetDefault(
@"Drastically increases projectile and attack speed
Reduces damage to compensate for increased speed
Increases armor pen by 15
'Attack faster than the eye can see'"); */
        }

        public override Color nameColor => new(48, 49, 52);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<NinjaEffect>(Item);
        }

        public static void NinjaSpeedSetup(FargoSoulsPlayer modPlayer, Projectile projectile, FargoSoulsGlobalProjectile globalProj)
        {
            Player player = modPlayer.Player;
            float maxSpeedRequired = modPlayer.ForceEffect<NinjaEnchant>() ? 7 : 4; //the highest velocity at which your projectile speed is increased
            if (player.velocity.Length() < maxSpeedRequired)
            {
                const int speedIncrease = 1;
                globalProj.NinjaSpeedup = projectile.extraUpdates + speedIncrease;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient(ItemID.Gi)
                .AddIngredient(ItemID.Shuriken, 100)
                .AddIngredient(ItemID.ThrowingKnife, 100)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class NinjaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<NinjaEnchant>();
    }
}
