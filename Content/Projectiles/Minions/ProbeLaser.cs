using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class ProbeLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_389";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Probe Laser");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.MiniRetinaLaser;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1.2f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.56f, 0f, 0.35f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}