using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class ProbeLaser : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_389";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Probe Laser");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            aiType = ProjectileID.MiniRetinaLaser;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.alpha = 255;
            projectile.extraUpdates = 2;
            projectile.scale = 1.2f;
            projectile.timeLeft = 600;
            projectile.minion = true;
            projectile.tileCollide = false;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.56f, 0f, 0.35f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}