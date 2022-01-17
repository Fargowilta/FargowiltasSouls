using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.Localization;

namespace FargowiltasSouls.UI
{
    public class SoulTogglerButton : UIState
    {
        public UIImage Icon;
        public UIHoverTextImageButton IconHighlight;

        public override void OnActivate()
        {
            Icon = new UIImage(Fargowiltas.UserInterfaceManager.SoulTogglerButtonTexture);
            Icon.Left.Set(26, 0f);
            Icon.Top.Set(300, 0f);
            Append(Icon);

            IconHighlight = new UIHoverTextImageButton(Fargowiltas.UserInterfaceManager.SoulTogglerButton_MouseOverTexture, Language.GetTextValue("Mods.FargowiltasSouls.UISoulTogglerButton.HoverText"));//"Configure Accessory Effects"
            IconHighlight.Left.Set(-2, 0f);
            IconHighlight.Top.Set(-2, 0f);
            IconHighlight.SetVisibility(1f, 0f);
            IconHighlight.OnClick += IconHighlight_OnClick;
            Icon.Append(IconHighlight);

            base.OnActivate();
        }

        private void IconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.playerInventory)
            {
                return;
            }

            Fargowiltas.UserInterfaceManager.ToggleSoulToggler();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.playerInventory)
                base.Draw(spriteBatch);
        }
    }
}
