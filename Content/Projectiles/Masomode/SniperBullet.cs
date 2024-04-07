using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SniperBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("High Velocity Crystal Bullet");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BulletHighVelocity);
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 1800);

            /*int buffTime = 300;
            target.AddBuff(ModContent.BuffType<Crippled>(), buffTime);
            target.AddBuff(ModContent.BuffType<ClippedWings>(), buffTime);*/
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            for (int index1 = 0; index1 < 40; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, 0f, 0, default, 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 1.5f;
                Main.dust[index2].scale *= 0.9f;
            }

            if (FargoSoulsUtil.HostCheck)
            {
                for (int index = 0; index < 24; ++index)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.Next(30, 60) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -Projectile.velocity.Y * Main.rand.Next(30, 60) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.position.X + SpeedX, Projectile.position.Y + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<SniperBulletShard>(), Projectile.damage / 2, 0f, Projectile.owner);
                }
            }
        }
    }
}