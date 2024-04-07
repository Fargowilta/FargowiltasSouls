using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class EModeEarlyBirdLockDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && (Main.hardMode || !WorldSavingSystem.EternityMode);

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.EModeEarlyBirdHM");
    }
}
