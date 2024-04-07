using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class GalacticGlobe : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Galactic Globe");
            /* Tooltip.SetDefault(@"Grants immunity to Nullification Curse, Flipped, Unstable, and Curse of the Moon
Allows the holder to control gravity
Summons the true eyes of Cthulhu to protect you
Increases flight time by 100%
'Always watching'"); */
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
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 8);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<FlippedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedHallowBuff>()] = true;
            player.buffImmune[ModContent.BuffType<NullificationCurseBuff>()] = true;
            player.buffImmune[ModContent.BuffType<UnstableBuff>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoonBuff>()] = true;
            //player.buffImmune[BuffID.ChaosState] = true;

            player.AddEffect<MasoGravEffect>(Item);
            player.AddEffect<MasoTrueEyeMinion>(Item);


            player.FargoSouls().GravityGlobeEXItem = Item;
            player.FargoSouls().WingTimeModifier += 1f;
        }
    }
    public class MasoGravEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<GalacticGlobe>();
        public override void PostUpdateEquips(Player player)
        {
            player.gravControl = true;
        }
    }
    public class MasoTrueEyeMinion : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<GalacticGlobe>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<TrueEyesBuff>(), 2);
        }
    }
}