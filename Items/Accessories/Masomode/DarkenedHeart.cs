using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class DarkenedHeart : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Darkened Heart");
            Tooltip.SetDefault(@"Grants immunity to Rotting
10% increased movement speed and increased turnaround traction
You spawn mini eaters to seek out enemies every few attacks
'Flies refuse to approach it'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "腐化之心");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'苍蝇都不想接近它'
            // 免疫腐败
            // 增加10%移动速度
            // 每隔几次攻击就会产生一个迷你噬魂者追踪敌人");

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
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Rotting>()] = true;
            player.moveSpeed += 0.1f;
            modPlayer.DarkenedHeartItem = Item;
            if (modPlayer.DarkenedHeartCD > 0)
                modPlayer.DarkenedHeartCD--;
        }
    }
}