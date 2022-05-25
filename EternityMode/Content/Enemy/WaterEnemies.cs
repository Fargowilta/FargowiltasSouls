using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy
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
            NPCID.SeaTurtle
        );

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.GetGlobalNPC<EModeGlobalNPC>().isWaterEnemy = true;
        }
    }
}
