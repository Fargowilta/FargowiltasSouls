using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Tiles
{
    public class TophatSquirrelBanner : SoulsItem
    {
        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 36;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.buyPrice(0, 0, 10, 0);
            item.createTile = mod.TileType("FMMBanner");
            item.placeStyle = 0;
        }
    }
}