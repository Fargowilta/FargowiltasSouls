using System;
using Microsoft.Xna.Framework; 
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PhantasmalFlail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Phantasmal Flail");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 52;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.melee = true;
            projectile.tileCollide = false;
            //projectile.usesLocalNPCImmunity = true;
            //projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (projectile.timeLeft == 120) projectile.ai[0] = 1f;

            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }

            Main.player[projectile.owner].itemAnimation = 5;
            Main.player[projectile.owner].itemTime = 5;

            if (projectile.alpha == 0)
            {
                if (projectile.position.X + projectile.width / 2 > Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2)
                    Main.player[projectile.owner].ChangeDir(1);
                else
                    Main.player[projectile.owner].ChangeDir(-1);
            }

            Vector2 vector14 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
            float num166 = Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2 - vector14.X;
            float num167 = Main.player[projectile.owner].position.Y + Main.player[projectile.owner].height / 2 - vector14.Y;
            float distance = (float)Math.Sqrt(num166 * num166 + num167 * num167);
            if (projectile.ai[0] == 0f)
            {
                if (distance > 600f) projectile.ai[0] = 1f;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
                projectile.ai[1] += 1f;
                if (projectile.ai[1] > 8f) projectile.ai[1] = 8f;
                if (projectile.velocity.X < 0f)
                    projectile.spriteDirection = -1;
                else
                    projectile.spriteDirection = 1;

                /*if (projectile.owner == Main.myPlayer)
                {
                    Vector2 speed = 10f * Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 2);
                    Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<PhantasmalBolt>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                    Projectile.NewProjectile(projectile.Center, -speed, ModContent.ProjectileType<PhantasmalBolt>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                }*/
            }
            //plz retract sir
            else if (projectile.ai[0] == 1f)
            {
                if (projectile.localAI[1] == 0)
                {
                    projectile.localAI[1] = 1f;
                    if (projectile.owner == Main.myPlayer)
                    {
                        const int max = 8;
                        const float dist = 100f;
                        const float rotation = 2f * (float)Math.PI / max;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 spawnPos = projectile.Center + new Vector2(dist, 0f).RotatedBy(rotation * i);
                            Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<PhantasmalSphere2>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                        }

                        const int boltMax = 32;
                        float rotationOffset = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < boltMax; i++)
                        {
                            Vector2 speed = 10f * Vector2.UnitX.RotatedBy(rotationOffset + MathHelper.TwoPi / boltMax * i);
                            int p = Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<PhantasmalBolt>(), projectile.damage / 2, projectile.knockBack, projectile.owner);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft /= 2;
                        }
                    }
                }

                projectile.tileCollide = false;
                projectile.rotation = (float)Math.Atan2(num167, num166) - 1.57f;
                float num169 = 30f;

                if (distance < 50f) projectile.Kill();
                distance = num169 / distance;
                num166 *= distance;
                num167 *= distance;
                projectile.velocity.X = num166 * 2;
                projectile.velocity.Y = num167 * 2;
                if (projectile.velocity.X < 0f)
                    projectile.spriteDirection = 1;
                else
                    projectile.spriteDirection = -1;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.penetrate < 0)
                target.immune[projectile.owner] = 1;

            projectile.penetrate = -1;
            projectile.maxPenetrate = -1;

            int type = ModContent.ProjectileType<PhantasmalEyeLeashProj>();
            if (Main.player[projectile.owner].ownedProjectileCounts[type] < 100)
            {
                int dist = 1200;
                for (int i = 0; i < 15; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * dist);
                    offset.Y += (float)(Math.Cos(angle) * dist);

                    Vector2 position = target.Center + offset - new Vector2(4, 4);
                    Vector2 velocity = Vector2.Normalize(target.Center - position) * 50;

                    int p = Projectile.NewProjectile(position, velocity,
                        type, projectile.damage / 2, projectile.knockBack, projectile.owner, -10f);
                }
            }

            /*const int max = 10;
            for (int i = 0; i < max; i++)
            {
                Vector2 vector55 = Vector2.UnitX.RotatedBy(Math.PI * 2 / max * (i + Main.rand.NextDouble()));
                vector55 *= 50;
                int p = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector55.X, vector55.Y,
                    ModContent.ProjectileType<PhantasmalEyeLeashProj>(), projectile.damage, projectile.knockBack, projectile.owner, -10f);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].penetrate = 3;
            }*/

            //retract
            projectile.ai[0] = 1f; 
        }

        // chain voodoo
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("FargowiltasSouls/Projectiles/BossWeapons/PhantasmalLeashFlailChain");

            Vector2 position = projectile.Center;
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Rectangle? sourceRectangle = new Rectangle?();
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            float num1 = texture.Height;
            Vector2 vector24 = mountedCenter - position;
            float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                flag = false;
            if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                flag = false;
            while (flag)
                if (vector24.Length() < num1 + 1.0)
                {
                    flag = false;
                }
                else
                {
                    Vector2 vector21 = vector24;
                    vector21.Normalize();
                    position += vector21 * num1;
                    vector24 = mountedCenter - position;
                    Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                    color2 = projectile.GetAlpha(color2);
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                }


            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.5f)
            {
                Color color27 = Color.White * projectile.Opacity;
                color27.A = 0;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 value4 = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                float num165 = MathHelper.Lerp(projectile.oldRot[(int)i], projectile.oldRot[max0], 1 - i % 1);
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}