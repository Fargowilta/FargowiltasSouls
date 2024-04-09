using FargowiltasSouls.Core.Systems;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.Core.ItemDropRules
{
    public class ChampionEnchDropRule : IItemDropRule
    {
        public readonly int[] DropIds;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public ChampionEnchDropRule(int[] drops)
        {
            DropIds = drops;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public bool CanDrop(DropAttemptInfo info) => true;

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            int max = 1;
            if (info.IsExpertMode)
                max++;
            if (WorldSavingSystem.EternityMode)
                max++;

            List<int> enchesToDrop = new(DropIds);
            while (enchesToDrop.Count > max)
                enchesToDrop.RemoveAt(info.rng.Next(enchesToDrop.Count));

            foreach (int itemType in enchesToDrop)
                CommonCode.DropItem(info, itemType, 1);

            return new ItemDropAttemptResult()
            {
                State = ItemDropAttemptResultState.Success
            };
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float personalDropRate = 1f;
            float dropRate = 1f / DropIds.Length * (personalDropRate * ratesInfo.parentDroprateChance);

            for (int index = 0; index < DropIds.Length; ++index)
                drops.Add(new DropRateInfo(DropIds[index], 1, 1, dropRate, ratesInfo.conditions));

            Chains.ReportDroprates(ChainedRules, personalDropRate, drops, ratesInfo);
        }
    }
}
