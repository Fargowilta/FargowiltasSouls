
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class RichMahoganyEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Rich Mahogany Enchantment");
            /* Tooltip.SetDefault(
@"All grappling hooks pull 1.5x as fast, shoot 2x as fast, and retract 3x as fast
While grappling you gain 10% damage resistance for one hit and a 50% thorns effect
'Guaranteed to keep you hooked'"); */

            //in force multiplier is 2.5x pull speed, DR increases to 50% and thorns to 500%
        }

        public override Color nameColor => new(181, 108, 100);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MahoganyEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RichMahoganyHelmet)
            .AddIngredient(ItemID.RichMahoganyBreastplate)
            .AddIngredient(ItemID.RichMahoganyGreaves)
            .AddIngredient(ItemID.Moonglow)
            .AddIngredient(ItemID.Pineapple)
            .AddIngredient(ItemID.GrapplingHook)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class MahoganyEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TimberHeader>();
        public override int ToggleItemType => ModContent.ItemType<RichMahoganyEnchant>();
        
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect<RichMahoganyEnchant>();

            if (player.grapCount > 0)
            {
                player.thorns += forceEffect ? 5.0f : 0.5f;

                if (modPlayer.MahoganyCanUseDR)
                    player.endurance += forceEffect ? 0.3f : 0.1f;
            }
            else //when not grapple, refresh DR
            {
                modPlayer.MahoganyCanUseDR = true;
            }
        }
        public static void MahoganyHookAI(Projectile projectile, FargoSoulsPlayer modPlayer)
        {
            if (projectile.extraUpdates < 1)
                projectile.extraUpdates = 1;
        }
    }
}
