using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviMedusa : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_3269";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Medusa Head");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BrainOfConfusion);
            Projectile.scale = 5f;
            AIType = ProjectileID.BrainOfConfusion;
        }

        public override void AI()
        {
            Projectile.frame = 0;
            Projectile.frameCounter = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}