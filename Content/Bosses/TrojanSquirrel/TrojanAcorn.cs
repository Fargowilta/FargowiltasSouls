using FargowiltasSouls.Content.Bosses.Champions.Timber;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.TrojanSquirrel
{
    public class TrojanAcorn : TimberAcorn
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Timber/TimberAcorn";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.tileCollide = true;
            CooldownSlot = -1;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int index = 0; index < 10; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
        }
    }
}