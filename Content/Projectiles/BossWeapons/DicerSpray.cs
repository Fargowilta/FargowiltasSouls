using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    internal class DicerSpray : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_484";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SeedlerThorn);
            AIType = ProjectileID.SeedlerThorn;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }
    }
}