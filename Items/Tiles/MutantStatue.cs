using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Tiles
{
    public class MutantStatue : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Statue");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体雕像");
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
            item.createTile = mod.TileType("MutantStatue");
        }
    }
}
