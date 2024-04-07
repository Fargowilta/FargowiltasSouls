using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class EternalEnergy : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eternal Energy");
            /* Tooltip.SetDefault(@"Grants immunity to almost all Eternity Mode debuffs
'Proof of having embraced eternity'"); */
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "施虐狂");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
            //@"'受苦的证明'
            //免疫几乎所有受虐模式的Debuff");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Purple;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Buffs.Masomode.SadismBuff>();
            Item.buffTime = 25200;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.sellPrice(0, 5);
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;//new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
        }
    }
}