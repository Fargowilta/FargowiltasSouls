using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class TimsConcoction : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Tim's Concoction");
            Tooltip.SetDefault(@"Certain enemies will drop potions when defeated
'Smells funny'");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("MasoConcoction"))
                player.GetModPlayer<FargoPlayer>().TimsConcoction = true;
        }
    }
}