using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantGlowything : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Retiray telegraph");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.5f;
            Projectile.alpha = 0;
            CooldownSlot = 1;
        }

        Vector2 spawnPoint;

        //readonly float scalefactor;
        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];

            if (spawnPoint == Vector2.Zero)
                spawnPoint = Projectile.Center;
            Projectile.Center = spawnPoint + Vector2.UnitX.RotatedBy(Projectile.ai[0]) * 96 * Projectile.scale;

            if (Projectile.scale < 4f) //grow over time
            {
                Projectile.scale += 0.2f;
            }
            else //if full size, start fading away
            {
                Projectile.scale = 4f;
                Projectile.alpha += 10;
            }
            if (Projectile.alpha > 255) //die if fully faded away
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int rect1 = glow.Height;
            int rect2 = 0;
            Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = new(255, 0, 0, 0);

            float scale = Projectile.scale;
            Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), Projectile.GetAlpha(glowcolor),
                Projectile.rotation, gloworigin2, scale * 2, SpriteEffects.None, 0);


            return false;
        }
    }
}