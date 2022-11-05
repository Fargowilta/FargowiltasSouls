using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PalmWoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Palm Wood Enchantment");
            Tooltip.SetDefault(
@"Double tap down to spawn a palm tree sentry that throws nuts at enemies
'Alarmingly calm'");

            //attack rate and damage increased, you can spawn 2 additional trees
        }

        protected override Color nameColor => new Color(183, 141, 86);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.PalmWood");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PalmEffect(player, Item);
        }

        public static void PalmEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.PalmEnchantItem = item;

            if (player.GetToggleValue("Palm") && player.whoAmI == Main.myPlayer && modPlayer.DoubleTap)
            {
                Vector2 mouse = Main.MouseWorld;

                int maxSpawn = 1;

                if (modPlayer.WoodForce)
                {
                    maxSpawn = 3;
                }


                if (player.ownedProjectileCounts[ModContent.ProjectileType<PalmTreeSentry>()] > maxSpawn - 1)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<PalmTreeSentry>() && proj.owner == player.whoAmI)
                        {
                            proj.Kill();
                            break;
                        }
                    }
                }

                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), mouse - 10 * Vector2.UnitY, Vector2.Zero, ModContent.ProjectileType<PalmTreeSentry>(), modPlayer.WoodForce ? 100 : 15, 0f, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PalmWoodHelmet)
                .AddIngredient(ItemID.PalmWoodBreastplate)
                .AddIngredient(ItemID.PalmWoodGreaves)
                .AddIngredient(ItemID.Coral)
                .AddIngredient(ItemID.Banana)
                .AddIngredient(ItemID.Coconut)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
