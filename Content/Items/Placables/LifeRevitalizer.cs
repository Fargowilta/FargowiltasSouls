using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class LifeRevitalizer : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life Revitalizer");
            // Tooltip.SetDefault("Right click tile to set your spawn point\nNo housing required");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            //Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 3));
            //ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 15;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Expert;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.LifeRevitalizerPlaced>();

            Item.expert = true;
        }
    }
}