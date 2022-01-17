using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls
{
    public class FargoGlobalTile : GlobalTile
    {
        internal static Point16 PlayerCenterTile(Player player) => new Point16((int)(player.Center.X / 16), (int)(player.Center.Y / 16));
        internal static int PlayerCenterTileX(Player player) => (int)(player.Center.X / 16);
        internal static int PlayerCenterTileY(Player player) => (int)(player.Center.Y / 16);

        internal static bool InGameWorldLeft(int x) => x > 39;
        internal static bool InGameWorldRight(int x) => x < Main.maxTilesX - 39;
        internal static bool InGameWorldTop(int y) => y > 39;
        internal static bool InGameWorldBottom(int y) => y < Main.maxTilesY - 39;
        internal static bool InGameWorld(int x, int y) => x > 39 && x < Main.maxTilesX - 39 && y > 39 && y < Main.maxTilesY - 39;

        internal static bool InWorldLeft(int x) => x >= 0;
        internal static bool InWorldRight(int x) => x < Main.maxTilesX;
        internal static bool InWorldTop(int y) => y >= 0;
        internal static bool InWorldBottom(int y) => y < Main.maxTilesY;
        internal static bool InWorld(int x, int y) => x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;

        internal static bool PlaceGrassTileCheck(int x, int y) => (y > 0 && !WorldGen.SolidTile(x, y - 1))
                                                                    || (x > 0 && !WorldGen.SolidTile(x - 1, y))
                                                                    || (x < Main.maxTilesX - 1 && !WorldGen.SolidTile(x + 1, y));

        internal static void DestroyChest(int x, int y) => DestroyChest(Chest.FindChest(x, y), x, y);
        internal static void DestroyChest(int chest, int x, int y)
        {
            int chestType = 1;

            if (chest != -1)
            {
                for (int i = 0; i < 40; i++)
                    Main.chest[chest].item[i] = new Item();

                Main.chest[chest] = null;

                if (Main.tile[x, y].type == TileID.Containers2)
                    chestType = 5;
                if (Main.tile[x, y].type >= TileID.Count)
                    chestType = 101;
            }

            for (int i = x; i < x + 2; i++)
                for (int j = y; j < y + 2; j++)
                    ClearTile(i, j);

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (chest != -1)
                    NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, chestType, x, y, 0f, chest, Main.tile[x, y].type, 0);

                NetMessage.SendTileSquare(-1, x, y, 3);
            }
        }

        internal static int FindChestSafe(int x, int y)
        {
            if (Main.tile[x, y] != null)
            {
                Point16 pos = FindChestTopLeft(x, y, false);
                return Chest.FindChest(pos.X, pos.Y);
            }

            return Chest.FindChestByGuessing(x, y);
        }

        internal static Point16 FindChestTopLeft(int x, int y, bool destroy)
        {
            if (TileID.Sets.BasicChest[Main.tile[x, y].type])
            {
                if (Main.tile[x, y]?.frameX % 36 != 0)
                    x--;
                if (Main.tile[x, y]?.frameY % 36 != 0)
                    y--;

                if (!destroy)
                    return new Point16(x, y);

                DestroyChest(x, y);
                return new Point16(x, y);
            }
            return Point16.NegativeOne;
        }

        internal static void ClearLiquid(int x, int y) => ClearLiquid(Main.tile[x, y], x, y);
        internal static void ClearLiquid(Tile tile, int x, int y)
        {
            tile.liquid = 0;
            tile.lava(false);
            tile.honey(false);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.sendWater(x, y);
        }
        internal static void ClearTile(int x, int y) => ClearTile(Main.tile[x, y]);
        internal static void ClearTile(Tile tile)
        {
            tile.type = 0;
            tile.sTileHeader = 0;
            tile.frameX = 0;
            tile.frameY = 0;
        }
        internal static void ClearWall(int x, int y) => ClearWall(Main.tile[x, y]);
        internal static void ClearWall(Tile tile)
        {
            tile.wall = 0;
            tile.bTileHeader = 0;
            tile.bTileHeader2 = 0;
            tile.bTileHeader3 = 0;
        }
        internal static void ClearEverything(int x, int y)
        {
            //FindChestTopLeft(x, y, true);
            Tile tile = Main.tile[x, y];

            if (tile != null)
            {
                ClearTile(tile);
                ClearWall(tile);
                ClearLiquid(tile, x, y);
            }

           
        }
        internal static void ClearTileWithNet(int x, int y)
        {
            ClearTile(x, y);
            SquareUpdate(x, y);
        }
        internal static void ClearWallWithNet(int x, int y)
        {
            ClearWall(x, y);
            SquareUpdate(x, y);
        }
        internal static void ClearEverythingWithNet(int x, int y)
        {
            ClearEverything(x, y);
            SquareUpdate(x, y);
        }
        internal static void SquareUpdate(int x, int y)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendTileSquare(-1, x, y, 1);
        }
        internal static bool NoDungeon(int x, int y) => NoBlueDungeon(x, y) && NoGreenDungeon(x, y) && NoPinkDungeon(x, y);
        internal static bool NoBlueDungeon(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.type != TileID.BlueDungeonBrick && tile.wall != WallID.BlueDungeonSlabUnsafe
                && tile.wall != WallID.BlueDungeonTileUnsafe && tile.wall != WallID.BlueDungeonUnsafe;
        }
        internal static bool NoGreenDungeon(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.type != TileID.GreenDungeonBrick && tile.wall != WallID.GreenDungeonSlabUnsafe
                && tile.wall != WallID.GreenDungeonTileUnsafe && tile.wall != WallID.GreenDungeonUnsafe;
        }
        internal static bool NoPinkDungeon(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.type != TileID.PinkDungeonBrick && tile.wall != WallID.PinkDungeonSlabUnsafe
                && tile.wall != WallID.PinkDungeonTileUnsafe && tile.wall != WallID.PinkDungeonUnsafe;
        }
        internal static bool NoUndergroundDesert(int x, int y)
        {
            int wall = Main.tile[x, y].wall;
            return wall != WallID.Sandstone && wall != WallID.CorruptSandstone && wall != WallID.CrimsonSandstone && wall != WallID.HallowSandstone;
        }
        internal static bool PlanteraBulb(int x, int y) => Main.tile[x, y].type == TileID.PlanteraBulb;
        internal static bool NoTemple(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.wall != WallID.LihzahrdBrickUnsafe && tile.type != TileID.LihzahrdBrick
                && !(tile.type == TileID.ClosedDoor && tile.frameY >= 594 && tile.frameY <= 646);
        }
        internal static bool Temple(int x, int y) => !NoTemple(x, y);
        internal static bool TempleAndGolemIsDead(int x, int y) => !NoTemple(x, y) && NPC.downedGolemBoss;
        internal static bool NoTempleOrGolemIsDead(int x, int y) => NoTemple(x, y) || NPC.downedGolemBoss;
        internal static bool NoOrbOrAltar(int x, int y) => Main.tile[x, y].type != TileID.ShadowOrbs && Main.tile[x, y].type != TileID.DemonAltar;

        internal static int CoordsX(int x) => x * 2 - Main.maxTilesX;
        internal static int CoordsY(int y) => y * 2 - (int)Main.worldSurface * 2;
        internal static string CoordsString(int x, int y)
        {
            x = x * 2 - Main.maxTilesX;
            y = y * 2 - (int)Main.worldSurface * 2;
            string xCoord = x < 0 ? Language.GetTextValue("Mods.FargowiltasSouls.FargoGlobalTile.West") : Language.GetTextValue("Mods.FargowiltasSouls.FargoGlobalTile.East");
            string yCoord = y < 0 ? Language.GetTextValue("Mods.FargowiltasSouls.FargoGlobalTile.Surface") : Language.GetTextValue("Mods.FargowiltasSouls.FargoGlobalTile.Underground");
            x = x < 0 ? x * -1 : x;
            y = y < 0 ? y * -1 : y;
            return x + xCoord + y + yCoord;
        }
        internal static void MakeTileSafe(int x, int y)
        {
            if (Main.tile[x, y] == null)
                Main.tile[x, y] = new Tile();
        }
        internal static void MakeTileSafe(ref Tile tile)
        {
            if (tile == null)
                tile = new Tile();
        }
        internal static bool TileIsNull(int x, int y) => Main.tile[x, y] == null;
        internal static bool SolidTile(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return !TileIsNull(x, y) && tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type] && !tile.halfBrick()
                && tile.slope() == 0 && !tile.inActive();
        }

        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            if (type == TileID.LihzahrdAltar && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost
                && Collision.CanHit(new Vector2(i * 16 + 8, j * 16 + 8), 0, 0, Main.LocalPlayer.Center, 0, 0)
                && Main.LocalPlayer.Distance(new Vector2(i * 16 + 8, j * 16 + 8)) < 3000
                && Framing.GetTileSafely(Main.LocalPlayer.Center).wall == WallID.LihzahrdBrickUnsafe)
            {
                if (Main.LocalPlayer.active)
                {
                    if (!Main.LocalPlayer.HasBuff(mod.BuffType("LihzahrdBlessing")))
                    {
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.FargoGlobalTile.LihzahrdBlessing"), Color.Orange);
                        Main.PlaySound(SoundID.Item4, Main.LocalPlayer.Center);
                        for (int k = 0; k < 50; k++)
                        {
                            int d = Dust.NewDust(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(3f, 6f));
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 9f;
                        }
                    }
                    Main.LocalPlayer.AddBuff(mod.BuffType("LihzahrdBlessing"), 60 * 60 * 10 + 60); //10mins
                }
            }

            if ((type == TileID.Platforms || type == TileID.PlanterBox) && Main.LocalPlayer.GetModPlayer<FargoPlayer>().LowGround
                && Framing.GetTileSafely(i, j).inActive())
            {
                float distance = Main.LocalPlayer.Distance(new Vector2(i * 16 + 8, j * 16 + 8));
                if (distance > 100 && distance < 1000)
                    Framing.GetTileSafely(i, j).inActive(false);
            }
        }

        private bool CanBreakTileMaso(int i, int j, int type)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                if (Framing.GetTileSafely(i, j).wall == WallID.LihzahrdBrickUnsafe && (type == TileID.Traps || type == TileID.PressurePlates))
                {
                    int p = Player.FindClosest(new Vector2(i * 16 + 8, j * 16 + 8), 0, 0);
                    if (p != -1)
                    {
                        //if player INSIDE TEMPLE, but not cursed, its ok to break
                        Tile tile = Framing.GetTileSafely(Main.player[p].Center);
                        if (tile.wall == WallID.LihzahrdBrickUnsafe && !Main.player[p].GetModPlayer<FargoPlayer>().LihzahrdCurse)
                            return true;
                    }
                    //if player outside temple, or player in temple but is cursed, dont break
                    return false;
                }
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (!CanBreakTileMaso(i, j, type))
                return false;

            return true;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (!CanBreakTileMaso(i, j, type))
                return false;

            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                switch (type)
                {
                    case TileID.ShadowOrbs:
                        //if (!WorldGen.shadowOrbSmashed && !NPC.downedGoblins) FargoSoulsWorld.forceMeteor = true;
                        if (Main.invasionType == 0 && !NPC.downedGoblins && WorldGen.shadowOrbSmashed) //force goblins
                        {
                            int p = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                            if (p != -1 && Main.player[p].statLifeMax2 >= 200)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Main.invasionDelay = 0;
                                    Main.StartInvasion(1);
                                }
                                else
                                {
                                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, p, -1f);
                                }
                            }
                        }
                        break;

                    case TileID.DemonAltar:
                        /*if (Main.hardMode && Main.invasionType == 0 && !NPC.downedPirates && WorldGen.altarCount > 2)
                        {
                            int p = Player.FindClosest(new Vector2(i * 16, j * 16), 0, 0);
                            if (p != -1 && Main.player[p].statLifeMax2 >= 200)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Main.invasionDelay = 0;
                                    Main.StartInvasion(3);
                                }
                                else
                                {
                                    NetMessage.SendData(61, -1, -1, null, p, -1f);
                                }
                            }
                        }*/
                        break;

                    /*case TileID.Trees:
                        Player player = Main.player[Main.myPlayer];

                        if (player.ZoneJungle && player.ownedProjectileCounts[ProjectileID.BeeHive] == 0)
                        {
                            Projectile.NewProjectile(new Vector2(i * 16, j * 16), Vector2.Zero, ProjectileID.BeeHive, 0, 0, player.whoAmI);
                        }

                        break;*/

                    default:
                        break;
                }
            }


            
        }
    }
}