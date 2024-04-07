using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Sasha
{
    public class WaterStream : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Stream");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterStream);
            AIType = ProjectileID.WaterStream;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        //public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        //{
        //    target.immune[Projectile.owner] = 7;
        //}
    }
}