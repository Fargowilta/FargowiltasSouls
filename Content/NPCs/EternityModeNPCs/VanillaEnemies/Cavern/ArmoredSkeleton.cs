using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class ArmoredSkeleton : Shooters
    {
        public ArmoredSkeleton() : base(300, ProjectileID.SwordBeam, 10, 1, DustID.AmberBolt, 500) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ArmoredSkeleton);
    }
}
