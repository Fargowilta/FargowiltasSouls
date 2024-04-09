using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Terraria.ID;

namespace FargowiltasSouls.Content.Patreon.Catsounds
{
    public class KingSlimeBallPiercing : SlimeBall
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/SlimeBall";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = 2;
            Projectile.DamageType = Terraria.ModLoader.DamageClass.Summon;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }
    }
}