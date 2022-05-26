using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.IO;

namespace FargowiltasSouls.Toggler
{
    public class ToggleBackend
    {
        public static string ConfigPath = Path.Combine(Main.SavePath, "ModConfigs", "FargowiltasSouls_Toggles.json");
        public Preferences Config;

        public Dictionary<string, Toggle> Toggles;
        public Point TogglerPosition;
        public bool CanPlayMaso;

        public bool Initialized;

        public void LoadInMenu()
        {
            if (Initialized)
                return;

            //Main.NewText("OOBA");
            Config = new Preferences(ConfigPath);

            Toggles = ToggleLoader.LoadedToggles;
            TogglerPosition = new Point(0, 0);

            if (!Main.dedServ)
            {
                if (!Config.Load())
                    Save();
            }

            Dictionary<string, int> togglerPositionUnpack = Config.Get("TogglerPosition", new Dictionary<string, int>() { { "X", Main.screenWidth / 2 - 300 }, { "Y", Main.screenHeight / 2 - 200 } });
            TogglerPosition = new Point(togglerPositionUnpack["X"], togglerPositionUnpack["Y"]);

            if (!Main.dedServ)
                FargowiltasSouls.UserInterfaceManager.SoulToggler.SetPositionToPoint(TogglerPosition);

            CanPlayMaso = Config.Get("CanPlayMaso", false);

            Initialized = true;
        }

        public void Save()
        {
            if (!Main.dedServ)
            {
                Config.Put("CanPlayMaso", CanPlayMaso);
                //Config.Put(TogglesByPlayer, ParsePackedToggles());

                TogglerPosition = FargowiltasSouls.UserInterfaceManager.SoulToggler.GetPositionAsPoint();
                Config.Put("TogglerPosition", UnpackPosition());
                Config.Save();
            }
        }

        public void LoadPlayerToggles(FargoSoulsPlayer modPlayer)
        {
            Toggles = ToggleLoader.LoadedToggles;
            SetAll(true);

            foreach (string entry in modPlayer.disabledToggles)
                Main.LocalPlayer.SetToggleValue(entry, false);

            foreach (KeyValuePair<string, Toggle> entry in Toggles)
                modPlayer.TogglesToSync[entry.Key] = entry.Value.ToggleBool;
        }

        public Dictionary<string, int> UnpackPosition() => new Dictionary<string, int>() {
            { "X", TogglerPosition.X },
            { "Y", TogglerPosition.Y }
        };

        public void SetAll(bool value)
        {
            foreach (Toggle toggle in Toggles.Values)
            {
                Main.LocalPlayer.SetToggleValue(toggle.InternalName, value);
            }
        }

        public void MinimalEffects()
        {
            Player player = Main.LocalPlayer;

            SetAll(false);
            player.SetToggleValue("Mythril", true);
            player.SetToggleValue("Palladium", true);
            player.SetToggleValue("IronM", true);
            player.SetToggleValue("CthulhuShield", true);
            //player.SetToggleValue("Tin", true);
            player.SetToggleValue("Beetle", true);
            player.SetToggleValue("Spider", true);
            player.SetToggleValue("GoldToPiggy", true);
            player.SetToggleValue("JungleDash", true);
            player.SetToggleValue("SupersonicTabi", true);
            player.SetToggleValue("Valhalla", true);
            player.SetToggleValue("Nebula", true);
            player.SetToggleValue("Solar", true);
            player.SetToggleValue("Mythril", true);
            player.SetToggleValue("Huntress", true);
            player.SetToggleValue("CrystalDash", true);

            player.SetToggleValue("DeerSinewDash", true);
            player.SetToggleValue("MasoGraze", true);
            //player.SetToggleValue("MasoGrazeRing", true);
            player.SetToggleValue("MasoIconDrops", true);
            player.SetToggleValue("MasoNymph", true);
            player.SetToggleValue("TribalCharm", true);
            player.SetToggleValue("MasoGrav2", true);
            player.SetToggleValue("PrecisionSealHurtbox", true);

            player.SetToggleValue("YoyoBag", true);
            player.SetToggleValue("MiningHunt", true);
            player.SetToggleValue("MiningDanger", true);
            player.SetToggleValue("MiningSpelunk", true);
            player.SetToggleValue("MiningShine", true);
            player.SetToggleValue("Trawler", true);
            player.SetToggleValue("RunSpeed", true);
            player.SetToggleValue("SupersonicRocketBoots", true);
            player.SetToggleValue("Momentum", true);
            player.SetToggleValue("FlightMasteryInsignia", true);
            player.SetToggleValue("FlightMasteryGravity", true);
            player.SetToggleValue("Universe", true);
            player.SetToggleValue("DefensePaladin", true);
            player.SetToggleValue("MasoAeolus", true);
            player.SetToggleValue("MasoConcoction", true);
            player.SetToggleValue("ManaFlower", true);
        }
    }
}
