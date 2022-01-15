using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HiveSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hive");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.sentry = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 7200;
            projectile.sentry = true;
        }

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];

            projectile.velocity.Y = projectile.velocity.Y + 0.2f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            projectile.ai[1] += 1f;

            if (projectile.ai[1] >= 120)
            {
                float num = 2000f;
                int npcIndex = -1;

                for (int i = 0; i < 200; i++)
                {
                    float dist = Vector2.Distance(projectile.Center, Main.npc[i].Center);

                    if (dist < num && dist < 300 && Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        npcIndex = i;
                        num = dist;
                    }
                }

                if (npcIndex != -1)
                {
                    NPC target = Main.npc[npcIndex];

                    for (int i = 0; i < 10; i++)
                    {
                        int p = Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), owner.beeType(), owner.beeDamage(projectile.damage), owner.beeKB(0f), projectile.owner);
                        Main.projectile[p].minion = true;
                        Main.projectile[p].ranged = false;
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Honey, -projectile.velocity.X * 0.2f,
                            -projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 2f;
                        dust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 59, -projectile.velocity.X * 0.2f,
                            -projectile.velocity.Y * 0.2f, 100);
                        Main.dust[dust].velocity *= 2f;
                    }
                }
                projectile.ai[1] = 0f;

                float distance = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);

                //kill if too far away
                if (distance > 2000)
                {
                    projectile.Kill();
                }
                else if (distance < 20)
                {
                    Main.player[projectile.owner].AddBuff(BuffID.Honey, 300);
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.position += projectile.velocity;
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}
