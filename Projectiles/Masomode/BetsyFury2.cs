using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyFury2 : BetsyFury
    {
        public override string Texture => "Terraria/Images/Projectile_709";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);
            }

            if (++Projectile.localAI[0] < 120)
            {
                Projectile.velocity *= 1.033f;
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = Main.player[(int)Projectile.ai[0]].Center - Projectile.Center;
                float targetAngle = vel.ToRotation();
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.02f));
            }

            Projectile.alpha = Projectile.alpha - 30;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.4f, 0.85f, 0.9f);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            int num1 = 3;
            int num2 = 10;

            for (int index1 = 0; index1 < num1; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0.0f, 0.0f, 100, default, 1.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + Projectile.Center;
            }
            for (int index1 = 0; index1 < num2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0.0f, 0.0f, 0, default, 1.5f);
                Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + Projectile.Center;
                Main.dust[index2].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryCircle, Projectile.Center);
        }
    }
}