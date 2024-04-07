using Terraria;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SlimeBallHoming : SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/SlimeBall";

        int bounce;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 2;
        }

        public override void AI()
        {
            base.AI();

            if (bounce == 0)
            {
                bounce = Projectile.timeLeft - Main.rand.Next(150);
            }

            if (Projectile.timeLeft == bounce)
            {
                bounce = 0;

                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * Projectile.velocity.Length();
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.immune[Projectile.owner] = 9;
        }
    }
}