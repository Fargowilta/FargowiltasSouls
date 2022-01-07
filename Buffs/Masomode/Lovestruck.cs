using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Lovestruck : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Lovestruck");
            //Description.SetDefault("You are in love!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "热恋");
            //Description.AddTranslation(GameCulture.Chinese, "坠入爱河!");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.loveStruck = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 2;
        }
    }
}