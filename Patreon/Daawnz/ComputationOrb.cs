using Terraria;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Daawnz
{
    public class ComputationOrb : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Computation Orb");
            Tooltip.SetDefault(
@"Non-magic/summon attacks deal 25% extra damage but are affected by Mana Sickness
Non-magic/summon weapons require 10 mana to use
'Within the core, a spark of hope remains.'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "演算宝珠");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"非魔法攻击将额外造成25%伤害, 并消耗10法力");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = 8;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
            modPlayer.CompOrb = true;
        }
    }
}