using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class FargoUIReloadButton : UIElement
    {
        public Texture2D Texture;
        public Func<string> Text; //Needs to be a Func<string> to make it work with localization. Language.GetTextValue does not work correctly on initialize.

        public FargoUIReloadButton(Texture2D tex, Func<string> text)
        {
            Texture = tex;
            Text = text;

            Width.Set(20, 0);
            Height.Set(20, 0);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle style = GetDimensions();
            bool hovered = false;
            // Logic
            if (IsMouseHovering)
            {
                Vector2 textPosition = style.Position() + new Vector2(0, style.Height + 8);
                Utils.DrawBorderString(spriteBatch, Text.Invoke(), textPosition, Color.White);
                hovered = true;
            }

            // Drawing
            Texture2D outlineTexture = FargoUIManager.PresetButtonOutline.Value;
            Vector2 position = style.Position();
            spriteBatch.Draw(outlineTexture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            position += new Vector2(2);
            Rectangle frame = new(0, 0, 20, 20);
            if (hovered)
                frame.X += 20;
            spriteBatch.Draw(Texture, position, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
