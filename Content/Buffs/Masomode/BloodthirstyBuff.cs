﻿using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class BloodthirstyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bloodthirsty");
            // Description.SetDefault("Hugely increased enemy spawn rate");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "嗜血");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "极大提高刷怪速率");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //crazy spawn rate
            player.FargoSouls().Bloodthirsty = true;
        }
    }
}