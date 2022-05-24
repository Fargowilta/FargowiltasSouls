using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Shadowflame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowflame");
            Description.SetDefault("Losing life");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暗影烈焰");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "流失生命");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Shadowflame = true;
        }
    }
}