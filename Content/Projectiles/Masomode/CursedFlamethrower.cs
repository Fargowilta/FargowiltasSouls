using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class CursedFlamethrower : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_101";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire); //has 4 updates per tick
            AIType = ProjectileID.EyeFire;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.width = 20;
            Projectile.height = 400;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(39, 480, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(39, 300, true);
            else if (Main.rand.NextBool())
                target.AddBuff(39, 180, true);

            target.AddBuff(BuffID.OnFire, 300);
            /*target.AddBuff(ModContent.BuffType<ClippedWings>(), 180);
            target.AddBuff(ModContent.BuffType<Crippled>(), 60);*/
        }
    }
}