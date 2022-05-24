using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearArc : ModProjectile
    {
        public int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Arc");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.5f;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.alpha = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 120 * (Projectile.extraUpdates + 1);
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (counter == 0)
            {
                Projectile.timeLeft = (int)Projectile.ai[1] * Projectile.MaxUpdates;
                Projectile.ai[1] = Main.rand.Next(100);
                Projectile.netUpdate = true;
            }

            if (++counter > 15 * Projectile.MaxUpdates)
                Projectile.velocity = Vector2.Zero;

            Projectile.frameCounter = Projectile.frameCounter + 1;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.45f, 0.5f);
            if (Projectile.velocity == Vector2.Zero)
            {
                /*if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool flag = true;
                    for (int index = 1; index < Projectile.oldPos.Length; ++index)
                    {
                        if (Projectile.oldPos[index] != Projectile.oldPos[0])
                            flag = false;
                    }
                    if (flag)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (Main.rand.Next(Projectile.extraUpdates) != 0)
                    return;
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    float num1 = Projectile.rotation + (float)((Main.rand.Next(2) == 1 ? -1.0 : 1.0) * 1.57079637050629);
                    float num2 = (float)(Main.rand.NextDouble() * 0.800000011920929 + 1.0);
                    Vector2 vector2 = new Vector2((float)Math.Cos((double)num1) * num2, (float)Math.Sin((double)num1) * num2);
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, 226, vector2.X, vector2.Y, 0, new Color(), 1f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].scale = 1.2f;
                }
                if (Main.rand.Next(5) != 0)
                    return;
                int index3 = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(1.57079637050629, new Vector2()) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width - Vector2.One * 4f, 8, 8, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index3];
                dust.velocity = dust.velocity * 0.5f;
                Main.dust[index3].velocity.Y = -Math.Abs(Main.dust[index3].velocity.Y);*/
            }
            else
            {
                if (Projectile.frameCounter < Projectile.extraUpdates * 2)
                    return;
                Projectile.frameCounter = 0;
                float num1 = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
                int num2 = 0;
                Vector2 spinningpoint = -Vector2.UnitY;
                Vector2 rotationVector2;
                int num3;
                do
                {
                    int num4 = unifiedRandom.Next();
                    Projectile.ai[1] = (float)num4;
                    rotationVector2 = ((float)((double)(num4 % 100) / 100.0 * 6.28318548202515)).ToRotationVector2();
                    if ((double)rotationVector2.Y > 0.0)
                        rotationVector2.Y--;
                    bool flag = false;
                    if ((double)rotationVector2.Y > -0.0199999995529652)
                        flag = true;
                    if ((double)rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * (double)num1 + (double)Projectile.localAI[0] > 40.0)
                        flag = true;
                    if ((double)rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * (double)num1 + (double)Projectile.localAI[0] < -40.0)
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
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1] = 1f;
                goto label_3461;
            label_3460:
                spinningpoint = rotationVector2;
            label_3461:
                if (Projectile.velocity == Vector2.Zero || Projectile.velocity.Length() < 4f)
                {
                    Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.ai[0]).RotatedByRandom(Math.PI / 4) * 7f;
                    Projectile.ai[1] = Main.rand.Next(100);
                    return;
                }
                Projectile.localAI[0] += (float)((double)spinningpoint.X * (double)(Projectile.extraUpdates + 1) * 2.0) * num1;
                Projectile.velocity = spinningpoint.RotatedBy((double)Projectile.ai[0] + 1.57079637050629, new Vector2()) * num1;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.570796f;
            }

            /*for (int index1 = 1; index1 < Projectile.oldPos.Length; index1++)
            {
                const int max = 5;
                Vector2 offset = Projectile.oldPos[index1 - 1] - Projectile.oldPos[index1];
                offset /= max;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 position = Projectile.oldPos[index1] + offset * i;
                    int index2 = Dust.NewDust(position, Projectile.width, Projectile.height, 160, 0.0f, 0.0f, 0, new Color(), 1f);
                    Main.dust[index2].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[index2].velocity *= 0.2f;
                }
            }*/
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int index = 0; index < Projectile.oldPos.Length && ((double)Projectile.oldPos[index].X != 0.0 || (double)Projectile.oldPos[index].Y != 0.0); ++index)
            {
                Rectangle myRect = projHitbox;
                myRect.X = (int)Projectile.oldPos[index].X;
                myRect.Y = (int)Projectile.oldPos[index].Y;
                if (myRect.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            float num2 = (float)(Projectile.rotation + 1.57079637050629 + (Main.rand.Next(2) == 1 ? -1.0 : 1.0) * 1.57079637050629);
            float num3 = (float)(Main.rand.NextDouble() * 2.0 + 2.0);
            Vector2 vector2 = new Vector2((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                int index = Dust.NewDust(Projectile.oldPos[i], 0, 0, 229, vector2.X, vector2.Y, 0, new Color(), 1f);
                Main.dust[index].noGravity = true;
                Main.dust[index].scale = 1.7f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.CurseoftheMoon>(), 600);
            target.immune[Projectile.owner] = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = Projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i - 1] == Projectile.oldPos[i])
                    continue;
                Vector2 offset = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                int length = (int)offset.Length();
                offset.Normalize();
                const int step = 5;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = Projectile.oldPos[i] + offset * j;
                    Main.EntitySpriteDraw(texture2D13, value5 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                }
            }
            //Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}