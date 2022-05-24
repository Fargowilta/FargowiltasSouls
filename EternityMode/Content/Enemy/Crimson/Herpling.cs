using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Crimson
{
    public class Herpling : Jungle.Derpling
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Herpling);
    }
}
