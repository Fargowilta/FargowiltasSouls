using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DrakanianDaybreak : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Daybreak");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Daybreak);
            aiType = ProjectileID.Daybreak;
            projectile.friendly = false;
            projectile.thrown = false;
            projectile.hostile = true;
        }

        public override void AI()
        {
            projectile.alpha = 0;
            projectile.hide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 900);
            target.AddBuff(BuffID.Burning, 180);
        }
    }
}