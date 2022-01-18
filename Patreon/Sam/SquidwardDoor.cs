using FargowiltasSouls.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Sam
{
    public class SquidwardDoor : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("'After you Mr. Squidward'");
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
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }
    }
}