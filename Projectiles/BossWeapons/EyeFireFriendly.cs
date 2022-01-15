using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class EyeFireFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_101";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeFire);
            aiType = ProjectileID.EyeFire;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.magic = false;
            projectile.melee = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
        }
    }
}