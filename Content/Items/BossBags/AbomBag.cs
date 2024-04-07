using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class AbomBag : BossBag
    {
        protected override bool IsPreHMBag => false;


        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<AbomBoss>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AbomEnergy>(), 1, 15, 30));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AbomEnergy>(), 1, 15, 30));
            //itemLoot.Add(ItemDropRule.ByCondition(new PatreonDemonKingDropCondition("This is a patreon drop"), ModContent.ItemType<StaffOfUnleashedOcean>()));
        }
    }
}