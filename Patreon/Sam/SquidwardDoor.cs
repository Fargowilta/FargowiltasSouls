using FargowiltasSouls.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Sam
{
    public class SquidwardDoor : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'After you Mr. Squidward'");
            Tooltip.AddTranslation(GameCulture.Chinese, "'章鱼哥先生，您先请'");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 28;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 150;
            item.createTile = mod.TileType("SquidwardDoorClosed");
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
        if (Language.ActiveCulture == GameCulture.Chinese)
            {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> 捐赠者物品 <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
            }
            else
            {
            TooltipLine line = new TooltipLine(mod, "tooltip", ">> Patreon Item <<");
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
            }
        }
    }
}
