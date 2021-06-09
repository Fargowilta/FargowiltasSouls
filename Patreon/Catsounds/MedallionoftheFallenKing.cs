using FargowiltasSouls.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Catsounds
{
    public class MedallionoftheFallenKing : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medallion of the Fallen King");
            Tooltip.SetDefault(
@"Spawns a King Slime Minion that scales with summon damage");
            DisplayName.AddTranslation(GameCulture.Chinese, "堕落国王的勋章");
            Tooltip..AddTranslation(GameCulture.Chinese, 
@"召唤一只史莱姆王仆从，史莱姆王仆从的大小取决于你的召唤伤害");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = 1;
            item.value = 50000;
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(ModContent.BuffType<KingSlimeMinionBuff>(), 2);
        }
    }
}
