using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.Systems;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.Core.ItemDropRules
{
    public class DropBasedOnEMode : IItemDropRule, INestedItemDropRule
    {
        protected readonly IItemDropRule RuleForEMode;
        protected readonly IItemDropRule RuleForDefault;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public DropBasedOnEMode(IItemDropRule ruleForEMode, IItemDropRule ruleForDefault)
        {
            RuleForEMode = ruleForEMode;
            RuleForDefault = ruleForDefault;
            ChainedRules = new();
        }

        public bool CanDrop(DropAttemptInfo info) => WorldSavingSystem.EternityMode ? RuleForEMode.CanDrop(info) : RuleForDefault.CanDrop(info);

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            DropRateInfoChainFeed ratesInfo1 = ratesInfo.With(1f);
            ratesInfo1.AddCondition(new EModeDropCondition());
            RuleForEMode.ReportDroprates(drops, ratesInfo1);
            DropRateInfoChainFeed ratesInfo2 = ratesInfo.With(1f);
            ratesInfo2.AddCondition(new NotEModeDropCondition());
            RuleForDefault.ReportDroprates(drops, ratesInfo2);
            Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) => new()
        {
            State = ItemDropAttemptResultState.DidNotRunCode
        };

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction) => WorldSavingSystem.EternityMode ? resolveAction(RuleForEMode, info) :
            resolveAction(RuleForDefault, info);
    }
}
