using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomSickle3 : AbomSickle
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomSickle";

        public override void SetDefaults()
        {
            base.SetDefaults();
            //Projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Projectile.Center.X;
                Projectile.localAI[1] = Projectile.Center.Y;
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

            if (Projectile.ai[1] == 0)
            {
                Player target = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (target != null)
                {
                    Vector2 spawnPoint = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                    if (Projectile.Distance(spawnPoint) > target.Distance(spawnPoint) - 160)// && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.ai[1] = 1;
                        Projectile.velocity.Normalize();
                        Projectile.timeLeft = 300;
                        Projectile.netUpdate = true;

                        /*foreach (Projectile p in Main.Projectile.Where(p => p.active && p.hostile && p.type == Projectile.type && p.ai[1] == 0))
                        {
                            p.ai[1] = 1;
                            p.velocity.Normalize();
                            p.timeLeft = 300;
                            p.netUpdate = true;
                        }*/
                    }
                }
            }
            else
            {
                if (++Projectile.ai[1] < 60)
                    Projectile.velocity *= 1.065f;
            }
        }
    }
}