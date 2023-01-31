using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

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
        public UIPresetButton SomeEffectsButton;
        public UIPresetButton[] CustomButton = new UIPresetButton[3];

        public override void OnInitialize()
        {
            Vector2 offset = new(Main.screenWidth / 2f - (BackWidth / 2f), Main.screenHeight / 2f - (BackHeight / 2f));
            
            NeedsToggleListBuilding = true;
            DisplayMod = "";
            SortCatagory = "";

            // This entire layout is cancerous and dangerous to your health because red protected UIElements children
            // If I want to give extra non-children to BackPanel to count as children when seeing if it should drag, I have to abandon
            // all semblence of organization in favour of making it work. Enjoy my write only UI laying out.
            // Oh well, at least it works...

            Scrollbar = new UIScrollbar();
            Scrollbar.SetView(200f, 1000f);
            Scrollbar.Width.Set(20, 0);
            Scrollbar.OverflowHidden = true;
            Scrollbar.OnScrollWheel += hotbarScrollFix;

            ToggleList = new UIToggleList();
            ToggleList.SetScrollbar(Scrollbar);
            ToggleList.OnScrollWheel += hotbarScrollFix;

            BackPanel = new UIDragablePanel(Scrollbar, ToggleList);
            BackPanel.Left.Set(offset.X, 0);
            BackPanel.Top.Set(offset.Y, 0);
            BackPanel.Width.Set(BackWidth, 0);
            BackPanel.Height.Set(BackHeight, 0);
            BackPanel.PaddingLeft = BackPanel.PaddingRight = BackPanel.PaddingTop = BackPanel.PaddingBottom = 0;
            BackPanel.BackgroundColor = new Color(29, 33, 70) * 0.7f;

            InnerPanel = new UIPanel();
            InnerPanel.Width.Set(BackWidth - 12, 0);
            InnerPanel.Height.Set(BackHeight - 70, 0);
            InnerPanel.Left.Set(6, 0);
            InnerPanel.Top.Set(32, 0);
            InnerPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;

            SearchBar = new UISearchBar(BackWidth - 8, 26);
            SearchBar.Left.Set(4, 0);
            SearchBar.Top.Set(4, 0);
            SearchBar.OnTextChange += SearchBar_OnTextChange;

            ToggleList.Width.Set(InnerPanel.Width.Pixels - InnerPanel.PaddingLeft * 2f - Scrollbar.Width.Pixels, 0);
            ToggleList.Height.Set(InnerPanel.Height.Pixels - InnerPanel.PaddingTop * 2f, 0);

            Scrollbar.Height.Set(InnerPanel.Height.Pixels - 16, 0);
            Scrollbar.Left.Set(InnerPanel.Width.Pixels - Scrollbar.Width.Pixels - 18, 0);

            PresetPanel = new UIPanel();
            PresetPanel.Left.Set(5, 0);
            PresetPanel.Top.Set(SearchBar.Height.Pixels + InnerPanel.Height.Pixels + 8, 0);
            PresetPanel.Width.Set(BackWidth - 10, 0);
            PresetPanel.Height.Set(32, 0);
            PresetPanel.PaddingTop = PresetPanel.PaddingBottom = 0;
            PresetPanel.PaddingLeft = PresetPanel.PaddingRight = 0;
            PresetPanel.BackgroundColor = new Color(74, 95, 172);

            OffButton = new UIPresetButton(FargowiltasSouls.UserInterfaceManager.PresetOffButton.Value, (toggles) =>
            {
                toggles.SetAll(false);
            }, FargoSoulsUtil.IsChinese() ? "关闭所有饰品效果" : "Turn all toggles off");
            OffButton.Top.Set(6, 0);
            OffButton.Left.Set(8, 0);

            OnButton = new UIPresetButton(FargowiltasSouls.UserInterfaceManager.PresetOnButton.Value, (toggles) =>
            {
                toggles.SetAll(true);
            }, FargoSoulsUtil.IsChinese() ? "开启所有饰品效果" : "Turn all toggles on");
            OnButton.Top.Set(6, 0);
            OnButton.Left.Set(30, 0);

            SomeEffectsButton = new UIPresetButton(FargowiltasSouls.UserInterfaceManager.PresetMinimalButton.Value, (toggles) =>
            {
                toggles.SomeEffects();
            }, FargoSoulsUtil.IsChinese() ? "部分效果预设" : "Some effects preset");
            SomeEffectsButton.Top.Set(6, 0);
            SomeEffectsButton.Left.Set(52, 0);

            MinimalButton = new UIPresetButton(FargowiltasSouls.UserInterfaceManager.PresetMinimalButton.Value, (toggles) =>
            {
                toggles.MinimalEffects();
            }, FargoSoulsUtil.IsChinese() ? "最小化影响预设" : "Minimal effects preset");
            MinimalButton.Top.Set(6, 0);
            MinimalButton.Left.Set(74, 0);

            Append(BackPanel);
            BackPanel.Append(InnerPanel);
            BackPanel.Append(SearchBar);
            BackPanel.Append(PresetPanel);
            InnerPanel.Append(Scrollbar);
            InnerPanel.Append(ToggleList);
            PresetPanel.Append(OffButton);
            PresetPanel.Append(OnButton);
            PresetPanel.Append(SomeEffectsButton);
            PresetPanel.Append(MinimalButton);

            const int xOffset = 74; //ensure this matches the Left.Set of preceding button
            for (int i = 0; i < ToggleBackend.CustomPresetCount; i++)
            {
                int slot = i + 1;
                CustomButton[i] = new UIPresetButton(FargowiltasSouls.UserInterfaceManager.PresetCustomButton.Value,
                toggles => toggles.LoadCustomPreset(slot),
                toggles => toggles.SaveCustomPreset(slot),
                $"Custom preset {slot} (right click to save)");
                CustomButton[i].Top.Set(6, 0);
                CustomButton[i].Left.Set(xOffset + 22 * slot, 0);
                PresetPanel.Append(CustomButton[i]);
            }

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
            ToggleBackend toggler = player.GetModPlayer<FargoSoulsPlayer>().Toggler;

            IEnumerable<Toggle> displayToggles = toggler.Toggles.Values.Where((toggle) =>
            {
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

        /*public void SetPositionToPoint(Point point)
        {
            BackPanel.Left.Set(point.X, 0);
            BackPanel.Top.Set(point.Y, 0);
        }

        public Point GetPositionAsPoint() => new Point((int)BackPanel.Left.Pixels, (int)BackPanel.Top.Pixels);*/
    }
}
