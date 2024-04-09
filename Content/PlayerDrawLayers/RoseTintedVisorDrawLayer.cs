using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class RoseTintedVisorDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<RoseTintedVisor>() && !drawInfo.drawPlayer.dead;
        public override Position GetDefaultPosition() => new Between();
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            Vector2 visorPos = (player.gravDir > 0 ? player.Top : player.Bottom) - Main.screenPosition;

            Texture2D visorTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/RoseTintedVisorDrawLayer", AssetRequestMode.ImmediateLoad).Value;
            Rectangle visorRectangle = visorTexture.Bounds;

            SpriteEffects flip = player.direction == -player.gravDir ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            visorPos.Y += 7 * player.gravDir;
            DrawData visor = new(visorTexture, visorPos, visorRectangle, Color.White, player.gravDir < 0 ? MathHelper.Pi : 0f, visorRectangle.Size() / 2, 1f, flip, 0);
            drawInfo.DrawDataCache.Add(visor);
        }
    }
}