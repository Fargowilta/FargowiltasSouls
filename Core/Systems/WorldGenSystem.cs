using FargowiltasSouls.Content.Tiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Systems
{
    public class WorldGenSystem : ModSystem
    {
        public static bool TryPlacingStatue(int baseCheckX, int baseCheckY)
        {
            List<int> legalBlocks = new()
                {
                    TileID.Stone,
                    TileID.Grass,
                    TileID.Dirt,
                    TileID.SnowBlock,
                    TileID.IceBlock,
                    TileID.ClayBlock,
                    TileID.Mud,
                    TileID.JungleGrass,
                    TileID.Sand,
                    TileID.Ash,
                    TileID.AshGrass
                };

            bool canPlaceStatueHere = true;
            for (int i = 0; i < 3; i++) //check no obstructing blocks
                for (int j = 0; j < 4; j++)
                {
                    Tile tile = Framing.GetTileSafely(baseCheckX + i, baseCheckY + j);
                    if (WorldGen.SolidOrSlopedTile(tile))
                    {
                        canPlaceStatueHere = false;
                        break;
                    }
                }
            for (int i = 0; i < 3; i++) //check for solid foundation
            {
                Tile tile = Framing.GetTileSafely(baseCheckX + i, baseCheckY + 4);
                if (!WorldGen.SolidTile(tile) || !legalBlocks.Contains(tile.TileType))
                {
                    canPlaceStatueHere = false;
                    break;
                }
            }

            if (canPlaceStatueHere)
            {
                for (int i = 0; i < 3; i++) //MAKE SURE nothing in the way
                    for (int j = 0; j < 4; j++)
                        WorldGen.KillTile(baseCheckX + i, baseCheckY + j);

                WorldGen.PlaceTile(baseCheckX, baseCheckY + 4, TileID.GrayBrick, false, true);
                WorldGen.PlaceTile(baseCheckX + 1, baseCheckY + 4, TileID.GrayBrick, false, true);
                WorldGen.PlaceTile(baseCheckX + 2, baseCheckY + 4, TileID.GrayBrick, false, true);
                Tile tile = Main.tile[baseCheckX, baseCheckY + 4]; tile.Slope = 0;
                tile = Main.tile[baseCheckX + 1, baseCheckY + 4]; tile.Slope = 0;
                tile = Main.tile[baseCheckX + 2, baseCheckY + 4]; tile.Slope = 0;
                WorldGen.PlaceTile(baseCheckX + 1, baseCheckY + 3, ModContent.TileType<MutantStatueGift>(), false, true);

                return true;
            }

            return false;
        }
        public override void PostWorldGen()
        {
            /*WorldGen.PlaceTile(Main.spawnTileX - 1, Main.spawnTileY, TileID.GrayBrick, false, true);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY, TileID.GrayBrick, false, true);
            WorldGen.PlaceTile(Main.spawnTileX + 1, Main.spawnTileY, TileID.GrayBrick, false, true);
            Main.tile[Main.spawnTileX - 1, Main.spawnTileY].slope(0);
            Main.tile[Main.spawnTileX, Main.spawnTileY].slope(0);
            Main.tile[Main.spawnTileX + 1, Main.spawnTileY].slope(0);
            WorldGen.PlaceTile(Main.spawnTileX, Main.spawnTileY - 1, ModContent.Find<ModTile>("Fargowiltas", "RegalStatueSheet"), false, true);*/


            int positionX = Main.spawnTileX - 1; //offset by dimensions of statue
            int positionY = Main.spawnTileY - 4;
            int checkUp = -30;
            int checkDown = 10;
            bool placed = false;
            for (int offsetX = -50; offsetX <= 50; offsetX++)
            {
                for (int offsetY = checkUp; offsetY <= checkDown; offsetY++)
                {
                    if (TryPlacingStatue(positionX + offsetX, positionY + offsetY))
                    {
                        placed = true;
                        WorldSavingSystem.PlacedMutantStatue = true;
                        break;
                    }
                }

                if (placed)
                    break;
            }

        }
    }
}
