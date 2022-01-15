using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HellSkull2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_585";

        public float targetRotation;
        /*public int targetID = -1;
        public int searchTimer = 30;*/

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hell Skull");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.ClothiersCurse];
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120; //600;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.scale = 2f;
        }

        public override void AI()
        {
            const int period = 60;

            if (projectile.localAI[0] == 0.0)
            {
                projectile.localAI[0] = 1f;
                projectile.localAI[1] = 50;
                targetRotation = projectile.velocity.ToRotation();

                Main.PlaySound(SoundID.Item8, projectile.position);
                for (int i = 0; i < 3; ++i)
                {
                    int index2 = Dust.NewDust(projectile.position, (int)(projectile.width * projectile.scale), (int)(projectile.height * projectile.scale),
                        27, projectile.velocity.X, projectile.velocity.Y, 0, default, 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity = projectile.DirectionTo(Main.dust[index2].position);
                    Main.dust[index2].velocity *= -5f;
                    Main.dust[index2].velocity += projectile.velocity / 2f;
                    Main.dust[index2].noLight = true;
                }
            }

            float speed = projectile.velocity.Length();
            float rotation = targetRotation + (float)Math.PI / 4 * (float)Math.Sin(2 * (float)Math.PI * projectile.localAI[1] / period) * projectile.ai[1];
            if (++projectile.localAI[1] > period)
                projectile.localAI[1] = 0;
            projectile.velocity = speed * rotation.ToRotationVector2();

            if (projectile.alpha > 0)
                projectile.alpha -= 50;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            
            if (++projectile.frameCounter >= 12)
                projectile.frameCounter = 0;
            projectile.frame = projectile.frameCounter / 2;
            if (projectile.frame > 3)
                projectile.frame = 6 - projectile.frame;
            
            Lighting.AddLight(projectile.Center, NPCID.Sets.MagicAuraColor[54].ToVector3());

            projectile.spriteDirection = projectile.direction = projectile.velocity.X < 0 ? -1 : 1;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.direction < 0)
                projectile.rotation += (float)Math.PI;

            /*if (targetID == -1) //no target atm
            {
                if (searchTimer <= 0)
                {
                    searchTimer = 30;

                    int possibleTarget = -1;
                    float closestDistance = 1000f;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.CanBeChasedBy())
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

                if (npc.CanBeChasedBy()) //target is still valid
                {
                    if (projectile.Distance(npc.Center) > npc.width + npc.height)
                        targetRotation = (npc.Center - projectile.Center).ToRotation();
                }
                else //target lost, reset
                {
                    targetID = -1;
                    searchTimer = 0;
                    projectile.netUpdate = true;
                }
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<Buffs.Souls.HellFire>(), 300);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.Center.X, (int)projectile.Center.Y, 52, 0.5f, 0.2f);

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, DustID.Shadowflame, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width,
                    projectile.height, DustID.Shadowflame, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = new Color(212, 148, 255) * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), 
                    color27, num165, origin2, scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}