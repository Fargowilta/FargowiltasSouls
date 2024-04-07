using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class Acorn : TrojanAcorn
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Timber/TimberAcorn";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acorn");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int index = 0; index < 10; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
        }
    }
}