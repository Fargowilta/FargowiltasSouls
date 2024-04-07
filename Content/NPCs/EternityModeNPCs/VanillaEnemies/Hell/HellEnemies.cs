using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Hell
{
    public class HellEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Hellbat,
            NPCID.LavaSlime,
            NPCID.FireImp,
            NPCID.Demon,
            NPCID.VoodooDemon,
            NPCID.BoneSerpentBody,
            NPCID.BoneSerpentHead,
            NPCID.BoneSerpentTail,
            NPCID.Lavabat,
            NPCID.RedDevil,
            NPCID.BurningSphere,
            NPCID.Lavafly,
            NPCID.MagmaSnail,
            NPCID.HellButterfly,
            NPCID.DemonTaxCollector
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.OnFire3] = true;
        }
    }
}
