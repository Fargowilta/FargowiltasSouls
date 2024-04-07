using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    internal class DicerMine : ModProjectile
    {
        public int Counter = 1;

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 19;
            Projectile.height = 19;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (Counter >= 75)
            {
                Projectile.scale += .1f;
                Projectile.rotation += 0.2f;
            }

            Counter++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int proj2 = ModContent.ProjectileType<DicerSpray>();
                Vector2 baseVel = Main.rand.NextBool() ? Vector2.UnitX : Vector2.UnitX.RotatedBy(Math.PI * 2 / 8 * 0.5);
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 5f * baseVel.RotatedBy(Math.PI * 2 / 8 * i), proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
            }
        }
    }
}