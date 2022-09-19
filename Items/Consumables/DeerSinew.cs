using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Consumables
{
    public class DeerSinew : SoulsItem
    {

        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deer Sinew");
            Tooltip.SetDefault(
@"Allows the ability to dash
Double tap a direction
Taking damage may briefly freeze you
Reduces damage when your speed increases, down to 0.85x at max
All effects negated if toggled off or another dash is already in use
'Cold but twitching'");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;
            Item.value = Item.sellPrice(0, 3);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<FargoSoulsPlayer>().DeerSinew;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoSoulsPlayer>().DeerSinew = true;
            }
            return true;
        }
    }
}
