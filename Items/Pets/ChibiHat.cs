using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Pets
{
    public class ChibiHat : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chibi Hat");
            Tooltip.SetDefault("Summons Chibi Devi\nShe follows your mouse\n'Cute! Cute! Cute!'");
            DisplayName.AddTranslation(GameCulture.Chinese, "小德维安帽");
            Tooltip.AddTranslation(GameCulture.Chinese, "召唤小德维安帽\n她会跟着你的嘴巴\n'你给我翻译翻译,什么tmd叫可爱!'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.value = Item.sellPrice(0, 5);
            item.rare = -13;
            item.shoot = mod.ProjectileType("ChibiDevi");
            item.buffType = mod.BuffType("ChibiDeviBuff");
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

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
