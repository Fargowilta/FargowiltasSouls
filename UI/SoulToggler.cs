using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using System;
using Terraria.Localization;
using Terraria.UI.Chat;
using System.Text.RegularExpressions;

namespace FargowiltasSouls.UI
{
    public class SoulToggler : UIState
    {
        public static Regex RemoveItemTags = new Regex(@"\[[^\[\]]*\]");

        public bool NeedsToggleListBuilding;
        public string DisplayMod;
        public string SortCatagory;

        public const int BackWidth = 400;
        public const int BackHeight = 658;

        public UIDragablePanel BackPanel;
        public UIPanel InnerPanel;
        public UIPanel PresetPanel;
        public UIScrollbar Scrollbar;
        public UIToggleList ToggleList;
        public UISearchBar SearchBar;

        public UIPresetButton OffButton;
        public UIPresetButton OnButton;
        public UIPresetButton MinimalButton;

        public override void OnInitialize()
        {
            NeedsToggleListBuilding = true;
            DisplayMod = "";
            SortCatagory = "";

            // This entire layout is cancerous and dangerous to your health because red protected UIElements children
            // If I want to give extra non-children to BackPanel to count as children when seeing if it should drag, I have to abandon
            // all semblence of organization in favour of making it work. Enjoy my write only UI laying out.
            // Oh well, at least it works...

            Scrollbar = new UIScrollbar();
            Scrollbar.SetView(200f, 1000f);
            Scrollbar.Width.Set(20, 0f);
            Scrollbar.OverflowHidden = true;
            Scrollbar.OnScrollWheel += hotbarScrollFix;

            ToggleList = new UIToggleList();
            ToggleList.SetScrollbar(Scrollbar);
            ToggleList.OnScrollWheel += hotbarScrollFix;

            BackPanel = new UIDragablePanel(Scrollbar, ToggleList);
            BackPanel.Left.Set(0, 0f);
            BackPanel.Top.Set(0, 0f);
            BackPanel.Width.Set(BackWidth, 0f);
            BackPanel.Height.Set(BackHeight, 0f);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            InnerPanel = new UIPanel();
            InnerPanel.Width.Set(BackWidth - 12, 0f);
            InnerPanel.Height.Set(BackHeight - 70, 0);
            InnerPanel.Left.Set(6, 0f);
            InnerPanel.Top.Set(32, 0f);
            InnerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            SearchBar = new UISearchBar(BackWidth - 8, 26);
            SearchBar.Left.Set(4, 0f);
            SearchBar.Top.Set(4, 0f);
            SearchBar.OnTextChange += SearchBar_OnTextChange;

            ToggleList.Width.Set(InnerPanel.Width.Pixels - InnerPanel.PaddingLeft * 2f - Scrollbar.Width.Pixels, 0f);
            ToggleList.Height.Set(InnerPanel.Height.Pixels - InnerPanel.PaddingTop * 2f, 0f);

            Scrollbar.Height.Set(InnerPanel.Height.Pixels - 16, 0f);
            Scrollbar.Left.Set(InnerPanel.Width.Pixels - Scrollbar.Width.Pixels - 18, 0f);

            PresetPanel = new UIPanel();
            PresetPanel.Left.Set(5, 0f);
            PresetPanel.Top.Set(SearchBar.Height.Pixels + InnerPanel.Height.Pixels + 8, 0f);
            PresetPanel.Width.Set(BackWidth - 10, 0f);
            PresetPanel.Height.Set(32, 0f);
            PresetPanel.PaddingTop = PresetPanel.PaddingBottom = 0;
            PresetPanel.PaddingLeft = PresetPanel.PaddingRight = 0;
            PresetPanel.BackgroundColor = new Color(74, 95, 172);

            OffButton = new UIPresetButton(Fargowiltas.UserInterfaceManager.PresetOffButton, (toggles) => {
                toggles.SetAll(false);
            }, Language.GetTextValue("Mods.FargowiltasSouls.UIPresetButton.Off"));//"Turn all toggles off"
            OffButton.Top.Set(6, 0f);
            OffButton.Left.Set(8, 0f);

            OnButton = new UIPresetButton(Fargowiltas.UserInterfaceManager.PresetOnButton, (toggles) => {
                toggles.SetAll(true);
            }, Language.GetTextValue("Mods.FargowiltasSouls.UIPresetButton.On"));//"Turn all toggles on"
            OnButton.Top.Set(6, 0f);
            OnButton.Left.Set(30, 0f);

            MinimalButton = new UIPresetButton(Fargowiltas.UserInterfaceManager.PresetMinimalButton, (toggles) => {
                toggles.MinimalEffects();
            }, Language.GetTextValue("Mods.FargowiltasSouls.UIPresetButton.Minimal"));//"Minimal effects preset"
            MinimalButton.Top.Set(6, 0f);
            MinimalButton.Left.Set(52, 0f);

            Append(BackPanel);
            BackPanel.Append(InnerPanel);
            BackPanel.Append(SearchBar);
            BackPanel.Append(PresetPanel);
            InnerPanel.Append(Scrollbar);
            InnerPanel.Append(ToggleList);
            PresetPanel.Append(OffButton);
            PresetPanel.Append(OnButton);
            PresetPanel.Append(MinimalButton);

            base.OnInitialize();
        }

        private void SearchBar_OnTextChange(string oldText, string currentText)
        {
            NeedsToggleListBuilding = true;
        }

        private void hotbarScrollFix(UIScrollWheelEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.ScrollHotbar(PlayerInput.ScrollWheelDelta / 120);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (NeedsToggleListBuilding)
            {
                BuildList();
                NeedsToggleListBuilding = false;
            }
        }

        public void BuildList()
        {
            ToggleList.Clear();
            Player player = Main.LocalPlayer;
            ToggleBackend toggler = player.GetModPlayer<FargoPlayer>().Toggler;

            IEnumerable<Toggle> displayToggles = toggler.Toggles.Values.Where((toggle) => {
                string[] words = GetRawToggleName(toggle.InternalName).Split(' ');
                return
                (string.IsNullOrEmpty(DisplayMod) || toggle.Mod == DisplayMod) &&
                (string.IsNullOrEmpty(SortCatagory) || toggle.Catagory == SortCatagory) &&
                (string.IsNullOrEmpty(SearchBar.Input) || words.Any(s => s.StartsWith(SearchBar.Input, StringComparison.OrdinalIgnoreCase)));
            });

            HashSet<string> usedHeaders = new HashSet<string>();
            List<Toggle> togglesAsLists = ToggleLoader.LoadedToggles.Values.ToList();

            foreach (Toggle toggle in displayToggles)
            {
                if (ToggleLoader.LoadedHeaders.ContainsKey(toggle.InternalName) && SearchBar.IsEmpty)
                {
                    if (ToggleList.Count > 0) // Don't add for the first header
                        ToggleList.Add(new UIText("", 0.2f)); // Blank line

                    (string name, int item) header = ToggleLoader.LoadedHeaders[toggle.InternalName];
                    ToggleList.Add(new UIHeader(header.name, header.item, (BackWidth - 16, 20)));
                }
                else if (!SearchBar.IsEmpty)
                {
                    int index = togglesAsLists.FindIndex(t => t.InternalName == toggle.InternalName);
                    int closestHeader = ToggleLoader.HeaderToggles.OrderBy(i =>
                        Math.Abs(index - i)).First();

                    if (closestHeader > index)
                        closestHeader = ToggleLoader.HeaderToggles[ToggleLoader.HeaderToggles.FindIndex(i => i == closestHeader) - 1];

                    (string name, int item) header = ToggleLoader.LoadedHeaders[togglesAsLists[closestHeader].InternalName];

                    if (!usedHeaders.Contains(header.name))
                    {
                        if (ToggleList.Count > 0) // Don't add for the first header
                            ToggleList.Add(new UIText("", 0.2f)); // Blank line

                        ToggleList.Add(new UIHeader(header.name, header.item, (BackWidth - 16, 20)));
                        usedHeaders.Add(header.name);
                    }
                }

                ToggleList.Add(new UIToggle(toggle.InternalName));
            }
        }

        public string GetRawToggleName(string key)
        {
            string baseText = Language.GetTextValue($"Mods.FargowiltasSouls.{key}Config");
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

        public void SetPositionToPoint(Point point)
        {
            BackPanel.Left.Set(point.X, 0f);
            BackPanel.Top.Set(point.Y, 0f);
        }

        public Point GetPositionAsPoint() => new Point((int)BackPanel.Left.Pixels, (int)BackPanel.Top.Pixels);
    }
}
