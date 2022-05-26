using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class RushJob : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Description.SetDefault("The Nurse cannot heal you again yet");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (FargoSoulsUtil.AnyBossAlive() && player.buffTime[buffIndex] < 10)
                player.buffTime[buffIndex] = 10;
        }
    }
}