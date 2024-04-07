using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class EyeFireFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_101";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire);
            AIType = ProjectileID.EyeFire;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.tileCollide = true;
            Projectile.penetrate = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }
    }
}