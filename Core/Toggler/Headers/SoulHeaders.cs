using FargowiltasSouls.Content.Items.Accessories.Souls;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Toggler.Content
{
    public abstract class SoulHeader : Header
    {
        public override float Priority => 2;
        public override string SortCategory => "Souls";
    }
    public class UniverseHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<UniverseSoul>();
        public override float Priority => 2.1f;
    }
    public class WorldShaperHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<WorldShaperSoul>();
        public override float Priority => 2.2f;
    }
    public class ColossusHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<ColossusSoul>();
        public override float Priority => 2.3f;
    }
    public class FlightMasteryHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<FlightMasterySoul>();
        public override float Priority => 2.4f;
    }
    public class SupersonicHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<SupersonicSoul>();
        public override float Priority => 2.5f;
    }
    public class TrawlerHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<TrawlerSoul>();
        public override float Priority => 2.6f;
    }
    public class EternityHeader : SoulHeader
    {
        public override int Item => ModContent.ItemType<EternitySoul>();
        public override float Priority => 2.7f;
    }
    /*
    public class SoulToggles : ToggleCollection
    {
        public override string Mod => "FargowiltasSouls";
        public override string SortCategory => "Souls";
        public override float Priority => 1;
        public override bool Active => true;

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
        public string ShimmerImmunity;

        public int FlightMasteryHeader = ModContent.ItemType<FlightMasterySoul>();
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
    */
}
