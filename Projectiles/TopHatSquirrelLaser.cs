using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class TopHatSquirrelLaser : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_257";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Laser");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.PurpleLaser);
            aiType = ProjectileID.PurpleLaser;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.light = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}