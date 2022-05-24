using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class EModeEarlyBirdLockDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return Main.hardMode || !FargoSoulsWorld.EternityMode;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return $"[i:{ModContent.ItemType<Items.Masochist>()}]Hardmode Eternity Mode drop rate";
        }
    }
}
