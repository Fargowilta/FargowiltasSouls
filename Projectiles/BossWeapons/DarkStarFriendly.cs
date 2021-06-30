using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class DarkStarFriendly : Masomode.DarkStar
    {
        public override string Texture => "Terraria/Projectile_12";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.timeLeft = 75;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.rotation + (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;
            projectile.soundDelay = 0;
            if(projectile.velocity.Length() < 22) //fix stars not being aligned properly by making sure their total velocity is the same???
            {
                projectile.velocity.Normalize();
                projectile.velocity *= 22;
            }
            projectile.velocity *= 1.02f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
        }

        public override bool PreKill(int timeleft)
        {
            int num1 = 10;
            int num2 = 3;

            for (int index = 0; index < num1; ++index)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
            for (int index = 0; index < num2; ++index)
            {
                int Type = Main.rand.Next(16, 18);
                if (projectile.type == 503)
                    Type = 16;
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Type, 1f);
            }

            for (int index = 0; index < 10; ++index)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
            for (int index = 0; index < 3; ++index)
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);

            return false;
        }
    }
}