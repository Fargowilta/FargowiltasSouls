using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hell
{
    public class FireImp : Teleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.FireImp);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(5))
            {
                npc.TargetClosest(false);
                if (npc.HasValidTarget && Main.player[npc.target].ZoneUnderworldHeight)
                    EModeGlobalNPC.Horde(npc, Main.rand.Next(8) + 1);
            }
        }
    }
}
