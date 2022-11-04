using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class RichMahoganyEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Rich Mahogany Enchantment");
            Tooltip.SetDefault(
@"All grappling hooks pull 1.5x as fast, shoot 2x as fast, and retract 3x as fast
While grappling you gain 10% damage resistance for one hit and a 50% thorns effect
'Guaranteed to keep you hooked'");

            //in force multiplier is 2.5x pull speed, DR increases to 50% and thorns to 500%
        }

        protected override Color nameColor => new Color(181, 108, 100);
        public override string wizardEffect => "Hooks pull 2.5x as fast, damage resistance and thorns increased greatly";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MahoganyEnchantItem = Item;
        }

        public static void PostUpdate(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.grapCount > 0)
            {
                modPlayer.Player.thorns += modPlayer.WoodForce ? 5.0f : 0.5f;

                if (modPlayer.MahoganyCanUseDR)
                    modPlayer.Player.endurance += modPlayer.WoodForce ? 0.3f : 0.1f;
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
}
