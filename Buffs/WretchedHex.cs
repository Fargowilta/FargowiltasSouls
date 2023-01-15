using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class WretchedHex : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wretched Hex");
            Description.SetDefault("Shadowflame tentacles and vastly increased damage, but vastly decreased speed and defenses");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}