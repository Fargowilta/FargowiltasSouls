using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class DownedEvilBossDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && NPC.downedBoss2;

        public bool CanShowItemDropInUI() => NPC.downedBoss2;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.DownedEvilBoss");
    }
}
