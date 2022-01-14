using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Tiles
{
    public class GoldenDippingVat : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golden Dipping Vat");
            Tooltip.SetDefault("Used to craft Gold Critters");
            DisplayName.AddTranslation(GameCulture.Chinese, "黄金浸渍缸");
            Tooltip.AddTranslation(GameCulture.Chinese, "用来制作黄金动物");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 10);
            item.useAnimation = 15;
            item.useTime = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = mod.TileType("GoldenDippingVatSheet");
        }

        public override void AddRecipes()
        {
            AddCritter(ItemID.Bird, ItemID.GoldBird);
            AddCritter(ItemID.Bunny, ItemID.GoldBunny);
            AddCritter(ItemID.Frog, ItemID.GoldFrog);
            AddCritter(ItemID.Grasshopper, ItemID.GoldGrasshopper);
            AddCritter(ItemID.Mouse, ItemID.GoldMouse);
            //AddCritter(ItemID.Squirrel, ItemID.SquirrelGold);
            AddCritter(ItemID.Worm, ItemID.GoldWorm);

            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyButterfly");
            recipe.AddIngredient(ItemID.GoldDust, 100);
            recipe.AddTile(mod.TileType("GoldenDippingVatSheet"));
            recipe.SetResult(ItemID.GoldButterfly);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnySquirrel");
            recipe.AddIngredient(ItemID.GoldDust, 100);
            recipe.AddTile(mod.TileType("GoldenDippingVatSheet"));
            recipe.SetResult(ItemID.SquirrelGold);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyCommonFish");
            recipe.AddIngredient(ItemID.GoldDust, 100);
            recipe.AddTile(mod.TileType("GoldenDippingVatSheet"));
            recipe.SetResult(ItemID.GoldenCarp);
            recipe.AddRecipe();
        }

        private void AddCritter(int critterID, int goldCritterID)
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(critterID);
            recipe.AddIngredient(ItemID.GoldDust, 100);
            recipe.AddTile(mod.TileType("GoldenDippingVatSheet"));
            recipe.SetResult(goldCritterID);
            recipe.AddRecipe();
        }
    }
}