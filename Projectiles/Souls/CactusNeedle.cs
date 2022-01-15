using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class CactusNeedle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cactus Needle");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.PineNeedleFriendly);
            projectile.aiStyle = 336;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            cooldownSlot = 1;
        }

	public override void AI()
	{
	    projectile.ai[0] += 1f;
	    
	    if (projectile.ai[0] >= 50f)
	    {
		projectile.ai[0] = 50f;
		projectile.velocity.Y += 0.5f;
	    }
	    if (projectile.ai[0] >= 15f)
	    {
		projectile.ai[0] = 15f;
		projectile.velocity.Y += 0.1f;
	    }
	    
	    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
	    
	    if (projectile.velocity.Y > 16f)
	    {
		projectile.velocity.Y = 16f;
	    }
	}
    }
}
