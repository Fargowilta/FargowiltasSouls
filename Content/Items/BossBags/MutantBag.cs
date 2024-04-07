using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class MutantBag : BossBag
    {
        protected override bool IsPreHMBag => false;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Masochist>()));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<MutantBoss>()));
            itemLoot.Add(ItemDropRule.ByCondition(new EModeDropCondition(), ModContent.ItemType<EternalEnergy>(), 1, 15, 20));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MutantsFury>()));
        }
    }
}