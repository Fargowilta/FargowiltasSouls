using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Midas : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Midas");
            Description.SetDefault("Drop money on hit");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "迈达斯");
            Description.AddTranslation(GameCulture.Chinese, "被攻击时掉落钱币");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Midas = true;
        }
    }
}
