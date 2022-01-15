﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class PumpkinFlame : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Empty";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Flame");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.MolotovFire);
            aiType = ProjectileID.MolotovFire;

            projectile.width = 14;
            projectile.height = 16;
        }

        public override void AI()
        {
			if (projectile.wet)
			{
				projectile.Kill();
			}

			int num199 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1f);
			Dust expr_8956_cp_0 = Main.dust[num199];
			expr_8956_cp_0.position.X = expr_8956_cp_0.position.X - 2f;
			Dust expr_8974_cp_0 = Main.dust[num199];
			expr_8974_cp_0.position.Y = expr_8974_cp_0.position.Y + 2f;
			Main.dust[num199].scale += (float)Main.rand.Next(50) * 0.01f;
			Main.dust[num199].noGravity = true;
			Dust expr_89C7_cp_0 = Main.dust[num199];
			expr_89C7_cp_0.velocity.Y = expr_89C7_cp_0.velocity.Y - 2f;
			if (Main.rand.NextBool())
			{
				int num200 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 1f);
				Dust expr_8A2E_cp_0 = Main.dust[num200];
				expr_8A2E_cp_0.position.X = expr_8A2E_cp_0.position.X - 2f;
				Dust expr_8A4C_cp_0 = Main.dust[num200];
				expr_8A4C_cp_0.position.Y = expr_8A4C_cp_0.position.Y + 2f;
				Main.dust[num200].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
				Main.dust[num200].noGravity = true;
				Main.dust[num200].velocity *= 0.1f;
			}
			if ((double)projectile.velocity.Y < 0.25 && (double)projectile.velocity.Y > 0.15)
			{
				projectile.velocity.X = projectile.velocity.X * 0.8f;
			}
			projectile.rotation = -projectile.velocity.X * 0.05f;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }
    }
}