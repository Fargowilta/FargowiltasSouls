using System;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class RuntimeDropCondition : IItemDropRuleCondition
    {
        protected readonly Func<bool> condition;
        protected readonly string description;

        public RuntimeDropCondition(Func<bool> condition, string description)
        {
            this.condition = condition;
            this.description = description;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return condition();
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return description;
        }
    }
}
