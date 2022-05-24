using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace FargowiltasSouls.UI
{
    public class UIToggle : UIElement
    {
        public const int CheckboxTextSpace = 4;

        public DynamicSpriteFont Font => Terraria.GameContent.FontAssets.ItemStack.Value;

        public string Key;

        public UIToggle(string key)
        {
            Key = key;

            Width.Set(19, 0);
            Height.Set(21, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().Position();

            if (IsMouseHovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Player player = Main.LocalPlayer;
                FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                modPlayer.Toggler.Toggles[Key].ToggleBool = !modPlayer.Toggler.Toggles[Key].ToggleBool;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    modPlayer.SyncToggle(Key);
            }

            spriteBatch.Draw(FargowiltasSouls.UserInterfaceManager.CheckBox.Value, position, Color.White);
            if (Main.LocalPlayer.GetToggleValue(Key, false))
                spriteBatch.Draw(FargowiltasSouls.UserInterfaceManager.CheckMark.Value, position, Color.White);

            string text = Language.GetTextValue($"Mods.FargowiltasSouls.{Key}Config");
            position += new Vector2(Width.Pixels * Main.UIScale, 0);
            position += new Vector2(CheckboxTextSpace, 0);
            position += new Vector2(0, Font.MeasureString(text).Y * 0.175f);

            Utils.DrawBorderString(spriteBatch, text, position, Color.White);
        }
    }
}
