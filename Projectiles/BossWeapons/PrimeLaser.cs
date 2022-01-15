using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PrimeLaser : ModProjectile 
    {
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Prime Laser");
		}
		public override void SetDefaults()
		{
			projectile.width = 5;
			projectile.height = 5;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.extraUpdates = 3;
			projectile.scale = 1f;
			projectile.timeLeft = 120;
			projectile.penetrate = 1;
			projectile.magic = true;
			projectile.ignoreWater = true;
		}

		public override void AI() //basically everything below is gross decompiled vanilla code im sorry
		{

			if (projectile.alpha > 0)
			{
				projectile.alpha -= 25;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}

			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.8f, 0f, 0.9f);
			float num1 = 100f;
			float num2 = 3f;
			if ((double)projectile.ai[1] == 0.0)
			{
				projectile.localAI[0] += num2;
				if ((double)projectile.localAI[0] > (double)num1)
					projectile.localAI[0] = num1;
			}
			else
			{
				projectile.localAI[0] -= num2;
				if ((double)projectile.localAI[0] <= 0.0)
				{
					projectile.Kill();
					return;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = oldVelocity;
			return true;
		}

		public override void Kill(int timeLeft)
		{
			int num = Main.rand.Next(6, 8);
			for (int index1 = 0; index1 < num; ++index1)
			{
				Vector2 position = projectile.Center - (projectile.velocity * index1/2);
				int index2 = Dust.NewDust(position, 0, 0, 130, 0.0f, 0.0f, 100, new Color(255, 196, 196), 2.1f);
				Dust dust = Main.dust[index2];
				dust.fadeIn = 0.2f;
				dust.scale *= 0.66f;
				dust.velocity = (projectile.velocity * 1.25f).RotatedByRandom(MathHelper.Pi / 12);
				Main.dust[index2].noGravity = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Microsoft.Xna.Framework.Color color25 = Color.White;
			float num150 = (float)(Main.projectileTexture[projectile.type].Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
			Microsoft.Xna.Framework.Rectangle value7 = new Microsoft.Xna.Framework.Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
			if (projectile.getRect().Intersects(value7))
			{
				Vector2 value8 = new Vector2(projectile.position.X - Main.screenPosition.X + num150, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY);
				float num176 = 100f * ((projectile.ai[0] == 1) ? 1.5f : 1f);
				float scaleFactor = 3f;
				if (projectile.ai[1] == 1f)
				{
					num176 = (float)((int)projectile.localAI[0]);
				}
				int num43;
				for (int num177 = 1; num177 <= (int)projectile.localAI[0]; num177 = num43 + 1)
				{
					Vector2 value9 = Vector2.Normalize(projectile.velocity) * (float)num177 * scaleFactor;
					Microsoft.Xna.Framework.Color color32 = projectile.GetAlpha(color25);
					color32 *= (num176 - (float)num177) / num176;
					color32.A = 0;
					SpriteBatch arg_7727_0 = Main.spriteBatch;
					Texture2D arg_7727_1 = Main.projectileTexture[projectile.type];
					Vector2 arg_7727_2 = value8 - value9;
					Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = null;
					arg_7727_0.Draw(arg_7727_1, arg_7727_2, sourceRectangle2, color32, projectile.rotation, new Vector2(num150, (float)(projectile.height / 2)), projectile.scale * ((projectile.ai[0] == 1) ? 2f : 1f), SpriteEffects.None, 0f);
					num43 = num177;
				}
			}
			return false;
		}
	}
}