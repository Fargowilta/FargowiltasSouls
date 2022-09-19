using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class BetsyDash : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dash");
            Description.SetDefault("You're moving at high speed!");
            //Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            /*player.controlLeft = false;
            player.controlRight = false;*/
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.controlMount = false;

            //player.immune = true;
            //player.immuneTime = Math.Max(player.immuneTime, 2);
            //player.hurtCooldowns[0] = Math.Max(player.hurtCooldowns[0], 2);
            //player.hurtCooldowns[1] = Math.Max(player.hurtCooldowns[1], 2);
        }
    }
}