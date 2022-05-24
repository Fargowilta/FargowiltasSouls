using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Tiles
{
    public class FargoGlobalTile : GlobalTile
    {
        //internal static Point16 PlayerCenterTile(Player player) => new Point16((int)(player.Center.X / 16), (int)(player.Center.Y / 16));
        //internal static int PlayerCenterTileX(Player player) => (int)(player.Center.X / 16);
        //internal static int PlayerCenterTileY(Player player) => (int)(player.Center.Y / 16);

        //internal static bool InGameWorldLeft(int x) => x > 39;
        //internal static bool InGameWorldRight(int x) => x < Main.maxTilesX - 39;
        //internal static bool InGameWorldTop(int y) => y > 39;
        //internal static bool InGameWorldBottom(int y) => y < Main.maxTilesY - 39;
        internal static bool InGameWorld(int x, int y) => x > 39 && x < Main.maxTilesX - 39 && y > 39 && y < Main.maxTilesY - 39;

        //internal static bool InWorldLeft(int x) => x >= 0;
        //internal static bool InWorldRight(int x) => x < Main.maxTilesX;
        //internal static bool InWorldTop(int y) => y >= 0;
        //internal static bool InWorldBottom(int y) => y < Main.maxTilesY;
        internal static bool InWorld(int x, int y) => x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;

        //internal static bool PlaceGrassTileCheck(int x, int y) => (y > 0 && !WorldGen.SolidTile(x, y - 1))
        //                                                            || (x > 0 && !WorldGen.SolidTile(x - 1, y))
        //                                                            || (x < Main.maxTilesX - 1 && !WorldGen.SolidTile(x + 1, y));

        //internal static void DestroyChest(int x, int y) => DestroyChest(Chest.FindChest(x, y), x, y);
        //internal static void DestroyChest(int chest, int x, int y)
        //{
        //    int chestType = 1;

        //    if (chest != -1)
        //    {
        //        for (int i = 0; i < 40; i++)
        //            Main.chest[chest].item[i] = new Item();

        //        Main.chest[chest] = null;

        //        if (Main.tile[x, y].TileType == TileID.Containers2)
        //            chestType = 5;
        //        if (Main.tile[x, y].TileType >= TileID.Count)
        //            chestType = 101;
        //    }

        //    for (int i = x; i < x + 2; i++)
        //        for (int j = y; j < y + 2; j++)
        //            ClearTile(i, j);

        //    if (Main.netMode != NetmodeID.SinglePlayer)
        //    {
        //        if (chest != -1)
        //            NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, chestType, x, y, 0f, chest, Main.tile[x, y].TileType, 0);

        //        NetMessage.SendTileSquare(-1, x, y, 3);
        //    }
        //}

        //internal static int FindChestSafe(int x, int y)
        //{
        //    if (Main.tile[x, y] != null)
        //    {
        //        Point16 pos = FindChestTopLeft(x, y, false);
        //        return Chest.FindChest(pos.X, pos.Y);
        //    }

        //    return Chest.FindChestByGuessing(x, y);
        //}

        //internal static Point16 FindChestTopLeft(int x, int y, bool destroy)
        //{
        //    if (TileID.Sets.BasicChest[Main.tile[x, y].TileType])
        //    {
        //        if (Main.tile[x, y].TileFrameX % 36 != 0)
        //            x--;
        //        if (Main.tile[x, y].TileFrameY % 36 != 0)
        //            y--;

        //        if (!destroy)
        //            return new Point16(x, y);

        //        DestroyChest(x, y);
        //        return new Point16(x, y);
        //    }
        //    return Point16.NegativeOne;
        //}

        //internal static void ClearLiquid(int x, int y) => ClearLiquid(Main.tile[x, y], x, y);
        //internal static void ClearLiquid(Tile tile, int x, int y)
        //{
        //    tile.LiquidAmount = 0;

        //    if (Main.netMode == NetmodeID.Server)
        //        NetMessage.sendWater(x, y);
        //}
        //internal static void ClearTile(int x, int y) => ClearTile(Main.tile[x, y]);
        //internal static void ClearTile(Tile tile)
        //{
        //    tile.TileType = 0;
        //    //tile.sTileHeader = 0;
        //    tile.TileFrameX = 0;
        //    tile.TileFrameY = 0;
        //}
        //internal static void ClearWall(int x, int y) => ClearWall(Main.tile[x, y]);
        //internal static void ClearWall(Tile tile)
        //{
        //    tile.WallType = 0;
        //    //tile.bTileHeader = 0;
        //    //tile.bTileHeader2 = 0;
        //    //tile.bTileHeader3 = 0;
        //}
        //internal static void ClearEverything(int x, int y)
        //{
        //    //FindChestTopLeft(x, y, true);
        //    Tile tile = Main.tile[x, y];

        //    if (tile != null)
        //    {
        //        ClearTile(tile);
        //        ClearWall(tile);
        //        ClearLiquid(tile, x, y);
        //    }
        //}
        //internal static void ClearTileWithNet(int x, int y)
        //{
        //    ClearTile(x, y);
        //    SquareUpdate(x, y);
        //}
        //internal static void ClearWallWithNet(int x, int y)
        //{
        //    ClearWall(x, y);
        //    SquareUpdate(x, y);
        //}
        //internal static void ClearEverythingWithNet(int x, int y)
        //{
        //    ClearEverything(x, y);
        //    SquareUpdate(x, y);
        //}
        //internal static void SquareUpdate(int x, int y)
        //{
        //    if (Main.netMode != NetmodeID.SinglePlayer)
        //        NetMessage.SendTileSquare(-1, x, y, 1);
        //}
        internal static bool NoDungeon(int x, int y) => NoBlueDungeon(x, y) && NoGreenDungeon(x, y) && NoPinkDungeon(x, y);
        internal static bool NoBlueDungeon(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.TileType != TileID.BlueDungeonBrick && tile.WallType != WallID.BlueDungeonSlabUnsafe
                && tile.WallType != WallID.BlueDungeonTileUnsafe && tile.WallType != WallID.BlueDungeonUnsafe;
        }
        internal static bool NoGreenDungeon(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.TileType != TileID.GreenDungeonBrick && tile.WallType != WallID.GreenDungeonSlabUnsafe
                && tile.WallType != WallID.GreenDungeonTileUnsafe && tile.WallType != WallID.GreenDungeonUnsafe;
        }
        internal static bool NoPinkDungeon(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.TileType != TileID.PinkDungeonBrick && tile.WallType != WallID.PinkDungeonSlabUnsafe
                && tile.WallType != WallID.PinkDungeonTileUnsafe && tile.WallType != WallID.PinkDungeonUnsafe;
        }
        internal static bool NoUndergroundDesert(int x, int y)
        {
            int wall = Main.tile[x, y].WallType;
            return wall != WallID.Sandstone && wall != WallID.CorruptSandstone && wall != WallID.CrimsonSandstone && wall != WallID.HallowSandstone;
        }
        internal static bool PlanteraBulb(int x, int y) => Main.tile[x, y].TileType == TileID.PlanteraBulb;
        internal static bool NoTemple(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.WallType != WallID.LihzahrdBrickUnsafe && tile.TileType != TileID.LihzahrdBrick
                && !(tile.TileType == TileID.ClosedDoor && tile.TileFrameY >= 594 && tile.TileFrameY <= 646);
        }
        internal static bool Temple(int x, int y) => !NoTemple(x, y);
        internal static bool TempleAndGolemIsDead(int x, int y) => !NoTemple(x, y) && NPC.downedGolemBoss;
        internal static bool NoTempleOrGolemIsDead(int x, int y) => NoTemple(x, y) || NPC.downedGolemBoss;
        internal static bool NoOrbOrAltar(int x, int y) => Main.tile[x, y].TileType != TileID.ShadowOrbs && Main.tile[x, y].TileType != TileID.DemonAltar;

        //internal static int CoordsX(int x) => x * 2 - Main.maxTilesX;
        //internal static int CoordsY(int y) => y * 2 - (int)Main.worldSurface * 2;
        //internal static string CoordsString(int x, int y)
        //{
        //    x = x * 2 - Main.maxTilesX;
        //    y = y * 2 - (int)Main.worldSurface * 2;
        //    string xCoord = x < 0 ? " west, " : " east, ";
        //    string yCoord = y < 0 ? " surface." : " underground.";
        //    x = x < 0 ? x * -1 : x;
        //    y = y < 0 ? y * -1 : y;
        //    return x + xCoord + y + yCoord;
        //}
        //internal static void MakeTileSafe(int x, int y)
        //{
        //    if (Main.tile[x, y] == null)
        //        Main.tile[x, y] = new Tile();
        //}
        //internal static void MakeTileSafe(ref Tile tile)
        //{
        //    if (tile == null)
        //        tile = new Tile();
        //}
        //internal static bool TileIsNull(int x, int y) => Main.tile[x, y] == null;
        internal static bool SolidTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && !tile.IsHalfBlock
                && tile.Slope == 0 && !tile.HasUnactuatedTile;
        }

        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if ((type == TileID.Platforms || type == TileID.PlanterBox) && Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().LowGround && Framing.GetTileSafely(i, j).HasTile)
            {
                float distance = Main.LocalPlayer.Distance(new Vector2(i * 16 + 8, j * 16 + 8));
                if (distance > 100 && distance < 1000)
                {
                    Framing.GetTileSafely(i, j).IsActuated = false;
                    NetMessage.SendTileSquare(-1, i, j, 1);
                }
            }
        }
    }
}