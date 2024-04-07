using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Materials
{
    public class AbomEnergy : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominable Energy");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 4, 0, 0);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}