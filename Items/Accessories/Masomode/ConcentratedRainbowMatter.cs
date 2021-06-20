using Terraria;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Toggler;

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
            DisplayName.AddTranslation(GameCulture.Chinese, "浓缩彩虹物质");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫宇宙之火
召唤一个彩虹史莱姆宝宝
'品尝彩虹'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("FlamesoftheUniverse")] = true;
            if (player.GetToggleValue("MasoRainbow"))
                player.AddBuff(mod.BuffType("RainbowSlime"), 2);
        }
    }
}
