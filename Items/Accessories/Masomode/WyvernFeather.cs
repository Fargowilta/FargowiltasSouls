using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
Stabilizes gravity in space and in liquids
'Warm to the touch'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "龙牙");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'触感温暖'
            // 免疫剪除羽翼和残疾
            // 攻击有10%概率对非Boss单位造成剪除羽翼效果");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            player.buffImmune[ModContent.BuffType<Crippled>()] = true;
            if (player.GetToggleValue("MasoClipped"))
                player.GetModPlayer<FargoSoulsPlayer>().DragonFang = true;
			player.GetModPlayer<FargoSoulsPlayer>().StabilizedGravity = true;
        }
    }
}