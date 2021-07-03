using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SparklingAdoration : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Adoration");
            Tooltip.SetDefault(@"Grants immunity to Lovestruck and Fake Hearts
Graze projectiles to gain up to 25% increased critical damage
Critical damage bonus decreases over time and is fully lost on hit
Your attacks periodically summon life-draining hearts
'With all of your emotion!'");
Tooltip.AddTranslation(GameCulture.Chinese, @"免疫热恋和假心
擦弹能获得最多25%的暴击伤害加成
暴击伤害加成会随时间流逝而减少，被击中时清零
你的攻击会周期性地生成假心
'带着你所有的感情！'");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 11));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[mod.BuffType("Lovestruck")] = true;

            if (player.GetToggleValue("MasoGraze", false))
                player.GetModPlayer<FargoPlayer>().Graze = true;

            if (player.GetToggleValue("MasoDevianttHearts"))
                player.GetModPlayer<FargoPlayer>().DevianttHearts = true;
        }
    }
}
