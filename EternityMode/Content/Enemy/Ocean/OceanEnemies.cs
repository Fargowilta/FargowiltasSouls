using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public class OceanEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Crab,
            NPCID.SeaSnail,
            NPCID.Squid
        );

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Starfish, 10, 1, 3));
            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.Seashell, 10, 1, 3));
        }
    }
}
