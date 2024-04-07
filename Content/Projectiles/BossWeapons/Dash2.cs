namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class Dash2 : Dash
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/Dash";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 15 * 60 * (Projectile.extraUpdates + 1);
        }
    }
}