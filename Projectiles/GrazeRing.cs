using Microsoft.Xna.Framework;
using Terraria;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Projectiles
{
    public class GrazeRing : GlowRingHollow
    {
        public override string Texture => "FargowiltasSouls/Projectiles/GlowRingHollow";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Glow Ring");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.alpha = 0;
            projectile.hostile = false;
            projectile.friendly = true;
            color = Color.HotPink;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

            if (!player.active || player.dead || player.ghost || (projectile.owner == Main.myPlayer && (!fargoPlayer.Graze || !player.GetToggleValue("MasoGrazeRing", false))))
            {
                projectile.Kill();
                return;
            }

            float radius = Player.defaultHeight + fargoPlayer.GrazeRadius;

            projectile.timeLeft = 2;
            projectile.Center = player.Center;

            projectile.alpha = 0;

            projectile.scale = radius * 2f / 1000f;

            projectile.position = projectile.Center;
            projectile.width = projectile.height = (int)(1000 * projectile.scale);
            projectile.Center = projectile.position;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return base.GetAlpha(lightColor) * 0.8f;
        }
    }
}