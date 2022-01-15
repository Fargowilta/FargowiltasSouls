using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class DungeonGuardianNecro : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_197";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dungeon Guardian");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

		public override void SetDefaults()
		{
			projectile.width = 42;
			projectile.height = 48;
			projectile.aiStyle = 0;
			aiType = ProjectileID.Bullet;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.timeLeft = 1000;
		}
		
        public override void AI()
        {
            projectile.rotation += 0.2f;

            const int aislotHomingCooldown = 0;
            //int homingDelay = 10;
            int homingDelay = (int) projectile.ai[1];
            const float desiredFlySpeedInPixelsPerFrame = 45;
            const float amountOfFramesToLerpBy = 25; // minimum of 1, please keep in full numbers even though it's  float!

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

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = Main.player[projectile.owner].GetModPlayer<FargoPlayer>().TerrariaSoul;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new Vector2(projectile.Center.X + Main.rand.Next(-20, 20), projectile.Center.Y + Main.rand.Next(-20, 20));
                int dust = Dust.NewDust(pos, projectile.width, projectile.height, DustID.Blood, 0, 0, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}

