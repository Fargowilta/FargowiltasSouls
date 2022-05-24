using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TitaniumEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Titanium Enchantment");
            Tooltip.SetDefault(
@"Attacking generates a defensive barrier of titanium shards
'The power of titanium in the palm of your hand'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "钛金魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            // @"攻击敌人后会使你无敌一小段时间
            // 'Hit me with your best shot'（某歌曲名）");
        }

        protected override Color nameColor => new Color(130, 140, 136);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TitaniumEffect(player, Item);
        }

        public static void TitaniumEffect(Player player, Item item)
        {
            if (player.GetToggleValue("Titanium"))
            {
                player.GetModPlayer<FargoSoulsPlayer>().TitaniumEnchantItem = item;
            }
        }

        public static void TitaniumShards(FargoSoulsPlayer modPlayer, Player player)
        {
            player.AddBuff(306, 600, true, false);
            if (player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] < 25)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.TitaniumEnchantItem), player.Center, Vector2.Zero, 908 /*ModContent.ProjectileType<TitaniumShard>()*/, 50, 15f, player.whoAmI, 0f, 0f);

                //for (int i = 0; i < Main.maxProjectiles; i++)
                //{
                //    Projectile proj = Main.projectile[i];

                //    if (proj.active && proj.type == ProjectileID.TitaniumStormShard)
                //    {
                //        proj.aiStyle = 1;
                //        proj.velocity = (Main.MouseWorld - proj.Center).SafeNormalize(-Vector2.UnitY) * 20f;
                //    }

                //}

            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyTitaHead")
                .AddIngredient(ItemID.TitaniumBreastplate)
                .AddIngredient(ItemID.TitaniumLeggings)
                .AddIngredient(ItemID.Chik)
                .AddIngredient(ItemID.CrystalStorm)
                .AddIngredient(ItemID.CrystalVileShard)

                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
