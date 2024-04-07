using FargowiltasSouls.Common.Graphics.Drawers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.PlayerDrawLayers
{
    internal class EternitySoulWingPlayerLayer : PlayerDrawLayer
    {
        // Seriously? Why is this namespace the same as a vanilla one?
        public override Position GetDefaultPosition() => new BeforeParent(Terraria.DataStructures.PlayerDrawLayers.Wings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            DrawWings(ref drawInfo);
        }

        internal static void DrawWings(ref PlayerDrawSet drawinfo)
        {
            if (EternityWingDrawer.DoNotDrawSpecialWings || drawinfo.drawPlayer.dead || drawinfo.hideEntirePlayer || drawinfo.drawPlayer.wings != EternitySoul.WingSlotID)
                return;

            Main.instance.LoadWings(EternitySoul.WingSlotID);

            Vector2 drawPosition = drawinfo.Position - Main.screenPosition + new Vector2(drawinfo.drawPlayer.width / 2,
                drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height / 2) + Vector2.UnitY * 7f;

            drawPosition += new Vector2(-9, -7) * drawinfo.drawPlayer.Directions;

            Texture2D wingTexture = TextureAssets.Wings[drawinfo.drawPlayer.wings].Value;

            int frameCount = 5;
            Rectangle frame = new(0, wingTexture.Height / frameCount * drawinfo.drawPlayer.wingFrame, wingTexture.Width, wingTexture.Height / frameCount);
            DrawData data = new(wingTexture, drawPosition.Floor(), frame, drawinfo.colorArmorBody, drawinfo.drawPlayer.bodyRotation, frame.Size() * 0.5f, 1f, drawinfo.playerEffect)
            {
                shader = drawinfo.cWings
            };
            drawinfo.DrawDataCache.Add(data);
        }
    }
}
