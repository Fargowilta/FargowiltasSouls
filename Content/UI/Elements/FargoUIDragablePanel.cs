using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI.Elements
{
    // From ExampleMod
    public class FargoUIDragablePanel : UIPanel
    {
        // Stores the offset from the top left of the UIPanel while dragging.
        private Vector2 offset;
        public bool dragging;
        public UIElement[] ExtraChildren;

        public FargoUIDragablePanel() { }
        public FargoUIDragablePanel(params UIElement[] countMeAsChildren)
        {
            ExtraChildren = countMeAsChildren;
        }

        private void DragStart(Vector2 pos)
        {
            offset = new Vector2(pos.X - Left.Pixels, pos.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(Vector2 pos)
        {
            Vector2 end = pos;
            dragging = false;

            Left.Set(end.X - offset.X, 0);
            Top.Set(end.Y - offset.Y, 0);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.

            // Checking ContainsPoint and then setting mouseInterface to true is very common. This causes clicks on this UIElement to not cause the player to use current items. 
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (!dragging && ContainsPoint(Main.MouseScreen) && Main.mouseLeft && PlayerInput.MouseInfoOld.LeftButton == ButtonState.Released)
            {
                bool upperMost = true;
                if (ExtraChildren != null)
                {
                    IEnumerable<UIElement> children = Elements.Concat(ExtraChildren);

                    foreach (UIElement element in children)
                    {
                        if (element.ContainsPoint(Main.MouseScreen) && element as UIPanel == null)
                        {
                            upperMost = false;
                            break;
                        }
                    }
                }

                if (upperMost)
                    DragStart(Main.MouseScreen);
            }
            else if (dragging && !Main.mouseLeft)
            {
                DragEnd(Main.MouseScreen);
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0); // Main.MouseScreen.X and Main.mouseX are the same.
                Top.Set(Main.mouseY - offset.Y, 0);
                Recalculate();
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle. 
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate forces the UI system to do the positioning math again.
                Recalculate();
            }
        }
    }
}
