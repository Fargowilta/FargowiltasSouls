using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace FargowiltasSouls.Content.Tiles
{
	public class MutantStatueGift : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.LavaDeath = false;
			//TileObjectData.newTile.Origin = new Point16(0, 1);
			//TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Mutant Statue");
			AddMapEntry(new Color(144, 144, 144), name);

		}
		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<Items.Masochist>();
		}
		public override bool RightClick(int i, int j)
		{

			Tile tile = Framing.GetTileSafely(i, j);
			//account for possibly clicking on any part of the multi-tile, negate it to have coords of top left corner
			i -= tile.TileFrameX / 18;
			j -= tile.TileFrameY / 18;
			//add offset to get middle bottom tile
			i += 1;
			j += 2;
			WorldGen.KillTile(i, j, noItem: true);

			return true;
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			//account for possibly clicking on any part of the multi-tile, negate it to have coords of top left corner
			i -= tile.TileFrameX / 18;
			j -= tile.TileFrameY / 18;
			//add offset to get middle bottom tile
			i += 1;
			j += 2;

			Item.NewItem(new EntitySource_TileBreak(i, j - 1), i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Masochist>());
			WorldGen.PlaceTile(i, j, ModContent.TileType<MutantStatue>());
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 0;
		}
		public override bool CanDrop(int i, int j) => false;

	}
}