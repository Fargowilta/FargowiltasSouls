using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class BrokenShell : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Broken Shell");
            //Description.SetDefault("You cannot enter your shell yet");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            canBeCleared = false;
        }
    }
}