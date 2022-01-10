﻿using FargowiltasSouls.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Mounts
{
    public class TrojanSquirrelMountBuff : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Trojan Squirrel");
            //Description.SetDefault("pog");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<TrojanSquirrelMount>(), player);
            player.buffTime[buffIndex] = 10;

            player.GetModPlayer<FargoPlayer>().SquirrelMount = true;
        }
    }
}
