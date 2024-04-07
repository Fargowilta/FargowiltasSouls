using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables
{
    public class ScremPainting : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Screm Painting");
            // Tooltip.SetDefault("'Merry N. Tuse'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "尖叫猫猫");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "Merry N. Tuse");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Purple;
            Item.createTile = ModContent.TileType<ScremPaintingSheet>();
        }
    }
}