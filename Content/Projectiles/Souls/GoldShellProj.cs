using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class GoldShellProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gold Shell");
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (player.dead)
            {
                modPlayer.GoldShell = false;
            }

            if (!modPlayer.GoldShell)
            {
                Projectile.Kill();
                return;
            }

            Projectile.position.X = Main.player[Projectile.owner].Center.X - Projectile.width / 2;
            Projectile.position.Y = Main.player[Projectile.owner].Center.Y - Projectile.height / 2;
        }
    }
}