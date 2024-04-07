using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
	public abstract class PlanteraPart : EModeNPCBehaviour
	{
		public override void OnFirstTick(NPC npc)
		{
			base.OnFirstTick(npc);

			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.Venom] = true;
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
		}

		public override void LoadSprites(NPC npc, bool recolor)
		{
			base.LoadSprites(npc, recolor);

			LoadNPCSprite(recolor, npc.type);
		}
	}

	public class Plantera : PlanteraPart
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Plantera);

		// aiStyle = 51

		public int DicerTimer;
		public int RingTossTimer;
		public int TentacleTimer = 480; //line up first tentacles with ring toss lmao, 600

		public int CrystalRedirectTimer = 0;
		//public int TentacleTimerMaso;

		public float TentacleAttackAngleOffset;

		public bool IsVenomEnraged;
		public bool InPhase2;
		public bool EnteredPhase2;
		public bool EnteredPhase3;

		public bool DroppedSummon;


		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			base.SendExtraAI(npc, bitWriter, binaryWriter);

			binaryWriter.Write7BitEncodedInt(DicerTimer);
			binaryWriter.Write7BitEncodedInt(RingTossTimer);
			binaryWriter.Write7BitEncodedInt(TentacleTimer);
			binaryWriter.Write7BitEncodedInt(CrystalRedirectTimer);
			//binaryWriter.Write7BitEncodedInt(TentacleTimerMaso);
			bitWriter.WriteBit(IsVenomEnraged);
			bitWriter.WriteBit(InPhase2);
			bitWriter.WriteBit(EnteredPhase2);
			bitWriter.WriteBit(EnteredPhase3);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			base.ReceiveExtraAI(npc, bitReader, binaryReader);

			DicerTimer = binaryReader.Read7BitEncodedInt();
			RingTossTimer = binaryReader.Read7BitEncodedInt();
			TentacleTimer = binaryReader.Read7BitEncodedInt();
			CrystalRedirectTimer = binaryReader.Read7BitEncodedInt();
			//TentacleTimerMaso = binaryReader.Read7BitEncodedInt();
			IsVenomEnraged = bitReader.ReadBit();
			InPhase2 = bitReader.ReadBit();
			EnteredPhase2 = bitReader.ReadBit();
			EnteredPhase3 = bitReader.ReadBit();
		}
		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);
			npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.75f);
		}

		public override bool SafePreAI(NPC npc)
		{
			bool result = base.SafePreAI(npc);

			IsVenomEnraged = false;

			if (WorldSavingSystem.SwarmActive)
				return result;

			if (!EnteredPhase3 && (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000))
			{
				npc.velocity.Y++;
				npc.TargetClosest(false);
				if (!npc.HasValidTarget || npc.Distance(Main.player[npc.target].Center) > 3000)
				{
					if (npc.timeLeft > 60)
						npc.timeLeft = 60;
				}
			}

			Player player = Main.player[npc.target];

			const float innerRingDistance = 130f;
			const int delayForRingToss = 360 + 120;

			#region Phase 3
			if (!EnteredPhase3 && npc.GetLifePercent() < 0.25f)
			{
				EnteredPhase3 = true;
				SoundEngine.PlaySound(SoundID.Zombie21, npc.Center);


				npc.localAI[1] = 0;
				// these are unused but safeguarding anyway
				npc.ai[0] = 0;
				npc.ai[1] = 0;
				npc.ai[2] = 0;
				npc.ai[3] = 0;

				FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
				foreach (NPC n in Main.npc.Where(n => n.TypeAlive<CrystalLeaf>() && n.ai[0] == npc.whoAmI)) // delete crystal ring
				{
					n.life = 0;
					n.HitEffect();
					n.checkDead();
					n.active = false;
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
				}

				const int halfAmt = 20;
				for (int i = -halfAmt; i <= halfAmt; i++)
				{
					if (FargoSoulsUtil.HostCheck)
					{
						int type = Main.rand.NextFromList(ModContent.ProjectileType<PlanteraManEater>(), ModContent.ProjectileType<PlanteraSnatcher>(), ModContent.ProjectileType<PlanteraTrapper>());
						float offset = Main.rand.NextFloat(MathHelper.PiOver2 / halfAmt);
						Vector2 dir = Vector2.UnitY.RotatedBy(offset + MathHelper.PiOver2 * ((float)i / halfAmt));
						Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * 2000, -dir * 6,
							type, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, dir.ToRotation());
					}
				}
				//int ritual1 = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero,
				//ModContent.ProjectileType<PlanteraRitual>(), 0, 0f, Main.myPlayer, npc.lifeMax, npc.whoAmI);
			}
			if (EnteredPhase3)
			{

				ref float timer = ref npc.ai[0];
				ref float state = ref npc.ai[1];
				ref float movementTimer = ref npc.ai[2];
				ref float ai3 = ref npc.ai[3];


				//Targeting
				if (!player.active || player.dead || player.ghost || npc.Distance(player.Center) > 3000)
				{
					npc.TargetClosest(false);
					player = Main.player[npc.target];
					if (!player.active || player.dead || player.ghost || npc.Distance(player.Center) > 3000)
					{
						if (npc.timeLeft > 60)
							npc.timeLeft = 60;
						npc.velocity.Y -= 0.4f;
						return false;
					}
				}
				else
				{
					if (npc.timeLeft < 60)
						npc.timeLeft = 60;
				}

				if (FargoSoulsUtil.HostCheck && !Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
				{
					const int max = 5;
					float rotation = 2f * (float)Math.PI / max;
					for (int i = 0; i < max; i++)
					{
						Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(rotation * i);
						FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, rotation * i);
					}
				}

				EnsureInnerRingSpawned();

				npc.rotation = npc.SafeDirectionTo(player.Center).ToRotation() + MathHelper.PiOver2;

				#region Movement
				void Movement(Vector2 target, float speed, bool fastX = false)
				{
					float turnaroundModifier = 1f;
					float maxSpeed = 14f;


					if (Math.Abs(npc.Center.X - target.X) > 10)
					{
						if (npc.Center.X < target.X)
						{
							npc.velocity.X += speed;
							if (npc.velocity.X < 0)
								npc.velocity.X += speed * (fastX ? 2 : 1) * turnaroundModifier;
						}
						else
						{
							npc.velocity.X -= speed;
							if (npc.velocity.X > 0)
								npc.velocity.X -= speed * (fastX ? 2 : 1) * turnaroundModifier;
						}
					}
					if (npc.Center.Y < target.Y)
					{
						npc.velocity.Y += speed;
						if (npc.velocity.Y < 0)
							npc.velocity.Y += speed * 2 * turnaroundModifier;
					}
					else
					{
						npc.velocity.Y -= speed;
						if (npc.velocity.Y > 0)
							npc.velocity.Y -= speed * 2 * turnaroundModifier;
					}

					if (Math.Abs(npc.velocity.X) > maxSpeed)
						npc.velocity.X = maxSpeed * Math.Sign(npc.velocity.X);
					if (Math.Abs(npc.velocity.Y) > maxSpeed)
						npc.velocity.Y = maxSpeed * Math.Sign(npc.velocity.Y);

					//if (fastX && Math.Sign(npc.velocity.X) != Math.Sign(target.X - npc.Center.X))
					//npc.velocity.X = 0;
				}

				if (state == 0) // Phase transition movement, go up while avoiding player
				{
					Vector2 playerToNPC = (npc.Center - player.Center);
					float distX;
					if (Math.Sign(playerToNPC.Y) > -10)
						distX = Utils.Clamp(Math.Abs(playerToNPC.X), 300, 500) * Math.Sign(playerToNPC.X);
					else
						distX = 0;
					float targetX = player.Center.X + Math.Sign(playerToNPC.X) * distX;
					float distY = 50;
					float targetY = player.Center.Y - distY;
					Vector2 targetPos = Vector2.UnitX * targetX + Vector2.UnitY * targetY;


					if (playerToNPC.Y > -distY)
					{
						Movement(targetPos, 0.3f, true);
						if (timer > 50)
							timer--;

						if (Math.Abs(playerToNPC.Y) < Math.Abs(playerToNPC.X)) //anti go into you and fuck you when going up
						{
							npc.velocity.Y *= 0.95f;
							npc.velocity.X += Math.Sign(playerToNPC.X) * 0.5f;
						}
					}
					else
					{
						npc.velocity *= 0.96f;
					}
					if (timer > 60)
					{
						timer = 0;
						state = 1;
						npc.TargetClosest(false);
					}
				}
				else
				{
					movementTimer++;
				}

				void WallHugMovement(bool fastX = false, float speedMult = 1, float heightMult = 1, float targetPosX = 0)
				{
					ref float movementTimer = ref npc.ai[2];

					int searchWidth = 100;
					int searchHeight = 200 + 120 * (int)MathF.Sin(MathHelper.TwoPi * movementTimer / (60 * 8.35f));
					searchHeight = (int)(searchHeight * heightMult);
					bool collisionAbove = Collision.SolidCollision(npc.Center - Vector2.UnitX * searchWidth / 2 - Vector2.UnitY * searchHeight, searchWidth, searchHeight);

					if (!Collision.CanHitLine(npc.Center, 0, 0, player.Center, 0, 0)) // if collision between you and player, and are above, go down
					{
						collisionAbove = true;
					}
					if (player.Center.X - npc.Center.X > 900) //cap it
					{
						collisionAbove = true;
					}
					float speedY;
					if (collisionAbove && player.Center.Y - npc.Center.Y > 150)
					{
						speedY = 1;
					}
					else
					{
						speedY = -1;
					}
					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						float targetY = player.Center.Y - 150;
						speedY = Math.Sign(targetY - npc.Center.Y);
					}
					float targetX = player.Center.X + 130 * MathF.Sin(MathHelper.TwoPi * movementTimer / (60 * 5));
					if (targetPosX != 0)
					{
						targetX = targetPosX;
					}
					float speedX = Math.Sign(targetX - npc.Center.X);

					Vector2 targetPos = Vector2.UnitX * (npc.Center.X + speedX * 50) + Vector2.UnitY * (npc.Center.Y + speedY * 60);
					Movement(targetPos, 0.15f * speedMult, fastX);
					//npc.velocity += Vector2.UnitX * speedX * mod + Vector2.UnitY * speedY * mod;
				}
				#endregion
				const int scanWidth = 500;
				bool collisionLeft = Collision.SolidTiles(npc.Center - Vector2.UnitX * scanWidth, scanWidth, npc.height);
				bool collisionRight = Collision.SolidTiles(npc.Center, scanWidth, npc.height);
				float playerXAvoidWalls = player.Center.X;
				if (collisionLeft && !collisionRight)
					playerXAvoidWalls += 500;
				if (!collisionLeft && collisionRight)
					playerXAvoidWalls -= 500;

				//repulsed by player whenever too close
				const float minDist = 250;
				float distance = npc.Distance(Main.player[npc.target].Center);
				if (npc.HasValidTarget && distance < minDist)
				{
					if (!(npc.ai[1] == 1f && npc.ai[2] > 2f) || npc.ai[1] == 2) // when not spinning or dg phase
					{
						float pushStrength = 1f * (1 - distance / minDist);
						npc.velocity -= pushStrength * npc.SafeDirectionTo(Main.player[npc.target].Center);
					}
				}

				#region Attacks
				switch (state) // ATTACKS
				{
					case 1: // crystal madness
						{
							if (timer < 60 * 6)
								WallHugMovement();
							else
								WallHugMovement(true, 4, 0.1f, playerXAvoidWalls);



							const int shotTime = 17;
							if (timer % shotTime == shotTime - 1 && timer < 60 * 6)
							{
								foreach (NPC leaf in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
								{
									SoundEngine.PlaySound(SoundID.Grass, leaf.Center);
									if (FargoSoulsUtil.HostCheck)
									{
										Vector2 dir = npc.SafeDirectionTo(leaf.Center);
										Projectile.NewProjectile(Entity.InheritSource(leaf), leaf.Center, 7f * dir, ModContent.ProjectileType<CrystalLeafShot>(),
											FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, ai0: npc.whoAmI);
									}
								}
							}

							if (timer == 60 * 6) // redirect
							{
								SoundEngine.PlaySound(SoundID.Zombie21 with { Pitch = -0.3f }, npc.Center);
								bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
								Color color = recolor ? Color.DeepSkyBlue : Color.LimeGreen;
								Color color2 = recolor ? Color.DarkBlue : Color.ForestGreen;
								Particle particle = new ExpandingBloomParticle(npc.Center, Vector2.Zero, color, Vector2.Zero, Vector2.One * 100f, 20, true, color2);
								particle.Spawn();

								foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == ModContent.ProjectileType<CrystalLeafShot>() && p.ai[0] == npc.whoAmI)) //my crystal leaves
								{
									p.ai[1] = 1;
									p.ai[2] = player.whoAmI;
									p.netUpdate = true;
								}
							}
							if (timer >= 60 * 9f)
							{
								timer = 0;
								state = 2;
								npc.TargetClosest(false);
							}
						}
						break;
					case 2:  // vine funnels
						{
							ref float repeatCheck = ref npc.localAI[1];

							const int vineSpawnTime = 100;

							if (timer >= vineSpawnTime || WorldSavingSystem.MasochistModeReal)
							{
								if (timer % 50 == 0)
								{
									SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
									if (FargoSoulsUtil.HostCheck)
									{
										float spreadAngle = WorldSavingSystem.MasochistModeReal ? 0.18f : 0.27f;
										int spreadAmount = WorldSavingSystem.MasochistModeReal ? 3 : 2;
										for (int i = -spreadAmount; i <= spreadAmount; i++)
										{
											float angle = npc.SafeDirectionTo(player.Center).ToRotation();
											float speed = 1;
											angle += i * MathHelper.PiOver2 * spreadAngle;
											Vector2 dir = angle.ToRotationVector2();
											Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.width / 2f, dir * speed,
												ModContent.ProjectileType<PlanteraThornChakram>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 4);
										}
										for (int i = -spreadAmount; i <= spreadAmount + 1; i++)
										{
											float x = i - 0.5f;
											float angle = npc.SafeDirectionTo(player.Center).ToRotation();
											float speed = 2;
											angle += x * MathHelper.PiOver2 * spreadAngle;
											Vector2 dir = angle.ToRotationVector2();
											Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.width / 2f, dir * speed,
												ModContent.ProjectileType<PlanteraThornChakram>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 8);
										}
									}
								}
							}

							if (timer < vineSpawnTime)
							{

								float vineProgress = timer / vineSpawnTime;

								if (repeatCheck == 0)
								{
									const int scanWidth2 = 500;
									bool scanLeft = Collision.SolidTiles(npc.Center - Vector2.UnitX * scanWidth2, scanWidth2, npc.height);
									bool scanRight = Collision.SolidTiles(npc.Center, scanWidth2, npc.height);
									if (timer == 0)
									{
										ai3 = Math.Sign(npc.Center.X - player.Center.X);
										if (scanLeft && !scanRight)
											ai3 = 1;
										if (!scanLeft && scanRight)
											ai3 = -1;
									}
									if (ai3 == 0)
										ai3 = Main.rand.NextBool() ? 1 : -1;
								}

								if (timer < vineSpawnTime * 0.7f && !(repeatCheck == 1 && timer < vineSpawnTime * 0.6f))
								{
									WallHugMovement(true, 4, 0.1f, playerXAvoidWalls);
								}
								else
									npc.velocity *= 0.96f;

								float side = ai3 * (repeatCheck == 1 ? -1 : 1);
								float attackAngle = Vector2.Lerp(Vector2.UnitX * side, Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * 0.15f * side), vineProgress).ToRotation();

								const int freq = 5;
								if (timer % freq == freq - 1)
								{
									if (FargoSoulsUtil.HostCheck)
									{
										Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, attackAngle.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * 24,
											ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle);
									}
								}
							}
							else
							{
								npc.velocity *= 0.96f;
								if (timer > vineSpawnTime * (repeatCheck == 1 ? 4.4f : 3.9f))
								{

									timer = 0;
									if (repeatCheck == 0)
									{
										repeatCheck = 1;
									}
									else
									{
										repeatCheck = 0;
										state = 3;
										npc.TargetClosest(false);
									}
								}
							}

						}
						break;
					case 3: // cone shots
						{
							const int vineSpawnTime = 100;

							if (timer < vineSpawnTime)
							{

								float vineProgress = timer / vineSpawnTime;

								if (timer < vineSpawnTime * 0.7f)
								{
									WallHugMovement(true, 4, 1, playerXAvoidWalls);
								}
								else
									npc.velocity *= 0.96f;

								for (int i = -1; i <= 1; i += 2)
								{
									float attackAngle = Vector2.Lerp(-Vector2.UnitY.RotatedBy(-i * MathHelper.PiOver2 * 0.1f), Vector2.UnitY.RotatedBy(i * MathHelper.PiOver2 * 0.3f), vineProgress).ToRotation();


									const int freq = 5;
									if (timer % freq == freq - 1)
									{
										if (FargoSoulsUtil.HostCheck)
										{
											Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, attackAngle.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * 24,
												ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle);
										}
									}
								}
							}
							else
							{
								//WallHugMovement(true, 1, 1);
								npc.velocity *= 0.96f;
								/*
                                if (timer % 200 == 0)
                                {
                                    float attackAngle = npc.SafeDirectionTo(player.Center).ToRotation();
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, attackAngle.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * 24,
                                            ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle);
                                        if (p.IsWithinBounds(Main.maxProjectiles))
                                        {
                                            Main.projectile[p].extraUpdates += 1;
                                        }
                                    }
                                }
                                */
								int freq = WorldSavingSystem.MasochistModeReal ? 9 : 14;
								if (timer % freq == 0 && (timer > vineSpawnTime || WorldSavingSystem.MasochistModeReal))
								{
									if (timer % (freq * 4) <= freq * 2)
									{
										Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, npc.SafeDirectionTo(player.Center),
											ModContent.ProjectileType<PlanteraMushroomThing>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
									}
								}
								if (timer == vineSpawnTime * 5 - 60) // kill and instantly respawn ring, 1 second before attack1 starts
								{
									for (int i = 0; i < Main.maxNPCs; i++)
									{
										if (Main.npc[i].TypeAlive<CrystalLeaf>() && Main.npc[i].ai[0] == npc.whoAmI)
										{
											Main.npc[i].StrikeInstantKill();
										}
									}
								}
								if (timer > vineSpawnTime * 5)
								{
									timer = 0;
									state = 1;
									npc.TargetClosest(false);
								}
							}
						}
						break;
				}
				#endregion

				timer++;
				return false;
			}
			#endregion

			void EnsureInnerRingSpawned()
			{
				if (FargoSoulsUtil.HostCheck && !Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
				{
					const int max = 5;
					float rotation = 2f * (float)Math.PI / max;
					for (int i = 0; i < max; i++)
					{
						Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(rotation * i);
						FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, rotation * i);
					}
				}
			}
			if (--RingTossTimer < 0)
			{
				RingTossTimer = delayForRingToss;
				EnsureInnerRingSpawned();
			}
			else if (RingTossTimer == 120)
			{

				//if (WorldSavingSystem.MasochistModeReal)
				//    RingTossTimer = 0; //instantly spawn next set of crystals

				npc.netUpdate = true;
				NetSync(npc);

				if (FargoSoulsUtil.HostCheck) // do ring toss
				{
					float speed = 8f;
					Vector2 direction;
					if (WorldSavingSystem.MasochistModeReal)
					{
						direction = FargoSoulsUtil.PredictiveAim(npc.Center, player.Center, player.velocity, speed);
						direction.Normalize();
					}
					else
					{
						direction = npc.SafeDirectionTo(player.Center);
					}
					int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed * direction, ModContent.ProjectileType<MutantMark2>(), npc.defDamage / 4, 0f, Main.myPlayer);
					if (p != Main.maxProjectiles)
					{
						Main.projectile[p].timeLeft -= 300;

						foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance)) //my crystal leaves
						{
							SoundEngine.PlaySound(SoundID.Grass, n.Center);
							Projectile.NewProjectile(npc.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<PlanteraCrystalLeafRing>(), npc.defDamage / 4, 0f, Main.myPlayer, Main.projectile[p].identity, n.ai[3]);

							n.life = 0;
							n.HitEffect();
							n.checkDead();
							n.active = false;
							if (Main.netMode == NetmodeID.Server)
								NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
						}
					}
				}

			}
			if (!InPhase2) // redirect attack
			{
				if (RingTossTimer == 360 + 60)
				{
					if (CrystalRedirectTimer >= 2) // every 3 throws, redirect instead of throwing
					{
						SoundEngine.PlaySound(SoundID.Zombie21 with { Pitch = -0.3f }, npc.Center);
						bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
						Color color = recolor ? Color.DeepSkyBlue : Color.LimeGreen;
						Color color2 = recolor ? Color.DarkBlue : Color.ForestGreen;
						Particle particle = new ExpandingBloomParticle(npc.Center, Vector2.Zero, color, Vector2.Zero, Vector2.One * 100f, 20, true, color2);
						particle.Spawn();

						foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == ModContent.ProjectileType<CrystalLeafShot>() && p.ai[0] == npc.whoAmI)) //my crystal leaves
						{
							p.ai[1] = 1;
							p.ai[2] = player.whoAmI;
							p.netUpdate = true;
						}
						CrystalRedirectTimer = 0;
						npc.netUpdate = true;
					}
					else
					{
						CrystalRedirectTimer++;
						npc.netUpdate = true;
					}

				}
				if (RingTossTimer.IsWithinBounds(360 - 60, 360 + 60) && CrystalRedirectTimer == 0 && !EnteredPhase2) // For 2 seconds after doing redirect
				{
					npc.velocity *= 0.96f;
					npc.localAI[1] = 0; // Don't fire vanilla projectiles
				}
			}

			if (npc.life > npc.lifeMax / 2)
			{
				if (--DicerTimer < 0)
				{
					DicerTimer = 150 * 4 + 25;
					if (WorldSavingSystem.MasochistModeReal && npc.HasValidTarget && FargoSoulsUtil.HostCheck)
					{
						Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, Vector2.Zero, ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 0, 0);
						for (int i = 0; i < 3; i++)
						{
							Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center, 30f * npc.SafeDirectionTo(Main.player[npc.target].Center).RotatedBy(2 * (float)Math.PI / 3 * i),
							  ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 1, 1);
						}
					}
				}
			}
			else
			{
				if (!InPhase2)
				{
					InPhase2 = true;
					DicerTimer = 0;
				}

				void SpawnOuterLeafRing()
				{
					const int max = 12;
					const float distance = 250;
					float rotation = 2f * (float)Math.PI / max;
					for (int i = 0; i < max; i++)
					{
						Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
						FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, distance, 0, rotation * i);
					}
				}
				if (!EnteredPhase2)
				{
					EnteredPhase2 = true;

					if (FargoSoulsUtil.HostCheck)
					{
						if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] == innerRingDistance))
						{
							const int innerMax = 5;
							float innerRotation = 2f * (float)Math.PI / innerMax;
							for (int i = 0; i < innerMax; i++)
							{
								Vector2 spawnPos = npc.Center + new Vector2(innerRingDistance, 0f).RotatedBy(innerRotation * i);
								FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), spawnPos, ModContent.NPCType<CrystalLeaf>(), 0, npc.whoAmI, innerRingDistance, 0, innerRotation * i);
							}
						}

						SpawnOuterLeafRing();
					}
					DespawnProjs();
				}

				//explode time * explode repetitions + spread delay * propagations
				const int delayForDicers = 150 * 4 + 25 * 8;

				if (--DicerTimer < -120)
				{
					DicerTimer = delayForDicers + delayForRingToss + 240;
					//Counter3 = delayForDicers + 120; //extra compensation for the toss offset

					npc.netUpdate = true;
					NetSync(npc);

					if (FargoSoulsUtil.HostCheck)
					{
						Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer);
						for (int i = 0; i < 3; i++)
						{
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 25f * npc.SafeDirectionTo(Main.player[npc.target].Center).RotatedBy(2 * (float)Math.PI / 3 * i),
							  ModContent.ProjectileType<DicerPlantera>(), npc.defDamage / 4, 0f, Main.myPlayer, 1, 8);
						}
					}
				}

				if (DicerTimer > delayForDicers || DicerTimer < 0)
				{
					if (RingTossTimer > 120) //to still respawn the leaf ring if it's missing but disable throwing it
						RingTossTimer = 120;
				}
				else if (DicerTimer < delayForDicers)
				{
					RingTossTimer -= 1;

					if (RingTossTimer % 2 == 0) //make sure plantera can get the timing for its check
						RingTossTimer--;
				}
				else if (DicerTimer == delayForDicers)
				{
					RingTossTimer = 121; //activate it immediately as the mines fade
				}

				IsVenomEnraged = npc.HasPlayerTarget && Main.player[npc.target].venom;

				if (--TentacleTimer <= 0)
				{
					float slowdown = Math.Min(0.9f, -TentacleTimer / 60f);
					if (WorldSavingSystem.MasochistModeReal && slowdown > 0.75f)
						slowdown = 0.75f;
					npc.position -= npc.velocity * slowdown;

					if (TentacleTimer == 0)
					{
						TentacleAttackAngleOffset = Main.rand.NextFloat(MathHelper.TwoPi);

						SoundEngine.PlaySound(SoundID.Roar, npc.Center);

						npc.netUpdate = true;
						NetSync(npc);

						foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<CrystalLeaf>() && n.ai[0] == npc.whoAmI && n.ai[1] > innerRingDistance)) //my crystal leaves
						{
							SoundEngine.PlaySound(SoundID.Grass, n.Center);

							n.life = 0;
							n.HitEffect();
							n.checkDead();
							n.active = false;
							if (Main.netMode == NetmodeID.Server)
								NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n.whoAmI);
						}
					}

					const int maxTime = 30;
					const int interval = 3;
					float maxDegreeCoverage = 45f; //on either side of the middle, the full coverage of one side is x2 this
					if (TentacleTimer >= -maxTime && TentacleTimer % interval == 0)
					{
						int tentacleSpawnOffset = Math.Abs(TentacleTimer) / interval;
						for (int i = -tentacleSpawnOffset; i <= tentacleSpawnOffset; i += tentacleSpawnOffset * 2)
						{
							float attackAngle = MathHelper.WrapAngle(
								TentacleAttackAngleOffset
								+ MathHelper.ToRadians(maxDegreeCoverage / (maxTime / interval)) * (i + Main.rand.NextFloat(-0.5f, 0.5f))
							);

							if (FargoSoulsUtil.HostCheck)
							{
								Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
									ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle);
								Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
									ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, attackAngle + MathHelper.Pi);
							}

							if (i == 0)
								break;
						}
					}

					if (TentacleTimer < -390)
					{
						TentacleTimer = 600 + Main.rand.Next(120);

						if (!WorldSavingSystem.MasochistModeReal)
							npc.velocity = Vector2.Zero;

						npc.netUpdate = true;
						NetSync(npc);

						SpawnOuterLeafRing();
					}
				}
				else
				{
					npc.position -= npc.velocity * (IsVenomEnraged ? 0.1f : 0.2f);
				}

				//if (WorldSavingSystem.MasochistModeReal && --TentacleTimerMaso < 0)
				//{
				//    TentacleTimerMaso = 420;
				//    if (FargoSoulsUtil.HostCheck)
				//    {
				//        float angle = npc.SafeDirectionTo(Main.player[npc.target].Center).ToRotation();
				//        for (int i = -1; i <= 1; i++)
				//        {
				//            float offset = MathHelper.ToRadians(6) * i;
				//            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
				//              ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, angle + offset);
				//            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Main.rand.NextVector2CircularEdge(24, 24),
				//                ModContent.ProjectileType<PlanteraTentacle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, -angle + offset);
				//        }
				//    }

				//}
			}

			EModeUtils.DropSummon(npc, "PlanterasFruit", NPC.downedPlantBoss, ref DroppedSummon);

			void DespawnProjs()
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (Main.projectile[i].active && Main.projectile[i].hostile &&
						(Main.projectile[i].type == ProjectileID.ThornBall
						|| Main.projectile[i].type == ModContent.ProjectileType<DicerPlantera>()
						|| Main.projectile[i].type == ModContent.ProjectileType<PlanteraCrystalLeafRing>()
						|| Main.projectile[i].type == ModContent.ProjectileType<CrystalLeafShot>()))
					{
						Main.projectile[i].Kill();
					}
				}
			}

			return result;
		}

		public override void SafePostAI(NPC npc)
		{
			base.SafePostAI(npc);

			npc.defense = Math.Max(npc.defense, npc.defDefense);
		}

		public override Color? GetAlpha(NPC npc, Color drawColor)
		{
			return !IsVenomEnraged ? base.GetAlpha(npc, drawColor) : new Color(255, drawColor.G / 2, drawColor.B / 2);
		}
		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
			if (EnteredPhase3)
			{
				Vector2 drawPos = npc.Center - screenPos;

				Color glowColor = recolor ? Color.Blue : Color.Green;
				for (int j = 0; j < 12; j++)
				{
					Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;

					spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, drawPos + afterimageOffset, npc.frame, glowColor, npc.rotation, npc.frame.Size() * 0.5f, npc.scale, SpriteEffects.None, 0f);
				}
			}
			return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
		}
		public static float DR(NPC npc) =>
			npc.GetLifePercent() < 0.25f ? 0.4f // phase 3
			: npc.GetLifePercent() < 0.5f ? 0.5f // phase 2
			: 0; // phase 1
		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage *= 1 - DR(npc);
		}
		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			npc.lifeRegen = (int)Math.Round(npc.lifeRegen * (1 - DR(npc)));
		}

		public override void LoadSprites(NPC npc, bool recolor)
		{
			base.LoadSprites(npc, recolor);

			LoadBossHeadSprite(recolor, 11);
			LoadBossHeadSprite(recolor, 12);
			LoadGoreRange(recolor, 378, 391);
			LoadSpecial(recolor, ref TextureAssets.Chain26, ref FargowiltasSouls.TextureBuffer.Chain26, "Chain26");
			LoadSpecial(recolor, ref TextureAssets.Chain27, ref FargowiltasSouls.TextureBuffer.Chain27, "Chain27");
			LoadProjectile(recolor, ProjectileID.SeedPlantera);
			LoadProjectile(recolor, ProjectileID.PoisonSeedPlantera);
			LoadProjectile(recolor, ProjectileID.ThornBall);
		}
	}

	public class PlanterasHook : PlanteraPart
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PlanterasHook);

		public override bool SafePreAI(NPC npc)
		{
			bool result = base.SafePreAI(npc);

			if (WorldSavingSystem.SwarmActive)
				return result;

			npc.damage = 0;
			npc.defDamage = 0;

			NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
			if (plantera != null && plantera.life < plantera.lifeMax / 2 && plantera.HasValidTarget)
			{
				if (npc.Distance(Main.player[plantera.target].Center) > 600)
				{
					Vector2 targetPos = Main.player[plantera.target].Center / 16; //pick a new target pos near player
					targetPos.X += Main.rand.Next(-25, 26);
					targetPos.Y += Main.rand.Next(-25, 26);

					Tile tile = Framing.GetTileSafely((int)targetPos.X, (int)targetPos.Y);
					npc.localAI[0] = 600; //reset vanilla timer for picking new block
					if (FargoSoulsUtil.HostCheck)
						npc.netUpdate = true;

					npc.ai[0] = targetPos.X;
					npc.ai[1] = targetPos.Y;
				}

				if (npc.Distance(new Vector2(npc.ai[0] * 16 + 8, npc.ai[1] * 16 + 8)) > 32)
					npc.position += npc.velocity;
			}

			return result;
		}
	}

	public class PlanterasTentacle : PlanteraPart
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PlanterasTentacle);

		public int ChangeDirectionTimer;
		public int RotationDirection;
		public int MaxDistanceFromPlantera;
		public int CanHitTimer;

		public bool DroppedSummon;

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			base.SendExtraAI(npc, bitWriter, binaryWriter);

			binaryWriter.Write7BitEncodedInt(ChangeDirectionTimer);
			binaryWriter.Write7BitEncodedInt(RotationDirection);
			binaryWriter.Write7BitEncodedInt(MaxDistanceFromPlantera);
			binaryWriter.Write7BitEncodedInt(CanHitTimer);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			base.ReceiveExtraAI(npc, bitReader, binaryReader);

			ChangeDirectionTimer = binaryReader.Read7BitEncodedInt();
			RotationDirection = binaryReader.Read7BitEncodedInt();
			MaxDistanceFromPlantera = binaryReader.Read7BitEncodedInt();
			CanHitTimer = binaryReader.Read7BitEncodedInt();
		}

		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);

			MaxDistanceFromPlantera = 200;
		}

		public override bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot)
		{
			return base.CanHitPlayer(npc, target, ref CooldownSlot) && CanHitTimer > 60;
		}

		public override bool SafePreAI(NPC npc)
		{
			bool result = base.SafePreAI(npc);

			if (WorldSavingSystem.SwarmActive)
				return result;

			NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
			if (plantera != null)
			{
				npc.position += plantera.velocity / 3;
				if (npc.Distance(plantera.Center) > MaxDistanceFromPlantera) //snap back in really fast if too far
				{
					Vector2 vel = plantera.Center - npc.Center;
					vel += MaxDistanceFromPlantera * plantera.DirectionFrom(npc.Center).RotatedBy(MathHelper.ToRadians(45) * RotationDirection);
					npc.velocity = Vector2.Lerp(npc.velocity, vel / 15, 0.05f);
				}
			}

			if (++ChangeDirectionTimer > 120)
			{
				ChangeDirectionTimer = Main.rand.Next(30);
				if (FargoSoulsUtil.HostCheck)
				{
					RotationDirection = Main.rand.NextBool() ? -1 : 1;
					MaxDistanceFromPlantera = 50 + Main.rand.Next(150);
					npc.netUpdate = true;
					NetSync(npc);
				}
			}

			++CanHitTimer;

			return result;
		}

		public override void LoadSprites(NPC npc, bool recolor)
		{
			base.LoadSprites(npc, recolor);

			LoadNPCSprite(recolor, npc.type);
		}
	}

	public class Spore : PlanteraPart
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Spore);
	}
}
