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
        public override void AI()
		{
            if (Timer == 45f)
			{
				int damage = Projectile.damage;
				float knockBack = 3f;

				if (Projectile.ai[1] != 2)
				{
					Projectile.velocity *= 2;
					if (Projectile.velocity.Length() < 15)
						Projectile.velocity = 15 * Vector2.Normalize(Projectile.velocity);
				}

				Vector2 shootoffset1 = Projectile.velocity.RotatedBy(-Math.PI / 3.0);
				Vector2 shootoffset2 = Projectile.velocity.RotatedBy(Math.PI / 3.0);

				int type = Projectile.ai[1] == 2 && FargoSoulsWorld.EternityMode ? Projectile.type : ModContent.ProjectileType<LifeProjSmall>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset1, type, damage, knockBack, Main.myPlayer, Projectile.ai[0] - 10000000);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset2, type, damage, knockBack, Main.myPlayer,  Projectile.ai[0] - 20000000);
				}

				if (Projectile.ai[1] == 2 && FargoSoulsWorld.EternityMode)
					Projectile.Kill();
			}

            base.AI();
		}
    }
}
