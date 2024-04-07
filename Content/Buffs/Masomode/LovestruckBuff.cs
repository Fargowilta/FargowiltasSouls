using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class LovestruckBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lovestruck");
            // Description.SetDefault("You are in love!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "热恋");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "坠入爱河!");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.loveStruck = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 3;
        }
    }
}