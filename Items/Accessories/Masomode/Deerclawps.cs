using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class Deerclawps : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deerclawps");
            Tooltip.SetDefault("Grants immunity to Slow and Frozen" +
                "\nDashing leaves a trail of ice spikes" +
                "\n'The trimmed nails of a defeated foe'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "冰鹿爪");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"免疫缓慢和冰冻
            // 冲刺会留下一串冰刺
            // “从被击败的敌人的脚上剪下来的指甲”");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Slow] = true;
            player.buffImmune[BuffID.Frozen] = true;

            player.GetModPlayer<FargoSoulsPlayer>().DeerclawpsItem = Item;
        }
    }
}
