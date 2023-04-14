using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{
    public class LifeSplittingProjSmall : LifeProjSmall
	{
        public override string Texture => "FargowiltasSouls/Projectiles/Challengers/LifeProjSmall";
        public override void AI()
		{
            if (Projectile.ai[0] == 45f)
			{
				int damage = Projectile.damage;
				float knockBack = 3f;

                Projectile.velocity *= 2;
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity = 15 * Vector2.Normalize(Projectile.velocity);

                Vector2 shootoffset1 = Projectile.velocity.RotatedBy(-Math.PI / 3.0);
				Vector2 shootoffset2 = Projectile.velocity.RotatedBy(Math.PI / 3.0);


                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (Projectile.ai[1] != 11 && Projectile.ai[1] != 9)
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset1, ModContent.ProjectileType<LifeProjSmall>(), damage, knockBack, Main.myPlayer);
                    if (Projectile.ai[1] != 10 && Projectile.ai[1] != 8)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset2, ModContent.ProjectileType<LifeProjSmall>(), damage, knockBack, Main.myPlayer);
                }
			}

            base.AI();
		}
    }
}
