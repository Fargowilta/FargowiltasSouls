using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Desert
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

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.OasisCrate));
        }
    }
}
