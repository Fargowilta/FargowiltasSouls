using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Ammos
{
    public class FargoArrow : SoulsItem
    {
        private Mod fargos = ModLoader.GetMod("Fargowiltas");

        public override bool Autoload(ref string name)
        {
            return ModLoader.GetMod("Fargowiltas") != null;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Amalgamated Arrow Quiver");
            Tooltip.SetDefault("Bounces several times\n" +
                "Each impact explodes, summons falling stars, and fires laser arrows\n" +
                "Inflicts several debuffs");
            DisplayName.AddTranslation(GameCulture.Chinese, "混合箭袋");
            Tooltip.AddTranslation(GameCulture.Chinese, "弹跳多次\n" +
                "每次撞击都会爆炸、召唤流星、发射激光箭\n" +
                "造成多种减益");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.ranged = true;
            item.width = 26;
            item.height = 26;
            item.knockBack = 8f; //same as hellfire
            item.rare = ItemRarityID.Red;
            item.shoot = mod.ProjectileType("FargoArrowProj");
            item.shootSpeed = 6.5f; // same as hellfire arrow
            item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            //recipe.AddIngredient(ItemID.EndlessQuiver);
            recipe.AddIngredient(fargos, "FlameQuiver");
            recipe.AddIngredient(fargos, "FrostburnQuiver");
            recipe.AddIngredient(fargos, "UnholyQuiver");
            recipe.AddIngredient(fargos, "BoneQuiver");
            recipe.AddIngredient(fargos, "JesterQuiver");
            recipe.AddIngredient(fargos, "HellfireQuiver");
            recipe.AddIngredient(fargos, "CursedQuiver");
            recipe.AddIngredient(fargos, "IchorQuiver");
            recipe.AddIngredient(fargos, "HolyQuiver");
            recipe.AddIngredient(fargos, "VenomQuiver");
            recipe.AddIngredient(fargos, "ChlorophyteQuiver");
            recipe.AddIngredient(fargos, "LuminiteQuiver");
            recipe.AddIngredient(mod.ItemType("Sadism"), 15);
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
