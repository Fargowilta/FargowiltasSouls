﻿using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class InfestedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infested");
            // Description.SetDefault("This can only get worse");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "感染");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "这只会变得更糟");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer p = player.FargoSouls();

            //weak DOT that grows exponentially stronger
            if (p.FirstInfection)
            {
                p.MaxInfestTime = player.buffTime[buffIndex];
                p.FirstInfection = false;
            }

            p.Infested = true;
        }

        /*public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }*/

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().Infested = true;
        }
    }
}