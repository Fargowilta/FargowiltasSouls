using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeScar : ModProjectile
	{
		bool init = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Mine");
            Main.projFrames[Projectile.type] = 3;
        }
		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
			Projectile.Opacity = 0f;
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //circular hitbox
		{
			int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
			int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

			if (Math.Abs(clampedX) > targetHitbox.Width / 2)
				clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
			if (Math.Abs(clampedY) > targetHitbox.Height / 2)
				clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

			int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
			int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

			return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
		}

		public override void AI()
		{
			if (!init)
			{
				Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
				init = true;
			}
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.5f;
            if (Projectile.ai[0] <= 60f)
            {
                Projectile.Opacity += 3 / 60f;
            }
            if (Projectile.ai[1] >= 1620)
			{
				Projectile.Opacity -= 1 / 60f;
			}
			if (Projectile.ai[1] >= 1650f)
			{
				Projectile.damage = 0;
			}
            if (Projectile.ai[1] > 1680f)
			{
				Projectile.Kill();
			}
			Projectile.ai[1]++;
			Projectile.ai[0]++;
		}
	}
}
