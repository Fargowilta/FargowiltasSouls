﻿using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class CoffinTossBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.releaseHook = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
            player.FargoSouls().Stunned = true;
            player.FargoSouls().NoUsingItems = 2;

            player.velocity = Vector2.Normalize(player.velocity) * 30;
            player.fullRotation = player.velocity.ToRotation() + MathHelper.PiOver2;
            player.fullRotationOrigin = player.Center - player.position;

            if (player.buffTime[buffIndex] < 2) // make sure you get unrotated
            {
                player.fullRotation = 0;
                player.DelBuff(buffIndex);
            }
                

            if (Collision.SolidCollision(player.position + player.velocity, player.width, player.height))
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.CoffinToss", player.name)), 35, 0, false, false, 0, false);
                player.DelBuff(buffIndex);
                SoundEngine.PlaySound(SoundID.NPCHit18, player.Center);
                player.velocity *= -1;
                player.fullRotation = 0;
            }
        }
    }
}