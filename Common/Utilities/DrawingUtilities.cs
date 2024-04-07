using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace FargowiltasSouls
{
    public static partial class FargoSoulsUtil
    {
        public static void CreatePerspectiveMatrixes(out Matrix view, out Matrix projection)
        {
            int height = Main.instance.GraphicsDevice.Viewport.Height;

            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix zoomScaleMatrix = Matrix.CreateScale(zoom.X, zoom.Y, 1f);

            // Get a matrix that aims towards the Z axis (these calculations are relative to a 2D world).
            view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

            // Offset the matrix to the appropriate position.
            view *= Matrix.CreateTranslation(0f, -height, 0f);

            // Flip the matrix around 180 degrees.
            view *= Matrix.CreateRotationZ(MathHelper.Pi);

            if (Main.LocalPlayer.gravDir == -1f)
                view *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, height, 0f);

            // Account for the current zoom.
            view *= zoomScaleMatrix;

            projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth * zoom.X, 0f, Main.screenHeight * zoom.Y, 0f, 1f) * zoomScaleMatrix;
        }

        private static readonly FieldInfo shaderTextureField = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo shaderTextureField2 = typeof(MiscShaderData).GetField("_uImage2", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Uses reflection to set uImage1. Its underlying data is private and the only way to change it publicly
        /// is via a method that only accepts paths to vanilla textures.
        /// </summary>
        /// <param name="shader">The shader</param>
        /// <param name="texture">The texture to set</param>
        public static void SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField.SetValue(shader, texture);

        /// <summary>
        /// Uses reflection to set uImage2. Its underlying data is private and the only way to change it publicly
        /// is via a method that only accepts paths to vanilla textures.
        /// </summary>
        /// <param name="shader">The shader</param>
        /// <param name="texture">The texture to set</param>
        public static void SetShaderTexture2(this MiscShaderData shader, Asset<Texture2D> texture) => shaderTextureField2.SetValue(shader, texture);

        /// <summary>
        /// Prepares a <see cref="SpriteBatch"/> for shader-based drawing.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public static void EnterShaderRegion(this SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// Ends changes to a <see cref="SpriteBatch"/> based on shader-based drawing in favor of typical draw begin states.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public static void ExitShaderRegion(this SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public static void SetTexture1(this Texture2D texture) => Main.instance.GraphicsDevice.Textures[1] = texture;

        public static void SetTexture2(this Texture2D texture) => Main.instance.GraphicsDevice.Textures[2] = texture;

        public static Vector4 ToVector4(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public static string VanillaTextureProjectile(int projectileID) => $"Terraria/Images/Projectile_{projectileID}";
        public static string VanillaTextureNPC(int npcID) => $"Terraria/Images/NPC_{npcID}";

        public static void GenericProjectileDraw(Projectile projectile, Color lightColor, Texture2D texture = null, Vector2? drawPos = null, float? rotation = null)
        {
            rotation ??= projectile.rotation;
            drawPos ??= projectile.Center;
            texture ??= TextureAssets.Projectile[projectile.type].Value;

            int sizeY = texture.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int frameY = projectile.frame * sizeY;
            Rectangle rectangle = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos.Value - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, projectile.GetAlpha(lightColor),
                    rotation.Value, origin, projectile.scale, spriteEffects, 0);
        }
        public static void ProjectileWithTrailDraw(Projectile projectile, Color lightColor, Texture2D texture = null, int? trailLength = null, bool additiveTrail = false, bool alsoAdditiveMainSprite = true)
        {

            texture ??= TextureAssets.Projectile[projectile.type].Value;

            int sizeY = texture.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int frameY = projectile.frame * sizeY;
            Rectangle rectangle = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            trailLength ??= ProjectileID.Sets.TrailCacheLength[projectile.type];

            if (additiveTrail)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            for (int i = 0; i < trailLength.Value; i++)
            {
                Color oldColor = lightColor * 0.75f;
                oldColor = (Color)(oldColor * ((float)(trailLength - i) / trailLength));
                Vector2 oldPos = projectile.oldPos[i] + rectangle.Size() / 2;
                float oldRot = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture, oldPos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, projectile.GetAlpha(oldColor),
                    oldRot, origin, projectile.scale, spriteEffects, 0);
            }
            if (additiveTrail && !alsoAdditiveMainSprite)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            GenericProjectileDraw(projectile, lightColor, texture);
            if (additiveTrail && alsoAdditiveMainSprite)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
        public static string EmptyTexture => "FargowiltasSouls/Content/Projectiles/Empty";
    }
}
