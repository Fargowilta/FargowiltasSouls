﻿using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class OceanicMaulBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Oceanic Maul");
            // Description.SetDefault("Defensive stats and max life are savaged");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "海洋重击");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "降低防御力和最大生命值");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().OceanicMaul = true;
            player.bleed = true;
            player.statDefense -= 10;
            player.endurance -= 0.1f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().OceanicMaul = true;
        }
    }
}