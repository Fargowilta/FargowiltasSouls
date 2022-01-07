using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class ThiefCD : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Thief Cooldown");
            //Description.SetDefault("Your items cannot be stolen again yet");
            Main.buffNoSave[Type] = true;
        }
    }
}