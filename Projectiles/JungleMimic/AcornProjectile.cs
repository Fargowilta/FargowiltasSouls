using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.JungleMimic
{
    public class AcornProjectile : ModProjectile
	{
        public float bounce = 1;
        public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Acorn");
            
		}
		public override void SetDefaults()
        {
            projectile.aiStyle = 0;
            projectile.width = 18;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.ignoreWater = false;
            projectile.tileCollide = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            bounce += 1;
            if (bounce == 4)
            {
                projectile.Kill();
            }
            
            if (projectile.velocity.X != oldVelocity.X && Math.Abs (oldVelocity.X) > 0.1f) {
                projectile.velocity.X = oldVelocity.X * -0.8f;
            }
            if (projectile.velocity.Y != oldVelocity.Y && Math.Abs (oldVelocity.Y) > 0.1f) {
                projectile.velocity.Y = oldVelocity.Y * -0.8f;
            }
            
            return false;
        }
		public override void AI()
        {
			projectile.rotation += 0.2f * (float)projectile.direction;
            if (projectile.velocity.Y < 0)
                projectile.velocity.Y += 0.3f;
            else
                projectile.velocity.Y += 0.4f;
		}
		public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("AcornProjectileExplosion"), projectile.damage / 2, projectile.knockBack, projectile.owner, 0f, 0f);
        	}
        	Main.PlaySound(SoundID.Item62, projectile.position);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 120;
			projectile.height = 120;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
        }

    }
}