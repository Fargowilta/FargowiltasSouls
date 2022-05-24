using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Whirlpool : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_386";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whirlpool");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 75;
            Projectile.height = 21;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 0.5f;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            int num599 = 16;
            int num600 = 16;
            float num601 = 1.5f;
            int num602 = 150;
            int num603 = 42;

            if (Projectile.velocity.X != 0f) Projectile.direction = Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame >= 6) Projectile.frame = 0;
            if (Projectile.localAI[0] == 0f && Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[0] = 1f;
                Projectile.position.X = Projectile.position.X + Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y + Projectile.height / 2;
                Projectile.scale = (num599 + num600 - Projectile.ai[1]) * num601 / (num600 + num599);
                Projectile.width = (int)(num602 * Projectile.scale);
                Projectile.height = (int)(num603 * Projectile.scale);
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[1] != -1f)
            {
                Projectile.scale = (num599 + num600 - Projectile.ai[1]) * num601 / (num600 + num599);
                Projectile.width = (int)(num602 * Projectile.scale);
                Projectile.height = (int)(num603 * Projectile.scale);
            }

            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 100) Projectile.alpha = 100;
            }
            else
            {
                Projectile.alpha += 30;
                if (Projectile.alpha > 150) Projectile.alpha = 150;
            }

            if (Projectile.ai[0] > 0f) Projectile.ai[0] -= 1f;
            if (Projectile.ai[0] == 1f && Projectile.ai[1] > 0f && Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
                Vector2 center = Projectile.Center;
                center.Y -= num603 * Projectile.scale / 2f;
                float num604 = (num599 + num600 - Projectile.ai[1] + 1f) * num601 / (num600 + num599);
                center.Y -= num603 * num604 / 2f;
                center.Y += 2f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), center.X, center.Y, Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<Whirlpool>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 10f,
                    Projectile.ai[1] - 1f);
                int num605 = 2;
            }

            if (Projectile.ai[0] <= 0f)
            {
                float num608 = 0.104719758f;
                float num609 = Projectile.width / 5f;
                if (Projectile.type == 386) num609 *= 2f;
                float num610 = (float)(Math.Cos(num608 * -(double)Projectile.ai[0]) - 0.5) * num609;
                Projectile.position.X = Projectile.position.X - num610 * -(float)Projectile.direction;
                Projectile.ai[0] -= 1f;
                num610 = (float)(Math.Cos(num608 * -(double)Projectile.ai[0]) - 0.5) * num609;
                Projectile.position.X = Projectile.position.X + num610 * -(float)Projectile.direction;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num254 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 172, Projectile.direction * 2, 0f, 100, default(Color), 1.4f);
                Dust dust13 = Main.dust[num254];
                dust13.color = Color.CornflowerBlue;
                dust13.color = Color.Lerp(dust13.color, Color.White, 0.3f);
                dust13.noGravity = true;
            }
        }
    }
}