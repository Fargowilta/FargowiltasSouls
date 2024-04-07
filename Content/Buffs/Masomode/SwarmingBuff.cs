﻿using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class SwarmingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Swarming");
            // Description.SetDefault("Hornets are attacking from every direction!");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "蜂群");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "黄蜂正从四面八方向你发起进攻!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Swarming = true;
        }
    }
}