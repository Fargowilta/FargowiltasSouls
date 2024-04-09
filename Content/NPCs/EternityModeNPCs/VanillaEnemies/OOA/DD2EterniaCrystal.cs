using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2EterniaCrystal : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2EterniaCrystal);

        public int InvulTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //if (DD2Event.Ongoing && DD2Event.TimeLeftBetweenWaves > 600) DD2Event.TimeLeftBetweenWaves = 600;

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy) && EModeGlobalNPC.betsyBoss >= 0 && EModeGlobalNPC.betsyBoss != Main.maxNPCs)
            {
                if (Main.player.Any(p => p.active && !p.dead)) //if any alive players
                {
                    InvulTimer = 15; //wait before becoming fully vulnerable
                    if (npc.life < npc.lifeMax && npc.life < 500)
                        npc.life++;
                }
            }

            if (InvulTimer > 0)
            {
                npc.chaseable = false; //this doesn't fucking work thank you redigit
                InvulTimer--;
            }
            else
            {
                npc.chaseable = true;
            }

        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (InvulTimer > 0)
                modifiers.Null();

            base.ModifyIncomingHit(npc, ref modifiers);
        }
    }
}
