using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.FrostMoon;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PumpkinMoon;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items
{
	public class EmodeItemBalance : ModSystem
	{
		/// <summary>
		/// Applies an Eternity-exclusive balance change to chosen item type. <br />
		/// If the balanceTextKeys include the string "Damage" or "Speed", those effects will be automatically applied.
		/// Default balanceNumber is -1. <br />
		/// Default balanceTextKeys is null. <br />
		/// Default extra is empty. <br />
		/// Number and extra is used on the first tooltip line.
		/// </summary>
		public static EModeChange EmodeBalance(ref Item item, ref float balanceNumber, ref string[] balanceTextKeys, ref string extra)
		{
			switch (item.type)
			{
				case ItemID.RodofDiscord:
				case ItemID.RodOfHarmony:
					balanceTextKeys = new string[] { "RodofDiscord" };
					return EModeChange.Nerf;

				case ItemID.WaterBolt:
					if (!NPC.downedBoss3)
					{
						balanceTextKeys = new string[] { "WaterBolt" };
						return EModeChange.Nerf;
					}
					return EModeChange.None;

				case ItemID.HallowedGreaves:
				case ItemID.HallowedHeadgear:
				case ItemID.HallowedHelmet:
				case ItemID.HallowedHood:
				case ItemID.HallowedMask:
				case ItemID.HallowedPlateMail:
				case ItemID.AncientHallowedGreaves:
				case ItemID.AncientHallowedHeadgear:
				case ItemID.AncientHallowedHelmet:
				case ItemID.AncientHallowedHood:
				case ItemID.AncientHallowedMask:
				case ItemID.AncientHallowedPlateMail:
					balanceTextKeys = new string[] { "HolyDodge" };
					return EModeChange.Nerf;

				case ItemID.FrozenTurtleShell:
				case ItemID.FrozenShield:
					balanceTextKeys = new string[] { "FrozenTurtleShell" };
					return EModeChange.Nerf;

				case ItemID.BrainOfConfusion:
					balanceTextKeys = new string[] { "BrainOfConfusion" };
					return EModeChange.Nerf;

				case ItemID.Zenith:
					if (WorldSavingSystem.DownedMutant || ModLoader.HasMod("CalamityMod"))
					{
						balanceTextKeys = new string[] { "ZenithNone" };
						return EModeChange.Neutral;
					}
					else
					{
						string bossesToKill = "";
						if (!WorldSavingSystem.DownedAbom)
						{
							bossesToKill += $"{Language.GetTextValue("Mods.FargowiltasSouls.NPCs.AbomBoss.DisplayName")}, ";
						}
						bossesToKill += $"{Language.GetTextValue("Mods.FargowiltasSouls.NPCs.MutantBoss.DisplayName")}";

						balanceTextKeys = new string[] { "ZenithHitRate" };
						extra = bossesToKill;
						return EModeChange.Nerf;
					}


				case ItemID.VampireKnives:
					balanceTextKeys = new string[] { "VampireKnives" };
					return EModeChange.Nerf;

				case ItemID.ZapinatorGray:
				case ItemID.ZapinatorOrange:
					balanceTextKeys = new string[] { "Zapinator" };
					return EModeChange.Nerf;


				case ItemID.CoinGun:
					balanceTextKeys = new string[] { "CoinGun" };
					return EModeChange.Nerf;

				case ItemID.StarCannon:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 0.6f;
					return EModeChange.Nerf;
				case ItemID.SuperStarCannon:
					balanceTextKeys = new string[] { "SuperStarCannon" };
					balanceNumber = 7;
					return EModeChange.Nerf;

				case ItemID.CandyCornRifle:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 1.5f;
					return EModeChange.Buff;
				case ItemID.StakeLauncher:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 1.3f;
					return EModeChange.Buff;

				case ItemID.Razorpine:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 0.8f;
					return EModeChange.Nerf;
				case ItemID.BlizzardStaff:
					balanceTextKeys = new string[] { "Damage", "Speed" };
					balanceNumber = 0.7f;
					return EModeChange.Nerf;

				case ItemID.DD2SquireBetsySword:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 0.70f;
					return EModeChange.Nerf;

				case ItemID.Uzi:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 0.88f;
					return EModeChange.Nerf;

				case ItemID.MagicDagger:
					{
						if (!Main.hardMode)
						{
							balanceTextKeys = new string[] { "Damage" };
							balanceNumber = 0.5f;
							return EModeChange.Nerf;
						}
						return EModeChange.None;
					}


				case ItemID.DemonScythe:
					{
						if (!NPC.downedBoss2)
						{
							balanceTextKeys = new string[] { "DemonScythe", "DamageNoTooltip", "SpeedNoTooltip" };
							balanceNumber = 0.6f;
							return EModeChange.Nerf;
						}
						return EModeChange.None;
					}


				case ItemID.FetidBaghnakhs:
					balanceTextKeys = new string[] { "Speed" };
					balanceNumber = 0.75f;
					return EModeChange.Nerf;

				case ItemID.MoonlordTurretStaff:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 0.5f;
					return EModeChange.Nerf;
				case ItemID.RainbowCrystalStaff:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 0.6f;
					return EModeChange.Nerf;

				case ItemID.SlimeStaff:
				case ItemID.ImpStaff:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 1.1f;
					return EModeChange.Buff;

				case ItemID.Shroomerang:
					balanceTextKeys = new string[] { "Damage" };
					balanceNumber = 1.2f;
					return EModeChange.Buff;
				case ItemID.IceBlade:
					balanceTextKeys = new string[] { "IceBladeFrostburn" };
					balanceNumber = 1;
					return EModeChange.Buff;

				/*
            case ItemID.PiercingStarlight:
                balanceTextKeys = new string[] { "Damage" };
                balanceNumber = 0.6f;
                return EModeChange.Nerf;
                */

				case ItemID.PumpkinMoonMedallion:
					balanceNumber = PumpkinMoonBosses.WAVELOCK;
					if (WorldSavingSystem.MasochistModeReal)
					{
						balanceTextKeys = new string[] { "MoonsDrops", "MoonsWaves" };
						return EModeChange.Nerf;
					}
					else
					{
						balanceTextKeys = new string[] { "MoonsDrops" };
						return EModeChange.Nerf;
					}
				case ItemID.NaughtyPresent:
					balanceNumber = FrostMoonBosses.WAVELOCK;
					if (WorldSavingSystem.MasochistModeReal)
					{
						balanceTextKeys = new string[] { "MoonsDrops", "MoonsWaves" };
						return EModeChange.Nerf;
					}
					else
					{
						balanceTextKeys = new string[] { "MoonsDrops" };
						return EModeChange.Nerf;
					}

				case ItemID.CrossNecklace:
					balanceTextKeys = new string[] { "CrossNecklaceNerf" };
					return EModeChange.Nerf;
				case ItemID.StarCloak:
					if (!Main.hardMode)
					{
						balanceTextKeys = new string[] { "StarCloak" };
						return EModeChange.Nerf;
					}
					return EModeChange.None;
				case ItemID.StarVeil:
					balanceTextKeys = new string[] { "CrossNecklaceNerf" };
					if (!Main.hardMode)
						balanceTextKeys = new string[] { "StarCloak" };
					return EModeChange.Nerf;

				#region Sword and Spear Reworks
				case ItemID.CobaltNaginata:
					balanceNumber = -1;
					balanceTextKeys = new string[] { "SpearRework", "CobaltNaginataRework" };
					return EModeChange.Buff;
				case ItemID.CobaltSword:
					balanceNumber = 1.5f;
					balanceTextKeys = new string[] { "Speed", "CobaltNaginataRework" };
					return EModeChange.Buff;
				case ItemID.MythrilHalberd:
					balanceNumber = -1;
					balanceTextKeys = new string[] { "SpearRework", "MythrilHalberdRework" };
					return EModeChange.Buff;
				case ItemID.MythrilSword:
					balanceNumber = 1.5f;
					balanceTextKeys = new string[] { "Speed", "MythrilHalberdRework" };
					return EModeChange.Buff;
				case ItemID.OrichalcumHalberd:
					balanceNumber = -1;
					balanceTextKeys = new string[] { "SpearRework", "OrichalcumHalberdRework" };
					return EModeChange.Buff;
				case ItemID.OrichalcumSword:
					balanceNumber = 1.5f;
					balanceTextKeys = new string[] { "Speed", "OrichalcumHalberdRework" };
					return EModeChange.Buff;
				case ItemID.PalladiumPike:
					balanceNumber = -1;
					balanceTextKeys = new string[] { "SpearRework", "PalladiumPikeRework" };
					return EModeChange.Buff;
				case ItemID.PalladiumSword:
					balanceNumber = 1.5f;
					balanceTextKeys = new string[] { "Speed", "PalladiumPikeRework" };
					return EModeChange.Buff;

				case ItemID.TitaniumSword:
				case ItemID.AdamantiteSword:
					balanceNumber = 1.20f;
					balanceTextKeys = new string[] { "Speed", "Damage" };
					return EModeChange.Buff;

				case ItemID.Spear:
				case ItemID.Trident:
				case ItemID.Swordfish:
				case ItemID.ChlorophytePartisan:
					balanceNumber = 1;
					balanceTextKeys = new string[] { "SpearRework" };
					return EModeChange.Buff;

				case ItemID.AdamantiteGlaive:
				case ItemID.TitaniumTrident:
					balanceNumber = 1.15f;
					balanceTextKeys = new string[] { "Damage", "SpearRework" };
					return EModeChange.Buff;

				case ItemID.Excalibur:
				case ItemID.TrueExcalibur:
				case ItemID.Gungnir:
					balanceNumber = 1.2f;
					balanceTextKeys = new string[] { "Speed", "Damage" };
					return EModeChange.Buff;

				case ItemID.ObsidianSwordfish:
					balanceNumber = 0.8f;
					balanceTextKeys = new string[] { "Damage", "SpearRework" };
					#endregion

					return EModeChange.Buff;
				case ItemID.WarmthPotion:
					balanceTextKeys = new string[] { "WarmthPotionNerf" };
					return EModeChange.Nerf;

				case ItemID.JungleRose:
					balanceTextKeys = new string[] { "JungleRose" };
					return EModeChange.Buff;

				default:
					return EModeChange.None;
			}
		}

		public static void BalanceWeaponStats(Player player, Item item, ref StatModifier damage)
		{
			if (!WorldSavingSystem.EternityMode)
				return;
			string extra = string.Empty;
			float balanceNumber = -1;
			string[] balanceTextKeys = null;
			EmodeBalance(ref item, ref balanceNumber, ref balanceTextKeys, ref extra);
			if (balanceTextKeys != null)
			{
				for (int i = 0; i < balanceTextKeys.Length; i++)
				{
					switch (balanceTextKeys[i])
					{
						case "Damage":
						case "DamageNoTooltip":
							{
								damage *= balanceNumber;
								break;
							}


						case "Speed":
						case "SpeedNoTooltip":
							{
								player.FargoSouls().AttackSpeed *= balanceNumber;
								break;
							}

					}
				}
			}
		}
		public enum EModeChange
		{
			None,
			Nerf,
			Buff,
			Neutral
		}

		static void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, int amount = 0)
		{
			string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
			string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}", amount == 0 ? null : amount);
			tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, $"{change}{key}", $"{prefix}{nerf}"));
		}

		static void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, string extra)
		{
			string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
			string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}");
			tooltips.Add(new TooltipLine(FargowiltasSouls.Instance, $"{change}{key}", $"{prefix}{nerf}{extra}"));
		}

		public static void BalanceTooltips(Item item, ref List<TooltipLine> tooltips)
		{

			if (!WorldSavingSystem.EternityMode)
				return;

			//if (item.damage > 0 && (item.ammo == AmmoID.Arrow || item.ammo == AmmoID.Bullet || item.ammo == AmmoID.Dart))
			//{
			//    tooltips.Add(new TooltipLine(Mod, "masoAmmoNerf", "[c/ff0000:Eternity Mode:] Contributes 50% less damage to weapons"));
			//}
			string extra = string.Empty;
			float balanceNumber = -1;
			string[] balanceTextKeys = null;
			EModeChange balance = EmodeBalance(ref item, ref balanceNumber, ref balanceTextKeys, ref extra);

			if (balanceTextKeys != null)
			{
				for (int i = 0; i < balanceTextKeys.Length; i++)
				{
					switch (balanceTextKeys[i])
					{
						case "Damage":
							{

								EModeChange change = balanceNumber > 1 ? EModeChange.Buff : balanceNumber < 1 ? EModeChange.Nerf : EModeChange.Neutral;
								int amount = change == EModeChange.Buff ? (int)Math.Round((balanceNumber - 1f) * 100f) : (int)Math.Round((1f - balanceNumber) * 100f);
								string key = change == EModeChange.Buff ? "DamagePositive" : "Damage";
								ItemBalance(tooltips, change, key, amount);
								break;
							}

						case "Speed":
							{
								EModeChange change = balanceNumber > 1 ? EModeChange.Buff : balanceNumber < 1 ? EModeChange.Nerf : EModeChange.Neutral;
								int amount = change == EModeChange.Buff ? (int)Math.Round((balanceNumber - 1f) * 100f) : (int)Math.Round((1f - balanceNumber) * 100f);
								string key = change == EModeChange.Buff ? "SpeedPositive" : "Speed";
								ItemBalance(tooltips, change, key, amount);
								break;
							}

						case "DamageNoTooltip":
						case "SpeedNoTooltip":
							break;

						default:
							{
								EModeChange change = balance;
								if (balanceNumber != -1 && balanceTextKeys != null && i == 0)
								{
									ItemBalance(tooltips, change, balanceTextKeys[i], (int)balanceNumber);
								}
								else if (extra != string.Empty && balanceTextKeys != null && i == 0)
								{
									ItemBalance(tooltips, change, balanceTextKeys[i], extra);
								}
								else
								{
									ItemBalance(tooltips, change, balanceTextKeys[i]);
								}
								break;
							}
					}
				}
			}
			//TODO: mana pot rework
			/*
            if (item.healMana > 0)
            {
                ItemBalance(tooltips, EModeChange.Neutral, "ManaPots");
            }
            */
			if (item.shoot > ProjectileID.None && ProjectileID.Sets.IsAWhip[item.shoot])
			{
				ItemBalance(tooltips, EModeChange.Nerf, "WhipSpeed");
				ItemBalance(tooltips, EModeChange.Nerf, "WhipStack");
			}
			if (item.prefix >= PrefixID.Hard && item.prefix <= PrefixID.Warding)
			{
				ItemBalance(tooltips, EModeChange.Neutral, "DefensePrefix");
			}
			//else if (item.CountsAsClass(DamageClass.Summon))
			//{
			//    if (!(EModeGlobalProjectile.IgnoreMinionNerf.TryGetValue(item.shoot, out bool ignoreNerf) && ignoreNerf))
			//        ItemBalance(tooltips, EModeChange.Nerf, "MinionStack");

			//    ItemBalance(tooltips, EModeChange.Nerf, "SummonMulticlass");
			//}
		}
	}
}
