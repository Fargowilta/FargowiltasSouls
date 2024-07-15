using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Core.Systems
{
    public class WorldSavingSystem : ModSystem
    {
        internal enum Downed //to keep them organized and synced, DO NOT rearrange
        {
            TimberChampion,
            TerraChampion,
            EarthChampion,
            NatureChampion,
            LifeChampion,
            ShadowChampion,
            SpiritChampion,
            WillChampion,
            CosmosChampion,
            TrojanSquirrel,
            Lifelight,
            CursedCoffin,
            BanishedBaron,
            Magmaw
        }

        //masomode
        public const int MaxCountPreHM = 560;
        public const int MaxCountHM = 240;

        internal static bool swarmActive;
        internal static bool downedBetsy;
        internal static bool shouldBeEternityMode;
        internal static bool masochistModeReal;
        internal static bool canPlayMaso;
        internal static bool downedFishronEX;
        internal static bool downedDevi;
        internal static bool downedAbom;
        internal static bool downedMutant;
        internal static bool angryMutant;
        internal static bool haveForcedAbomFromGoblins;
        internal static int skipMutantP1;
        internal static bool receivedTerraStorage;
        internal static bool spawnedDevi;
        internal static bool downedAnyBoss;
        internal static bool[] downedBoss = new bool[Enum.GetValues(typeof(Downed)).Length];
        internal static bool wOFDroppedDeviGift2;

        public static bool EternityMode { get; set; }

        public static bool EternityVanillaBehaviour { get; set; }

        public static bool DownedAnyBoss { get => downedAnyBoss; set => downedAnyBoss = value; }

        public static int SkipMutantP1 { get => skipMutantP1; set => skipMutantP1 = value; }

        public static bool WOFDroppedDeviGift2 { get => wOFDroppedDeviGift2; set => wOFDroppedDeviGift2 = value; }

        public static bool[] DownedBoss { get => downedBoss; set => downedBoss = value; }

        public static bool ShouldBeEternityMode { get => shouldBeEternityMode; set => shouldBeEternityMode = value; }

        public static bool MasochistModeReal { get => masochistModeReal; set => masochistModeReal = value; }

        public static bool CanPlayMaso { get => canPlayMaso; set => canPlayMaso = value; }

        public static bool DownedFishronEX { get => downedFishronEX; set => downedFishronEX = value; }

        public static bool DownedDevi { get => downedDevi; set => downedDevi = value; }

        public static bool DownedAbom { get => downedAbom; set => downedAbom = value; }

        public static bool DownedMutant { get => downedMutant; set => downedMutant = value; }

        public static bool AngryMutant { get => angryMutant; set => angryMutant = value; }

        public static bool HaveForcedAbomFromGoblins { get => haveForcedAbomFromGoblins; set => haveForcedAbomFromGoblins = value; }

        public static bool ReceivedTerraStorage { get => receivedTerraStorage; set => receivedTerraStorage = value; }

        public static bool SpawnedDevi { get => spawnedDevi; set => spawnedDevi = value; }

        public static bool DownedBetsy { get => downedBetsy; set => downedBetsy = value; }

        public static bool SwarmActive { get => swarmActive; set => swarmActive = value; }

        public static bool PlacedMutantStatue;

        public static List<int> IronUsedList = [];

        public override void Unload() => DownedBoss = null;

        private static void ResetFlags()
        {
            SwarmActive = false;
            DownedBetsy = false;

            ShouldBeEternityMode = false;
            EternityMode = false;
            EternityVanillaBehaviour = true;
            CanPlayMaso = false;
            MasochistModeReal = false;
            DownedFishronEX = false;
            DownedDevi = false;
            DownedAbom = false;
            DownedMutant = false;
            AngryMutant = false;

            HaveForcedAbomFromGoblins = false;
            SkipMutantP1 = 0;

            ReceivedTerraStorage = false;
            SpawnedDevi = false;

            for (int i = 0; i < DownedBoss.Length; i++)
                DownedBoss[i] = false;

            DownedAnyBoss = false;
            WOFDroppedDeviGift2 = false;

            PlacedMutantStatue = false;
        }

        public override void OnWorldLoad() => ResetFlags();

        public override void OnWorldUnload() => ResetFlags();

        public override void SaveWorldData(TagCompound tag)
        {

            List<string> downed = [];
            if (DownedBetsy)
                downed.Add("betsy");

            if (ShouldBeEternityMode)
                downed.Add("shouldBeEternityMode");

            if (EternityMode)
                downed.Add("eternity");

            if (CanPlayMaso)
                downed.Add("CanPlayMaso");

            if (MasochistModeReal)
                downed.Add("getReal");

            if (DownedFishronEX)
                downed.Add("downedFishronEX");

            if (DownedDevi)
                downed.Add("downedDevi");

            if (DownedAbom)
                downed.Add("downedAbom");

            if (DownedMutant)
                downed.Add("downedMutant");

            if (AngryMutant)
                downed.Add("AngryMutant");

            if (HaveForcedAbomFromGoblins)
                downed.Add("haveForcedAbomFromGoblins");

            if (ReceivedTerraStorage)
                downed.Add("ReceivedTerraStorage");

            if (SpawnedDevi)
                downed.Add("spawnedDevi");

            if (DownedAnyBoss)
                downed.Add("downedAnyBoss");

            if (WOFDroppedDeviGift2)
                downed.Add("WOFDroppedDeviGift2");
            if (PlacedMutantStatue)
                downed.Add("PlacedMutantStatue");

            if (IronUsedList.Count > 0)
            {
                string ironData = "IronUsedList";
                foreach (int type in IronUsedList)
                {
                    if (type >= ItemID.Count) // modded item, variable type, add name instead
                    {
                        if (ItemLoader.GetItem(type) is ModItem modItem && modItem != null)
                            ironData += $"_{modItem.FullName}";
                    }
                    else // vanilla item
                    {
                        ironData += $"_{type}";
                    }
                }
                downed.Add(ironData);
            }

            for (int i = 0; i < DownedBoss.Length; i++)
            {
                if (DownedBoss[i])
                    downed.Add("downedBoss" + i.ToString());
            }

            tag.Add("downed", downed);
            tag.Add("mutantP1", SkipMutantP1);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downed");
            DownedBetsy = downed.Contains("betsy");
            ShouldBeEternityMode = downed.Contains("shouldBeEternityMode");
            EternityMode = downed.Contains("eternity") || downed.Contains("masochist");
            EternityVanillaBehaviour = true;
            CanPlayMaso = downed.Contains("CanPlayMaso");
            MasochistModeReal = downed.Contains("getReal");
            DownedFishronEX = downed.Contains("downedFishronEX");
            DownedDevi = downed.Contains("downedDevi");
            DownedAbom = downed.Contains("downedAbom");
            DownedMutant = downed.Contains("downedMutant");
            AngryMutant = downed.Contains("AngryMutant");
            HaveForcedAbomFromGoblins = downed.Contains("haveForcedAbomFromGoblins");
            ReceivedTerraStorage = downed.Contains("ReceivedTerraStorage");
            SpawnedDevi = downed.Contains("spawnedDevi");
            DownedAnyBoss = downed.Contains("downedAnyBoss");
            WOFDroppedDeviGift2 = downed.Contains("WOFDroppedDeviGift2");
            PlacedMutantStatue = downed.Contains("PlacedMutantStatue");

            if (downed.Contains("IronUsedList_"))
            {
                string ironData = downed.First(i => i.Contains("IronUsedList"));
                string[] ironEntries = ironData.Split("_");
                foreach (string entry in ironEntries)
                {
                    if (entry != "IronUsedList")
                    {
                        if (int.TryParse(entry, out int type) && type < ItemID.Count)
                        {
                            IronUsedList.Add(type);
                        }
                        else
                        {
                            ModItem item = ModContent.Find<ModItem>(entry);
                            IronUsedList.Add(item.Type);
                        }

                    }
                }
            }

            for (int i = 0; i < DownedBoss.Length; i++)
                DownedBoss[i] = downed.Contains($"downedBoss{i}") || downed.Contains($"downedChampion{i}");

            if (tag.ContainsKey("mutantP1"))
                SkipMutantP1 = tag.GetAsInt("mutantP1");
        }

        public override void NetReceive(BinaryReader reader)
        {
            SkipMutantP1 = reader.ReadInt32();

            BitsByte flags = reader.ReadByte();
            DownedBetsy = flags[0];
            EternityMode = flags[1];
            DownedFishronEX = flags[2];
            DownedDevi = flags[3];
            DownedAbom = flags[4];
            DownedMutant = flags[5];
            AngryMutant = flags[6];
            HaveForcedAbomFromGoblins = flags[7];

            flags = reader.ReadByte();
            ReceivedTerraStorage = flags[0];
            SpawnedDevi = flags[1];
            MasochistModeReal = flags[2];
            CanPlayMaso = flags[3];
            ShouldBeEternityMode = flags[4];
            DownedAnyBoss = flags[5];
            WOFDroppedDeviGift2 = flags[6];
            PlacedMutantStatue = flags[7];

            for (int i = 0; i < DownedBoss.Length; i++)
            {
                int bits = i % 8;
                if (bits == 0)
                    flags = reader.ReadByte();

                DownedBoss[i] = flags[bits];
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(SkipMutantP1);

            writer.Write(new BitsByte
            {
                [0] = DownedBetsy,
                [1] = EternityMode,
                [2] = DownedFishronEX,
                [3] = DownedDevi,
                [4] = DownedAbom,
                [5] = DownedMutant,
                [6] = AngryMutant,
                [7] = HaveForcedAbomFromGoblins
            });

            writer.Write(new BitsByte
            {
                [0] = ReceivedTerraStorage,
                [1] = SpawnedDevi,
                [2] = MasochistModeReal,
                [3] = CanPlayMaso,
                [4] = ShouldBeEternityMode,
                [5] = DownedAnyBoss,
                [6] = WOFDroppedDeviGift2,
                [7] = PlacedMutantStatue
            });

            BitsByte bitsByte = new();
            for (int i = 0; i < DownedBoss.Length; i++)
            {
                int bit = i % 8;

                if (bit == 0 && i != 0)
                {
                    writer.Write(bitsByte);
                    bitsByte = new BitsByte();
                }

                bitsByte[bit] = DownedBoss[i];
            }
            writer.Write(bitsByte);
        }
    }
}
