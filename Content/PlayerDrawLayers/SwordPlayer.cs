using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class SwordDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return SwordGlobalItem.IsBroadsword(drawInfo.drawPlayer.HeldItem) && drawInfo.drawPlayer.ItemAnimationActive;
        }
        public override Position GetDefaultPosition() => new Between(Terraria.DataStructures.PlayerDrawLayers.SolarShield, Terraria.DataStructures.PlayerDrawLayers.ArmOverItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Asset<Texture2D> t = TextureAssets.Item[drawInfo.drawPlayer.HeldItem.type];

            FargoSoulsPlayer player = drawInfo.drawPlayer.FargoSouls();
            Vector2 position = drawInfo.drawPlayer.itemLocation - Main.screenPosition;

            int dir = drawInfo.drawPlayer.direction;
            int swingDir = player.swingDirection;

            SpriteEffects effects = SpriteEffects.None;
            float rotationAdd = 0;
            Vector2 origin = new Vector2(2, t.Height() - 2);
            if (dir == -1 && swingDir == 1 || swingDir == -1 && dir == 1)
            {

                effects = SpriteEffects.FlipHorizontally;
                origin = new Vector2(t.Width() - 2, t.Height() - 2);
            }
            if (swingDir == -1)
            {
                rotationAdd += MathHelper.ToRadians(90 * dir);

            }


            drawInfo.DrawDataCache.Add(new DrawData(
                t.Value,
                position,
                null,
                Lighting.GetColor(drawInfo.drawPlayer.itemLocation.ToTileCoordinates()),
                drawInfo.drawPlayer.itemRotation + rotationAdd,
                origin,
                drawInfo.drawPlayer.GetAdjustedItemScale(drawInfo.heldItem),
                effects,
                0


                )
            );

        }
    }
}
