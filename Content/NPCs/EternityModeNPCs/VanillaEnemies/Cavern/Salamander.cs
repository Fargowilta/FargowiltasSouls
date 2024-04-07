using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
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

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);

            if (!WasHit)
            {
                WasHit = true;
                npc.Opacity *= 5;
            }
        }
    }
}
