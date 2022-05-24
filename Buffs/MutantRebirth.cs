using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class MutantRebirth : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Rebirth");
            Description.SetDefault("Deathray revive is recharging");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变重生");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "死光复苏蓄能中");
            Main.debuff[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}