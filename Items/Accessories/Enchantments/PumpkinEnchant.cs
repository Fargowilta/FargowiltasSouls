using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class PumpkinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Pumpkin Enchantment");
            Tooltip.SetDefault(
@"You will grow pumpkins while walking on the ground
When fully grown, they will heal 25 HP and spawn damaging embers
Enemies that touch them will destroy them and take damage
'Your sudden pumpkin craving will never be satisfied'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "南瓜魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"你在地上行走时会种下南瓜
            // 南瓜成熟时会为你回复25点生命值并产生伤害性余烬
            // 敌人与南瓜接触时会摧毁南瓜但会受到伤害
            // '你对南瓜的突发渴望永远不会得到满足'");
        }

        protected override Color nameColor => new Color(227, 101, 28);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 20000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PumpkinEffect(player, Item);
        }

        public static void PumpkinEffect(Player player, Item accessorySource)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (player.GetToggleValue("Pumpkin") && (player.controlLeft || player.controlRight) && !modPlayer.IsStandingStill && player.whoAmI == Main.myPlayer)
            {
                if (modPlayer.PumpkinSpawnCD <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<GrowingPumpkin>()] < 10)
                {
                    int x = (int)player.Center.X / 16;
                    int y = (int)(player.position.Y + player.height - 1f) / 16;

                    //if (Main.tile[x, y] == null)
                    //{
                    //    Main.tile[x, y] = new Tile();
                    //}

                    if ((!Main.tile[x, y].HasTile && Main.tile[x, y].LiquidType == 0 && Main.tile[x, y + 1] != null && (WorldGen.SolidTile(x, y + 1) || Main.tile[x, y + 1].TileType == TileID.Platforms))
                        || modPlayer.LifeForce)
                    {
                        Projectile.NewProjectile(player.GetSource_Accessory(accessorySource), player.Center, Vector2.Zero, ModContent.ProjectileType<GrowingPumpkin>(), 0, 0, player.whoAmI);
                        modPlayer.PumpkinSpawnCD = 300;
                    }
                }
            }

            if (modPlayer.PumpkinSpawnCD > 0)
            {
                modPlayer.PumpkinSpawnCD--;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PumpkinHelmet)
                .AddIngredient(ItemID.PumpkinBreastplate)
                .AddIngredient(ItemID.PumpkinLeggings)
                .AddIngredient(ItemID.MolotovCocktail, 50)
                .AddIngredient(ItemID.Sickle)
                .AddIngredient(ItemID.PumpkinPie)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
