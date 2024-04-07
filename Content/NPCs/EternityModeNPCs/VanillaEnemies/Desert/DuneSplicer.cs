using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Desert
{
    public class DuneSplicer : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DuneSplicerBody,
            NPCID.DuneSplicerHead,
            NPCID.DuneSplicerTail
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (Main.hardMode)
            {
                npc.lifeMax *= 3;
            }
            else
            {
                npc.defense /= 2;
                npc.damage /= 2;
            }
        }

        public override bool SafePreAI(NPC npc)
        {
            int p = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
            if (p != -1 && npc.Distance(Main.player[p].Center) < 2400)
            {
                Main.player[p].ZoneUndergroundDesert = true; //always attack them
            }

            return base.SafePreAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<ClippedWingsBuff>(), 300);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);


        }
    }
}
