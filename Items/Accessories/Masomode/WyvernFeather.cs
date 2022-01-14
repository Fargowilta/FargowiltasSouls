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
            /*DisplayName.SetDefault("Wyvern Feather");
            Tooltip.SetDefault(@"Grants immunity to Clipped Wings and Crippled
Your attacks have a 10% chance to inflict Clipped Wings on non-boss enemies
'Warm to the touch'");
            DisplayName.AddTranslation(GameCulture.Chinese, "龙牙");
            Tooltip.AddTranslation(GameCulture.Chinese, @"'触感温暖'
免疫剪除羽翼和残疾
攻击有10%概率对非Boss单位造成剪除羽翼效果");*/
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