using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class PumpkinFlame : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flame");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MolotovFire);
            AIType = ProjectileID.MolotovFire;

            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Generic;
        }

        public override void AI()
        {
            if (Projectile.wet)
            {
                Projectile.Kill();
            }

            int num199 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
            Dust expr_8956_cp_0 = Main.dust[num199];
            expr_8956_cp_0.position.X -= 2f;
            Dust expr_8974_cp_0 = Main.dust[num199];
            expr_8974_cp_0.position.Y += 2f;
            Main.dust[num199].scale += Main.rand.Next(50) * 0.01f;
            Main.dust[num199].noGravity = true;
            Dust expr_89C7_cp_0 = Main.dust[num199];
            expr_89C7_cp_0.velocity.Y -= 2f;
            if (Main.rand.NextBool())
            {
                int num200 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
                Dust expr_8A2E_cp_0 = Main.dust[num200];
                expr_8A2E_cp_0.position.X -= 2f;
                Dust expr_8A4C_cp_0 = Main.dust[num200];
                expr_8A4C_cp_0.position.Y += 2f;
                Main.dust[num200].scale += 0.3f + Main.rand.Next(50) * 0.01f;
                Main.dust[num200].noGravity = true;
                Main.dust[num200].velocity *= 0.1f;
            }
            if (Projectile.velocity.Y < 0.25 && Projectile.velocity.Y > 0.15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.8f;
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}