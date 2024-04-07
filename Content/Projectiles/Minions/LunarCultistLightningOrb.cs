using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class LunarCultistLightningOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_465";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.FargoSouls().DeletionImmuneRank = 1;

            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                fargo.Call("LowRenderProj", Projectile);
        }

        public override void AI()
        {
            Projectile.alpha += Projectile.timeLeft > 51 ? -10 : 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            if (Projectile.timeLeft % 30 == 0)
            {
                /*const int max = 5;
                int[] nums = new int[max];
                float[] dist = new float[max];
                int j = 0;
                int farthest = 0;
                float distance = 2000f;
                for (int i = 0; i < 200; i++) //find the five closest npcs
                {
                    if (Main.npc[i].CanBeChasedBy())
                    {
                        float newDist = Vector2.Distance(Main.npc[i].Center, Projectile.Center);
                        if (newDist < distance && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
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
                    Vector2 dir = Main.npc[nums[i]].Center - Projectile.Center;
                    float ai1 = Main.rand.Next(100);
                    Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<LunarCultistLightningArc>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, dir.ToRotation(), ai1);
                    //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ProjectileID.CultistBossLightningOrbArc, Projectile.damage, Projectile.knockBack / 2, Projectile.owner, dir.ToRotation(), ai1);
                }*/

                int cultistTarget = -1;
                Projectile cultist = FargoSoulsUtil.ProjectileExists(Projectile.ai[0], ModContent.ProjectileType<LunarCultist>());
                if (cultist != null)
                {
                    NPC cultistTargetNpc = FargoSoulsUtil.NPCExists(cultist.ai[0]);
                    if (cultistTargetNpc != null)
                    {
                        cultistTarget = cultistTargetNpc.whoAmI;
                        if (cultistTargetNpc.CanBeChasedBy() && Collision.CanHitLine(Projectile.Center, 0, 0, cultistTargetNpc.Center, 0, 0))
                        {
                            Vector2 dir = cultistTargetNpc.Center - Projectile.Center;
                            float ai1New = Main.rand.Next(100);
                            Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<LunarCultistLightningArc>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, dir.ToRotation(), ai1New);
                        }
                    }
                }

                float maxDistance = 2000f;
                int possibleTarget = -1;
                foreach (NPC n in Main.npc.Where(n => n.CanBeChasedBy() && Collision.CanHitLine(Projectile.Center, 0, 0, n.Center, 0, 0)))
                {
                    float npcDistance = Projectile.Distance(n.Center);
                    if (npcDistance < maxDistance && n.whoAmI != cultistTarget)
                    {
                        maxDistance = npcDistance;
                        possibleTarget = n.whoAmI;
                    }
                }

                if (possibleTarget != -1)
                {
                    Vector2 dir = Main.npc[possibleTarget].Center - Projectile.Center;
                    float ai1 = Main.rand.Next(100);
                    Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 7f;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<LunarCultistLightningArc>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, dir.ToRotation(), ai1);
                }
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
            if ((double)num11 < -0.5)
                num11 = -0.5f;
            if ((double)num11 > 0.5)
                num11 = 0.5f;
            Vector2 vector21 = new Vector2(-Projectile.width * 0.2f * Projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
            int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
            Main.dust[index21].position = Projectile.Center + vector21;
            Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
            Main.dust[index21].noGravity = true;
            float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
            if ((double)num1 < -0.5)
                num1 = -0.5f;
            if ((double)num1 > 0.5)
                num1 = 0.5f;
            Vector2 vector2 = new Vector2(-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
            int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
            Main.dust[index2].velocity = Vector2.Zero;
            Main.dust[index2].position = Projectile.Center + vector2;
            Main.dust[index2].noGravity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}