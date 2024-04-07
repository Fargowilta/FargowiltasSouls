using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class TheLightningRod : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("The Lightning Rod");
            // Tooltip.SetDefault("Charges power as it is spun\nDamage decreases per hit when thrown");
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.shootSpeed = 1f;
            Item.knockBack = 6f;
            Item.width = 68;
            Item.height = 68;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<TheLightningRodProj>();
            Item.value = Item.sellPrice(0, 2);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.Diamond)
            .AddIngredient(ItemID.Topaz, 2)
            .AddIngredient(ItemID.DemoniteBar, 7)
            .AddIngredient(ItemID.ShadowScale, 7)
            .AddTile(TileID.Anvils)
            .Register();

            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.Diamond)
            .AddIngredient(ItemID.Topaz, 2)
            .AddIngredient(ItemID.CrimtaneBar, 7)
            .AddIngredient(ItemID.TissueSample, 7)
            .AddTile(TileID.Anvils)
            .Register();
        }
        /*
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<TreasureGuardianBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
        }
        */
    }
}