using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Expert
{
    public class BoxofGizmos : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Box of Gizmos");
            Tooltip.SetDefault(@"Grants autofire to all items
Reduces use speed of affected items by 20%");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 1);

            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().BoxofGizmos = true;
        }
    }
}