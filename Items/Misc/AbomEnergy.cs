using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Items.Misc
{
    public class AbomEnergy : SoulsItem
    {
        public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Abominable Energy");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.ItemNoGravity[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 99;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 4, 0, 0);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}