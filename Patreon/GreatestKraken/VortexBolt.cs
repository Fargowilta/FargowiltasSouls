using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexBolt : Projectiles.LightningArc
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;

            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = false;

            Projectile.timeLeft = 30 * (Projectile.extraUpdates + 1);
        }
    }
}
