using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MechElectricOrbHoming : MechElectricOrb
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/MechElectricOrb";
        public override void AI()
        {
            base.AI();

            if (Projectile.ai[1] == 0)
            {
                bool playerExists = Projectile.ai[0] > -1 && Projectile.ai[0] < Main.maxPlayers;
                bool proceed = false;

                if (playerExists)
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)Projectile.ai[0]].Center - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.2f));
                    if (vel.Length() < 300 || vel.Length() > 3000 || !Main.player[(int)Projectile.ai[0]].active || Main.player[(int)Projectile.ai[0]].dead || Main.player[(int)Projectile.ai[0]].ghost)
                        proceed = true;
                }

                if (!playerExists || proceed)
                {
                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                    Projectile.timeLeft = 180;
                    Projectile.velocity.Normalize();
                }
            }
            else if (Projectile.ai[1] > 0)
            {
                if (Projectile.ai[0] > -1 && Projectile.ai[0] < Main.maxPlayers) //homing mode
                {
                    if (++Projectile.ai[1] < 100)
                    {
                        Projectile.velocity *= 1.035f;

                        float rotation = Projectile.velocity.ToRotation();
                        Vector2 vel = Main.player[(int)Projectile.ai[0]].Center - Projectile.Center;
                        float targetAngle = vel.ToRotation();
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.035f));
                    }
                }
                else //straight accel mode
                {
                    if (++Projectile.ai[1] < 75)
                        Projectile.velocity *= 1.06f;
                }
            }
            else //ai1 below 0 rn
            {
                Projectile.ai[1]++;
            }
        }
    }
}