using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
{
    public class Salamander : EModeNPCBehaviour
    {
        public bool WasHit;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Salamander,
            NPCID.Salamander2,
            NPCID.Salamander3,
            NPCID.Salamander4,
            NPCID.Salamander5,
            NPCID.Salamander6,
            NPCID.Salamander7,
            NPCID.Salamander8,
            NPCID.Salamander9
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.Opacity /= 5;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit)
        {
            base.OnHitByAnything(npc, player, damage, knockback, crit);

            if (!WasHit)
            {
                WasHit = true;
                npc.Opacity *= 5;
            }
        }
    }
}
