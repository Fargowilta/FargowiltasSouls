using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Hypothermia : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Hypothermia");
            //Description.SetDefault("Increased damage taken from cold attacks");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Hypothermia = true;
        }
    }
}