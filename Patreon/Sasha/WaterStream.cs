using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Sasha
{
    public class WaterStream : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Water Stream");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WaterStream);
            aiType = ProjectileID.WaterStream;

            projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
        }
    }
}