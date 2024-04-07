using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class AnglerFish : EModeNPCBehaviour
    {
        public bool WasHit;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.AnglerFish);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.Opacity /= 5;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!WasHit)
                Lighting.AddLight(npc.Center, 0.1f, 0.5f, 0.5f);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Bleeding, 300);
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
