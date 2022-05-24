using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hell
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
            NPCID.HellButterfly
        );

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.OnFire3] = true;
        }
    }
}
