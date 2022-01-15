using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SlimeSlash : ModProjectile 
    {
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Slime Slash");

			ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}
		public override void SetDefaults()
		{
			projectile.width = 5;
			projectile.height = 5;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.alpha = 100;
			projectile.scale = 0.5f;
			//projectile.timeLeft = 120;
			projectile.extraUpdates = 1;
			projectile.penetrate = -1;
			projectile.melee = true;
			projectile.ignoreWater = true;
			projectile.usesIDStaticNPCImmunity = true;
			projectile.idStaticNPCHitCooldown = 6;
			projectile.tileCollide = false;
			projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
		}

		Color DrawColor = Color.Blue;
		public override void AI() //basically everything below is gross decompiled vanilla code im sorry
		{

			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0f, 0.9f);
			float num1 = 100f;
			float num2 = 7f;
			if ((double)projectile.ai[1] == 0.0)
			{
				projectile.localAI[0] += num2;
				if ((double)projectile.localAI[0] > (double)num1)
				{
					projectile.localAI[0] = num1;
					projectile.ai[1] = 1;
				}

			}
			else
			{
				projectile.localAI[0] -= num2 * 0.66f;
				if ((double)projectile.localAI[0] <= 40)
				{
					projectile.Kill();
					return;
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			for(int i = 0; i < projectile.localAI[0]; i += 7)
			{
				Vector2 hitboxpos = projectile.Center - (i * Vector2.Normalize(projectile.velocity));
				Rectangle newhitbox = new Rectangle((int)hitboxpos.X, (int)hitboxpos.Y, projectile.width, projectile.height);
				if (targetHitbox.Intersects(newhitbox))
					return true;
			}
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Microsoft.Xna.Framework.Color color25 = DrawColor;
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
					Microsoft.Xna.Framework.Color color32 = color25;
					color32 *= (num176 - (float)num177) / num176;
					color32 = Color.Lerp(color32, Color.Transparent, (projectile.alpha) / 255);
					SpriteBatch arg_7727_0 = Main.spriteBatch;
					Texture2D arg_7727_1 = Main.projectileTexture[projectile.type];
					Vector2 arg_7727_2 = value8 - value9;
					float scale = projectile.scale * (float)Math.Sin((num177 * 10 / projectile.localAI[0]) / MathHelper.Pi);
					Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = null;
					arg_7727_0.Draw(arg_7727_1, arg_7727_2, sourceRectangle2, color32, projectile.rotation, new Vector2(num150, (float)(projectile.height / 2)), 
						scale * ((projectile.ai[0] == 1) ? 2f : 1f), SpriteEffects.None, 0f);
					num43 = num177;
				}
			}
			return false;
		}
	}
}