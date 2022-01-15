using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GeyserFriendly : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Explosion";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Geyser");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.GeyserTrap);
            projectile.trap = false;
            projectile.hostile = false;
            aiType = ProjectileID.GeyserTrap;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }
    }
}