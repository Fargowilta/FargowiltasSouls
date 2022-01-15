using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class MiniSaucer : ModProjectile
    {
        private int rotation = 0;
        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mini Saucer");
            //ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 25;
            projectile.height = 25;
            projectile.scale = 1f;
            projectile.timeLeft *= 5;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.scale = 1.5f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().MiniSaucer)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
                projectile.damage = (int)(50f * player.minionDamage);

            /*NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
            {
                projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                projectile.netUpdate = true;
            }*/
            
            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
                mousePos.Y -= 250f;
            }

            if (projectile.Distance(Main.player[projectile.owner].Center) > 2000)
            {
                projectile.Center = player.Center;
                projectile.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 12f;
            }

            Vector2 distance = mousePos - projectile.Center;
            float length = distance.Length();
            if (length > 10f)
            {
                distance /= 18f;
                projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (projectile.velocity.Length() < 12f)
                    projectile.velocity *= 1.05f;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    projectile.netUpdate = true;
                }

                if (player.controlUseItem)
                {
                    if (++projectile.localAI[0] > 5f) //shoot laser
                    {
                        projectile.localAI[0] = 0f;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            Vector2 vel = projectile.DirectionTo(Main.MouseWorld) * 16f;
                            Main.PlaySound(SoundID.Item12, projectile.Center);

                            Projectile.NewProjectile(projectile.Center + projectile.velocity * 2.5f,
                                vel.RotatedBy((Main.rand.NextDouble() - 0.5) * 0.785398185253143 / 3.0),
                                mod.ProjectileType("SaucerLaser"), projectile.damage / 2, projectile.knockBack, projectile.owner);
                        }
                    }

                    if (++projectile.localAI[1] > 20f) //try to find target for rocket
                    {
                        projectile.localAI[1] = 0f;

                        float maxDistance = 500f;
                        int possibleTarget = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.CanBeChasedBy(projectile) && Collision.CanHitLine(projectile.Center, 0, 0, npc.Center, 0, 0))
                            {
                                float npcDistance = player.Distance(npc.Center);
                                if (npcDistance < maxDistance)
                                {
                                    maxDistance = npcDistance;
                                    possibleTarget = i;
                                }
                            }
                        }

                        if (possibleTarget >= 0) //shoot rocket
                        {
                            Vector2 vel = new Vector2(0f, -10f).RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI);
                            Projectile.NewProjectile(projectile.Center, vel, ModContent.ProjectileType<SaucerRocket>(),
                                projectile.damage, projectile.knockBack * 4f, projectile.owner, possibleTarget, 20f);
                        }

                        Projectile.NewProjectile(projectile.Center, Vector2.UnitY, ModContent.ProjectileType<SaucerDeathray>(),
                            projectile.damage / 2, projectile.knockBack / 2f, projectile.owner, 0f, projectile.identity);
                    }
                }
            }

            const float cap = 32f;
            if (projectile.velocity.X > cap)
                projectile.velocity.X = cap;
            if (projectile.velocity.X < -cap)
                projectile.velocity.X = -cap;
            if (projectile.velocity.Y > cap)
                projectile.velocity.Y = cap;
            if (projectile.velocity.Y < -cap)
                projectile.velocity.Y = -cap;
            
            if (Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SaucerDeathray>()] > 0)
            {
                projectile.rotation = 0;
                rotation = 0;
            }
            else
            {
                projectile.rotation = (float)Math.Sin(2 * Math.PI * rotation++ / 90) * (float)Math.PI / 8f;
                if (rotation > 180)
                    rotation = 0;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 360);
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

            SpriteEffects effects = SpriteEffects.None;

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