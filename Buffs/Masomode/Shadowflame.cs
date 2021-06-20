using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Shadowflame : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Shadowflame");
            Description.SetDefault("Losing life");
            DisplayName.AddTranslation(GameCulture.Chinese, "暗影焰");
            Description.AddTranslation(GameCulture.Chinese, "正在损失生命值");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Shadowflame = true;
        }
    }
}
