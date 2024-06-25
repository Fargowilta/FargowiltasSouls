using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI.Chat;

namespace FargowiltasSouls.Core.Toggler
{
    /// <summary>
    /// The toggle's header in the display. <para/>
    /// If the effect shouldn't have a toggle, set this to null.
    /// </summary>
    public class Toggle
    {
        public string Mod;
        public AccessoryEffect Effect;
        public Header Header => Effect.ToggleHeader;

        public string Category => Effect.ToggleHeader.SortCategory;
        public bool ToggleBool;

        public Toggle(AccessoryEffect effect, string mod)
        {
            Effect = effect;
            Mod = mod;

            ToggleBool = effect.DefaultToggle;
        }

        public override string ToString() => $"Mod: {Mod}, Category: {Category}, Effect: {Effect.Name}, Toggled: {ToggleBool}";

        public string GetRawToggleName()
        {
            string baseText = Effect.ToggleDescription;
            List<TextSnippet> parsedText = ChatManager.ParseMessage(baseText, Color.White);
            string rawText = "";

            foreach (TextSnippet snippet in parsedText)
            {
                if (!snippet.Text.StartsWith("["))
                {
                    rawText += snippet.Text.Trim();
                }
            }

            return rawText;
        }
    }
}
