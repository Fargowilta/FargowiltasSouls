using FargowiltasSouls.Content.Buffs.Mounts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Mounts
{
    public class TrojanSquirrelMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            //MountData.spawnDust = mod.DustType("Smoke");
            MountData.buff = ModContent.BuffType<TrojanSquirrelMountBuff>();
            MountData.heightBoost = 20;
            MountData.fallDamage = 0f;
            MountData.runSpeed = 15f;
            MountData.dashSpeed = 8f;
            MountData.flightTimeMax = 0;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 50;
            MountData.acceleration = 0.2f;
            MountData.jumpSpeed = 15f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 6;
            MountData.constantJump = true;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 125;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = 0;
            MountData.yOffset = -84;
            MountData.bodyFrame = 0;

            //MountData.playerHeadOffset = -200; useless?
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 1;
            MountData.runningFrameCount = 6;
            MountData.runningFrameDelay = 25;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 5;
            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 1;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            MountData.textureWidth = MountData.backTexture.Width();
            MountData.textureHeight = MountData.backTexture.Height();
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {

            drawColor = new Color(255, 255, 255, 255);

            return base.Draw(playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
        }
    }
}