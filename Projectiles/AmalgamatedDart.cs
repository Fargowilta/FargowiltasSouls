using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class AmalgamatedDart : ModProjectile
    {
        private int cursedCounter = 0;
        private int[] dusts = new int[] { 130, 55, 133, 131, 132 };
        private int currentDust = 0;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fargo Dart");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;


            projectile.penetrate = -1; //same as luminite
            projectile.timeLeft = 600;
            //projectile.light = 1f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;


            //projectile.usesLocalNPCImmunity = true;
            //projectile.localNPCHitCooldown = 2;
        }


        public override void AI()
        {
            //dust
            Dust.NewDust(projectile.position, projectile.width, projectile.height, dusts[currentDust], projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
            currentDust++;
            if (currentDust > 4)
            {
                currentDust = 0;
            }

            //all
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[0] >= 20f)
            {
                projectile.ai[0] = 20f;
                if (projectile.type != 477)
                {
                    projectile.velocity.Y = projectile.velocity.Y + 0.075f;
                }
            }
            //crystal
            if (projectile.localAI[1] < 5f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                projectile.localAI[1] += 1f;
            }
            else
            {
                projectile.rotation = (projectile.rotation * 2f + (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f) / 3f;
            }

            //cursed
            cursedCounter++;
            if (cursedCounter > 10)
            {
                cursedCounter = 0;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileID.CursedDartFlame, (int)((double)projectile.damage * 0.8), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
            }


            //ichor
            if (projectile.ai[1] >= 0f)
            {
                projectile.penetrate = -1;
            }
            else if (projectile.penetrate < 0)
            {
                projectile.penetrate = 1;
            }
            if (projectile.ai[1] >= 0f)
            {
                projectile.ai[1] += 1f;
            }
            if (projectile.ai[1] > (float)Main.rand.Next(5, 30))
            {
                projectile.ai[1] = -1000f;
                float scaleFactor4 = projectile.velocity.Length();
                Vector2 velocity = projectile.velocity;
                velocity.Normalize();
                int num194 = Main.rand.Next(5, 7);
                if (Main.rand.NextBool(4))
                {
                    num194++;
                }
                for (int num195 = 0; num195 < num194; num195++)
                {
                    Vector2 vector21 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    vector21.Normalize();
                    vector21 += velocity * 2f;
                    vector21.Normalize();
                    vector21 *= scaleFactor4;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector21.X, vector21.Y, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0f, -1000f);
                }
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHit();

            //crystal wtf
            /*projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            projectile.velocity.X = -projectile.velocity.X;
            projectile.velocity.Y = -projectile.velocity.Y;
           /* if (projectile.velocity.X != velocity.X)
            {
                
            }
            if (projectile.velocity.Y != velocity.Y)
            {
                
            }
            /*if (projectile.penetrate > 0 && projectile.owner == Main.myPlayer)
            {
                int[] array = new int[10];
                int num17 = 0;
                int num18 = 700;
                int num19 = 20;
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(projectile, false))
                    {
                        float num20 = (projectile.Center - Main.npc[i].Center).Length();
                        if (num20 > (float)num19 && num20 < (float)num18 && Collision.CanHitLine(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                        {
                            array[num17] = i;
                            num17++;
                            if (num17 >= 9)
                            {
                                break;
                            }
                        }
                    }
                }
                if (num17 > 0)
                {
                    num17 = Main.rand.Next(num17);
                    Vector2 value7 = Main.npc[array[num17]].Center - projectile.Center;
                    float scaleFactor = projectile.velocity.Length();
                    value7.Normalize();
                    projectile.velocity = value7 * scaleFactor;
                    projectile.netUpdate = true;
                }
            }*/

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHit();

            /*//crystal
            int[] array = new int[10];
            int num16 = 0;
            int num17 = 700;
            int num18 = 20;
            for (int k = 0; k < 200; k++)
            {
                if (k != i && Main.npc[k].CanBeChasedBy(projectile, false))
                {
                    float num19 = (projectile.Center - Main.npc[k].Center).Length();
                    if (num19 > (float)num18 && num19 < (float)num17 && Collision.CanHitLine(projectile.Center, 1, 1, Main.npc[k].Center, 1, 1))
                    {
                        array[num16] = k;
                        num16++;
                        if (num16 >= 9)
                        {
                            break;
                        }
                    }
                }
            }
            if (num16 > 0)
            {
                num16 = Main.rand.Next(num16);
                Vector2 value4 = Main.npc[array[num16]].Center - projectile.Center;
                float scaleFactor3 = projectile.velocity.Length();
                value4.Normalize();
                projectile.velocity = value4 * scaleFactor3;
                projectile.netUpdate = true;
            }*/






            //poison
            target.AddBuff(BuffID.Poisoned, 600);
            //cursed
            //target.AddBuff(BuffID.CursedFlames, 600);
            //ichor
            target.AddBuff(BuffID.Ichor, 600);
        }

        public void OnHit()
        {
            
        }

        public override void Kill(int timeleft)
        {
            //cursed
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileID.CursedDartFlame, (int)((double)projectile.damage * 0.8), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
            }

            OnHit();
        }
    }
}