using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Whirlpool : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_386";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Whirlpool");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 75;
            projectile.height = 21;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.scale = 0.5f;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            int num599 = 16;
            int num600 = 16;
            float num601 = 1.5f;
            int num602 = 150;
            int num603 = 42;

            if (projectile.velocity.X != 0f) projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);
            projectile.frameCounter++;
            if (projectile.frameCounter > 2)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }

            if (projectile.frame >= 6) projectile.frame = 0;
            if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
            {
                projectile.localAI[0] = 1f;
                projectile.position.X = projectile.position.X + projectile.width / 2;
                projectile.position.Y = projectile.position.Y + projectile.height / 2;
                projectile.scale = (num599 + num600 - projectile.ai[1]) * num601 / (num600 + num599);
                projectile.width = (int) (num602 * projectile.scale);
                projectile.height = (int) (num603 * projectile.scale);
                projectile.position.X = projectile.position.X - projectile.width / 2;
                projectile.position.Y = projectile.position.Y - projectile.height / 2;
                projectile.netUpdate = true;
            }

            if (projectile.ai[1] != -1f)
            {
                projectile.scale = (num599 + num600 - projectile.ai[1]) * num601 / (num600 + num599);
                projectile.width = (int) (num602 * projectile.scale);
                projectile.height = (int) (num603 * projectile.scale);
            }

            if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 100) projectile.alpha = 100;
            }
            else
            {
                projectile.alpha += 30;
                if (projectile.alpha > 150) projectile.alpha = 150;
            }

            if (projectile.ai[0] > 0f) projectile.ai[0] -= 1f;
            if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
            {
                projectile.netUpdate = true;
                Vector2 center = projectile.Center;
                center.Y -= num603 * projectile.scale / 2f;
                float num604 = (num599 + num600 - projectile.ai[1] + 1f) * num601 / (num600 + num599);
                center.Y -= num603 * num604 / 2f;
                center.Y += 2f;
                Projectile.NewProjectile(center.X, center.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<Whirlpool>(), projectile.damage, projectile.knockBack, projectile.owner, 10f,
                    projectile.ai[1] - 1f);
                int num605 = 2;
            }

            if (projectile.ai[0] <= 0f)
            {
                float num608 = 0.104719758f;
                float num609 = projectile.width / 5f;
                if (projectile.type == 386) num609 *= 2f;
                float num610 = (float) (Math.Cos(num608 * -(double) projectile.ai[0]) - 0.5) * num609;
                projectile.position.X = projectile.position.X - num610 * -(float) projectile.direction;
                projectile.ai[0] -= 1f;
                num610 = (float) (Math.Cos(num608 * -(double) projectile.ai[0]) - 0.5) * num609;
                projectile.position.X = projectile.position.X + num610 * -(float) projectile.direction;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num254 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, projectile.direction * 2, 0f, 100, default(Color), 1.4f);
                Dust dust13 = Main.dust[num254];
                dust13.color = Color.CornflowerBlue;
                dust13.color = Color.Lerp(dust13.color, Color.White, 0.3f);
                dust13.noGravity = true;
            }
        }
    }
}