using FargowiltasSouls.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

// Lowest namespace for convenience. 
namespace FargowiltasSouls
{
    public class PrimDrawer
    {
        #region Fields/Properties

        public BasicEffect BaseEffect;
        public MiscShaderData Shader;
        public WidthTrailFunction WidthFunc;
        public ColorTrailFunction ColorFunc;

        /// <summary>
        /// This allows the width to dynamically change along the trail if desired.
        /// </summary>
        /// <param name="trailInterpolant">How far (0-1) the current position is on the trail</param>
        /// <returns></returns>
        public delegate float WidthTrailFunction(float trailInterpolant);

        /// <summary>
        /// This allows the color to dynamically change along the trail if desired.
        /// </summary>
        /// <param name="trailInterpolant">How far (0-1) the current position is on the trail</param>
        /// <returns></returns>
        public delegate Color ColorTrailFunction(float trailInterpolant);
        #endregion

        #region Drawing Methods
        /// <summary>
        /// Call this in PreDraw etc to draw your prims.
        /// </summary>
        /// <param name="basePoints"></param>
        /// <param name="baseOffset"></param>
        /// <param name="totalTrailPoints"></param>
        public void DrawPrims(IEnumerable<Vector2> basePoints, Vector2 baseOffset, int totalTrailPoints)
        {
            // Set the correct rasterizer state.
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // First, we need to offset the points by the base offset. This is almost always going to be -Main.screenPosition, but is changeable for flexability.
            List<Vector2> drawPointsList = CorrectlyOffsetPoints(basePoints, baseOffset, totalTrailPoints);

            // If the list is too short, any points in it are NaNs, or they are all the same point, return.
            if (drawPointsList.Count < 2 || drawPointsList.Any((drawPoint) => drawPoint.HasNaNs()) || drawPointsList.All(point => point == drawPointsList[0]))
                return;

            UpdateBaseEffect(out Matrix projection, out Matrix view);

            // Get an array of primitive triangles to pass through. Color data etc is stored in the struct.
            BasePrimTriangle[] pointVertices = CreatePrimitiveVertices(drawPointsList);
            // Get an array of the indices for each primitive triangle.
            short[] triangleIndices = CreatePrimitiveIndices(drawPointsList.Count);

            // If these are too short, or the indices isnt fully completed, return.
            if (triangleIndices.Length % 6 != 0 || pointVertices.Length <= 3)
                return;

            // If the shader exists, set the correct view and apply it.
            if (Shader != null)
            {
                Shader.Shader.Parameters["uWorldViewProjection"].SetValue(view * projection);
                Shader.Apply();
            }
            // Else, apply the base effect.
            else
                BaseEffect.CurrentTechnique.Passes[0].Apply();

            // Draw the prims! Also apply the main pixel shader. Specify the type of primitives this should be expecting, and pass through the array of the struct using the correct interface.
            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, pointVertices, 0, pointVertices.Length, triangleIndices, 0, triangleIndices.Length / 3);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>
        /// Use this in the <see cref="IPixelPrimitiveDrawer.DrawPrixelPrimitives(SpriteBatch)"/> method only.
        /// </summary>
        /// <param name="basePoints"></param>
        /// <param name="baseOffset"></param>
        /// <param name="totalTrailPoints"></param>
        public void DrawPixelPrims(IEnumerable<Vector2> basePoints, Vector2 baseOffset, int totalTrailPoints)
        {
            // Set the correct rasterizer state.
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // First, we need to offset the points by the base offset. This is almost always going to be -Main.screenPosition, but is changeable for flexability.
            List<Vector2> drawPointsList = CorrectlyOffsetPoints(basePoints, baseOffset, totalTrailPoints);

            // If the list is too short, any points in it are NaNs, or they are all the same point, return.
            if (drawPointsList.Count < 2 || drawPointsList.Any((drawPoint) => drawPoint.HasNaNs()) || drawPointsList.All(point => point == drawPointsList[0]))
                return;

            UpdateBaseEffectPixel(out var effectProjetion, out Matrix view);
            // Get an array of primitive triangles to pass through. Color data etc is stored in the struct.
            BasePrimTriangle[] pointVertices = CreatePrimitiveVertices(drawPointsList);
            // Get an array of the indices for each primitive triangle.
            short[] triangleIndices = CreatePrimitiveIndices(drawPointsList.Count);

            // If these are too short, or the indices isnt fully completed, return.
            if (triangleIndices.Length % 6 != 0 || pointVertices.Length <= 3)
                return;

            // If the shader exists, set the correct view and apply it.
            if (Shader != null)
            {
                Shader.Shader.Parameters["uWorldViewProjection"].SetValue(view * effectProjetion);
                Shader.Apply();
            }
            // Else, apply the base effect.
            else
                BaseEffect.CurrentTechnique.Passes[0].Apply();

            // Draw the prims! Also apply the main pixel shader. Specify the type of primitives this should be expecting, and pass through the array of the struct using the correct interface.
            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, pointVertices, 0, pointVertices.Length, triangleIndices, 0, triangleIndices.Length / 3);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Cache this and use it to call draw from.
        /// </summary>
        /// <param name="widthFunc">The width function</param>
        /// <param name="colorFunc">The color function</param>
        /// <param name="shader">The shader, if any</param>
        public PrimDrawer(WidthTrailFunction widthFunc, ColorTrailFunction colorFunc, MiscShaderData shader = null)
        {
            WidthFunc = widthFunc;
            ColorFunc = colorFunc;
            Shader = shader;
            // Create a basic effect.
            BaseEffect = new BasicEffect(Main.instance.GraphicsDevice)
            {
                VertexColorEnabled = true,
                TextureEnabled = false
            };
            UpdateBaseEffect(out _, out _);
        } 

        private void UpdateBaseEffect(out Matrix effectProjection, out Matrix effectView)
        {
            // Get the screen bounds.
            int height = Main.instance.GraphicsDevice.Viewport.Height;

            // Get the zoom and the scaling zoom matrix from it.
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix zoomScaleMatrix = Matrix.CreateScale(zoom.X, zoom.Y, 1f);

            // Get a matrix that aims towards the Z axis (these calculations are relative to a 2D world).
            effectView = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

            // Offset the matrix to the appropriate position based off the height.
            effectView *= Matrix.CreateTranslation(0f, -height, 0f);

            // Flip the matrix around 180 degrees.
            effectView *= Matrix.CreateRotationZ(MathHelper.Pi);

            // Account for the inverted gravity effect.
            if (Main.LocalPlayer.gravDir == -1f)
                effectView *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, height, 0f);

            // And account for the current zoom.
            effectView *= zoomScaleMatrix;

            // Create a projection in 2D using the screen width/height, and the zoom.
            effectProjection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth * zoom.X, 0f, Main.screenHeight * zoom.Y, 0f, 1f) * zoomScaleMatrix;
            BaseEffect.View = effectView;
            BaseEffect.Projection = effectProjection;
        }

        private void UpdateBaseEffectPixel(out Matrix effectProjetion, out Matrix effectView)
        {
            // Get the screen bounds.
            effectProjetion = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
            effectView = Matrix.Identity;
            BaseEffect.Projection = effectProjetion;
            BaseEffect.View = effectView;
        }

        private static List<Vector2> CorrectlyOffsetPoints(IEnumerable<Vector2> basePoints, Vector2 baseOffset, int totalPoints)
        {
            List<Vector2> newList = new();
            for (int i = 0; i < basePoints.Count(); i++)
            {
                // Don't incorporate points that are zeroed out.
                // They are almost certainly a result of incomplete oldPos arrays.
                if (basePoints.ElementAt(i) == Vector2.Zero)
                    continue;

                newList.Add(basePoints.ElementAt(i) + baseOffset);
            }

            // WAYY less demanding smoothening.
            if (newList.Count <= 1)
                return newList;

            List<Vector2> controlPoints = new();
            for (int i = 0; i < basePoints.Count(); i++)
            {
                // Don't incorporate points that are zeroed out.
                // They are almost certainly a result of incomplete oldPos arrays.
                if (basePoints.ElementAt(i) == Vector2.Zero)
                    continue;

                Vector2 offset = baseOffset;
                controlPoints.Add(basePoints.ElementAt(i) + offset);
            }
            List<Vector2> points = new();

            // Avoid stupid index errors.
            if (controlPoints.Count <= 4)
                return controlPoints;

            for (int j = 0; j < totalPoints; j++)
            {
                float splineInterpolant = j / (float)totalPoints;
                float localSplineInterpolant = splineInterpolant * (controlPoints.Count - 1f) % 1f;
                int localSplineIndex = (int)(splineInterpolant * (controlPoints.Count - 1f));

                Vector2 farLeft;
                Vector2 left = controlPoints[localSplineIndex];
                Vector2 right = controlPoints[localSplineIndex + 1];
                Vector2 farRight;

                // Special case: If the spline attempts to access the previous/next index but the index is already at the very beginning/end, simply
                // cheat a little bit by creating a phantom point that's mirrored from the previous one.
                if (localSplineIndex <= 0)
                {
                    Vector2 mirrored = left * 2f - right;
                    farLeft = mirrored;
                }
                else
                    farLeft = controlPoints[localSplineIndex - 1];

                if (localSplineIndex >= controlPoints.Count - 2)
                {
                    Vector2 mirrored = right * 2f - left;
                    farRight = mirrored;
                }
                else
                    farRight = controlPoints[localSplineIndex + 2];

                points.Add(Vector2.CatmullRom(farLeft, left, right, farRight, localSplineInterpolant));
            }

            // Manually insert the front and end points.
            points.Insert(0, controlPoints.First());
            points.Add(controlPoints.Last());

            return points;
        }

        private BasePrimTriangle[] CreatePrimitiveVertices(List<Vector2> points)
        {
            List<BasePrimTriangle> rectPrims = new();

            // Loop throught the points, ignoring the final one as it doesnt need to connect to anything.
            for (int i = 0; i < points.Count - 1; i++)
            {
                // How far along in the list of points we are.
                float trailCompletionRatio = i / (float)points.Count;

                // Get the current width and color from the delegates.
                float width = WidthFunc(trailCompletionRatio);
                Color color = ColorFunc(trailCompletionRatio);

                // Get the current point, and the point ahead (next in the list).
                Vector2 point = points[i];
                Vector2 aheadPoint = points[i + 1];

                // Get the direction to the ahead point, not calling DirectionTo for performance.
                Vector2 directionToAhead = (aheadPoint - point).SafeNormalize(Vector2.Zero);

                // Get the left and right coordinates, with the current trail completion for the X value.
                Vector2 leftCurrentTextureCoord = new(trailCompletionRatio, 0f);
                Vector2 rightCurrentTextureCoord = new(trailCompletionRatio, 1f);

                // Point 90 degrees away from the direction towards the next point, and use it to mark the edges of a rectangle.
                // This doesn't use RotatedBy for the sake of performance as well.
                Vector2 sideDirection = new(-directionToAhead.Y, directionToAhead.X);

                // This is defining a rectangle based on two triangles.
                // See https://cdn.discordapp.com/attachments/770382926545813515/1050185533780934766/a.png for a visual of this.
                // The two triangles can be imagined as the point being the tip, and the sides being the opposite side.
                // How to connect it all is defined in the CreatePrimitiveIndices() function.
                // The resulting rectangles combined are what make the trail itself.
                rectPrims.Add(new BasePrimTriangle(point - sideDirection * width, color, leftCurrentTextureCoord));
                rectPrims.Add(new BasePrimTriangle(point + sideDirection * width, color, rightCurrentTextureCoord));
            }

            return rectPrims.ToArray();
        }

        private static short[] CreatePrimitiveIndices(int totalPoints)
        {
            // What this is doing is basically representing each point on the vertices list as
            // indices. These indices should come together to create a tiny rectangle that acts
            // as a segment on the trail. This is achieved here by splitting the indices (or rather, points)
            // into 2 triangles, which requires 6 points. This is the aforementioned connecting of the
            // triangles using the indices.

            // Get the total number of indices, -1 because the last point doesn't connect to anything, and
            // * 6 because each point has 6 indices.
            int totalIndices = (totalPoints - 1) * 6;

            // Create an array to hold them with the correct size.
            short[] indices = new short[totalIndices];

            // Loop through the points, creating each indice.
            for (int i = 0; i < totalPoints - 2; i++)
            {
                // This might look confusing, but its basically going around the rectangle, and adding the points in the appropriate place.
                // Use this as a visual aid. https://cdn.discordapp.com/attachments/864078125657751555/1050218596623716413/image.png
                int startingTriangleIndex = i * 6;
                int connectToIndex = i * 2;
                indices[startingTriangleIndex] = (short)connectToIndex;
                indices[startingTriangleIndex + 1] = (short)(connectToIndex + 1);
                indices[startingTriangleIndex + 2] = (short)(connectToIndex + 2);
                indices[startingTriangleIndex + 3] = (short)(connectToIndex + 2);
                indices[startingTriangleIndex + 4] = (short)(connectToIndex + 1);
                indices[startingTriangleIndex + 5] = (short)(connectToIndex + 3);
            }
            // Return the array.
            return indices;
        }
        #endregion
    }
}
