using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.AbomBoss
{
    public class AbomSickleSplit1 : AbomSickle
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/AbomBoss/AbomSickle";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }
            Projectile.rotation += 0.8f;
            /*for (int i = 0; i < 6; i++)
            {
                Vector2 offset = new Vector2(0, -20).RotatedBy(Projectile.rotation);
                offset = offset.RotatedByRandom(MathHelper.Pi / 6);
                int d = Dust.NewDust(Projectile.Center, 0, 0, 87, 0f, 0f, 150);
                Main.dust[d].position += offset;
                float velrando = Main.rand.Next(20, 31) / 10;
                Main.dust[d].velocity = Projectile.velocity / velrando;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void OnKill(int timeLeft)
        {
            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 vel = Vector2.Normalize(Projectile.velocity).RotatedBy(Math.PI / 4 * i);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<AbomSickleSplit2>(), Projectile.damage, 0f, Projectile.owner);
                }
            }
        }
    }
}