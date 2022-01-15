using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class SuperFlocko : ModProjectile
    {
        public override string Texture => "Terraria/NPC_352";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Super Flocko");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.Homing[projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 54;
            projectile.height = 54;
            projectile.timeLeft *= 5;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().SuperFlocko)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
                projectile.damage = (int)(40f * player.minionDamage);

            NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
            {
                projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                projectile.netUpdate = true;
            }

            if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];

                if (npc.CanBeChasedBy(projectile))
                {
                    Vector2 distance = npc.Center - projectile.Center;
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

                    projectile.localAI[0]++;
                    if (projectile.localAI[0] > 45)
                    {
                        projectile.localAI[0] = 0f;
                        if (projectile.owner == Main.myPlayer)
                        {
                            Vector2 vel = distance;
                            vel.Normalize();
                            vel *= 9f;
                            Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("FrostWave"),
                                projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }

                    projectile.localAI[1]++;
                    if (projectile.localAI[1] > 6)
                    {
                        projectile.localAI[1] = 0f;
                        if (projectile.owner == Main.myPlayer)
                        {
                            Vector2 speed = new Vector2(Main.rand.Next(-1000, 1001), Main.rand.Next(-1000, 1001));
                            speed.Normalize();
                            speed *= 12f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(projectile.Center + speed * 4f, speed, mod.ProjectileType("FrostShard"),
                                    projectile.damage / 2, projectile.knockBack / 2, projectile.owner);
                        }
                    }
                }
                else //forget target
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Vector2 distance = player.Center - projectile.Center;
                float length = distance.Length();
                if (length > 1500f)
                {
                    projectile.Center = player.Center;
                    projectile.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 12f;
                }
                else if (length > 100f)
                {
                    distance /= 12f;
                    projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
                }
                else
                {
                    if (projectile.velocity.Length() < 12f)
                        projectile.velocity *= 1.05f;
                }

                projectile.localAI[1]++;
                if (projectile.localAI[1] > 6f)
                {
                    projectile.localAI[1] = 0f;
                    projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 1000f);
                    if (projectile.ai[0] != -1)
                        projectile.netUpdate = true;
                }
            }

            //projectile.rotation = projectile.velocity.ToRotation();
            projectile.rotation += projectile.velocity.Length() / 12f * (projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++projectile.frameCounter > 3)
            {
                if (++projectile.frame >= 6)
                    projectile.frame = 0;
                projectile.frameCounter = 0;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 360);
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
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}