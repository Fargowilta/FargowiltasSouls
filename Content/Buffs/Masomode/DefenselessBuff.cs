using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class DefenselessBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Defenseless");
            // Description.SetDefault("Your guard is completely broken");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "毫无防御");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的防御完全崩溃了");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //-30 defense, no damage reduction, cross necklace and knockback prevention effects disabled
            player.FargoSouls().Defenseless = true;
            if (player.beetleDefense)
            {
                player.beetleOrbs = 0;
                player.beetleCounter = 0;
            }
        }
    }
}