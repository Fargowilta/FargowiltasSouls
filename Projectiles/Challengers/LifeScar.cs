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
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.Opacity = 0f;
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

            if (++Projectile.localAI[0] <= 60f)
            {
                Projectile.Opacity += 3 / 60f;
            }

            if (++Projectile.ai[0] >= Projectile.ai[1] - 30)
			{
				Projectile.Opacity -= 1 / 60f;
			}
			if (Projectile.ai[0] >= Projectile.ai[1])
			{
				Projectile.damage = 0;
			}
            if (Projectile.ai[0] > Projectile.ai[1] + 30)
			{
				Projectile.Kill();
			}

            //pulsate
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] += Main.rand.Next(60);
            Projectile.scale = 1.1f + 0.1f * (float)Math.Sin(MathHelper.TwoPi / 15 * ++Projectile.localAI[1]);

			
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }

		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity;
	}
}
