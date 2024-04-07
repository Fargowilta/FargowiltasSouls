using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Sky
{
    public class MainMenuBackgroundStyle : ModSurfaceBackgroundStyle
    {
        //theres some unnecessary clutter in here but oh well

        private float intensity = 0.6f;
        private float lifeIntensity = 1f;
        private float specialColorLerp = 0f;
        private Color? specialColor = null;
        private int delay = 0;
        private readonly int[] xPos = new int[50];
        private readonly int[] yPos = new int[50];
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public int fadeIn = 0;
        private Color ColorToUse(ref float opacity)
        {
            Color color = new(51, 255, 191);
            opacity = intensity * 0.5f + lifeIntensity * 0.5f;
            opacity *= Math.Min(fadeIn / 60f, 1);

            if (specialColorLerp > 0 && specialColor != null)
            {
                color = Color.Lerp(color, (Color)specialColor, specialColorLerp);
                if (specialColor == Color.Black)
                    opacity = Math.Min(1f, opacity + Math.Min(intensity, lifeIntensity) * 0.5f);
            }

            return color;
        }
        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            fadeIn++;
            float opacity = 0f;
            Color color = ColorToUse(ref opacity);

            spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color * opacity);

            if (--delay < 0)
            {
                delay = Main.rand.Next(5 + (int)(85f * (1f - lifeIntensity)));
                for (int i = 0; i < 50; i++) //update positions
                {
                    xPos[i] = Main.rand.Next(Main.screenWidth);
                    yPos[i] = Main.rand.Next(Main.screenHeight);
                }
            }

            for (int i = 0; i < 50; i++) //static on screen
            {
                int width = Main.rand.Next(3, 251);
                spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantStatic", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                new Rectangle(xPos[i] - width / 2, yPos[i], width, 3),
                color * 1 * 0.75f);
            }
            return base.PreDrawCloseBackground(spriteBatch);

            /*
            spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantSky2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * 0.9f);
            return base.PreDrawCloseBackground(spriteBatch);
            */
        }

    }
}