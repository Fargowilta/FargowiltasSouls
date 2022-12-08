using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeCageTelegraph : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cage Telegraph");
        }
        public override void SetDefaults()
        {
            Projectile.width = 800;
            Projectile.height = 800;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.light = 2f;
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
