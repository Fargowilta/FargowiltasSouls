using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.IO;

namespace FargowiltasSouls.Toggler
{
    public class ToggleBackend
    {
        public static string ConfigPath = Path.Combine(Main.SavePath, "ModConfigs", "FargowiltasSouls_Toggles.json");
        public Preferences Config;

        public Dictionary<string, Toggle> Toggles;
        //public Point TogglerPosition;
        public bool CanPlayMaso;

        public bool Initialized;

        public void LoadInMenu()
        {
            if (Initialized)
                return;

            //Main.NewText("OOBA");
            Config = new Preferences(ConfigPath);

            Toggles = ToggleLoader.LoadedToggles;
            //TogglerPosition = new Point(0, 0);

            if (!Main.dedServ)
            {
                if (!Config.Load())
                    Save();
            }

            //Dictionary<string, int> togglerPositionUnpack = Config.Get("TogglerPosition", new Dictionary<string, int>() { { "X", Main.screenWidth / 2 - 300 }, { "Y", Main.screenHeight / 2 - 200 } });
            //TogglerPosition = new Point(togglerPositionUnpack["X"], togglerPositionUnpack["Y"]);

            //if (!Main.dedServ)
            //    FargowiltasSouls.UserInterfaceManager.SoulToggler.SetPositionToPoint(TogglerPosition);

            CanPlayMaso = Config.Get("CanPlayMaso", false);

            Initialized = true;
        }

        public void Save()
        {
            if (!Main.dedServ)
            {
                Config.Put("CanPlayMaso", CanPlayMaso);
                //Config.Put(TogglesByPlayer, ParsePackedToggles());

                //TogglerPosition = FargowiltasSouls.UserInterfaceManager.SoulToggler.GetPositionAsPoint();
                //Config.Put("TogglerPosition", UnpackPosition());
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

        /*public Dictionary<string, int> UnpackPosition() => new Dictionary<string, int>() {
            { "X", TogglerPosition.X },
            { "Y", TogglerPosition.Y }
        };*/

        public void SetAll(bool value)
        {
            foreach (Toggle toggle in Toggles.Values)
            {
                Main.LocalPlayer.SetToggleValue(toggle.InternalName, value);
            }
        }

        public void SomeEffects()
        {
            Player player = Main.LocalPlayer;

            SetAll(true);

            player.SetToggleValue("Boreal", false);
            player.SetToggleValue("Ebon", false);
            player.SetToggleValue("Shade", false);
            player.SetToggleValue("ShadeOnHit", false);
            player.SetToggleValue("Pearl", false);

            player.SetToggleValue("Orichalcum", false);
            player.SetToggleValue("PalladiumOrb", false);

            player.SetToggleValue("CopperConfig", false);

            player.SetToggleValue("Gladiator", false);
            player.SetToggleValue("RedRidingRain", false);

            player.SetToggleValue("Pumpkin", false);
            player.SetToggleValue("Cactus", false);

            player.SetToggleValue("Jungle", false);
            player.SetToggleValue("Rain", false);
            player.SetToggleValue("Molten", false);
            player.SetToggleValue("ShroomiteShroom", false);

            player.SetToggleValue("DarkArt", false);
            player.SetToggleValue("Necro", false);
            player.SetToggleValue("Shadow", false);
            player.SetToggleValue("Spooky", false);

            player.SetToggleValue("Meteor", false);

            player.SetToggleValue("MasoDevianttHearts", false);

            player.SetToggleValue("MasoSlime", false);
            player.SetToggleValue("MasoSkele", false);

            player.SetToggleValue("MasoCarrot", false);
            player.SetToggleValue("MasoRainbow", false);
            player.SetToggleValue("MasoPouch", false);

            player.SetToggleValue("MasoLightning", false);

            player.SetToggleValue("MasoPungentCursor", false);
            player.SetToggleValue("MasoPugent", false);
            player.SetToggleValue("Deerclawps", false);

            player.SetToggleValue("MasoCultist", false);
            player.SetToggleValue("MasoBoulder", false);
            player.SetToggleValue("MasoCelest", false);
            player.SetToggleValue("MasoVision", false);

            player.SetToggleValue("MasoPump", false);
            player.SetToggleValue("IceQueensCrown", false);
            player.SetToggleValue("MasoUfo", false);
            player.SetToggleValue("MasoGrav", false);
            player.SetToggleValue("MasoTrueEye", false);

            player.SetToggleValue("MasoFishron", false);

            player.SetToggleValue("MasoAbom", false);
            player.SetToggleValue("MasoRing", false);

            player.SetToggleValue("MagmaStone", false);
            player.SetToggleValue("Sniper", false);

            player.SetToggleValue("Builder", false);

            player.SetToggleValue("DefenseStar", false);
            player.SetToggleValue("DefenseBee", false);

            player.SetToggleValue("SupersonicClimbing", false);
            player.SetToggleValue("Supersonic", false);

            player.SetToggleValue("TrawlerSpore", false);

            foreach (Toggle toggle in Toggles.Values.Where(toggle => toggle.InternalName.Contains("Pet")))
            {
                player.SetToggleValue(toggle.InternalName, false);
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
            player.SetToggleValue("MasoHealingPotion", true);
            player.SetToggleValue("TribalCharm", true);
            //player.SetToggleValue("TribalCharmClickBonus", true);
            player.SetToggleValue("MasoGrav2", true);
            player.SetToggleValue("PrecisionSealHurtbox", true);

            player.SetToggleValue("MasoEyeInstall", true);
            player.SetToggleValue("FusedLensInstall", true);

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
