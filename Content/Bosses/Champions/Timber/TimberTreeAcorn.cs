using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Timber
{
    public class TimberTreeAcorn : TimberAcorn
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Timber/TimberAcorn";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center - Projectile.velocity - Vector2.UnitY * 160, Vector2.Zero,
                    ModContent.ProjectileType<TimberTree>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
            }
        }
    }
}