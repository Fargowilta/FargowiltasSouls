using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Misc
{
    public class BrokenHilt : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Hilt");
            DisplayName.AddTranslation(GameCulture.Chinese, "断柄"); 
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Purple;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = Main.DiscoColor;
                }
            }
        }
    }
}
