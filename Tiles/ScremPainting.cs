using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Tiles
{
    public class ScremPainting : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Screm Painting");
            Tooltip.SetDefault("'Merry N. Tuse'");
            DisplayName.AddTranslation(GameCulture.Chinese, "尖叫猫猫");
            Tooltip.AddTranslation(GameCulture.Chinese, "Merry N. Tuse");
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.rare = ItemRarityID.Purple;
            item.createTile = mod.TileType("ScremPaintingSheet");
        }
    }
}