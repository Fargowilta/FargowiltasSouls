using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Jungle
{
    public class DoctorBones : Shooters
    {
        public DoctorBones() : base(480, ProjectileID.Boulder, 14, 4) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DoctorBones);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
        }
    }
}
