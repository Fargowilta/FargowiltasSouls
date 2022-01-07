using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class HolyPrice : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Holy Price");
            //Description.SetDefault("Your attacks inflict 33% less damage");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().HolyPrice = true;
        }
    }
}