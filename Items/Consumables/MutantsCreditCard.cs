using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Items.Consumables
{
    public class MutantsCreditCard : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Credit Card");
            Tooltip.SetDefault("Permanently reduces Mutant's shop prices by 30%\n" +
                "'Wait, how did you get this?'");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 10);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<FargoSoulsPlayer>().MutantsCreditCard;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoSoulsPlayer>().MutantsCreditCard = true;
                SoundEngine.PlaySound(SoundID.Roar, player.Center);
            }
            return true;
        }
    }
}