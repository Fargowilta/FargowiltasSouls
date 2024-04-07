using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
	public class MinosPrime : ModProjectile
	{
		public ref float Timer => ref Projectile.ai[0];

		private Vector2 mousePos;
		private int syncTimer;
		private float Wobble;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lunar Cultist");
			//Main.projFrames[Projectile.type] = 12;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			//ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 84;
			Projectile.height = 98;
			Projectile.timeLeft *= 5;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;


			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(mousePos.X);
			writer.Write(mousePos.Y);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Vector2 buffer;
			buffer.X = reader.ReadSingle();
			buffer.Y = reader.ReadSingle();
			if (Projectile.owner != Main.myPlayer)
			{
				mousePos = buffer;
			}
		}
		public override bool? CanHitNPC(NPC target) => false; //no contact damage
		public override void AI()
		{

			Player player = Main.player[Projectile.owner];
			if (player.active && !player.dead && player.HasEffect<PrimeSoulEffect>())
				Projectile.timeLeft = 2;

			SyncMouse(player);
			Wobble = 20f * (float)Math.Sin(MathHelper.TwoPi * (Timer / 60f));
			Movement(player);
		}
		public void SyncMouse(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				mousePos = Main.MouseWorld;

				if (++syncTimer > 20)
				{
					syncTimer = 0;
					Projectile.netUpdate = true;
				}
			}
		}
		public void Movement(Player player)
		{
			int offset = Math.Sign(mousePos.X - player.Center.X);
			Projectile.spriteDirection = Projectile.direction = -offset;
			offset *= 225;
			Vector2 idlePosition = mousePos + Vector2.UnitX * offset;
			Vector2 toIdlePosition = idlePosition - Projectile.Center;
			float distance = toIdlePosition.Length();
			float speed = 38f;
			float inertia = 15f;
			toIdlePosition.Normalize();
			toIdlePosition *= speed;
			Projectile.velocity = (Projectile.velocity * (inertia - 1f) + toIdlePosition) / inertia;
			if (distance == 0)
				Projectile.velocity = Vector2.Zero;
			if (distance < Projectile.velocity.Length())
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * distance;
			if (Projectile.velocity == Vector2.Zero && distance > 10)
			{
				Projectile.velocity.X = -0.15f;
				Projectile.velocity.Y = -0.05f;
			}
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;

			Color color = lightColor;
			Player player = Main.player[Projectile.owner];
			if (player.Alive())
			{
				Color? soulColor = GetColor(player);
				if (soulColor.HasValue)
					color = soulColor.Value;
			}


			//Color color26 = lightColor;
			//color26 = Projectile.GetAlpha(color26);

			//Texture2D texture2D14 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Minions/LunarCultistTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
			{
				Color color2 = color;
				color2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
				Vector2 value4 = Projectile.oldPos[i];
				float num165 = Projectile.oldRot[i];
				Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Wobble + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, num165, origin2, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + Vector2.UnitY * Wobble + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.rotation, origin2, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			return false;
		}
		public int ColorTimer = 0;
		public Color? GetColor(Player player)
		{
			ColorTimer++;
			List<Color> colors = new();

			FargoSoulsPlayer modPlayer = player.FargoSouls();

			if (modPlayer.MeleeSoul)
				colors.Add(BerserkerSoul.ItemColor);
			if (modPlayer.RangedSoul)
				colors.Add(SnipersSoul.ItemColor);
			if (modPlayer.MagicSoul)
				colors.Add(ArchWizardsSoul.ItemColor);
			if (modPlayer.SummonSoul)
				colors.Add(ConjuristsSoul.ItemColor);
			if (modPlayer.WorldShaperSoul)
				colors.Add(WorldShaperSoul.ItemColor);
			if (modPlayer.MasochistSoul)
				colors.Add(MasochistSoul.ItemColor);
			if (modPlayer.SupersonicSoul)
				colors.Add(SupersonicSoul.ItemColor);
			if (modPlayer.ColossusSoul)
				colors.Add(ColossusSoul.ItemColor);
			if (modPlayer.FlightMasterySoul)
				colors.Add(FlightMasterySoul.ItemColor);
			if (modPlayer.FishSoul2)
				colors.Add(TrawlerSoul.ItemColor);

			int colorAmt = colors.Count;
			if (colorAmt == 0)
				return null;

			const float ColorTime = 300f;

			if (ColorTimer >= ColorTime * colorAmt)
				ColorTimer = 1;

			float lerp = (ColorTimer % ColorTime) / ColorTime; // Current lerp progress between colors

			int i = (int)Math.Floor(ColorTimer / ColorTime); // Current soul color being lerped from
			if (colorAmt <= 1)
				return colors[i];
			int j = i + 1; // Current soul color being lerped to
			if (j >= colorAmt)
				j = 0;

			Color color = Color.Lerp(colors[i], colors[j], lerp);

			return color;
		}
	}
}
