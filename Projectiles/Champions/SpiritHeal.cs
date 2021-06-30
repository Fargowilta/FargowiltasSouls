using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class SpiritHeal : SpiritSpirit
    {
        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (--projectile.ai[1] < 0 && projectile.ai[1] > -300)
            {
                if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs && Main.npc[(int)projectile.ai[0]].active
                    && Main.npc[(int)projectile.ai[0]].type == ModContent.NPCType<NPCs.Champions.SpiritChampion>())
                {
                    NPC n = Main.npc[(int)projectile.ai[0]];
                    if (projectile.Distance(n.Center) > 50) //stop homing when in certain range
                    {
                        for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                        {
                            Vector2 change = projectile.DirectionTo(n.Center) * 3f;
                            projectile.velocity = (projectile.velocity * 29f + change) / 30f;
                        }
                    }
                    else //die and feed it
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            n.life += projectile.damage;
                            n.HealEffect(projectile.damage);
                            if (n.life > n.lifeMax)
                                n.life = n.lifeMax;
                            projectile.Kill();
                        }
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }

            for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
            {
                projectile.position += projectile.velocity;
                
                /*for (int j = 0; j < 5; ++j)
                {
                    Vector2 vel = projectile.velocity * 0.2f * j;
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 175, 0f, 0f, 100, default, 1.3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = Vector2.Zero;
                    Main.dust[d].position -= vel;
                }*/
            }
        }
    }
}