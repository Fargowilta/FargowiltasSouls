using Terraria;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class NecromanticBrew : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necromantic Brew");
            Tooltip.SetDefault(@"Grants immunity to Lethargic
Summons 2 Skeletron arms to whack enemies
'The bone-growing solution of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "死灵密酿");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫昏昏欲睡减益
召唤两条骷髅王手臂重击敌人
'一位被打败的敌人的促进骨生长的溶液'");
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
            player.buffImmune[mod.BuffType("Lethargic")] = true;
            player.GetModPlayer<FargoPlayer>().NecromanticBrew = true;
            if (player.GetToggleValue("MasoSkele"))
                player.AddBuff(mod.BuffType("SkeletronArms"), 2);
        }
    }
}
