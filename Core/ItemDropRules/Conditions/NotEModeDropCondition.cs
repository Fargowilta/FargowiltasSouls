using FargowiltasSouls.Core.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class NotEModeDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && !WorldSavingSystem.EternityMode;

        public bool CanShowItemDropInUI() => !WorldSavingSystem.EternityMode;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.NotEMode");
    }
}
