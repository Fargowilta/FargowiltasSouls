using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class FishStickWhirlpool : FishStickProj
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/FishStickProj";

        public const int TornadoHeight = 12;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.timeLeft = 30;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            if (Projectile.owner == Main.myPlayer)
            {
                foreach (Projectile tornado in Main.projectile.Where(p => p.active && p.ModProjectile is Whirlpool))
                {
                    tornado.Kill();
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<WhirlpoolBase>(), Projectile.damage, Projectile.knockBack,
                    Projectile.owner, 10, TornadoHeight);
            }
        }
    }
}