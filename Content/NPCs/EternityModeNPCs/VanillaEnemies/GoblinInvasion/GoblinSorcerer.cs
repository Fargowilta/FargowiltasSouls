using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.GoblinInvasion
{
    public class GoblinSorcerer : Teleporters
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GoblinSorcerer);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (!Main.hardMode && !NPC.downedSlimeKing && !NPC.downedBoss1 && NPC.CountNPCS(npc.type) > 2)
                npc.Transform(NPCID.GoblinPeon);
        }
    }
}
