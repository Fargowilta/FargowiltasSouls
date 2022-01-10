using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class VortexCD : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Vortex Cooldown");
            //Description.SetDefault("You cannot spawn another vortex yet");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            canBeCleared = false;
        }
    }
}