using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class ConcentratedRainbowMatter : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Concentrated Rainbow Matter");
            Tooltip.SetDefault(@"Grants immunity to Flames of the Universe
Summons a baby rainbow slime to fight for you
'Taste the rainbow'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "浓缩彩虹物质");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'品尝彩虹'
            // 免疫宇宙之火
            // 召唤一个彩虹史莱姆宝宝");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.FlamesoftheUniverse>()] = true;
            if (player.GetToggleValue("MasoRainbow"))
                player.AddBuff(ModContent.BuffType<Buffs.Minions.RainbowSlime>(), 2);
        }
    }
}