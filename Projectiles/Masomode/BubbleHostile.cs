using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BubbleHostile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_410";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bubble);
            AIType = ProjectileID.Bubble;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 600);
        }
    }
}