using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Corruption
{
    public class Corruptor : Shooters
    {
        public Corruptor() : base(6, ProjectileID.EyeFire, 4f, 1, -1, 60, 0) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Corruptor);

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Weak, 600);
            target.AddBuff(ModContent.BuffType<RottingBuff>(), 900);
        }
    }
}
