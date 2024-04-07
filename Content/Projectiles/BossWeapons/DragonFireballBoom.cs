using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class DragonFireballBoom : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_612";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.SolarWhipSwordExplosion];
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.scale = 2;
            Projectile.tileCollide = false;
            //CooldownSlot = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Fuchsia;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame > Main.projFrames[Projectile.type])
                Projectile.Kill();
        }
    }
}