using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.StupidSnowmanEvent
{
    public class MisterStabby : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MisterStabby);

        public bool WasHit;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.Opacity /= 5;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!WasHit)
            {
                npc.position.X += npc.velocity.X / 2;
            }
        }

        public override void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit)
        {
            base.OnHitByAnything(npc, player, damage, knockback, crit);

            if (!WasHit)
            {
                WasHit = true;
                npc.Opacity *= 5;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300);
            target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
        }
    }
}
