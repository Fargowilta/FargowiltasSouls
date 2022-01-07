using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Flipped : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Flipped");
            //Description.SetDefault("Your gravity is reversed");
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "翻转");
            //Description.AddTranslation(GameCulture.Chinese, "你的重力颠倒了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Flipped = true;
        }
    }
}