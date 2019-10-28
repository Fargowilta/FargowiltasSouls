using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasSouls.MonsterMadhouse
{
    class ProgressBar
    {
        public void DrawBar(SpriteBatch spriteBatch)
        {
            float alpha = 0.5f;
            Texture2D backGround1 = Main.colorBarTexture;
            Texture2D progressColor = Main.colorBarTexture;
            Texture2D MMIcon = Fargowiltas.Instance.GetTexture("MonsterMadhouse/MMIcon");
            float scmp = 0.5f + 0.75f * 0.5f;
            Color descColor = new Color(77, 39, 135);
            Color waveColor = new Color(255, 241, 51);
            const int offsetX = 20;
            const int offsetY = 20;
            int width = (int)(200f * scmp);
            int height = (int)(46f * scmp);
            Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
            Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);
            float cleared = MMWorld.MMPoints / MMWorld.MMWaveThreshhold;
            string waveText = "Cleared " + Math.Round(100 * cleared) + "%";
            Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y + 5), Color.White, scmp * 0.8f, 0.5f, -0.1f);
            Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
            Rectangle waveProgressAmount = new Rectangle(0, 0, (int)(progressColor.Width * MathHelper.Clamp(cleared, 0f, 1f)), progressColor.Height);
            Vector2 offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * scmp)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scmp)) * 0.5f);
            spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
            spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
            const int internalOffset = 6;
            Vector2 descSize = new Vector2(154, 40) * scmp;
            Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
            Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize * .8f);
            Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);
            int descOffset = (descBackground.Height - (int)(32f * scmp)) / 2;
            Rectangle icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * scmp), (int)(32 * scmp));
            spriteBatch.Draw(MMIcon, icon, Color.White);
            Utils.DrawBorderString(spriteBatch, "Monster Madhouse", new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
        }
    }
}
