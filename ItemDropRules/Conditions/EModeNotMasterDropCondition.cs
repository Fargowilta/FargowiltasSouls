using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class EModeNotMasterDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return !info.IsMasterMode && FargoSoulsWorld.EternityMode;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "This is a Master/Eternity Mode drop rate";
        }
    }
}
