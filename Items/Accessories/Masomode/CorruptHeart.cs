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
            /*DisplayName.SetDefault("Corrupt Heart");
            Tooltip.SetDefault(@"Grants immunity to Rotting
10% increased movement speed
You spawn mini eaters to seek out enemies every few attacks
'Flies refuse to approach it'");
            DisplayName.AddTranslation(GameCulture.Chinese, "腐化之心");
            Tooltip.AddTranslation(GameCulture.Chinese, @"'苍蝇都不想接近它'
免疫腐败
增加10%移动速度
每隔几次攻击就会产生一个迷你噬魂者追踪敌人");*/
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