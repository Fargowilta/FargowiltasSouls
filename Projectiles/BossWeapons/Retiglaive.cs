using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Retiglaive : ModProjectile
    {
        bool empowered = false;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Retiglaive");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(empowered);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            empowered = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.friendly = true;
            projectile.light = 0.4f;

            projectile.width = 50;
            projectile.height = 50;
            projectile.penetrate = -1;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;

            projectile.extraUpdates = 1;
        }

        public override bool CanDamage() => false;

        public override bool PreAI()
        {
            if (projectile.ai[0] == 1)
            {
                projectile.ai[1]++;

                projectile.rotation += projectile.direction * -0.4f;

                if (projectile.ai[1] <= 50)
                {
                    projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Zero, 0.1f);
                    //fire lasers at cursor
                    if (projectile.ai[1] % 10 == 0)
                    {
                        Vector2 cursor = Main.MouseWorld;
                        Vector2 velocity = Vector2.Normalize(cursor - projectile.Center);

                        if (projectile.ai[1] > 10)
                            velocity = velocity.RotatedByRandom(Math.PI / 24);

                        float num = 24f;
                        for (int index1 = 0; index1 < num; ++index1)
                        {
                            int type = 235;

                            Vector2 v = (Vector2.UnitX * 0.0f + -Vector2.UnitY.RotatedBy(index1 * (MathHelper.TwoPi / num), new Vector2()) * new Vector2(1f, 4f)).RotatedBy(velocity.ToRotation());
                            int index2 = Dust.NewDust(projectile.Center, 0, 0, type, 0.0f, 0.0f, 150, new Color(255, 153, 145), 1f);
                            Main.dust[index2].scale = 1.5f;
                            Main.dust[index2].fadeIn = 1.3f;
                            Main.dust[index2].noGravity = true;
                            Main.dust[index2].position = projectile.Center + (v * projectile.scale * 1.5f);
                            Main.dust[index2].velocity = v.SafeNormalize(Vector2.UnitY);
                        }

                        Player player = Main.player[projectile.owner];

                        Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<RetiDeathray>(), projectile.damage, 1f, projectile.owner, 0, projectile.identity);
                        projectile.velocity = -velocity * 8;

                        if (empowered)
                        {
                            float baseRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            const int max = 3;
                            for (int i = 0; i < max; i++)
                            {
                                int p = Projectile.NewProjectile(projectile.Center, 1.25f * velocity.RotatedBy(MathHelper.TwoPi / max * i + baseRotation),
                                    ModContent.ProjectileType<DarkStarHomingFriendly>(), projectile.damage, 1f, projectile.owner, -1, 0);
                                if (p != Main.maxProjectiles)
                                {
                                    Main.projectile[p].minion = false;
                                    Main.projectile[p].melee = true;
                                    Main.projectile[p].timeLeft = 90;
                                }
                            }
                        }
                    }
                }

                if (projectile.ai[1] > 60)
                {
                    projectile.ai[1] = 15;
                    projectile.ai[0] = 2;
                }

                return false;
            }

            return true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == ModContent.ProjectileType<Spazmaglaive>())
            {
                empowered = true;
                projectile.ai[0] = 0;
            }
            else if (projectile.ai[0] == ModContent.ProjectileType<Retiglaive>())
            {
                projectile.ai[0] = 0;
            }

            //travelling out
            if (projectile.ai[0] == 0)
            {
                projectile.ai[1]++;

                if (projectile.ai[1] > 30)
                {
                    //projectile.velocity /= 3;
                    projectile.ai[0] = 1;
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }
            }
            //travel back to player
            else if (projectile.ai[0] == 2)
            {
                projectile.ai[1] += 0.6f;
                //projectile.extraUpdates = (projectile.ai[1] < 40) ? 0 : 1;
                float lerpspeed = (projectile.ai[1] < 40) ? 0.07f : 0.3f;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * projectile.ai[1], lerpspeed);

                //kill when back to player
                if (projectile.Distance(Main.player[projectile.owner].Center) <= 30)
                    projectile.Kill();
            }

            //spin
            projectile.rotation += projectile.direction * -0.4f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            /*if (!hitSomething)
            {
                hitSomething = true;
                if (projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (k == target.whoAmI)
                            continue;

                        NPC npc = Main.npc[k];
                        float distance = Vector2.Distance(npc.Center, projectile.Center);

                        if ((distance < 500) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            Vector2 velocity = (npc.Center - projectile.Center) * 20;

                            int p = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.PurpleLaser, projectile.damage, 0, projectile.owner);
                            if (p != Main.maxProjectiles)
                            {
                                Main.projectile[p].melee = true;
                                Main.projectile[p].magic = false;
                            }

                            break;
                        }
                    }
                }
            }*/
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //smaller tile hitbox
            width = 22;
            height = 22;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                if (i < 4)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
                }
                if(empowered)
                {
                    Texture2D glow = mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                    Color glowcolor = new Color(255, 50, 50);
                    glowcolor = Color.Lerp(glowcolor, Color.Transparent, 0.6f);
                    float glowscale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Main.spriteBatch.Draw(glow, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, glowcolor, num165, glow.Size()/2, glowscale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}