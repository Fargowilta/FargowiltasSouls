using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Ammos
{
    public class AmalgamatedDartBox : SoulsItem
    {
        private Mod fargos = ModLoader.GetMod("Fargowiltas");

        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amalgamated Dart Box");
            Tooltip.SetDefault("");
            DisplayName.AddTranslation(GameCulture.Chinese, "混合飞镖袋");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.ranged = true;
            item.width = 26;
            item.height = 26;
            item.knockBack = 3.5f;
            item.rare = ItemRarityID.Red;
            item.shoot = mod.ProjectileType("AmalgamatedDart");
            item.shootSpeed = 3f;
            item.ammo = AmmoID.Dart;
            //item.UseSound = SoundID.Item63;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(fargos, "PoisonDartBox");
            recipe.AddIngredient(fargos, "CursedDartBox");
            recipe.AddIngredient(fargos, "IchorDartBox");
            recipe.AddIngredient(fargos, "CrystalDartBox");

            recipe.AddIngredient(mod.ItemType("Sadism"), 15);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
