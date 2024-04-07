using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class TimsConcoctionDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && info.npc.lastInteraction != -1 && Main.player[info.npc.lastInteraction].FargoSouls().TimsConcoction;
        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.TimsConcoction");
    }
}
