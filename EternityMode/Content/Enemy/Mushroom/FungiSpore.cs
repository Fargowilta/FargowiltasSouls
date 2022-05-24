using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Mushroom
{
    public class FungiSpore : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.FungiSpore);

        public int Counter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            Counter = Main.rand.Next(30);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.dontTakeDamage = ++Counter < 60;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
