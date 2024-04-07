using Terraria;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    internal class BlenderSpray : DicerSpray
    {
        public override string Texture => "Terraria/Images/Projectile_484";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 60;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.immune[projectile.owner] = 6;
        }
    }
}