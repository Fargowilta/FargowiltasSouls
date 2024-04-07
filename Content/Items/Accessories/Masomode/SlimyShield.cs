using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class SlimyShield : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Shield");
            /* Tooltip.SetDefault(@"Grants immunity to Slimed
Increases fall speed
When you land after a jump, slime will fall from the sky over your cursor
Slime inflicts Slimed and Oiled
'Torn from the innards of a defeated foe'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "粘液盾");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'从被打败的敌人的内脏中撕裂而来'
            // 免疫黏糊
            // 增加15%下落速度
            // 跳跃落地后,在光标处落下史莱姆");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1);
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Slimed] = true;

            player.AddEffect<SlimeFallEffect>(Item);

            player.FargoSouls().SlimyShieldItem = Item;
        }
    }
    public class SlimeFallEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SlimyShield>();

        public override void PostUpdateEquips(Player player)
        {
            player.maxFallSpeed *= 1.5f;
        }
    }
    public class SlimyShieldEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SupremeFairyHeader>();
        public override int ToggleItemType => ModContent.ItemType<SlimyShield>();
        public override bool ExtraAttackEffect => true;
    }
}