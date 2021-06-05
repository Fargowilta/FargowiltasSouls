using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face)]
    public class WyvernFeather : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wyvern Feather");
            Tooltip.SetDefault(@"Grants immunity to Clipped Wings and Crippled
Your attacks have a 10% chance to inflict Clipped Wings on non-boss enemies
'Warm to the touch'");
            DisplayName.AddTranslation(GameCulture.Chinese, "飞龙之羽");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫剪除羽翼和残废减益
攻击有10%几率对非Boss敌人造成剪除羽翼减益
'摸起来很温暖'");
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
            player.buffImmune[mod.BuffType("ClippedWings")] = true;
            player.buffImmune[mod.BuffType("Crippled")] = true;
            if (player.GetToggleValue("MasoClipped"))
                player.GetModPlayer<FargoPlayer>().DragonFang = true;
        }
    }
}
