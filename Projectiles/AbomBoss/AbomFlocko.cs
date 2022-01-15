using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFlocko : ModProjectile
    {
        public override string Texture => "Terraria/NPC_352";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Super Flocko");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.timeLeft = 420;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

            Vector2 target = npc.Center;
            target.X += (npc.localAI[3] > 1 ? 1200 : 2000) * (float)Math.Sin(2 * Math.PI / 720 * projectile.ai[1]++);
            target.Y -= 1100;

            Vector2 distance = target - projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (projectile.velocity.Length() < 12f)
                    projectile.velocity *= 1.05f;
            }

            /*projectile.localAI[0]++;
            if (projectile.localAI[0] > 45)
            {
                projectile.localAI[0] = 0f;
                if (projectile.owner == Main.myPlayer)
                {
                    Vector2 vel = distance;
                    vel.Normalize();
                    vel *= 9f;
                    Projectile.NewProjectile(projectile.Center, vel, ModContent.ProjectileType<FrostWave>(),
                        projectile.damage, projectile.knockBack, projectile.owner);
                }
            }*/

            if (++projectile.localAI[0] > 90 && ++projectile.localAI[1] > (npc.localAI[3] > 1 ? 4 : 2)) //spray shards
            {
                Main.PlaySound(SoundID.Item27, projectile.position);
                projectile.localAI[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Math.Abs(npc.Center.X - projectile.Center.X) > 400)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                            speed.Normalize();
                            speed *= 8f;
                            Projectile.NewProjectile(projectile.Center + speed * 4f, speed, ModContent.ProjectileType<AbomFrostShard>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                    if (Main.player[npc.target].active && !Main.player[npc.target].dead && Main.player[npc.target].Center.Y < projectile.Center.Y)
                    {
                        Main.PlaySound(SoundID.Item120, projectile.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 vel = projectile.DirectionTo(Main.player[npc.target].Center + new Vector2(Main.rand.Next(-200, 201), Main.rand.Next(-200, 201))) * 12f;
                            Projectile.NewProjectile(projectile.Center, vel, ModContent.ProjectileType<AbomFrostWave>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                }
            }

            projectile.rotation += projectile.velocity.Length() / 12f * (projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++projectile.frameCounter > 3)
            {
                if (++projectile.frame >= 6)
                    projectile.frame = 0;
                projectile.frameCounter = 0;
            }
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 80 : 76, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale++;
                Main.dust[index2].velocity *= 4f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
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
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}