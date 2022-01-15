using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviBigMimic : DeviMimic
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Biome Mimic");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 48;
            projectile.height = 42;
        }

        public override void AI()
        {
            base.AI();

            Player player = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
            if (player != null)
                projectile.tileCollide = projectile.position.Y + projectile.height >= player.position.Y + player.height - 32;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 5; i++)
                    Projectile.NewProjectile(projectile.position.X + Main.rand.Next(projectile.width), projectile.position.Y + Main.rand.Next(projectile.height),
                        Main.rand.Next(-30, 31) * .1f, Main.rand.Next(-40, -15) * .1f, mod.ProjectileType("FakeHeart"), 20, 0f, Main.myPlayer);
            }

            projectile.position = projectile.Center;
            projectile.width = projectile.height = 200;
            projectile.Center = projectile.position;

            base.Kill(timeLeft);
        }
    }
}