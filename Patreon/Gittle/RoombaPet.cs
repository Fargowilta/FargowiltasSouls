using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.Gittle
{
    public class RoombaPet : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Roomba");
            Tooltip.SetDefault("Summons a Roomba to follow you around in hopes of cleaning the whole world");
            DisplayName.AddTranslation(GameCulture.Chinese, "扫地机器人");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤一个跟着你的扫地机器人，它希望清洁整个世界");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = mod.ProjectileType("RoombaPetProj");
            item.buffType = mod.BuffType("RoombaPetBuff");
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

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
