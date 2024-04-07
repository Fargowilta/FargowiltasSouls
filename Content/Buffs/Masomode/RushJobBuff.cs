using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class RushJobBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rush Job");
            // Description.SetDefault("The Nurse cannot heal you again yet");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (LumUtils.AnyBosses() && player.buffTime[buffIndex] < 10)
                player.buffTime[buffIndex] = 10;
        }
    }
}