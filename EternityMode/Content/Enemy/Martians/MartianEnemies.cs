using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Martians
{
    public class MartianEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.ScutlixRider,
            NPCID.GigaZapper,
            NPCID.MartianEngineer,
            NPCID.MartianOfficer,
            NPCID.RayGunner,
            NPCID.GrayGrunt,
            NPCID.BrainScrambler,
            NPCID.MartianDrone,
            NPCID.MartianWalker,
            NPCID.MartianTurret,
            NPCID.Scutlix,
            NPCID.ScutlixRider,
            NPCID.MartianSaucer,
            NPCID.MartianSaucerCannon,
            NPCID.MartianSaucerCore,
            NPCID.MartianSaucerTurret
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.Confused] = true;
            npc.buffImmune[BuffID.Electrified] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Electrified, 300);
        }
    }
}
