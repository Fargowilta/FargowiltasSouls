using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class PungentEyeball : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pungent Eyeball");
            Tooltip.SetDefault(@"Grants immunity to Blackout and Obstructed
Increases your max number of minions by 2
Increases your max number of sentries by 2
'It's fermenting'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "辛辣的眼球");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'它在发酵'
            // 免疫致盲和阻塞
            // +2最大召唤栏
            // +2最大哨兵栏");

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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.maxMinions += 2;
            player.maxTurrets += 2;
        }
    }
}