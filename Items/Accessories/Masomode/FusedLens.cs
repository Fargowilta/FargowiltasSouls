using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class FusedLens : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fused Lens");
            Tooltip.SetDefault(@"Grants immunity to Cursed Inferno and Ichor
Your attacks inflict Cursed Inferno and Ichor
'Too melted to improve vision'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "融合晶状体");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'融化过度,无法改善视力'
            // 免疫诅咒地狱和脓液
            // 攻击造成诅咒地狱和脓液");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.GetModPlayer<FargoSoulsPlayer>().FusedLens = true;
        }
    }
}