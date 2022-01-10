using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class GoldenStasisCD : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Golden Stasis Cooldown");
            //Description.SetDefault("You cannot turn gold yet");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            canBeCleared = false;
        }
    }
}