using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class DeviatingEnergy : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deviating Energy");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}