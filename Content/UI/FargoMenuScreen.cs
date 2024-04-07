using FargowiltasSouls.Content.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.UI
{
    public class FargoMenuScreen : ModMenu
    {
        bool forgor = false;
        public override Asset<Texture2D> Logo => forgor ?
            ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/ForgorMenuLogo") :
            ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/MenuLogo");

        //public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"");

        //public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"");

        public override int Music => ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Nein") : MusicID.MenuMusic;

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<MainMenuBackgroundStyle>();

        public override string DisplayName => Language.GetTextValue("Mods.FargowiltasSouls.UI.MainMenu");

        public override void OnSelected()
        {
            ((MainMenuBackgroundStyle)MenuBackgroundStyle).fadeIn = 0;
            forgor = Main.rand.NextBool(100);
            //SoundEngine.PlaySound(SoundID.Roar);
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            //drawColor = Color.Cyan; // Changes the draw color of the logo
            return true;
        }
    }
}