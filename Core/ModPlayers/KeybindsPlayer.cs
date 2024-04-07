using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.UI;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ModPlayers
{
	public partial class FargoSoulsPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			#region ignores stuns

			if (Mash)
			{
				Player.doubleTapCardinalTimer[0] = 0;
				Player.doubleTapCardinalTimer[1] = 0;
				Player.doubleTapCardinalTimer[2] = 0;
				Player.doubleTapCardinalTimer[3] = 0;

				const int increment = 1;

				if (triggersSet.Up)
				{
					if (!MashPressed[0])
						MashCounter += increment;
					MashPressed[0] = true;
				}
				else
					MashPressed[0] = false;

				if (triggersSet.Left)
				{
					if (!MashPressed[1])
						MashCounter += increment;
					MashPressed[1] = true;
				}
				else
					MashPressed[1] = false;

				if (triggersSet.Right)
				{
					if (!MashPressed[2])
						MashCounter += increment;
					MashPressed[2] = true;
				}
				else
					MashPressed[2] = false;

				if (triggersSet.Down)
				{
					if (!MashPressed[3])
						MashCounter += increment;
					MashPressed[3] = true;
				}
				else
					MashPressed[3] = false;
			}

			if (FargowiltasSouls.FreezeKey.JustPressed)
			{
				if (Player.HasEffect<StardustEffect>() && !Player.HasBuff(ModContent.BuffType<TimeStopCDBuff>()))
				{
					int cooldownInSeconds = 60;
					if (ForceEffect<StardustEnchant>())
						cooldownInSeconds = 50;
					if (TerrariaSoul)
						cooldownInSeconds = 40;
					if (Eternity)
						cooldownInSeconds = 30;
					Player.ClearBuff(ModContent.BuffType<TimeFrozenBuff>());
					Player.AddBuff(ModContent.BuffType<TimeStopCDBuff>(), cooldownInSeconds * 60);

					FreezeTime = true;
					freezeLength = StardustEffect.TIMESTOP_DURATION;

					SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ZaWarudo"), Player.Center);
				}
				/*else if (Player.HasEffect<SnowEffect>() && !Player.HasBuff(ModContent.BuffType<SnowstormCDBuff>())
                    && !Player.HasBuff(ModContent.BuffType<MutantPresenceBuff>()))
                {
                    Player.AddBuff(ModContent.BuffType<SnowstormCDBuff>(), 60 * 60);

                    ChillSnowstorm = true;
                    chillLength = CHILL_DURATION;

                    SoundEngine.PlaySound(SoundID.Item27, Player.Center);

                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire, 0, 0, 0, default, 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity *= 9f;
                    }
                }*/
			}

			if (PrecisionSeal)
			{
				if (SoulConfig.Instance.PrecisionSealIsHold)
					PrecisionSealNoDashNoJump = FargowiltasSouls.PrecisionSealKey.Current;
				else
				{
					if (FargowiltasSouls.PrecisionSealKey.JustPressed)
						PrecisionSealNoDashNoJump = !PrecisionSealNoDashNoJump;
				}
			}
			else
				PrecisionSealNoDashNoJump = false;

			if (PrecisionSealNoDashNoJump)
			{
				Player.doubleTapCardinalTimer[2] = 0;
				Player.doubleTapCardinalTimer[3] = 0;
			}

			if (FargowiltasSouls.AmmoCycleKey.JustPressed && CanAmmoCycle)
				AmmoCycleKey();

			if (FargowiltasSouls.SoulToggleKey.JustPressed)
				FargoUIManager.ToggleSoulToggler();

			if (FargowiltasSouls.GoldKey.JustPressed && Player.HasEffect<GoldEffect>())
			{
				GoldKey();
			}

			#endregion

			if (GoldShell || Player.CCed || NoUsingItems > 2)
			{
				return;
			}

			#region blocked by stuns

			if (FargowiltasSouls.SmokeBombKey.JustPressed && CrystalEnchantActive && SmokeBombCD == 0)
				CrystalAssassinEnchant.SmokeBombKey(this);

			if (FargowiltasSouls.SpecialDashKey.JustPressed && (BetsysHeartItem != null || QueenStingerItem != null))
				SpecialDashKey();

			if (FargowiltasSouls.MagicalBulbKey.JustPressed && MagicalBulb)
				MagicalBulbKey();

			if (FrigidGemstoneItem != null)
			{
				if (FrigidGemstoneCD > 0)
					FrigidGemstoneCD--;

				if (FargowiltasSouls.FrigidSpellKey.Current)
					FrigidGemstoneKey();
			}

			if (FargowiltasSouls.BombKey.JustPressed)
				BombKey();

			if (FargowiltasSouls.DebuffInstallKey.JustPressed)
				DebuffInstallKey();

			#endregion
		}
	}
}
