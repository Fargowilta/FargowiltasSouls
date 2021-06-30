using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DicerPlantera : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/DicerProj2";

        private const float range = 200;

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 1200;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, .4f, 1.2f, .4f); //glow in the dark

            if (projectile.localAI[0] == 0) //random rotation direction
            {
                projectile.localAI[0] = Main.rand.Next(2) == 0 ? 1 : -1;
            }

            if (projectile.localAI[1] >= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(2) == 0 ? 107 : 157);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].scale = 1.5f;
                }

                if (++projectile.localAI[1] > 25)
                {
                    projectile.localAI[1] = -1;

                    if (projectile.ai[1] > 0) //propagate
                    {
                        Main.PlaySound(SoundID.Grass, projectile.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(projectile.Center, projectile.velocity,
                                projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1] - 1);
                            if (projectile.ai[0] == 1)
                            {
                                Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(MathHelper.ToRadians(120)),
                                  projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 0, projectile.ai[1] - 1);
                            }
                        }
                    }

                    for (int index1 = 0; index1 < 30; ++index1)
                    {
                        int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(2) == 0 ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= 5f;
                    }

                    projectile.localAI[0] = 50 * (projectile.ai[1] % 3);

                    projectile.velocity = Vector2.Zero;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = true;

                projectile.localAI[0]--;
                if (projectile.localAI[0] >= -30) //delay
                {
                    projectile.scale = 1f;
                }
                if (projectile.localAI[0] < -30 && projectile.localAI[0] > -120)
                {
                    projectile.scale += 0.06f;
                    projectile.rotation += 0.3f * projectile.localAI[0];
                }
                else if (projectile.localAI[0] == -120)
                {
                    const int max = 30; //make some indicator dusts
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 vector6 = Vector2.UnitY * 5f;
                        vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + projectile.Center;
                        Vector2 vector7 = vector6 - projectile.Center;
                        int d = Dust.NewDust(vector6 + vector7, 0, 0, 107, 0f, 0f, 0, default(Color), 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = vector7;
                    }
                }
                else if (projectile.localAI[0] < -150) //explode
                {
                    projectile.localAI[0] = 0;
                    projectile.netUpdate = true;

                    Main.PlaySound(SoundID.Item, projectile.Center, 14); //spray
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool planteraAlive = NPC.plantBoss > -1 && NPC.plantBoss < Main.maxNPCs && Main.npc[NPC.plantBoss].active && Main.npc[NPC.plantBoss].type == NPCID.Plantera;
                        //die after this many explosions, plantera is dead, or if i have no decent line of sight to plantera
                        if (!planteraAlive || !Collision.CanHitLine(projectile.Center, 0, 0, Main.npc[NPC.plantBoss].Center + (projectile.Center - Main.npc[NPC.plantBoss].Center) / 2, 0, 0))
                        {
                            projectile.Kill();
                            return;
                        }
                        else //do the actual attack
                        {
                            const int time = 12;
                            const int max = 12;
                            float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int i = 0; i < max; i++)
                            {
                                int p = Projectile.NewProjectile(projectile.Center, range / time * Vector2.UnitX.RotatedBy(Math.PI * 2 / max * i + rotation),
                                    ModContent.ProjectileType<PoisonSeed2>(), projectile.damage, projectile.knockBack, projectile.owner);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = time;
                            }
                        }

                        if (projectile.localAI[1]-- < -3)
                            projectile.Kill();
                    }
                }
            }
        }

        /*public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Infested"), 180);
            target.AddBuff(mod.BuffType("IvyVenom"), 240);
        }*/

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath1, projectile.Center);
        }
    }
}