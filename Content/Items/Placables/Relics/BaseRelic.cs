using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Placables.Relics
{
    public abstract class BaseRelic : SoulsItem
    {
        protected abstract int TileType { get; }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.DefaultToPlaceableTile(TileType);

            Item.width = 30;
            Item.height = 40;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.buyPrice(0, 5);
        }
    }
}
