using FargowiltasSouls.Assets.ExtraTextures;

using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.PlayerDrawLayers;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Graphics.Drawers
{
	public class EternityWingDrawer : ModSystem
	{
		public static ManagedRenderTarget WingTarget
		{
			get;
			private set;
		}

		private static bool WingsInUse
		{
			get;
			set;
		}

		public static bool DoNotDrawSpecialWings
		{
			get;
			private set;
		}

		public override void Load()
		{
			WingTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget, true);
			Main.OnPreDraw += PrepareWingTarget;
			On_LegacyPlayerRenderer.DrawPlayers += DrawWingTarget;
			On_PlayerDrawLayers.DrawPlayer_09_Wings += PreventDefaultWingDrawing;
			On_Player.WingFrame += ManuallyAnimateCustomWingsIHateThis;
		}

		// This suffices for now, but a better system should be used if multiple wings with more frames are wanted.
		private void ManuallyAnimateCustomWingsIHateThis(On_Player.orig_WingFrame orig, Player self, bool wingFlap, bool isCustomWings)
		{
			if (self.wings == EternitySoul.WingSlotID)
			{
				if (wingFlap || self.jump > 0)
				{
					int animateRate = 4;
					int frameCount = 5;
					self.wingFrameCounter++;
					if (self.wingFrameCounter > animateRate)
					{
						self.wingFrame++;
						self.wingFrameCounter = 0;
						if (self.wingFrame >= frameCount)
							self.wingFrame = 0;
					}
				}
				else
					self.wingFrame = 0;
				return;
			}
			orig(self, wingFlap, isCustomWings);
		}

		public override void Unload()
		{
			Main.OnPreDraw -= PrepareWingTarget;
			On_LegacyPlayerRenderer.DrawPlayers -= DrawWingTarget;
			On_PlayerDrawLayers.DrawPlayer_09_Wings -= PreventDefaultWingDrawing;
			On_Player.WingFrame -= ManuallyAnimateCustomWingsIHateThis;
		}

		private void PrepareWingTarget(GameTime obj)
		{
			WingsInUse = false;

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];

				if (player.wings != EternitySoul.WingSlotID || !player.active || player.dead)
					continue;

				WingsInUse = true;
				break;
			}

			if (!ShaderManager.HasFinishedLoading || Main.gameMenu || !WingsInUse)
				return;

			WingTarget.SwapToRenderTarget();

			// Prepare the shader.
			var shader = ShaderManager.GetShader("EternitySoulWings");
			FargoSoulsUtil.SetTexture1(FargosTextureRegistry.TurbulentNoise.Value);
			FargoSoulsUtil.SetTexture2(FargosTextureRegistry.ColorNoiseMap.Value);

			shader.TrySetParameter("lightning", Color.White);
			shader.TrySetParameter("resolution", WingTarget.Target.Size() / 2);
			shader.Apply();

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.Shader.Value, Matrix.Identity);

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];
				if (player.wings != EternitySoul.WingSlotID || player.dead || !player.active)
					continue;

				// Append the player's wings to the draw cache.
				PlayerDrawSet drawInfo = default;
				drawInfo.BoringSetup(player, new List<DrawData>(), new List<int>(), new List<int>(), player.TopLeft + Vector2.UnitY * player.gfxOffY, 0f, player.fullRotation, player.fullRotationOrigin);
				DoNotDrawSpecialWings = false;
				EternitySoulWingPlayerLayer.DrawWings(ref drawInfo);
				DoNotDrawSpecialWings = true;

				// Draw the wings with the activated shader.
				foreach (DrawData wingData in drawInfo.DrawDataCache)
					wingData.Draw(Main.spriteBatch);
			}

			Main.spriteBatch.End();
			Main.instance.GraphicsDevice.SetRenderTarget(null);
		}

		private void DrawWingTarget(On_LegacyPlayerRenderer.orig_DrawPlayers orig, LegacyPlayerRenderer self, Camera camera, IEnumerable<Player> players)
		{
			if (WingsInUse)
			{
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, camera.Rasterizer, null, camera.GameViewMatrix.TransformationMatrix);

				// Optionally apply the wing shader.
				if (Main.LocalPlayer.cWings != 0)
					GameShaders.Armor.Apply(Main.LocalPlayer.cWings, Main.LocalPlayer);

				Main.spriteBatch.Draw(WingTarget, Main.screenLastPosition - Main.screenPosition, Color.White);
				Main.spriteBatch.End();
			}

			orig(self, camera, players);
		}

		// Don't draw these wings here.
		private void PreventDefaultWingDrawing(On_PlayerDrawLayers.orig_DrawPlayer_09_Wings orig, ref PlayerDrawSet drawinfo)
		{
			if (drawinfo.hideEntirePlayer || drawinfo.drawPlayer.dead || drawinfo.drawPlayer.wings == EternitySoul.WingSlotID)
				return;

			orig(ref drawinfo);
		}
	}
}
