using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class EModeEarlyBirdRewardDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return FargoSoulsWorld.EternityMode && !Main.hardMode;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return $"[i:{ModContent.ItemType<Items.Masochist>()}]Pre-Hardmode Eternity Mode drop rate";
        }
    }
}
