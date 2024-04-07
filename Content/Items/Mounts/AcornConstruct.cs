using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Mounts
{
    public class AcornConstruct : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Ride the Squirrel");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 30000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item79;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<TrojanSquirrelMount>();
        }
    }
}