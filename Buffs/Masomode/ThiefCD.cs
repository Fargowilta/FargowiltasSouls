using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class ThiefCD : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Thief Cooldown");
            Description.SetDefault("Your items cannot be stolen again yet");
            DisplayName.AddTranslation(GameCulture.Chinese, "盗窃冷却");
            Description.AddTranslation(GameCulture.Chinese, "你的物品暂时不会再被偷");
            Main.buffNoSave[Type] = true;
        }
    }
}
