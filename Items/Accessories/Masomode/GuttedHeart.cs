using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "破碎的心");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'曾经还在敌人的脑中跳动着'
            // 免疫嗜血
            // 增加10%最大生命值
            // 爬行者徘徊周围来阻挡伤害
            // 每15秒生成一个新的爬行者,最多同时存在5个");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            player.statLifeMax2 += player.statLifeMax / 10;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Bloodthirsty>()] = true;
            fargoPlayer.GuttedHeart = true;
        }
    }
}