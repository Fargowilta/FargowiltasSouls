using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Martians
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

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Confused] = true;
            npc.buffImmune[BuffID.Electrified] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Electrified, 300);
        }
    }
}
