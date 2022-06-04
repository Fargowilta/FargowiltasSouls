using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class NotEModeDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return !FargoSoulsWorld.EternityMode;
        }

        public bool CanShowItemDropInUI()
        {
            return !FargoSoulsWorld.EternityMode;
        }

        public string GetConditionDescription()
        {
            return $"[i:{ModContent.ItemType<Items.Masochist>()}]Non-Eternity Mode drop rate";
        }
    }
}
