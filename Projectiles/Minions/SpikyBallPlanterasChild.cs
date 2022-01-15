using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SpikyBallPlanterasChild : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_277";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spiky Ball");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.ignoreWater = true;
            projectile.aiStyle = 14;
            aiType = ProjectileID.SpikyBallTrap;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = oldVelocity.X * -0.95f;
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 1f)
                projectile.velocity.Y = oldVelocity.Y * -0.95f;

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("Infested"), 360);
            target.AddBuff(BuffID.Venom, 360);
            target.AddBuff(BuffID.Poisoned, 360);
        }
    }
}