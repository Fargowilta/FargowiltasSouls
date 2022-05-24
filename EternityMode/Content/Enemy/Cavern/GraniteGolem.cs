using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class GraniteGolem : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GraniteGolem);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[2] < 0f) //while shielding, reflect
                EModeGlobalNPC.CustomReflect(npc, DustID.Granite, 2);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Stoned, 60);
        }
    }
}
