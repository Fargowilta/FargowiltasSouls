using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.Shucks
{
    public class CrimetroidEgg : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crimetroid Egg");
            //Tooltip.SetDefault("Summons the Baby\nNot to be confused with criminal androids");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<Crimetroid>();
            item.buffType = ModContent.BuffType<CrimetroidBuff>();
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}