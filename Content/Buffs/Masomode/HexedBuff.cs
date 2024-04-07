﻿using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class HexedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hexed");
            // Description.SetDefault("Your attacks heal enemies");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "着魔");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你的攻击会治愈敌人");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().Hexed = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 3;
        }
    }
}