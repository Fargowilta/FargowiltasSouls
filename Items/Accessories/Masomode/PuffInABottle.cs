using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class PuffInABottle : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Puff in a Bottle");
            Tooltip.SetDefault(@"Allows the holder to double jump");
            DisplayName.AddTranslation(GameCulture.Chinese, "气体瓶");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你获得二段跳能力");
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
