using FargowiltasSouls.Content.Items.Accessories.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.GreatestKraken
{
    public class VortexMagnetRitual : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Vortex Ritual");
            // Tooltip.SetDefault("'Power surges in your hand'");
        }

        public override void SetDefaults()
        {
            Item.damage = 333;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 4f;
            Item.mana = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<VortexRitualProj>();
            Item.shootSpeed = 12f;
            Item.channel = true;

            Item.width = 28;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 12);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item21;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //initial spawn
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VortexRitualProj>()] <= 0)
            {
                Vector2 mouse = Main.MouseWorld;
                Projectile.NewProjectile(source, mouse, Vector2.Zero, ModContent.ProjectileType<VortexRitualProj>(), damage, knockback, player.whoAmI, 0, 300);

                //some funny dust
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.MagnetSphere)
            .AddIngredient(ItemID.FragmentVortex, 35)
            .AddIngredient(ItemID.LunarBar, 5)
            .AddIngredient(ModContent.ItemType<CelestialRune>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerCultist"))

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}
