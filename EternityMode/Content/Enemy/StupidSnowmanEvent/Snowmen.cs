using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.StupidSnowmanEvent
{
    public class Snowmen : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.MisterStabby,
            NPCID.SnowBalla,
            NPCID.SnowmanGangsta
        );

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<OrdinaryCarrot>(), 50));
        }
    }
}
