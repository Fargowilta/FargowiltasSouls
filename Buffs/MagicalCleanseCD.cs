using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class MagicalCleanseCD : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magical Cleanse Cooldown");
            Description.SetDefault("You cannot cleanse debuffs yet");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}