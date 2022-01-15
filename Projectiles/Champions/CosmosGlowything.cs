using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosGlowything : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cosmic Invader");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.scale = 0.5f;
            cooldownSlot = 1;
        }

        float scalefactor;
        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                scalefactor = 0.07f;
            }
            else
            {
                scalefactor -= 0.005f;
            }
            projectile.scale += scalefactor;


            if (projectile.scale > 2f)
            {
                projectile.ai[0]++;
                scalefactor = 0f;
            }

            if (projectile.scale <= 0)
                projectile.Kill();

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glow = Main.projectileTexture[projectile.type];
            int rect1 = glow.Height;
            int rect2 = 0;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(196, 247, 255, 0), Color.Transparent, 0.8f);

            Color color27 = glowcolor;
            float scale = projectile.scale;
            Main.spriteBatch.Draw(glow, projectile.Center + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 2, SpriteEffects.None, 0f);


            return false;
        }
    }
}