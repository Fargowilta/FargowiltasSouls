
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Items.Placables.Relics;
using FargowiltasSouls.Content.Items.Placables.Trophies;
using FargowiltasSouls.Content.Items.Summons;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Bosses.BanishedBaron
{
	[AutoloadBossHead]
	public class BanishedBaron : ModNPC
	{
		Player player => Main.player[NPC.target];

		public static readonly SoundStyle BaronRoar = new("FargowiltasSouls/Assets/Sounds/BaronRoar");
		public static readonly SoundStyle BaronYell = new("FargowiltasSouls/Assets/Sounds/BaronYell");

		#region Variables
		public enum StateEnum //ALL states
		{
			Opening,
			Phase2Transition,
			P1BigNuke,
			P1RocketStorm,
			P1SurfaceMines,
			P1FadeDash,
			P1SineSwim,
			P1Whirlpool,
			P2PredictiveDash,
			P2CarpetBomb,
			P2RocketStorm,
			P2SpinDash,
			P2MineFlurry,
			P2Whirlpool,
			P2LaserSweep,
			Swim,
			DeathAnimation
		}

		public static List<int> P1Attacks = new() //these are randomly chosen attacks in p1
        {
			(int)StateEnum.P1BigNuke,
			(int)StateEnum.P1RocketStorm,
			(int)StateEnum.P1SurfaceMines,
			(int)StateEnum.P1FadeDash,
			(int)StateEnum.P1SineSwim,
			(int)StateEnum.P1Whirlpool
		};
		public static List<int> P2Attacks = new() //these are randomly chosen attacks in p2
        {
			(int)StateEnum.P2PredictiveDash,
			(int)StateEnum.P2CarpetBomb,
			(int)StateEnum.P2RocketStorm,
			(int)StateEnum.P2SpinDash,
			(int)StateEnum.P2MineFlurry,
			(int)StateEnum.P2Whirlpool,
			(int)StateEnum.P2LaserSweep
		};

		public bool Attacking = true;
		public bool HitPlayer = true;
		public int Trail = 0;

		bool DidWhirlpool = false;

		public int Frame = 0;
		public int Phase = 1;
		public int Anim = 0;

		public int ArenaProjID = -1;

		public const int MaxWhirlpools = 40;

		public List<int> availablestates = new(0);

		public Vector2 LockVector1 = Vector2.Zero;

		public int LastState;
		public Vector2 LockVector2 = Vector2.Zero;

		//NPC.ai[] overrides
		public ref float Timer => ref NPC.ai[0];
		public ref float State => ref NPC.ai[1];
		public ref float AI2 => ref NPC.ai[2];
		public ref float AI3 => ref NPC.ai[3];
		public ref float AI4 => ref NPC.localAI[0];
		#endregion
		#region Standard
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 19;
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 3;
			NPCID.Sets.MPAllowedEnemies[Type] = true;

			NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

			NPC.AddDebuffImmunities(new List<int>
			{
				BuffID.Confused,
				BuffID.Chilled,
				BuffID.Suffocation,
				ModContent.BuffType<LethargicBuff>(),
				ModContent.BuffType<ClippedWingsBuff>()
			});
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Rotation = MathHelper.Pi,
				Position = Vector2.UnitX * 60
			});
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
			});
		}
		public override void SetDefaults()
		{
			NPC.aiStyle = -1;
			NPC.lifeMax = 32000;
			NPC.defense = 15;
			NPC.damage = 69;
			NPC.knockBackResist = 0f;
			NPC.width = 132; //194
			NPC.height = 132;
			NPC.boss = true;
			NPC.lavaImmune = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronDeath");
			NPC.alpha = 255;

			Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod) ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Baron") : MusicID.DukeFishron;
			SceneEffectPriority = SceneEffectPriority.BossLow;

			NPC.value = Item.buyPrice(0, 2);

		}
		public override Color? GetAlpha(Color drawColor)
		{
			if (NPC.IsABestiaryIconDummy)
			{
				// This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
				return NPC.GetBestiaryEntryColor();
			}
			return base.GetAlpha(drawColor);
		}
		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * balance);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(NPC.localAI[0]);
			writer.Write7BitEncodedInt(ArenaProjID);
			writer.Write7BitEncodedInt(Phase);
			writer.WriteVector2(LockVector1);
			writer.WriteVector2(LockVector2);
			writer.Write7BitEncodedInt(LastState);
			writer.Write(DidWhirlpool);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			NPC.localAI[0] = reader.ReadSingle();
			ArenaProjID = reader.Read7BitEncodedInt();
			Phase = reader.Read7BitEncodedInt();
			LockVector1 = reader.ReadVector2();
			LockVector2 = reader.ReadVector2();
			LastState = reader.Read7BitEncodedInt();
			DidWhirlpool = reader.ReadBoolean();
		}
		#endregion
		#region Overrides
		#region Hitbox
		public override bool CanHitPlayer(Player target, ref int CooldownSlot)
		{
			if (HitPlayer)
			{
				Vector2 boxPos = target.position;
				Vector2 boxDim = target.Size;
				return Collides(boxPos, boxDim);
			}
			return false;
		}
		public override bool CanHitNPC(NPC target)
		{
			if (HitPlayer)
			{
				Vector2 boxPos = target.position;
				Vector2 boxDim = target.Size;
				return Collides(boxPos, boxDim);
			}
			return false;
		}
		public bool Collides(Vector2 boxPos, Vector2 boxDim)
		{
			//circular hitbox-inator
			Vector2 ellipseDim = NPC.Size;
			Vector2 ellipseCenter = NPC.position + 0.5f * new Vector2(NPC.width, NPC.height);

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

			return x * x / (a * a) + y * y / (b * b) < 1; //point collision detection
		}
		#endregion
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => !(Timer < 90 + 50 && State == (int)StateEnum.P1FadeDash);

		public override void BossHeadSlot(ref int index)
		{
			if ((Timer < 90 + 50 && State == (int)StateEnum.P1FadeDash))
			{
				index = -1;
			}
			else
			{
				index = NPCID.Sets.BossHeadTextures[Type];
			}
		}
		public override bool? CanFallThroughPlatforms() => true;
		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Bleeding, 60 * 10);
			if (!WorldSavingSystem.EternityMode)
			{
				target.AddBuff(BuffID.Rabies, 60 * 5);
				return;
			}
			target.AddBuff(BuffID.Rabies, 60 * 10);
			target.FargoSouls().MaxLifeReduction += 50;
			target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 60 * 30);
		}
		public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
		{
			if (State == (int)StateEnum.Phase2Transition)
			{
				modifiers.FinalDamage /= 4;
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{

			Texture2D bodytexture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
			Vector2 drawPos = NPC.Center - screenPos;
			float rot = NPC.rotation + (NPC.direction == 1 ? 0 : MathHelper.Pi);
			if (NPC.aiStyle != -1 && NPC.ai[0] != 7)
			{
				Vector2 dir = NPC.DirectionTo(player.Center);
				NPC.direction = Math.Sign(dir.X);
				rot = dir.ToRotation() + (NPC.direction == 1 ? 0 : MathHelper.Pi);
			}

			int currentFrame = NPC.frame.Y / (bodytexture.Height / Main.npcFrameCount[NPC.type]);
			SpriteEffects flip = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


			for (int i = 0; i < Math.Min(Trail, NPCID.Sets.TrailCacheLength[NPC.type]); i++) //math.min to safeguard against uncached trail
			{
				float oldrot = NPC.oldRot[i] + (NPC.direction == 1 ? 0 : MathHelper.Pi);
				Vector2 value4 = NPC.oldPos[i];
				int oldFrame = Frame;
				Rectangle oldRectangle = new(0, oldFrame * bodytexture.Height / Main.npcFrameCount[NPC.type], bodytexture.Width, bodytexture.Height / Main.npcFrameCount[NPC.type]);
				DrawData oldGlow = new(bodytexture, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(oldRectangle), NPC.GetAlpha(drawColor) * (0.5f / i), oldrot, new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), NPC.scale, flip, 0);
				GameShaders.Misc["LCWingShader"].UseColor(Color.Blue).UseSecondaryColor(Color.Black);
				GameShaders.Misc["LCWingShader"].Apply(oldGlow);
				oldGlow.Draw(spriteBatch);
			}

			spriteBatch.Draw(origin: new Vector2(bodytexture.Width / 2, bodytexture.Height / 2 / Main.npcFrameCount[NPC.type]), texture: bodytexture, position: drawPos, sourceRectangle: NPC.frame, color: NPC.GetAlpha(drawColor), rotation: rot, scale: NPC.scale, effects: flip, layerDepth: 0f);
			return false;
		}

		float AnimationSpeed = 1f;
		public override void FindFrame(int frameHeight)
		{
			//FRAMES OF ANIMATIONS:
			//P1 Normal fly loop: 0-5 (Anim 0)
			//P1 Open mouth: 6-7 (Anim 1)
			//Unused: 8-10
			//P2 Normal fly loop: 11-16 (Anim 0)
			//P2 Open mouth: 17-18 (Anim 1)
			double fpf = 60 / (10 * AnimationSpeed); //  60/fps
			int StartFrame = 0;
			int EndFrame = 5;
			switch (Phase)
			{
				case 1:
					switch (Anim)
					{
						case 0:
							//no change required, default values
							break;
						case 1:
							StartFrame = 6;
							EndFrame = 7;
							break;
					}
					break;
				case 2:
					switch (Anim)
					{
						case 0:
							StartFrame = 11;
							EndFrame = 16;
							break;
						case 1:
							StartFrame = 17;
							EndFrame = 18;
							break;
					}
					break;
			}
			NPC.spriteDirection = NPC.direction;
			if (++NPC.frameCounter >= fpf)
			{
				if (++Frame > EndFrame || Frame < StartFrame)
				{
					Frame = StartFrame;
				}
				NPC.frameCounter = 0;
			}
			NPC.frame.Y = frameHeight * Frame;

			if (Anim == 3) //phase transition
			{
				NPC.frame.Y = frameHeight * 9;
			}
		}
		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref WorldSavingSystem.downedBoss[(int)WorldSavingSystem.Downed.BanishedBaron], -1);
		}
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life - hit.Damage < 0 && !NPC.dontTakeDamage)
			{
				hit.Null();
				NPC.life = 10;
				NPC.dontTakeDamage = true;
				State = (int)StateEnum.DeathAnimation;
				Timer = 0;
				NPC.velocity = Vector2.Zero;

				SoundEngine.PlaySound(SoundID.Item62 with { Pitch = 0.0f, Volume = 2f }, NPC.Center);

				Vector2 expos = NPC.position - NPC.Size / 2;
				int exwidth = NPC.width * 2;
				int exheight = NPC.height * 2;
				for (int i = 0; i < 30; i++)
				{
					int dust = Dust.NewDust(expos, exwidth,
						exheight, DustID.Smoke, 0f, 0f, 100, default, 3f);
					Main.dust[dust].velocity *= 1.4f;
				}

				for (int i = 0; i < 20; i++)
				{
					int dust = Dust.NewDust(expos, exwidth,
						exheight, DustID.Torch, 0f, 0f, 100, default, 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 7f;
					dust = Dust.NewDust(expos, exwidth,
						exheight, DustID.Torch, 0f, 0f, 100, default, 1.5f);
					Main.dust[dust].velocity *= 3f;
				}

				float scaleFactor9 = 0.5f;
				if (!Main.dedServ)
				{
					for (int j = 0; j < 4; j++)
					{
						int gore = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center,
							default,
							Main.rand.Next(61, 64));

						Main.gore[gore].velocity *= scaleFactor9;
						Main.gore[gore].velocity += new Vector2(1f, 1f).RotatedBy(MathHelper.TwoPi / 4 * j);
					}
				}

			}
			if (NPC.life <= 0)
			{
				Main.LocalPlayer.FargoSouls().Screenshake = 60;
				for (int i = 1; i <= 4; i++)
				{
					Vector2 pos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
					float vel = NPC.velocity.Length();
					float spd = Main.rand.NextFloat(vel * 1.25f, vel * 1.6f);
					if (!Main.dedServ)
						Gore.NewGore(NPC.GetSource_FromThis(), pos, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * spd, ModContent.Find<ModGore>(Mod.Name, $"BaronGore{i}").Type, NPC.scale);
				}

				for (int j = 0; j < 20; j++)
				{
					Vector2 pos = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
					Vector2 vel = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(16, 24);
					int type = Main.rand.Next(6) + 1;
					if (!Main.dedServ)
						Gore.NewGore(NPC.GetSource_FromThis(), pos, vel, ModContent.Find<ModGore>(Mod.Name, $"BaronScrapGore{type}").Type, NPC.scale);
				}

				if (FargoSoulsUtil.HostCheck)
				{
					NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.Goldfish);
				}
			}
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BanishedBaronBag>()));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BaronTrophy>(), 10));

			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BaronRelic>()));

			LeadingConditionRule rule = new(new Conditions.NotExpert());
			rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<TheBaronsTusk>(), ModContent.ItemType<RoseTintedVisor>(), ModContent.ItemType<NavalRustrifle>(), ModContent.ItemType<DecrepitAirstrikeRemote>()));
			rule.OnSuccess(ItemDropRule.Common(5003, 1, 1, 5)); //seaside crate
			rule.OnSuccess(ItemDropRule.OneFromOptions(3, ItemID.Sextant, ItemID.WeatherRadio, ItemID.FishermansGuide));
			rule.OnSuccess(ItemDropRule.Common(ItemID.FishingBobber, 4, 1, 1));
			rule.OnSuccess(ItemDropRule.Common(ItemID.FishingPotion, 3, 2, 5));
			rule.OnSuccess(ItemDropRule.Common(ItemID.SonarPotion, 2, 2, 5));
			rule.OnSuccess(ItemDropRule.Common(ItemID.CratePotion, 5, 2, 5));
			rule.OnSuccess(ItemDropRule.Common(ItemID.GoldenBugNet, 50, 1, 1));
			rule.OnSuccess(ItemDropRule.Common(ItemID.FishHook, 50, 1, 1));
			rule.OnSuccess(ItemDropRule.Common(ItemID.GoldenFishingRod, 150, 1, 1));
			npcLoot.Add(rule);
		}

		#endregion
		#region AI
		public override bool PreAI()
		{
			/* baron ex (fake)
            for (int i = 0; i < 3; i++)
            {
                AI();
                NPC.position += NPC.velocity;
            }
            */
			return base.PreAI();
		}
		public override void AI()
		{
			if (State == (int)StateEnum.DeathAnimation)
			{
				//un-duke your fishron
				NPC.aiStyle = -1;
			}
			else if (Main.getGoodWorld)
			{
				if (NPC.aiStyle == -1)
				{
					//ftw desperation: literally become fishron
					if (NPC.life < NPC.lifeMax * .15)
					{
						int heal = NPC.lifeMax / 2 - NPC.life;
						NPC.HealEffect(heal);
						NPC.life += heal;
						NPC.aiStyle = NPCAIStyleID.DukeFishron;
						NPC.velocity = Vector2.Zero;
						NPC.ai[0] = 0;
						NPC.ai[1] = 0;
						NPC.ai[2] = 0;
						NPC.ai[3] = 0;
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.localAI[2] = 0;
						NPC.localAI[3] = 0;
						NPC.netUpdate = true;

						Projectile arena = Main.projectile[ArenaProjID];
						bool arenaReal = arena != null && arena.active && arena.type == ModContent.ProjectileType<BaronArenaWhirlpool>();
						if (arenaReal)
						{
							arena.ai[2] = 0; //deactivate projectile shooting AI of arena
							arena.netUpdate = true;
						}
					}
				}
				else
				{
					return;
				}
			}

			//Defaults
			NPC.noTileCollide =
				Phase == 2 ||
				WorldSavingSystem.MasochistModeReal ||
				Collision.SolidCollision(NPC.position + NPC.Size / 10, (int)(NPC.width * 0.9f), (int)(NPC.height * 0.9f)) ||
				!Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
			//no tile collide in p2, tile collide in p1 except if in tiles, or if obstructed

			if (Phase == 1 && NPC.Center.Y < player.Center.Y && !(Collision.WetCollision(NPC.position, NPC.width, NPC.height) || Collision.SolidCollision(NPC.position, NPC.width, NPC.height)))
			{
				NPC.position.Y += 4f;
			}
			NPC.defense = NPC.defDefense;
			NPC.direction = NPC.spriteDirection = NPC.rotation.ToRotationVector2().X > 0 ? 1 : -1;

			if ((HitPlayer || State == (float)StateEnum.Phase2Transition) && !(Timer < 90 && State == (int)StateEnum.P1FadeDash))
			{
				Lighting.AddLight(NPC.Center, TorchID.Pink);
			}

			if (!AliveCheck(player))
				return;

			if (State == 0 && Timer == 0) //opening
			{
				NPC.position = player.Center + new Vector2(Math.Sign(player.Center.X - Main.spawnTileX) * 1400, -100) - NPC.Size / 2;
				NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
				NPC.velocity = Vector2.Zero;
			}
			//Phase 1/2 specific passive attributes
			switch (Phase)
			{
				case 1:
					if (State != (float)StateEnum.Opening || Timer >= 60) //also do it during opening but only after 1 sec
					{
						//inflict all nearby permanently with Please Go Underwater debuff that PRESSES them down
						if (NPC.Distance(Main.LocalPlayer.Center) < 2000)
						{
							Main.LocalPlayer.AddBuff(ModContent.BuffType<BaronsBurdenBuff>(), 1);
						}
					}
					if (Wet() && NPC.velocity.Length() > 3)
					{
						//stream of water dust and bubbles behind
						if (Main.rand.NextBool(2) && Main.netMode != NetmodeID.Server)
						{
							Vector2 backPos = NPC.Center - Vector2.Normalize(NPC.velocity) * 120 / 2f + Main.rand.NextVector2Circular(10, 10);
							Particle p = new Bubble(backPos, -NPC.velocity.RotatedByRandom(MathF.PI * 0.12f) * Main.rand.NextFloat(0.6f, 1f) / 2f, 1, 30, rotation: Main.rand.NextFloat(MathF.Tau));
							p.Spawn();
							Dust.NewDust(backPos, 2, 2, DustID.Water, -NPC.velocity.X, -NPC.velocity.Y, 0, default, 1f);
						}
					}
					break;
				case 2:
					int type = ModContent.ProjectileType<BaronArenaWhirlpool>();
					if (ArenaProjID == -1 && FargoSoulsUtil.HostCheck)
					{
						ArenaProjID = Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center, Vector2.Zero, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 3f, Main.myPlayer, NPC.whoAmI, 0);

					}
					NPC.netUpdate = true;
					break;
			}
			//Normal looping attack AI

			if (Attacking) //Phases and random attack choosing
			{
				switch (State) //Attack Choices
				{
					case (float)StateEnum.Opening:
						Opening();
						break;
					case (float)StateEnum.Phase2Transition:
						Phase2Transition();
						break;
					case (float)StateEnum.Swim:
						Swim();
						break;
					case (float)StateEnum.P1BigNuke:
						P1BigNuke();
						break;
					case (float)StateEnum.P1RocketStorm:
						if (!Main.expertMode)
						{
							goto default;
						}
						else
						{
							P1RocketStorm();
						}
						break;
					case (float)StateEnum.P1SurfaceMines:
						P1SurfaceMines();
						break;
					case (float)StateEnum.P1FadeDash:
						P1FadeDash();
						break;
					case (float)StateEnum.P1SineSwim:
						P1SineSwim();
						break;
					case (float)StateEnum.P1Whirlpool:
						if (!WorldSavingSystem.EternityMode)
						{
							goto default;
						}
						else
						{
							P1Whirlpool();
						}
						break;
					case (float)StateEnum.P2PredictiveDash:
						P2PredictiveDash();
						break;
					case (float)StateEnum.P2CarpetBomb:
						P2CarpetBomb();
						break;
					case (float)StateEnum.P2RocketStorm:
						if (!Main.expertMode)
						{
							goto default;
						}
						else
						{
							P2RocketStorm();
						}
						break;
					case (float)StateEnum.P2SpinDash:
						if (!Main.expertMode)
						{
							goto default;
						}
						else
						{
							P2SpinDash();
						}
						break;
					case (float)StateEnum.P2MineFlurry:
						P2MineFlurry();
						break;
					case (float)StateEnum.P2Whirlpool:
						if (!WorldSavingSystem.EternityMode)
						{
							goto default;
						}
						else
						{
							P2Whirlpool();
						}
						break;
					case (float)StateEnum.P2LaserSweep:
						P2LaserSweep();
						break;
					case (float)StateEnum.DeathAnimation:
						DeathAnimation();
						break;
					default:
						StateReset();
						break;
				}
			}
			Timer++;
		}
		bool Wet(Vector2? pos = null) //small helper
		{
			if (pos == null)
			{
				pos = NPC.position;
			}
			return Collision.WetCollision((Vector2)pos, NPC.width, NPC.height);
		}
		#endregion
		#region States
		void Opening()
		{
			HitPlayer = false;
			Anim = 1;
			//NPC.rotation = (float)(Math.Sin(MathHelper.ToRadians(Timer) * 16) * MathHelper.Pi/24);
			if (LockVector1 == Vector2.Zero)
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile p = Main.projectile[i];
					if (p.TypeAlive<MechLureProjectile>())
					{
						LockVector1 = p.Center;
						break;
					}
				}
				if (LockVector1 == Vector2.Zero) //no lure found
				{
					LockVector1 = player.Center;
				}
			}
			if (NPC.alpha > 0)
			{
				NPC.alpha -= 2;
			}
			else
			{
				NPC.alpha = 0;
			}
			if (LockVector1 != Vector2.Zero)
			{
				RotateTowards(LockVector1, 1.5f);
				NPC.velocity = Vector2.Lerp(NPC.velocity, (LockVector1 - NPC.Center) * (Timer / 90f) * 0.4f, 0.3f);
			}

			if (Timer == 60)
			{
				SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
				SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
				if (FargoSoulsUtil.HostCheck)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -24);
				}
				if (WorldSavingSystem.EternityMode && !WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.BanishedBaron] && FargoSoulsUtil.HostCheck)
					Item.NewItem(NPC.GetSource_Loot(), Main.player[NPC.target].Hitbox, ModContent.ItemType<MechLure>());
			}
			if (Timer > 90)
			{
				HitPlayer = true;
				Anim = 0;
				StateReset();
			}
		}
		void Phase2Transition()
		{

			const int transTime = 150;
			//comedy rotation anim to look at falling propeller
			if (Timer < 60)
			{
				RotateTowards(NPC.Center + LockVector1, 15);
			}
			else if (Timer < 135)
			{
				Vector2 dir = Vector2.Normalize(-LockVector1) * 2 + Vector2.UnitY * 1;
				RotateTowards(NPC.Center + dir, 15);
			}
			else if (Timer < 150)
			{
				Vector2 up = NPC.Center + new Vector2(3 * Math.Sign(NPC.SafeDirectionTo(player.Center).X), -10);
				RotateTowards(up, 15);
			}

			if (Timer == 1)
			{
				SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/BaronHit"), NPC.Center);
				if (!Main.dedServ)
					Main.LocalPlayer.FargoSouls().Screenshake = 100;

				HitPlayer = false;
				LockVector1 = Vector2.UnitX * Math.Sign(player.Center.X - NPC.Center.X); //register for rotation animation

			}
			if (Timer == 45)
			{
				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);
				Vector2 propellerPos = NPC.Center - NPC.rotation.ToRotationVector2() * (NPC.width * 0.25f);
				if (!Main.dedServ)
				{
					Gore g = Gore.NewGorePerfect(NPC.GetSource_FromThis(), propellerPos, -NPC.rotation.ToRotationVector2() * 4, ModContent.Find<ModGore>(Mod.Name, $"BaronGorePropeller").Type);
					g.light = 1f;
				}

				for (int i = 0; i < 5; i++)
				{
					Particle p = new SparkParticle(propellerPos, -NPC.rotation.ToRotationVector2().RotatedByRandom(MathHelper.Pi / 8) * Main.rand.NextFloat(8, 12), Color.Orange, 1, 40);
					p.Spawn();
				}
			}
			if (Timer > 45 && Timer < transTime)
			{
				Anim = 3;
			}
			else
			{
				Anim = 0;
			}
			if (Timer < transTime)
			{
				if (Main.LocalPlayer.wet)
				{
					Main.LocalPlayer.velocity.Y -= 0.05f;
				}
			}
			if (Timer == transTime)
			{
				NPC.velocity = NPC.rotation.ToRotationVector2() * 20;
				SoundEngine.PlaySound(BaronYell, NPC.Center);
				Phase = 2;
				FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);

				Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod) ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Baron2") : MusicID.Boss2;

			}
			if (Timer > transTime)
			{
				if (!Wet() || Timer > transTime + 60)
				{
					NPC.velocity *= 0.95f;
					if (NPC.velocity.Length() < 0.1f)
					{
						DidWhirlpool = false; // reset this
						HitPlayer = true;
						availablestates.Clear();
						StateReset();

					}
				}
				if (Main.LocalPlayer.wet && Main.LocalPlayer.velocity.Y > -30)
				{
					Main.LocalPlayer.velocity.Y -= 1.25f;
					Main.LocalPlayer.position.Y -= 6f;
				}
			}

		}
		void DeathAnimation()
		{
			const int AnimTime = 200;

			Main.musicFade[Main.curMusic] = (float)Utils.Lerp(1f, 0.3f, Timer / AnimTime);
			NPC.velocity *= 0.96f;
			Trail = 8;
			Anim = 1;
			NPC.noTileCollide = false;
			NPC.waterMovementSpeed = 0.75f;
			if (NPC.velocity.Y < 20)
			{
				NPC.velocity.Y += 0.6f;
			}
			if (Timer % 20 == 19)
			{
				SoundEngine.PlaySound(BaronNuke.Beep with { Pitch = Utils.Clamp(-1 + 1.25f * (Timer / AnimTime), -1, 1) }, NPC.Center);
			}
			int expFreq = (int)Math.Round(30 - 28 * (Timer / AnimTime));
			if (Timer % expFreq == 0)
			{
				Vector2 topleft = NPC.position + NPC.Size / 3;
				Rectangle rect = new((int)topleft.X, (int)topleft.Y, (int)(NPC.width * 1 / 3f), (int)(NPC.height * 1 / 3f));
				Vector2 exCenter = Main.rand.NextVector2FromRectangle(rect);
				SoundEngine.PlaySound(SoundID.Item14, exCenter);
				Vector2 pos = exCenter; //circle with highest density in middle
				Vector2 vel = NPC.velocity;
				Particle p = new ExpandingBloomParticle(pos, vel, Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat()), startScale: Vector2.One * 3, endScale: Vector2.One * 6, lifetime: 15);
				p.Spawn();

				Vector2 dir = Main.rand.NextVector2CircularEdge(1, 1);
				float speed = Main.rand.NextFloat(6, 15);
				int type = Main.rand.Next(6) + 1;
				if (!Main.dedServ)
					Gore.NewGore(NPC.GetSource_FromThis(), pos, dir * speed, ModContent.Find<ModGore>(Mod.Name, $"BaronScrapGore{type}").Type, NPC.scale);
			}
			if (Wet())
			{
				NPC.velocity *= 0.85f;
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.rotation.ToRotationVector2() * NPC.velocity.Length(), 0.2f);
				RotateTowards(NPC.Center + Vector2.UnitX * Math.Sign(NPC.rotation.ToRotationVector2().X), 0.25f);
			}
			else
			{
				RotateTowards(NPC.Center + Vector2.UnitY, 1.2f);
			}
			if (Collision.SolidCollision(NPC.position + NPC.velocity, NPC.width, NPC.height) || Timer > AnimTime)
			{
				SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
				NPC.StrikeInstantKill();
			}
		}
		#region Phase 1 Attacks
		void Swim()
		{
			void Thirsty()
			{
				int x = (int)NPC.Center.X / 16;
				int y = (int)NPC.Center.Y / 16;
				Tile tile = Main.tile[x, y];

				if (tile.LiquidAmount > 0)
				{
					if (tile.LiquidType == LiquidID.Water)
					{
						tile.LiquidAmount = 0;
						CombatText.NewText(NPC.Hitbox, Color.Blue, Language.GetTextValue("Mods.FargowiltasSouls.NPCs.BanishedBaron.Slurp"));
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.sendWater(x, y);
							NetMessage.SendTileSquare(-1, x, y, 1);
						}
						else
						{
							WorldGen.SquareTileFrame(x, y, true);
						}
					}
				}
			}
			int Distance = (int)Math.Min(NPC.Distance(player.Center) + 50, 450);
			if (Timer == 1)
			{
				/*
                int xdif = (int)Math.Abs(NPC.Center.X - player.Center.X);
                int ydif = (int)Math.Abs(NPC.Center.Y - player.Center.Y);
                if (availablestates.Contains((int)StateEnum.P1SurfaceMines) && Main.rand.NextBool() && xdif.IsWithinBounds(200, 600) && ydif.IsWithinBounds(200))
                {
                    StateReset(false);
                    State = (float)StateEnum.P1SurfaceMines;
                    availablestates.Remove((int)StateEnum.P1SurfaceMines);
                    return;
                }
                */
				if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) //check if originated in collision, set AI3 to negative
				{
					AI3 = -2;
					for (int i = 0; i < 30; i++) //max of 30 checks
					{
						LockVector1 = player.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Distance;
						if (!Collision.SolidCollision(LockVector1 - NPC.Size / 2, NPC.width, NPC.height) && Wet(LockVector1 - NPC.Size / 2)) //if found valid spot, stop searching
						{
							break;
						}
					}
				}
				else
				{
					LockVector1 = player.Center + player.SafeDirectionTo(NPC.Center) * Distance;
				}

				if (Wet() && WorldSavingSystem.MasochistModeReal) //chug the ocean in masomode
				{
					Thirsty();
				}
			}

			//bool NewCollision = (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && AI3 != -2); //if didn't originate in collision, 
			if (Vector2.Distance(NPC.Center, LockVector1) < 25 || Timer > 100 || (AI3 != -2 && AI3 != 0)/* || NewCollision*/) //if ai3 not 0 (default) or -2 (set when original collision)
			{

				AI3 = (AI3 + 1) * 3; //stays negative if set to -2 by checking original collision, otherwise goes positive
				NPC.velocity *= 0.85f;
				RotateTowards(player.Center, 3);
				if (NPC.velocity.Length() < 0.1f && Timer > 50)
				{
					if (Wet() && WorldSavingSystem.MasochistModeReal) //chug the ocean in masomode
					{
						Thirsty();
					}
					NPC.velocity = Vector2.Zero;
					StateReset();
				}
			}
			else
			{
				if (Collision.SolidCollision(NPC.Center + NPC.velocity, NPC.width, NPC.height) && Timer < 40 && AI3 != -2)
				{
					Timer = 40; //so it doesn't get stuck on tiles too long
				}
				Vector2 vectorToIdlePosition = LockVector1 - NPC.Center;
				float speed = AI3 >= 0 ? 28 : 14; //less speed if originated in collision
				if (WorldSavingSystem.EternityMode && !WorldSavingSystem.MasochistModeReal) //maso speed is already very fast because ignore water
				{
					speed *= 1.2f;
				}
				float inertia = AI3 >= 0 ? 20f : 10f;
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				NPC.velocity = Vector2.Lerp(NPC.velocity, (NPC.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia, 0.3f);
				if (NPC.velocity == Vector2.Zero)
				{
					NPC.velocity.X = -0.15f;
					NPC.velocity.Y = -0.05f;
				}
				NPC.rotation = NPC.velocity.ToRotation();
			}
		}
		void P1BigNuke()
		{
			/*
            if (Timer == 1)
            {
                if (FargoSoulsUtil.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, NPC.whoAmI);
            }
            */

			RotateTowards(player.Center, 2);
			if (Timer < 30)
				Anim = 1;
			else
				Anim = 0;
			if (Timer == 30)
			{
				SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
				if (FargoSoulsUtil.HostCheck)
				{
					Vector2 vel = NPC.rotation.ToRotationVector2() * 10f;
					AI2 = Main.rand.NextFloat(190, 220); //nuke duration
					NPC.netUpdate = true;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, AI2, player.whoAmI);
				}
			}
			if (Timer > AI2 + 10 && AI2 > 0)
			{
				Anim = 0;
				StateReset();
			}
		}
		void P1RocketStorm()
		{
			RotateTowards(player.Center, 3);
			const int StartupTime = 30;
			if (Timer < StartupTime + 50 && Timer % 6 == 0 && Timer > StartupTime)
			{
				Anim = 1;
				SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
				if (FargoSoulsUtil.HostCheck)
				{
					float progress = (Timer - 10) / 60;
					float rotation = progress * (MathHelper.Pi / 6);
					Vector2 vel = (NPC.rotation + ((MathHelper.PiOver2 + rotation) * -NPC.direction)).ToRotationVector2() * (10 * progress + 10);
					float trackingPower = WorldSavingSystem.MasochistModeReal ? 1.2f : 1f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, player.whoAmI, trackingPower); //ai2 is tracking power, above 1 is pseudo-predictive
				}
			}
			if (Timer > StartupTime + 50)
			{
				Anim = 0;
				StateReset();
			}
		}
		void P1SurfaceMines()
		{
			NPC.noTileCollide = true;
			//Anim = 1;
			if (Timer == 1)
			{
				Vector2 dif = player.Center - NPC.Center;
				if (Math.Abs(dif.X) < 200 || Math.Abs(dif.Y) > 350)
				{
					State = (float)StateEnum.Swim;
					StateReset();
				}
				LockVector1 = new Vector2(player.Center.X, player.Center.Y + 800);
			}
			if (Timer >= 1)
				RotateTowards(LockVector1, 2);
			if (Timer < 33)
			{
				Vector2 desiredPos = Vector2.UnitX * (player.Center.X + Math.Sign(NPC.Center.X - player.Center.X) * 400) + Vector2.UnitY * player.Center.Y;
				NPC.velocity = Vector2.Lerp(NPC.velocity, (desiredPos - NPC.Center) * (Timer / 33) / 3, 0.3f);
			}
			if (Timer >= 33 && Timer <= 40)
			{
				NPC.velocity = Vector2.Lerp(NPC.velocity / 3, NPC.rotation.ToRotationVector2() * 12, (Timer - 33) / 7);
			}
			if (Timer == 40)
			{
				SoundEngine.PlaySound(BaronYell, NPC.Center);

			}
			if (Timer > 60)
				NPC.velocity *= 0.97f;
			else if (player.velocity.Y > 0)
				NPC.velocity.Y += player.velocity.Y;

			if (NPC.velocity.Length() > 12)
				NPC.velocity = Vector2.Normalize(NPC.velocity) * 12;

			if (Timer >= 60 && Timer % 5 == 0 && AI3 < 4)
			{
				AI3++;
				SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
				if (FargoSoulsUtil.HostCheck)
				{
					Vector2 dir = NPC.rotation.ToRotationVector2();
					float extraY = Math.Max(player.velocity.Y, 0);
					Vector2 vel = Vector2.UnitX * Math.Sign(dir.X) * AI3 * 8 + Vector2.UnitY * dir.Y * (8 + extraY);
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI);
					if (WorldSavingSystem.MasochistModeReal)
					{
						Vector2 vel2 = Vector2.UnitX * Math.Sign(-dir.X) * (AI3 - 1) * 8 + Vector2.UnitY * dir.Y * (8 + extraY);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel2, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI);
					}

				}
			}
			if (Timer > 160)
			{
				StateReset();
			}
		}
		void P1FadeDash()
		{
			int ReactionTime = WorldSavingSystem.EternityMode ? 30 : 50;
			if (Timer == 1)
			{
				if (Main.rand.NextBool()) //bias towards diagonals
				{
					LockVector1 = (-Vector2.UnitY * 620).RotatedBy(MathHelper.PiOver2 * (Main.rand.NextBool() ? 1 : -1)).RotatedByRandom(MathHelper.Pi / 4); //approximately diagonal angle
				}
				else
				{
					LockVector1 = (-Vector2.UnitY * Main.rand.Next(500, 600)).RotatedByRandom(MathHelper.PiOver2);
				}
				NPC.netUpdate = true;
			}
			if (Timer < 90 && Timer % 3 == 0) //keep looking for valid spot every 3 ticks, including when spot gets invalidated
			{
				bool findNew = false;
				Vector2 chosenPos = player.Center + LockVector1 - NPC.Size / 2;
				if (!Collision.SolidCollision(chosenPos, NPC.width, NPC.height) && !Collision.WetCollision(chosenPos, NPC.width, NPC.height))
					findNew = true;
				for (float i = 0; i < 0.6f; i += 0.1f) //don't check the entire way, otherwise every spot is invalid if you're standing on ground
				{
					Vector2 pos = chosenPos - ((LockVector1) * i);

					if (Collision.SolidCollision(pos, NPC.width, NPC.height))
					{
						findNew = true;
						break;
					}

				}
				if (findNew)
				{
					LockVector1 = (Vector2.UnitY * Main.rand.Next(500, 600)).RotatedByRandom(MathHelper.Pi);
					NPC.netUpdate = true;
				}
			}
			if (Timer < 60)
			{
				if (NPC.buffType[0] != 0) //cleanse all buffs
					NPC.DelBuff(0);

				RotateTowards(player.Center, 2);
				if (NPC.velocity.Length() < 1)
				{
					NPC.velocity += NPC.rotation.ToRotationVector2() * 0.02f;
					NPC.velocity *= 1.03f;
				}
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.rotation.ToRotationVector2() * NPC.velocity.Length(), 0.3f);
				NPC.Opacity -= 1f / 50;
				if (NPC.Opacity < 0)
					NPC.Opacity = 0;
				HitPlayer = false;
			}
			if (Timer == 60)
			{
				if (FargoSoulsUtil.HostCheck)
				{

					NPC.netUpdate = true;
					NPC.rotation = NPC.SafeDirectionTo(player.Center + LockVector1).ToRotation();
				}
			}
			if (Timer > 60 && Timer < 90) //gone
			{
				if (NPC.buffType[0] != 0) //cleanse all buffs
					NPC.DelBuff(0);

				NPC.rotation = NPC.SafeDirectionTo(player.Center + LockVector1).ToRotation();
				NPC.noTileCollide = true;
				NPC.Center = player.Center + LockVector1;
				NPC.netUpdate = true;
				NPC.dontTakeDamage = true;
			}
			if (Timer == 90)
			{
				if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
				{
					NPC.noTileCollide = false;
				}
				NPC.velocity = Vector2.Zero;
				NPC.rotation = NPC.SafeDirectionTo(player.Center).ToRotation();
				if (FargoSoulsUtil.HostCheck)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.rotation.ToRotationVector2() * NPC.width * 0.35f, Vector2.Zero, ModContent.ProjectileType<BaronEyeFlash>(), 0, 0, Main.myPlayer, NPC.whoAmI);
				}
			}
			if (Timer > 90 && Timer < 90 + ReactionTime)
			{
				RotateTowards(player.Center, 2);
				if (NPC.velocity.Length() < 3)
				{
					NPC.velocity += NPC.rotation.ToRotationVector2() * 0.03f;
					NPC.velocity *= 1.03f;
				}
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.rotation.ToRotationVector2() * NPC.velocity.Length(), 0.3f);
				NPC.Opacity += 1f / ReactionTime;
				if (NPC.Opacity > 1)
					NPC.Opacity = 1;
				HitPlayer = true;
			}
			int attackTime = 90 + ReactionTime;
			int accelTime = 7;
			float baseSpeed = 45;
			float extraSpeed = (float)Math.Sqrt((player.Center - NPC.Center).Length()) / 1.5f;
			if (Timer > attackTime - accelTime && Timer < attackTime)
			{
				float speed = Math.Max(((Timer - (attackTime - accelTime)) / accelTime), 0);
				NPC.velocity = NPC.rotation.ToRotationVector2() * (baseSpeed + extraSpeed) * speed;
			}
			if (Timer == attackTime)
			{
				NPC.Opacity = 1;
				NPC.dontTakeDamage = false;
				SoundEngine.PlaySound(BaronRoar, NPC.Center);

				NPC.velocity = NPC.rotation.ToRotationVector2() * (baseSpeed + extraSpeed);

			}
			if (Timer > 90 + ReactionTime)
			{
				if (NPC.noTileCollide) //dash same speed if collision is off
				{
					NPC.position -= NPC.velocity / 2;
				}
				else
				{
					if (Collision.SolidCollision(NPC.position + NPC.velocity, NPC.width, NPC.height))
					{
						SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
						NPC.velocity = -NPC.velocity / 4;
						NPC.velocity = NPC.velocity.RotatedByRandom(MathHelper.Pi * 0.15f);
						NPC.SimpleStrikeNPC(500, 1);
						AI3 = 1;

						if (FargoSoulsUtil.HostCheck)
						{
							for (int i = 0; i < 6; i++)
							{
								Vector2 v = Vector2.Normalize(NPC.velocity.RotatedByRandom(MathHelper.Pi * 0.2f)) * Main.rand.NextFloat(7, 10);
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + v, v, ModContent.ProjectileType<BaronScrap>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
							}
						}
					}
				}
				NPC.dontTakeDamage = false; //here too for safety
				NPC.velocity *= 0.975f;

				if (NPC.velocity.Length() > 8 && WorldSavingSystem.EternityMode && AI3 != 1)
				{
					int rocketTime = 11;
					if (Timer % rocketTime == 0)
					{
						SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
						for (int i = -1; i < 2; i += 2)
						{
							if (FargoSoulsUtil.HostCheck)
							{
								Vector2 v = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 1f;
								Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v * 20), v * 0.3f, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 4, player.whoAmI);
							}
						}
					}
				}
			}
			if ((Timer == 90 + ReactionTime + 15 || Timer == 90 + ReactionTime + 30) && WorldSavingSystem.EternityMode && AI3 != 1)
			{
				SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
				if (FargoSoulsUtil.HostCheck)
				{
					Vector2 vel = NPC.velocity * 1.2f;
					float trackingPower = WorldSavingSystem.MasochistModeReal ? 1.5f : 1f;
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 3, player.whoAmI, trackingPower); //ai2 is tracking power, above 1 is pseudo-predictive
				}
			}
			if (Timer > 90 + ReactionTime + 30 && NPC.velocity.Length() < 1.5f)
			{
				NPC.noTileCollide = false;
				StateReset();
			}
		}
		void P1SineSwim()
		{
			int Duration = WorldSavingSystem.MasochistModeReal ? 100 : WorldSavingSystem.EternityMode ? 150 : 180;
			const int Waves = 2;
			const int Ymax = 600;
			const int Xstart = 1000;

			float prog = (float)Math.Pow(Timer / Duration, 2);

			NPC.noTileCollide = true;
			if (Timer == 1)
			{
				AI2 = Math.Sign(NPC.Center.X - player.Center.X);
				LockVector1 = player.Center + new Vector2(Xstart * AI2, 0);
				SoundEngine.PlaySound(BaronYell, NPC.Center);
				NPC.netUpdate = true;
			}
			if (AI3 == 0)
			{
				Vector2 target = LockVector1 + new Vector2(-AI2 * prog * (1.75f * Xstart), Ymax * (float)Math.Sin(MathHelper.TwoPi * prog * Waves));
				NPC.rotation = NPC.SafeDirectionTo(target).ToRotation();
				//float modifier = Math.Min(Timer / 50, 1);
				float modifier = 1;
				NPC.velocity = Vector2.Lerp(NPC.velocity, modifier * NPC.SafeDirectionTo(target) * NPC.Distance(target) / 1.2f, 0.3f);
				if (NPC.velocity.Length() > 25)
				{
					NPC.velocity *= 25 / NPC.velocity.Length();
				}
				if (Timer == Math.Round(Duration * 0.33f) || Timer == Math.Round(Duration * 0.66f) || Timer == Math.Round(Duration * 0.99f))
				{
					SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
					if (FargoSoulsUtil.HostCheck)
					{
						Vector2 vel = (-NPC.rotation).ToRotationVector2() * 5;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
					}
				}
				if (prog > 1)
				{
					AI3 = 1;
				}
			}
			if (AI3 == 1)
			{
				NPC.velocity *= 0.96f;
			}
			if (NPC.velocity.Length() < 1f)
			{
				NPC.noTileCollide = false; //this is probably automatically set in AI anyway but just to make sure
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		void P1Whirlpool()
		{
			NPC.noTileCollide = true;
			const int Height = 2000;
			if (Timer == 1)
			{
				if (DidWhirlpool)
				{
					DidWhirlpool = false;
					LastState = (int)State;
					State = (float)StateEnum.Swim;
					StateReset();
					return;
				}
				DidWhirlpool = true;
				const int checks = 20;
				for (int i = 0; i < checks; i++) //find approx surface level above player
				{
					LockVector1 = player.Center - Vector2.UnitY * Height * (checks - i);

					if (Wet(LockVector1 + Vector2.UnitY * 2f * Height / (float)checks)) //check 2 checks down
					{
						NPC.netUpdate = true;
						break;
					}
				}

			}
			if (Timer < 60)
			{
				HitPlayer = false;
				NPC.velocity = Vector2.Lerp(NPC.velocity, (LockVector1 - NPC.Center) * (Timer / 60) * 0.04f, 0.3f);
				RotateTowards(LockVector1, 2);
				if (!Wet(NPC.position + Vector2.UnitY * NPC.height / 4) && NPC.Center.Y < player.Center.Y)
				{
					if (Collision.WetCollision(player.position, player.width, player.height))
					{
						NPC.velocity.Y *= 0.2f;
					}
					else if (NPC.Center.Y < player.Center.Y - 500)
					{
						NPC.velocity.Y *= 0.2f;
					}

				}
				else
				{
					NPC.velocity.Y += 0.25f;
				}
				if (NPC.velocity.LengthSquared() >= 30 * 30)
				{
					NPC.velocity = Vector2.Normalize(NPC.velocity) * 30;
				}
			}
			else
			{
				HitPlayer = true;
				NPC.velocity = Vector2.Zero;
			}
			if (Timer > 50)
			{
				RotateTowards(NPC.Center - Vector2.UnitY.RotatedBy(MathHelper.ToRadians(1)), 16);
			}
			if (Timer == 60 && FargoSoulsUtil.HostCheck)
			{
				AnimationSpeed = 2f;
				int variant = Main.rand.Next(2);
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitY * ((NPC.height / 2) + 24), Vector2.Zero, ModContent.ProjectileType<BaronWhirlpool>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, MaxWhirlpools, variant);
			}
			if (Timer >= 60 * 8)
			{
				AnimationSpeed = 1f;
				NPC.noTileCollide = false; //this is probably automatically set in AI anyway but just to make sure
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		#endregion
		#region Phase 2 Attacks
		void P2PredictiveDash()
		{
			float PredictStr = 35;
			const int ReactTime = 72;
			if (Timer == 1)
			{
				if (FargoSoulsUtil.HostCheck)
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, NPC.whoAmI);
			}
			if (Timer == ReactTime - 5) //slight telegraph sound to be cute
			{
				SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
			}
			if (Timer < ReactTime - 5) //stop aligning 5 frames before dashing
			{
				Vector2 target = player.Center + (Vector2.Normalize(NPC.Center - player.Center) * 500);
				if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
				{
					AI3 = -1;
					NPC.netUpdate = true;
				}

				if (AI3 == 0)
				{
					if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
					{
						NPC.velocity += NPC.SafeDirectionTo(target) * 0.02f;
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * NPC.velocity.Length() * 1.05f, 0.5f);
					}
					else
					{
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * (20 + (player.velocity.Length() / 2)), 0.5f);

					}
				}
				else
				{
					NPC.velocity = Vector2.Lerp(NPC.velocity, (target - NPC.Center) / 10, 0.3f);
				}
				if (NPC.Distance(player.Center) < 200)
					NPC.velocity -= NPC.SafeDirectionTo(player.Center);

				LockVector1 = FargoSoulsUtil.PredictiveAim(NPC.Center, player.Center, player.velocity, PredictStr);
				Vector2 dir = Vector2.Lerp(NPC.rotation.ToRotationVector2(), Vector2.Normalize(LockVector1), 0.2f);
				NPC.rotation = dir.ToRotation();

				//LockVector1 = NPC.SafeDirectionTo(player.Center + (player.velocity * PredictStr)) * PredictStr;
				//RotateTowards(NPC.Center + LockVector1, 4);
			}
			int accelTime = 7;
			if (Timer > ReactTime - accelTime && Timer < ReactTime)
			{
				float speed = Math.Max(((Timer - (ReactTime - accelTime)) / accelTime), 0);
				NPC.velocity = NPC.rotation.ToRotationVector2() * PredictStr * speed;
			}
			if (Timer == ReactTime)
			{
				SoundEngine.PlaySound(BaronRoar, NPC.Center);
				Trail = 8;
				NPC.velocity = NPC.rotation.ToRotationVector2() * PredictStr;
				Vector2 uv = Vector2.Normalize(NPC.velocity);
				Vector2 lp = player.Center - NPC.Center;
				float lambda = Vector2.Dot(uv, lp);
				LockVector2 = NPC.Center + (uv * lambda);

				//below: instantly decelerate if dash is at large angle from player
				Vector2 PV = NPC.SafeDirectionTo(player.Center);
				Vector2 LV = LockVector1;
				float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
				if (Math.Abs(anglediff) > MathHelper.PiOver2)
				{
					AI3 = 1;
				}
			}
			if (Timer >= ReactTime && Timer < ReactTime + 65 && NPC.velocity.Length() > 5)
			{
				int rocketTime = (int)(8 * 20 / NPC.velocity.Length());
				if (Timer % rocketTime == 0)
				{
					SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
					for (int i = -1; i < 2; i += 2)
					{
						if (FargoSoulsUtil.HostCheck)
						{
							Vector2 v = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * i) * 1f;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (v * 20), v * 0.3f, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
						}
					}
				}
			}

			if (NPC.Distance(LockVector2) <= NPC.velocity.Length() || (Timer > ReactTime + 45 && Timer < ReactTime + 90) || (Timer > ReactTime + 90 + 45) && WorldSavingSystem.EternityMode)
			{
				AI3 = 1;
			}
			if (AI3 == 1)
			{
				NPC.velocity *= 0.97f;
				if (Timer >= ReactTime + 65)
					RotateTowards(player.Center, 8);
			}
			if (Timer == ReactTime + 90 && WorldSavingSystem.EternityMode)
			{
				SoundEngine.PlaySound(BaronRoar, NPC.Center);
				NPC.velocity = NPC.rotation.ToRotationVector2() * PredictStr * 1.2f;

			}
			if (NPC.velocity.Length() < 5f && AI3 == 1 && (!WorldSavingSystem.EternityMode || Timer > ReactTime + 90))
			{
				Trail = 0;
				NPC.velocity *= 0.9f;
				if (NPC.velocity.Length() < 0.7f)
					StateReset();
			}
		}
		void P2CarpetBomb()
		{
			const int Xstart = 800;
			const int YMax = 500;
			const int YMin = 300;
			if (Timer == 1)
			{
				AI2 = Math.Sign(NPC.Center.X - player.Center.X);

			}
			if (AI3 == 0) //startup, reach starting point
			{
				float x = Xstart * AI2;
				LockVector1 = player.Center + new Vector2(x, -YMin);
				NPC.rotation = NPC.SafeDirectionTo(LockVector1).ToRotation();
				if (NPC.velocity.Length() < 30 + player.velocity.Length())
				{
					NPC.velocity += 0.5f * NPC.rotation.ToRotationVector2();
					NPC.velocity *= 1.03f;
				}
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.velocity.Length() * NPC.rotation.ToRotationVector2(), 0.3f);
				if (NPC.Distance(LockVector1) < 25)
				{
					AI3 = 1; //reached point, start attack
				}
			}
			if (AI3 > 0) //arc attack
			{
				const int ArcTime = 90;

				//funky real math i did on paper and translated to code

				Vector2 Curve(float p) //makes a smooth wide curve
				{
					float x = AI2 * Xstart * (1 - (2 * p));
					return new Vector2(x, (float)(-YMin - ((YMax - YMin) * Math.Sin(MathHelper.Pi * p))));
				}
				float prog = AI3 / ArcTime;

				Vector2 curve = Curve(prog);
				Vector2 dydx = Curve(prog + 0.00001f) - curve;
				NPC.rotation = dydx.ToRotation();
				LockVector1 = player.Center + curve;
				NPC.velocity = LockVector1 - NPC.Center;
				AI3++;

				float mineCount = WorldSavingSystem.MasochistModeReal ? 8f : WorldSavingSystem.EternityMode ? 7f : 6f;
				if ((AI3 % Math.Round(ArcTime / mineCount)) == 0)
				{
					SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
					if (FargoSoulsUtil.HostCheck)
					{
						Vector2 vel = new(0, Main.rand.Next(15, 20));
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
					}
				}

				if (prog >= 1)
				{
					AI3 = -1; //end attack, start next part
					SoundEngine.PlaySound(BaronYell, NPC.Center);
				}
			}
			if (AI3 < 0) //home into player after arc
			{

				if (NPC.Distance(player.Center) < 300 && AI3 < -30)
				{
					AI2 = -2;
				}
				if (AI2 == -2)
				{
					NPC.velocity *= 0.975f;
				}
				else
				{
					float maxSpeed = 15 + (player.velocity.Length() / 2);
					if (NPC.velocity.Length() < maxSpeed)
					{
						NPC.velocity += NPC.rotation.ToRotationVector2() * 0.02f;
						NPC.velocity = NPC.rotation.ToRotationVector2() * NPC.velocity.Length() * 1.03f;
					}
					else
					{
						NPC.velocity = NPC.rotation.ToRotationVector2() * maxSpeed;
					}
					float rotSpeed = 2.5f;
					RotateTowards(player.Center, rotSpeed);
				}
				AI3--;

			}
			if ((AI3 < -30 && NPC.velocity.Length() < 1.25) || AI3 < -220) //second condition is failsafe for circle cheese
			{
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		void P2RocketStorm()
		{
			const int distance = 500;
			float rotModifier = 1;
			float minRot = 0.5f;
			if (Timer == 1)
			{
				AI2 = Math.Sign(NPC.Center.X - player.Center.X);
				LockVector1 = new Vector2(distance * AI2, 0);

			}

			RotateTowards(player.Center, 4);

			float rotation = MathHelper.ToRadians(AI3 + (minRot * Math.Sign(AI3)));
			LockVector1 = LockVector1.RotatedBy(rotation * rotModifier);

			Vector2 target = player.Center + LockVector1;

			if ((NPC.Distance(target) < 200 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
			{
				AI3 = Main.rand.NextFloat(-1, 1);
				NPC.netUpdate = true;
			}

			if (AI3 == 0)
			{
				if (NPC.velocity.Length() <= 20 + (player.velocity.Length() / 2))
				{
					NPC.velocity += NPC.SafeDirectionTo(target) * 0.02f;
					NPC.velocity = NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * NPC.velocity.Length() * 1.05f, 0.5f);
				}
				else
				{
					NPC.velocity = NPC.SafeDirectionTo(target) * (20 + (player.velocity.Length() / 2));

				}
				if (Timer > 2)
				{
					Timer = 2;
				}
			}
			else
			{
				NPC.velocity = Vector2.Lerp(NPC.velocity, (target - NPC.Center) / 10, 0.3f);
			}


			if (AI3 != 0 && Timer < 300)
			{

				bool volleyTime = (Timer > 40 && Timer < 100) || (Timer > 140 && Timer < 200) || (Timer > 240 && Timer < 300);
				if (volleyTime)
				{
					Anim = 1;
					if (Timer % 8 == 0)
					{
						SoundEngine.PlaySound(SoundID.Item64, NPC.Center);
						if (FargoSoulsUtil.HostCheck)
						{
							int side = (Timer > 140 && Timer < 200) ? -1 : 1;
							float speed = Main.rand.NextFloat(10, 25);
							Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * side).RotatedByRandom(MathHelper.Pi / 8) * speed;
							float trackingPower = WorldSavingSystem.MasochistModeReal ? 1 : 1;
							Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (Vector2.Normalize(vel) * 20), vel, ModContent.ProjectileType<BaronRocket>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 1, player.whoAmI, trackingPower); //ai2 is tracking power
						}
					}
				}
				else
				{
					Anim = 0;
				}
				//fire rockets
			}

			if (Timer > 300)
			{
				Anim = 0;
			}
			if (Timer > 310)
			{
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		void P2SpinDash()
		{
			const int radius = 500;
			const float startRot = MathHelper.PiOver2 * 0.7f;
			float rotModifier = 3.5f;
			if (Timer == 1)
			{
				AI2 = Math.Sign(NPC.Center.X - player.Center.X);

			}
			if (AI3 == 0) //startup, reach starting point
			{

				LockVector1 = new Vector2(0, radius).RotatedBy(startRot * -AI2);
				Vector2 target = player.Center + LockVector1;
				NPC.rotation = NPC.SafeDirectionTo(target).ToRotation();

				//velocity on rotation = radius * rotation per tick
				NPC.netUpdate = true;
				float rotVelocity = radius * MathHelper.ToRadians(3); //maximum value since random rotation velocity is not determined yet (could be done better by determining random before but would have to rework structure and i'm too lazy)
				if (NPC.velocity.Length() < rotVelocity + player.velocity.Length())
				{
					NPC.velocity += 0.5f * NPC.rotation.ToRotationVector2();
					NPC.velocity *= 1.03f;
				}
				NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.velocity.Length() * NPC.rotation.ToRotationVector2(), 0.3f);
				if (NPC.Distance(target) < 25)
				{
					AI3 = Main.rand.NextFloat(0.2f, 0.4f); //reached point, start attack, decide random rotation during charge windup
					NPC.netUpdate = true;
				}
			}
			if (AI3 > 0) //rotate a random bit below, and then dash
			{
				const int rotStart = 38;
				const int dashStart = 45;
				const int DashSpeed = 27;

				AI2 += Math.Sign(AI2); //internal timer, efficient variable use (?)

				Vector2 target = player.Center + LockVector1;



				if (Math.Abs(AI2) <= rotStart)
				{
					NPC.velocity = Vector2.Lerp(NPC.velocity, (target - NPC.Center) / 10, 0.3f);
					NPC.rotation = NPC.velocity.ToRotation();
					if (Math.Abs(AI2) == rotStart)
					{
						SoundEngine.PlaySound(BaronYell, NPC.Center);
					}
				}
				else //prepare for dash and dash
				{
					const int reactTime = 20;
					rotModifier = 0.025f; //rotate very slowly when preparing for dash
					RotateTowards(player.Center, 6f);
					if (Math.Abs(AI2) > dashStart + reactTime)
					{
						SoundEngine.PlaySound(BaronRoar, NPC.Center);
						SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
						NPC.velocity = NPC.rotation.ToRotationVector2() * DashSpeed;
						AI3 = -1;
						AI2 = Math.Sign(AI2);
					}
				}

				float rotation = MathHelper.ToRadians(AI3 * Math.Sign(AI2) * rotModifier);
				LockVector1 = LockVector1.RotatedBy(rotation);

			}
			const int end = -110;
			const int bulletStart = -42;
			if (AI3 < 0) //spin after dash
			{
				if (AI3 == -1)
				{
					AI2 = Math.Sign(NPC.Center.X - player.Center.X);
				}
				AI3--;


				if (AI3 >= end)
				{
					if (Math.Abs(AI2) < 60)
					{
						AI2 += Math.Sign(AI2) * 2;
					}
					NPC.velocity *= 0.995f;
					NPC.velocity.Y += 0.2f;
					NPC.rotation += -AI2 * MathHelper.Pi / 16 / 60;
				}
				else
				{
					if (Math.Abs(AI2) > 4)
					{
						AI2 -= Math.Sign(AI2) * 2;
					}

					NPC.velocity *= 0.955f;
					NPC.rotation += -AI2 * MathHelper.Pi / 16 / 60;
				}
				if (AI3 < bulletStart)
				{
					Anim = 1;
					if (FargoSoulsUtil.HostCheck)
					{
						float speed = Main.rand.NextFloat(8, 13);
						Vector2 v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + v, v * speed, ModContent.ProjectileType<BaronScrap>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
					}
				}
			}
			if (AI3 < end - 20)
			{
				Anim = 0;
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		void P2MineFlurry()
		{
			const int distance = 500;
			if (Timer == 1)
			{
				AI2 = Math.Sign(NPC.Center.X - player.Center.X);
				LockVector1 = new Vector2(distance * AI2, 0);

			}

			LockVector1 = new Vector2(distance * AI2, 0);
			Vector2 target = player.Center + LockVector1;

			if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
			{
				AI3 = 1;
			}

			if (AI3 == 0)
			{
				Timer--; //timer doesnt go up until you reach start position
				RotateTowards(player.Center, 4);
				if (NPC.velocity.Length() < 200 + (player.velocity.Length() / 2))
				{
					NPC.velocity += NPC.SafeDirectionTo(target) * 0.02f;
					NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * NPC.velocity.Length() * 1.05f, 0.7f);
				}
				else
				{
					NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * (20 + (player.velocity.Length() / 2)), 0.7f);

				}
			}
			else
			{

				NPC.velocity = Vector2.Lerp(NPC.velocity, (target - NPC.Center) / 10, 0.3f);
			}

			const int cd = 5;
			const int Flurry1Time = 20;
			const int Flurry2Time = 20 + 110;

			bool time = (Timer >= Flurry1Time && Timer <= Flurry1Time + (cd * 6)) || (Timer >= Flurry2Time && Timer <= Flurry2Time + (cd * 6));
			bool timeFirstHalf = (Timer >= Flurry1Time && Timer <= Flurry1Time + (cd * 3)) || (Timer >= Flurry2Time && Timer <= Flurry2Time + (cd * 3));
			if (Timer == Flurry1Time - 15) //choose side
			{
				AI4 = Main.rand.NextBool() ? 1 : -1;
				NPC.netUpdate = true;
			}
			if (Timer == Flurry2Time - 15)
			{
				AI4 = -AI4; //switch side
				NPC.netUpdate = true;
			}
			if (Timer == Flurry1Time - 15 || Timer == Flurry2Time - 15) //choose position
			{
				int ExtraDistance = 0;
				if (AI4 == Math.Sign(player.velocity.X))
				{
					ExtraDistance = (int)(player.velocity.X * 10);
				}
				LockVector2 = new Vector2(0, Main.rand.NextFloat(250 + ExtraDistance, 350 + ExtraDistance) * AI4);
				NPC.netUpdate = true;
			}
			if (Timer > Flurry1Time - 15)
			{
				int rotspeed = WorldSavingSystem.MasochistModeReal ? 15 : 10;
				RotateTowards(player.Center + LockVector2, rotspeed);
			}
			if (time)
			{
				Anim = 1;

				if (Timer % cd == cd - 2 && (!timeFirstHalf || (WorldSavingSystem.MasochistModeReal))) //shoot
				{
					SoundEngine.PlaySound(SoundID.Item63, NPC.Center);
					NPC.velocity -= NPC.rotation.ToRotationVector2() * 6;
					if (FargoSoulsUtil.HostCheck)
					{
						float speed = Main.rand.Next(10, 38);
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed * NPC.rotation.ToRotationVector2(), ModContent.ProjectileType<BaronMine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 2, player.whoAmI);
					}
				}
			}
			else
			{
				Anim = 0;
			}


			//int maxTime = WorldSavingSystem.MasochistModeReal ? 320 * 2 : 320;
			int maxTime = 320;
			if (Timer > maxTime)
			{
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		void P2Whirlpool()
		{
			Projectile arena = Main.projectile[ArenaProjID];
			bool arenaReal = arena != null && arena.active && arena.type == ModContent.ProjectileType<BaronArenaWhirlpool>();
			if (!arenaReal)
			{
				NPC.velocity = Vector2.Zero;
				StateReset();
				return;
			}

			if (Timer == 1)
			{
				if (DidWhirlpool)
				{
					DidWhirlpool = false;
					StateReset();
					return;
				}
				DidWhirlpool = true;
				Anim = 1;
				SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
				SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
				if (FargoSoulsUtil.HostCheck)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, NPC.whoAmI, -24);
				}
			}
			if (Timer == 61)
			{
				Anim = 0;
				arena.ai[2] = 1; //activate projectile shooting AI of arena
				arena.netUpdate = true;
			}
			if (Timer >= 61)
			{
				LockVector1 = Vector2.Normalize(NPC.Center - player.Center) * 500;

				Vector2 target = player.Center + LockVector1;
				if ((NPC.Distance(target) < 25 && AI3 == 0) || (Timer % 60 == 0 && AI3 != 0))
				{
					AI3 = Main.rand.NextFloat(-1, 1);
					NPC.netUpdate = true;
				}

				if (AI3 == 0)
				{
					if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
					{
						NPC.velocity += NPC.SafeDirectionTo(target) * 0.02f;
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * NPC.velocity.Length() * 1.05f, 0.3f);
					}
					else
					{
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * (20 + (player.velocity.Length() / 2)), 0.3f);

					}
				}
				else
				{
					NPC.velocity = Vector2.Lerp(NPC.velocity, (target - NPC.Center) / 10, 0.3f);
				}
			}
			RotateTowards(player.Center, 2f);
			if (Timer > 420)
			{
				arena.ai[2] = 0; //deactivate projectile shooting AI of arena
				arena.netUpdate = true;
			}
			int endTime = WorldSavingSystem.MasochistModeReal ? 560 : 500; //maso alternation makes it kinda bullshit otherwise
			if (Timer > endTime)
			{
				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		void P2LaserSweep()
		{
			const int PositioningTime = 30;
			const int WindupTime = 90;
			int AttackTime = 120;
			const int Endlag = 10;
			const int distance = 500;
			const float MaxRot = MathHelper.Pi / 5;

			if (Timer == 1)
			{

				//go towards center of arena so you don't get fucked by arena border and can't go around
				Projectile arena = Main.projectile[ArenaProjID];
				bool arenaReal = arena != null && arena.active && arena.type == ModContent.ProjectileType<BaronArenaWhirlpool>();
				if (!arenaReal)
				{
					LockVector1 = new Vector2(distance * Math.Sign(NPC.Center.X - player.Center.X), -distance * 0.7f);
				}
				else
				{
					int x = (int)Math.Min(Math.Abs(arena.Center.X - player.Center.X), distance) * Math.Sign(arena.Center.X - player.Center.X);
					LockVector1 = new Vector2(x, -distance * 0.7f);
					HitPlayer = false;
				}


			}
			if (Timer < PositioningTime && NPC.Distance(player.Center + LockVector1) > 15) //don't progress time if too far away from start position
			{
				Timer--;
			}
			else if (Timer < PositioningTime) //if close enough, start attack
			{
				Timer = PositioningTime;
				HitPlayer = true;
				if (WorldSavingSystem.EternityMode)
				{
					SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
					if (FargoSoulsUtil.HostCheck)
					{
						Vector2 vel = NPC.rotation.ToRotationVector2() * 10f;
						int nukeDur = Main.rand.Next(160, 160); //nuke duration
						NPC.netUpdate = true;
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -vel, ModContent.ProjectileType<BaronNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, nukeDur, player.whoAmI, 1);
					}
				}
			}
			if (Timer < PositioningTime) //get to position
			{
				Vector2 target = player.Center + LockVector1;
				if (NPC.velocity.Length() < 20 + (player.velocity.Length() / 2))
				{
					NPC.velocity += NPC.SafeDirectionTo(target) * 0.02f;
					NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * NPC.velocity.Length() * 1.05f, 0.75f);
				}
				else
				{
					NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(target) * (20 + (player.velocity.Length() / 2)), 0.75f);

				}
				RotateTowards(player.Center, 2);
			}
			else if (Timer < PositioningTime + WindupTime) //rotate with telegraph line
			{
				NPC.velocity *= 0.8f;
				if (Timer == PositioningTime)
				{
					SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/LaserTelegraph") with { Volume = 1.25f }, NPC.Center);
					AI3 = Main.rand.NextBool() ? 1 : -1; //sign for rotation
					LockVector1 = (player.Center - NPC.Center).RotatedBy(AI3 * MaxRot);
					if (FargoSoulsUtil.HostCheck)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloomLine>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 7, NPC.whoAmI, WindupTime + 20);
					}
					NPC.netUpdate = true;
				}
				RotateTowards(NPC.Center + LockVector1, 1.75f);
			}
			else if (Timer < PositioningTime + WindupTime + AttackTime) //rotate back with attack deathray
			{
				float RotationSpeed = WorldSavingSystem.EternityMode ? 1.1f : 1f;

				if (Timer == PositioningTime + WindupTime)
				{
					/*
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Vector2 vel = NPC.rotation.ToRotationVector2() * 10f;
                            int nukeDur = Main.rand.Next(160, 160); //nuke duration
                            NPC.netUpdate = true;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, -vel, ModContent.ProjectileType<BaronNuke>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, nukeDur, player.whoAmI, 1);
                        }
                    }
                    */
					//const float RotationFactor = 0.75f;


					AI4 = FargoSoulsUtil.RotationDifference(NPC.rotation.ToRotationVector2(), (player.Center - NPC.Center)); //cache rotation direction towards player

					SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/LaserSound_Slow") with { Pitch = -0.2f }, NPC.Center);
					if (FargoSoulsUtil.HostCheck)
					{
						Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.rotation.ToRotationVector2(), ModContent.ProjectileType<BaronDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, ai0: NPC.whoAmI, ai2: AttackTime);
					}
					NPC.netUpdate = true;
				}
				float rotDirection = Math.Sign(AI4);
				NPC.rotation += rotDirection * RotationSpeed * MathHelper.Pi / 180; //keep rotating in direction rotated first frame, no turning direction


			}
			else if (WorldSavingSystem.EternityMode && Timer > PositioningTime + WindupTime + AttackTime) // in emode, go right into predictive dash without endlag
			{
				StateReset();
			}
			else if (Timer > PositioningTime + WindupTime + AttackTime + Endlag)
			{

				NPC.velocity = Vector2.Zero;
				StateReset();
			}
		}
		#endregion
		#endregion
		#region Help Methods
		void StateReset(bool randomizeState = true)
		{
			NPC.TargetClosest(false);
			if (WorldSavingSystem.EternityMode && State == (int)StateEnum.P2LaserSweep) //combos into predictive dash in emode
			{
				State = (int)StateEnum.P2PredictiveDash;
				availablestates.Remove((int)StateEnum.P2LaserSweep);
				availablestates.Remove((int)StateEnum.P2PredictiveDash);
			}
			else if ((State == (int)StateEnum.Swim) || Phase == 2)
			{
				if (randomizeState)
					RandomizeState();
			}
			else
				State = (int)StateEnum.Swim;
			bool expertP2 = NPC.GetLifePercent() < (2f / 3) && Phase == 1 && Main.expertMode;
			bool nonexpertP2 = NPC.GetLifePercent() < 0.5f && Phase == 1 && !Main.expertMode;
			if (expertP2 || nonexpertP2)
			{
				State = (float)StateEnum.Phase2Transition;
			}
			Timer = 0;
			AI2 = 0;
			AI3 = 0;
		}
		void RandomizeState() //it's done this way so it cycles between attacks in a random order: for increased variety
		{
			int index;
			if (availablestates.Count < 1)
			{
				availablestates.Clear();
				if (Phase == 1)
				{
					availablestates.AddRange(P1Attacks);
				}
				else if (Phase == 2)
				{
					availablestates.AddRange(P2Attacks);
				}
				availablestates.Remove(LastState); //avoid possible repeat after refilling list
			}
			if (FargoSoulsUtil.HostCheck) //only run for host in mp, will sync to others
			{
				index = Main.rand.Next(availablestates.Count);
				State = availablestates[index];
				availablestates.RemoveAt(index);
			}
			LastState = (int)State;
			NPC.netUpdate = true;
		}
		bool AliveCheck(Player p, bool forceDespawn = false)
		{
			if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
			{
				NPC.TargetClosest();
				p = Main.player[NPC.target];
				if (forceDespawn || !p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > 5000f)
				{
					NPC.noTileCollide = true;
					if (NPC.timeLeft > 30)
						NPC.timeLeft = 30;
					NPC.velocity.Y += 1f;
					if (NPC.timeLeft == 1)
					{
						if (FargoSoulsUtil.HostCheck)
						{
							FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
						}
					}
					return false;
				}
			}
			if (NPC.timeLeft < 600)
				NPC.timeLeft = 600;

			return true;
		}
		void RotateTowards(Vector2 target, float speed)
		{
			Vector2 LV = NPC.rotation.ToRotationVector2();
			Vector2 PV = NPC.SafeDirectionTo(target);
			float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
			if (!(anglediff > 0 || anglediff < 0))
				return;
			//change rotation towards target
			NPC.rotation = NPC.rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
		}
		#endregion
	}
}