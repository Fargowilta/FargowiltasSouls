using FargowiltasSouls.Buffs.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpazmarang : MutantRetirang
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Spazmarang";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spazmarang");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
        }
    }
}