using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace FargowiltasSouls.Core.Toggler
{
    public abstract class Header : ModType
    {

        public abstract string SortCategory { get; }
        /// <summary>
        /// Dictates ordering of Headers in the toggle display. <para/>
        /// Lower number -> header appears higher up.
        /// </summary>
        public abstract float Priority { get; }
        public abstract int Item { get; }
        public string HeaderDescription
        {
            get
            {
                string desc = Language.GetTextValue($"Mods.{Mod.Name}.Toggler.{Name}");
                if (Item <= 0) return desc;
                string itemIcon = $"[i:{Item}]";
                /*
                ModItem modItem = ModContent.GetModItem(Item);
                if (modItem == null)
                {
                    itemIcon = $"[i:{Item}]";
                }
                else
                {
                    itemIcon = $"[i:{modItem.Mod.Name}/{modItem.Name}]";
                }
                */
                return $"{itemIcon} {desc}";
            }
        }
        public override void Load() // Must do this here so headers are registered before Toggles
        {
            ToggleLoader.RegisterHeader(this);
            ModTypeLookup<Header>.Register(this);
        }

        protected override void Register()
        {
            /*
            ToggleLoader.RegisterHeader(this);
            ModTypeLookup<Header>.Register(this);
            */
        }

        public override string ToString() => $"Mod: {Mod}, Item: {Item}, Category: {SortCategory}, Priority: {Priority}";
        public string GetRawToggleName()
        {
            string baseText = HeaderDescription;
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

        public static T GetHeader<T>() where T : Header => ModContent.GetInstance<T>();
        public static Header GetHeaderFromItem<T>() where T : ModItem => ToggleLoader.LoadedHeaders.FirstOrDefault(h => h.Item == ModContent.ItemType<T>());
        public static Header GetHeaderFromItemType(int type) => ToggleLoader.LoadedHeaders.FirstOrDefault(h => h.Item == type);
    }
}
