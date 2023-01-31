using Microsoft.Xna.Framework;
using SteelSeries.GameSense;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

        public const int CustomPresetCount = 3;
        public List<string>[] CustomPresets = new List<string>[CustomPresetCount];

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

            //TODO: figure out how to extract a plain list from json, only using Dict rn because i know it can be loaded from json
            for (int i = 0; i < CustomPresets.Length; i++)
            {
                var toggleUnpack = Config.Get<Dictionary<string, bool>>($"CustomPresetsOff{i + 1}", null);
                if (toggleUnpack != null)
                    CustomPresets[i] = toggleUnpack.Keys.ToList();
            }

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

                for (int i = 0; i < CustomPresets.Length; i++)
                {
                    if (CustomPresets[i] == null)
                        continue;

                    Dictionary<string, bool> togglesOff = new Dictionary<string, bool>(CustomPresets.Length);
                    foreach (string toggle in CustomPresets[i])
                        togglesOff[toggle] = false;
                    Config.Put($"CustomPresetsOff{i + 1}", togglesOff);
                }

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

            player.SetToggleValue("Eternity", true);

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

        public void SaveCustomPreset(int slot)
        {
            var togglesOff = new List<string>();
            foreach (KeyValuePair<string, Toggle> entry in Toggles)
            {
                if (!Toggles[entry.Key].ToggleBool)
                    togglesOff.Add(entry.Key);
            }

            if (!Main.dedServ)
            {
                CustomPresets[slot - 1] = togglesOff;
                //Save();
                Main.NewText($"Toggles saved to custom set {slot}!", Color.Yellow);
            }
        }

        public void LoadCustomPreset(int slot)
        {
            List<string> togglesOff = CustomPresets[slot - 1];
            if (togglesOff == null)
            {
                Main.NewText($"No toggles found in custom set {slot}.", Color.Yellow);
                return;
            }

            FargoSoulsPlayer modPlayer = Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.disabledToggles = new List<string>(togglesOff);

            LoadPlayerToggles(modPlayer);
            modPlayer.disabledToggles.Clear();
        }
    }
}
