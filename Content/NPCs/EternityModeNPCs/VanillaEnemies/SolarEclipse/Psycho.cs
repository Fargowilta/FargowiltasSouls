using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse
{
    public class Psycho : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Psycho);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //alpha is controlled by vanilla ai so npc is necessary
            if (Counter < 200)
                Counter += 2;
            if (npc.alpha < Counter)
                npc.alpha = Counter;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Obstructed, 120);
        }

        public override void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByAnything(npc, player, hit, damageDone);

            Counter = 0;
        }
    }
}
