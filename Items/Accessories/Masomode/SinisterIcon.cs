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
            /*擦弹能获得最多30%的暴击伤害提升
            提升的伤害会随时间流逝而减少并会在被击中后清零");*/
            DisplayName.AddTranslation(GameCulture.Chinese, "邪恶画像");
            Tooltip.AddTranslation(GameCulture.Chinese, @"阻止受虐模式导致的Boss自然生成
提高刷怪速率
小于等于2000血量的敌人掉落双倍物品，但不掉落钱币
'肯定不是活着的'");
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
