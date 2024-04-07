using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Skeleton : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2SkeletonT1,
            NPCID.DD2SkeletonT3
        );

        public int AttackTimer;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            AttackTimer = Main.rand.Next(180);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++AttackTimer > 420)
            {
                AttackTimer = 0;

                FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.ChaosBall);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);
            target.AddBuff(ModContent.BuffType<RottingBuff>(), 1200);
        }
    }
}
