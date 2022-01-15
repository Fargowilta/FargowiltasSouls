using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GoblinSpikyBall : ModProjectile
    {
	
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spiky Ball");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.SpikyBall);
            aiType = ProjectileID.SpikyBall;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.thrown = false;
            projectile.penetrate = 1;
            projectile.timeLeft /= 6;
            projectile.scale = 1.5f;
        }

        public override void AI()
        {
            if (Main.invasionType != 1 && projectile.timeLeft > 90)
                projectile.timeLeft = 90; //despawn fast outside goblin event

            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, 0, 0, 100, default(Color), 3f);
                Main.dust[dustId].noGravity = true;
                Main.dust[dustId].velocity *= 8f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }
    }
}
