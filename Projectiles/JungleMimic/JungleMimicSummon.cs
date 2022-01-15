using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasSouls.Projectiles.JungleMimic
{
    public class JungleMimicSummon : ModProjectile
    {
        public int counter;
        public bool trailbehind;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Jungle Mimic");
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 2f;
            projectile.penetrate = -1;
            projectile.aiStyle = 26;
            projectile.width = 52;
            projectile.height = 56;
            aiType = ProjectileID.BabySlime;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                player.ClearBuff(mod.BuffType("JungleMimicSummonBuff"));
            }

            if (player.HasBuff(mod.BuffType("JungleMimicSummonBuff")))
            {
                projectile.timeLeft = 2;
            }

            counter++;
            if (counter % 15 == 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    NPC targetNPC = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000, true));
                    if (targetNPC != null)
                    {
                        Vector2 shootVel = projectile.DirectionTo(targetNPC.Center);
                        Main.PlaySound(SoundID.Item11, projectile.Center);
                        Projectile.NewProjectile(projectile.Center, shootVel * 14f + targetNPC.velocity/2, mod.ProjectileType("JungleMimicSummonCoin"), projectile.damage / 4, projectile.knockBack, Main.myPlayer);
                    }
                }
            }

            if(counter > 180)
            {
                if (counter > 300)
                    counter = 0;

                if (projectile.owner == Main.myPlayer)
                {
                    NPC targetNPC = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000, true));
                    if (targetNPC != null)
                    {
                        projectile.frameCounter++;
                        trailbehind = true;
                        if(projectile.frameCounter > 8)
                        {
                            projectile.frame++;
                            if (projectile.frame > 5)
                                projectile.frame = 2;
                        }

                        for (int index = 0; index < 1000; ++index)
                        {
                            if (index != projectile.whoAmI && Main.projectile[index].active && (Main.projectile[index].owner == projectile.owner && Main.projectile[index].type == projectile.type) && (double)Math.Abs((float)(projectile.position.X - Main.projectile[index].position.X)) + (double)Math.Abs((float)(projectile.position.Y - Main.projectile[index].position.Y)) < (double)projectile.width)
                            {
                                if (projectile.position.X < Main.projectile[index].position.X)
                                {
                                    projectile.velocity.X -= 0.05f;
                                }
                                else
                                {
                                    projectile.velocity.X += 0.05f;
                                }
                                if (projectile.position.Y < Main.projectile[index].position.Y)
                                {
                                    projectile.velocity.Y -= 0.05f;
                                }
                                else
                                {
                                    projectile.velocity.Y += 0.05f;
                                }
                            }
                        }

                        Vector2 dashVel = projectile.DirectionTo(targetNPC.Center);
                        projectile.velocity = Vector2.Lerp(projectile.velocity, dashVel * 18, 0.03f);
                        projectile.rotation = 0;
                        projectile.tileCollide = false;
                        projectile.direction = Math.Sign(projectile.velocity.X);
                        projectile.spriteDirection = -projectile.direction;
                        return false;
                    }
                }
            }
            trailbehind = false;
            projectile.tileCollide = true;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type] && trailbehind; i++)
            {
                Color color27 = projectile.GetAlpha(lightColor) * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), 
                    color27, num165, origin2, projectile.scale, projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY - 4),
                new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2,
                projectile.scale, projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }
    }
}

   