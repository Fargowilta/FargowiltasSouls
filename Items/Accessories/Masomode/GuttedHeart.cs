using Terraria;
using Terraria.Localization;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class GuttedHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gutted Heart");
            Tooltip.SetDefault(@"Grants immunity to Bloodthirsty
10% increased max life
Creepers hover around you blocking some damage
A new Creeper appears every 15 seconds, and 5 can exist at once
Creeper respawn speed increases when not moving
'Once beating in the mind of a defeated foe'");
            DisplayName.AddTranslation(GameCulture.Chinese, "破碎的心");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫嗜血减益
增加10%最大生命值
飞眼怪徘徊在你周围并阻挡一部分伤害
每过15秒便生成一只新的飞眼怪，至多存在5只飞眼怪
站定不动时增加飞眼怪的生成速度
'曾经还在敌人的脑中跳动着'");
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
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            player.statLifeMax2 += player.statLifeMax / 10;
            player.buffImmune[mod.BuffType("Bloodthirsty")] = true;
            fargoPlayer.GuttedHeart = true;
        }
    }
}
