using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class VultureFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Vulture Feather");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.HarpyFeather);
            Projectile.aiStyle = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            CooldownSlot = 1; // do we need this?
        }

        public override void OnKill(int timeLeft)
        {
            for (int num610 = 0; num610 < 10; num610++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Harpy, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, Color.SandyBrown, 1f);
            }
        }
    }
}
