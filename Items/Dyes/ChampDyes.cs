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
            DisplayName.AddTranslation(GameCulture.Chinese, "神圣染料");
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
            DisplayName.AddTranslation(GameCulture.Chinese, "坚毅染料");
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
            DisplayName.AddTranslation(GameCulture.Chinese, "盖亚染料");
        }

        public override void SetDefaults()
        {
            item.maxStack = 99;
            item.rare = ItemRarityID.Orange;
        }
    }
}
