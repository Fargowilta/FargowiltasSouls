using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Misc
{
    public class Sadism : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternal Energy");
            Tooltip.SetDefault(@"Grants immunity to almost all Eternity Mode debuffs
'Proof of having embraced eternity'");
            DisplayName.AddTranslation(GameCulture.Chinese, "施虐狂");
            Tooltip.AddTranslation(GameCulture.Chinese,
@"免疫几乎所有受虐模式的Debuff
'受苦的证明'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 30;
            item.rare = ItemRarityID.Purple;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
            item.buffType = mod.BuffType("Sadism");
            item.buffTime = 25200;
            item.UseSound = SoundID.Item3;
            item.value = Item.sellPrice(0, 5);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = Main.DiscoColor;//new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
        }
    }
}
