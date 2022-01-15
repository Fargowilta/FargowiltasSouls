using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class LunarCultistLightningOrb : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_465";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;

            if (ModLoader.GetMod("Fargowiltas") != null)
                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", projectile);
        }

        public override void AI()
        {
            projectile.alpha += projectile.timeLeft > 51 ? -10 : 10;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;

            if (projectile.timeLeft % 30 == 0)
            {
                /*const int max = 5;
                int[] nums = new int[max];
                float[] dist = new float[max];
                int j = 0;
                int farthest = 0;
                float distance = 2000f;
                for (int i = 0; i < 200; i++) //find the five closest npcs
                {
                    if (Main.npc[i].CanBeChasedBy(projectile))
                    {
                        float newDist = Vector2.Distance(Main.npc[i].Center, projectile.Center);
                        if (newDist < distance && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
                        {
                            if (j >= max) //found an npc closer than the farthest tracked npc
                            {
                                nums[farthest] = i; //replace farthest with this npc
                                dist[farthest] = newDist;
                                for (int k = 0; k < 5; k++) //update farthest to track the actual farthest npc
                                    if (dist[farthest] < dist[k])
                                        farthest = k;
                            }
                            else //haven't filled array yet, accept anyone
                            {
                                nums[j] = i; //track npc's id
                                dist[j] = newDist; //track npc's distance
                                j++;
                                if (j == 5) //array is full now
                                {
                                    for (int k = 0; k < max; k++) //keep track of npc w/ highest distance
                                        if (dist[farthest] < dist[k])
                                            farthest = k;
                                    distance = dist[farthest]; //now only accepting npcs closer than the farthest tracked npc
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < j; i++)
                {
                    Vector2 dir = Main.npc[nums[i]].Center - projectile.Center;
                    float ai1 = Main.rand.Next(100);
                    Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                    Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("LunarCultistLightningArc"), projectile.damage, projectile.knockBack / 2, projectile.owner, dir.ToRotation(), ai1);
                    //Projectile.NewProjectile(projectile.Center, vel, ProjectileID.CultistBossLightningOrbArc, projectile.damage, projectile.knockBack / 2, projectile.owner, dir.ToRotation(), ai1);
                }*/

                int cultistTarget = -1;
                Projectile cultist = FargoSoulsUtil.ProjectileExists(projectile.ai[0], ModContent.ProjectileType<LunarCultist>());
                if (cultist != null)
                {
                    NPC cultistTargetNpc = FargoSoulsUtil.NPCExists(cultist.ai[0]);
                    if (cultistTargetNpc != null)
                    {
                        cultistTarget = cultistTargetNpc.whoAmI;
                        if (cultistTargetNpc.CanBeChasedBy(projectile) && Collision.CanHitLine(projectile.Center, 0, 0, cultistTargetNpc.Center, 0, 0))
                        {
                            Vector2 dir = cultistTargetNpc.Center - projectile.Center;
                            float ai1New = Main.rand.Next(100);
                            Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                            if (projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("LunarCultistLightningArc"), projectile.damage, projectile.knockBack / 2, projectile.owner, dir.ToRotation(), ai1New);
                        }
                    }
                }

                float maxDistance = 2000f;
                int possibleTarget = -1;
                foreach (NPC n in Main.npc.Where(n => n.CanBeChasedBy() && Collision.CanHitLine(projectile.Center, 0, 0, n.Center, 0, 0)))
                {
                    float npcDistance = projectile.Distance(n.Center);
                    if (npcDistance < maxDistance && n.whoAmI != cultistTarget)
                    {
                        maxDistance = npcDistance;
                        possibleTarget = n.whoAmI;
                    }
                }

                if (possibleTarget != -1)
                {
                    Vector2 dir = Main.npc[possibleTarget].Center - projectile.Center;
                    float ai1 = Main.rand.Next(100);
                    Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("LunarCultistLightningArc"), projectile.damage, projectile.knockBack / 2, projectile.owner, dir.ToRotation(), ai1);
                }
            }

            Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                    projectile.frame = 0;
            }

            float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
            if ((double)num11 < -0.5)
                num11 = -0.5f;
            if ((double)num11 > 0.5)
                num11 = 0.5f;
            Vector2 vector21 = new Vector2((float)-projectile.width * 0.2f * projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
            int index21 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
            Main.dust[index21].position = projectile.Center + vector21;
            Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - projectile.Center) * 2f;
            Main.dust[index21].noGravity = true;
            float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
            if ((double)num1 < -0.5)
                num1 = -0.5f;
            if ((double)num1 > 0.5)
                num1 = 0.5f;
            Vector2 vector2 = new Vector2((float)-projectile.width * 0.6f * projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
            int index2 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)projectile.velocity.X / 3.0), (float)(-(double)projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
            Main.dust[index2].velocity = Vector2.Zero;
            Main.dust[index2].position = projectile.Center + vector2;
            Main.dust[index2].noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}