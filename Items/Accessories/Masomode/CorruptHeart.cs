using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class CorruptHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupt Heart");
            Tooltip.SetDefault(@"Grants immunity to Rotting
10% increased movement speed
You spawn mini eaters to seek out enemies every few attacks
'Flies refuse to approach it'");
            DisplayName.AddTranslation(GameCulture.Chinese, "腐化之心");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫腐败减益
增加10%移动速度
每攻击几次便释放迷你吞噬者来搜寻并攻击敌人
'就连苍蝇也不想靠近这玩意'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.sellPrice(0, 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            player.buffImmune[mod.BuffType("Rotting")] = true;
            player.moveSpeed += 0.1f;
            modPlayer.CorruptHeart = true;
            if (modPlayer.CorruptHeartCD > 0)
                modPlayer.CorruptHeartCD--;
        }
    }
}
