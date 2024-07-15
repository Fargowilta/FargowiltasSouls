using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Common.Utilities
{
    public static class MathUtils
    {
        public static float HyperbolicScaling(float max, float scaleFactor)
        {
            return max * ((0.15f * scaleFactor) / (0.15f * scaleFactor + 1));
        }
    }
}
