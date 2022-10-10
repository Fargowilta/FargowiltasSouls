using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class GalacticGlobe : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Globe");
            Tooltip.SetDefault(@"Grants immunity to Nullification Curse, Flipped, Unstable, and Curse of the Moon
Allows the holder to control gravity
Stabilizes gravity in space and in liquids
Summons the true eyes of Cthulhu to protect you
Increases flight time by 100%
'Always watching'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "银河球");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'时刻注视'
            // 免疫翻转,不稳定,扭曲和月之诅咒
            // 允许使用者改变重力
            // 召唤真·克苏鲁之眼保护你
            // 增加100%飞行时间");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 8);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Flipped>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedHallow>()] = true;
            player.buffImmune[ModContent.BuffType<NullificationCurse>()] = true;
            player.buffImmune[ModContent.BuffType<Unstable>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoon>()] = true;
            //player.buffImmune[BuffID.ChaosState] = true;

            if (player.GetToggleValue("MasoGrav"))
                player.gravControl = true;

            if (player.GetToggleValue("MasoTrueEye"))
                player.AddBuff(ModContent.BuffType<TrueEyes>(), 2);

            player.GetModPlayer<FargoSoulsPlayer>().GravityGlobeEXItem = Item;
            player.GetModPlayer<FargoSoulsPlayer>().WingTimeModifier += 1f;
        }
    }
}