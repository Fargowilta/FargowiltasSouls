using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class OpticSpazmatism : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spazmatism");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1.5f;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.netImportant = true;
            projectile.scale = .5f;

            /*projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;*/
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().TwinsEX)
                projectile.timeLeft = 2;

            bool collide = true;

            if (projectile.ai[0] >= 0 && projectile.ai[0] < 200) //has target
            {
                NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
                    projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                NPC npc = Main.npc[(int)projectile.ai[0]];
                if (npc.CanBeChasedBy(projectile))
                {
                    if (projectile.ai[1] <= 0)
                    {
                        projectile.position += npc.velocity / 2f;

                        Vector2 targetPos = npc.Center + projectile.DirectionFrom(npc.Center) * 300;
                        if (projectile.Distance(targetPos) > 50)
                        {
                            Movement(targetPos, 0.5f);
                        }
                        else if (--projectile.ai[1] < -30) //in target range for 1 second, initiate dash
                        {
                            projectile.velocity = projectile.DirectionTo(npc.Center + npc.velocity * 10) * 30f;
                            projectile.ai[1] = 20;
                            projectile.netUpdate = true;
                            collide = false;
                        }
                        projectile.rotation = projectile.DirectionTo(npc.Center).ToRotation() - (float)Math.PI / 2;

                        if (++projectile.localAI[0] > 7)
                        {
                            projectile.localAI[0] = 0;
                            Main.PlaySound(SoundID.Item34, projectile.Center);
                            if (projectile.owner == Main.myPlayer)
                            {
                                Projectile.NewProjectile(projectile.Center - (projectile.rotation + (float)Math.PI / 2).ToRotationVector2() * 60,
                                    8 * projectile.DirectionTo(npc.Center).RotatedByRandom(MathHelper.ToRadians(12)), ModContent.ProjectileType<OpticFlame>(),
                                    projectile.damage, projectile.knockBack, projectile.owner);
                            }
                        }
                    }
                    else //is dashing
                    {
                        projectile.ai[1]--;
                        projectile.rotation = projectile.velocity.ToRotation() - (float)Math.PI / 2;
                        collide = false;
                    }
                }
                else //forget target
                {
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500);
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Vector2 targetPos = player.Center;
                targetPos.Y -= 100;

                if (projectile.Distance(targetPos) > 3000)
                    projectile.Center = player.Center;
                else if (projectile.Distance(targetPos) > 200)
                    Movement(targetPos, 0.5f);

                projectile.rotation = projectile.velocity.ToRotation() - (float)Math.PI / 2;
                projectile.ai[1] = 0;

                if (++projectile.localAI[1] > 6)
                {
                    projectile.localAI[1] = 0;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1500);
                    if (projectile.ai[0] != -1)
                        projectile.netUpdate = true;
                }
            }

            if (++projectile.frameCounter > 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 6)
                    projectile.frame = 3;
            }
            if (projectile.frame < 3)
                projectile.frame = 3;

            if (collide)
            {
                const float IdleAccel = 0.05f;
                int otherMinion = ModContent.ProjectileType<OpticSpazmatism>();
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.owner == projectile.owner && (p.type == projectile.type || p.type == otherMinion) && p.whoAmI != projectile.whoAmI && p.Distance(projectile.Center) < projectile.width))
                {
                    projectile.velocity.X += IdleAccel * (projectile.Center.X < p.Center.X ? -1 : 1);
                    projectile.velocity.Y += IdleAccel * (projectile.Center.Y < p.Center.Y ? -1 : 1);
                    p.velocity.X += IdleAccel * (p.Center.X < projectile.Center.X ? -1 : 1);
                    p.velocity.Y += IdleAccel * (p.Center.Y < projectile.Center.Y ? -1 : 1);
                }
            }
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
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.Ichor, 600);
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

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}