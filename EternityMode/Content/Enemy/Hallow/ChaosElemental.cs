using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hallow
{
    public class ChaosElemental : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ChaosElemental);

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Confused] = true;

            if (Main.rand.NextBool(3))
                EModeGlobalNPC.Horde(npc, Main.rand.Next(3, 10));
        }


        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Unstable>(), 240);
        }
    }
}
