using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class FargoUIHoverTextImageButton : UIImageButton
    {
        public string Text;

        public FargoUIHoverTextImageButton(Asset<Texture2D> texture, string text) : base(texture)
        {
            Text = text;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = Text;
            }
        }
    }
}
