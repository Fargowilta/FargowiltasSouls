using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class FleshLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_100";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Red Laser");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PurpleLaser);
            AIType = ProjectileID.PurpleLaser;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}