using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class LihzahrdTreasureBox : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lihzahrd Treasure Box");
            Tooltip.SetDefault(@"Grants immunity to Burning, Fused, and Low Ground
Press down in the air to fastfall
Fastfall will create a fiery eruption on impact after falling a certain distance
When you land after a jump, you create a burst of boulders
'Too many booby traps to open'");
            DisplayName.AddTranslation(GameCulture.Chinese, "神庙蜥蜴宝藏盒");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫燃烧、导火线和低地减益
在空中按'下'键会进行快速下落
在一定高度使用快速下落后会在撞击地面时产生猛烈的火焰喷发
跳跃落地后，你会释放一堆滚石
'陷阱太多，无法打开'");//Provisional
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 6);
            item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[mod.BuffType("Fused")] = true;
            player.buffImmune[mod.BuffType("LihzahrdCurse")] = true;
            player.buffImmune[mod.BuffType("LowGround")] = true;
            player.GetModPlayer<FargoPlayer>().LihzahrdTreasureBox = true;
        }
    }
}
