using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Critters
{
    public class TopHatSquirrelLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_257";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Squirrel's Judgement");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PurpleLaser);
            AIType = ProjectileID.PurpleLaser;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}