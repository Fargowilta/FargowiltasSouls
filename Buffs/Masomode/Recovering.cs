using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Recovering : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Recovering");
            //Description.SetDefault("The Nurse cannot heal you again yet");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            canBeCleared = false;
        }
    }
}