using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Mushroom
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

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}
