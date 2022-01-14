using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Tiles
{
    public class GoldenDippingVatSheet : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = true;
            //TileObjectData.newTile.Origin = new Point16(0, 1);
            //TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Golden Dipping Vat");
            AddMapEntry(new Color(255, 215, 0), name);
            name.AddTranslation(GameCulture.Chinese, "黄金浸渍缸");

            animationFrameHeight = 54;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType("GoldenDippingVat"));
		}

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 10) //replace with duration of frame in ticks
            {
                frameCounter = 0;
                frame++;
                frame %= 12;
            }
        }
    }
}