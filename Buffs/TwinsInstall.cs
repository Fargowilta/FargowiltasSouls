using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class TwinsInstall : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twins Install");
            Description.SetDefault("Effects of Cursed Inferno and Ichor");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.onFire2 = true;
            player.ichor = true;
        }
    }
}