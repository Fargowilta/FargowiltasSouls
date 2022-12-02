using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeSplittingProjSmall : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Splitting Life Shot");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
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
			//Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 91, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 0.25f);
			Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;

            if (Projectile.ai[0] == 45f)
			{
				int damage = Projectile.damage;
				float knockBack = 3f;
				Vector2 shootoffset1 = Projectile.velocity.RotatedBy(-Math.PI / 3.0);
				Vector2 shootoffset2 = Projectile.velocity.RotatedBy(Math.PI / 3.0);
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset1, ModContent.ProjectileType<LifeProjSmall>(), damage, knockBack, Main.myPlayer);
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootoffset2, ModContent.ProjectileType<LifeProjSmall>(), damage, knockBack, Main.myPlayer);
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
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f * 0.5f;
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
