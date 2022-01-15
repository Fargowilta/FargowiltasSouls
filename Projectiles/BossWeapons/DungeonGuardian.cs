using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class DungeonGuardian : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dungeon Guardian");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

		public override void SetDefaults()
		{
			projectile.width = 80;
			projectile.height = 102;
			projectile.aiStyle = 0;
			aiType = ProjectileID.Bullet;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.timeLeft = 600;
		}
		
        public override void AI()
        {
            projectile.rotation += 0.2f;

            const int aislotHomingCooldown = 0;
            //int homingDelay = 10;
            int homingDelay = (int) projectile.ai[1];
            const float desiredFlySpeedInPixelsPerFrame = 60;
            const float amountOfFramesToLerpBy = 20; // minimum of 1, please keep in full numbers even though it's  float!

            projectile.ai[aislotHomingCooldown]++;
            if (projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000));
                if (n != null)
                {
                    Vector2 desiredVelocity = projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 1;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new Vector2(projectile.position.X, projectile.position.Y);
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}

