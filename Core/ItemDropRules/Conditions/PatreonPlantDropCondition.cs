using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.Core.ItemDropRules.Conditions
{
    public class PatreonPlantDropCondition : IItemDropRuleCondition
    {
        protected readonly string Description;

        public PatreonPlantDropCondition(string description) => Description = description;

        public bool CanDrop(DropAttemptInfo info) => !info.IsInSimulation && SoulConfig.Instance.PatreonPlant &&
            info.npc.lastInteraction != -1 && Main.player[info.npc.lastInteraction].ZoneJungle;

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Description;
    }
}
