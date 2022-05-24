using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.OOA
{
    public class DD2WitherBeast : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2WitherBeastT2,
            NPCID.DD2WitherBeastT3
        );

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 300, BuffID.WitheredArmor, false, 119);
            EModeGlobalNPC.Aura(npc, 300, BuffID.WitheredWeapon, false, 14);
        }
    }
}
