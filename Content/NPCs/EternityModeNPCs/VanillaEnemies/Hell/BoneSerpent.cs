using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hell
{
    public class BoneSerpent : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.BoneSerpentHead);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 300 && Counter % 20 == 0)
            {
                int t = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (t != -1 && npc.Distance(Main.player[t].Center) < 800 && FargoSoulsUtil.HostCheck)
                    FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.BurningSphere);
            }

            if (Counter > 420)
                Counter = 0;
        }
    }
}
