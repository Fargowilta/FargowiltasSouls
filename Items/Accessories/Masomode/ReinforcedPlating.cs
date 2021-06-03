using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class ReinforcedPlating : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reinforced Plating");
            Tooltip.SetDefault(@"Grants immunity to Defenseless, Nano Injection, and knockback
Reduces damage taken by 5%
'The sturdiest piece of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "强化钢板");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫毫无防御和纳米注射减益以及击退
减少5%所受伤害
'一位被打败的敌人的坚固碎块'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(0, 4);
            item.defense = 15;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("Defenseless")] = true;
            player.buffImmune[mod.BuffType("NanoInjection")] = true;
            player.endurance += 0.05f;
            player.noKnockback = true;
        }
    }
}
