using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class MutantsCreditCard : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.UseSound = SoundID.Roar;
            Item.value = Item.sellPrice(0, 10);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.FargoSouls().MutantsCreditCard;
        }

        public override bool? UseItem(Player player)
        {
            player.FargoSouls().MutantsCreditCard = true;
            return true;
        }
    }
}