using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PrimeLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prime Laser");
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI() //basically everything below is gross decompiled vanilla code im sorry
        {

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.8f, 0f, 0.9f);
            float num1 = 100f;
            float num2 = 3f;
            if ((double)Projectile.ai[1] == 0.0)
            {
                Projectile.localAI[0] += num2;
                if ((double)Projectile.localAI[0] > (double)num1)
                    Projectile.localAI[0] = num1;
            }
            else
            {
                Projectile.localAI[0] -= num2;
                if ((double)Projectile.localAI[0] <= 0.0)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            int num = Main.rand.Next(6, 8);
            for (int index1 = 0; index1 < num; ++index1)
            {
                Vector2 position = Projectile.Center - (Projectile.velocity * index1 / 2);
                int index2 = Dust.NewDust(position, 0, 0, 130, 0.0f, 0.0f, 100, new Color(255, 196, 196), 2.1f);
                Dust dust = Main.dust[index2];
                dust.fadeIn = 0.2f;
                dust.scale *= 0.66f;
                dust.velocity = (Projectile.velocity * 1.25f).RotatedByRandom(MathHelper.Pi / 12);
                Main.dust[index2].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Microsoft.Xna.Framework.Color color25 = Color.White;
            float num150 = (float)(TextureAssets.Projectile[Projectile.type].Value.Width - Projectile.width) * 0.5f + (float)Projectile.width * 0.5f;
            Microsoft.Xna.Framework.Rectangle value7 = new Microsoft.Xna.Framework.Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
            if (Projectile.getRect().Intersects(value7))
            {
                Vector2 value8 = new Vector2(Projectile.position.X - Main.screenPosition.X + num150, Projectile.position.Y - Main.screenPosition.Y + (float)(Projectile.height / 2) + Projectile.gfxOffY);
                float num176 = 100f * ((Projectile.ai[0] == 1) ? 1.5f : 1f);
                float scaleFactor = 3f;
                if (Projectile.ai[1] == 1f)
                {
                    num176 = (float)((int)Projectile.localAI[0]);
                }
                int num43;
                for (int num177 = 1; num177 <= (int)Projectile.localAI[0]; num177 = num43 + 1)
                {
                    Vector2 value9 = Vector2.Normalize(Projectile.velocity) * (float)num177 * scaleFactor;
                    Microsoft.Xna.Framework.Color color32 = Projectile.GetAlpha(color25);
                    color32 *= (num176 - (float)num177) / num176;
                    color32.A = 0;
                    SpriteBatch arg_7727_0 = Main.spriteBatch;
                    Texture2D arg_7727_1 = TextureAssets.Projectile[Projectile.type].Value;
                    Vector2 arg_7727_2 = value8 - value9;
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = null;
                    arg_7727_0.Draw(arg_7727_1, arg_7727_2, sourceRectangle2, color32, Projectile.rotation, new Vector2(num150, (float)(Projectile.height / 2)), Projectile.scale * ((Projectile.ai[0] == 1) ? 2f : 1f), SpriteEffects.None, 0);
                    num43 = num177;
                }
            }
            return false;
        }
    }
}