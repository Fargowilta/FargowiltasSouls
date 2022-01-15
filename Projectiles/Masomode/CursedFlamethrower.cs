using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CursedFlamethrower : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_101";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eye Fire");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeFire); //has 4 updates per tick
            aiType = ProjectileID.EyeFire;
            projectile.magic = false;
            projectile.tileCollide = false;
            projectile.width = 20;
            projectile.height = 400;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(6))
                target.AddBuff(39, 480, true);
            else if (Main.rand.NextBool(4))
                target.AddBuff(39, 300, true);
            else if (Main.rand.NextBool())
                target.AddBuff(39, 180, true);

            target.AddBuff(BuffID.OnFire, 300);
            /*target.AddBuff(mod.BuffType("ClippedWings"), 180);
            target.AddBuff(mod.BuffType("Crippled"), 60);*/
        }
    }
}