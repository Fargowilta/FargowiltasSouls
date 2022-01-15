using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SeedPlanterasChild : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_275";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Seed");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 1)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 1)
                    projectile.frame = 0;
            }
        }
    }
}