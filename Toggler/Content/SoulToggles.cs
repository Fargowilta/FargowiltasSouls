using FargowiltasSouls.Items.Accessories.Souls;
using Terraria.ModLoader;

namespace FargowiltasSouls.Toggler.Content
{
    public class SoulToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Souls";
        public override int Priority => 1;

        public int UniverseHeader = ModContent.ItemType<UniverseSoul>();
        public string Melee;
        public string MagmaStone;
        public string YoyoBag;
        public string MoonCharm;
        public string NeptuneShell;
        public string Sniper;
        public string ManaFlower;
        public string Universe;

        public int WorldShaperHeader = ModContent.ItemType<WorldShaperSoul>();
        public string MiningHunt;
        public string MiningDanger;
        public string MiningSpelunk;
        public string MiningShine;
        public string Builder;

        public int ColossusHeader = ModContent.ItemType<ColossusSoul>();
        public string DefenseStar;
        public string DefenseBee;
        public string DefensePanic;
        public string DefenseFleshKnuckle;
        public string DefensePaladin;
        public string DefenseBrain;
        public string DefenseFrozen;

        public int FlightMasteryHeader = ModContent.ItemType<SupersonicSoul>();
        public string FlightMasteryInsignia;
        public string FlightMasteryGravity;

        public int SupersonicHeader = ModContent.ItemType<SupersonicSoul>();
        public string RunSpeed;
        public string Momentum;
        public string Supersonic;
        // Supersonic speed multiplier
        public string SupersonicJumps;
        public string SupersonicRocketBoots;
        public string SupersonicCarpet;
        public string SupersonicFlower;
        public string SupersonicTabi;
        public string SupersonicClimbing;
        public string CthulhuShield;
        public string BlackBelt;

        public int TrawlerHeader = ModContent.ItemType<TrawlerSoul>();
        public string Trawler;
        public string TrawlerJump;
        public string TrawlerGel;
        public string TrawlerSpore;

        public int EternityHeader = ModContent.ItemType<EternitySoul>();
        public string Eternity;
    }
}
