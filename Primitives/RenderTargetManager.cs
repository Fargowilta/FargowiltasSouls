using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Primitives
{
    public class RenderTargetManager : ModSystem
    {
        #region Fields And Properities
        private static RenderTarget2D PixelRenderTarget;

        private static readonly List<IPixelPrimitiveDrawer> PixelPrimDrawersList = new();

        private Vector2 PreviousScreenSize;
        #endregion

        #region Overrides
        public override void Load()
        {
            On.Terraria.Main.CheckMonoliths += DrawToCustomRenderTargets;
            On.Terraria.Main.DoDraw_DrawNPCsOverTiles += DrawPixelRenderTarget;
            ResizeRenderTarget(true);
        }

        public override void Unload() 
        {
            On.Terraria.Main.CheckMonoliths -= DrawToCustomRenderTargets;
            On.Terraria.Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRenderTarget;
        }

        public override void PostUpdateEverything() => ResizeRenderTarget(false);
        #endregion

        #region Methods
        private void DrawPixelRenderTarget(On.Terraria.Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            // Draw our RT. The scale is important, it is 2 here as this RT is 0.5x the main screen size.
            Main.spriteBatch.Draw(PixelRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }

        private void DrawToCustomRenderTargets(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            // Clear our render target from the previous frame.
            PixelPrimDrawersList.Clear();

            // Check every active projectile.
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile projectile = Main.projectile[i];
                // If the projectile is active, a mod projectile, and uses our interface,
                if (projectile.active && projectile.ModProjectile != null && projectile.ModProjectile is IPixelPrimitiveDrawer pixelPrimitiveProjectile)
                    // Add it to the list of prims to draw this frame.
                    PixelPrimDrawersList.Add(pixelPrimitiveProjectile);
            }

            // Draw the prims. The render target gets set here.
            DrawPrimsToRenderTarget(PixelRenderTarget, PixelPrimDrawersList);

            // Clear the current render target.
            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            // Call orig.
            orig();
        }

        private static void DrawPrimsToRenderTarget(RenderTarget2D renderTarget, List<IPixelPrimitiveDrawer> pixelPrimitives)
        {
            // Swap to our custom render target.
            SwapToRenderTarget(renderTarget);
            // If the list has any entries.
            if (pixelPrimitives.Any())
            {
                // Start a spritebatch, as one does not exist in the method we're detouring.
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

                // Loop through the list and call each draw function.
                foreach (IPixelPrimitiveDrawer pixelPrimitiveDrawer in pixelPrimitives)
                    pixelPrimitiveDrawer.DrawPixelPrimitives(Main.spriteBatch);

                // End the spritebatch we started.
                Main.spriteBatch.End();
            }
        }

        private static void SwapToRenderTarget(RenderTarget2D renderTarget)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            // If we are in the menu, a server, or any of these are null, return.
            if (Main.gameMenu || Main.dedServ || renderTarget is null || graphicsDevice is null || spriteBatch is null)
                return;

            // Else, set the render target.
            graphicsDevice.SetRenderTarget(renderTarget);
            // "Flush" the screen, removing any previous things drawn to it.
            graphicsDevice.Clear(Color.Transparent);
        }

        private void ResizeRenderTarget(bool load)
        {
            // If not in the game menu, and we arent a dedicated server,
            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                // Get the current screen size.
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                // If it does not match the previous one, we need to update it.
                if (currentScreenSize != PreviousScreenSize)
                {
                    // Render target stuff should be done on the main thread only.
                    Main.QueueMainThreadAction(() =>
                    {
                        // If it is not null, or already disposed, dispose it.
                        if (PixelRenderTarget != null && !PixelRenderTarget.IsDisposed)
                            PixelRenderTarget.Dispose();

                        // Recreate the render target with the current, accurate screen dimensions.
                        // In our case, we want to half them to downscale it, pixelating it.
                        PixelRenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
                    });

                }
                // Set the current one to the previous one for next frame.
                PreviousScreenSize = currentScreenSize;
            }
        }
        #endregion
    }
}
