using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasSouls.Content.UI
{
	public static class FargoUIManager
	{
		public static UserInterface TogglerUserInterface { get; private set; }

		public static UserInterface TogglerToggleUserInterface { get; private set; }

		public static SoulToggler SoulToggler { get; private set; }

		public static SoulTogglerButton SoulTogglerButton { get; private set; }

		private static GameTime LastUpdateUIGameTime { get; set; }

		public static Asset<Texture2D> CheckMark { get; private set; }

		public static Asset<Texture2D> CheckBox { get; private set; }

		public static Asset<Texture2D> Cross { get; private set; }

		public static Asset<Texture2D> SoulTogglerButtonTexture { get; private set; }

		public static Asset<Texture2D> SoulTogglerButton_MouseOverTexture { get; private set; }

		public static Asset<Texture2D> PresetButtonOutline { get; private set; }

		public static Asset<Texture2D> PresetOffButton { get; private set; }

		public static Asset<Texture2D> PresetOnButton { get; private set; }

		public static Asset<Texture2D> PresetMinimalButton { get; private set; }

		public static Asset<Texture2D> PresetCustomButton { get; private set; }

		public static Asset<Texture2D> ReloadButtonTexture { get; private set; }
		public static Asset<Texture2D> DisplayAllButtonTexture { get; private set; }

		public static Asset<Texture2D> OncomingMutantTexture { get; private set; }

		public static Asset<Texture2D> OncomingMutantAuraTexture { get; private set; }

		public static void LoadUI()
		{
			if (!Main.dedServ)
			{
				// Load textures
				CheckMark = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckMark", AssetRequestMode.ImmediateLoad);
				CheckBox = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/CheckBox", AssetRequestMode.ImmediateLoad);
				Cross = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/Cross", AssetRequestMode.ImmediateLoad);
				SoulTogglerButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle", AssetRequestMode.ImmediateLoad);
				SoulTogglerButton_MouseOverTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/SoulTogglerToggle_MouseOver", AssetRequestMode.ImmediateLoad);
				PresetButtonOutline = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOutline", AssetRequestMode.ImmediateLoad);
				PresetOffButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOff", AssetRequestMode.ImmediateLoad);
				PresetOnButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetOn", AssetRequestMode.ImmediateLoad);
				PresetMinimalButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetMinimal", AssetRequestMode.ImmediateLoad);
				PresetCustomButton = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/PresetCustom", AssetRequestMode.ImmediateLoad);
				DisplayAllButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/DisplayAllButton", AssetRequestMode.ImmediateLoad);
				ReloadButtonTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ReloadButton", AssetRequestMode.ImmediateLoad);
				OncomingMutantTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutant", AssetRequestMode.ImmediateLoad);
				OncomingMutantAuraTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantAura", AssetRequestMode.ImmediateLoad);

				// Initialize UserInterfaces
				TogglerUserInterface = new UserInterface();
				TogglerToggleUserInterface = new UserInterface();

				// Activate UIs
				SoulToggler = new();
				SoulToggler.Activate();
				SoulTogglerButton = new();
				SoulTogglerButton.Activate();

				TogglerToggleUserInterface.SetState(SoulTogglerButton);
			}
		}

		public static void UpdateUI(GameTime gameTime)
		{
			LastUpdateUIGameTime = gameTime;

			if (!Main.playerInventory && SoulConfig.Instance.HideTogglerWhenInventoryIsClosed)
				CloseSoulToggler();
			if (!Main.playerInventory)
			{
				CloseSoulTogglerButton();
			}
			else
			{
				OpenSoulTogglerButton();
			}
			if (TogglerUserInterface?.CurrentState != null)
				TogglerUserInterface.Update(gameTime);
			if (TogglerToggleUserInterface?.CurrentState != null)
				TogglerToggleUserInterface.Update(gameTime);
		}

		public static bool IsSoulTogglerOpen() => TogglerUserInterface?.CurrentState == null;

		public static void CloseSoulToggler()
		{
			TogglerUserInterface?.SetState(null);

			if (SoulConfig.Instance.ToggleSearchReset)
			{
				SoulToggler.SearchBar.Input = "";

			}
			SoulToggler.NeedsToggleListBuilding = true;
		}

		public static bool IsTogglerOpen() => TogglerUserInterface.CurrentState == SoulToggler;

		public static void OpenToggler() => TogglerUserInterface.SetState(SoulToggler);
		public static void CloseSoulTogglerButton() => TogglerToggleUserInterface.SetState(null);
		public static void OpenSoulTogglerButton() => TogglerToggleUserInterface.SetState(SoulTogglerButton);

		public static void ToggleSoulToggler()
		{
			if (IsSoulTogglerOpen())
			{
				SoundEngine.PlaySound(SoundID.MenuOpen);
				OpenToggler();
			}
			else if (IsTogglerOpen())
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				CloseSoulToggler();
			}
		}

		public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int index = layers.FindIndex((layer) => layer.Name == "Vanilla: Inventory");
			if (index != -1)
			{
				layers.Insert(index - 1, new LegacyGameInterfaceLayer("Fargos: Soul Toggler", delegate
				{
					if (LastUpdateUIGameTime != null && TogglerUserInterface?.CurrentState != null)
						TogglerUserInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);
					return true;
				}, InterfaceScaleType.UI));

				layers.Insert(index, new LegacyGameInterfaceLayer("Fargos: Soul Toggler Toggler", delegate
				{
					if (LastUpdateUIGameTime != null && TogglerToggleUserInterface?.CurrentState != null)
						TogglerToggleUserInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);

					return true;
				}, InterfaceScaleType.UI));
			}
		}
	}
}
