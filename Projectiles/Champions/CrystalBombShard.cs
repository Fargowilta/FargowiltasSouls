using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CrystalBombShard : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_90";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Shard");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.CrystalShard);
            aiType = ProjectileID.CrystalShard;
            projectile.friendly = false;
            projectile.ranged = false;
            projectile.hostile = true;
            projectile.alpha = 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}