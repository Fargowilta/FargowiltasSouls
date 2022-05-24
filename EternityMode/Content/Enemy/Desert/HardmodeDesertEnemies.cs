using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Desert
{
    public class HardmodeDesertEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DesertBeast,
            NPCID.DesertScorpionWalk,
            NPCID.DesertScorpionWall,
            NPCID.DesertLamiaDark,
            NPCID.DesertLamiaLight,
            NPCID.DesertGhoul,
            NPCID.DesertGhoulCorruption,
            NPCID.DesertGhoulCrimson,
            NPCID.DesertGhoulHallow,
            NPCID.DesertDjinn
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.buffImmune[BuffID.Slow] = true;
            npc.buffImmune[BuffID.Weak] = true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.DesertFossil, 3, 1, 10));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.FlyingCarpet, 100));
        }
    }
}
