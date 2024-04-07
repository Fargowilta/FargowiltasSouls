using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.CursedCoffin
{
	public class CoffinSlamShockwave : ModProjectile
	{
		//public override string Texture => FargoSoulsUtil.EmptyTexture;
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 70;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.light = 1;
			Projectile.timeLeft = 60 * 3;
		}

		public override void AI()
		{

			int frameCounterMax = (int)Math.Round(12 - MathHelper.Clamp(6 * Projectile.velocity.X / 60f, 0, 6));
			Projectile.Animate(frameCounterMax);

			if (Math.Abs(Projectile.velocity.X) < 15)
				Projectile.velocity.X *= 1.035f;

			int p = Player.FindClosest(Projectile.Center, 0, 0);
			if (p.IsWithinBounds(Main.maxPlayers) && Main.player[p] is Player player && player.Alive())
			{
				// scaling light amount based on distance
				float distance = Math.Abs(player.Center.X - Projectile.Center.X);
				Projectile.light = distance < 500 ? (500 - distance) / 500 : 0;
			}

			// lock on block grid
			Projectile.position.Y = (MathF.Floor((Projectile.position.Y + Projectile.height) / 16) * 16) - Projectile.height;

			int i = 0;
			const int maxIter = 10;
			do
			{
				i++;
				Point tilePos = Projectile.Bottom.ToTileCoordinates();
				Tile tile = Main.tile[tilePos.X, tilePos.Y - 1];
				Tile tileBelow = Main.tile[tilePos.X, tilePos.Y];
				bool tileSolid = tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
				bool tileBelowSolid = tileBelow.HasUnactuatedTile && (Main.tileSolid[tileBelow.TileType] || Main.tileSolidTop[tileBelow.TileType]);
				if (tileBelowSolid && !tileSolid)
					break;
				if (tileSolid)
					Projectile.Center -= Vector2.UnitY * 16;
				else
					Projectile.Center += Vector2.UnitY * 16;
			}
			while (i < maxIter);

			if (i >= maxIter - 1)
				Projectile.Kill();
			/*
            for (int j = 0; j < 5; j++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X / 2, Main.rand.NextFloat(-5, 5), Scale: Main.rand.NextFloat(1, 3));
            }
            */
		}
		public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
		{
			modifiers.Null();
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<StunnedBuff>(), 60 * 2);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			float rotation = Projectile.rotation;
			Vector2 drawPos = Projectile.Center;
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 scale = Vector2.UnitX * (Projectile.scale / 2 + (Math.Abs(Projectile.velocity.X) / 13)) + Vector2.UnitY * Projectile.scale;

			int sizeY = texture.Height / Main.projFrames[Type]; //ypos of lower right corner of sprite to draw
			int frameY = Projectile.frame * sizeY;
			Rectangle rectangle = new(0, frameY, texture.Width, sizeY);
			Vector2 origin = rectangle.Size() / 2f;
			SpriteEffects spriteEffects = Projectile.velocity.X >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
			{
				Color oldColor = lightColor;
				oldColor *= 0.5f;
				oldColor *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
				Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2;
				float oldRot = Projectile.oldRot[i];
				Main.EntitySpriteDraw(texture, oldPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(oldColor),
					oldRot, origin, scale, spriteEffects, 0);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor),
					rotation, origin, scale, spriteEffects, 0);

			return false;
		}
	}
}
