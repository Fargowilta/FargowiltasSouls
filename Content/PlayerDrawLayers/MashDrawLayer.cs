using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    public class MashDrawLayer : PlayerDrawLayer
    {
        public override bool IsHeadLayer => false;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) =>
            drawInfo.drawPlayer.whoAmI == Main.myPlayer
            && drawInfo.drawPlayer.active
            && !drawInfo.drawPlayer.dead
            && !drawInfo.drawPlayer.ghost
            && drawInfo.shadow == 0
            && drawInfo.drawPlayer.FargoSouls().Mash;

        public override Position GetDefaultPosition() => new Between();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            Texture2D dpad = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/DPad", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = dpad.Height / 4; //ypos of lower right corner of sprite to draw
            int y3 = num156 * (int)(Main.GlobalTimeWrappedHourly % 0.5 * 8); //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, dpad.Width, num156);

            Vector2 drawPos = (drawPlayer.gravDir > 0 ? drawPlayer.Bottom : drawPlayer.Top) - Main.screenPosition;
            drawPos.Y += 48 * drawPlayer.gravDir;

            const float scale = 2f;

            DrawData data = new(dpad, drawPos, rectangle, Color.White, drawPlayer.gravDir < 0 ? MathHelper.Pi : 0f, rectangle.Size() / 2, scale, SpriteEffects.None, 0);
            drawInfo.DrawDataCache.Add(data);
        }
    }
}
