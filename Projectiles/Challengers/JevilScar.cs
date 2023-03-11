using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.NPCs.Challengers;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class JevilScar : ModProjectile
	{
        //private bool init = false;

        //private NPC lifelight;

        //private Vector2 ScopeAtPlayer = Vector2.Zero;

        private float rotspeed = 0;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Enchanted Lightblade");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		public override void SetDefaults()
		{
			Projectile.width = 54; //actually 56 but it's diagonal
			Projectile.height = 54; //actually 56 but it's diagonal
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
            //Old AI, for jevilsknife attack
            /*
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
			Projectile.ai[0]++;
			*/

            //New AI, for hexagon attack

            if (Projectile.frameCounter > 4)
            {
                Projectile.frame %= 3;
                Projectile.frameCounter = 0;
            }
            Projectile.frameCounter++;

            if (Projectile.ai[0] > 30f)
            {
                if (Projectile.ai[0] == 31f)
                    Projectile.ai[1] = Player.FindClosest(Projectile.Center, 0, 0);

                if (Main.player[(int)Projectile.ai[1]].active && !Main.player[(int)Projectile.ai[1]].dead)
                {
                    Vector2 vectorToIdlePosition = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
                    float speed = FargoSoulsWorld.MasochistModeReal ? 24f : 20f;
                    float inertia = 48f;
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                    if (Projectile.velocity == Vector2.Zero)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                }
            }
            if (Projectile.ai[0] > 1200 || NPC.CountNPCS(ModContent.NPCType<LifeChallenger>()) < 1) //set to 1200 at end of attack by lieflight, then fades out
            {
                Projectile.alpha += 17;
				Projectile.hostile = false;
            }
			if (Projectile.alpha >= 240)
			{
				Projectile.Kill();
			}
            Projectile.ai[0] += 1f;

			if (rotspeed == 0)
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (rotspeed < MathHelper.Pi / 10)
				rotspeed += MathHelper.Pi / 10 / 90;

			Projectile.rotation += rotspeed;
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

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.5f)
            {
                Color color27 = new Color(255, 51, 153) * Projectile.Opacity * 0.5f;
                color27.A = (byte)(color26.A / 2);
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            return false;
        }
    }
}
