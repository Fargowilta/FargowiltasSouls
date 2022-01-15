using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HellFlame : ModProjectile
    {
        public int targetID = -1;
        public int searchTimer = 18;

        public override string Texture => "Terraria/Projectile_687";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hell Flame");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.Homing[projectile.type] = true;
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetID = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.ranged = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Black;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = Main.rand.NextFloat(0.25f, 2f); //used for random variation in homing
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            if (projectile.timeLeft > 120) projectile.timeLeft = 120;
            projectile.ai[1]++;
            projectile.scale = 1f + projectile.ai[1] / 80;

            projectile.rotation += 0.3f * projectile.direction;

            projectile.frameCounter++;
            if (projectile.frameCounter > 17)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }

            if (projectile.frame > 6)
                projectile.Kill();

            if (projectile.frame > 4)
            {
                projectile.alpha = 155;
                return;
            }

            if (targetID == -1) //no target atm
            {
                if (searchTimer == 0) //search every 18/3=6 ticks
                {
                    searchTimer = 18;
                    targetID = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 300);
                    projectile.netUpdate = true;
                }
                searchTimer--;
            }
            else //currently have target
            {
                NPC npc = Main.npc[targetID];

                if (npc.active && npc.CanBeChasedBy()) //target is still valid
                {
                    Vector2 distance = npc.Center - projectile.Center;
                    double angle = distance.ToRotation() - projectile.velocity.ToRotation();
                    if (angle > Math.PI)
                        angle -= 2.0 * Math.PI;
                    if (angle < -Math.PI)
                        angle += 2.0 * Math.PI;

                    if (projectile.ai[0] == -1)
                    {
                        if (Math.Abs(angle) > Math.PI * 0.75)
                        {
                            projectile.velocity = projectile.velocity.RotatedBy(angle * 0.07 * projectile.localAI[0]);
                        }
                        else
                        {
                            float range = distance.Length();
                            float difference = 12.7f / range;
                            distance *= difference;
                            distance /= 7f;
                            projectile.velocity += distance;
                            if (range > 70f)
                            {
                                projectile.velocity *= 0.977f;
                            }
                        }
                    }
                    else
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(angle * 0.1 * projectile.localAI[0]);
                    }
                }
                else //target lost, reset
                {
                    targetID = -1;
                    searchTimer = 0;
                    projectile.netUpdate = true;
                }
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= 30;
            hitbox.Y -= 30;
            hitbox.Width += 60;
            hitbox.Height += 60;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
            target.AddBuff(BuffID.OnFire, 180, false);
            target.AddBuff(BuffID.Oiled, 180, false);
            target.AddBuff(BuffID.BetsysCurse, 180, false);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.Fuchsia * projectile.Opacity;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale;// * 0.9f;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i] + (Main.GlobalTime * 0.6f);
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                    color27, num165, origin2, scale, effects, 0f);
            }*/

            Color color27 = Color.Fuchsia * projectile.Opacity;
            float scale = projectile.scale;
            Vector2 value4 = projectile.Center;
            if (projectile.velocity != Vector2.Zero && !projectile.velocity.HasNaNs())
                value4 -= Vector2.Normalize(projectile.velocity) * 4f;
            float num165 = projectile.rotation + (Main.GlobalTime * 0.6f);
            
            Vector2 previousPosOffset = projectile.oldPos[2] - projectile.position;
            float prevPosRotation = projectile.oldRot[2] + (Main.GlobalTime * 0.6f);
            Main.spriteBatch.Draw(texture2D13, previousPosOffset + value4 - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color27, prevPosRotation, origin2, scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D13, previousPosOffset + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Color.Black * projectile.Opacity, prevPosRotation, origin2, projectile.scale, effects, 0f);

            Main.spriteBatch.Draw(texture2D13, value4 - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color27, num165, origin2, scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), 
                Color.Black * projectile.Opacity, num165, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}