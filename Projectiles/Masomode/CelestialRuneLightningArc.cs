using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CelestialRuneLightningArc : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_466";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lightning Arc");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        float colorlerp;
        bool playedsound = false;
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.scale = 0.5f;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 100;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 120 * (projectile.extraUpdates + 1);
            projectile.penetrate = -1;

            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;

            if (ModLoader.GetMod("Fargowiltas") != null)
                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", projectile);
        }

        public override void AI()
        {
            projectile.frameCounter = projectile.frameCounter + 1;
            Lighting.AddLight(projectile.Center, 0.3f, 0.45f, 0.5f);
            colorlerp += 0.05f;

            if(!playedsound)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 122, 0.5f, -0.5f);
                playedsound = true;
            }

            if (projectile.velocity == Vector2.Zero)
            {
                if (projectile.frameCounter >= projectile.extraUpdates * 2)
                {
                    projectile.frameCounter = 0;
                    bool flag = true;
                    for (int index = 1; index < projectile.oldPos.Length; ++index)
                    {
                        if (projectile.oldPos[index] != projectile.oldPos[0])
                            flag = false;
                    }
                    if (flag)
                    {
                        projectile.Kill();
                        return;
                    }
                }
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
            else
            {
                if (projectile.frameCounter < projectile.extraUpdates * 2)
                    return;
                projectile.frameCounter = 0;
                float num1 = projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)projectile.ai[1]);
                int num2 = 0;
                Vector2 spinningpoint = -Vector2.UnitY;
                Vector2 rotationVector2;
                int num3;
                do
                {
                    int num4 = unifiedRandom.Next();
                    projectile.ai[1] = (float)num4;
                    rotationVector2 = ((float)((double)(num4 % 100) / 100.0 * 6.28318548202515)).ToRotationVector2();
                    if ((double)rotationVector2.Y > 0.0)
                        rotationVector2.Y--;
                    bool flag = false;
                    if ((double)rotationVector2.Y > -0.0199999995529652)
                        flag = true;
                    if ((double)rotationVector2.X * (double)(projectile.extraUpdates + 1) * 2.0 * (double)num1 + (double)projectile.localAI[0] > 40.0)
                        flag = true;
                    if ((double)rotationVector2.X * (double)(projectile.extraUpdates + 1) * 2.0 * (double)num1 + (double)projectile.localAI[0] < -40.0)
                        flag = true;
                    if (flag)
                    {
                        num3 = num2;
                        num2 = num3 + 1;
                    }
                    else
                        goto label_3460;
                }
                while (num3 < 100);
                projectile.velocity = Vector2.Zero;
                projectile.localAI[1] = 1f;
                goto label_3461;
                label_3460:
                spinningpoint = rotationVector2;
                label_3461:
                if (projectile.velocity == Vector2.Zero || projectile.velocity.Length() < 4f)
                {
                    projectile.velocity = Vector2.UnitX.RotatedBy(projectile.ai[0]).RotatedByRandom(Math.PI / 4) * 7f;
                    projectile.ai[1] = Main.rand.Next(100);
                    return;
                }
                projectile.localAI[0] += (float)((double)spinningpoint.X * (double)(projectile.extraUpdates + 1) * 2.0) * num1;
                projectile.velocity = spinningpoint.RotatedBy((double)projectile.ai[0] + 1.57079637050629, new Vector2()) * num1;
                projectile.rotation = projectile.velocity.ToRotation() + 1.570796f;
            }

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

            return Color.Lerp(Color.LightSkyBlue, Color.White, 0.5f + (float)Math.Sin(colorlerp)/2) * 0.5f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero || projectile.oldPos[i-1] == projectile.oldPos[i])
                    continue;
                Vector2 offset = projectile.oldPos[i - 1] - projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = projectile.scale * (float)Math.Sin(i/MathHelper.Pi);
                offset.Normalize();
                const int step = 3;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, projectile.rotation, origin2, scale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
            //Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}