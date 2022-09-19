using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class FishStickProjTornado : FishStickProj
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/FishStickProj";

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<Whirlpool>() || p.type == ModContent.ProjectileType<WhirlpoolBase>()))
                    p.Kill();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WhirlpoolBase>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 16, 11);
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 59, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 59, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
        }
    }
}