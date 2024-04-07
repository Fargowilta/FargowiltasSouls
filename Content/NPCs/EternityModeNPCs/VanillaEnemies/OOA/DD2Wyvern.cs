using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Wyvern : Shooters
    {
        public DD2Wyvern() : base(300, ProjectileID.Fireball, 6f) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2WyvernT1,
            NPCID.DD2WyvernT2,
            NPCID.DD2WyvernT3
        );

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<MutantNibbleBuff>(), 300);
            target.AddBuff(BuffID.Rabies, 3600);
        }
    }
}
