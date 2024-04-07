using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class ElfArcherArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frostburn Arrow");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FrostArrow);
            AIType = ProjectileID.FrostArrow;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.arrow = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.coldDamage = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            target.AddBuff(BuffID.Chilled, 120);
        }
    }
}