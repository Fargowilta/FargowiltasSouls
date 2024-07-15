using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{
    public class LifeHomingProjSmall : LifeProjSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 60 * 4;
        }
        public override string Texture => "FargowiltasSouls/Content/Bosses/Lifelight/LifeProjSmall";
        public override void AI()
        {
            Projectile.velocity *= 1.006f;
            Projectile.velocity = Projectile.velocity.ClampLength(0, 40);

            if (Projectile.ai[0] < 120)
            {
                float modifier = Projectile.ai[0] / 120;
                modifier = MathHelper.Clamp(modifier, 0, 1);
                Player player = FargoSoulsUtil.PlayerExists(Player.FindClosest(Projectile.Center, 0, 0));
                if (player != null && player.Alive())
                {
                    Projectile.velocity = Projectile.velocity.RotateTowards(Projectile.DirectionTo(player.Center).ToRotation(), 0.02f * modifier);
                }

            }
            

            base.AI();
        }
    }
}
