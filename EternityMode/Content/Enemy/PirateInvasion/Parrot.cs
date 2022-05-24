using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.PirateInvasion
{
    public class Parrot : NoclipFliers
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Parrot);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            CanNoclip = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<SqueakyToy>(), 120);
            target.AddBuff(ModContent.BuffType<Midas>(), 600);
            //target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
        }
    }
}
