using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class SecurityWallet : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Security Wallet");
            /* Tooltip.SetDefault(@"Grants immunity to Midas
Drastically improves reforges
'Not secure against being looted off of one's corpse'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "安全钱包");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'无法保证在多人游戏中的效果'
            // 免疫点金手和偷取物品的敌人
            // 阻止你重铸带有特定词缀的物品
            // 可以在灵魂开关菜单中选择受保护的词缀
            // 重铸价格降低50%");

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

        private void Effects(Player player)
        {
            player.buffImmune[Terraria.ModLoader.ModContent.BuffType<Buffs.Masomode.MidasBuff>()] = true;
            player.buffImmune[Terraria.ModLoader.ModContent.BuffType<Buffs.Masomode.LoosePocketsBuff>()] = true;
            player.FargoSouls().SecurityWallet = true;
        }

        public override void UpdateInventory(Player player)
        {
            Effects(player);
        }

        public override void UpdateVanity(Player player)
        {
            Effects(player);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Effects(player);
        }
    }
}