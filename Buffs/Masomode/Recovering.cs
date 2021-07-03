using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Recovering : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Recovering");
            Description.SetDefault("The Nurse cannot heal you again yet");
            DisplayName.AddTranslation(GameCulture.Chinese, "恢复中");
            Description.AddTranslation(GameCulture.Chinese, "护士不能再次治疗你");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            canBeCleared = false;
        }
    }
}
