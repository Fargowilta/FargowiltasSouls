using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.Bosses.VanillaEternity
{
	public class SkeletronHead : EModeNPCBehaviour
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronHead);

		public int ReticleTarget;
		public int BabyGuardianTimer;
		public bool DGDaytime;
		public int DGSpeedRampup;
		public int MasoArmsTimer;

		public bool InPhase2;

		public bool DroppedSummon;
		public bool SpawnedArms;
		public bool HasSaidEndure;
		public bool FirstCycle;

		public int SpawnGrace;
		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);
			npc.damage = (int)(npc.damage * 1.15f);
			npc.lifeMax = (int)(npc.lifeMax * 0.8f);
		}
		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			base.SendExtraAI(npc, bitWriter, binaryWriter);

			binaryWriter.Write7BitEncodedInt(ReticleTarget);
			binaryWriter.Write7BitEncodedInt(BabyGuardianTimer);
			binaryWriter.Write7BitEncodedInt(DGSpeedRampup);
			bitWriter.WriteBit(InPhase2);
			binaryWriter.Write7BitEncodedInt(SpawnGrace);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			base.ReceiveExtraAI(npc, bitReader, binaryReader);

			ReticleTarget = binaryReader.Read7BitEncodedInt();
			BabyGuardianTimer = binaryReader.Read7BitEncodedInt();
			DGSpeedRampup = binaryReader.Read7BitEncodedInt();
			InPhase2 = bitReader.ReadBit();
			SpawnGrace = binaryReader.Read7BitEncodedInt();
		}

		public override void OnSpawn(NPC npc, IEntitySource source)
		{
			if (WorldSavingSystem.SwarmActive || !WorldSavingSystem.EternityMode)
				return;
			SpawnGrace = 60 * 4;
		}
		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => SpawnGrace > 0 ? false : base.CanHitPlayer(npc, target, ref cooldownSlot);
		static int BabyGuardianTimerRefresh(NPC npc) => !WorldSavingSystem.MasochistModeReal && NPC.AnyNPCs(NPCID.SkeletronHand) && npc.life > npc.lifeMax * 0.25 ? 240 : 180;

		void GrowHands(NPC npc, bool secondSet = false)
		{
			for (int i = -1; i < 2; i += 2) //two hands: one -1 and one 1
			{
				int n = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.SkeletronHand, npc.whoAmI, i, npc.whoAmI, 0f, 0f, npc.target);
				NPC hand = Main.npc[n];
				if (secondSet && n != Main.maxNPCs)
				{
					hand.GetGlobalNPC<SkeletronHand>().secondSet = true;
				}
				if (npc.ai[1] == 1f || npc.ai[1] == 2f) //spinning or DG mode
				{

					//make them go to minimum clap distance
					hand.localAI[2] = hand.width + npc.width;
					hand.GetGlobalNPC<SkeletronHand>().AttackTimer = SkeletronHand.GuardianTime + SkeletronHand.GuardianDelay;
				}
			}

			FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.NPCs.EMode.RegrowArms", new Color(175, 75, 255), npc.FullName);
		}

		public override bool SafePreAI(NPC npc)
		{
			bool result = base.SafePreAI(npc);

			EModeGlobalNPC.skeleBoss = npc.whoAmI;

			if (WorldSavingSystem.SwarmActive)
				return result;

			if (SpawnGrace > 0)
				SpawnGrace--;

			if (ArmDR(npc))
			{
				npc.HitSound = SoundID.NPCHit4;
			}
			else
			{
				npc.HitSound = SoundID.NPCHit2;
			}
			if (!SpawnedArms && npc.life < npc.lifeMax * .5)
			{
				if (NPC.AnyNPCs(NPCID.SkeletronHand)) //don't go below half health if first arms are still alive
				{
					npc.life = (int)Math.Round(npc.lifeMax * 0.5f) + 10;
				}
				else
				{
					SpawnedArms = true;
					GrowHands(npc);
					if (WorldSavingSystem.MasochistModeReal)
					{
						GrowHands(npc, true);
					}
				}
			}
			if (npc.ai[1] == 0f)
			{
				if (npc.ai[2] == 800 - 90) //telegraph spin
				{
					if (FargoSoulsUtil.HostCheck && !WorldSavingSystem.MasochistModeReal)
						Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<TargetingReticle>(), 0, 0f, Main.myPlayer, npc.whoAmI, npc.type);
				}
				if (npc.ai[2] < 800 - 5)
				{
					ReticleTarget = npc.target;
				}
			}

			if (npc.ai[1] == 1f || npc.ai[1] == 2f) //spinning or DG mode
			{
				//only runs once per spin
				if (ReticleTarget > -1 && ReticleTarget < Main.maxPlayers)
				{
					//ensure consistency
					int threshold = BabyGuardianTimerRefresh(npc);
					if (BabyGuardianTimer > threshold)
						BabyGuardianTimer = threshold;

					//force targeted player back to the one i telegraphed with reticle (otherwise, may target another player when spin starts)
					npc.target = ReticleTarget;
					ReticleTarget = -1;

					npc.netUpdate = true;
					NetSync(npc);

					if (!npc.HasValidTarget)
						npc.TargetClosest(false);

					if (npc.ai[1] == 1)
					{
						FirstCycle = true;
						CrossGuardianAttack(npc);
					}
				}

				float ratio = (float)npc.life / npc.lifeMax;
				float cooldown = 20f;
				if (!WorldSavingSystem.MasochistModeReal)
					cooldown += 100f * ratio;
				if (++npc.localAI[2] >= cooldown) //spray bones
				{
					npc.localAI[2] = 0f;
					if (cooldown > 0 && npc.HasPlayerTarget && FargoSoulsUtil.HostCheck && (!NPC.AnyNPCs(NPCID.SkeletronHand) || npc.ai[1] == 2f))
					{
						Vector2 speed = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 6f;
						for (int i = 0; i < 8; i++)
						{
							Vector2 vel = speed.RotatedBy(Math.PI * 2 / 8 * i);
							vel += npc.velocity * (1f - ratio);
							vel.Y -= Math.Abs(vel.X) * 0.2f;
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<SkeletronBone>(), npc.defDamage / 9 * 2, 0f, Main.myPlayer);
						}
					}
				}

				if (npc.life < npc.lifeMax * .75 && npc.ai[1] == 1f && --BabyGuardianTimer < 0)
				{
					BabyGuardianTimer = BabyGuardianTimerRefresh(npc);

					SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);

					if (FargoSoulsUtil.HostCheck)
					{
						SprayHomingBabies(npc);

						if (WorldSavingSystem.MasochistModeReal && !NPC.AnyNPCs(NPCID.SkeletronHand) && !FirstCycle) //skip the first cycle so you never get fucked by Double Cross Guardian Attack
							DungeonGuardianAttack(npc);
					}
					FirstCycle = false;
				}
			}
			else
			{

				if (npc.ai[2] == 0)
				{
					//compensate for not changing targets when beginning spin
					npc.TargetClosest(false);

					//prevent skeletron from firing his stupid tick 1 no telegraph skull right after finishing spin
					//if (!WorldSavingSystem.MasochistModeReal)
					npc.ai[2] = 1;
				}

				if (npc.life < npc.lifeMax * .75) //phase 2
				{
					/*
                    //vomit skeletons
                    if (npc.ai[2] <= 60 && npc.ai[2] % 15 == 0 && !NPC.AnyNPCs(NPCID.SkeletronHand))
                    {
                        int[] skeletons = {
                            NPCID.BoneThrowingSkeleton,
                            NPCID.BoneThrowingSkeleton2,
                            NPCID.BoneThrowingSkeleton3,
                            NPCID.BoneThrowingSkeleton4
                        };

                        if (Main.npc.Count(n => n.active && skeletons.Contains(n.type)) < 12)
                        {
                            float gravity = 0.4f; //shoot down
                            const float time = 60f;
                            Vector2 distance = Main.player[npc.target].Top - npc.Center + Main.rand.NextVector2Circular(80, 80);
                            distance.X /= time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;

                            FargoSoulsUtil.NewNPCEasy(
                                npc.GetSource_FromAI(),
                                npc.Center,
                                Main.rand.Next(skeletons),
                                velocity: distance);

                            SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                        }
                    }
                    */
					if (--BabyGuardianTimer < 0)
					{
						BabyGuardianTimer = BabyGuardianTimerRefresh(npc);
						if (!WorldSavingSystem.MasochistModeReal)
							BabyGuardianTimer += 60;

						SoundEngine.PlaySound(SoundID.ForceRoarPitched, npc.Center);

						for (int j = -1; j <= 1; j++) //to both sides
						{
							if (j == 0)
								continue;

							const int gap = 40;
							const int max = 14;
							float modifier = 1f - (float)npc.life / npc.lifeMax;
							modifier *= 4f / 3f; //scaling maxes at 25% life
							if (modifier > 1f || WorldSavingSystem.MasochistModeReal) //cap it, or force it to cap in emode
								modifier = 1f;
							int actualNumberToSpawn = (int)(max * modifier);
							Vector2 baseVel = npc.SafeDirectionTo(Main.player[npc.target].Center).RotatedBy(MathHelper.ToRadians(gap) * j);
							for (int k = 0; k < actualNumberToSpawn; k++) //a fan of skulls
							{
								if (FargoSoulsUtil.HostCheck)
								{
									float velModifier = 1f + 9f * k / max;
									Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velModifier * baseVel.RotatedBy(MathHelper.ToRadians(10) * j * k),
										ModContent.ProjectileType<SkeletronGuardian2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
								}
							}
						}

						if (FargoSoulsUtil.HostCheck) //one more shot straight behind skeletron
						{
							float velModifier = 10f;
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velModifier * npc.DirectionFrom(Main.player[npc.target].Center),
								ModContent.ProjectileType<SkeletronGuardian2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer);
						}
					}
				}
			}

			if (npc.ai[1] == 2f)
			{
				npc.defense = 9999;
				npc.damage = npc.defDamage * 15;

				if (!Main.dayTime && !WorldSavingSystem.MasochistModeReal)
				{
					if (++DGSpeedRampup < 120)
					{
						npc.position -= npc.velocity * (120 - DGSpeedRampup) / 120;
					}
				}

				if (Main.dayTime && !Main.remixWorld)
				{
					npc.Transform(NPCID.DungeonGuardian);
					//DGDaytime = true;
				}
				/*
                if (DGDaytime)
                {
                    npc.position += npc.velocity * DGSpeedRampup / 120;
                    DGSpeedRampup++;
                }
                */
			}

			EModeUtils.DropSummon(npc, "SuspiciousSkull", NPC.downedBoss3, ref DroppedSummon);

			return result;
		}
		public override void SafePostAI(NPC npc)
		{
			if (!ArmDR(npc) && npc.ai[1] != 2f)
			{
				npc.defense = 10; //no giga defense during p2 hands
			}
			base.SafePostAI(npc);
		}
		void CrossGuardianAttack(NPC npc)
		{
			if (!WorldSavingSystem.MasochistModeReal)
			{
				for (int i = 0; i < Main.maxProjectiles; i++) //also clear leftover babies
				{
					if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].type == ModContent.ProjectileType<SkeletronGuardian2>())
						Main.projectile[i].Kill();
				}
			}

			if ((npc.life >= npc.lifeMax * .75 || WorldSavingSystem.MasochistModeReal) && FargoSoulsUtil.HostCheck)
			{
				for (int i = 0; i < 4; i++)
				{
					for (int j = -2; j <= 2; j++)
					{
						Vector2 spawnPos = new(1200, 80 * j);
						Vector2 vel = -8 * Vector2.UnitX;
						spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * (i + 0.5));
						vel = vel.RotatedBy(Math.PI / 2 * (i + 0.5));
						int p = Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<ShadowGuardian>(),
							FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
						if (p != Main.maxProjectiles)
							Main.projectile[p].timeLeft = 1200 / 8 + 1;
					}
				}
			}
		}

		void SprayHomingBabies(NPC npc)
		{
			const int max = 30;
			float modifier = 1f - (float)npc.life / npc.lifeMax;
			modifier *= 4f / 3f; //scaling maxes at 25% life
			if (modifier > 1f || WorldSavingSystem.MasochistModeReal) //cap it, or force it to cap in maso
				modifier = 1f;
			int actualNumberToSpawn = (int)(max * modifier);
			for (int i = 0; i < actualNumberToSpawn; i++)
			{
				float speed = Main.rand.NextFloat(3f, 9f);
				Vector2 velocity = speed * npc.DirectionFrom(Main.player[npc.target].Center).RotatedBy(Math.PI * (Main.rand.NextDouble() - 0.5));
				float ai1 = speed / (60f + Main.rand.NextFloat(actualNumberToSpawn * 2));
				Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ModContent.ProjectileType<SkeletronGuardian>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 0.8f), 0f, Main.myPlayer, 0f, ai1);
			}
		}

		void DungeonGuardianAttack(NPC npc)
		{
			switch (Main.rand.Next(4))
			{
				case 0: //walls of guardians
					for (int i = 0; i < 4; i++)
					{
						for (int j = -2; j <= 2; j++)
						{
							Vector2 spawnPos = new(1200, 80 * j);
							Vector2 vel = -8 * Vector2.UnitX;
							spawnPos = Main.player[npc.target].Center + spawnPos.RotatedBy(Math.PI / 2 * i);
							vel = vel.RotatedBy(Math.PI / 2 * i);
							Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, vel, ModContent.ProjectileType<ShadowGuardian>(),
								FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
						}
					}
					break;

				case 1: //ring of babies
					{
						const int max = 16;
						Vector2 baseOffset = npc.SafeDirectionTo(Main.player[npc.target].Center);
						for (int i = 0; i < max; i++)
						{
							int p = Projectile.NewProjectile(npc.GetSource_FromThis(), Main.player[npc.target].Center + 1000 * baseOffset.RotatedBy(2 * Math.PI / max * i),
								-8f * baseOffset.RotatedBy(2 * Math.PI / max * i), ModContent.ProjectileType<DeviGuardian>(),
								FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
							if (p != Main.maxProjectiles)
							{
								Main.projectile[p].light = 1;
							}
						}
					}
					break;

				case 2: //homing skulls
					{
						Vector2 speed = Main.player[npc.target].Center - npc.Center;
						speed.X += Main.rand.Next(-20, 21);
						speed.Y += Main.rand.Next(-20, 21);
						speed.Normalize();
						speed *= 3f;
						for (int i = 0; i < 6; i++)
						{
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed.RotatedBy(Math.PI / 3 * i),
								ProjectileID.Skull, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer, -1f, 0);
						}
					}
					break;

				case 3:
					CrossGuardianAttack(npc);
					break;

				default:
					SprayHomingBabies(npc);
					break;
			}
		}

		static bool ArmDR(NPC npc) => !WorldSavingSystem.SwarmActive && Main.npc.Any(n => n.active && n.type == NPCID.SkeletronHand && n.ai[1] == npc.whoAmI);
		static float GetDR(NPC npc)
		{
			if (ArmDR(npc))
			{
				float percent = npc.GetLifePercent();
				return !npc.GetGlobalNPC<SkeletronHead>().SpawnedArms ? Math.Max(percent - 0.5f, 0) : percent;
			}
			return 1;
		}
		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			modifiers.FinalDamage *= GetDR(npc);
		}
		public override void UpdateLifeRegen(NPC npc, ref int damage)
		{
			if (npc.lifeRegen < 0)
				npc.lifeRegen = (int)(npc.lifeRegen * GetDR(npc));
		}

		public override bool CheckDead(NPC npc)
		{
			if (npc.ai[1] != 2f && !WorldSavingSystem.SwarmActive)
			{
				SoundEngine.PlaySound(SoundID.Roar, npc.Center);

				npc.life = npc.lifeMax / 176;
				if (npc.life < 50)
					npc.life = 50;

				npc.defense = 9999;
				npc.damage = npc.defDamage * 15;

				npc.ai[1] = 2f;
				npc.netUpdate = true;
				NetSync(npc);

				if (!HasSaidEndure)
				{
					HasSaidEndure = true;
					FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.NPCs.EMode.GuardianForm", new Color(175, 75, 255), npc.FullName);
				}
				return false;
			}

			return base.CheckDead(npc);
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
			target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);
		}

		public override void LoadSprites(NPC npc, bool recolor)
		{
			base.LoadSprites(npc, recolor);

			LoadNPCSprite(recolor, npc.type);
			LoadBossHeadSprite(recolor, 19);
			LoadGoreRange(recolor, 54, 57);
			LoadProjectile(recolor, ProjectileID.Skull);

			LoadSpecial(recolor, ref TextureAssets.BoneArm, ref FargowiltasSouls.TextureBuffer.BoneArm, "Arm_Bone");
		}
	}

	public class SkeletronHand : EModeNPCBehaviour
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SkeletronHand);

		public int AttackTimer;
		public int AI_Timer;
		public Vector2 storedVel; //needed because vanilla ai fucks with velocity and we want to override it entirely
		public int collisionCooldown = 60;
		public bool secondSet = false;
		public bool HitPlayer = true;
		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);
			npc.damage = (int)(npc.damage * 1.8f); //deals slightly more damage than head
			npc.lifeMax = (int)(npc.lifeMax * 1.4f);
		}
		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			base.SendExtraAI(npc, bitWriter, binaryWriter);

			binaryWriter.Write7BitEncodedInt(AttackTimer);
			binaryWriter.Write7BitEncodedInt(AI_Timer);
			binaryWriter.Write7BitEncodedInt(collisionCooldown);
			binaryWriter.Write(npc.localAI[0]);
			binaryWriter.Write(npc.localAI[1]);
			binaryWriter.Write(npc.localAI[2]);
			binaryWriter.Write(npc.localAI[3]);
			binaryWriter.WriteVector2(storedVel);
			binaryWriter.Write(secondSet);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			base.ReceiveExtraAI(npc, bitReader, binaryReader);

			AttackTimer = binaryReader.Read7BitEncodedInt();
			AI_Timer = binaryReader.Read7BitEncodedInt();
			collisionCooldown = binaryReader.Read7BitEncodedInt();
			npc.localAI[0] = binaryReader.ReadSingle();
			npc.localAI[1] = binaryReader.ReadSingle();
			npc.localAI[2] = binaryReader.ReadSingle();
			npc.localAI[3] = binaryReader.ReadSingle();
			storedVel = binaryReader.ReadVector2();
			secondSet = binaryReader.ReadBoolean();
		}
		//arm ai notes: ai3 is attack timer, when 300 the arm attacks and it stays at 300
		public const int GuardianTime = 65;
		public const int GuardianDelay = 150;


		public override bool SafePreAI(NPC npc)
		{
			bool result = base.SafePreAI(npc);


			if (WorldSavingSystem.SwarmActive)
				return result;

			NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronHead);
			if (head == null)
				return result;

			if (npc.timeLeft < 60) //never despawn normally
				npc.timeLeft = 60;

			//vanilla ai sometimes throws hand too far away and self despawns
			//if too far, tp back and reset ai
			if (npc.Distance(head.Center) > 1600)
			{
				npc.Center = head.Center;
				npc.ai[2] = 0;
				npc.ai[3] = 0;
				npc.localAI[0] = 0;
				npc.localAI[1] = 0;
				npc.localAI[2] = 0;
				npc.localAI[3] = 0;
				npc.netUpdate = true;
			}


			if (head.ai[1] == 1f || head.ai[1] == 2f) //spinning or DG mode
			{
				AttackTimer--;
				if (AttackTimer >= 0 && head.life >= head.lifeMax * .75) //for a short period
				{
					if (AttackTimer < GuardianTime)
					{
						//Vector2 centerPoint = head.Center - 10 * 16 * Vector2.UnitY;
						//if (!npc.HasValidTarget || npc.Distance(centerPoint) > 15 * 16)
						if (!npc.HasValidTarget)
						{
							AttackTimer++; //pause here, dont begin guardians attack until in range
						}
						else if (AttackTimer % 7 == 0 && FargoSoulsUtil.HostCheck)
						{
							Vector2 vel = npc.SafeDirectionTo(Main.player[npc.target].Center);
							if (AttackTimer < GuardianTime * 3 / 4) //first quarter of projectiles are shot towards player, other three quarters are shot straight out
							{
								vel = head.SafeDirectionTo(npc.Center);
							}
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<SkeletronGuardian2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
						}
					}
				}
			}
			else
			{
				if (AttackTimer != GuardianTime + GuardianDelay)
				{
					AttackTimer = GuardianTime + GuardianDelay;

					if (FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && head.life >= head.lifeMax / 2) //throw undead miner
					{
						float gravity = 0.4f; //shoot down
						const float time = 60f;
						Vector2 distance = Main.player[npc.target].Top - npc.Center + Main.rand.NextVector2Circular(80, 80);
						distance.X /= time;
						distance.Y = distance.Y / time - 0.5f * gravity * time;

						SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
						FargoSoulsUtil.NewNPCEasy(
							npc.GetSource_FromAI(),
							npc.Center,
							Main.rand.Next(new int[] {
								NPCID.BoneThrowingSkeleton,
								NPCID.BoneThrowingSkeleton2,
								NPCID.BoneThrowingSkeleton3,
								NPCID.BoneThrowingSkeleton4
							}),
							velocity: distance);
					}
				}
			}

			return result;
		}
		public override void SafePostAI(NPC npc)
		{
			if (WorldSavingSystem.SwarmActive || Main.dayTime)
				return;

			NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronHead);
			if (head == null)
				return;
			Player player = Main.player[head.target];
			if (player == null || !player.active || player.dead)
				return;

			npc.ai[3] = 0; //no vanilla attacking

			ref float handSide = ref npc.ai[0];

			int restDistance = (head.width / 2) + (npc.width / 2) + 30;
			int rotwaveTime = 60 * 5;
			const int extraDistMult = 50;

			float restrot = (float)Math.Sin(handSide * ((float)AI_Timer / rotwaveTime) * MathHelper.Pi) * MathHelper.Pi / 3;
			if (secondSet)
			{
				restrot += handSide * MathHelper.PiOver2;
			}
			Vector2 restPos = head.Center - (Vector2.UnitX * (restDistance + extraDistMult * Math.Abs(restrot)) * handSide).RotatedBy(restrot);

			if (HeadSpinning(npc)) //during spin
			{
				SpinAttack();
			}
			else //outside of spin
			{
				NonSpinAI();
			}
			void SpinAttack()
			{
				ref float lockedRotation = ref npc.localAI[0];
				ref float lockedDistance = ref npc.localAI[2];
				ref float rotDir = ref npc.localAI[3];
				ref float handSide = ref npc.ai[0];
				if (AttackTimer == GuardianTime + 30) //lock rotation to player
				{
					lockedRotation = (-head.SafeDirectionTo(player.Center)).ToRotation();
					rotDir = -handSide;
					if (secondSet)
					{
						lockedRotation += rotDir * MathHelper.Pi * 18f / 16;
					}
					else
					{
						lockedRotation += rotDir * MathHelper.Pi * 2f / 16f;
					}
					lockedDistance = Math.Max(head.Distance(player.Center), head.width + npc.width);

					AI_Timer = rotwaveTime - 45; //no desyncing
					collisionCooldown = 30 + 20;
					NetSync(npc);
				}


				HitPlayer = (AttackTimer < GuardianTime - 15); //don't hit player until 15 ticks after attack starts
				if (AttackTimer < -30 && !WorldSavingSystem.MasochistModeReal || AttackTimer > GuardianTime + 30)
				{
					Neutral();
				}
				else
				{
					if (WorldSavingSystem.MasochistModeReal && head.life < head.lifeMax * 0.75f && AttackTimer < 0) //in maso, a bit after first clap in p2
					{
						lockedDistance += 1;
					}
					if (npc.Distance(head.Center) > (head.width + npc.width / 2)) //don't do collide thing if too close to head (means they just spam collide) (this happens if it's spinning when they spawn)
					{
						if (collisionCooldown <= 0)
						{
							if (CollidingWithOtherHand(npc))
							{

								SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, npc.Center);
								collisionCooldown = 20;
								if (FargoSoulsUtil.HostCheck)
								{
									for (int i = 0; i < 2; i++)
									{
										Vector2 vel = head.SafeDirectionTo(npc.Center) * 10;
										vel = vel.RotatedBy(i * rotDir * MathHelper.Pi / 22); //curve second slightly inward so you can't blindspot in center
										int p = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<SkeletronGuardian2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
									}

								}
								rotDir = -rotDir;
							}
						}
					}
					int desiredDistance = restDistance * 3;
					if (head.life < head.lifeMax / 2)
					{
						desiredDistance = (int)lockedDistance + 10;
					}
					float halfRotationTime = (GuardianTime + 10f);

					//float rot = (float)AttackTimer / (GuardianTime + 10f) * MathHelper.Pi * rotDir;
					float moveStrength = 0.2f;
					if (AttackTimer > GuardianTime)
					{
						moveStrength /= 4;
					}
					else
					{
						lockedRotation += rotDir * MathHelper.Pi / halfRotationTime;
					}
					Vector2 desiredPos = head.Center + (lockedRotation.ToRotationVector2() * desiredDistance);

					npc.velocity = (desiredPos - npc.Center) * moveStrength;
					npc.velocity += head.velocity;
				}
			}
			void NonSpinAI()
			{
				const int LungeWindup = 30;
				const int ClapWindup = 60;
				ref float Timer = ref npc.localAI[1];
				ref float handSide = ref npc.ai[0];
				if (AI_Timer % rotwaveTime == 0 && AI_Timer >= rotwaveTime && head.ai[2] + LungeWindup < 800) //don't do it instantly on first round, or if head is about to spin before can lunge
				{
					//telegraph lunge
					int sideToLunge = AI_Timer % (rotwaveTime * 2) == 0 ? 1 : -1;
					if (handSide == sideToLunge || WorldSavingSystem.MasochistModeReal)
					{
						PrepareLunge();
					}
				}
				if (head.life > head.lifeMax * 0.75f && AI_Timer % rotwaveTime == rotwaveTime / 2 && head.ai[2] + ClapWindup < 800)
				{
					int sideToLunge = AI_Timer % (rotwaveTime * 2) == rotwaveTime / 2 ? 1 : -1;
					if (handSide == sideToLunge)
					{
						PrepareLunge();
					}
					/*
                    if (Main.npc.Any(n => OtherHandAlive(npc, n)))
                    {
                        //start clap
                        Timer = -1;
                        NetSync(npc);
                    }
                    */
				}
				void PrepareLunge()
				{
					ref float Timer = ref npc.localAI[1];
					SoundEngine.PlaySound(SoundID.Zombie82 with { Volume = 1.4f }, head.Center);
					Timer = 1;
					const int sparks = 10;
					for (int i = 0; i < sparks; i++)
					{
						Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * (float)i / sparks).RotatedByRandom(MathHelper.Pi / sparks);
						vel *= Main.rand.NextFloat(3, 8);
						Particle p = new SparkParticle(npc.Center, vel, Color.Red, 1, LungeWindup);
						p.Spawn();
						Timer = 1;
					}
					NetSync(npc);
				}
				if (Timer == 0)
				{
					Neutral();
				}
				if (Timer > 0) //lunge
				{

					if (Timer < LungeWindup)
					{
						float modifier = 1 - (float)(Timer / LungeWindup);
						storedVel = -npc.SafeDirectionTo(player.Center) * modifier * 3;
					}
					if (Timer == LungeWindup)
					{
						storedVel = npc.SafeDirectionTo(player.Center) * 30;
					}
					if (Timer > LungeWindup + 10 && Timer <= LungeWindup + 30)
					{
						storedVel *= 0.95f;
					}
					if (Timer > LungeWindup + 30)
					{
						storedVel += npc.SafeDirectionTo(restPos) * 0.3f;
						if (Timer > LungeWindup + 100 || npc.Distance(restPos) < npc.width)
						{
							Timer = 0;
							storedVel = Vector2.Zero;
							NetSync(npc);
						}
					}

				}
				//this didn't work properly and isn't really needed, might give it another go another time
				//the idea is: position hands to each side of player, then clap them together towards the player
				/*
                float ClapTimer = -Timer; //to not confuse with minuses
                if (ClapTimer > 0) //horizontal clap
                {
                    Vector2 sidePos = player.Center + Vector2.UnitX * 500 * -handSide;
                    if (ClapTimer < ClapWindup)
                    {
                        storedVel.X += Math.Sign(sidePos.X - npc.Center.X) * 1;
                        storedVel.Y += Math.Sign(sidePos.Y - npc.Center.Y) * 0.5f;
                        if (npc.Distance(sidePos) < npc.width)
                        {
                            Timer = -ClapWindup;
                        }
                    }
                    if (ClapTimer == ClapWindup)
                    {
                        storedVel = Vector2.UnitX * Math.Sign(player.Center.X - npc.Center.X) * 30;
                    }
                    if (ClapTimer > ClapWindup && ClapTimer < ClapWindup + 100)
                    {
                        if (CollidingWithOtherHand(npc))
                        {
                            Timer = -(ClapWindup + 100);
                            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, npc.Center);
                            storedVel = -storedVel / 2;
                        }
                        if (ClapTimer > ClapWindup + 20) //hand missing or missed
                        {
                            Timer = -(ClapWindup + 100);
                            storedVel /= 2;
                        }
                    }
                    if (ClapTimer >= ClapWindup + 100)
                    {
                        if (npc.Distance(restPos) < npc.width || ClapTimer >= ClapWindup + 160)
                        {
                            Timer = 0;
                            storedVel = Vector2.Zero;
                            NetSync(npc);
                        }
                        else
                        {
                            float rotdif = FargoSoulsUtil.RotationDifference(storedVel, restPos - npc.Center);
                            float rotSpeed = 2;
                            storedVel = storedVel.RotatedBy(Math.Sign(rotdif) * Math.Min(Math.Abs(rotdif), rotSpeed * MathHelper.Pi / 180));
                        }
                    }
                }
                */
				if (Timer != 0)
				{
					Timer += Math.Sign(Timer);
					npc.velocity = storedVel + head.velocity;
				}
			}
			void Neutral()
			{
				npc.velocity = (restPos - npc.Center) * 0.02f;
				npc.velocity += head.velocity;
			}
			if (collisionCooldown > 0)
			{
				collisionCooldown--;
			}
			AI_Timer++;
		}
		public static bool OtherHandAlive(NPC self, NPC other) => other != null && other.active && other.type == NPCID.SkeletronHand && other.ai[1] == self.ai[1] && other.whoAmI != self.whoAmI;
		private bool CollidingWithOtherHand(NPC npc) => Main.npc.Any(n => OtherHandAlive(npc, n) && npc.Hitbox.Intersects(n.Hitbox));
		public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
		{
			NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronHead);
			if (head != null && head.type == NPCID.SkeletronHead)
				if (head.GetGlobalNPC<SkeletronHead>().SpawnGrace > 0)
					return false;

			return HitPlayer;
		}
		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);

			//not while spinning outside maso
			if (!HeadSpinning(npc) || WorldSavingSystem.MasochistModeReal)
			{
				target.AddBuff(BuffID.Dazed, 60);
			}

		}
		public static bool HeadSpinning(NPC npc)
		{
			NPC head = FargoSoulsUtil.NPCExists(npc.ai[1], NPCID.SkeletronHead);
			if (head == null || !head.active)
			{
				return false;
			}
			return (head.ai[1] == 1f || head.ai[1] == 2f);
		}
		public override void LoadSprites(NPC npc, bool recolor)
		{
			base.LoadSprites(npc, recolor);

			LoadNPCSprite(recolor, npc.type);
		}
	}
}
