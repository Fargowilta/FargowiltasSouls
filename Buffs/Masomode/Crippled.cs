using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Crippled : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Crippled");
            //Description.SetDefault("You cannot run");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "残废");
            //Description.AddTranslation(GameCulture.Chinese, "不能奔跑");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //disables running :v
            player.GetModPlayer<FargoPlayer>().Kneecapped = true;
            player.slow = true;
        }
    }
}