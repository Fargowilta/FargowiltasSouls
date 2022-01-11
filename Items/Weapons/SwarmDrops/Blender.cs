using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class Blender : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Blender");
            //Tooltip.SetDefault("'The reward for slaughtering many...'");

            //DisplayName.AddTranslation(GameCulture.Chinese, "绞肉机");
            //Tooltip.AddTranslation(GameCulture.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.width = 24;
            item.height = 24;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.channel = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<BlenderYoyoProj>();
            item.useAnimation = 25;
            item.useTime = 25;
            item.shootSpeed = 16f;
            item.knockBack = 2.5f;
            item.damage = 377;

            item.value = Item.sellPrice(0, 25);
            item.rare = ItemRarityID.Purple;
        }

        public override void HoldItem(Player player)
        {
            //player.counterWeight = 556 + Main.rand.Next(6);
            player.stringColor = 5;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Dicer");
            recipe.AddIngredient(null, "AbomEnergy", 10);
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerPlant"));
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}