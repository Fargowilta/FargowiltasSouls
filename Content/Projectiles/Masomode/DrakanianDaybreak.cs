using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class DrakanianDaybreak : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Daybreak");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Daybreak);
            AIType = ProjectileID.Daybreak;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Default;
        }

        public override void AI()
        {
            Projectile.alpha = 0;
            Projectile.hide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 900);
            target.AddBuff(BuffID.Burning, 180);
        }
    }
}