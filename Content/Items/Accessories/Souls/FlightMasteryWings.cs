using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public abstract class FlightMasteryWings : BaseSoul
    {
        public abstract bool HasSupersonicSpeed { get; }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {

            player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
            ascentWhenFalling = 0.85f;
            if (player.HasEffect<FlightMasteryGravity>())
                ascentWhenFalling *= 1.5f;
            ascentWhenRising = 0.25f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 1.75f;
            constantAscend = 0.135f;
            if (player.controlUp)
            {
                ascentWhenFalling *= 6f;
                ascentWhenRising *= 6f;
                constantAscend *= 6f;
            }
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 18f;
            acceleration = 0.75f;

            if (HasSupersonicSpeed && player.HasEffect<SupersonicSpeedEffect>())
                speed = 25f;
        }
    }
    public class SupersonicSpeedEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<FlightMasteryHeader>();
        public override int ToggleItemType => ModContent.ItemType<FlightMasterySoul>();
    }
}
