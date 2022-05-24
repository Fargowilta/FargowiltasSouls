using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Snow
{
    public class IceTortoise : Jungle.GiantTortoise
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IceTortoise);

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            float reduction = (float)npc.life / npc.lifeMax;
            if (reduction < 0.5f)
                reduction = 0.5f;
            damage = (int)(damage * reduction);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Frozen, 60);
        }
    }
}
