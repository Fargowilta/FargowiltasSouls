using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Expert
{
    public class UniverseCore : SoulsItem
    {
        public override int NumFrames => 8;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Universe Core");
            Tooltip.SetDefault(@"Crits deal 5x instead of 2x
All attacks inflict Flames of the Universe
'Bursting with ultra-high-energy cosmic rays'");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 50);

            Item.scale *= 0.5f;

            Item.expert = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().UniverseCore = true;
        }
    }
}