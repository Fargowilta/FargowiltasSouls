using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class TwentyTwo : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Player.defaultHeight;
            Projectile.height = Player.defaultHeight;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.ScaryScream, Projectile.Center); //arte scream
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;

                Color color26 = Projectile.GetAlpha(lightColor);

                float scale = Projectile.scale * Main.screenHeight / 254f / Main.GameZoomTarget;

                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}