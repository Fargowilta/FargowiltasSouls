using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class CavernWorms : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.GiantWormHead,
            NPCID.DiggerHead
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            if (Main.rand.NextBool(4) && npc.FargoSouls().CanHordeSplit)
                EModeGlobalNPC.Horde(npc, Main.rand.Next(6) + 1);
        }
    }
}
