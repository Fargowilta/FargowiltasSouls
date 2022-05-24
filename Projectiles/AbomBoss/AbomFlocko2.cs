using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFlocko2 : AbomFlocko
    {
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0 || Projectile.ai[0] >= Main.maxPlayers)
            {
                Projectile.Kill();
                return;
            }

            Player player = Main.player[(int)Projectile.ai[0]];

            Vector2 target = player.Center;
            target.X += 700 * Projectile.ai[1];

            Vector2 distance = target - Projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            if (++Projectile.localAI[0] > 90 && ++Projectile.localAI[1] > 60) //fire frost wave
            {
                Projectile.localAI[1] = 0f;
                SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = Projectile.DirectionTo(player.Center) * 7f;
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.ToRadians(10) * i), ModContent.ProjectileType<AbomFrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            Projectile.rotation += Projectile.velocity.Length() / 12f * (Projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }
    }
}