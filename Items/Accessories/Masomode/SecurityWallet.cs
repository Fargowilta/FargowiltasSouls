using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class SecurityWallet : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Security Wallet");
            Tooltip.SetDefault(@"Grants immunity to Midas and enemies that steal items
50% discount on reforges
'Not secure against being looted off of one's corpse'");
            DisplayName.AddTranslation(GameCulture.Chinese, "安全钱包");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫迈达斯减益和敌人的偷取物品效果
减少50%重铸价格
'不保证不会被尸体抢劫'");
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
            player.buffImmune[mod.BuffType("Midas")] = true;
            player.GetModPlayer<FargoPlayer>().SecurityWallet = true;
        }
    }
}
