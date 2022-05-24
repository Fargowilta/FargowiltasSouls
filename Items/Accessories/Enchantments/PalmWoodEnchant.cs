using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "棕榈木魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"双击'下'键会召唤一个会向敌人扔橡子的棕榈树哨兵
            // '出奇的宁静'");
        }

        protected override Color nameColor => new Color(183, 141, 86);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PalmEffect(player);
        }

        public static void PalmEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.PalmEnchantActive = true;

            if (player.GetToggleValue("Palm") && player.whoAmI == Main.myPlayer && modPlayer.DoubleTap)
            {
                Vector2 mouse = Main.MouseWorld;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<PalmTreeSentry>()] > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];

                        if (proj.type == ModContent.ProjectileType<PalmTreeSentry>() && proj.owner == player.whoAmI)
                            proj.Kill();
                    }
                }

                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), mouse - 10 * Vector2.UnitY, Vector2.Zero, ModContent.ProjectileType<PalmTreeSentry>(), modPlayer.WoodForce ? 45 : 15, 0f, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PalmWoodHelmet)
                .AddIngredient(ItemID.PalmWoodBreastplate)
                .AddIngredient(ItemID.PalmWoodGreaves)
                .AddIngredient(ItemID.BreathingReed)
                .AddIngredient(ItemID.Coconut)
                .AddIngredient(ItemID.Seagull)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
