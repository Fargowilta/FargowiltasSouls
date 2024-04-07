using FargowiltasSouls.Core.NPCMatching;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class ArmoredViking : Shooters
    {
        public ArmoredViking() : base(10, ProjectileID.IceSickle, 14f, 1, -1, 450, 0, true) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ArmoredViking);
    }
}
