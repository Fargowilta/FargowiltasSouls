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
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫点金手和偷取物品的敌人
阻止你重铸带有特定词缀的物品
可以在灵魂开关菜单中选择受保护的词缀
重铸价格降低50%
'无法保证在多人游戏中的效果'");
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
