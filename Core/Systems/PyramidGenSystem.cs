using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace FargowiltasSouls.Core.Systems
{
    // TODO: 1.7 pyramid gen 
    /*
    public class PyramidGenSystem : ModSystem
    {
        public override void Load()
        {
            Terraria.On_WorldGen.Pyramid += OnPyramidGen;
        }
        public override void Unload()
        {
            Terraria.On_WorldGen.Pyramid -= OnPyramidGen;
        }
        public Point PyramidLocation = new();
        public override void PreWorldGen()
        {
            PyramidLocation = new();
        }
        public bool OnPyramidGen(On_WorldGen.orig_Pyramid orig, int i, int j)
        {
            bool ret = orig(i, j);
            if (ret)
            {
                if (PyramidLocation == Point.Zero)
                {
                    PyramidLocation = new(i, j);
                }
            }
            return ret;
        }
        public override void PreUpdateWorld()
        {
            //Main.NewText("pyramid pos: " + PyramidLocation.X + " " + PyramidLocation.Y + " your pos: " + Main.LocalPlayer.Bottom.ToTileCoordinates().X + " " + Main.LocalPlayer.Bottom.ToTileCoordinates().Y);
        }
        // Makes a Dunes biome and designates a Pyramid spot in it
        public static void GenerateDunesWithPyramid()
        {
            double worldSizeXMod = (double)Main.maxTilesX / 4200.0;
            DunesBiome dunesBiome = GenVars.configuration.CreateBiome<DunesBiome>();
            while (true)
            {
                Point dunePoint = Point.Zero;
                bool validated = false;
                int huh = 0; // what's up with this variable. it's very strange.
                while (!validated)
                {
                    dunePoint = WorldGen.RandomWorldPoint(0, 500, 0, 500);
                    bool nearJungle = Math.Abs(dunePoint.X - GenVars.jungleOriginX) < (int)(600.0 * worldSizeXMod);
                    bool nearEdge = Math.Abs(dunePoint.X - Main.maxTilesX / 2) < 300;
                    bool nearSnow = dunePoint.X > GenVars.snowOriginLeft - 300 && dunePoint.X < GenVars.snowOriginRight + 300;
                    huh++;
                    if (huh >= Main.maxTilesX)
                    {
                        nearJungle = false;
                    }
                    if (huh >= Main.maxTilesX * 2)
                    {
                        nearSnow = false;
                    }
                    validated = !(nearJungle || nearEdge || nearSnow);
                }
                dunesBiome.Place(dunePoint, GenVars.structures);
                int x = WorldGen.genRand.Next(dunePoint.X - 200, dunePoint.X + 200);
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    if (Main.tile[x, y].HasTile)
                    {
                        GenVars.PyrX[GenVars.numPyr] = x;
                        GenVars.PyrY[GenVars.numPyr] = y + 20;
                        GenVars.numPyr++;
                        break;
                    }
                }
                if (GenVars.numPyr > 0)
                    break;
            }
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // Find index of Dunes pass
            int dunesIndex = tasks.FindIndex(g => g.Name == "Dunes");
            // After Dunes pass, generate a Dunes biome and designate a Pyramid spot if no pyramid spot was designated
            // Note that this pyramid spot may still be invalidated by later worldgen; which is why later code exists
            tasks.Insert(dunesIndex + 1, new PassLegacy("GuaranteePyramid", delegate
            {
                if (GenVars.numPyr <= 0)
                    GenerateDunesWithPyramid();
            }));
            // Find index of Pyramid pass
            int pyramidIndex = tasks.FindIndex(g => g.Name == "Pyramids");
            // Generate Cursed Coffin arena right after Pyramid pass
            tasks.Insert(pyramidIndex + 1, new PassLegacy("CursedCoffinArena", delegate
            {
                if (!ModLoader.HasMod("Remnants")) // don't do this if remnants is enabled, because it's not compatible. instead use item that spawns the arena if you have remnants
                {
                    if (PyramidLocation == Point.Zero)
                    {
                        Rectangle undergroundDesertLocation = GenVars.UndergroundDesertLocation;
                        int x = undergroundDesertLocation.Center.X;
                        int y = undergroundDesertLocation.Top - 10;
                        WorldGen.Pyramid(x, y);
                    }

                }
            }));
        }
    }
    */
}
