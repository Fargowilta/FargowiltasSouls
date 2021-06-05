using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Dyes
{
    public class LifeDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenly Dye");
            DisplayName.AddTranslation(GameCulture.Chinese, "圣洁染料");
        }

        public override void SetDefaults()
        {
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
        }
    }

    public class WillDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Willpower Dye");
            DisplayName.AddTranslation(GameCulture.Chinese, "意志染料");
        }

        public override void SetDefaults()
        {
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
        }
    }

    public class GaiaDye : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Items/Dyes/LifeDye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Dye");
            DisplayName.AddTranslation(GameCulture.Chinese, "大地染料");
        }

        public override void SetDefaults()
        {
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
        }
    }
}
