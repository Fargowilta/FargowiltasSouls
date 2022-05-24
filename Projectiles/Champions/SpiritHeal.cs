using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class SpiritHeal : SpiritSpirit
    {
        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (--Projectile.ai[1] < 0 && Projectile.ai[1] > -300)
            {
                NPC n = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.SpiritChampion>());
                if (n != null)
                {
                    if (Projectile.Distance(n.Center) > 50) //stop homing when in certain range
                    {
                        for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                        {
                            Vector2 change = Projectile.DirectionTo(n.Center) * 3f;
                            Projectile.velocity = (Projectile.velocity * 29f + change) / 30f;
                        }
                    }
                    else //die and feed it
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            n.life += Projectile.damage;
                            n.HealEffect(Projectile.damage);
                            if (n.life > n.lifeMax)
                                n.life = n.lifeMax;
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }

            for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
            {
                Projectile.position += Projectile.velocity;

                /*for (int j = 0; j < 5; ++j)
                {
                    Vector2 vel = Projectile.velocity * 0.2f * j;
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 175, 0f, 0f, 100, default, 1.3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = Vector2.Zero;
                    Main.dust[d].position -= vel;
                }*/
            }
        }
    }
}