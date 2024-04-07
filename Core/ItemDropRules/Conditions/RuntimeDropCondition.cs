using System;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class RuntimeDropCondition : IItemDropRuleCondition
    {
        protected readonly Func<bool> Condition;
        protected readonly string Description;

        public RuntimeDropCondition(Func<bool> condition, string description)
        {
            Condition = condition;
            Description = description;
        }

        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && Condition();

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Description;
    }
}
