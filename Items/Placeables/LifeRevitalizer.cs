using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Placeables
{
    public class LifeRevitalizer : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Revitalizer");
            Tooltip.SetDefault("Right click tile to set world spawn point \nOverridden by beds \nNo housing required");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 3));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.maxStack = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Expert;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.LifeRevitalizerPlaced>();
        }
    }
}