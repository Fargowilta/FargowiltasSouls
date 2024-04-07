using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class MutantsDiscountCard : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, BuffID.WellFed3, 60 * 60 * 8); // Yes, this is intentional.
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.FargoSouls().MutantsDiscountCard;
        }

        public override bool? UseItem(Player player)
        {
            player.FargoSouls().MutantsDiscountCard = true;
            return true;
        }
    }
}