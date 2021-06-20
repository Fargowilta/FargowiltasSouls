using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Swarming : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Swarming");
            Description.SetDefault("Hornets are attacking from every direction!");
            DisplayName.AddTranslation(GameCulture.Chinese, "蜂群");
            Description.AddTranslation(GameCulture.Chinese, "黄蜂正从四面八方向你发起进攻！");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Swarming = true;
        }
    }
}
