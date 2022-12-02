using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeCrosshair : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crosshair");
        }
        public override void SetDefaults()
        {
            Projectile.width = 184;
            Projectile.height = 184;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 120;
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
