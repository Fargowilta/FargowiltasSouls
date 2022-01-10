using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class SquireBuff : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Power of Squire");
            //Description.SetDefault("Removing some enemy immunity frames");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().squireReduceIframes = true;
        }
    }
}