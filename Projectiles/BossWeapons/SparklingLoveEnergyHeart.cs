using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingLoveEnergyHeart : ModProjectile
    {   
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Energy Heart");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            
            projectile.timeLeft = 90;
            projectile.extraUpdates = 1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.Center.X;
                projectile.localAI[1] = projectile.Center.Y;
                Main.PlaySound(SoundID.Item44, projectile.Center);
            }

            projectile.rotation = projectile.ai[0];

            float speed = projectile.velocity.Length();
            speed += projectile.ai[1];
            projectile.velocity = Vector2.Normalize(projectile.velocity) * speed;
        }

        public override void Kill(int timeLeft)
        {
            FargoSoulsUtil.HeartDust(projectile.Center, projectile.rotation + MathHelper.PiOver2);

            /*for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 86, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }*/

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.UnitX.RotatedBy(projectile.rotation),
                        ModContent.ProjectileType<SparklingLoveDeathray2>(), projectile.damage, projectile.knockBack, projectile.owner);
                Projectile.NewProjectile(projectile.Center, Vector2.UnitX.RotatedBy(projectile.rotation + (float)Math.PI),
                        ModContent.ProjectileType<SparklingLoveDeathray2>(), projectile.damage, projectile.knockBack, projectile.owner);

                Projectile.NewProjectile(new Vector2(projectile.localAI[0], projectile.localAI[1]), Vector2.UnitX.RotatedBy(projectile.rotation - (float)Math.PI / 2),
                    ModContent.ProjectileType<SparklingLoveDeathray2>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 1;
            target.AddBuff(BuffID.Lovestruck, 300);
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

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}