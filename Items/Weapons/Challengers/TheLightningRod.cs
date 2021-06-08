using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Utilities;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class TheLightningRod : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Lightning Rod");
            Tooltip.SetDefault("Charges power as it is spun\nDamage decreases per hit when thrown");
            DisplayName.AddTranslation(GameCulture.Chinese, "引雷杖");
            Tooltip.AddTranslation(GameCulture.Chinese, "旋转时进行充能\n扔出后每次击中敌人时都会降低伤害");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 30;
            item.useTime = 30;
            item.shootSpeed = 1f;
            item.knockBack = 6f;
            item.width = 68;
            item.height = 68;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.shoot = mod.ProjectileType("TheLightningRodProj");
            item.value = Item.sellPrice(0, 2);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTurn = false;
            item.melee = true;
            item.autoReuse = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            return player.ownedProjectileCounts[item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            recipe.AddIngredient(ItemID.Diamond);
            recipe.AddIngredient(ItemID.Topaz, 2);
            recipe.AddIngredient(ItemID.DemoniteBar, 7);
            recipe.AddIngredient(ItemID.ShadowScale, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            recipe.AddIngredient(ItemID.Diamond);
            recipe.AddIngredient(ItemID.Topaz, 2);
            recipe.AddIngredient(ItemID.CrimtaneBar, 7);
            recipe.AddIngredient(ItemID.TissueSample, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
