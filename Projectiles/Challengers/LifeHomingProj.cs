using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeHomingProj : ModProjectile
	{
		public bool home = true;

        public override string Texture => "Terraria/Images/NPC_75";

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lesser Fairy");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.Pixie];
        }
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
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
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.X < 0 ? Projectile.velocity.ToRotation() + (float)Math.PI : Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(6))
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
				Main.dust[d].noGravity = true;
				Main.dust[d].velocity *= 0.5f;
			}

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (Projectile.ai[0] > 30f)
			{
                if (Projectile.ai[0] == 31f)
                    Projectile.ai[1] = Player.FindClosest(Projectile.Center, 0, 0);

				if (Main.player[(int)Projectile.ai[1]].active && !Main.player[(int)Projectile.ai[1]].dead)
				{
					Vector2 vectorToIdlePosition = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
					float num = vectorToIdlePosition.Length();
					float speed = 24f;
					float inertia = 15f;
					float deadzone = 150f;
					if (num > deadzone && home)
					{
						vectorToIdlePosition.Normalize();
						vectorToIdlePosition *= speed;
						Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
					}
					else if (Projectile.velocity == Vector2.Zero)
					{
						Projectile.velocity.X = -0.15f;
						Projectile.velocity.Y = -0.05f;
					}
					if (num < deadzone)
					{
						home = false;
					}
				}
			}
			if (Projectile.ai[0] > 600f)
			{
				Projectile.Kill();
			}
			Projectile.ai[0] += 1f;
		}
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
