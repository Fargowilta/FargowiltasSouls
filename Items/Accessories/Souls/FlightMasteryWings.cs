using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public abstract class FlightMasteryWings : BaseSoul
    {
        protected abstract bool HasSupersonicSpeed { get; }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            //if (player.GetToggleValue("FlightMasteryStarboard"))
            //{
            //    player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
            //    ascentWhenFalling = 0.95f;
            //    ascentWhenRising = 0.15f;
            //    maxCanAscendMultiplier = 1f;
            //    maxAscentMultiplier = 4.5f;
            //    constantAscend = 0.1f;
            //}
            //else
            //{
            player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
            ascentWhenFalling = 0.85f;
            if (player.GetToggleValue("FlightMasteryGravity", false))
                ascentWhenFalling *= 1.5f;
            ascentWhenRising = 0.25f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 1.75f;
            constantAscend = 0.135f;
            if (player.controlUp)
            {
                ascentWhenFalling *= 3f;
                ascentWhenRising *= 3f;
                constantAscend *= 3f;
            }
            //}
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            //if (player.GetToggleValue("FlightMasteryStarboard"))
            //{
            //    if (player.TryingToHoverDown)
            //    {
            //        speed = 16f;
            //        acceleration = 2.5984f;
            //    }
            //    else
            //    {
            //        speed = 8f;
            //        acceleration = 0.7308f;
            //    }
            //}
            //else
            //{
            speed = 18f;
            acceleration = 0.75f;
            //}

            if (HasSupersonicSpeed && player.GetToggleValue("Supersonic"))
                speed = 25f;
        }
    }
}
