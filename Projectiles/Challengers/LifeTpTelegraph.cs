using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeTpTelegraph : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Teleport Telegraph");
		}
		public override void SetDefaults()
		{
			Projectile.width = 150;
			Projectile.height = 150;
			Projectile.aiStyle = 0;
			Projectile.hostile = false;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 160;
			Projectile.light = 1f;
		}

		public override void AI()
		{
			if (Projectile.ai[0] > 0f)
			{
				Projectile.Kill();
			}
			Projectile.ai[0] += 1f;
		}
	}
}
