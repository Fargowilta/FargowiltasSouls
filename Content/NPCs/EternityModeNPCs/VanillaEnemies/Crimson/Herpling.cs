using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Crimson
{
    public class Herpling : Jungle.Derpling
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Herpling);
    }
}
