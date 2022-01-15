using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosLightning : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_466";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lightning Arc");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        float colorlerp;
        bool playedsound = false;
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 0.75f;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.alpha = 100;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.3f, 0.45f, 0.5f);
            colorlerp += 0.15f;
            projectile.localAI[0]++;

            if (!playedsound)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 122, 0.5f, -0.5f);

                playedsound = true;
            }

            if (Main.rand.NextBool(6))
            {
                if (Main.rand.Next(projectile.extraUpdates) != 0)
                    return;
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    float num1 = projectile.rotation + (float)((Main.rand.Next(2) == 1 ? -1.0 : 1.0) * 1.57079637050629);
                    float num2 = (float)(Main.rand.NextDouble() * 0.800000011920929 + 1.0);
                    Vector2 vector2 = new Vector2((float)Math.Cos((double)num1) * num2, (float)Math.Sin((double)num1) * num2);
                    int index2 = Dust.NewDust(projectile.Center, 0, 0, 226, vector2.X, vector2.Y, 0, new Color(), 1f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].scale = 1.2f;
                }
                if (Main.rand.Next(5) != 0)
                    return;
                int index3 = Dust.NewDust(projectile.Center + projectile.velocity.RotatedBy(1.57079637050629, new Vector2()) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width - Vector2.One * 4f, 8, 8, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index3];
                dust.velocity = dust.velocity * 0.5f;
                Main.dust[index3].velocity.Y = -Math.Abs(Main.dust[index3].velocity.Y);
            }

            float num3 = projectile.velocity.Length(); //take length of initial velocity
            Vector2 spinningpoint = Vector2.UnitX.RotatedBy(projectile.ai[0]) * num3; //create a base velocity to modify for actual velocity of projectile
            Vector2 rotationVector2 = spinningpoint.RotatedBy(projectile.ai[1] * (Math.Floor(Math.Sin((projectile.localAI[0]- MathHelper.Pi/4) * 2)) + 0.5f) * MathHelper.Pi/4); //math thing for zigzag pattern
            projectile.velocity = rotationVector2;
            projectile.rotation = projectile.velocity.ToRotation() + 1.570796f;

            /*for (int index1 = 1; index1 < projectile.oldPos.Length; index1++)
            {
                const int max = 5;
                Vector2 offset = projectile.oldPos[index1 - 1] - projectile.oldPos[index1];
                offset /= max;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 position = projectile.oldPos[index1] + offset * i;
                    int index2 = Dust.NewDust(position, projectile.width, projectile.height, 160, 0.0f, 0.0f, 0, new Color(), 1f);
                    Main.dust[index2].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[index2].velocity *= 0.2f;
                }
            }*/
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int index = 0; index < projectile.oldPos.Length && ((double)projectile.oldPos[index].X != 0.0 || (double)projectile.oldPos[index].Y != 0.0); ++index)
            {
                Rectangle myRect = projHitbox;
                myRect.X = (int)projectile.oldPos[index].X;
                myRect.Y = (int)projectile.oldPos[index].Y;
                if (myRect.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float num2 = (float)(projectile.rotation + 1.57079637050629 + (Main.rand.Next(2) == 1 ? -1.0 : 1.0) * 1.57079637050629);
            float num3 = (float)(Main.rand.NextDouble() * 2.0 + 2.0);
            Vector2 vector2 = new Vector2((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                int index = Dust.NewDust(projectile.oldPos[i], 0, 0, 229, vector2.X, vector2.Y, 0, new Color(), 1f);
                Main.dust[index].noGravity = true;
                Main.dust[index].scale = 1.7f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {

            return Color.Lerp(new Color(34, 221, 251), new Color(208, 253, 235), 0.5f + (float)Math.Sin(colorlerp)/2); //vortex colors
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = new Color(33, 160, 141);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero || projectile.oldPos[i-1] == projectile.oldPos[i])
                    continue;
                Vector2 offset = projectile.oldPos[i - 1] - projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = projectile.scale * (float)Math.Sin((i * 0.5f) / MathHelper.Pi);
                offset.Normalize();
                const int step = 3;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, projectile.rotation, origin2, scale + 0.15f, SpriteEffects.FlipHorizontally, 0f);
                }
            }
            //Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero || projectile.oldPos[i - 1] == projectile.oldPos[i])
                    continue;
                Vector2 offset = projectile.oldPos[i - 1] - projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = projectile.scale * (float)Math.Sin((i * 0.5f) / MathHelper.Pi);
                offset.Normalize();
                const int step = 3;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, projectile.rotation, origin2, scale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}