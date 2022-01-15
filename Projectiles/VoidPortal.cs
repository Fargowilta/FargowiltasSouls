using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class VoidPortal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Void Portal");
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.aiStyle = 0;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            aiType = ProjectileID.Bullet;
            Main.projFrames[projectile.type] = 10;
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            projectile.frameCounter++; //Making the timer go up.
            if (projectile.frameCounter >= 4) //Change the 4 to how fast you want the animation to be
            {
                projectile.frame++; //Making the frame go up...
                projectile.frameCounter = 0; //Resetting the timer.
                if (projectile.frame > 9) //Change the 3 to the amount of frames your projectile has.
                    projectile.frame = 0;
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int) projectile.position.X, (int) projectile.position.Y, 34);
            int dustId = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 2f), projectile.width + 5, projectile.height + 5, 62, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 0.8f);
            Main.dust[dustId].noGravity = true;
        }
    }
}