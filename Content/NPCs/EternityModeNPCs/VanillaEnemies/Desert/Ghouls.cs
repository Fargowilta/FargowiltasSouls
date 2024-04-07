using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class Ghouls : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DesertGhoul,
            NPCID.DesertGhoulCorruption,
            NPCID.DesertGhoulCrimson,
            NPCID.DesertGhoulHallow
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(4))
                npc.Transform(Main.rand.Next(NPCID.DesertGhoulCorruption, NPCID.DesertGhoulHallow + 1));
        }
    }
}
