using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class PatreonPlantDropCondition : IItemDropRuleCondition
    {
        protected readonly string description;

        public PatreonPlantDropCondition(string description)
        {
            this.description = description;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return SoulConfig.Instance.PatreonPlant && info.npc.lastInteraction != -1 && Main.player[info.npc.lastInteraction].ZoneJungle;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return description;
        }
    }
}
