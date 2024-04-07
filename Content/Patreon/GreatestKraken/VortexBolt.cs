using FargowiltasSouls.Content.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.GreatestKraken
{
    public class VortexBolt : LightningArc
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
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = false;

            Projectile.timeLeft = 30 * (Projectile.extraUpdates + 1);
        }
    }
}
