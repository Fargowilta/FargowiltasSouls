using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Corruption
{
    public class Corruptor : Shooters
    {
        public Corruptor() : base(6, ProjectileID.EyeFire, 4f, 1, -1, 60, 0) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Corruptor);

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Weak, 600);
            target.AddBuff(ModContent.BuffType<Rotting>(), 900);
        }
    }
}
