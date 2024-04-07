using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class Explosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged; //cringe
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.position.X = Projectile.position.X + Projectile.width / 2f;
            Projectile.position.Y = Projectile.position.Y + Projectile.height / 2f;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2f;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2f;

            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int i = 0; i < 5; i++)
            {
                float scaleFactor9 = 0.5f;
                if (i == 1 || i == 3) scaleFactor9 = 1f;

                for (int j = 0; j < 4; j++)
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                        default,
                        Main.rand.Next(61, 64));

                    Main.gore[gore].velocity *= scaleFactor9;
                    Main.gore[gore].velocity.X += 1f;
                    Main.gore[gore].velocity.Y += 1f;
                }
            }
        }
    }
}