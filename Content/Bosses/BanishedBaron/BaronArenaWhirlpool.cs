using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{

	public class BaronArenaWhirlpool : ModProjectile
	{
		public override string Texture => "FargowiltasSouls/Content/Bosses/BanishedBaron/BaronWhirlpool";
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Banished Baron's Mine");
			Main.projFrames[Type] = 16;
			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
		}
		public override void SetDefaults()
		{
			Projectile.width = 186;
			Projectile.height = 48;
			Projectile.aiStyle = -1;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.alpha = 255;
			Projectile.timeLeft = 60 * 60 * 60;
			Projectile.FargoSouls().DeletionImmuneRank = 2;
		}
		private static int BaseMaxDistance = 1000;
		private int WaterwallDistance = 0;
		public override void AI()
		{
			//ref float ParentID = ref Projectile.ai[0];
			ref float Timer = ref Projectile.ai[1];
			ref float State = ref Projectile.ai[2];

			int ParentID = NPC.FindFirstNPC(ModContent.NPCType<BanishedBaron>());
			if (!ParentID.IsWithinBounds(Main.maxNPCs))
			{
				Projectile.Kill();
				return;
			}

			NPC baron = Main.npc[ParentID];
			if (baron == null || !baron.active || baron.type != ModContent.NPCType<BanishedBaron>())
			{
				Projectile.Kill();
				return;
			}

			Projectile.timeLeft = 60 * 60 * 60;

			float p2MaxLife = baron.lifeMax / 2;
			float modifier = 1f - ((float)baron.life / p2MaxLife);
			float distanceDecrease = 400 * modifier;
			float MaxDistance = (float)BaseMaxDistance - distanceDecrease;
			if (WorldSavingSystem.masochistModeReal)
			{
				MaxDistance -= 300;
			}

			Player player = baron.target.IsWithinBounds(Main.maxPlayers) ? Main.player[baron.target] : null;

			if (player != null && player.active && !player.dead && !player.ghost)
			{
				if (Timer == 0) //done this way to work with world borders
				{

					const int WorldEdgeExtraWidth = 200; //extra width for projectile attack to be visible
					int worldSide = Math.Sign(Main.maxTilesX * 8 - Projectile.Center.X); //half world width minus pos of this
					Projectile.Center = Main.screenPosition + new Vector2(WorldEdgeExtraWidth * worldSide + Main.screenWidth / 2, Main.screenHeight / 2); //not player center to work with map borders

				}
				Projectile.Center = Projectile.Center.X * Vector2.UnitX + player.Center.Y * Vector2.UnitY;

				int wallAttackTime = WorldSavingSystem.MasochistModeReal ? 55 : WorldSavingSystem.EternityMode ? 70 : 80;
				if (State == 1 && Timer % wallAttackTime == 0)
				{
					if (FargoSoulsUtil.HostCheck)
					{
						int side = LumUtils.NonZeroSign(player.Center.X - Projectile.Center.X);
						if (WorldSavingSystem.MasochistModeReal) //sides alternate in maso
						{
							side = Timer % (wallAttackTime * 2) == 0 ? 1 : -1;
						}
						FireBolts(player, side);
					}

				}
			}
			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 3;
			}
			else
			{
				Projectile.alpha = 0;
			}
			if (++Projectile.frameCounter > 2)
			{
				if (++Projectile.frame >= Main.projFrames[Type])
				{
					Projectile.frame = 0;
				}
				Projectile.frameCounter = 0;
			}
			if (WaterwallDistance < MaxDistance)
			{
				WaterwallDistance += 10;
			}
			if (WaterwallDistance > MaxDistance + 5)
			{
				WaterwallDistance -= 5;
			}
			//WaterwallDistance += Math.Min(5, Math.Abs((int)MaxDistance - WaterwallDistance)) * Math.Sign((int)MaxDistance - WaterwallDistance);

			WaterWalls(Projectile.Center, WaterwallDistance);
			Timer++;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			Rectangle textureSize = new(0, 0, texture.Width, num156);


			SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Color color = Projectile.GetAlpha(lightColor);
			for (int Side = -1; Side < 2; Side += 2)
			{
				for (int i = -50; i <= 20; i++)
				{
					Vector2 pos = Projectile.Center.X * Vector2.UnitX + Main.LocalPlayer.Center.Y * Vector2.UnitY; //draw locally so you always see the wall on your screen
					Vector2 center = pos + Vector2.UnitX * Side * (WaterwallDistance + (textureSize.Width / 2)) + Vector2.UnitY * i * Projectile.height;
					center.Y = (float)Math.Floor(center.Y / textureSize.Height) * textureSize.Height; //makes them not smoothly move up and down, but jump one chunk at a time

					int num = (int)(center.Y / textureSize.Height);
					int frame = (Projectile.frame + num) % Main.projFrames[Projectile.type];
					int y3 = num156 * frame; //ypos of upper left corner of sprite to draw
					Rectangle rectangle = new(0, y3, texture.Width, num156);
					Vector2 origin2 = rectangle.Size() / 2f;


					if (Collision.SolidCollision(center - rectangle.Size() / 2, rectangle.Width, rectangle.Height))
					{
						continue;
					}

					Main.EntitySpriteDraw(texture, center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), color, Projectile.rotation, origin2, Projectile.scale, effects, 0);
				}
			}
			return false;
		}
		//private int nearestMultiple(int value, int factor) { return (int)Math.Round((value / (double)factor), MidpointRounding.AwayFromZero) * factor; }

		private void WaterWalls(Vector2 location, int threshold)
		{
			//Aura
			if (true)
			{
				Player player = Main.LocalPlayer;
				bool OutsideRange = Math.Abs(player.Center.X - location.X) >= threshold;
				bool OutsideRangex2 = Math.Abs(player.Center.X - location.X) >= threshold * 2;
				bool OutsideRangex4 = Math.Abs(player.Center.X - location.X) >= threshold * 4;
				if (player.active && !player.dead && !player.ghost) //pull into arena
				{
					if (OutsideRange && !OutsideRangex4)
					{
						if (OutsideRangex2)
						{
							player.controlLeft = false;
							player.controlRight = false;
							player.controlUp = false;
							player.controlDown = false;
							player.controlUseItem = false;
							player.controlUseTile = false;
							player.controlJump = false;
							player.controlHook = false;
							if (player.grapCount > 0)
								player.RemoveAllGrapplingHooks();
							if (player.mount.Active)
								player.mount.Dismount(player);
							player.velocity.X = 0f;
							//player.velocity.Y = -0.4f;
							player.FargoSouls().NoUsingItems = 2;
						}

						Vector2 movement = new(location.X - player.Center.X, 0);
						float difference = movement.Length() - threshold;
						movement.Normalize();
						movement *= difference < 17f ? difference : 17f;
						player.position += movement;
						player.AddBuff(BuffID.Bleeding, 120);

						//remove outwards velocity
						int playerDir = Math.Sign(player.Center.X - location.X);
						if (Math.Sign(player.velocity.X) != 0 && Math.Sign(player.velocity.X) != playerDir)
						{
							player.velocity.X = 0;
						}

						for (int i = 0; i < 10; i++)
						{
							int DustType = DustID.WaterCandle;
							int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default(Color), 1.25f);
							Main.dust[d].noGravity = true;
							Main.dust[d].velocity *= 5f;
						}
					}
				}
			}
		}

		private void FireBolts(Player player, int side)
		{

			for (int i = -10; i <= 10; i++)
			{
				const int speed = 7;
				const float distancePos = 3.2f;
				const float randomPos = 9;
				const float maxRot = MathHelper.Pi / 28;
				float posX = Projectile.Center.X + (side * (Projectile.width * 0.8f + WaterwallDistance));
				float posY = player.Center.Y + (i * Projectile.height * distancePos) + Main.rand.NextFloat(-randomPos, randomPos);
				Vector2 pos = posX * Vector2.UnitX + posY * Vector2.UnitY;
				Vector2 vel = (Vector2.UnitX * side).RotatedBy(Main.rand.NextFloat(-maxRot, maxRot)) * speed;
				Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), pos, vel, ModContent.ProjectileType<BaronWhirlpoolBolt>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 2, -side);
			}
			SoundEngine.PlaySound(SoundID.Item21, Projectile.Center + (Vector2.UnitX * side * (Projectile.width * 0.8f + WaterwallDistance)));
		}
	}
}