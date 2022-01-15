using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingDevi : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Eternals/DevianttSoul";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Deviantt");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 50;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.netImportant = true;
            projectile.timeLeft = 115;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            projectile.scale = 1;

            Player player = Main.player[projectile.owner];
            
            int target = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 2000);
            
            if (++projectile.ai[0] == 50) //spawn axe
            {
                projectile.netUpdate = true;
                if (projectile.owner == Main.myPlayer)
                {
                    Vector2 offset = new Vector2(0, -275).RotatedBy(Math.PI / 4 * projectile.spriteDirection);
                    Projectile.NewProjectile(projectile.Center + offset, Vector2.Zero, ModContent.ProjectileType<SparklingLoveBig>(), 
                        projectile.damage, projectile.knockBack, projectile.owner, 0f, projectile.identity);
                }
            }
            else if (projectile.ai[0] < 100)
            {
                Vector2 targetPos;

                if (target != -1 && Main.npc[target].CanBeChasedBy(projectile))
                {
                    targetPos = Main.npc[target].Center;
                    projectile.direction = projectile.spriteDirection = projectile.Center.X > targetPos.X ? 1 : -1;
                    targetPos.X += 500 * projectile.direction;
                    targetPos.Y -= 200;
                }
                else
                {
                    projectile.direction = projectile.spriteDirection = -Main.player[projectile.owner].direction;
                    targetPos = Main.player[projectile.owner].Center + new Vector2(100 * projectile.direction, -100);
                }
                
                if (projectile.Distance(targetPos) > 50)
                    Movement(targetPos, 1f);
            }
            else if (projectile.ai[0] == 99 || projectile.ai[0] == 100)
            {
                projectile.netUpdate = true;

                if (projectile.owner == Main.myPlayer)
                {
                    Vector2 targetPos;

                    if (target != -1 && Main.npc[target].CanBeChasedBy(projectile))
                    {
                        targetPos = Main.npc[target].Center + Main.npc[target].velocity * 10;
                    }
                    else
                    {
                        targetPos = Main.MouseWorld;
                    }

                    projectile.direction = projectile.spriteDirection = projectile.Center.X > targetPos.X ? 1 : -1;

                    targetPos.X += 360 * projectile.direction;

                    if (projectile.ai[0] == 100)
                    {
                        projectile.velocity = (targetPos - projectile.Center) / projectile.timeLeft;

                        projectile.position += projectile.velocity; //makes sure the offset is right
                    }
                }
            }

            if (++projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 4)
                    projectile.frame = 0;
            }

            int num812 = Dust.NewDust(projectile.position, projectile.width, projectile.height,
                86, projectile.velocity.X / 2, projectile.velocity.Y / 2, 0, default(Color), 1.5f);
            Main.dust[num812].noGravity = true;
        }

        private void Movement(Vector2 targetPos, float speedModifier)
        {
            if (projectile.Center.X < targetPos.X)
            {
                projectile.velocity.X += speedModifier;
                if (projectile.velocity.X < 0)
                    projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                projectile.velocity.X -= speedModifier;
                if (projectile.velocity.X > 0)
                    projectile.velocity.X -= speedModifier * 2;
            }
            if (projectile.Center.Y < targetPos.Y)
            {
                projectile.velocity.Y += speedModifier;
                if (projectile.velocity.Y < 0)
                    projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                projectile.velocity.Y -= speedModifier;
                if (projectile.velocity.Y > 0)
                    projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(projectile.velocity.X) > 24)
                projectile.velocity.X = 24 * Math.Sign(projectile.velocity.X);
            if (Math.Abs(projectile.velocity.Y) > 24)
                projectile.velocity.Y = 24 * Math.Sign(projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Lovestruck, 300);
            target.immune[projectile.owner] = 1;
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 150) * projectile.Opacity * 0.75f;
        }
    }
}