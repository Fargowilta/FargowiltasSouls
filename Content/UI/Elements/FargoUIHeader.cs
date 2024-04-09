using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    public class FargoUIHeader : UIElement
    {
        public const int TextureTextPadding = 8;
        public const int TextureBarPadding = 4;
        public const int ItemTextureDimensions = 32;

        readonly string Text;
        readonly string Mod;
        readonly int Item;

        public FargoUIHeader(string text, string mod, int item, (int width, int height) dimensions)
        {
            Text = text;
            Mod = mod;
            Item = item;
            Width.Set(dimensions.width, 0);
            Height.Set(dimensions.height, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = new(dimensions.X, dimensions.Y);
            // I honestly forget why I did half of these calculations at this point
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + 2, /*Y*/ (int)dimensions.Y + 22 - 1 + 0, (int)dimensions.Width - 2, 1), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + 2, /*Y*/ (int)dimensions.Y + 22 - 1 + 1, (int)dimensions.Width - 2, 1), Color.LightGray);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + 2, /*Y*/ (int)dimensions.Y + 22 - 1 + 2, (int)dimensions.Width - 2, 1), Color.Gray);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + 2, /*Y*/ (int)dimensions.Y + 22 - 1 + 3, (int)dimensions.Width - 2, 1), Color.Black);

            // "Caps" at the end of the line
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + 1, /*Y*/ (int)dimensions.Y + 22 - 1 + 1, 1, 2), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + (int)dimensions.Width, /*Y*/ (int)dimensions.Y + 22 - 1 + 1, 1, 2), Color.Black);

            Utils.DrawBorderString(spriteBatch, Text, position, Color.White);
        }
    }
}
