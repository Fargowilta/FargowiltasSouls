using Terraria;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SinisterIcon : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sinister Icon");
            Tooltip.SetDefault(@"Prevents Eternity Mode-induced natural boss spawns
Increases spawn rate
Enemies with 2000 or less max life will drop doubled loot but zero coins
'Most definitely not alive'");
            /*Graze projectiles to gain up to 30% increased crit damage
            Crit damage bonus decreases over time and is fully lost on hit");*/
            DisplayName.AddTranslation(GameCulture.Chinese, "邪秽魔颅");
            Tooltip.AddTranslation(GameCulture.Chinese, @"永恒模式下的Boss不再自然生成
增加刷怪率
最大生命值小于2000的敌人死亡后掉落的战利品翻倍，但不会掉落钱币
'绝对不是活的'");
            /*擦弹会增加暴击伤害，至多增加30%暴击伤害
            暴击伤害会随时间流逝而降低，你被击中时失去全部暴击伤害加成");*/
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
            if (player.GetToggleValue("MasoIcon"))
                player.GetModPlayer<FargoPlayer>().SinisterIcon = true;

            if (player.GetToggleValue("MasoIconDrops"))
                player.GetModPlayer<FargoPlayer>().SinisterIconDrops = true;

            //player.GetModPlayer<FargoPlayer>().Graze = true;
        }
    }
}
