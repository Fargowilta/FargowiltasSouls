using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class ChampionEnchDropRule : IItemDropRule
    {
        public int[] dropIds;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public ChampionEnchDropRule(int[] drops)
        {
            //Recipe recipe = Main.recipe.First(r => r.HasResult(forceType));

            //List<int> enches = new List<int>();
            //foreach (Item material in recipe.requiredItem)
            //{
            //    if (material.Name.EndsWith("Enchantment"))
            //        enches.Add(material.type);
            //}
            //dropIds = enches.ToArray();

            dropIds = drops;

            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public bool CanDrop(DropAttemptInfo info) => true;

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            int max = 1;
            if (info.IsExpertMode)
                max++;
            if (FargoSoulsWorld.EternityMode)
                max++;

            List<int> enchesToDrop = new List<int>(dropIds);
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
            float dropRate = 1f / dropIds.Length * (personalDropRate * ratesInfo.parentDroprateChance);
            for (int index = 0; index < dropIds.Length; ++index)
                drops.Add(new DropRateInfo(dropIds[index], 1, 1, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, personalDropRate, drops, ratesInfo);
        }
    }
}
