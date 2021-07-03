using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Mounts
{
    public class AcornConstruct : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Ride the Squirrel");
            Tooltip.AddTranslation(GameCulture.Chinese, "骑上松鼠");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = 30000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item79;
            item.noMelee = true;
            item.mountType = ModContent.MountType<TrojanSquirrelMount>();
        }
    }
}
