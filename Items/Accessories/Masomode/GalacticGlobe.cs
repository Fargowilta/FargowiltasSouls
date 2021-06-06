using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class GalacticGlobe : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Globe");
            Tooltip.SetDefault(@"Grants immunity to Flipped, Unstable, Distorted, and Curse of the Moon
Allows the holder to control gravity
Stabilizes gravity in space and in liquids
Summons the true eyes of Cthulhu to protect you
Increases flight time by 100%
'Always watching'");
            DisplayName.AddTranslation(GameCulture.Chinese, "银河球");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫翻转、不稳定、扭曲和月之诅咒减益
允许你控制重力
免疫太空的低重力环境并无视水的阻力
召唤真·克苏鲁之眼来保护你
延长100%飞行时间
'时刻注视着'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Red;
            item.value = Item.sellPrice(0, 8);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[mod.BuffType("Flipped")] = true;
            player.buffImmune[mod.BuffType("FlippedHallow")] = true;
            player.buffImmune[mod.BuffType("Unstable")] = true;
            player.buffImmune[mod.BuffType("CurseoftheMoon")] = true;
            player.buffImmune[BuffID.VortexDebuff] = true;
            //player.buffImmune[BuffID.ChaosState] = true;

            if (player.GetToggleValue("MasoGrav"))
                player.gravControl = true;

            if (player.GetToggleValue("MasoTrueEye"))
                player.AddBuff(mod.BuffType("TrueEyes"), 2);

            player.GetModPlayer<FargoPlayer>().GravityGlobeEX = true;
            player.GetModPlayer<FargoPlayer>().wingTimeModifier += 1f;
        }
    }
}
