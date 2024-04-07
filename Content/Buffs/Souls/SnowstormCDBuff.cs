using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class SnowstormCDBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Snowstorm Cooldown");
            // Description.SetDefault("You cannot chill yet");
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}