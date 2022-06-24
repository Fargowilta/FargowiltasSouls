using FargowiltasSouls.Projectiles.Masomode;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviBigMimic : DeviMimic
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biome Mimic");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 42;
        }

        public override void AI()
        {
            base.AI();

            Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
            if (player != null)
                Projectile.tileCollide = Projectile.position.Y + Projectile.height >= player.position.Y + player.height - 32;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 5; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position.X + Main.rand.Next(Projectile.width), Projectile.position.Y + Main.rand.Next(Projectile.height),
                        Main.rand.Next(-30, 31) * .1f, Main.rand.Next(-40, -15) * .1f, Terraria.ModLoader.ModContent.ProjectileType<FakeHeart>(), 20, 0f, Main.myPlayer);
                    if (p != Main.maxProjectiles && !FargoSoulsWorld.MasochistModeReal)
                        Main.projectile[p].timeLeft = 120 + Main.rand.Next(60);
                }
            }

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 200;
            Projectile.Center = Projectile.position;

            base.Kill(timeLeft);
        }
    }
}