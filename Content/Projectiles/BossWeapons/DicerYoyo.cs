using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    internal class DicerYoyo : ModProjectile
    {
        public int Counter = 1;

        public override void SetStaticDefaults()
        {
            // Vanilla values range from 3f(Wood) to 16f(Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 16f;
            // Vanilla values range from 130f(Wood) to 400f(Terrarian), and defaults to 200f
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 400;
            // Vanilla values range from 9f(Wood) to 17.5f(Terrarian), and defaults to 10f
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15f;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Kraken);
            Projectile.extraUpdates = 0;
            Projectile.width = 30;
            Projectile.height = 30;
            //yoyo ai
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;

            Projectile.extraUpdates = 1;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            if (++Counter >= 60)
            {
                Counter = 0;

                if (Projectile.owner == Main.myPlayer)
                {
                    int proj2 = ModContent.ProjectileType<DicerMine>();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, proj2, Projectile.damage, 0, Main.myPlayer);
                }
            }
        }

        public override void PostAI()
        {
            /*if (Main.rand.NextBool())
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 1.6f;
            }*/
        }
    }
}