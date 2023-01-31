using FargowiltasSouls.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls
{
    public class UIManager
    {
        public UserInterface TogglerUserInterface;
        public UserInterface TogglerToggleUserInterface;
        public SoulToggler SoulToggler;
        public SoulTogglerButton SoulTogglerButton;
        private GameTime _lastUpdateUIGameTime;

        public Asset<Texture2D> CheckMark;
        public Asset<Texture2D> CheckBox;
        public Asset<Texture2D> SoulTogglerButtonTexture;
        public Asset<Texture2D> SoulTogglerButton_MouseOverTexture;
        public Asset<Texture2D> PresetButtonOutline;
        public Asset<Texture2D> PresetOffButton;
        public Asset<Texture2D> PresetOnButton;
        public Asset<Texture2D> PresetMinimalButton;
        public Asset<Texture2D> PresetCustomButton;
        public Asset<Texture2D> OncomingMutantTexture;
        public Asset<Texture2D> OncomingMutantAuraTexture;

        public void LoadUI()
        {
            if (!Main.dedServ)
            {
                // Load textures
                CheckMark = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/CheckMark", AssetRequestMode.ImmediateLoad);
                CheckBox = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/CheckBox", AssetRequestMode.ImmediateLoad);
                SoulTogglerButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/SoulTogglerToggle", AssetRequestMode.ImmediateLoad);
                SoulTogglerButton_MouseOverTexture = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/SoulTogglerToggle_MouseOver", AssetRequestMode.ImmediateLoad);
                PresetButtonOutline = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/PresetOutline", AssetRequestMode.ImmediateLoad);
                PresetOffButton = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/PresetOff", AssetRequestMode.ImmediateLoad);
                PresetOnButton = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/PresetOn", AssetRequestMode.ImmediateLoad);
                PresetMinimalButton = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/PresetMinimal", AssetRequestMode.ImmediateLoad);
                PresetCustomButton = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/PresetCustom", AssetRequestMode.ImmediateLoad);
                OncomingMutantTexture = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/OncomingMutant", AssetRequestMode.ImmediateLoad);
                OncomingMutantAuraTexture = ModContent.Request<Texture2D>("FargowiltasSouls/UI/Assets/OncomingMutantAura", AssetRequestMode.ImmediateLoad);

                // Initialize UserInterfaces
                TogglerUserInterface = new UserInterface();
                TogglerToggleUserInterface = new UserInterface();

                // Activate UIs
                SoulToggler = new SoulToggler();
                SoulToggler.Activate();
                SoulTogglerButton = new SoulTogglerButton();
                SoulTogglerButton.Activate();

                TogglerToggleUserInterface.SetState(SoulTogglerButton);
            }
        }

        public void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUIGameTime = gameTime;

            if (!Main.playerInventory && SoulConfig.Instance.HideTogglerWhenInventoryIsClosed)
                CloseSoulToggler();

            if (TogglerUserInterface?.CurrentState != null)
                TogglerUserInterface.Update(gameTime);
            if (TogglerToggleUserInterface?.CurrentState != null)
                TogglerToggleUserInterface.Update(gameTime);
        }

        public bool IsSoulTogglerOpen() => TogglerUserInterface?.CurrentState == null;
        public void CloseSoulToggler()
        {
            TogglerUserInterface?.SetState(null);
            
            if (SoulConfig.Instance.ToggleSearchReset)
            {
                SoulToggler.SearchBar.Input = "";
                SoulToggler.NeedsToggleListBuilding = true;
            }
        }
        public bool IsTogglerOpen() => TogglerUserInterface.CurrentState == SoulToggler;
        public void OpenToggler() => TogglerUserInterface.SetState(SoulToggler);

        public void ToggleSoulToggler()
        {
            if (IsSoulTogglerOpen())
            {
                //Main.NewText("we opening");

                SoundEngine.PlaySound(SoundID.MenuOpen);
                OpenToggler();
            }
            else if (IsTogglerOpen())
            {
                //Main.NewText("we closing");
                SoundEngine.PlaySound(SoundID.MenuClose);
                CloseSoulToggler();
            }
        }

        public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((layer) => layer.Name == "Vanilla: Inventory");
            if (index != -1)
            {
                layers.Insert(index - 1, new LegacyGameInterfaceLayer("Fargos: Soul Toggler", delegate
                {
                    if (_lastUpdateUIGameTime != null && TogglerUserInterface?.CurrentState != null)
                        TogglerUserInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);
                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(index, new LegacyGameInterfaceLayer("Fargos: Soul Toggler Toggler", delegate
                {
                    if (_lastUpdateUIGameTime != null && TogglerToggleUserInterface?.CurrentState != null)
                        TogglerToggleUserInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);

                    return true;
                }, InterfaceScaleType.UI));
            }
        }
    }
}
