using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class StyxScythe : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_274";

        public int rotationDirection;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Styx Scythe");
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                rotationDirection = Math.Sign(projectile.ai[1]);
                Main.PlaySound(SoundID.Item71, projectile.Center);
            }

            if (projectile.localAI[0] == 1) //extend out, locked to move around player
            {
                projectile.ai[0] += projectile.velocity.Length();
                projectile.Center = Main.player[projectile.owner].Center + Vector2.Normalize(projectile.velocity) * projectile.ai[0];

                if (projectile.Distance(Main.player[projectile.owner].Center) > Math.Abs(projectile.ai[1]))
                {
                    projectile.localAI[0]++;
                    projectile.localAI[1] = Math.Sign(projectile.ai[1]);
                    projectile.ai[0] = Math.Abs(projectile.ai[1]) - projectile.velocity.Length();
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.localAI[0] == 2) //orbit player, please dont ask how this code works i dont know either
            {
                //projectile.ai[0] += projectile.velocity.Length();
                projectile.Center = Main.player[projectile.owner].Center + Vector2.Normalize(projectile.velocity) * projectile.ai[0];
                projectile.Center += projectile.velocity.RotatedBy(Math.PI / 2 * projectile.localAI[1]);
                projectile.velocity = projectile.DirectionFrom(Main.player[projectile.owner].Center) * projectile.velocity.Length();

                if (++projectile.ai[1] > 180)
                {
                    projectile.localAI[0]++;
                    projectile.localAI[1] = 0;
                    projectile.ai[0] = 0;
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.timeLeft > 60) //now flying away, go into homing mode
            {
                if (projectile.ai[0] >= 0 && projectile.ai[0] < Main.maxNPCs)
                {
                    int ai0 = (int)projectile.ai[0];
                    if (Main.npc[ai0].CanBeChasedBy())
                    {
                        double num4 = (Main.npc[ai0].Center - projectile.Center).ToRotation() - projectile.velocity.ToRotation();
                        if (num4 > Math.PI)
                            num4 -= 2.0 * Math.PI;
                        if (num4 < -1.0 * Math.PI)
                            num4 += 2.0 * Math.PI;
                        projectile.velocity = projectile.velocity.RotatedBy(num4 * 0.17f);
                    }
                    else
                    {
                        projectile.ai[0] = -1f;
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    if (++projectile.localAI[1] > 12f)
                    {
                        projectile.localAI[1] = 0f;
                        projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 2000);
                        projectile.netUpdate = true;
                    }
                }
            }

            projectile.direction = projectile.spriteDirection = rotationDirection;
            projectile.rotation += projectile.spriteDirection * 0.7f * rotationDirection;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
            target.immune[projectile.owner] = 6;
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

            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}