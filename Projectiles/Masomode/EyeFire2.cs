using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class EyeFire2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_96";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeFire); //has 4 updates per tick
            aiType = ProjectileID.EyeFire;
            projectile.magic = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.alpha = 0;
        }

        public override void AI()
        {
            projectile.rotation += 0.3f;
        }

        public override void Kill(int timeLeft)
        {
            for (int index1 = 0; index1 < 5; ++index1) //vanilla code
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, default, 2f * projectile.scale);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, default, 1f * projectile.scale);
                Main.dust[index3].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(39, 480, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(39, 300, true);
            else if (Main.rand.NextBool())
                target.AddBuff(39, 180, true);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}