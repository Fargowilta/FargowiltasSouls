using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class LunarCultistLight : ModProjectile
    {
        public override string Texture => "Terraria/NPC_522";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ancient Light");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.alpha = 0;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 240;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() - 1.570796f;
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1f;
                projectile.ai[0] = -1f;
                for (int index1 = 0; index1 < 13; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 261, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 90, new Color(), 2.5f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].fadeIn = 1f;
                    Dust dust = Main.dust[index2];
                    dust.velocity = dust.velocity * 4f;
                    Main.dust[index2].noLight = true;
                }
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                if (Main.rand.Next(10 - (int)Math.Min(7f, projectile.velocity.Length())) < 1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 261, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 90, new Color(), 2.5f);
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity = dust1.velocity * 0.2f;
                    Main.dust[index2].fadeIn = 0.4f;
                    if (Main.rand.NextBool(6))
                    {
                        Dust dust2 = Main.dust[index2];
                        dust2.velocity = dust2.velocity * 5f;
                        Main.dust[index2].noLight = true;
                    }
                    else
                        Main.dust[index2].velocity = projectile.DirectionFrom(Main.dust[index2].position) * Main.dust[index2].velocity.Length();
                }
            }
            if (projectile.timeLeft < 180)
                projectile.velocity = projectile.velocity.RotatedBy(projectile.ai[1], new Vector2());
            if (projectile.timeLeft < 120)
                projectile.velocity = projectile.velocity * 0.98f;
            if (projectile.velocity.Length() < 0.2f)
            {
                projectile.velocity = Vector2.Zero;
            }
            if (projectile.ai[0] > -1 && projectile.ai[0] < 200) //has target
            {
                Vector2 speed = Main.npc[(int)projectile.ai[0]].Center - projectile.Center;
                speed.Normalize();
                speed *= 9f;
                projectile.velocity.X += speed.X / 100f;
                if (projectile.velocity.X > 9f)
                    projectile.velocity.X = 9f;
                else if (projectile.velocity.X < -9f)
                    projectile.velocity.X = -9f;
                projectile.velocity.Y += speed.Y / 100f;
                if (projectile.velocity.Y > 9f)
                    projectile.velocity.Y = 9f;
                else if (projectile.velocity.Y < -9f)
                    projectile.velocity.Y = -9f;
            }
            else
            {
                if (projectile.localAI[1]++ > 15)
                {
                    projectile.localAI[1] = 0;
                    float maxDistance = 2000f;
                    int possibleTarget = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile))// && Collision.CanHitLine(projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            float npcDistance = projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                possibleTarget = i;
                            }
                        }
                    }
                    if (possibleTarget > -1)
                    {
                        projectile.ai[0] = possibleTarget;
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int index1 = 0; index1 < 13; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 262, 0f, 0f, 90, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].fadeIn = 1f;
                Dust dust = Main.dust[index2];
                dust.velocity = dust.velocity * 4f;
                Main.dust[index2].noLight = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}