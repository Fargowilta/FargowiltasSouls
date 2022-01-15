using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingLove : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/SparklingLove";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            //projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 2;

            projectile.aiStyle = -1;
            projectile.scale = 2f;
        }
        
        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.direction = projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;

                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 2 * i),
                            ModContent.ProjectileType<SparklingLoveDeathray>(), projectile.damage, projectile.knockBack, projectile.owner,
                            (float)Math.PI / 2 * 1.0717f * projectile.direction, projectile.identity);
                    }
                }
            }

            if (projectile.ai[0] == 0)
            {
                if (projectile.Distance(Main.player[projectile.owner].Center) > 800)
                {
                    projectile.ai[0] = 1;
                    projectile.netUpdate = true;

                    if (projectile.localAI[1] == 0)
                        projectile.localAI[1] = 1;
                }
            }
            else
            {
                projectile.extraUpdates = 0;
                projectile.velocity = projectile.DirectionTo(Main.player[projectile.owner].Center) * (projectile.velocity.Length() + 1f / 10f);

                if (projectile.Distance(Main.player[projectile.owner].Center) <= projectile.velocity.Length())
                    projectile.Kill();
            }

            if (projectile.localAI[1] == 1)
            {
                projectile.localAI[1] = 2;
                HeartBurst(projectile.Center);
            }

            projectile.rotation += projectile.direction * -0.4f;

            for (int i = 0; i < 2; i++)
            {
                int num812 = Dust.NewDust(projectile.position, projectile.width, projectile.height,
                    86, projectile.velocity.X / 2, projectile.velocity.Y / 2, 0, default(Color), 1.7f);
                Main.dust[num812].noGravity = true;
            }
        }

        private void HeartBurst(Vector2 spawnPos)
        {
            if (projectile.owner != Main.myPlayer)
                return;

            Main.PlaySound(SoundID.Item21, spawnPos);
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = 14f * Vector2.Normalize(projectile.velocity).RotatedBy(Math.PI / 4 * (i + 0.5));
                Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<SparklingLoveHeart>(), projectile.damage, projectile.knockBack, projectile.owner, -1, 45);
                FargoSoulsUtil.HeartDust(spawnPos, vel.ToRotation(), vel);
            }

            /*for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 272, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 272, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 60; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 5f * projectile.scale;
                vector6 = vector6.RotatedBy((i - (60 / 2 - 1)) * 6.28318548f / 60) + spawnPos;
                Vector2 vector7 = vector6 - spawnPos;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 86, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 2;
                HeartBurst(target.Center);
            }

            target.AddBuff(BuffID.Lovestruck, 300);
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
            Texture2D texture2D14 = mod.GetTexture("Items/Weapons/FinalUpgrades/SparklingLove_glow");
            Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}