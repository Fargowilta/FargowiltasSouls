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
While grappling you gain 10 defense and a 50% thorns effect
'Guaranteed to keep you hooked'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "红木魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"所有钩爪的抛出速度、牵引速度和回收速度x1.5
            // '保证钩到你'");
        }

        protected override Color nameColor => new Color(181, 108, 100);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MahoganyEnchantActive = true;
        }

        public static void MahoganyHookAI(Projectile projectile, Player player)
        {
            if (projectile.extraUpdates < 1)
                projectile.extraUpdates = 1;

            if (projectile.ai[0] == 2 && player.velocity != Vector2.Zero) //grappling 
            {
                //this runs twice per frame due to extra update so its actually 2x this
                player.statDefense += 5;
                player.thorns += 0.25f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.RichMahoganyHelmet)
            .AddIngredient(ItemID.RichMahoganyBreastplate)
            .AddIngredient(ItemID.RichMahoganyGreaves)
            .AddIngredient(ItemID.GrapplingHook)
            .AddIngredient(ItemID.Moonglow)
            .AddIngredient(ItemID.Pineapple)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
