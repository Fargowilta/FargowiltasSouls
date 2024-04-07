using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
	internal class FargoGlobalBuff : GlobalBuff
	{
		public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
		{
			if (WorldSavingSystem.EternityMode)
			{
				if (type == BuffID.ShadowDodge)
					tip += "\n" + Language.GetTextValue("Mods.FargowiltasSouls.EModeBalance.ShadowDodge");
				else if (type == BuffID.IceBarrier)
					tip += "\n" + Language.GetTextValue("Mods.FargowiltasSouls.EModeBalance.IceBarrier");
			}
		}

		public static int[] DebuffsToLetDecreaseNormally => new int[] {
			BuffID.Frozen,
			BuffID.Stoned,
			BuffID.Cursed,
			ModContent.BuffType<FusedBuff>(),
			ModContent.BuffType<TimeFrozenBuff>(),
			ModContent.BuffType<StunnedBuff>()
		};

		public override void Update(int type, Player player, ref int buffIndex)
		{
			switch (type)
			{
				case BuffID.Slimed:
					Main.buffNoTimeDisplay[type] = false;
					if (WorldSavingSystem.EternityMode)
						player.FargoSouls().Slimed = true;
					break;

				case BuffID.BrainOfConfusionBuff:
					if (WorldSavingSystem.EternityMode)
						player.AddBuff(ModContent.BuffType<BrainOfConfusionBuff>(), player.buffTime[buffIndex] * 2);
					break;

				case BuffID.OnFire:
					if (WorldSavingSystem.EternityMode && Main.raining && player.position.Y < Main.worldSurface * 16
						&& Framing.GetTileSafely(player.Center).WallType == WallID.None && player.buffTime[buffIndex] > 2)
						player.buffTime[buffIndex] -= 1;
					break;

				case BuffID.Chilled:
					if (WorldSavingSystem.EternityMode && player.buffTime[buffIndex] > 60 * 15)
						player.buffTime[buffIndex] = 60 * 15;
					break;

				case BuffID.Dazed:
					if (player.whoAmI == Main.myPlayer && player.buffTime[buffIndex] % 60 == 55)
						SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/DizzyBird"));
					break;


				case BuffID.ThornWhipPlayerBuff:
				case BuffID.SwordWhipPlayerBuff:
				case BuffID.ScytheWhipPlayerBuff:
				case BuffID.CoolWhipPlayerBuff:
					if (WorldSavingSystem.EternityMode)
					{
						if (player.Eternity().HasWhipBuff)
							player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex], 1);
						player.Eternity().HasWhipBuff = true;
					}
					break;

				default:
					break;
			}

			if (WorldSavingSystem.EternityMode && player.buffTime[buffIndex] > 5 && Main.debuff[type] && player.Eternity().ShorterDebuffsTimer <= 0
				&& !Main.buffNoTimeDisplay[type]
				&& type != BuffID.Tipsy && (!BuffID.Sets.NurseCannotRemoveDebuff[type] || type == BuffID.ManaSickness || type == BuffID.PotionSickness)
				&& !DebuffsToLetDecreaseNormally.Contains(type)
				&& !(type == BuffID.Confused && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.brainBoss, NPCID.BrainofCthulhu)))
			{
				player.buffTime[buffIndex] -= 1;
			}

			base.Update(type, player, ref buffIndex);
		}

		public override void Update(int type, NPC npc, ref int buffIndex)
		{
			switch (type)
			{
				case BuffID.BrokenArmor:
					npc.FargoSouls().BrokenArmor = true;
					break;

				//                //case BuffID.Chilled: npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>().Chilled = true; break;

				case BuffID.Darkness:
					npc.color = Color.Gray;

					if (npc.buffTime[buffIndex] % 30 == 0)
					{
						if (FargoSoulsUtil.HostCheck)
						{
							Player player = Main.player.FirstOrDefault(p => p.active && !p.dead && p.HasEffect<AncientShadowDarkness>());
							if (player != null && player.active && !player.dead)
							{
								for (int i = 0; i < Main.maxNPCs; i++)
								{
									NPC target = Main.npc[i];
									if (target.active && !target.friendly && Vector2.Distance(npc.Center, target.Center) < 250)
									{
										Vector2 velocity = Vector2.Normalize(target.Center - npc.Center) * 5;
										int p = Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, velocity, ProjectileID.ShadowFlame, 40 + FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
										if (p.IsWithinBounds(Main.maxProjectiles))
										{
											Main.projectile[p].friendly = true;
											Main.projectile[p].hostile = false;
										}
										if (Main.rand.NextBool(3))
											break;
									}
								}
							}
						}

					}
					break;

				case BuffID.Electrified:
					npc.FargoSouls().Electrified = true;
					break;

				case BuffID.Suffocation:
					npc.FargoSouls().Suffocation = true;
					break;

				case BuffID.OnFire:
					if (WorldSavingSystem.EternityMode && Main.raining && npc.position.Y < Main.worldSurface * 16
						&& Framing.GetTileSafely(npc.Center).WallType == WallID.None && npc.buffTime[buffIndex] > 2)
						npc.buffTime[buffIndex] -= 1;
					break;

				case BuffID.BoneWhipNPCDebuff:
				case BuffID.MaceWhipNPCDebuff:
				case BuffID.RainbowWhipNPCDebuff:
				case BuffID.SwordWhipNPCDebuff:
				case BuffID.ThornWhipNPCDebuff:
					if (WorldSavingSystem.EternityMode && npc.Eternity().HasWhipDebuff)
						npc.buffTime[buffIndex] = Math.Min(npc.buffTime[buffIndex], 1);
					npc.Eternity().HasWhipDebuff = true;
					break;

				default:
					break;
			}
		}
	}
}