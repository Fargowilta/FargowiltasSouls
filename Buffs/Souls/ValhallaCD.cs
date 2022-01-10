using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class ValhallaCD : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Power of Cooldown");
            //Description.SetDefault("You cannot trigger Power of Squire or Power of Valhalla yet");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            canBeCleared = false;
        }
    }
}