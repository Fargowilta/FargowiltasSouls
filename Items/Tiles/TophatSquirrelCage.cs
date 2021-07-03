using FargowiltasSouls.Items.Misc;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Tiles
{
    public class TophatSquirrelCage : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Top Hat Squirrel Cage");
            DisplayName.AddTranslation(GameCulture.Chinese, "高顶礼帽松鼠笼"); 
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.SquirrelCage);
            item.createTile = TileType<TophatSquirrelCageSheet>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Terrarium);
            recipe.AddIngredient(ItemType<TopHatSquirrelCaught>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
