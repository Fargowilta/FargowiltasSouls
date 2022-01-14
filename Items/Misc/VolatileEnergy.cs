using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class VolatileEnergy : SoulsItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Volatile Energy");
            DisplayName.AddTranslation(GameCulture.Chinese, "不稳定能量");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(0, 0, 3, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(this, 50);
            recipe.AddIngredient(ItemID.SoulofLight, 100);
            recipe.AddIngredient(ItemID.HallowedBar, 5);

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.RodofDiscord);
            recipe.AddRecipe();
        }
    }
}