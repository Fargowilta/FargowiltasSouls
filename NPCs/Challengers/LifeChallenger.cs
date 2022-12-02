using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.Chat;
using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Projectiles.Challengers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace FargowiltasSouls.NPCs.Challengers
{

	[AutoloadBossHead]
	public class LifeChallenger : ModNPC
	{
		private int state;

		private int timer;

		private bool talk = true;

		private bool EnterDialogue;

		private bool LoseDialogue;

		private bool TooFar;

		private int SnakeCount;

		private int CumOceanCount;

		private int soul;

		private bool firsttime1 = true;

		private int dustcounter;

		private int oldstate;

		private int statecount = 6;

		private bool initialtp = false;

		private int npcIndex = -1;

		private float SPR = 0.7f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(Language.GetTextValue("Lifelight"));
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

		}
		public override void SetDefaults()
		{
			NPC.aiStyle = -1;
            NPC.lifeMax = 2000;
            NPC.damage = 55;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
			NPC.width = 150;
			NPC.height = 150;
			NPC.boss = true;
			NPC.lavaImmune = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;

            Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1; //Zone
            NPC.value = Item.buyPrice(0, 0, 0, 1);
        }

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(state);
			writer.Write(oldstate);
			writer.Write(SnakeCount);
			writer.Write(CumOceanCount);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			state = reader.ReadInt32();
			oldstate = reader.ReadInt32();
			SnakeCount = reader.ReadInt32();
			CumOceanCount = reader.ReadInt32();
		}

		public override void AI()
		{
			Player Player = Main.player[NPC.target];
			Main.time = 27000; //noon
			Main.dayTime = true;
			NPC.rotation += (float)((Math.PI / 30f) / SPR); //divide by sec/rotation);
            npcIndex = -1;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i] == NPC)
				{
					npcIndex = i;
				}
			}
			if (initialtp == false)
			{
				NPC.position.X = Player.Center.X - (NPC.width / 2);
				NPC.position.Y = Player.Center.Y - 400 - (NPC.height / 2);
				initialtp = true;
			}
			bool expertmode = Main.expertMode;
		
			if (dustcounter > 5)
			{
				for (int j = 0; j < 180; j++)
				{
					double rad = 2.0 * (double)j * (Math.PI / 180.0);
					double dustdist = 1200.0;
					int DustX = (int)NPC.Center.X - (int)(Math.Cos(rad) * dustdist);
					int DustY = (int)NPC.Center.Y - (int)(Math.Sin(rad) * dustdist);
					Dust.NewDust(new Vector2(DustX, DustY), 1, 1, DustID.GemTopaz);
				}
				dustcounter = 0;
			}
			dustcounter++;
			if (!EnterDialogue)
			{
				switch (Main.hardMode)
				{
				case true:
					UtterWordsRed("");
					break;
				case false:
					UtterWordsRed("");
					break;
				}
				EnterDialogue = true;
			}
			if (!LoseDialogue && Player.dead)
			{
				UtterWordsRed("");
				LoseDialogue = true;
			}
			Player allPlayers = Main.player[Main.myPlayer];
			float distance = NPC.Distance(allPlayers.Center);
			if (distance > 1200f && !TooFar)
			{
				if (!firsttime1 && !Player.dead)
				{
					UtterWordsRed("");
				}
				TooFar = true;
				firsttime1 = false;
			}
			if (distance > 1200f && distance < 3000f)
			{
				allPlayers.AddBuff(ModContent.BuffType<FadingSoul>(), 10);
			}
			if (TooFar && distance < 1200f)
			{
				TooFar = false;
			}
			if (!Player.active || Player.dead)
			{
				Retarget(false);
				Player = Main.player[NPC.target];
				if (!Player.active || Player.dead)
				{
					NPC.velocity = new Vector2(0f, 1000f);
					if (NPC.timeLeft > 10)
					{
						NPC.timeLeft = 10;
					}
					return;
				}
			}
			if (NPC.ai[0] == 0f)
			{
				if (NPC.ai[1] == 30f)
				{
					Retarget(true);
				}
				if (NPC.ai[1] >= 60f)
				{
					NPC.ai[1] = 0f;
					NPC.ai[0] = 1f;
				}
			}
			if (NPC.ai[0] == 1f)
			{
				if (state == oldstate) //ensure you never get the same attack twice
                {
					RandomizeState();
				}
				if (state != oldstate)
				{
					switch (state)
					{
					case 0: //can't bother to split attacks into their own methods there's not many of them anyway
						if (talk)
						{
							UtterWordsWhite("");
							talk = false;
							NPC.netUpdate = true;
						}
						if (NPC.ai[1] > 60f && (NPC.ai[2] >= 20f || (NPC.ai[2] >= 5f && Main.hardMode)))
						{
							SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								float ProjectileSpeed2 = 16f;
								float knockBack2 = 3f;
								Vector2 shootatPlayer2 = NPC.DirectionTo(Player.Center) * ProjectileSpeed2;
								Vector2 shootoffset1 = shootatPlayer2.RotatedBy(-Math.PI / 5.0);
								Vector2 shootoffset2 = shootatPlayer2.RotatedBy(Math.PI / 5.0);
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootatPlayer2, ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset1, ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootoffset2, ModContent.ProjectileType<LifeSplittingProjSmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack2, Main.myPlayer);
								NPC.ai[2] = 0f;
							}
						}
						if (NPC.ai[1] >= 240f)
						{
							oldstate = state;
							StateReset();
						}
						break;
					case 1:
						RandomizeState();
						break;
					case 2:
						if (talk)
						{
							UtterWordsWhite("");
							talk = false;
						}
						NPC.netUpdate = true;
						if (NPC.ai[1] == 70f)
						{
							SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								float ProjectileSpeed3 = 12f;
								float knockBack3 = 300f;
								Vector2 shootatPlayer3 = NPC.DirectionTo(Player.Center) * ProjectileSpeed3;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootatPlayer3, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack3, Main.myPlayer);
							}
						}
						if (NPC.ai[1] >= 145f)
						{
							oldstate = state;
							StateReset();
						}
						break;
					case 3:
						if (talk)
						{
							UtterWordsWhite("");
							talk = false;
						}
						if (NPC.ai[1] == 70f)
						{
                            SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
							for (int i = 0; i < 5; i++)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									float bigSpeed = Main.rand.Next(25, 95);
									float knockBack4 = 3f;
									double rotationrad = MathHelper.ToRadians(Main.rand.Next(-40, 40));
									Vector2 shootrandom = (NPC.DirectionTo(Player.Center) * (bigSpeed / 6f)).RotatedBy(rotationrad);
									Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootrandom, ModContent.ProjectileType<LifeBomb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack4, Main.myPlayer);
								}
							}
							NPC.netUpdate = true;
						}
						if (NPC.ai[1] >= 85f)
						{
							oldstate = state;
							StateReset();
						}
						break;
					case 4:
						if (talk)
						{
							UtterWordsWhite("");
							talk = false;
						}
						if (NPC.ai[2] >= 10f && NPC.ai[1] < 130f)
						{
							SoundEngine.PlaySound(SoundID.NPCDeath7, NPC.Center);
							int SoulX = (int)NPC.position.X + NPC.width / 2 - (200 - soul * 50);
							int SoulY = (int)NPC.position.Y + NPC.height / 2 - 50;
							Vector2 SoulV = new Vector2(SoulX, SoulY);
							int SoulX2 = (int)NPC.position.X + NPC.width / 2 - (200 - soul * 50);
							int SoulY2 = (int)NPC.position.Y + NPC.height / 2 + 75;
							Vector2 SoulV2 = new Vector2(SoulX2, SoulY2);
							float knockBack5 = 3f;
							if (Main.hardMode)
							{
								SoulX = (int)NPC.position.X + NPC.width / 2 - (200 - soul * 50);
								SoulY = (int)NPC.position.Y + NPC.height / 2 - 50;
							}
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Vector2 soul1 = new Vector2(0f, -5f);
								Projectile.NewProjectile(NPC.GetSource_FromThis(), SoulV, soul1, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack5, Main.myPlayer, 0, npcIndex);
							}
							if (Main.hardMode)
							{
								SoundEngine.PlaySound(SoundID.Item8, SoulV2);
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Vector2 soul2 = new Vector2(0f, 5f);
									Projectile.NewProjectile(NPC.GetSource_FromThis(), SoulV2, soul2, ModContent.ProjectileType<LifeHomingProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack5, Main.myPlayer, 0, npcIndex);
								}
							}
							soul++;
							NPC.ai[2] = 0f;
							NPC.netUpdate = true;
						}
						if (NPC.ai[1] >= 200f)
						{
							soul = 0;
							oldstate = state;
							StateReset();
						}
						break;
					case 5:
						if (CumOceanCount >= 3)
						{
							RandomizeState();
						}
						if (talk && CumOceanCount < 3)
						{
							UtterWordsWhite("");
							NPC.defense = 200;
							NPC.netUpdate = true;
							talk = false;
						}
						if (NPC.ai[2] >= 45f && NPC.ai[1] <= 140f)
						{
							SoundEngine.PlaySound(SoundID.Item84, NPC.Center);
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								for (int k = 0; k < 45; k++)
								{
									float knockBack6 = 3f;
									double rotationrad2 = MathHelper.ToRadians(4 * k - 90);
									Vector2 shootrandom2 = (NPC.DirectionTo(Player.Center) * 0.8f).RotatedBy(rotationrad2);
									Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, shootrandom2, ModContent.ProjectileType<LifeWave>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack6, Main.myPlayer);
								}
								NPC.ai[2] = 0f;
							}
							NPC.netUpdate = true;
						}
						if (NPC.ai[1] >= 180f)
						{
							NPC.defense = 0;
							NPC.netUpdate = true;
							CumOceanCount++;
							oldstate = state;
							StateReset();
						}
						break;
					}
				}
			}
			if (Main.hardMode)
			{
				if (NPC.ai[3] > 600f)
				{
					SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float ProjectileSpeed = 8f;
						float knockBack = 300f;
						Vector2 shootatPlayer = NPC.DirectionTo(Player.Center) * ProjectileSpeed;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero - shootatPlayer, ModContent.ProjectileType<LifeNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), knockBack, Main.myPlayer);
						NPC.ai[3] = 0f;
					}
					NPC.netUpdate = true;
				}
				NPC.ai[3] += 1f;
			}
			NPC.ai[1] += 1f;
			NPC.ai[2] += 1f;
		}
        #region Help Functions
        public void UtterWordsWhite(string text) //deactivated unless we the boss to say something
        {
		/*
			if (Main.netMode == 0)
			{
				Main.NewText(Language.GetTextValue(text), Color.Gray);
			}
			else if (Main.netMode == 2)
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), Color.Gray);
			}
			CombatText.NewText(NPC.Hitbox, Color.Gray, Language.GetTextValue(text), dramatic: true);
		*/
		}

		public void UtterWordsRed(string text) //deactivated unless we the boss to say something
        {
		/*
			if (Main.netMode == 0)
			{
				Main.NewText(Language.GetTextValue(text), Color.Red);
			}
			else if (Main.netMode == 2)
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), Color.Red);
			}
			CombatText.NewText(NPC.Hitbox, Color.Red, Language.GetTextValue(text), dramatic: true);
		*/
		}

		public void StateReset()
		{
			NPC.ai[0] = 0f;
			NPC.ai[1] = 0f;
			NPC.ai[2] = 0f;
			talk = true;
			NPC.netUpdate = true;
		}

		public void Retarget(bool faTa)
		{
			Player oldtarget = Main.player[NPC.target];
			NPC.TargetClosest(faceTarget: faTa);
			if (oldtarget != Main.player[NPC.target])
			{
				Projectile.NewProjectile(NPC.GetSource_FromThis(), Main.player[NPC.target].Center, Vector2.Zero, ModContent.ProjectileType<TargetCrosshair>(), 0, 0, Main.myPlayer, 0f, npcIndex);
			}
		}

		public void RandomizeState()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				state = Main.rand.Next(statecount);
			}
			NPC.netUpdate = true;
		}
		#endregion

		#region Overrides
		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 5;
            return true;
        }
        public override bool CheckDead()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				//if (Main.hardMode)
				//{
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.position.Y + NPC.height, ModContent.NPCType<LifeChallenger2>(), NPC.whoAmI);
				//}
				//else
				//{
					//UtterWordsRed("Hands off the cum stash, bubs. (Return in Hardmode)");
				//}
			}
			return true;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life > 0)
			{
				return;
			}
			for (int i = 0; i < 25; i++)
			{
				Vector2 gorepos = new Vector2(NPC.position.X + (float)Main.rand.Next(NPC.width), NPC.position.Y + (float)Main.rand.Next(NPC.height));
				switch (Main.rand.Next(3))
				{
				case 0:
					Gore.NewGore(NPC.GetSource_Death(), gorepos, NPC.velocity, 825, 2f);
					break;
				case 1:
					Gore.NewGore(NPC.GetSource_Death(), gorepos, NPC.velocity, 825, 2f);
					break;
				case 2:
					Gore.NewGore(NPC.GetSource_Death(), gorepos, NPC.velocity, 826, 2f);
					break;
				}
			}
		}


        public override bool PreKill()
		{
			if (Main.hardMode)
			{
				return false;
			}
			return true;
		}
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D wingtexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Challengers/LifeChallenger_Wings", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            int currentFrame = NPC.frame.Y / (bodytexture.Height / Main.npcFrameCount[NPC.type]);
            int wingHeight = wingtexture.Height / Main.npcFrameCount[NPC.type];
            Rectangle wingRectangle = new Rectangle(0, currentFrame * wingHeight, wingtexture.Width, wingHeight);
            Vector2 wingOrigin = new Vector2(wingtexture.Width / 2, wingtexture.Height / 2 / Main.npcFrameCount[NPC.type]);

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 value4 = NPC.oldPos[i];
                double fpf = (int)(60 / Main.npcFrameCount[NPC.type] * SPR); //multiply by sec/rotation)
                int oldFrame = (int)((NPC.frameCounter - i) / fpf);
                Rectangle oldWingRectangle = new Rectangle(0, oldFrame * wingHeight, wingtexture.Width, wingHeight);
                DrawData wingTrailGlow = new DrawData(wingtexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldWingRectangle), drawColor * (0.5f / i), 0, wingOrigin, NPC.scale, SpriteEffects.None, 0);
                GameShaders.Misc["LCWingShader"].UseColor(Color.HotPink).UseSecondaryColor(Color.HotPink);
                GameShaders.Misc["LCWingShader"].Apply(wingTrailGlow);
                wingTrailGlow.Draw(spriteBatch);
            }

            spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: drawColor, rotation: NPC.rotation, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
            spriteBatch.Draw(origin: wingOrigin, texture: wingtexture, position: drawPos, sourceRectangle: wingRectangle, color: drawColor, rotation: 0, scale: NPC.scale, effects: SpriteEffects.None, layerDepth: 0f);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            Texture2D star = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rect = new Rectangle(0, 0, star.Width, star.Height);
            float scale = 0.3f * Main.rand.NextFloat(1f, 2.5f);
            Vector2 origin = new Vector2((star.Width / 2) + scale, (star.Height / 2) + scale);

            spriteBatch.Draw(star, NPC.Center - screenPos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
            DrawData starDraw = new DrawData(star, NPC.Center - screenPos, new Rectangle?(rect), Color.HotPink, 0, origin, scale, SpriteEffects.None, 0);
            GameShaders.Misc["LCWingShader"].UseColor(Color.Goldenrod).UseSecondaryColor(Color.HotPink);
            GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
            starDraw.Draw(spriteBatch);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }
        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            //circular hitbox-inator
            Vector2 ellipseDim = NPC.Size;
            Vector2 ellipseCenter = NPC.position + 0.5f * new Vector2(NPC.width, NPC.height);
            Vector2 boxPos = target.position;
            Vector2 boxDim = target.Size;
            float x = 0f; //ellipse center
            float y = 0f; //ellipse center
            if (boxPos.X > ellipseCenter.X)
            {
                x = boxPos.X - ellipseCenter.X; //left corner
            }
            else if (boxPos.X + boxDim.X < ellipseCenter.X)
            {
                x = boxPos.X + boxDim.X - ellipseCenter.X; //right corner
            }
            if (boxPos.Y > ellipseCenter.Y)
            {
                y = boxPos.Y - ellipseCenter.Y; //top corner
            }
            else if (boxPos.Y + boxDim.Y < ellipseCenter.Y)
            {
                y = boxPos.Y + boxDim.Y - ellipseCenter.Y; //bottom corner
            }
            float a = ellipseDim.X / 2f;
            float b = ellipseDim.Y / 2f;
            return (x * x) / (a * a) + (y * y) / (b * b) < 1; //point collision detection
        }

        public override void FindFrame(int frameHeight)
        {
			double fpf = (int)(60/ Main.npcFrameCount[NPC.type] * SPR); //multiply by sec/rotation)
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter += 1;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type] * fpf;
            NPC.frame.Y = frameHeight * (int)(NPC.frameCounter / fpf);
        }
		#endregion
	}
}
