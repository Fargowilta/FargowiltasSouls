using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MechElectricOrbDestroyer : MechElectricOrb
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/MechElectricOrb";
        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Projectile.velocity.Length() / System.Math.Abs(Projectile.ai[1]);

            base.AI();

            if (++Projectile.ai[1] > 0)
            {
                if (Projectile.ai[1] > 30)
                    Projectile.velocity *= 1.06f;
                else if (Projectile.ai[1] == 30)
                {
                    Projectile.velocity = Projectile.ai[0].ToRotationVector2();
                    Projectile.timeLeft = 180;
                    Projectile.netUpdate = true;
                }
                else
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.ai[0].ToRotationVector2(), 0.1f);
            }
            else
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() - Projectile.localAI[1]);
            }
        }
    }
}