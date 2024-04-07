using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class ElfCopterBulletExplosion : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosive Bullet");
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int index = 0; index < 7; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
            for (int index1 = 0; index1 < 3; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity *= 3f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity *= 2f;
            }
            int index4 = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X - 10f, Projectile.position.Y - 10f), default, Main.rand.Next(61, 64), 1f);
            Gore gore = Main.gore[index4];
            gore.velocity *= 0.3f;
            Main.gore[index4].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
            Main.gore[index4].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
        }
    }
}