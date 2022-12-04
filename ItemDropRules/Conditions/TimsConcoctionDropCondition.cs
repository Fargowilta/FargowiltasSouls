using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class TimsConcoctionDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return info.npc.lastInteraction != -1 && Main.player[info.npc.lastInteraction].GetModPlayer<FargoSoulsPlayer>().TimsConcoction;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.FargowiltasSouls.DropRate.TimAcc", ModContent.ItemType<Items.Accessories.Masomode.TimsConcoction>());
        }
    }
}
