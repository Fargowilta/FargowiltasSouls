using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Defenseless : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Defenseless");
            //Description.SetDefault("Your guard is completely broken");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "毫无防御");
            //Description.AddTranslation(GameCulture.Chinese, "你的防御完全崩溃了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //-30 defense, no damage reduction, cross necklace and knockback prevention effects disabled
            player.GetModPlayer<FargoPlayer>().Defenseless = true;
            if (player.beetleDefense)
            {
                player.beetleOrbs = 0;
                player.beetleCounter = 0;
            }
        }
    }
}