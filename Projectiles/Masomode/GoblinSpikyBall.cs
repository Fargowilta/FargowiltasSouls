using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GoblinSpikyBall : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiky Ball");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SpikyBall);
            AIType = ProjectileID.SpikyBall;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Default;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            //if (Main.invasionType != 1 && Projectile.timeLeft > 90) Projectile.timeLeft = 90; //despawn fast outside goblin event

            int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.timeLeft = 0;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, 0, 0, 100, default(Color), 3f);
                Main.dust[dustId].noGravity = true;
                Main.dust[dustId].velocity *= 8f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}
