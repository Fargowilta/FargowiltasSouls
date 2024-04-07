using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class RabiesShot : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rabies Shot");
            // Tooltip.SetDefault("Cures Feral Bite");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "狂犬疫苗");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "治愈野性咬噬");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.UseSound = SoundID.Item3;
            Item.value = Item.sellPrice(0, 0, 4, 0);
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.ClearBuff(BuffID.Rabies);
            }
            return true;
        }
    }
}