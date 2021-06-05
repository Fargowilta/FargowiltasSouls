namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexBolt : Projectiles.LightningArc
    {
		public override string Texture => "Terraria/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.ranged = false;
            projectile.magic = true;

            projectile.usesIDStaticNPCImmunity = false;
            projectile.idStaticNPCHitCooldown = 0;

            projectile.timeLeft = 30 * (projectile.extraUpdates + 1);
        }
    }
}
