using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasSouls.Core.Systems.WorldSavingSystem;

namespace FargowiltasSouls.Core.Systems
{
	public class WorldUpdatingSystem : ModSystem
	{
		public override void PreUpdateNPCs() => SwarmActive = ModLoader.TryGetMod("Fargowiltas", out Mod fargo) && (bool)fargo.Call("SwarmActive");

		public override void PostUpdateWorld()
		{
			//NPC.LunarShieldPowerMax = NPC.downedMoonlord ? 50 : 100;

			if (!PlacedMutantStatue && (Main.zenithWorld || Main.remixWorld))
			{
				int positionX = Main.spawnTileX; //offset by dimensions of statue
				int positionY = Main.spawnTileY;
				int checkUp = -30;
				int checkDown = 10;
				bool placed = false;
				for (int offsetX = -50; offsetX <= 50; offsetX++)
				{
					for (int offsetY = checkUp; offsetY <= checkDown; offsetY++)
					{
						if (WorldGenSystem.TryPlacingStatue(positionX + offsetX, positionY + offsetY))
						{
							placed = true;
							PlacedMutantStatue = true;
							break;
						}
					}

					if (placed)
						break;
				}
			}

			if (ShouldBeEternityMode)
			{
				if (EternityMode && !FargoSoulsUtil.WorldIsExpertOrHarder())
				{
					EternityMode = false;
					FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityWrongDifficulty", new Color(175, 75, 255));
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.WorldData);
					if (!Main.dedServ)
						SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
				}
				else if (!EternityMode && FargoSoulsUtil.WorldIsExpertOrHarder() && !LumUtils.AnyBosses())
				{
					EternityMode = true;
					FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityOn", new Color(175, 75, 255));
					if (Main.masterMode && !CanPlayMaso)
						FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityMasterWarning", new Color(255, 255, 0));
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.WorldData);
					if (!Main.dedServ)
						SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
				}
			}
			else if (EternityMode)
			{
				EternityMode = false;
				FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.EternityOff", new Color(175, 75, 255));
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.WorldData);
				if (!Main.dedServ)
					SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
			}

			if (EternityMode)
			{
				//NPC.LunarShieldPowerMax = 25;

				if (/*Main.raining || Sandstorm.Happening || */Main.bloodMoon)
				{
					if (!HaveForcedAbomFromGoblins && !DownedAnyBoss //pre boss, disable some events
						&& ModContent.TryFind("Fargowiltas", "Abominationn", out ModNPC abom) && !NPC.AnyNPCs(abom.Type))
					{
						/*
                        Main.raining = false;
                        Main.rainTime = 0;
                        Main.maxRaining = 0;
                        Sandstorm.Happening = false;
                        Sandstorm.TimeLeft = 0;
                        if (Main.bloodMoon)
                        */
						FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.BloodMoonCancel", new Color(175, 75, 255));
						Main.bloodMoon = false;
						if (Main.netMode == NetmodeID.Server)
							NetMessage.SendData(MessageID.WorldData);
					}
				}

				if (!MasochistModeReal && EternityMode && ((FargoSoulsUtil.WorldIsMaster() && CanPlayMaso) || Main.zenithWorld) && !LumUtils.AnyBosses())
				{
					MasochistModeReal = true;
					FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.MasochistOn{(Main.zenithWorld ? "Zenith" : "")}", new Color(51, 255, 191, 0));
					if (Main.getGoodWorld)
						FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.MasochistFTWWarning", new Color(51, 255, 191, 0));
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.WorldData);
					if (!Main.dedServ)
						SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
				}
			}

			if (MasochistModeReal && !(EternityMode && ((FargoSoulsUtil.WorldIsMaster() && CanPlayMaso) || Main.zenithWorld)))
			{
				MasochistModeReal = false;
				FargoSoulsUtil.PrintLocalization($"Mods.{Mod.Name}.Message.{Name}.MasochistOff", new Color(51, 255, 191, 0));
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.WorldData);
				if (!Main.dedServ)
					SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
			}

			//Main.NewText(BuilderMode);

			#region commented

			//right when day starts
			/*if(/*Main.time == 0 && Main.dayTime && !Main.eclipse && WorldSavingSystem.masochistMode)
            {
                    SoundEngine.PlaySound(SoundID.Roar, player.Center);

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.eclipse = true;
                        //Main.NewText(Lang.misc[20], 50, 255, 130, false);
                    }
                    else
                    {
                        //NetMessage.SendData(61, -1, -1, "", player.whoAmI, -6f, 0f, 0f, 0, 0, 0);
                    }


            }*/

			// if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 361 && Main.CanStartInvasion(1, true))
			// {
			// this.itemTime = item.useTime;
			// SoundEngine.PlaySound(SoundID.Roar, this.Center);
			// if (FargoSoulsUtil.HostCheck)
			// {
			// if (Main.invasionType == 0)
			// {
			// Main.invasionDelay = 0;
			// Main.StartInvasion(1);
			// }
			// }
			// else
			// {
			// NetMessage.SendData(61, -1, -1, "", this.whoAmI, -1f, 0f, 0f, 0, 0, 0);
			// }
			// }
			// if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 602 && Main.CanStartInvasion(2, true))
			// {
			// this.itemTime = item.useTime;
			// SoundEngine.PlaySound(SoundID.Roar, this.Center);
			// if (FargoSoulsUtil.HostCheck)
			// {
			// if (Main.invasionType == 0)
			// {
			// Main.invasionDelay = 0;
			// Main.StartInvasion(2);
			// }
			// }
			// else
			// {
			// NetMessage.SendData(61, -1, -1, "", this.whoAmI, -2f, 0f, 0f, 0, 0, 0);
			// }
			// }
			// if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1315 && Main.CanStartInvasion(3, true))
			// {
			// this.itemTime = item.useTime;
			// SoundEngine.PlaySound(SoundID.Roar, this.Center);
			// if (FargoSoulsUtil.HostCheck)
			// {
			// if (Main.invasionType == 0)
			// {
			// Main.invasionDelay = 0;
			// Main.StartInvasion(3);
			// }
			// }
			// else
			// {
			// NetMessage.SendData(61, -1, -1, "", this.whoAmI, -3f, 0f, 0f, 0, 0, 0);
			// }
			// }
			// if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1844 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon && !DD2Event.Ongoing)
			// {
			// this.itemTime = item.useTime;
			// SoundEngine.PlaySound(SoundID.Roar, this.Center);
			// if (FargoSoulsUtil.HostCheck)
			// {
			// Main.NewText(Lang.misc[31], 50, 255, 130, false);
			// Main.startPumpkinMoon();
			// }
			// else
			// {
			// NetMessage.SendData(61, -1, -1, "", this.whoAmI, -4f, 0f, 0f, 0, 0, 0);
			// }
			// }

			// if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 3601 && NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
			// {
			// SoundEngine.PlaySound(SoundID.Roar, this.Center);
			// this.itemTime = item.useTime;
			// if (Main.netMode == NetmodeID.SinglePlayer)
			// {
			// WorldGen.StartImpendingDoom();
			// }
			// else
			// {
			// NetMessage.SendData(61, -1, -1, "", this.whoAmI, -8f, 0f, 0f, 0, 0, 0);
			// }
			// }
			// if (this.itemTime == 0 && this.itemAnimation > 0 && item.type == 1958 && !Main.dayTime && !Main.pumpkinMoon && !Main.snowMoon && !DD2Event.Ongoing)
			// {
			// this.itemTime = item.useTime;
			// SoundEngine.PlaySound(SoundID.Roar, this.Center);
			// if (FargoSoulsUtil.HostCheck)
			// {
			// Main.NewText(Lang.misc[34], 50, 255, 130, false);
			// Main.startSnowMoon();
			// }
			// else
			// {
			// NetMessage.SendData(61, -1, -1, "", this.whoAmI, -5f, 0f, 0f, 0, 0, 0);
			// }
			// }

			#endregion
		}
	}
}
