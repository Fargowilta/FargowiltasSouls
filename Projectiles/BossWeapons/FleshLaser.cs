using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class FleshLaser : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_100";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Red Laser");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.PurpleLaser);
            aiType = ProjectileID.PurpleLaser;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}