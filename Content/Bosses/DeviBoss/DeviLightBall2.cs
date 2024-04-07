using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviLightBall2 : DeviLightBall
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/LightBall";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.timeLeft = 600;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;
            }
        }
    }
}