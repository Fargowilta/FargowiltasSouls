using Terraria;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviGuardianHarmless : DeviGuardian
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.Opacity = 0.5f;
        }
        public override bool? CanDamage() => false;
        public override void PostAI()
        {
            base.PostAI();
            Projectile.Opacity -= 0.5f / 60;
        }
    }
}