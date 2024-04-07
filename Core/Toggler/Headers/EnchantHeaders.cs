using FargowiltasSouls.Content.Items.Accessories.Forces;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Toggler.Content
{
    public abstract class EnchantHeader : Header
    {
        public override float Priority => 0;
        public override string SortCategory => "Enchantments";
    }
    public class TimberHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<TimberForce>();
        public override float Priority => 0.1f;
    }
    public class TerraHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<TerraForce>();
        public override float Priority => 0.2f;
    }
    public class EarthHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<EarthForce>();
        public override float Priority => 0.3f;
    }
    public class NatureHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<NatureForce>();
        public override float Priority => 0.4f;
    }
    public class LifeHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<LifeForce>();
        public override float Priority => 0.5f;
    }
    public class ShadowHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<ShadowForce>();
        public override float Priority => 0.6f;
    }
    public class SpiritHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<SpiritForce>();
        public override float Priority => 0.7f;
    }
    public class WillHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<WillForce>();
        public override float Priority => 0.8f;
    }
    public class CosmoHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<CosmoForce>();
        public override float Priority => 0.9f;
    }
    /*
    public class EnchantToggles : ToggleCollection
    {
        public override string Mod => "FargowiltasSouls";
        public override string SortCategory => "Enchantments";
        public override float Priority => 0;
        public override bool Active => true;

        public int WoodHeader = ModContent.ItemType<TimberForce>();
        public string Boreal;
        public string Ebon;
        public string Shade;
        public string Mahogany;
        public string Palm;
        public string Pearl;

        public int EarthHeader = ModContent.ItemType<EarthForce>();
        public string Adamantite;
        public string Cobalt;
        public string AncientCobalt;
        public string Mythril;
        public string Orichalcum;
        public string Palladium;
        public string PalladiumOrb;
        public string Titanium;

        public int TerraHeader = ModContent.ItemType<TerraForce>();
        public string Copper;
        public string IronM;
        //public string CthulhuShield;
        public string Tin;
        public string SilverS;
        public string Tungsten;
        public string TungstenProj;
        public string Obsidian;
        public string AshWood;

        public int WillHeader = ModContent.ItemType<WillForce>();
        public string Gladiator;
        public string GladiatorBanner;
        public string Gold;
        public string GoldToPiggy;
        public string Huntress;
        public string RedRidingRain;
        public string Valhalla;
        public string SquireMountSpeed;
        public string SquireMountJump;
        public string SquirePanic;

        public int LifeHeader = ModContent.ItemType<LifeForce>();
        public string Bee;
        public string Beetle;
        public string Cactus;
        public string Pumpkin;
        public string Spider;
        public string Turtle;

        public int NatureHeader = ModContent.ItemType<NatureForce>();
        public string Chlorophyte;
        public string Crimson;
        public string Frost;
        public string Jungle;
        public string JungleDash;
        public string Molten;
        public string Rain;
        public string RainInnerTube;
        public string Shroomite;
        public string ShroomiteShroom;

        public int ShadowHeader = ModContent.ItemType<ShadowForce>();
        public string DarkArt;
        public string Apprentice;
        public string Necro;
        public string NecroGlove;
        public string AncientShadow;
        public string Shadow;
        public string Monk;
        public string Shinobi;
        public string ShinobiDash;
        public string Spooky;
        public string NinjaSpeed;
        public string CrystalDash;

        public int SpiritHeader = ModContent.ItemType<SpiritForce>();
        public string Fossil;
        public string Forbidden;
        public string HallowDodge;
        public string AHallowed;

        public string Spectre;
        public string Tiki;

        public int CosmoHeader = ModContent.ItemType<CosmoForce>();
        public string MeteorMomentum;
        public string Meteor;
        public string Nebula;
        public string Solar;
        public string SolarFlare;
        public string Stardust;
        public string VortexS;
        public string VortexV;
    }
    */
}
