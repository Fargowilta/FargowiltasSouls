using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class HuntressCD : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Arrow Rain Cooldown");
            //Description.SetDefault("You cannot trigger another rain of arrows yet");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            canBeCleared = false;
        }
    }
}