using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class OpticFlame : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_101";

        public int targetID = -1;
        public int searchTimer = 3;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eye Fire");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetID = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeFire);
            aiType = ProjectileID.EyeFire;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.magic = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;

            /*projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;*/
        }

        /*public override void AI()
        {
            if (targetID == -1) //no target atm
            {
                if (searchTimer == 0) //search every few ticks
                {
                    searchTimer = 6;

                    int possibleTarget = -1;
                    float closestDistance = 500f;

                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.active && npc.chaseable && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly && !npc.immortal)
                        {
                            float distance = Vector2.Distance(projectile.Center, npc.Center);

                            if (closestDistance > distance)
                            {
                                closestDistance = distance;
                                possibleTarget = i;
                            }
                        }
                    }

                    if (possibleTarget != -1)
                    {
                        targetID = possibleTarget;
                        projectile.netUpdate = true;
                    }
                }
                searchTimer--;
            }
            else //currently have target
            {
                NPC npc = Main.npc[targetID];

                if (npc.active && npc.chaseable && !npc.dontTakeDamage) //target is still valid
                {
                    Vector2 distance = npc.Center - projectile.Center;
                    double angle = distance.ToRotation() - projectile.velocity.ToRotation();
                    if (angle > Math.PI)
                        angle -= 2.0 * Math.PI;
                    if (angle < -Math.PI)
                        angle += 2.0 * Math.PI;

                    if (projectile.ai[0] == -1)
                    {
                        if (Math.Abs(angle) > Math.PI * 0.75)
                        {
                            projectile.velocity = projectile.velocity.RotatedBy(angle * 0.07);
                        }
                        else
                        {
                            float range = distance.Length();
                            float difference = 12.7f / range;
                            distance *= difference;
                            distance /= 7f;
                            projectile.velocity += distance;
                            if (range > 70f)
                            {
                                projectile.velocity *= 0.977f;
                            }
                        }
                    }
                    else
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(angle * 0.1);
                    }
                }
                else //target lost, reset
                {
                    targetID = -1;
                    searchTimer = 0;
                    projectile.netUpdate = true;
                }
            }
        }*/

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.CursedInferno, 600);
        }
    }
}