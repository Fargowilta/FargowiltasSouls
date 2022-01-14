using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Items.Tiles
{
    public class MutantStatueGift : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Statue (Gift)");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = mod.TileType("MutantStatueGift");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("MutantStatue"));
            recipe.AddIngredient(mod.ItemType("Masochist"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}