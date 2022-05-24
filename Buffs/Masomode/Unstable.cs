using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Unstable : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable");
            Description.SetDefault("You don't quite fit into reality");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "不稳定");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "现实似乎在排斥你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Unstable = true;
        }
    }
}