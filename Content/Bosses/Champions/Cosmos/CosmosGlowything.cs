using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosGlowything : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Invader");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.5f;
            CooldownSlot = 1;
        }

        float scalefactor;
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                scalefactor = 0.07f;
            }
            else
            {
                scalefactor -= 0.005f;
            }
            Projectile.scale += scalefactor;


            if (Projectile.scale > 2f)
            {
                Projectile.ai[0]++;
                scalefactor = 0f;
            }

            if (Projectile.scale <= 0)
                Projectile.Kill();

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int rect1 = glow.Height;
            int rect2 = 0;
            Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(196, 247, 255, 0), Color.Transparent, 0.8f);

            Color color27 = glowcolor;
            float scale = Projectile.scale;
            Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 2, SpriteEffects.None, 0);


            return false;
        }
    }
}