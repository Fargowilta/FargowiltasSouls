using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class EModeEarlyBirdRewardDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return FargoSoulsWorld.EternityMode && !Main.hardMode;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.FargowiltasSouls.DropRate.PHMEternity", ModContent.ItemType<Items.Masochist>());
        }
    }
}
