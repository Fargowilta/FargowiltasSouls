using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.SolarEclipse
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

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Obstructed, 120);
        }

        public override void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit)
        {
            base.OnHitByAnything(npc, player, damage, knockback, crit);

            Counter = 0;
        }
    }
}
