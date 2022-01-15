using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Spazmaglaive : ModProjectile
    {
        bool empowered = false;
        bool hitSomething = false;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spazmaglaive");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(empowered);
            writer.Write(hitSomething);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            empowered = reader.ReadBoolean();
            hitSomething = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.friendly = true;
            projectile.light = 0.4f;
            projectile.tileCollide = false;
            projectile.width = 50;
            projectile.height = 50;
            projectile.penetrate = -1;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == ModContent.ProjectileType<Retiglaive>())
            {
                empowered = true;
                projectile.ai[0] = 0;
            }
            else if (projectile.ai[0] == ModContent.ProjectileType<Spazmaglaive>())
            {
                projectile.ai[0] = 0;
            }
            if(projectile.ai[1] == 0)
            {
                projectile.ai[1] = Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
            }
            projectile.rotation += projectile.direction * -0.4f;
            projectile.ai[0]++;

            const int maxTime = 45;
            Vector2 DistanceOffset = new Vector2(950 * (float)Math.Sin(projectile.ai[0] * Math.PI / maxTime), 0).RotatedBy(projectile.velocity.ToRotation());
            DistanceOffset = DistanceOffset.RotatedBy(projectile.ai[1] - (projectile.ai[1] * projectile.ai[0] / (maxTime / 2)));
            projectile.Center = Main.player[projectile.owner].Center + DistanceOffset;
            if (projectile.ai[0] > maxTime)
                projectile.Kill();

            if (empowered && projectile.ai[0] == maxTime / 2 && projectile.owner == Main.myPlayer) //star spray on the rebound
            {
                Vector2 baseVel = Main.rand.NextVector2CircularEdge(1, 1);
                const int max = 24;
                for (int i = 0; i < max; i++)
                {
                    Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 105, 1f, -0.3f);
                    Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / max);
                    int p = Projectile.NewProjectile(projectile.Center, newvel / 2, mod.ProjectileType("DarkStarFriendly"), projectile.damage, projectile.knockBack, projectile.owner);
                    if (p < Main.maxProjectiles)
                    {
                        Main.projectile[p].magic = false;
                        Main.projectile[p].melee = true;
                        Main.projectile[p].timeLeft = 30;
                        Main.projectile[p].netUpdate = true;
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projectile.Distance(new Vector2(targetHitbox.Center.X, targetHitbox.Center.Y)) < 150; //big circular hitbox because otherwise it misses too often
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);

            if (!hitSomething)
            {
                hitSomething = true;
                if (projectile.owner == Main.myPlayer)
                {
                    Main.PlaySound(SoundID.Item74, projectile.Center);
                    Vector2 baseVel = Main.rand.NextVector2CircularEdge(1, 1);
                    float ai0 = 78;//empowered ? 120 : 78;
                    for(int i = 0; i < 5; i++)
                    {
                        Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / 5);
                        Projectile.NewProjectile(target.Center, newvel, mod.ProjectileType("SpazmaglaiveExplosion"), projectile.damage, projectile.knockBack, projectile.owner, ai0, target.whoAmI);
                    }
                    /*if (empowered)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 105, 1f, -0.3f);
                            Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / 12);
                            int p = Projectile.NewProjectile(target.Center, newvel/2, mod.ProjectileType("DarkStarFriendly"), projectile.damage, projectile.knockBack, projectile.owner, 0, target.whoAmI);
                            if(p < 1000)
                            {
                                Main.projectile[p].magic = false;
                                Main.projectile[p].melee = true;
                                Main.projectile[p].timeLeft = 30;
                                Main.projectile[p].netUpdate = true;
                            }
                        }
                    }*/
                }
                projectile.netUpdate = true;
            }
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

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.33f)
            {
                int max0 = Math.Max((int)i - 1, 0);
                Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], (1 - i % 1));
                if (i < 4)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Main.spriteBatch.Draw(texture2D13, center + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, projectile.oldRot[(int)i], origin2, projectile.scale, SpriteEffects.None, 0f);
                }
                if (empowered)
                {
                    Texture2D glow = mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                    Color glowcolor = new Color(142, 250, 176);
                    glowcolor = Color.Lerp(glowcolor, Color.Transparent, 0.6f);
                    float glowscale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Main.spriteBatch.Draw(glow, center + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, glowcolor, 0, glow.Size() / 2, glowscale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}