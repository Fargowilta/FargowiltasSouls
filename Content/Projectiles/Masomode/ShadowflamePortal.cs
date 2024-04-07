using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class ShadowflamePortal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_673";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadowflame Portal");
        }

        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.scale *= 0.5f;

            Projectile.timeLeft = 80;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.alpha -= 12;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Shadowflame);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 4f;
            Main.dust[d].scale += 0.5f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 150, 255, 150) * Projectile.Opacity;

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 speed = 8f * Projectile.ai[0].ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(30));
                    float ai1 = Main.rand.Next(10, 80) * (1f / 1000f);
                    if (Main.rand.NextBool())
                        ai1 *= -1f;
                    float ai0 = Main.rand.Next(10, 80) * (1f / 1000f);
                    if (Main.rand.NextBool())
                        ai0 *= -1f;
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, speed, ProjectileID.ShadowFlame, Projectile.damage, 0f, Main.myPlayer, ai0, ai1);
                }
            }
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