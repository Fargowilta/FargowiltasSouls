using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class AntlionCharger : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WalkingAntlion);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.knockBackResist /= 2f;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.FargoSouls().AddBuffNoStack(BuffID.Dazed, 60);
            target.AddBuff(BuffID.Slow, 120);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);


        }
    }
}
