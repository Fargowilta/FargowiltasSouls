using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.ChallengerItems
{
    public class SproutingAcorn : Acorn
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/TimberAcorn";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int sproutType = ModContent.ProjectileType<Sprout>();
            if (Projectile.owner == Main.myPlayer && !Main.projectile.Any(p => p.active && p.type == sproutType && p.owner == Projectile.owner && p.ai[0] == target.whoAmI))
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), target.Center, Vector2.Zero, sproutType, Projectile.damage, 0f, Main.myPlayer, target.whoAmI);
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int index = 0; index < 10; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 7);
        }
    }
}