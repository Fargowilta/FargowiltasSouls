using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Waist)]
    public class WretchedPouch : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wretched Pouch");
            Tooltip.SetDefault(
@"Grants immunity to Shadowflame
You erupt into Shadowflame tentacles when injured
'The accursed incendiary powder of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "诅咒袋子");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"使你免疫暗影焰减益
你在受到伤害时会释放暗影焰触手
'一位被打败的敌人的诅咒火药袋'");
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
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[mod.BuffType("Shadowflame")] = true;
            player.GetModPlayer<FargoPlayer>().WretchedPouch = true;
        }
    }
}
