using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class TimsConcoction : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tim's Concoction");
            Tooltip.SetDefault(@"Certain enemies will drop potions when defeated
'Smells funny'");
            DisplayName.AddTranslation(GameCulture.Chinese, "蒂姆的秘药");
            Tooltip.AddTranslation(GameCulture.Chinese, @"大多数敌人在死亡时会掉落药水
'味道闻起来很怪'");
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
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoConcoction"))
                player.GetModPlayer<FargoPlayer>().TimsConcoction = true;
        }
    }
}
