using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class DropBasedOnEMode : IItemDropRule, INestedItemDropRule
    {
        protected readonly IItemDropRule ruleForEMode;
        protected readonly IItemDropRule ruleForDefault;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public DropBasedOnEMode(IItemDropRule ruleForEMode, IItemDropRule ruleForDefault)
        {
            this.ruleForEMode = ruleForEMode;
            this.ruleForDefault = ruleForDefault;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return FargoSoulsWorld.EternityMode ? ruleForEMode.CanDrop(info) : ruleForDefault.CanDrop(info);
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            DropRateInfoChainFeed ratesInfo1 = ratesInfo.With(1f);
            ratesInfo1.AddCondition(new EModeDropCondition());
            ruleForEMode.ReportDroprates(drops, ratesInfo1);
            DropRateInfoChainFeed ratesInfo2 = ratesInfo.With(1f);
            ratesInfo2.AddCondition(new NotEModeDropCondition());
            ruleForDefault.ReportDroprates(drops, ratesInfo2);
            Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) => new ItemDropAttemptResult()
        {
            State = ItemDropAttemptResultState.DidNotRunCode
        };

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
        {
            return FargoSoulsWorld.EternityMode ? resolveAction(ruleForEMode, info) : resolveAction(ruleForDefault, info);
        }
    }
}
