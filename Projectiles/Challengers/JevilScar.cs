using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class JevilScar : ModProjectile
	{
		private bool init = false;

		private NPC lifelight;

		Vector2 ScopeAtPlayer = new Vector2();

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Enchanted Lightblade");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		public override void SetDefaults()
		{
			Projectile.width = 90; //actually 56 but it's diagonal
			Projectile.height = 90; //actually 56 but it's diagonal
            Projectile.aiStyle = 0;
			Projectile.hostile = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //circular hitbox
		{
			int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
			int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

			if (Math.Abs(clampedX) > targetHitbox.Width / 2)
				clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
			if (Math.Abs(clampedY) > targetHitbox.Height / 2)
				clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

			int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
			int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

			return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
		}

		public override void AI()
		{
			if (!init)
			{
				//at this time, Projectile.ai[1] is rotation in degrees
				lifelight = Main.npc[(int)Projectile.ai[0]]; //get npc
				ScopeAtPlayer = lifelight.Center + new Vector2(0, 600);
				Projectile.ai[0] = 0;
				init = true;
			}
			Projectile.rotation += 0.5f;
			if (Projectile.ai[0] > 60 * 12)
				Projectile.alpha += 10;

			if (Projectile.alpha > 250)
				Projectile.Kill();

			/* homing movement method, flawed and weak
				Vector2 vectorToIdlePosition = ScopeAtPlayer - Projectile.Center;
				float num = vectorToIdlePosition.Length();
				float speed = 10f;
				float inertia = 10f;
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;
				if (Projectile.velocity == Vector2.Zero)
				{
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}*/

			//teleport method, correct and superior but less cool
			//max amplitude MaxHeight, rotation during wave bounceTime, rotation starts at Projectile.ai[1] (provided by NewProjectile). 
			const int bounceTime = 100; //ticks per bounce
			const float MaxHeight = 500; //max distance from center
			const int degPB = 175; //degrees per bounce
			if (Projectile.ai[0] >= (bounceTime / 2)) //start at 40 ticks means we start at max height and get 40 seconds of reaction time before movement
			{

				float dist = Math.Abs(MaxHeight * (float)Math.Sin(Projectile.ai[0] * (Math.PI / bounceTime))); // pi/x means x is ticks per bounce, half of x is peak of bounce



				float spin = Projectile.ai[1] * (float)Math.PI / 180f;
				Projectile.Center = ScopeAtPlayer + dist * spin.ToRotationVector2();
				Projectile.ai[1] += (degPB / bounceTime); // degrees/bounce rotation
			}
			/*if (Projectile.ai[0] % bounceTime == 0) ; //play sound when at center
			{ 
				SoundEngine.PlaySound(SoundID.Item71, ScopeAtPlayer);
			}*/
			Projectile.ai[0]++;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (FargoSoulsWorld.EternityMode)
				target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
		}

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Red * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
