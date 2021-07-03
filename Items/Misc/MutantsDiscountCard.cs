using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Misc
{
    public class MutantsDiscountCard : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant's Discount Card");
            Tooltip.SetDefault(@"Permanently reduces Mutant's shop prices by 20%");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体折扣卡");
            Tooltip.AddTranslation(GameCulture.Chinese, @"永久降低突变体的商店价格20%");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightRed;
            item.maxStack = 1;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 17;
            item.useTime = 17;
            item.consumable = true;
            item.UseSound = SoundID.Item2;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<FargoPlayer>().MutantsDiscountCard;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.GetModPlayer<FargoPlayer>().MutantsDiscountCard = true;
            }
            return true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }
    }
}
