using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class HiveSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 42;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.sentry = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 7200;
            Projectile.sentry = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            Projectile.ai[1] += 1f;

            if (Projectile.ai[1] >= 120)
            {
                float num = 2000f;
                int npcIndex = -1;

                for (int i = 0; i < 200; i++)
                {
                    float dist = Vector2.Distance(Projectile.Center, Main.npc[i].Center);

                    if (dist < num && dist < 300 && Main.npc[i].CanBeChasedBy(Projectile, false))
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
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)), owner.beeType(), owner.beeDamage(Projectile.damage), owner.beeKB(0f), Projectile.owner);
                        Main.projectile[p].DamageType = DamageClass.Summon;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Honey, -Projectile.velocity.X * 0.2f,
                        -Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 2f;
                        dust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width, Projectile.height, DustID.BlueTorch, -Projectile.velocity.X * 0.2f,
                        -Projectile.velocity.Y * 0.2f, 100);
                        Main.dust[dust].velocity *= 2f;
                    }
                }
                Projectile.ai[1] = 0f;
                float distance = Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center);

                //kill if too far away
                if (distance > 2000)
                {
                    Projectile.Kill();
                }
                else if (distance < 20)
                {
                    Main.player[Projectile.owner].AddBuff(BuffID.Honey, 300);
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}