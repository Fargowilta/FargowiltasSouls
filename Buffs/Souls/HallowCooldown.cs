using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class HallowCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Hallowed Shield Cooldown");
            //Description.SetDefault("Your shield cannot reflect projectiles yet");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderDebuff";

            return true;
        }
    }
}