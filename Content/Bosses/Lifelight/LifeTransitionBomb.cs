using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeTransitionBomb : LifeBomb
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Lifelight/LifeBomb";

        Vector2 OriginalPosition = Vector2.Zero;
        Vector2 DesiredPosition = Vector2.Zero;
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                OriginalPosition = Projectile.position;
                DesiredPosition = new Vector2(Projectile.ai[1], Projectile.ai[2]);
                Projectile.rotation = Projectile.SafeDirectionTo(DesiredPosition).ToRotation() - (float)Math.PI / 2;
            }
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 0.25f);


            Projectile.ai[0] += 1f;
            Projectile.Center = Vector2.Lerp(OriginalPosition, DesiredPosition, Math.Min(Projectile.ai[0] / 90f, 1));
            if (Projectile.ai[0] >= 100f)
            {
                Projectile.Kill();
            }
        }
    }
}
