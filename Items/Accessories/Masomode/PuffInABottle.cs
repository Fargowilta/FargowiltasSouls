using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class PuffInABottle : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Puff in a Bottle");
            Tooltip.SetDefault(@"Allows the holder to double jump");*/
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.CloudinaBottle);
            item.value = (int)(item.value * 0.75);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.doubleJumpCloud = true;
        }
    }
}