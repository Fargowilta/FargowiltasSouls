﻿using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.ItemDropRules.Conditions
{
    public class NotEModeDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.IsInSimulation)
                return false;

            return !FargoSoulsWorld.EternityMode;
        }

        public bool CanShowItemDropInUI()
        {
            return !FargoSoulsWorld.EternityMode;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.FargowiltasSouls.DropRate.NotEternity", ModContent.ItemType<Items.Masochist>());
        }
    }
}
