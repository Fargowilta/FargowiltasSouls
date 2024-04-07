using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class WaterEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.BlueJellyfish,
            NPCID.Crab,
            NPCID.PinkJellyfish,
            NPCID.Piranha,
            NPCID.SeaSnail,
            NPCID.Shark,
            NPCID.Squid,
            NPCID.AnglerFish,
            NPCID.Arapaima,
            NPCID.BloodFeeder,
            NPCID.BloodJelly,
            NPCID.FungoFish,
            NPCID.GreenJellyfish,
            NPCID.Goldfish,
            NPCID.CorruptGoldfish,
            NPCID.CrimsonGoldfish,
            NPCID.WaterSphere,
            NPCID.Frog,
            NPCID.GoldFrog,
            NPCID.Grubby,
            NPCID.Sluggy,
            NPCID.Buggy,
            NPCID.Turtle,
            NPCID.TurtleJungle,
            NPCID.SeaTurtle,
            NPCID.Duck,
            NPCID.Duck2,
            NPCID.DuckWhite,
            NPCID.DuckWhite2,
            NPCID.Grebe,
            NPCID.Grebe2,
            NPCID.Dolphin,
            NPCID.Seagull,
            NPCID.Seagull2
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.Eternity().isWaterEnemy = true;
        }
    }
}
