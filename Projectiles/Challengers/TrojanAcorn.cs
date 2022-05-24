using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Challengers
{
    public class TrojanAcorn : Champions.TimberAcorn
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/TimberAcorn";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.tileCollide = true;
            CooldownSlot = -1;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int index = 0; index < 10; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 7);
        }
    }
}