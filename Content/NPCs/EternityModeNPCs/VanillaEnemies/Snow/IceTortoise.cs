using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Snow
{
    public class IceTortoise : Jungle.GiantTortoise
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IceTortoise);

        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByAnything(npc, player, ref modifiers);

            float reduction = (float)npc.life / npc.lifeMax;
            if (reduction < 0.5f)
                reduction = 0.5f;
            modifiers.FinalDamage *= reduction;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.FargoSouls().AddBuffNoStack(BuffID.Frozen, 60);
        }
    }
}
