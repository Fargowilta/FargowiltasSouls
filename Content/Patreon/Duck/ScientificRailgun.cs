using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Duck
{
    public class ScientificRailgun : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Scientific Railgun");
            /* Tooltip.SetDefault(
@"Uses coins for ammo
Higher valued coins do more damage
'Particular and specific'"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 2160;
            Item.crit = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.width = 64;
            Item.height = 26;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 20;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            //Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RailgunBlast>();
            Item.shootSpeed = 1000f;
            Item.useAmmo = AmmoID.Coin;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.SafeNormalize(Vector2.Zero);
            type = Item.shoot;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CoinGun)
                .AddIngredient(ItemID.ChargedBlasterCannon)
                .AddIngredient(ItemID.LastPrism)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ItemID.MartianConduitPlating, 100)
                .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
                .Register();
        }
    }
}
