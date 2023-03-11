using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeSlurp : ModProjectile
	{
        public override string Texture => "FargowiltasSouls/NPCs/Challengers/LifeChallengerParts/ShardGem1";

		private int rGem = 1;

        public bool home = true;

		public bool homingonPlayer;

		public bool chosenDirection;

		public bool First = true;

		public NPC lifelight;

        private int RotDirect = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gem");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
			Projectile.light = 0.5f;
			Projectile.scale = 1f;
		}

        public override void AI()
		{
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.Next(100);
                RotDirect = Main.rand.NextBool(2) ? -1 : 1;
				rGem = Main.rand.Next(1, 9);
            }
            Projectile.rotation += 0.2f * RotDirect;

            if (Main.rand.NextBool(6))
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.5f;
                Main.dust[index2].velocity.Y *= 0.5f;
            }

            if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 4;
			}
			if (Projectile.ai[0] > 30f)
			{
				if (First)
				{
					lifelight = Main.npc[(int)Projectile.ai[1]];
					Projectile.ai[1] = 0;
					First = false;
				}
				Player Player = Main.player[lifelight.target];
				Vector2 vectorToIdlePosition = Projectile.Center;
				float speed = 8f;
				float inertia = 5f;
				if (Projectile.ai[1] <= 90f)
				{
					vectorToIdlePosition = lifelight.Center - Projectile.Center;
					speed = 8f;
				}
				if (Projectile.ai[1] > 90f)
				{
					vectorToIdlePosition = Player.Center - Projectile.Center;
					speed = 12f;
					homingonPlayer = true;
					home = false;
				}
				float num = vectorToIdlePosition.Length();
				if (num < 200f && homingonPlayer)
				{
					home = false;
				}
				if (num < 20f)
				{
					Projectile.ai[1] += 1f;
				}
				if (num > 20f && home)
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
				if (!home && homingonPlayer && !chosenDirection)
				{
					double rotationrad = MathHelper.ToRadians(Main.rand.Next(-100, 100));
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition = vectorToIdlePosition.RotatedBy(rotationrad) * speed;
					Projectile.velocity = vectorToIdlePosition;
					chosenDirection = true;
				}
			}
			if (Projectile.ai[0] > 600f)
			{
				Projectile.Kill();
			}
			Projectile.ai[0] += 1f;
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Challengers/LifeChallengerParts/ShardGem{rGem}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Purple * Projectile.Opacity * 0.5f;
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
