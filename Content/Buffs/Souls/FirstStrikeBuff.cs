﻿using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class FirstStrikeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("First Strike");
            // Description.SetDefault("Your next attack will be enhanced");
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "先发制人");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的下一次攻击将会得到增强");
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex]++;
            player.FargoSouls().FirstStrike = true;

            player.shroomiteStealth = true;
            player.stealth = .2f;
            player.stealthTimer = 0;
            player.aggro -= 1200;
        }
    }
}