using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GoldenShowerHoming : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_288";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Golden Shower");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 900;
            projectile.hostile = true;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                Main.PlaySound(SoundID.Item17, projectile.Center);
            }

            if (projectile.ai[1] == 0)
            {
                Player player = FargoSoulsUtil.PlayerExists(projectile.ai[0]);
                if (player == null)
                {
                    projectile.ai[1] = 1;
                    projectile.netUpdate = true;
                    projectile.timeLeft = 180;
                    projectile.velocity.Normalize();
                }
                else
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = player.Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, projectile.localAI[0]));

                    if (projectile.localAI[0] < 0.5f)
                        projectile.localAI[0] += 1f / 3000f;

                    if (vel.Length() < 250 || !player.active || player.dead || player.ghost)
                    {
                        projectile.ai[1] = 1;
                        projectile.netUpdate = true;
                        projectile.timeLeft = 180;
                        projectile.velocity.Normalize();
                    }
                }
            }
            else if (projectile.ai[1] > 0)
            {
                if (++projectile.ai[1] < 120)
                {
                    projectile.velocity *= 1.03f;

                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[(int)projectile.ai[0]].Center - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.025f));
                }
            }
            else //ai1 below 0 rn
            {
                projectile.ai[1]++;
            }

            for (int i = 0; i < 3; i++) //vanilla dusts
            {
                for (int j = 0; j < 3; ++j)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 170, 0.0f, 0.0f, 100, default, 1f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].velocity += projectile.velocity * 0.5f;
                    Main.dust[d].position -= projectile.velocity / 3 * j;
                }
                if (Main.rand.NextBool(8))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 170, 0.0f, 0.0f, 100, default, 0.5f);
                    Main.dust[d].velocity *= 0.25f;
                    Main.dust[d].velocity += projectile.velocity * 0.5f;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Ichor, 900);
        }
    }
}