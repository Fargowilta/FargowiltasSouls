using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class Eridanium : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eridanium");
            // Tooltip.SetDefault("A shard of cosmic power");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "月之水晶");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "月球能量的碎片\n宇宙英灵掉落");
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Purple;
            Item.width = 12;
            Item.height = 12;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }
    }
}
