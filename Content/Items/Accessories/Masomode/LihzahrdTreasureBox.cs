using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class LihzahrdTreasureBox : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lihzahrd Treasure Box");
            /* Tooltip.SetDefault(@"Grants immunity to Burning, Fused, and Low Ground
Double tap DOWN in the air to fastfall
Fastfall will create a fiery eruption on impact after falling a certain distance
'Too many booby traps to open'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神庙蜥蜴宝藏盒");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'陷阱太多,打不开'
            // 免疫燃烧，导火线和低地
            // 受伤时爆发尖钉球
            // 在空中按'下'键快速下落
            // 在一定高度使用快速下落,会在撞击地面时产生猛烈的火焰喷发");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[ModContent.BuffType<FusedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LihzahrdCurseBuff>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;
            player.FargoSouls().LihzahrdTreasureBoxItem = Item;
            player.AddEffect<LihzahrdGroundPound>(Item);
            player.AddEffect<LihzahrdBoulders>(Item);
        }
    }
    public class LihzahrdGroundPound : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
    }
    public class LihzahrdBoulders : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<LihzahrdTreasureBox>();
        public override bool ExtraAttackEffect => true;
    }
}