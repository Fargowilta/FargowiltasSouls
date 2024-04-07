using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class CrippledBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crippled");
            // Description.SetDefault("You cannot run");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "残废");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "不能奔跑");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //disables running :v
            player.FargoSouls().Kneecapped = true;
            player.slow = true;
        }
    }
}