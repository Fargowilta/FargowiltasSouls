using Terraria;
using Terraria.ID;
using Terraria.Localization;

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
            DisplayName.AddTranslation(GameCulture.Chinese, "融合晶状体");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫诅咒地狱和脓液
攻击造成诅咒地狱和脓液
'融化过度,无法改善视力'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.GetModPlayer<FargoPlayer>().FusedLens = true;
        }
    }
}
