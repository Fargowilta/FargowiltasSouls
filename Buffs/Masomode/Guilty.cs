using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Guilty : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guilty");
            Description.SetDefault("Weapons dulled by the guilt of slaying innocent critters");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "内疚");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "杀害无辜动物的内疚使你的武器变得迟钝");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) -= 0.25f;
        }
    }
}